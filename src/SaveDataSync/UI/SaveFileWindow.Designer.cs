namespace SaveDataSync
{
    partial class SaveFileWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveFileWindow));
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.locationTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.singleFileCheckBox = new System.Windows.Forms.CheckBox();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // folderBrowser
            // 
            this.folderBrowser.HelpRequest += new System.EventHandler(this.folderBrowserDialog1_HelpRequest);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(441, 418);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(14, 98);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(164, 39);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(189, 98);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(164, 39);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(84, 12);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(116, 23);
            this.nameTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Location:";
            // 
            // locationTextBox
            // 
            this.locationTextBox.Location = new System.Drawing.Point(84, 59);
            this.locationTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(116, 23);
            this.locationTextBox.TabIndex = 6;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(208, 59);
            this.browseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(90, 23);
            this.browseButton.TabIndex = 7;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // singleFileCheckBox
            // 
            this.singleFileCheckBox.AutoSize = true;
            this.singleFileCheckBox.Location = new System.Drawing.Point(209, 15);
            this.singleFileCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.singleFileCheckBox.Name = "singleFileCheckBox";
            this.singleFileCheckBox.Size = new System.Drawing.Size(84, 19);
            this.singleFileCheckBox.TabIndex = 8;
            this.singleFileCheckBox.Text = "Single File?";
            this.singleFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // openFile
            // 
            this.openFile.FileName = "openFile";
            // 
            // SaveFileWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 150);
            this.Controls.Add(this.singleFileCheckBox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.locationTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SaveFileWindow";
            this.Text = "SaveDataSync - Save File";
            this.Load += new System.EventHandler(this.SaveFileWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox locationTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckBox singleFileCheckBox;
        private System.Windows.Forms.OpenFileDialog openFile;
    }
}