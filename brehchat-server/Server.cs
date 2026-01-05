using brehchat_messages;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace brehchat_server
{
    internal sealed class Server
    {
        private struct User(WebSocket wsc, string user)
        {
            public WebSocket WebSocket { get; private set; } = wsc;
            public string UserName { get; private set; } = user;
            public CancellationTokenSource TokenSource { get; private set; } = new();
            public SemaphoreSlim mut = new(1, 1);
        }

        private readonly HttpListener listener;
        private readonly CancellationTokenSource tokenSource;
        private readonly List<User> users;
        private readonly SemaphoreSlim mut;

        public Server()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            listener = new();
            tokenSource = new();
            mut = new(1, 1);
            users = [];
        }

        public async Task Run()
        {
            try
            {
                listener.Prefixes.Add("http://localhost:3062/");
                listener.Start();

                var token = tokenSource.Token;

                while (!token.IsCancellationRequested && listener.IsListening)
                {
                    var contextTask = listener.GetContextAsync();
                    var completed = await Task.WhenAny(contextTask, Task.Delay(Timeout.Infinite, token));
                    if (completed == contextTask)
                    {
                        var context = await contextTask;
                        _ = Task.Run(async () => await HandleRequest(context));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                listener.Abort();
            }
            finally
            {
                Console.WriteLine("Shutting down...");
                await Task.WhenAny(StopConnections(), Task.Delay(10000));
                listener.Abort();
            }
        }

        private async Task StopConnections()
        {
            await mut.WaitAsync();
            try
            {
                foreach (var user in users)
                {
                    user.TokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                mut.Release();
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod.Equals("GET")
                    && (context.Request.Url?.LocalPath.Equals("/brehchatping") ?? false)
                    && !context.Request.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "plaintext";
                    context.Response.ContentEncoding = Encoding.UTF8;
                    var res = Encoding.UTF8.GetBytes("Pong!");
                    context.Response.ContentLength64 = res.Length;
                    context.Response.OutputStream.Write(res);
                    context.Response.Close();
                    return;
                }
                if (!context.Request.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    return;
                }
                var sock = await context.AcceptWebSocketAsync("breh.projects.brehchat");
                if (sock == null)
                    return;
                if (!sock.WebSocket.SubProtocol?.Equals("breh.projects.brehchat") ?? false)
                {
                    await sock.WebSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                        "client must speak brehchat",
                        new());
                    return;
                }

                User user = new(sock.WebSocket, "usernamehereplaceholderfornow");
                await mut.WaitAsync();
                try
                {
                    users.Add(user);
                }
                finally
                {
                    mut.Release();
                }
                await HandleUser(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task HandleUser(User user)
        {
            try
            {
                byte[] buf = new byte[4096];
                using MemoryStream stream = new();
                while (user.WebSocket.State == WebSocketState.Open && !user.TokenSource.IsCancellationRequested)
                {
                    var res = await user.WebSocket.ReceiveAsync(buf, user.TokenSource.Token);
                    if (!res.EndOfMessage)
                    {
                        await stream.WriteAsync(buf, 0, res.Count, user.TokenSource.Token);
                        continue;
                    }
                    var bytes = stream.ToArray();
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                    if (res.MessageType == WebSocketMessageType.Close)
                    {
                        await user.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                            "closing", user.TokenSource.Token);
                        break;
                    }
                    if (res.MessageType == WebSocketMessageType.Binary)
                    {
                        await user.WebSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                            "a binary message is never to be sent",
                            user.TokenSource.Token);
                        break;
                    }
                    if (bytes.Length > 4096)
                    {
                        await user.WebSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig,
                            "the message was too big",
                            user.TokenSource.Token);
                        break;
                    }

                    Message? msg = Message.DecodeMsg(Encoding.UTF8.GetString(bytes));
                    if (!msg.HasValue || msg.Value.Type == MessageType.Invalid)
                    {
                        await user.WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                            "an invalid message has been sent",
                            user.TokenSource.Token);
                        break;
                    }
                    Message actual = msg.Value;
                    switch (actual.Type)
                    {
                        case MessageType.Ping:
                            var response = new Message(MessageType.Ping, ["Pong!"]);
                            bytes = Encoding.UTF8.GetBytes(response.EncodeMsg());
                            await user.mut.WaitAsync();
                            try
                            {
                                await user.WebSocket.SendAsync(bytes,
                                    WebSocketMessageType.Text,
                                    true,
                                    user.TokenSource.Token);
                            } finally
                            {
                                user.mut.Release();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                await mut.WaitAsync();
                user.WebSocket.Abort();
                users.Remove(user);
                mut.Release();
            }
        }

        private async Task BroadcastToAll(byte[] what)
        {
            await mut.WaitAsync();
            try
            {
                foreach (var user in users)
                {
                    await user.mut.WaitAsync();
                    try
                    {
                        await user.WebSocket.SendAsync(what,
                            WebSocketMessageType.Text,
                            true,
                            user.TokenSource.Token);
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    } finally
                    {
                        user.mut.Release();
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            } finally
            {
                mut.Release();
            }
        }

        private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            tokenSource.Cancel();
            e.Cancel = true;
        }
    }
}
