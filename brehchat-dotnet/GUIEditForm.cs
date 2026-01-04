using Windows.Win32.UI.WindowsAndMessaging;

namespace brehchat_dotnet
{
    public partial class GUIEditForm : Form
    {
        private class ChildForm : Form
        {
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == 0x84 && (int)m.Result == 0x1)
                    m.Result = 0x2;
                if (m.Msg == 0x83)
                    m.Result = 0;
            }
        }

        public Screen? display;
        private readonly ChildForm child;

        public GUIEditForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            FormClosing += GUIEditForm_FormClosing;
            child = new()
            {
                MdiParent = this,
                StartPosition = FormStartPosition.Manual,
                ControlBox = false,
                ShowIcon = false,
                FormBorderStyle = FormBorderStyle.Sizable,
                FormBorderColor = Color.White,
                BackColor = Color.White,
            };
        }

        private void GUIEditForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Config.x = child.Bounds.Location.X;
            Config.y = child.Bounds.Location.Y;
            Config.w = child.Bounds.Width;
            Config.h = child.Bounds.Height;
            child.Dispose();
            Config.settings.Show();
        }

        private void GUIEditForm_Shown(object sender, EventArgs e)
        {
            if (display == null)
            {
                if (Screen.AllScreens.Length == 0)
                {
                    Close();
                    Config.settings.Show();
                    return;
                }
                display = Screen.AllScreens[0];
            }
            var rect = display.Bounds;
            Location = new(rect.Location.X, rect.Location.Y);
            Width = rect.Width;
            Height = rect.Height;
            child.Location = new(Config.x, Config.y);
            child.Width = Config.w;
            child.Height = Config.h;
            child.Show();
            foreach (var control in Controls)
            {
                if (control is not MdiClient)
                    continue;
                MdiClient client = (MdiClient)control;
                client.BackColor = Color.Black;
                client.ForeColor = Color.Black;
                client.Dock = DockStyle.None;
                client.Bounds = ClientRectangle;
                client.Height += 100;
                client.Width += 100;
                client.Left -= 15;
                client.Top -= 15;
            }
            Config.InSettings = true;
        }

        private void GUIEditForm_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'q')
            {
                Close();
            }
        }
    }
}
