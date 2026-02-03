using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace brehchat_dotnet
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            foreach (var key in Enum.GetNames<VIRTUAL_KEY>())
                focbox.Items.Add(key);
        }

        public void StealFocus()
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

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Config.InSettings = false;
            Hide();
        }

        private void SettingsForm_VisibleChanged(object? sender, EventArgs e)
        {
            if (sender is SettingsForm && !((SettingsForm)sender).Visible)
                return;
            serverbox.Text = Config.Host;
            usernamebox.Text = Config.Token;
            targetbox.Text = Config.Target;
            xbox.Value = Config.x;
            ybox.Value = Config.y;
            wbox.Value = Config.w;
            hbox.Value = Config.h;
            opacbox.Value = Config.Opacity;
            focbox.SelectedItem = Config.Focuskey.ToString();
            screenbox.Items.Clear();
            for (int i = 1; i <= Screen.AllScreens.Length; ++i)
            {
                screenbox.Items.Add(i);
            }
            Config.InSettings = true;
        }

        private void cancelbutt_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void okbutt_Click(object sender, EventArgs e)
        {
            Config.Host = serverbox.Text;
            Config.Target = targetbox.Text;
            Config.Token = usernamebox.Text;
            Config.x = (int)xbox.Value;
            Config.y = (int)ybox.Value;
            Config.w = (int)wbox.Value;
            Config.h = (int)hbox.Value;
            Config.Opacity = (int)opacbox.Value;
            if (Enum.TryParse<VIRTUAL_KEY>(focbox.SelectedItem?.ToString(), true, out var foc))
                Config.Focuskey = foc;
            Config.Write();
            Hide();
        }

        private async void testserverbutt_Click(object sender, EventArgs e)
        {
            var original = Config.Host;
            Config.Host = serverbox.Text;
            if (!await Network.Ping())
            {
                MessageBox.Show("The server is not responding!");
            }
            else
            {
                MessageBox.Show("The server is responding!");
            }
            Config.Host = original;
        }

        private void guieditbutt_Click(object sender, EventArgs e)
        {
            if (screenbox.Items.Count == 0 || Screen.AllScreens.Length == 0)
                return;
            if (string.IsNullOrWhiteSpace(screenbox.Text))
            {
                MessageBox.Show("Please select a screen to show the setting on!");
                return;
            }
            var guieditform = new GUIEditForm();
            try
            {
                if (int.TryParse(screenbox.Text, out int res))
                    guieditform.display = Screen.AllScreens[res - 1];
                else
                    guieditform.display = null;
            }
            catch
            {
                guieditform.display = null;
            }
            MessageBox.Show("Q to exit!");
            guieditform.Show();
            Hide();
        }
    }
}
