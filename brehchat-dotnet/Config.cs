using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace brehchat_dotnet
{
    static internal class Config
    {
        static volatile public string Host = "wss://yourserverhere:port";
        static public string Target = "app.exe";
        static public int x = 0;
        static public int y = 0;
        static public int w = 800;
        static public int h = 600;
        static volatile public string Token = "yourtokenhere";
        static public int Opacity = 50;
        static public VIRTUAL_KEY Focuskey = VIRTUAL_KEY.VK_OEM_6;
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
                w = int.Parse(lines[0]);
                h = int.Parse(lines[1]);
                x = int.Parse(lines[2]);
                y = int.Parse(lines[3]);
                Target = lines[4];
                Host = lines[5];
                Token = lines[6];
                Opacity = int.Parse(lines[7]);
                Focuskey = Enum.Parse<VIRTUAL_KEY>(lines[8]);
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
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "chat.cfg"), $"{w}\n{h}\n{x}\n{y}\n{Target}\n{Host}\n{Token}\n{Opacity}\n{Enum.GetName(Focuskey)}");
            } catch
            {
                Debug.WriteLine("Failed to write config!");
            }
        }
    }
}
