namespace NimbusApp
{
    partial class MainWindow
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                reloadCancelTokenSource.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            importButton = new System.Windows.Forms.Button();
            exportButton = new System.Windows.Forms.Button();
            newSaveFileButton = new System.Windows.Forms.Button();
            mainProgressBar = new System.Windows.Forms.ProgressBar();
            label1 = new System.Windows.Forms.Label();
            settingsButton = new System.Windows.Forms.Button();
            saveFileList = new System.Windows.Forms.ListView();
            name = new System.Windows.Forms.ColumnHeader();
            location = new System.Windows.Forms.ColumnHeader();
            size = new System.Windows.Forms.ColumnHeader();
            status = new System.Windows.Forms.ColumnHeader();
            typeTitle = new System.Windows.Forms.Label();
            hostTitle = new System.Windows.Forms.Label();
            statusTitle = new System.Windows.Forms.Label();
            type = new System.Windows.Forms.Label();
            serverStatus = new System.Windows.Forms.Label();
            host = new System.Windows.Forms.Label();
            serverSettingsBtn = new System.Windows.Forms.Button();
            progressLabel = new System.Windows.Forms.Label();
            reloadDataButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // importButton
            // 
            importButton.Location = new System.Drawing.Point(895, 611);
            importButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            importButton.Name = "importButton";
            importButton.Size = new System.Drawing.Size(198, 36);
            importButton.TabIndex = 1;
            importButton.Text = "Import";
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += Import_Click;
            // 
            // exportButton
            // 
            exportButton.Location = new System.Drawing.Point(695, 611);
            exportButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            exportButton.Name = "exportButton";
            exportButton.Size = new System.Drawing.Size(198, 36);
            exportButton.TabIndex = 4;
            exportButton.Text = "Export";
            exportButton.UseVisualStyleBackColor = true;
            exportButton.Click += Export_Click;
            // 
            // newSaveFileButton
            // 
            newSaveFileButton.Location = new System.Drawing.Point(695, 548);
            newSaveFileButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            newSaveFileButton.Name = "newSaveFileButton";
            newSaveFileButton.Size = new System.Drawing.Size(398, 53);
            newSaveFileButton.TabIndex = 5;
            newSaveFileButton.Text = "New Save File";
            newSaveFileButton.UseVisualStyleBackColor = true;
            newSaveFileButton.Click += NewSaveFile_Click;
            // 
            // mainProgressBar
            // 
            mainProgressBar.Location = new System.Drawing.Point(619, 696);
            mainProgressBar.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            mainProgressBar.Name = "mainProgressBar";
            mainProgressBar.Size = new System.Drawing.Size(536, 36);
            mainProgressBar.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Noto Mono for Powerline", 19.8000011F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.Black;
            label1.Location = new System.Drawing.Point(742, 19);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(277, 40);
            label1.TabIndex = 7;
            label1.Text = "Server Status";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingsButton
            // 
            settingsButton.Location = new System.Drawing.Point(695, 651);
            settingsButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new System.Drawing.Size(398, 36);
            settingsButton.TabIndex = 8;
            settingsButton.Text = "Settings";
            settingsButton.UseVisualStyleBackColor = true;
            settingsButton.Click += Settings_Click;
            // 
            // saveFileList
            // 
            saveFileList.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            saveFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { name, location, size, status });
            saveFileList.FullRowSelect = true;
            saveFileList.GridLines = true;
            saveFileList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            saveFileList.LabelEdit = true;
            saveFileList.LabelWrap = false;
            saveFileList.Location = new System.Drawing.Point(16, 19);
            saveFileList.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            saveFileList.Name = "saveFileList";
            saveFileList.Size = new System.Drawing.Size(595, 736);
            saveFileList.TabIndex = 9;
            saveFileList.UseCompatibleStateImageBehavior = false;
            saveFileList.View = System.Windows.Forms.View.Details;
            saveFileList.MouseClick += SaveFileList_MouseClick;
            // 
            // name
            // 
            name.Text = "Name";
            name.Width = 120;
            // 
            // location
            // 
            location.Text = "Location";
            location.Width = 160;
            // 
            // size
            // 
            size.Text = "Size";
            // 
            // status
            // 
            status.Text = "Status";
            status.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            status.Width = 30;
            // 
            // typeTitle
            // 
            typeTitle.AutoSize = true;
            typeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            typeTitle.Location = new System.Drawing.Point(728, 79);
            typeTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            typeTitle.Name = "typeTitle";
            typeTitle.Size = new System.Drawing.Size(83, 31);
            typeTitle.TabIndex = 10;
            typeTitle.Text = "Type:";
            // 
            // hostTitle
            // 
            hostTitle.AutoSize = true;
            hostTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            hostTitle.Location = new System.Drawing.Point(728, 152);
            hostTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            hostTitle.Name = "hostTitle";
            hostTitle.Size = new System.Drawing.Size(79, 31);
            hostTitle.TabIndex = 11;
            hostTitle.Text = "Host:";
            // 
            // statusTitle
            // 
            statusTitle.AutoSize = true;
            statusTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            statusTitle.Location = new System.Drawing.Point(728, 224);
            statusTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            statusTitle.Name = "statusTitle";
            statusTitle.Size = new System.Drawing.Size(100, 31);
            statusTitle.TabIndex = 12;
            statusTitle.Text = "Status:";
            // 
            // type
            // 
            type.AutoSize = true;
            type.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            type.Location = new System.Drawing.Point(953, 79);
            type.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            type.Name = "type";
            type.Size = new System.Drawing.Size(79, 31);
            type.TabIndex = 13;
            type.Text = "None";
            // 
            // serverStatus
            // 
            serverStatus.AutoSize = true;
            serverStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            serverStatus.Location = new System.Drawing.Point(953, 224);
            serverStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            serverStatus.Name = "serverStatus";
            serverStatus.Size = new System.Drawing.Size(23, 31);
            serverStatus.TabIndex = 15;
            serverStatus.Text = "-";
            // 
            // host
            // 
            host.AutoSize = true;
            host.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            host.Location = new System.Drawing.Point(953, 152);
            host.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            host.Name = "host";
            host.Size = new System.Drawing.Size(23, 31);
            host.TabIndex = 16;
            host.Text = "-";
            host.Click += Label2_Click;
            // 
            // serverSettingsBtn
            // 
            serverSettingsBtn.Location = new System.Drawing.Point(729, 268);
            serverSettingsBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            serverSettingsBtn.Name = "serverSettingsBtn";
            serverSettingsBtn.Size = new System.Drawing.Size(318, 36);
            serverSettingsBtn.TabIndex = 17;
            serverSettingsBtn.Text = "Manage Server";
            serverSettingsBtn.UseVisualStyleBackColor = true;
            serverSettingsBtn.Click += ServerSettingsBtn_Click;
            // 
            // progressLabel
            // 
            progressLabel.AutoSize = true;
            progressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            progressLabel.Location = new System.Drawing.Point(891, 737);
            progressLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            progressLabel.Name = "progressLabel";
            progressLabel.Size = new System.Drawing.Size(0, 20);
            progressLabel.TabIndex = 18;
            progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // reloadDataButton
            // 
            reloadDataButton.Location = new System.Drawing.Point(729, 312);
            reloadDataButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            reloadDataButton.Name = "reloadDataButton";
            reloadDataButton.Size = new System.Drawing.Size(318, 36);
            reloadDataButton.TabIndex = 19;
            reloadDataButton.Text = "Reload Data";
            reloadDataButton.UseVisualStyleBackColor = true;
            reloadDataButton.Click += ReloadDataButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1162, 776);
            Controls.Add(reloadDataButton);
            Controls.Add(progressLabel);
            Controls.Add(serverSettingsBtn);
            Controls.Add(host);
            Controls.Add(serverStatus);
            Controls.Add(type);
            Controls.Add(statusTitle);
            Controls.Add(hostTitle);
            Controls.Add(typeTitle);
            Controls.Add(saveFileList);
            Controls.Add(settingsButton);
            Controls.Add(label1);
            Controls.Add(mainProgressBar);
            Controls.Add(newSaveFileButton);
            Controls.Add(exportButton);
            Controls.Add(importButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            MaximizeBox = false;
            Name = "MainWindow";
            Text = "Nimbus";
            Load += OnLoad;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.ProgressBar mainProgressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button newSaveFileButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.ListView saveFileList;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader location;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.ColumnHeader status;
        private System.Windows.Forms.Label typeTitle;
        private System.Windows.Forms.Label hostTitle;
        private System.Windows.Forms.Label statusTitle;
        private System.Windows.Forms.Label type;
        private System.Windows.Forms.Label serverStatus;
        private System.Windows.Forms.Label host;
        private System.Windows.Forms.Button serverSettingsBtn;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Button reloadDataButton;
    }
}

