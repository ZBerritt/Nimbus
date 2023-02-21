using NimbusApp.Controllers;
using NimbusApp.Utils;
using System;
using System.IO;
using System.Windows.Forms;

namespace NimbusApp
{
    internal partial class SaveFileWindow : Form
    {
        public bool ShouldReload { get; private set; } = false;
        private NimbusAppEngine engine;

        public SaveFileWindow(NimbusAppEngine engine)
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

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            var name = nameTextBox.Text;
            if (name == null || name.Length == 0) return;
            var location = locationTextBox.Text;
            if (location == null || location.Length == 0) return;
            if (!FileUtils.PathExists(location) && !singleFileCheckBox.Checked) // Make folder if necessary
            {
                var response = PopupDialog.WarningPrompt($"Folder {location} does not exist. " +
                    "Would you like to make it?");
                if (response == DialogResult.Yes)
                {
                    Directory.CreateDirectory(location);
                }
            }

            try
            {
                await engine.AddSave(name, location);
                ShouldReload = true;
                Close();
            }
            catch (InvalidSaveException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}