using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace brehchat_dotnet
{
    static internal class Config
    {
        static volatile public string Host = "yourserverhere";
        static public string Target = "app.exe";
        static public int x = 0;
        static public int y = 0;
        static public int w = 800;
        static public int h = 600;
        static volatile public string Token = "yourtokenhere";
        static public Form? Overlay;
        static public readonly Form settings = new SettingsForm();
        static volatile public bool InSettings = false;
        static public bool FirstLaunch = false;

        static public void Read()
        {
            if(!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "firstrun")))
            {
                FirstLaunch = true;
                try
                {
                    File.Create(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "firstrun")).Dispose();
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"Failed to create firstrun! {ex}");
                }
            }

            try
            {
                var cfg = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "chat.cfg"));
                using StringReader reader = new(cfg);
                List<string> lines = [];
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
            } catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read config! {ex}");
                Config.Write();
                FirstLaunch = true;
                //Application.Exit();
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
