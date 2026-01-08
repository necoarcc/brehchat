using brehchat_messages;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;

namespace brehchat_dotnet
{
    internal static class Network
    {
        public static bool Connected
        {
            get
            {
                return client.State == WebSocketState.Open;
            }
        }
        private static UriBuilder Host
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Config.Host))
                    return new();
                UriBuilder builder = new(Config.Host);
                if (string.IsNullOrWhiteSpace(builder.Scheme)
                    || !(builder.Scheme.Equals("ws") || builder.Scheme.Equals("wss")))
                    builder.Scheme = "wss";
                if (!builder.Uri.IsLoopback && !builder.Scheme.Equals("wss"))
                    return new();
                builder.Path = "";
                return builder;
            }
        }

        public readonly struct Msg(string user, string body)
        {
            public readonly string username = user;
            public readonly string body = body;
        }

        private readonly static HttpClient clientPing = new();
        private readonly static SemaphoreSlim mut = new(1, 1);
        private static TaskCompletionSource? pingwaiter = null;
        private static ClientWebSocket client = new();
        private static CancellationTokenSource TokenSource = new();
        private static Channel<Msg>? messages = null;

        static public void SetChannel(Channel<Msg> c) => messages = c;

        static public async Task<bool> Ping()
        {
            if (string.IsNullOrWhiteSpace(Host.Host))
                return false;

            if (!Connected)
            {
                try
                {
                    var req = Host;
                    req.Path = "/brehchatping";
                    var res = await clientPing.GetAsync(req.ToString());
                    return res.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                await mut.WaitAsync();
                var msg = new brehchat_messages.Message(MessageType.Ping, []);
                var bytes = Encoding.UTF8.GetBytes(msg.EncodeMsg());
                try
                {
                    await client.SendAsync(bytes, WebSocketMessageType.Text,
                        true, TokenSource.Token);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    mut.Release();
                }
                pingwaiter = new();
                var completed = await Task.WhenAny(pingwaiter.Task, Task.Delay(1500, TokenSource.Token));

                if (completed != pingwaiter.Task)
                {
                    await TokenSource.CancelAsync();
                    return false;
                }
                return true;
            }
        }

        static public async Task<bool> SendMessage(string text)
        {
            if (!Connected)
                return false;

            await mut.WaitAsync();
            try
            {
                var msg = new brehchat_messages.Message(MessageType.SendMessage, [text]);
                var bytes = Encoding.UTF8.GetBytes(msg.EncodeMsg());
                if (bytes.Length > 4096)
                    Array.Resize(ref bytes, 4096);
                await client.SendAsync(bytes, WebSocketMessageType.Text,
                    true, TokenSource.Token);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                mut.Release();
            }
        }

        static public async Task<bool> Connect()
        {
            if (string.IsNullOrWhiteSpace(Host.Host))
                return false;

            if (Connected)
                return true;

            client = new();
            TokenSource = new();

            try
            {
                client.Options.AddSubProtocol("brehchat");
                client.Options.SetRequestHeader("Token", Config.Token);
                await client.ConnectAsync(Host.Uri, TokenSource.Token);
                _ = Task.Run(Connection);
                return true;
            }
            catch
            {
                client.Dispose();
                return false;
            }
        }

        static private async Task Connection()
        {
            try
            {
                byte[] buf = new byte[4096];
                using MemoryStream stream = new();
                while (client.State == WebSocketState.Open && !TokenSource.IsCancellationRequested)
                {
                    var res = await client.ReceiveAsync(buf, TokenSource.Token);

                    byte[] bytes;
                    if (!res.EndOfMessage)
                    {
                        await stream.WriteAsync(buf, 0, res.Count, TokenSource.Token);
                        continue;
                    }
                    else
                    {
                        if (stream.Length > 0)
                        {
                            await stream.WriteAsync(buf, 0, res.Count, TokenSource.Token);
                            bytes = stream.ToArray();
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.SetLength(0);
                        }
                        else
                        {
                            bytes = buf;
                            Array.Resize(ref bytes, res.Count);
                        }
                    }

                    if (res.MessageType == WebSocketMessageType.Close)
                    {
                        await mut.WaitAsync();
                        try
                        {
                            await client.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                "closing", TokenSource.Token);
                        }
                        finally
                        {
                            mut.Release();
                        }
                        break;
                    }
                    if (res.MessageType == WebSocketMessageType.Binary)
                    {
                        await mut.WaitAsync();
                        try
                        {
                            await client.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                            "a binary message is never to be sent",
                            TokenSource.Token);
                        }
                        finally
                        {
                            mut.Release();
                        }
                        break;
                    }
                    if (bytes.Length > 4096)
                    {
                        await mut.WaitAsync();
                        try
                        {
                            await client.CloseAsync(WebSocketCloseStatus.MessageTooBig,
                            "message was too big",
                            TokenSource.Token);
                        }
                        finally
                        {
                            mut.Release();
                        }
                        break;
                    }
                    brehchat_messages.Message? msg = brehchat_messages.Message.DecodeMsg(Encoding.UTF8.GetString(bytes));
                    if (msg == null || msg.Value.Type == MessageType.Invalid)
                    {
                        await mut.WaitAsync();
                        try
                        {
                            await client.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                            "message invalid",
                            TokenSource.Token);
                        }
                        finally
                        {
                            mut.Release();
                        }
                        break;
                    }
                    brehchat_messages.Message actual = msg.Value;
                    switch (actual.Type)
                    {
                        case MessageType.SendMessage:
                            if (messages == null)
                                continue;
                            if (actual.Body.Length < 2)
                            {
                                await mut.WaitAsync();
                                try
                                {
                                    await client.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                                        "malformed message info",
                                        TokenSource.Token);
                                }
                                finally
                                {
                                    mut.Release();
                                }
                                break;
                            }
                            Msg cont = new(actual.Body[0], string.Join(Environment.NewLine, actual.Body.Skip(1)));
                            await messages.Writer.WriteAsync(cont);
                            break;
                        case MessageType.Ping:
                            pingwaiter?.SetResult();
                            break;
                    }
                }
            }
            catch
            {

            }
            finally
            {
                await TokenSource.CancelAsync();
                client.Abort();
                client.Dispose();
            }
        }

        static public async Task Disconnect()
        {
            await TokenSource.CancelAsync();
        }
    }
}

