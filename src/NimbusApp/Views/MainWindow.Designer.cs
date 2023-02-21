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
            this.importButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.newSaveFileButton = new System.Windows.Forms.Button();
            this.mainProgressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.settingsButton = new System.Windows.Forms.Button();
            this.saveFileList = new System.Windows.Forms.ListView();
            this.name = new System.Windows.Forms.ColumnHeader();
            this.location = new System.Windows.Forms.ColumnHeader();
            this.size = new System.Windows.Forms.ColumnHeader();
            this.status = new System.Windows.Forms.ColumnHeader();
            this.typeTitle = new System.Windows.Forms.Label();
            this.hostTitle = new System.Windows.Forms.Label();
            this.statusTitle = new System.Windows.Forms.Label();
            this.type = new System.Windows.Forms.Label();
            this.serverStatus = new System.Windows.Forms.Label();
            this.host = new System.Windows.Forms.Label();
            this.serverSettingsBtn = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.reloadDataButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(895, 611);
            this.importButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(198, 36);
            this.importButton.TabIndex = 1;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.Import_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(695, 611);
            this.exportButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(198, 36);
            this.exportButton.TabIndex = 4;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.Export_Click);
            // 
            // newSaveFileButton
            // 
            this.newSaveFileButton.Location = new System.Drawing.Point(695, 548);
            this.newSaveFileButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.newSaveFileButton.Name = "newSaveFileButton";
            this.newSaveFileButton.Size = new System.Drawing.Size(398, 53);
            this.newSaveFileButton.TabIndex = 5;
            this.newSaveFileButton.Text = "New Save File";
            this.newSaveFileButton.UseVisualStyleBackColor = true;
            this.newSaveFileButton.Click += new System.EventHandler(this.NewSaveFile_Click);
            // 
            // mainProgressBar
            // 
            this.mainProgressBar.Location = new System.Drawing.Point(619, 696);
            this.mainProgressBar.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.mainProgressBar.Name = "mainProgressBar";
            this.mainProgressBar.Size = new System.Drawing.Size(536, 36);
            this.mainProgressBar.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(769, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 39);
            this.label1.TabIndex = 7;
            this.label1.Text = "Server Status";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(695, 651);
            this.settingsButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(398, 36);
            this.settingsButton.TabIndex = 8;
            this.settingsButton.Text = "Settings";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.Settings_Click);
            // 
            // saveFileList
            // 
            this.saveFileList.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.saveFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.location,
            this.size,
            this.status});
            this.saveFileList.FullRowSelect = true;
            this.saveFileList.GridLines = true;
            this.saveFileList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.saveFileList.LabelEdit = true;
            this.saveFileList.LabelWrap = false;
            this.saveFileList.Location = new System.Drawing.Point(16, 19);
            this.saveFileList.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.saveFileList.Name = "saveFileList";
            this.saveFileList.Size = new System.Drawing.Size(595, 736);
            this.saveFileList.TabIndex = 9;
            this.saveFileList.UseCompatibleStateImageBehavior = false;
            this.saveFileList.View = System.Windows.Forms.View.Details;
            this.saveFileList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SaveFileList_MouseClick);
            // 
            // name
            // 
            this.name.Text = "Name";
            this.name.Width = 120;
            // 
            // location
            // 
            this.location.Text = "Location";
            this.location.Width = 160;
            // 
            // size
            // 
            this.size.Text = "Size";
            // 
            // status
            // 
            this.status.Text = "Status";
            this.status.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.status.Width = 30;
            // 
            // typeTitle
            // 
            this.typeTitle.AutoSize = true;
            this.typeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.typeTitle.Location = new System.Drawing.Point(728, 79);
            this.typeTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.typeTitle.Name = "typeTitle";
            this.typeTitle.Size = new System.Drawing.Size(75, 31);
            this.typeTitle.TabIndex = 10;
            this.typeTitle.Text = "Type";
            // 
            // hostTitle
            // 
            this.hostTitle.AutoSize = true;
            this.hostTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hostTitle.Location = new System.Drawing.Point(728, 152);
            this.hostTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.hostTitle.Name = "hostTitle";
            this.hostTitle.Size = new System.Drawing.Size(71, 31);
            this.hostTitle.TabIndex = 11;
            this.hostTitle.Text = "Host";
            // 
            // statusTitle
            // 
            this.statusTitle.AutoSize = true;
            this.statusTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.statusTitle.Location = new System.Drawing.Point(728, 224);
            this.statusTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.statusTitle.Name = "statusTitle";
            this.statusTitle.Size = new System.Drawing.Size(92, 31);
            this.statusTitle.TabIndex = 12;
            this.statusTitle.Text = "Status";
            // 
            // type
            // 
            this.type.AutoSize = true;
            this.type.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.type.Location = new System.Drawing.Point(954, 80);
            this.type.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(72, 29);
            this.type.TabIndex = 13;
            this.type.Text = "None";
            // 
            // serverStatus
            // 
            this.serverStatus.AutoSize = true;
            this.serverStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.serverStatus.Location = new System.Drawing.Point(953, 224);
            this.serverStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.serverStatus.Name = "serverStatus";
            this.serverStatus.Size = new System.Drawing.Size(23, 31);
            this.serverStatus.TabIndex = 15;
            this.serverStatus.Text = "-";
            // 
            // host
            // 
            this.host.AutoSize = true;
            this.host.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.host.Location = new System.Drawing.Point(953, 152);
            this.host.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(23, 31);
            this.host.TabIndex = 16;
            this.host.Text = "-";
            this.host.Click += new System.EventHandler(this.Label2_Click);
            // 
            // serverSettingsBtn
            // 
            this.serverSettingsBtn.Location = new System.Drawing.Point(729, 268);
            this.serverSettingsBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.serverSettingsBtn.Name = "serverSettingsBtn";
            this.serverSettingsBtn.Size = new System.Drawing.Size(318, 36);
            this.serverSettingsBtn.TabIndex = 17;
            this.serverSettingsBtn.Text = "Manage Server";
            this.serverSettingsBtn.UseVisualStyleBackColor = true;
            this.serverSettingsBtn.Click += new System.EventHandler(this.ServerSettingsBtn_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.progressLabel.Location = new System.Drawing.Point(891, 737);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(0, 20);
            this.progressLabel.TabIndex = 18;
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // reloadDataButton
            // 
            this.reloadDataButton.Location = new System.Drawing.Point(729, 312);
            this.reloadDataButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.reloadDataButton.Name = "reloadDataButton";
            this.reloadDataButton.Size = new System.Drawing.Size(318, 36);
            this.reloadDataButton.TabIndex = 19;
            this.reloadDataButton.Text = "Reload Data";
            this.reloadDataButton.UseVisualStyleBackColor = true;
            this.reloadDataButton.Click += new System.EventHandler(this.ReloadDataButton_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1162, 776);
            this.Controls.Add(this.reloadDataButton);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.serverSettingsBtn);
            this.Controls.Add(this.host);
            this.Controls.Add(this.serverStatus);
            this.Controls.Add(this.type);
            this.Controls.Add(this.statusTitle);
            this.Controls.Add(this.hostTitle);
            this.Controls.Add(this.typeTitle);
            this.Controls.Add(this.saveFileList);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mainProgressBar);
            this.Controls.Add(this.newSaveFileButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.importButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Nimbus";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

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

