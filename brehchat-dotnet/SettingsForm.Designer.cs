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
            Label label4;
            Label label3;
            Label label5;
            Label label6;
            Label label7;
            Label label8;
            Label label9;
            Label label10;
            Label label11;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            serverbox = new TextBox();
            testserverbutt = new Button();
            usernamebox = new TextBox();
            xbox = new NumericUpDown();
            ybox = new NumericUpDown();
            wbox = new NumericUpDown();
            hbox = new NumericUpDown();
            guieditbutt = new Button();
            screenbox = new ComboBox();
            okbutt = new Button();
            cancelbutt = new Button();
            targetbox = new TextBox();
            opacbox = new NumericUpDown();
            focbox = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label4 = new Label();
            label3 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            ((System.ComponentModel.ISupportInitialize)xbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ybox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)wbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)hbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)opacbox).BeginInit();
            SuspendLayout();
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
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(12, 68);
            label2.Name = "label2";
            label2.Size = new Size(137, 15);
            label2.TabIndex = 4;
            label2.Text = "Token (ask server owner)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.White;
            label4.Location = new Point(12, 124);
            label4.Name = "label4";
            label4.Size = new Size(91, 15);
            label4.TabIndex = 8;
            label4.Text = "Overlay settings";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(12, 149);
            label3.Name = "label3";
            label3.Size = new Size(14, 15);
            label3.TabIndex = 10;
            label3.Text = "X";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.White;
            label5.Location = new Point(91, 149);
            label5.Name = "label5";
            label5.Size = new Size(14, 15);
            label5.TabIndex = 12;
            label5.Text = "Y";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.White;
            label6.Location = new Point(12, 193);
            label6.Name = "label6";
            label6.Size = new Size(39, 15);
            label6.TabIndex = 14;
            label6.Text = "Width";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.White;
            label7.Location = new Point(90, 193);
            label7.Name = "label7";
            label7.Size = new Size(43, 15);
            label7.TabIndex = 16;
            label7.Text = "Height";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.White;
            label8.Location = new Point(90, 241);
            label8.Name = "label8";
            label8.Size = new Size(42, 15);
            label8.TabIndex = 19;
            label8.Text = "Screen";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = Color.White;
            label9.Location = new Point(170, 167);
            label9.Name = "label9";
            label9.Size = new Size(83, 15);
            label9.TabIndex = 23;
            label9.Text = "Target Process";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = Color.White;
            label10.Location = new Point(169, 211);
            label10.Name = "label10";
            label10.Size = new Size(48, 15);
            label10.TabIndex = 25;
            label10.Text = "Opacity";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.ForeColor = Color.White;
            label11.Location = new Point(223, 211);
            label11.Name = "label11";
            label11.Size = new Size(59, 15);
            label11.TabIndex = 27;
            label11.Text = "Focus key";
            // 
            // serverbox
            // 
            serverbox.AllowDrop = true;
            serverbox.Cursor = Cursors.IBeam;
            serverbox.Location = new Point(12, 31);
            serverbox.Name = "serverbox";
            serverbox.Size = new Size(244, 23);
            serverbox.TabIndex = 0;
            // 
            // testserverbutt
            // 
            testserverbutt.Location = new Point(262, 31);
            testserverbutt.Name = "testserverbutt";
            testserverbutt.Size = new Size(75, 23);
            testserverbutt.TabIndex = 2;
            testserverbutt.Text = "Test";
            testserverbutt.UseVisualStyleBackColor = true;
            testserverbutt.Click += testserverbutt_Click;
            // 
            // usernamebox
            // 
            usernamebox.Location = new Point(12, 86);
            usernamebox.Name = "usernamebox";
            usernamebox.Size = new Size(325, 23);
            usernamebox.TabIndex = 3;
            // 
            // xbox
            // 
            xbox.Location = new Point(12, 167);
            xbox.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            xbox.Minimum = new decimal(new int[] { 99999999, 0, 0, int.MinValue });
            xbox.Name = "xbox";
            xbox.Size = new Size(73, 23);
            xbox.TabIndex = 9;
            // 
            // ybox
            // 
            ybox.Location = new Point(91, 167);
            ybox.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            ybox.Minimum = new decimal(new int[] { 99999999, 0, 0, int.MinValue });
            ybox.Name = "ybox";
            ybox.Size = new Size(73, 23);
            ybox.TabIndex = 11;
            // 
            // wbox
            // 
            wbox.Location = new Point(12, 211);
            wbox.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            wbox.Name = "wbox";
            wbox.Size = new Size(73, 23);
            wbox.TabIndex = 13;
            // 
            // hbox
            // 
            hbox.Location = new Point(90, 211);
            hbox.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            hbox.Name = "hbox";
            hbox.Size = new Size(73, 23);
            hbox.TabIndex = 15;
            // 
            // guieditbutt
            // 
            guieditbutt.Location = new Point(12, 259);
            guieditbutt.Name = "guieditbutt";
            guieditbutt.Size = new Size(75, 23);
            guieditbutt.TabIndex = 17;
            guieditbutt.Text = "GUI Edit";
            guieditbutt.UseVisualStyleBackColor = true;
            guieditbutt.Click += guieditbutt_Click;
            // 
            // screenbox
            // 
            screenbox.DropDownStyle = ComboBoxStyle.DropDownList;
            screenbox.FormattingEnabled = true;
            screenbox.Location = new Point(90, 259);
            screenbox.Name = "screenbox";
            screenbox.Size = new Size(74, 23);
            screenbox.Sorted = true;
            screenbox.TabIndex = 18;
            // 
            // okbutt
            // 
            okbutt.Location = new Point(262, 297);
            okbutt.Name = "okbutt";
            okbutt.Size = new Size(75, 23);
            okbutt.TabIndex = 20;
            okbutt.Text = "OK";
            okbutt.UseVisualStyleBackColor = true;
            okbutt.Click += okbutt_Click;
            // 
            // cancelbutt
            // 
            cancelbutt.Location = new Point(181, 297);
            cancelbutt.Name = "cancelbutt";
            cancelbutt.Size = new Size(75, 23);
            cancelbutt.TabIndex = 21;
            cancelbutt.Text = "Cancel";
            cancelbutt.UseVisualStyleBackColor = true;
            cancelbutt.Click += cancelbutt_Click;
            // 
            // targetbox
            // 
            targetbox.Location = new Point(170, 185);
            targetbox.Name = "targetbox";
            targetbox.Size = new Size(167, 23);
            targetbox.TabIndex = 22;
            // 
            // opacbox
            // 
            opacbox.Location = new Point(169, 233);
            opacbox.Name = "opacbox";
            opacbox.Size = new Size(48, 23);
            opacbox.TabIndex = 24;
            // 
            // focbox
            // 
            focbox.DropDownStyle = ComboBoxStyle.DropDownList;
            focbox.FormattingEnabled = true;
            focbox.Location = new Point(223, 233);
            focbox.Name = "focbox";
            focbox.Size = new Size(114, 23);
            focbox.Sorted = true;
            focbox.TabIndex = 26;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(349, 332);
            Controls.Add(label11);
            Controls.Add(focbox);
            Controls.Add(label10);
            Controls.Add(opacbox);
            Controls.Add(label9);
            Controls.Add(targetbox);
            Controls.Add(cancelbutt);
            Controls.Add(okbutt);
            Controls.Add(label8);
            Controls.Add(screenbox);
            Controls.Add(guieditbutt);
            Controls.Add(label7);
            Controls.Add(hbox);
            Controls.Add(label6);
            Controls.Add(wbox);
            Controls.Add(label5);
            Controls.Add(ybox);
            Controls.Add(label3);
            Controls.Add(xbox);
            Controls.Add(label4);
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
            FormClosing += SettingsForm_FormClosing;
            VisibleChanged += SettingsForm_VisibleChanged;
            ((System.ComponentModel.ISupportInitialize)xbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)ybox).EndInit();
            ((System.ComponentModel.ISupportInitialize)wbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)hbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)opacbox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox serverbox;
        private Button testserverbutt;
        private TextBox usernamebox;
        private NumericUpDown xbox;
        private NumericUpDown ybox;
        private NumericUpDown wbox;
        private NumericUpDown hbox;
        private Button guieditbutt;
        private ComboBox screenbox;
        private Button okbutt;
        private Button cancelbutt;
        private TextBox targetbox;
        private NumericUpDown opacbox;
        private ComboBox focbox;
    }
}