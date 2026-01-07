namespace brehchat_dotnet
{
    partial class Chat
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            TrayIcon = new NotifyIcon(components);
            menu = new ContextMenuStrip(components);
            settingsToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            textbox = new RichTextBox();
            chatcontainer = new RichTextBox();
            menu.SuspendLayout();
            SuspendLayout();
            // 
            // TrayIcon
            // 
            TrayIcon.ContextMenuStrip = menu;
            TrayIcon.Icon = (Icon)resources.GetObject("TrayIcon.Icon");
            TrayIcon.Text = "Brehchat";
            TrayIcon.Visible = true;
            // 
            // menu
            // 
            menu.AccessibleRole = AccessibleRole.None;
            menu.Items.AddRange(new ToolStripItem[] { settingsToolStripMenuItem, exitToolStripMenuItem });
            menu.Name = "menu";
            menu.Size = new Size(117, 48);
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(116, 22);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(116, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // textbox
            // 
            textbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textbox.BackColor = Color.Silver;
            textbox.DetectUrls = false;
            textbox.Font = new Font("Segoe UI Black", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            textbox.ForeColor = Color.Black;
            textbox.Location = new Point(16, 242);
            textbox.Multiline = false;
            textbox.Name = "textbox";
            textbox.Size = new Size(272, 46);
            textbox.TabIndex = 1;
            textbox.Text = "";
            textbox.KeyDown += textbox_KeyDown;
            // 
            // chatcontainer
            // 
            chatcontainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatcontainer.BackColor = Color.Black;
            chatcontainer.BorderStyle = BorderStyle.None;
            chatcontainer.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            chatcontainer.ForeColor = Color.White;
            chatcontainer.Location = new Point(16, 12);
            chatcontainer.Name = "chatcontainer";
            chatcontainer.ReadOnly = true;
            chatcontainer.ScrollBars = RichTextBoxScrollBars.None;
            chatcontainer.Size = new Size(272, 224);
            chatcontainer.TabIndex = 2;
            chatcontainer.Text = "hi";
            // 
            // Chat
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(300, 300);
            ControlBox = false;
            Controls.Add(chatcontainer);
            Controls.Add(textbox);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Chat";
            Opacity = 0.75D;
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Chat";
            TopMost = true;
            menu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon TrayIcon;
        private ContextMenuStrip menu;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private RichTextBox textbox;
        private RichTextBox chatcontainer;
    }
}
