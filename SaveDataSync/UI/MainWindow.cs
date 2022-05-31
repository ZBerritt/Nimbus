using SaveDataSync.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
            // Grabs the engine which allows communication with the backend
            engine = SaveDataSyncEngine.CreateInstance();

            // Auto sizes the last column of the save list
            saveFileList.Columns[saveFileList.Columns.Count - 1].Width = -2;

            // Forces the save list to sort correctly
            saveFileList.ListViewItemSorter = new SaveListSorter();

            // Loads the save list with the imported data from the engine
            ReloadUI();
        }

        //Used to reload all UI data
        public void ReloadUI()
        {
            /* Check server status */
            var server = engine.GetServer();
            bool serverOnline = false; // Defaulting to false, shouldn't matter though.
            string serverType = "None";
            string status = "N/A";
            Color statusColor = Color.Black;
            string serverHost = "N/A";
            if (server != null)
            {
                serverType = server.Name();
                serverHost = server.Host();
                try
                {
                    serverOnline = server.ServerOnline();
                    status = serverOnline ? "Online" : "Offline";
                    statusColor = serverOnline ? Color.Green : Color.DarkGoldenrod;
                }
                catch (Exception)
                {
                    status = "Error";
                    statusColor = Color.Red;
                }
            }

            // Set the text of the server information
            type.Text = serverType;
            host.Text = serverHost;
            serverStatus.Text = status;
            serverStatus.ForeColor = statusColor;

            /* Reload the save file list */
            saveFileList.Items.Clear();
            var saves = engine.GetLocalSaveList().GetSaves();
            foreach (var save in saves)
            {
                ListViewItem saveItem = new ListViewItem(save.Key);
                saveItem.UseItemStyleForSubItems = false;
                saveItem.SubItems.Add(save.Value);

                // Get file size
                if (!File.Exists(save.Value) && !Directory.Exists(save.Value))
                {
                    saveItem.SubItems.Add("N/A");
                }
                else
                {
                    FileAttributes attr = File.GetAttributes(save.Value);
                    long saveSize = attr.HasFlag(FileAttributes.Directory)
                        ? new DirectoryInfo(save.Value).EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length)
                        : new FileInfo(save.Value).Length; // The size of the file/folder in bytes
                    string readableSize = FileUtils.ReadableFileSize(saveSize);
                    saveItem.SubItems.Add(readableSize);
                }

                // Get file sync status
                ListViewItem.ListViewSubItem statusItem = new ListViewItem.ListViewSubItem(saveItem, "");
                if (serverOnline && (File.Exists(save.Value) || Directory.Exists(save.Value)))
                {
                    var localSaveData = engine.GetLocalSaveList().GetSaveZipData(save.Key);
                    var remoteHash = engine.GetServer().GetRemoteSaveHash(save.Key);
                    var localHash = engine.GetServer().GetLocalSaveHash(localSaveData);
                    if (remoteHash == null)
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
                else if (!File.Exists(save.Value) && !Directory.Exists(save.Value))
                {
                    statusItem.Text = "No Local Save";
                    statusItem.ForeColor = Color.Gray;
                }
                else if (server != null)
                {
                    statusItem.Text = "Offline";
                    statusItem.ForeColor = Color.DarkGoldenrod;
                }
                else
                {
                    statusItem.Text = "No Server";
                    statusItem.ForeColor = Color.Black;
                }
                saveItem.SubItems.Add(statusItem);

                // Add to the table
                saveFileList.Items.Add(saveItem);
            }

            /* Add remove save files */
            if (server != null && serverOnline)
            {
                var remoteSaveNames = server.SaveNames();
                var filtered = remoteSaveNames.Where(c => !new List<string>(engine.GetLocalSaveList().GetSaves().Keys).Contains(c)).ToList();
                foreach (var save in filtered)
                {
                    ListViewItem saveItem = new ListViewItem(save);
                    saveItem.ForeColor = Color.DarkRed;
                    saveItem.SubItems.Add("Remote");
                    saveItem.SubItems.Add("N/A");
                    saveItem.SubItems.Add("On Server");
                    saveFileList.Items.Add(saveItem);
                }
            }
        }

        // Click Events
        private void NewSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileWindow sfw = new SaveFileWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = false
            };
            sfw.ShowDialog();
            ReloadUI();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            sw.ShowDialog();
            ReloadUI();
        }

        private void serverSettingsBtn_Click(object sender, EventArgs e)
        {
            ServerSettings ss = new ServerSettings(engine)
            {
                Owner = this,
                ShowInTaskbar = true
            };
            ss.ShowDialog();
            ReloadUI();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Export");
            //engine.ExportSaves();
        }

        private void Import_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Import");
            //engine.ImportSaves();
        }

        private void SaveFileList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selectedItem = saveFileList.FocusedItem;
                if (selectedItem == null) return;
                SaveFileContextMenu(selectedItem.Text).Show(saveFileList, new Point(e.X, e.Y));
            }
        }

        private ContextMenuStrip SaveFileContextMenu(string name)
        {
            var menu = new ContextMenuStrip();
            bool hasRemote = SelectingRemoteSave();

            // BEGIN TESTING

            if (!hasRemote)
            {
                var getHashes = menu.Items.Add("[DEBUG] Get Hashes");
                getHashes.Click += (object sender, EventArgs e) =>
                {
                    var selected = GetSelectedSaves(true);
                    var first = selected[0]; // I don't care I just want the first one
                    var localSaveData = engine.GetLocalSaveList().GetSaveZipData(first);
                    var remoteHash = engine.GetServer().GetRemoteSaveHash(first);
                    var localHash = engine.GetServer().GetLocalSaveHash(localSaveData);
                    MessageBox.Show("Remote Hash: " + remoteHash +
                        " (Length: " + remoteHash.Length + ")\nLocal Hash: " + localHash +
                        " (Length: " + remoteHash.Length + ")",
                               "Debug",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                };
            }
            // END TESTING

            if (!hasRemote)
            {
                var goToLocation = menu.Items.Add("Open File Location");
                goToLocation.Click += (object sender2, EventArgs e2) =>
                {
                    try
                    {
                        string savePath = engine.GetLocalSaveList().GetSavePath(name);
                        Process.Start("explorer.exe", string.Format("/select, \"{0}\"", savePath));
                    }
                    catch (Exception) { }
                };
            }

            if (!hasRemote)
            {
                var quickExport = menu.Items.Add("Quick Export");
                quickExport.Click += (object sender3, EventArgs e3) =>
                {
                    try
                    {
                        List<string> savesToExport = GetSelectedSaves(true);
                        using (var progressBar = ProgressBarControl.Start(mainProgressBar, progressLabel, savesToExport.Count))
                        {
                            var success = engine.ExportSaves(savesToExport.ToArray(), progressBar);
                            if (success.Length != 0)
                            {
                                MessageBox.Show("Successfully Exported:\n- " + string.Join("\n- ", success),
                                "Success!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            }
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
                };
            }

            var quickImport = menu.Items.Add("Quick Import");
            quickImport.Click += (object sender4, EventArgs e4) =>
            {
                try
                {
                    List<string> savesToImport = GetSelectedSaves(false);
                    using (var progressBar = ProgressBarControl.Start(mainProgressBar, progressLabel, savesToImport.Count))
                    {
                        var success = engine.ImportSaves(savesToImport.ToArray(), progressBar);
                        if (success.Length != 0)
                        {
                            MessageBox.Show("Successfully Imported:\n- " + string.Join("\n- ", success),
                                "Success!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
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
            };

            if (!hasRemote)
            {
                var removeSave = menu.Items.Add("Remove Local Save");
                removeSave.Click += (object sender5, EventArgs e5) =>
                {
                    var confirm = MessageBox.Show("Are you sure you want to remove this save file?",
                        "Confirm",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        engine.GetLocalSaveList().RemoveSave(name);
                        engine.Save();
                        ReloadUI();
                    }
                };
            }
            return menu;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            try
            {
                var url = "http://" + host.Text;
                Process.Start(url);
            }
            catch (Exception) { }
        }

        private List<string> GetSelectedSaves(bool noRemote)
        {
            var selected = saveFileList.SelectedItems;
            List<string> saves = new List<string>();
            foreach (ListViewItem item in selected)
            {
                if (noRemote && item.SubItems[1].Text == "Remote") throw new Exception("Remote save detected");
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
    }
}