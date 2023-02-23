namespace NimbusApp
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
            folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            button1 = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            saveButton = new System.Windows.Forms.Button();
            nameTextBox = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            locationTextBox = new System.Windows.Forms.TextBox();
            browseButton = new System.Windows.Forms.Button();
            singleFileCheckBox = new System.Windows.Forms.CheckBox();
            openFile = new System.Windows.Forms.OpenFileDialog();
            SuspendLayout();
            // 
            // folderBrowser
            // 
            folderBrowser.HelpRequest += folderBrowserDialog1_HelpRequest;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(504, 557);
            button1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(101, 36);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.Location = new System.Drawing.Point(16, 131);
            cancelButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(187, 52);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // saveButton
            // 
            saveButton.Location = new System.Drawing.Point(216, 131);
            saveButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(187, 52);
            saveButton.TabIndex = 2;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButton_Click;
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new System.Drawing.Point(96, 16);
            nameTextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new System.Drawing.Size(132, 27);
            nameTextBox.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(38, 20);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(52, 20);
            label1.TabIndex = 4;
            label1.Text = "Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(21, 83);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(69, 20);
            label2.TabIndex = 5;
            label2.Text = "Location:";
            // 
            // locationTextBox
            // 
            locationTextBox.Location = new System.Drawing.Point(96, 79);
            locationTextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            locationTextBox.Name = "locationTextBox";
            locationTextBox.Size = new System.Drawing.Size(132, 27);
            locationTextBox.TabIndex = 6;
            // 
            // browseButton
            // 
            browseButton.Location = new System.Drawing.Point(238, 79);
            browseButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(103, 31);
            browseButton.TabIndex = 7;
            browseButton.Text = "Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += BrowseButton_Click;
            // 
            // singleFileCheckBox
            // 
            singleFileCheckBox.AutoSize = true;
            singleFileCheckBox.Location = new System.Drawing.Point(239, 20);
            singleFileCheckBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            singleFileCheckBox.Name = "singleFileCheckBox";
            singleFileCheckBox.Size = new System.Drawing.Size(106, 24);
            singleFileCheckBox.TabIndex = 8;
            singleFileCheckBox.Text = "Single File?";
            singleFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // openFile
            // 
            openFile.FileName = "openFile";
            // 
            // SaveFileWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(421, 200);
            Controls.Add(singleFileCheckBox);
            Controls.Add(browseButton);
            Controls.Add(locationTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(nameTextBox);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
            Controls.Add(button1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            Name = "SaveFileWindow";
            Text = "Nimbus - Save Manager";
            Load += SaveFileWindow_Load;
            ResumeLayout(false);
            PerformLayout();
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