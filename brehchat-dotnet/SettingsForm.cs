using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace brehchat_dotnet
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            serverbox.Text = Config.Host;
            usernamebox.Text = Config.Token;
            targetbox.Text = Config.Target;
            xbox.Value = Config.x;
            ybox.Value = Config.y;
            wbox.Value = Config.w;
            hbox.Value = Config.h;
            screenbox.Items.Clear();
            for (int i = 1; i <= Screen.AllScreens.Length; ++i)
            {
                screenbox.Items.Add(i);
            }
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
            Config.Write();
            Hide();
        }

        private void testserverbutt_Click(object sender, EventArgs e)
        {

        }
    }
}
