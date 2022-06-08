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
            this.settingsTabs.SuspendLayout();
            this.dropboxTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(14, 228);
            this.closeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(139, 27);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(160, 228);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(215, 27);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save && Exit";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // settingsTabs
            // 
            this.settingsTabs.Controls.Add(this.dropboxTab);
            this.settingsTabs.Location = new System.Drawing.Point(14, 6);
            this.settingsTabs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.settingsTabs.Name = "settingsTabs";
            this.settingsTabs.SelectedIndex = 0;
            this.settingsTabs.Size = new System.Drawing.Size(360, 196);
            this.settingsTabs.TabIndex = 2;
            // 
            // dropboxTab
            // 
            this.dropboxTab.AccessibleName = "dropbox";
            this.dropboxTab.Controls.Add(this.dropboxLoginNotice);
            this.dropboxTab.Controls.Add(this.label1);
            this.dropboxTab.Controls.Add(this.loginWithDropboxButton);
            this.dropboxTab.Location = new System.Drawing.Point(4, 24);
            this.dropboxTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dropboxTab.Name = "dropboxTab";
            this.dropboxTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dropboxTab.Size = new System.Drawing.Size(352, 168);
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
            this.dropboxLoginNotice.Location = new System.Drawing.Point(0, 107);
            this.dropboxLoginNotice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.dropboxLoginNotice.Name = "dropboxLoginNotice";
            this.dropboxLoginNotice.Size = new System.Drawing.Size(242, 24);
            this.dropboxLoginNotice.TabIndex = 3;
            this.dropboxLoginNotice.Text = "❌ Not logged into Dropbox";
            this.dropboxLoginNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(38, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Click here to login with dropbox:";
            // 
            // loginWithDropboxButton
            // 
            this.loginWithDropboxButton.Location = new System.Drawing.Point(7, 54);
            this.loginWithDropboxButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.loginWithDropboxButton.Name = "loginWithDropboxButton";
            this.loginWithDropboxButton.Size = new System.Drawing.Size(337, 27);
            this.loginWithDropboxButton.TabIndex = 0;
            this.loginWithDropboxButton.Text = "Login With Dropbox";
            this.loginWithDropboxButton.UseVisualStyleBackColor = true;
            this.loginWithDropboxButton.Click += new System.EventHandler(this.loginWithDropboxButton_Click);
            // 
            // ServerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 269);
            this.Controls.Add(this.settingsTabs);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.closeButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ServerSettings";
            this.Text = "SaveDataSync - Server Settings";
            this.settingsTabs.ResumeLayout(false);
            this.dropboxTab.ResumeLayout(false);
            this.dropboxTab.PerformLayout();
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
    }
}