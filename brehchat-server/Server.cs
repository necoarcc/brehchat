using brehchat_messages;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace brehchat_server
{
    internal sealed class Server
    {
        private readonly struct User(WebSocket wsc, string user, CancellationTokenSource cts)
        {
            public readonly WebSocket WebSocket  = wsc;
            public readonly string UserName  = user;
            public readonly CancellationTokenSource TokenSource  = cts;
            public readonly SemaphoreSlim mut = new(1, 1);
        }

        private readonly HttpListener listener;
        private readonly CancellationTokenSource tokenSource;
        private readonly List<User> users;
        private readonly SemaphoreSlim mut;
        private readonly List<List<string>> tokens;

        public Server()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            listener = new();
            tokenSource = new();
            mut = new(1, 1);
            users = [];
            using (StreamReader r = new("tokens.json"))
            {
                var read = r.ReadToEnd();
                tokens = JsonSerializer.Deserialize<List<List<string>>>(read) ?? throw new Exception("tokens.json is wrong");
            }
        }

        public async Task Run(List<string> prefixes)
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls([.. prefixes]);

            using WebApplication app = builder.Build();
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.Request.Method != HttpMethods.Get)
                {
                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return;
                }
                await next();
            });
            app.MapGet("/brehchatping", () => "Pong!");
            app.MapGet("/", Authenticate);
            Console.WriteLine($"Listening on: {string.Join(", ", prefixes)}");

            try
            {
                await app.RunAsync(tokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Shutting down...");
            }
            catch (Exception e)
            {
                Console.Write($"A critical error has ocurred:\n{e}");
            }
        }

        private async Task Authenticate(HttpContext ctx)
        {
            if(!ctx.WebSockets.IsWebSocketRequest)
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.CompleteAsync();
                return;
            }

            string? username = null;
            if(!ctx.Request.Headers.TryGetValue("Token", out var token))
            {
                ctx.Response.StatusCode = 403;
                await ctx.Response.CompleteAsync();
                return;
            }
            try
            {
                foreach(var pair in tokens)
                {
                    if (pair[0].Equals(token))
                    {
                        username = pair[1];
                        break;
                    }
                    
                }
                if (username == null)
                    throw new Exception();
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("[WARNING] user rejected because of malformed config");
                ctx.Response.StatusCode = 403;
                await ctx.Response.CompleteAsync();
                return;
            }
            catch
            {
                ctx.Response.StatusCode = 403;
                await ctx.Response.CompleteAsync();
                return;
            }

            User? user = null;
            try
            {
                WebSocket sock = await ctx.WebSockets.AcceptWebSocketAsync("brehchat");

                user = new User(sock, username, CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token));
                {

                };
                await mut.WaitAsync();
                try
                {
                    users.Add(user.Value);
                }
                finally
                {
                    mut.Release();
                }
            }
            catch
            {
                return;
            }
            await HandleUser(user.Value);
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
                    byte[] bytes;
                    if (!res.EndOfMessage)
                    {
                        await stream.WriteAsync(buf, 0, res.Count, user.TokenSource.Token);
                        continue;
                    }
                    else
                    {
                        if (stream.Length > 0)
                        {
                            await stream.WriteAsync(buf, 0, res.Count, user.TokenSource.Token);
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
                        await user.mut.WaitAsync();
                        try
                        {
                            await user.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                "closing", user.TokenSource.Token);
                        }
                        finally
                        {
                            user.mut.Release();
                        }
                        break;
                    }
                    if (res.MessageType == WebSocketMessageType.Binary)
                    {
                        await user.mut.WaitAsync();
                        try
                        {
                            await user.WebSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                                "a binary message is never to be senta binary message is never to be sent",
                                user.TokenSource.Token);
                        }
                        finally
                        {
                            user.mut.Release();
                        }
                        break;
                    }
                    if (bytes.Length > 4096)
                    {
                        await user.mut.WaitAsync();
                        try
                        {
                            await user.WebSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig,
                                "the message was too big",
                                user.TokenSource.Token);
                        }
                        finally
                        {
                            user.mut.Release();
                        }
                        break;
                    }

                    Message? msg = Message.DecodeMsg(Encoding.UTF8.GetString(bytes));
                    if (!msg.HasValue || msg.Value.Type == MessageType.Invalid)
                    {
                        await user.mut.WaitAsync();
                        try
                        {
                            await user.WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                                "an invalid message has been sent",
                                user.TokenSource.Token);
                        }
                        finally
                        {
                            user.mut.Release();
                        }
                        break;
                    }
                    Message actual = msg.Value;
                    switch (actual.Type)
                    {
                        case MessageType.SendMessage:
                            if (actual.Body.Length == 0)
                                continue;
                            var response = new Message(MessageType.SendMessage, [user.UserName, .. actual.Body]);
                            await BroadcastToAll(Encoding.UTF8.GetBytes(response.EncodeMsg()));
                            break;
                        case MessageType.Ping:
                            response = new Message(MessageType.Ping, ["Pong!"]);
                            bytes = Encoding.UTF8.GetBytes(response.EncodeMsg());
                            await user.mut.WaitAsync();
                            try
                            {
                                await user.WebSocket.SendAsync(bytes,
                                    WebSocketMessageType.Text,
                                    true,
                                    user.TokenSource.Token);
                            }
                            finally
                            {
                                user.mut.Release();
                            }
                            break;
                    }
                }
            }
            catch
            {
                //Console.WriteLine(ex);
            }
            finally
            {
                await user.TokenSource.CancelAsync();
                user.WebSocket.Abort();
                await mut.WaitAsync();
                users.Remove(user);
                mut.Release();
                Console.WriteLine($"User disconnected: {user.UserName}");
            }
        }

        private async Task BroadcastToAll(byte[] what)
        {
            await mut.WaitAsync();
            List<User> copy;
            try
            {
                copy = [.. users];
            }
            finally
            {
                mut.Release();
            }

            var tasks = copy.Select(async user =>
            {
                if (user.WebSocket.State != WebSocketState.Open || user.TokenSource.IsCancellationRequested)
                    return;
                await user.mut.WaitAsync();
                try
                {
                    await user.WebSocket.SendAsync(what,
                        WebSocketMessageType.Text,
                        true,
                        user.TokenSource.Token);
                }
                catch
                {
                    //Console.WriteLine(ex);
                }
                finally
                {
                    user.mut.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            tokenSource.Cancel();
            e.Cancel = true;
        }
    }
}
