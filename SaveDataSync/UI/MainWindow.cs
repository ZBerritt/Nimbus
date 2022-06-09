using SaveDataSync.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private async void OnLoad(object sender, EventArgs e) // Can't use Task here since the program wants to return void

        {
            // Set name as debug if the program is in debug mode
#if DEBUG
            Text = "SaveDataSync - DEBUG";
#endif
            // Grabs the engine which allows communication with the backend
            engine = SaveDataSyncEngine.Start();

            // Auto sizes the last column of the save list
            saveFileList.Columns[^1].Width = -2;

            // Forces the save list to sort correctly
            saveFileList.ListViewItemSorter = new SaveListSorter();

            // Loads the save list with the imported data from the engine
            await ReloadUI();
        }

        //Used to reload all UI data
        public async Task ReloadUI()
        {
            /* Setup */
            saveFileList.Items.Clear();

            /* Get main window data asyncronously */
            var serverOnline = engine.Server is not null && await engine.Server.ServerOnline();
            var statusDataTask = SetServerStatus(serverOnline);
            var localSaveDataTask = SetLocalServerList(serverOnline);
            var remoteSaveDataTask = SetRemoteServerList(serverOnline);
            await Task.WhenAll(statusDataTask, localSaveDataTask, remoteSaveDataTask);

            /* Change buttons */
            await UpdateButtons();
        }

        public async Task SetServerStatus(bool serverOnline)
        {
            var server = engine.Server;
            string ServerType = "N/A";
            string ServerHost = "N/A";
            string ServerStatus = "None";
            /* Check server status */
            if (server is not null)
            {
                ServerType = server.Name;
                ServerHost = server.Host;
                try
                {
                    ServerStatus = serverOnline ? "Online" : "Offline";
                }
                catch (Exception)
                {
                    ServerStatus = "Error";
                }
            }

            var StatusColor = ServerStatus switch
            {
                "Online" => Color.Green,
                "Offline" => Color.DarkGoldenrod,
                "Error" => Color.Red,
                _ => Color.Black,
            };

            type.Text = ServerType;
            host.Text = ServerHost;
            serverStatus.Text = ServerStatus;
            serverStatus.ForeColor = StatusColor;
        }

        public async Task SetLocalServerList(bool serverOnline)
        {
            /* Get a list of the data for the table */
            var saves = engine.LocalSaves.Saves;
            foreach (var save in saves)
            {
                var saveItem = new ListViewItem(save.Key)
                {
                    UseItemStyleForSubItems = false
                };

                // Get location
                saveItem.SubItems.Add(save.Value);

                // Get file size
                var fileSize = File.Exists(save.Value) || Directory.Exists(save.Value)
                    ? FileUtils.ReadableFileSize(FileUtils.GetSize(save.Value))
                    : "N/A";
                saveItem.SubItems.Add(fileSize);

                // Get file sync status
                var statusItem = new ListViewItem.ListViewSubItem(saveItem, "Checking...");
                saveItem.SubItems.Add(statusItem);

                // Add to the list
                saveFileList.Items.Add(saveItem);
            }

            await SetLocalSaveStatuses(serverOnline);
        }

        public async Task SetRemoteServerList(bool serverOnline)
        {
            /* Add remote saves to the list */
            var server = engine.Server;
            var saveList = new List<ListViewItem>();
            if (serverOnline)
            {
                var remoteSaveNames = await server.SaveNames();
                var filtered = remoteSaveNames.Where(c => !engine.LocalSaves.Saves.ContainsKey(c));
                foreach (var s in filtered)
                {
                    var remoteSaveItem = new ListViewItem(s)
                    {
                        ForeColor = Color.DarkRed
                    };
                    remoteSaveItem.SubItems.Add("Remote");
                    remoteSaveItem.SubItems.Add("N/A");
                    remoteSaveItem.SubItems.Add("On Server");
                    saveFileList.Items.Add(remoteSaveItem);
                }
            }
        }

        public async Task SetLocalSaveStatuses(bool serverOnline)
        {
            foreach (ListViewItem item in saveFileList.Items)
            {
                var saveName = item.SubItems[0].Text;

                var foundSave = engine.LocalSaves.Saves.TryGetValue(saveName, out string location);
                if (!foundSave) return; // Ignore if its a remote save

                var statusItem = item.SubItems[^1];

                if (serverOnline && (File.Exists(location) || Directory.Exists(location)))
                {
                    var localHash = await engine.GetLocalHash(saveName);
                    var remoteHash = await engine.GetRemoteHash(saveName);
                    if (remoteHash is null)
                    {
                        statusItem.Text = "Not Uploaded";
                        statusItem.ForeColor = Color.Gray;
                    }
                    else if (remoteHash == localHash)
                    {
                        statusItem.Text = "Synced";
                        statusItem.ForeColor = Color.Green;
                    }
                    else
                    {
                        statusItem.Text = "Not Synced";
                        statusItem.ForeColor = Color.DarkRed;
                    }
                }
                else if (!File.Exists(location) && !Directory.Exists(location))
                {
                    statusItem.Text = "No Local Save";
                    statusItem.ForeColor = Color.Gray;
                }
                else if (engine.Server is not null)
                {
                    statusItem.Text = "Offline";
                    statusItem.ForeColor = Color.DarkGoldenrod;
                }
                else
                {
                    statusItem.Text = "No Server";
                    statusItem.ForeColor = Color.Black;
                }
            }
        }

        // Click Events
        private async void NewSaveFile_Click(object sender, EventArgs e)
        {
            var sfw = new SaveFileWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = false
            };
            sfw.ShowDialog();
            await ReloadUI();
        }

        private async void Settings_Click(object sender, EventArgs e)
        {
            var sw = new SettingsWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            sw.ShowDialog();
            await ReloadUI();
        }

        private async void ServerSettingsBtn_Click(object sender, EventArgs e)
        {
            var ss = new ServerSettings(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            ss.ShowDialog();
            await ReloadUI();
        }

        private async void Export_Click(object sender, EventArgs e)
        {
            await Export();
        }

        private async void Import_Click(object sender, EventArgs e)
        {
            await Import();
        }

        private async void SaveFileList_MouseClick(object sender, MouseEventArgs e)
        {
            await UpdateButtons();
            if (e.Button == MouseButtons.Right)
            {
                var selectedItem = saveFileList.FocusedItem;
                if (selectedItem == null)
                {
                    return;
                }
                var contextMenu = await SaveFileContextMenu(selectedItem.Text);
                contextMenu.Show(saveFileList, new Point(e.X, e.Y));
            }
        }

        private async Task<ContextMenuStrip> SaveFileContextMenu(string name)
        {
            var menu = new ContextMenuStrip();
            var selectedSaves = GetSelectedSaves();
            bool singleSelected = selectedSaves.Count == 1;
            bool hasRemote = SelectingRemoteSave();
            var server = engine.Server;
            bool serverOnline = server is not null && await server.ServerOnline();

#if DEBUG
            var getHashes = menu.Items.Add("[DEBUG] Get Hashes");
            getHashes.Enabled = !hasRemote && serverOnline && singleSelected;
            getHashes.Click += async (object sender, EventArgs e) =>
            {
                if (SelectingRemoteSave()) return;
                var selected = GetSelectedSaves();
                var first = selected[0]; // I don't care I just want the first one
                var remoteHash = await engine.GetRemoteHash(first);
                var localHash = await engine.GetLocalHash(first);
                MessageBox.Show($"Remote Hash: {remoteHash} (Length: {remoteHash.Length})\n" +
                    $"Local Hash: {localHash} (Length: {localHash.Length})",
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
            quickExport.Click += async (object sender3, EventArgs e3) =>
            {
                await Export();
            };

            var quickImport = menu.Items.Add("Quick Import");
            quickImport.Enabled = serverOnline;
            quickImport.Click += async (object sender4, EventArgs e4) =>
            {
                await Import();
            };

            var removeSave = menu.Items.Add("Remove Local Save");
            removeSave.Enabled = !hasRemote;
            removeSave.Click += async (object sender5, EventArgs e5) =>
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
                    await ReloadUI();
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

        private async Task UpdateButtons()
        {
            var selectingSaves = GetSelectedSaves().Count > 0;
            var server = engine.Server;
            var CanExportAndImport = server is not null && await server.ServerOnline() && selectingSaves;
            exportButton.Enabled = !SelectingRemoteSave() && CanExportAndImport; // Don't want to export remote saves
            importButton.Enabled = CanExportAndImport;
        }

        private async Task Export()
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
                    var success = await engine.ExportSaves(savesToExport.ToArray(), progressBar);
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
                    await ReloadUI();
                }
            }
        }

        private async Task Import()
        {
            try
            {
                List<string> savesToImport = GetSelectedSaves();
                using var progressBar = ProgressBarControl.Start(mainProgressBar, progressLabel, savesToImport.Count);
                var success = await engine.ImportSaves(savesToImport.ToArray(), progressBar);
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
                await ReloadUI();
            }
        }
    }
}