using SaveDataSync.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SaveDataSync
{
    /// <summary>
    /// Represents the main GUI window used in the app
    /// </summary>
    public partial class MainWindow : Form
    {
        private SaveDataSyncEngine engine;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Loading Events
        private void OnLoad(object sender, EventArgs e)
        {
            // Set name as debug if the program is in debug mode
#if DEBUG
            this.Text = "SaveDataSync - DEBUG";
#endif
            // Grabs the engine which allows communication with the backend
            engine = SaveDataSyncEngine.Start();

            // Auto sizes the last column of the save list
            saveFileList.Columns[^1].Width = -2;

            // Forces the save list to sort correctly
            saveFileList.ListViewItemSorter = new SaveListSorter();

            // Loads the save list with the imported data from the engine
            ReloadUI();
        }

        //Used to reload all UI data
        public void ReloadUI()
        {
            /* Get main window data asyncronously */
            var data = MainWindowData.GetMainWindowData(engine);

            type.Text = data.ServerType;
            host.Text = data.ServerHost;
            serverStatus.Text = data.ServerStatus;
            serverStatus.ForeColor = data.ServerColor;

            saveFileList.Items.Clear();
            foreach (var item in data.saveList)
            {
                saveFileList.Items.Add(item);
            }

            /* Change buttons */
            UpdateButtons();
        }

        // Click Events
        private void NewSaveFile_Click(object sender, EventArgs e)
        {
            var sfw = new SaveFileWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = false
            };
            sfw.ShowDialog();
            ReloadUI();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            var sw = new SettingsWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            sw.ShowDialog();
            ReloadUI();
        }

        private void ServerSettingsBtn_Click(object sender, EventArgs e)
        {
            var ss = new ServerSettings(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            ss.ShowDialog();
            ReloadUI();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void Import_Click(object sender, EventArgs e)
        {
            Import();
        }

        private void SaveFileList_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateButtons();
            if (e.Button == MouseButtons.Right)
            {
                var selectedItem = saveFileList.FocusedItem;
                if (selectedItem == null)
                {
                    return;
                }
                SaveFileContextMenu(selectedItem.Text).Show(saveFileList, new Point(e.X, e.Y));
            }
        }

        private ContextMenuStrip SaveFileContextMenu(string name)
        {
            var menu = new ContextMenuStrip();
            var selectedSaves = GetSelectedSaves();
            bool singleSelected = selectedSaves.Count == 1;
            bool hasRemote = SelectingRemoteSave();
            bool serverOnline = engine.Server is not null && engine.Server.ServerOnline();

#if DEBUG
            var getHashes = menu.Items.Add("[DEBUG] Get Hashes");
            getHashes.Enabled = !hasRemote && serverOnline && singleSelected;
            getHashes.Click += (object sender, EventArgs e) =>
            {
                if (SelectingRemoteSave()) return;
                var selected = GetSelectedSaves();
                var first = selected[0]; // I don't care I just want the first one
                var remoteHash = engine.GetRemoteHash(first);
                var localHash = engine.GetLocalHash(first);
                MessageBox.Show($"Remote Hash: {remoteHash} (Length: {remoteHash.Length})\n" +
                    $"Local Hash: {localHash} (Length: {remoteHash.Length})",
                           "Debug",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
            };

#endif

            var goToLocation = menu.Items.Add("Open File Location");
            goToLocation.Enabled = !hasRemote && singleSelected;
            goToLocation.Click += (object sender2, EventArgs e2) =>
            {
                try
                {
                    string savePath = engine.LocalSaves.GetSavePath(name);
                    if (!File.Exists(savePath) && !Directory.Exists(savePath))
                    {
                        MessageBox.Show("Save location cannot be found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    Process.Start("explorer.exe", $"/select, \"{savePath}\"");
                }
                catch (Exception) { }
            };

            var quickExport = menu.Items.Add("Quick Export");
            quickExport.Enabled = !hasRemote && serverOnline;
            quickExport.Click += (object sender3, EventArgs e3) =>
            {
                Export();
            };

            var quickImport = menu.Items.Add("Quick Import");
            quickImport.Enabled = !hasRemote && serverOnline;
            quickImport.Click += (object sender4, EventArgs e4) =>
            {
                Import();
            };

            var removeSave = menu.Items.Add("Remove Local Save");
            removeSave.Enabled = !hasRemote;
            removeSave.Click += (object sender5, EventArgs e5) =>
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.Append("Are you sure you want to remove the following saves?");
                foreach (var save in selectedSaves)
                {
                    messageBuilder.Append($"\n• {save}");
                };

                var confirm = MessageBox.Show(messageBuilder.ToString(),
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    engine.LocalSaves.RemoveSave(name);
                    engine.SaveAllData();
                    ReloadUI();
                }
            };

            return menu;
        }

        private void Label2_Click(object sender, EventArgs e)
        {
            var url = "http://" + host.Text;
            Utils.OpenUrl(url);
        }

        private List<string> GetSelectedSaves()
        {
            var selected = saveFileList.SelectedItems;
            var saves = new List<string>();
            foreach (ListViewItem item in selected)
            {
                saves.Add(item.SubItems[0].Text); // The first sub item will always be the name
            }

            return saves;
        }

        private bool SelectingRemoteSave()
        {
            var selected = saveFileList.SelectedItems;
            foreach (ListViewItem item in selected)
            {
                if (item.SubItems[1].Text == "Remote")
                    return true;
            }

            return false;
        }

        private void UpdateButtons()
        {
            var selectingSaves = GetSelectedSaves().Count > 0;
            var CanExportAndImport = engine.Server != null && engine.Server.ServerOnline() && selectingSaves;
            exportButton.Enabled = !SelectingRemoteSave() && CanExportAndImport; // Don't want to export remote saves
            importButton.Enabled = CanExportAndImport;
        }

        private void Export()
        {
            {
                try
                {
                    if (SelectingRemoteSave())
                    {
                        MessageBox.Show("Cannot export remote saves. Aborting!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    List<string> savesToExport = GetSelectedSaves();
                    using var progressBar = ProgressBarControl.Start(mainProgressBar, progressLabel, savesToExport.Count);
                    var success = engine.ExportSaves(savesToExport.ToArray(), progressBar);
                    if (success.Length != 0)
                    {
                        var successMessage = new StringBuilder();
                        successMessage.Append("Successfully Exported:");
                        foreach (var name in success)
                        {
                            successMessage.Append($"\n• {name}");
                        }
                        MessageBox.Show(successMessage.ToString(),
                        "Success!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An erorr has occured: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ReloadUI();
                }
            }
        }

        private void Import()
        {
            try
            {
                List<string> savesToImport = GetSelectedSaves();
                using var progressBar = ProgressBarControl.Start(mainProgressBar, progressLabel, savesToImport.Count);
                var success = engine.ImportSaves(savesToImport.ToArray(), progressBar);
                if (success.Length != 0)
                {
                    var successMessage = new StringBuilder();
                    successMessage.Append("Successfully Imported:");
                    foreach (var name in success)
                    {
                        successMessage.Append($"\n• {name}");
                    }
                    MessageBox.Show(successMessage.ToString(),
                    "Success!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An erorr has occured: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ReloadUI();
            }
        }
    }
}