using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace brehchat_dotnet
{
    static internal class Config
    {
        static public string Host = "";
        static public string Target = "";
        static public int x;
        static public int y;
        static public int w;
        static public int h;
        static public string Token = "";
        static public Form? Overlay;
        static public readonly Form settings = new SettingsForm();
        static public bool InSettings = false;

        static public void Read()
        {
            try
            {
                var cfg = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "chat.cfg"));
                using StringReader reader = new(cfg);
                List<string> lines = new();
                string? line = "";
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
                if (lines.Count < 7)
                    throw new Exception();
                w = int.Parse(lines[0]);
                h = int.Parse(lines[1]);
                x = int.Parse(lines[2]);
                y = int.Parse(lines[3]);
                Target = lines[4];
                Host = lines[5];
                Token = lines[6];
            } catch
            {
                Debug.WriteLine("Failed to read config!");
                Application.Exit();
            }
        }

        static public void Write()
        {
            try
            {
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "chat.cfg"), $"{w}\n{h}\n{x}\n{y}\n{Target}\n{Host}\n{Token}");
            } catch
            {
                Debug.WriteLine("Failed to write config!");
            }
        }
    }
}
