using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.System.Threading;
using Windows.Win32.Foundation;
using Windows.Win32;
using System.Diagnostics;

namespace brehchat_dotnet
{
    public partial class Chat : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                var par = base.CreateParams;
                par.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_TRANSPARENT | (int)WINDOW_EX_STYLE.WS_EX_TOPMOST | (int)WINDOW_EX_STYLE.WS_EX_NOACTIVATE;

                return par;
            }
        }

        private readonly System.Windows.Forms.Timer timer = new();
        private readonly SettingsForm settings = new();

        private void StealFocus()
        {
            HWND handle = (HWND)Handle;
            PInvoke.SetForegroundWindow(handle);
            if (PInvoke.GetForegroundWindow() == handle)
                return;
            var fore = PInvoke.GetWindowThreadProcessId(PInvoke.GetForegroundWindow());
            var curr = PInvoke.GetCurrentThreadId();
            PInvoke.AttachThreadInput(curr, fore, true);
            PInvoke.SetForegroundWindow(handle);
            PInvoke.AttachThreadInput(curr, fore, false);
            if (PInvoke.GetForegroundWindow() == handle)
                return;
            PInvoke.AllocConsole();
            PInvoke.SetForegroundWindow(handle);
            PInvoke.FreeConsole();
        }

        public Chat()
        {
            InitializeComponent();
            Config.Read();
            StartPosition = FormStartPosition.Manual;
            Width = Config.w;
            Height = Config.h;
            Location = new(Config.x, Config.y);
            timer.Interval = 20;
            timer.Tick += PollInput;
            timer.Tick += PollVisibility;
            timer.Start();
            Config.Overlay = this;
        }

        private void PollVisibility(object? sender, EventArgs e)
        {
            if (settings.Visible)
                return;

            HWND fore = PInvoke.GetForegroundWindow();
            PInvoke.GetWindowThreadProcessId(fore, out var pid);
            if (pid == Environment.ProcessId)
                return;
            try
            {
                var process = Process.GetProcessById((int)pid).MainModule?.FileName;
                if (Path.GetFileName(process).Equals(Config.Target))
                {
                    PInvoke.GetWindowRect(fore, out var rect);
                    var target = Screen.GetBounds((Rectangle)rect);
                    Left = target.Left + Config.x;
                    Top = target.Top + Config.y;
                    Show();
                }
                else
                    Hide();
            } catch
            {
                Hide();
                return;
            }
        }

        private bool _prev_press = false;
        private void PollInput(object? sender, EventArgs e)
        {
            if (!Visible)
                return;

            if (_prev_press)
            {
                if ((PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_OEM_1) & 0x8000) == 0)
                {
                    _prev_press = false;
                    StealFocus();
                    MessageBox.Show("You pressed semicolon or whatever!");
                }
                return;
            }

            if ((PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_OEM_1) & 0x8000) != 0 &&
                (PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_SHIFT) & 0x8000) == 0 &&
                (PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_MENU) & 0x8000) == 0 &&
                (PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_CONTROL) & 0x8000) == 0)
            {
                _prev_press = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.Show();
            Hide();
        }
    }
}
