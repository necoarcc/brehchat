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
                Console.WriteLine("Shutting down...");
                await Task.WhenAny(WaitForConnections(), Task.Delay(10000));
                listener.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                listener.Abort();
            }
        }

        private async Task WaitForConnections()
        {
            await mut.WaitAsync();
            try
            {
                foreach (var user in users)
                {
                    await user.WebSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Going away", user.TokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            } finally
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
                if(!sock.WebSocket.SubProtocol?.Equals("breh.projects.brehchat") ?? false)
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
                } finally
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
            while(user.WebSocket.State == WebSocketState.Open && !user.TokenSource.IsCancellationRequested)
            {
                // ...
            }
        }

        private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            tokenSource.Cancel();
            e.Cancel = true;
        }
    }
}
