using SaveDataSync.Controllers;
using SaveDataSync.UI;
using SaveDataSync.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveDataSync
{
    /// <summary>
    /// Represents the main GUI window used in the app
    /// </summary>
    [SupportedOSPlatform("windows7.0")]
    public partial class MainWindow : Form
    {
        private SaveDataSyncEngine engine;
        private Task ReloadTask;
        private CancellationTokenSource reloadCancelTokenSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Loading Events
        private async void OnLoad(object sender, EventArgs e) // Can't use Task here since the program wants to return void

        {
            // Set name as debug if the program is in debug mode
#if DEBUG
            Text = "SaveDataSync (DEBUG)";
#endif
            // Grabs the engine which allows communication with the backend
            engine = await SaveDataSyncEngine.Start();

            // Auto sizes the last column of the save list
            saveFileList.Columns[^1].Width = -2;

            // Forces the save list to sort correctly
            saveFileList.ListViewItemSorter = new SaveListSorter();

            // Loads the save list with the imported data from the engine
            await ReloadUI();
        }

        // Used to reload all UI data
        // Most likely prone to errors regarding cancellation, keep watch
        public async Task ReloadUI()
        {
            if (ReloadTask is not null && !ReloadTask.IsCompleted)
            {
                reloadCancelTokenSource.Cancel();
            }
            reloadCancelTokenSource = new CancellationTokenSource();
            ReloadTask = ReloadUITask(reloadCancelTokenSource.Token);
            await ReloadTask;
        }

        public async Task ReloadUITask(CancellationToken cancelToken)
        {
            // Remove all save list items to re-add later
            saveFileList.Items.Clear();

            // Change buttons - prevents invalid export/import errors
            await UpdateButtons();

            /* Get main window data asyncronously */
            try
            {
                var tasks = new MainWindowTasks(this, engine, cancelToken);
                await tasks.CheckServerStatus();
                await Task.WhenAll(tasks.SetServerStatus(),
                    tasks.SetLocalServerList(), tasks.SetRemoteServerList());
            }
            catch (OperationCanceledException) { } // Safely ignore operation cancellations 
        }



        public Label GetServerType() { return type; }
        public Label GetServerHost() { return host; }
        public Label GetServerStatus() { return serverStatus; }

        public ListView GetSaveFileList() { return saveFileList; }

        // Click Events
        private async void NewSaveFile_Click(object sender, EventArgs e)
        {
            var sfw = new SaveFileWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = false
            };
            sfw.ShowDialog();
            if (sfw.ShouldReload) await ReloadUI();
        }

        private async void Settings_Click(object sender, EventArgs e)
        {
            var sw = new SettingsWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            sw.ShowDialog();
            if (sw.ShouldReload) await ReloadUI();
        }

        private async void ServerSettingsBtn_Click(object sender, EventArgs e)
        {
            var ss = new ServerSettings(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            ss.ShowDialog();
            if (ss.ShouldReload) await ReloadUI();
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
            bool serverOnline = server is not null && await server.GetOnlineStatus();

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
                PopupDialog.InfoPopup($"Remote Hash: {remoteHash} (Length: {remoteHash.Length})\n" +
                    $"Local Hash: {localHash} (Length: {localHash.Length})");
            };

#endif

            var goToLocation = menu.Items.Add("Open File Location");
            goToLocation.Enabled = !hasRemote && singleSelected;
            goToLocation.Click += (object sender2, EventArgs e2) =>
            {
                if (!engine.LocalSaveList.HasSave(name)) return;
                string saveLocation = engine.LocalSaveList.GetSave(name).Location;
                if (!FileUtils.PathExists(saveLocation))
                {
                    PopupDialog.WarningPopup("Save location cannot be found!");
                    return;
                }
                Process.Start("explorer.exe", $"/select, \"{saveLocation}\"");
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
                foreach (var saveName in selectedSaves)
                {
                    messageBuilder.Append($"\n• {saveName}");
                };

                var confirm = PopupDialog.QuestionPrompt(messageBuilder.ToString(), "Confirm");
                if (confirm == DialogResult.Yes)
                {
                    foreach (var save in selectedSaves)
                    {
                        engine.LocalSaveList.RemoveSave(save);
                    }

                    await engine.Save();
                    await ReloadUI();
                }
            };

            return menu;
        }

        private void Label2_Click(object sender, EventArgs e)
        {
            var url = "http://" + host.Text;
            OtherUtils.OpenUrl(url);
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
            var CanExportAndImport = server is not null && await server.GetOnlineStatus() && selectingSaves;
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
                        PopupDialog.ErrorPopup("Cannot export remote saves. Aborting!");
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
                        PopupDialog.InfoPopup(successMessage.ToString());
                    }
                }
                catch (Exception ex)
                {
                    PopupDialog.ErrorPopup("An erorr has occured: " + ex.Message);
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
                    PopupDialog.InfoPopup(successMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                PopupDialog.ErrorPopup("An erorr has occured: " + ex.Message);
            }
            finally
            {
                await ReloadUI();
            }
        }

        private async void ReloadDataButton_Click(object sender, EventArgs e)
        {
            await ReloadUI();
        }
    }
}