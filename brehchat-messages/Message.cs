namespace brehchat_messages
{
    public readonly struct Message(MessageType type, string[] body)
    {
        public readonly MessageType Type = type;
        public readonly string[] Body = body;

        public static Message? DecodeMsg(string? msg)
        {
            if (msg == null)
                return null;

            List<string> all = [];
            using (StringReader reader = new(msg))
            {
                string? line = null;
                while ((line = reader.ReadLine()) != null)
                    all.Add(line);
            }

            if (all.Count < 1 || !int.TryParse(all[0], out int type) || !Enum.IsDefined((MessageType)type))
                return null;
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
