using System;
using System.Collections.Generic;
using System.Text;

namespace brehchat_dotnet
{
    internal static class Network
    {
        public static bool Connected { get; private set; }

        static public bool Ping()
        {
            return false;
        }

        static public bool SendMessage()
        {
            return false;
        }

        static public bool Connect()
        {
            return false;
        }
    }
}
