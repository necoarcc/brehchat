namespace brehchat_server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<string> prefixes = [.. args];
            try
            {
                if(File.Exists(Path.Combine(Environment.CurrentDirectory, "prefixes.cfg")))
                {
                    using StreamReader reader = new(Path.Combine(Environment.CurrentDirectory, "prefixes.cfg"));
                    string? read = null;
                    while ((read = reader.ReadLine()) != null)
                        prefixes.Add(read);
                }
            }
            catch {}
            if (prefixes.Count == 0)
                prefixes.Add("http://localhost:3062");
            Server serv = new();
            await serv.Run(prefixes);
        }
    }
}
