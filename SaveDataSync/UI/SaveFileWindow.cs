using System;
using System.IO;
using System.Windows.Forms;

namespace SaveDataSync
{
    internal partial class SaveFileWindow : Form
    {
        private SaveDataSyncEngine engine;

        public SaveFileWindow(SaveDataSyncEngine engine)
        {
            this.engine = engine;
            InitializeComponent();
        }

        public static string ImportWindow(SaveFileWindow window, string name)
        {
            window.nameTextBox.Text = name;
            window.nameTextBox.ReadOnly = true;
            window.ShowDialog();
            return window.locationTextBox.Text; // Preform all validation checks outside of the function
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
        }

        private void SaveFileWindow_Load(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (singleFileCheckBox.Checked)
            {
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    locationTextBox.Text = openFile.FileName;
                }
            }
            else
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    locationTextBox.Text = folderBrowser.SelectedPath;
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var name = nameTextBox.Text;
            if (name == null || name.Length == 0) return;
            var location = locationTextBox.Text;
            if (location == null || (!Directory.Exists(location) && !File.Exists(location))) return;
            try
            {
                engine.AddSave(name, location);
                Close();
            }
            catch (InvalidSaveException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}