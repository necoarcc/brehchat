namespace brehchat_server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Server serv = new();
            await serv.Run();
        }
    }
}
