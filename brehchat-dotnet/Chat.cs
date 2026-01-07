using System.Diagnostics;
using System.Threading.Channels;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

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
        private readonly System.Windows.Forms.Timer connector = new();
        private readonly CancellationTokenSource tokenSource = new();
        private readonly TaskCompletionSource completionSource = new();
        private readonly Channel<Network.Msg> channel = Channel.CreateUnbounded<Network.Msg>();
        private TaskCompletionSource connectorTask = new();


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
            Network.SetChannel(channel);
            StartPosition = FormStartPosition.Manual;
            Width = Config.w;
            Height = Config.h;
            Location = new(Config.x, Config.y);
            timer.Interval = 20;
            timer.Tick += PollInput;
            timer.Tick += PollVisibility;
            timer.Start();
            Config.Overlay = this;
            Application.ApplicationExit += OnExit;
            Config.settings.VisibleChanged += Settings_VisibleChanged;
            chatcontainer.Text = "Welcome to BrehChat!\n";
            connector.Interval = 15000;
            connector.Tick += async (object? a, EventArgs b) => { connectorTask.TrySetResult(); };
            connector.Start();
            _ = Task.Run(Heartbeat);
            _ = Task.Run(GetMessages);
            connectorTask.SetResult();
        }

        private async Task GetMessages()
        {
            try
            {
                await foreach (var msg in channel.Reader.ReadAllAsync(tokenSource.Token))
                {
                    _ = InvokeAsync(() =>
                    {
                        AppendToChat(msg);
                    });
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task Heartbeat()
        {
            try
            {
                var cancellation = Task.Delay(Timeout.Infinite, tokenSource.Token);
                bool dismissed = false;
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    var ticker = connectorTask.Task;
                    var completed = await Task.WhenAny(ticker, cancellation);
                    if (ticker == completed)
                    {
                        connectorTask = new();
                        if (!Config.InSettings && !await Network.Ping())
                        {
                            await InvokeAsync(() =>
                               {
                                   if (!GetLatestChat().Equals(": Reconnecting..."))
                                       AppendToChat(new Network.Msg("", "Reconnecting..."));
                                   if (!dismissed && !Config.InSettings)
                                   {
                                       dismissed = true;
                                       MessageBox.Show("The chosen BrehChat server is not responding!");
                                   }
                               }, tokenSource.Token);
                            continue;
                        }
                        if (!Config.InSettings && !Network.Connected && !await Network.Connect())
                        {
                            await InvokeAsync(() =>
                            {
                                if (!GetLatestChat().Equals(": Reconnecting..."))
                                    AppendToChat(new Network.Msg("", "Reconnecting..."));
                                if (!dismissed && !Config.InSettings)
                                {
                                    dismissed = true;
                                    MessageBox.Show("The chosen BrehChat server has failed to authenticate you (is the token correct?)");
                                }
                            }, tokenSource.Token);
                            continue;
                        }
                        await InvokeAsync(() =>
                        {
                            if (GetLatestChat().Equals(": Reconnecting..."))
                                AppendToChat(new Network.Msg("", "Connected!"));
                        });
                    }
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                await InvokeAsync(() =>
                {
                    MessageBox.Show(ex.ToString());
                });
            }
            finally
            {
                await Network.Disconnect();
                completionSource.SetResult();
                if (!tokenSource.Token.IsCancellationRequested)
                    await InvokeAsync(Application.Exit);
            }
        }

        private void AppendToChat(Network.Msg what)
        {
            var lines = chatcontainer.Lines;
            if (lines.Length + 1 > 100)
            {
                chatcontainer.SelectionStart = 0;
                chatcontainer.SelectionLength = lines[0].Length;
                chatcontainer.SelectedText = "";
            }
            var current = chatcontainer.Text.Length;
            var userstring = what.username + ": ";
            chatcontainer.AppendText(userstring);
            chatcontainer.SelectionStart = current;
            chatcontainer.SelectionLength = userstring.Length;
            chatcontainer.SelectionColor = Color.BlueViolet;
            chatcontainer.AppendText(what.body + '\n');
            chatcontainer.SelectionStart = chatcontainer.Text.Length - what.body.Length - 1;
            chatcontainer.SelectionLength = what.body.Length;
            chatcontainer.SelectionColor = Color.White;
            chatcontainer.SelectionStart = chatcontainer.Text.Length;
            chatcontainer.ScrollToCaret();
        }

        private string GetLatestChat()
        {
            var lines = chatcontainer.Lines;
            if (lines.Length == 0)
                return "";
            return lines[lines.Length - 1];
        }

        private void Settings_VisibleChanged(object? sender, EventArgs e)
        {
            if (sender is SettingsForm && ((SettingsForm)sender).Visible == false)
            {
                Width = Config.w;
                Height = Config.h;
                Location = new(Config.x, Config.y);
                Config.InSettings = false;
            }
        }

        private async void OnExit(object? sender, EventArgs e)
        {
            Config.Write();
            timer.Dispose();
            connector.Dispose();
            tokenSource.Cancel();
            Config.settings.Dispose();
        }

        private void PollVisibility(object? sender, EventArgs e)
        {
            if (Config.InSettings)
            {
                Hide();
                return;
            }

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
            }
            catch
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
                    textbox.Focus();
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

        private async void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.settings.Dispose();
            await tokenSource.CancelAsync();
            await Task.WhenAny(completionSource.Task, Task.Delay(3500));
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Read();
            Config.settings.Show();
            Hide();
        }

        private async void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                if (!Network.Connected)
                    if (!await Network.Connect())
                    {
                        MessageBox.Show("The server is not responding.");
                        return;
                    }
                await Network.SendMessage(textbox.Text);
                textbox.Text = "";
            }
        }
    }
}
