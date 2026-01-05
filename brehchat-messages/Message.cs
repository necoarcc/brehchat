namespace brehchat_messages
{
    public readonly struct Message(MessageType type, string[] body)
    {
        public readonly MessageType Type = type;
        public readonly string[] Body = body;

        public static Message DecodeMsg(string msg)
        {
            List<string> all = [];
            using (StringReader reader = new(msg))
            {
                string? line = null;
                while ((line = reader.ReadLine()) != null)
                    all.Add(line);
            }
            
            int type;
            if (all.Count < 2 || !int.TryParse(all[0], out type) || !Enum.IsDefined((MessageType)type))
                return new(MessageType.Invalid, []);
            all.RemoveAt(0);

            return new((MessageType)type, [.. all]);
        }

        public string EncodeMsg()
        {
            using StringWriter writer = new();
            writer.WriteLine((int)Type);
            foreach(string line in Body)
            {
                writer.WriteLine(line);
            }
            return writer.ToString();
        }
    }
}
