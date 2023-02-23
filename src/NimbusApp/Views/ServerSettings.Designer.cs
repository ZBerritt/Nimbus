namespace NimbusApp.UI
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
            closeButton = new System.Windows.Forms.Button();
            saveButton = new System.Windows.Forms.Button();
            settingsTabs = new System.Windows.Forms.TabControl();
            dropboxTab = new System.Windows.Forms.TabPage();
            dropboxLoginNotice = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            loginWithDropboxButton = new System.Windows.Forms.Button();
            webDavPage = new System.Windows.Forms.TabPage();
            webDavPasswordInput = new System.Windows.Forms.TextBox();
            webDavUsernameInput = new System.Windows.Forms.TextBox();
            webDavUrlInput = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            fileSystemPage = new System.Windows.Forms.TabPage();
            localDirectoryTextBox = new System.Windows.Forms.TextBox();
            localBrowseButton = new System.Windows.Forms.Button();
            localDirectoryLabel = new System.Windows.Forms.Label();
            localFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            settingsTabs.SuspendLayout();
            dropboxTab.SuspendLayout();
            webDavPage.SuspendLayout();
            fileSystemPage.SuspendLayout();
            SuspendLayout();
            // 
            // closeButton
            // 
            closeButton.Location = new System.Drawing.Point(16, 304);
            closeButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(159, 36);
            closeButton.TabIndex = 0;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // saveButton
            // 
            saveButton.Location = new System.Drawing.Point(183, 304);
            saveButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(246, 36);
            saveButton.TabIndex = 1;
            saveButton.Text = "Save && Exit";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // settingsTabs
            // 
            settingsTabs.Controls.Add(dropboxTab);
            settingsTabs.Controls.Add(webDavPage);
            settingsTabs.Controls.Add(fileSystemPage);
            settingsTabs.Location = new System.Drawing.Point(16, 8);
            settingsTabs.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            settingsTabs.Name = "settingsTabs";
            settingsTabs.SelectedIndex = 0;
            settingsTabs.Size = new System.Drawing.Size(411, 261);
            settingsTabs.TabIndex = 2;
            // 
            // dropboxTab
            // 
            dropboxTab.AccessibleName = "dropbox";
            dropboxTab.Controls.Add(dropboxLoginNotice);
            dropboxTab.Controls.Add(label1);
            dropboxTab.Controls.Add(loginWithDropboxButton);
            dropboxTab.Location = new System.Drawing.Point(4, 29);
            dropboxTab.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            dropboxTab.Name = "dropboxTab";
            dropboxTab.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            dropboxTab.Size = new System.Drawing.Size(403, 228);
            dropboxTab.TabIndex = 1;
            dropboxTab.Text = "Dropbox";
            dropboxTab.UseVisualStyleBackColor = true;
            // 
            // dropboxLoginNotice
            // 
            dropboxLoginNotice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dropboxLoginNotice.AutoSize = true;
            dropboxLoginNotice.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dropboxLoginNotice.Location = new System.Drawing.Point(0, 143);
            dropboxLoginNotice.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dropboxLoginNotice.Name = "dropboxLoginNotice";
            dropboxLoginNotice.Size = new System.Drawing.Size(308, 29);
            dropboxLoginNotice.TabIndex = 3;
            dropboxLoginNotice.Text = "❌ Not logged into Dropbox";
            dropboxLoginNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(43, 37);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(287, 25);
            label1.TabIndex = 2;
            label1.Text = "Click here to login with dropbox:";
            // 
            // loginWithDropboxButton
            // 
            loginWithDropboxButton.Location = new System.Drawing.Point(8, 72);
            loginWithDropboxButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            loginWithDropboxButton.Name = "loginWithDropboxButton";
            loginWithDropboxButton.Size = new System.Drawing.Size(385, 36);
            loginWithDropboxButton.TabIndex = 0;
            loginWithDropboxButton.Text = "Login With Dropbox";
            loginWithDropboxButton.UseVisualStyleBackColor = true;
            loginWithDropboxButton.Click += loginWithDropboxButton_Click;
            // 
            // webDavPage
            // 
            webDavPage.AccessibleName = "webdav";
            webDavPage.Controls.Add(webDavPasswordInput);
            webDavPage.Controls.Add(webDavUsernameInput);
            webDavPage.Controls.Add(webDavUrlInput);
            webDavPage.Controls.Add(label4);
            webDavPage.Controls.Add(label3);
            webDavPage.Controls.Add(label2);
            webDavPage.Location = new System.Drawing.Point(4, 29);
            webDavPage.Name = "webDavPage";
            webDavPage.Size = new System.Drawing.Size(403, 228);
            webDavPage.TabIndex = 2;
            webDavPage.Text = "WebDAV";
            webDavPage.UseVisualStyleBackColor = true;
            // 
            // webDavPasswordInput
            // 
            webDavPasswordInput.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            webDavPasswordInput.Location = new System.Drawing.Point(163, 142);
            webDavPasswordInput.Name = "webDavPasswordInput";
            webDavPasswordInput.Size = new System.Drawing.Size(213, 38);
            webDavPasswordInput.TabIndex = 5;
            webDavPasswordInput.UseSystemPasswordChar = true;
            // 
            // webDavUsernameInput
            // 
            webDavUsernameInput.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            webDavUsernameInput.Location = new System.Drawing.Point(163, 88);
            webDavUsernameInput.Name = "webDavUsernameInput";
            webDavUsernameInput.Size = new System.Drawing.Size(213, 38);
            webDavUsernameInput.TabIndex = 4;
            // 
            // webDavUrlInput
            // 
            webDavUrlInput.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            webDavUrlInput.Location = new System.Drawing.Point(163, 35);
            webDavUrlInput.Name = "webDavUrlInput";
            webDavUrlInput.Size = new System.Drawing.Size(213, 38);
            webDavUrlInput.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label4.Location = new System.Drawing.Point(13, 142);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(115, 31);
            label4.TabIndex = 2;
            label4.Text = "Password:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(13, 38);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(60, 31);
            label3.TabIndex = 1;
            label3.Text = "URL:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(13, 91);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(122, 31);
            label2.TabIndex = 0;
            label2.Text = "Username:";
            // 
            // fileSystemPage
            // 
            fileSystemPage.AccessibleName = "file";
            fileSystemPage.Controls.Add(localDirectoryTextBox);
            fileSystemPage.Controls.Add(localBrowseButton);
            fileSystemPage.Controls.Add(localDirectoryLabel);
            fileSystemPage.Location = new System.Drawing.Point(4, 29);
            fileSystemPage.Name = "fileSystemPage";
            fileSystemPage.Size = new System.Drawing.Size(403, 228);
            fileSystemPage.TabIndex = 3;
            fileSystemPage.Text = "Local";
            fileSystemPage.UseVisualStyleBackColor = true;
            // 
            // localDirectoryTextBox
            // 
            localDirectoryTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            localDirectoryTextBox.Location = new System.Drawing.Point(121, 37);
            localDirectoryTextBox.Name = "localDirectoryTextBox";
            localDirectoryTextBox.Size = new System.Drawing.Size(279, 38);
            localDirectoryTextBox.TabIndex = 9;
            // 
            // localBrowseButton
            // 
            localBrowseButton.Location = new System.Drawing.Point(121, 82);
            localBrowseButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            localBrowseButton.Name = "localBrowseButton";
            localBrowseButton.Size = new System.Drawing.Size(112, 41);
            localBrowseButton.TabIndex = 8;
            localBrowseButton.Text = "Browse...";
            localBrowseButton.UseVisualStyleBackColor = true;
            localBrowseButton.Click += localBrowseButton_Click;
            // 
            // localDirectoryLabel
            // 
            localDirectoryLabel.AutoSize = true;
            localDirectoryLabel.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            localDirectoryLabel.Location = new System.Drawing.Point(3, 37);
            localDirectoryLabel.Name = "localDirectoryLabel";
            localDirectoryLabel.Size = new System.Drawing.Size(112, 31);
            localDirectoryLabel.TabIndex = 2;
            localDirectoryLabel.Text = "Directory:";
            // 
            // ServerSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(443, 359);
            Controls.Add(settingsTabs);
            Controls.Add(saveButton);
            Controls.Add(closeButton);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            Name = "ServerSettings";
            Text = "Nimbus - Remote Server";
            settingsTabs.ResumeLayout(false);
            dropboxTab.ResumeLayout(false);
            dropboxTab.PerformLayout();
            webDavPage.ResumeLayout(false);
            webDavPage.PerformLayout();
            fileSystemPage.ResumeLayout(false);
            fileSystemPage.PerformLayout();
            ResumeLayout(false);
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
        private System.Windows.Forms.TabPage fileSystemPage;
        private System.Windows.Forms.Label localDirectoryLabel;
        private System.Windows.Forms.TextBox localDirectoryTextBox;
        private System.Windows.Forms.Button localBrowseButton;
        private System.Windows.Forms.FolderBrowserDialog localFolderBrowser;
    }
}