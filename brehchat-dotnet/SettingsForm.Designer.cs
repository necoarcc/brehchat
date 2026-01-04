namespace brehchat_dotnet
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Label label1;
            Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            serverbox = new TextBox();
            testserverbutt = new Button();
            usernamebox = new TextBox();
            usernamebutt = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // serverbox
            // 
            serverbox.AllowDrop = true;
            serverbox.Cursor = Cursors.IBeam;
            serverbox.Location = new Point(12, 31);
            serverbox.Name = "serverbox";
            serverbox.Size = new Size(254, 23);
            serverbox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.White;
            label1.Location = new Point(12, 13);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 1;
            label1.Text = "Server";
            // 
            // testserverbutt
            // 
            testserverbutt.Location = new Point(272, 31);
            testserverbutt.Name = "testserverbutt";
            testserverbutt.Size = new Size(75, 23);
            testserverbutt.TabIndex = 2;
            testserverbutt.Text = "Test";
            testserverbutt.UseVisualStyleBackColor = true;
            // 
            // usernamebox
            // 
            usernamebox.Location = new Point(12, 86);
            usernamebox.Name = "usernamebox";
            usernamebox.Size = new Size(254, 23);
            usernamebox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(12, 68);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 4;
            label2.Text = "Username";
            // 
            // usernamebutt
            // 
            usernamebutt.Location = new Point(272, 85);
            usernamebutt.Name = "usernamebutt";
            usernamebutt.Size = new Size(75, 23);
            usernamebutt.TabIndex = 5;
            usernamebutt.Text = "Taken?";
            usernamebutt.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(349, 332);
            Controls.Add(usernamebutt);
            Controls.Add(label2);
            Controls.Add(usernamebox);
            Controls.Add(testserverbutt);
            Controls.Add(label1);
            Controls.Add(serverbox);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox serverbox;
        private Label label1;
        private Button testserverbutt;
        private TextBox usernamebox;
        private Label label2;
        private Button usernamebutt;
    }
}