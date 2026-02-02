namespace brehchat_dotnet
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            MessageBox.Show("This is a development build of Brehchat: stuff may be broken and some protections are disabled.");
#else
            var mutex = new Mutex(true, "brehchat", out var createdNew);

            if(!createdNew)
            {
                MessageBox.Show("Only one instance of Brehchat can be running at once! Can't see it? Look for the application's icon in your notification area, right click it, and select 'Settings' to configure which app it overlays over and its size.");
                return;
            }
#endif
            try
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new Chat());
            } 
            finally
            {
#if !DEBUG
            mutex.Dispose();
#endif
            }
        }
    }
}