namespace SaveDataSync.UI
{
    partial class ServerSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerSettings));
            this.closeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.settingsTabs = new System.Windows.Forms.TabControl();
            this.dropboxTab = new System.Windows.Forms.TabPage();
            this.dropboxLoginNotice = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.loginWithDropboxButton = new System.Windows.Forms.Button();
            this.webDavPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.webDavUrlInput = new System.Windows.Forms.TextBox();
            this.webDavUsernameInput = new System.Windows.Forms.TextBox();
            this.webDavPasswordInput = new System.Windows.Forms.TextBox();
            this.settingsTabs.SuspendLayout();
            this.dropboxTab.SuspendLayout();
            this.webDavPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(16, 304);
            this.closeButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(159, 36);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(183, 304);
            this.saveButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(246, 36);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save && Exit";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // settingsTabs
            // 
            this.settingsTabs.Controls.Add(this.dropboxTab);
            this.settingsTabs.Controls.Add(this.webDavPage);
            this.settingsTabs.Location = new System.Drawing.Point(16, 8);
            this.settingsTabs.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.settingsTabs.Name = "settingsTabs";
            this.settingsTabs.SelectedIndex = 0;
            this.settingsTabs.Size = new System.Drawing.Size(411, 261);
            this.settingsTabs.TabIndex = 2;
            // 
            // dropboxTab
            // 
            this.dropboxTab.AccessibleName = "dropbox";
            this.dropboxTab.Controls.Add(this.dropboxLoginNotice);
            this.dropboxTab.Controls.Add(this.label1);
            this.dropboxTab.Controls.Add(this.loginWithDropboxButton);
            this.dropboxTab.Location = new System.Drawing.Point(4, 29);
            this.dropboxTab.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dropboxTab.Name = "dropboxTab";
            this.dropboxTab.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dropboxTab.Size = new System.Drawing.Size(403, 228);
            this.dropboxTab.TabIndex = 1;
            this.dropboxTab.Text = "Dropbox";
            this.dropboxTab.UseVisualStyleBackColor = true;
            // 
            // dropboxLoginNotice
            // 
            this.dropboxLoginNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dropboxLoginNotice.AutoSize = true;
            this.dropboxLoginNotice.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dropboxLoginNotice.Location = new System.Drawing.Point(0, 143);
            this.dropboxLoginNotice.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.dropboxLoginNotice.Name = "dropboxLoginNotice";
            this.dropboxLoginNotice.Size = new System.Drawing.Size(308, 29);
            this.dropboxLoginNotice.TabIndex = 3;
            this.dropboxLoginNotice.Text = "❌ Not logged into Dropbox";
            this.dropboxLoginNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(43, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Click here to login with dropbox:";
            // 
            // loginWithDropboxButton
            // 
            this.loginWithDropboxButton.Location = new System.Drawing.Point(8, 72);
            this.loginWithDropboxButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.loginWithDropboxButton.Name = "loginWithDropboxButton";
            this.loginWithDropboxButton.Size = new System.Drawing.Size(385, 36);
            this.loginWithDropboxButton.TabIndex = 0;
            this.loginWithDropboxButton.Text = "Login With Dropbox";
            this.loginWithDropboxButton.UseVisualStyleBackColor = true;
            this.loginWithDropboxButton.Click += new System.EventHandler(this.loginWithDropboxButton_Click);
            // 
            // webDavPage
            // 
            this.webDavPage.AccessibleName = "webdav";
            this.webDavPage.Controls.Add(this.webDavPasswordInput);
            this.webDavPage.Controls.Add(this.webDavUsernameInput);
            this.webDavPage.Controls.Add(this.webDavUrlInput);
            this.webDavPage.Controls.Add(this.label4);
            this.webDavPage.Controls.Add(this.label3);
            this.webDavPage.Controls.Add(this.label2);
            this.webDavPage.Location = new System.Drawing.Point(4, 29);
            this.webDavPage.Name = "webDavPage";
            this.webDavPage.Size = new System.Drawing.Size(403, 228);
            this.webDavPage.TabIndex = 2;
            this.webDavPage.Text = "WebDAV";
            this.webDavPage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(13, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 31);
            this.label2.TabIndex = 0;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(13, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 31);
            this.label3.TabIndex = 1;
            this.label3.Text = "URL:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(13, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 31);
            this.label4.TabIndex = 2;
            this.label4.Text = "Password:";
            // 
            // webDavUrlInput
            // 
            this.webDavUrlInput.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.webDavUrlInput.Location = new System.Drawing.Point(163, 35);
            this.webDavUrlInput.Name = "webDavUrlInput";
            this.webDavUrlInput.Size = new System.Drawing.Size(213, 38);
            this.webDavUrlInput.TabIndex = 3;
            // 
            // webDavUsernameInput
            // 
            this.webDavUsernameInput.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.webDavUsernameInput.Location = new System.Drawing.Point(163, 88);
            this.webDavUsernameInput.Name = "webDavUsernameInput";
            this.webDavUsernameInput.Size = new System.Drawing.Size(213, 38);
            this.webDavUsernameInput.TabIndex = 4;
            // 
            // webDavPasswordInput
            // 
            this.webDavPasswordInput.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.webDavPasswordInput.Location = new System.Drawing.Point(163, 142);
            this.webDavPasswordInput.Name = "webDavPasswordInput";
            this.webDavPasswordInput.Size = new System.Drawing.Size(213, 38);
            this.webDavPasswordInput.TabIndex = 5;
            this.webDavPasswordInput.UseSystemPasswordChar = true;
            // 
            // ServerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 359);
            this.Controls.Add(this.settingsTabs);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.closeButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "ServerSettings";
            this.Text = "SaveDataSync - Server Settings";
            this.settingsTabs.ResumeLayout(false);
            this.dropboxTab.ResumeLayout(false);
            this.dropboxTab.PerformLayout();
            this.webDavPage.ResumeLayout(false);
            this.webDavPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TabControl settingsTabs;
        private System.Windows.Forms.TabPage dropboxTab;
        private System.Windows.Forms.Label dropboxLoginNotice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button loginWithDropboxButton;
        private System.Windows.Forms.TabPage webDavPage;
        private System.Windows.Forms.TextBox webDavPasswordInput;
        private System.Windows.Forms.TextBox webDavUsernameInput;
        private System.Windows.Forms.TextBox webDavUrlInput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}