using SaveDataSync.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    internal class MainWindowTasks
    {
        private bool _serverOnline;
        private readonly SaveDataSyncEngine _engine;
        private readonly MainWindow _window;
        private readonly CancellationToken _cancelToken;
        public MainWindowTasks(MainWindow window, SaveDataSyncEngine engine, CancellationToken cancelToken)
        {
            _window = window;
            _engine = engine;
            _cancelToken = cancelToken;
        }

        /**
         * Gets the server online status 
         * MUST be run before any tasks can complete
         */
        public async Task CheckServerStatus()
        {
            _cancelToken.ThrowIfCancellationRequested();
            _serverOnline = _engine.Server is not null && await _engine.Server.GetOnlineStatus();
        }

        public Task SetServerStatus()
        {
            var server = _engine.Server;
            string ServerType = "N/A";
            string ServerHost = "N/A";
            string ServerStatus = "None";
            // Check server status
            if (server is not null)
            {
                ServerType = server.Name;
                ServerHost = server.Host;
                ServerStatus = _serverOnline ? "Online" : "Offline";
            }

            var StatusColor = ServerStatus switch
            {
                "Online" => Color.Green,
                "Offline" => Color.DarkGoldenrod,
                "Error" => Color.Red,
                _ => Color.Black,
            };
            _cancelToken.ThrowIfCancellationRequested();

            // Assign values to main window
            _window.GetServerType().Text = ServerType;
            _window.GetServerHost().Text = ServerHost;
            _window.GetServerStatus().Text = ServerStatus;
            _window.GetServerStatus().ForeColor = StatusColor;

            // Send completion signal
            return Task.CompletedTask;
        }

        public async Task SetLocalServerList()
        {
            /* Get a list of the data for the table */
            var saves = _engine.LocalSaves.Saves;
            foreach (var save in saves)
            {
                _cancelToken.ThrowIfCancellationRequested();
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
                _window.GetSaveFileList().Items.Add(saveItem);
            }

            await SetLocalSaveStatuses();
        }

        private async Task SetLocalSaveStatuses()
        {
            foreach (ListViewItem item in _window.GetSaveFileList().Items)
            {
                _cancelToken.ThrowIfCancellationRequested();
                var saveName = item.SubItems[0].Text;

                var foundSave = _engine.LocalSaves.Saves.TryGetValue(saveName, out string location);
                if (!foundSave) return; // Ignore if its a remote save

                var statusItem = item.SubItems[^1];

                if (_serverOnline && (File.Exists(location) || Directory.Exists(location)))
                {
                    var localHash = await _engine.GetLocalHash(saveName);
                    var remoteHash = await _engine.GetRemoteHash(saveName);
                    _cancelToken.ThrowIfCancellationRequested(); // Cancel before modifying the UI in any way
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
                else if (_engine.Server is not null)
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

        public async Task SetRemoteServerList()
        {
            // Add remote saves to the list
            var server = _engine.Server;
            var saveList = new List<ListViewItem>();
            if (_serverOnline)
            {
                var remoteSaveNames = await server.SaveNames();
                var filtered = remoteSaveNames.Where(c => !_engine.LocalSaves.Saves.ContainsKey(c));
                foreach (var s in filtered)
                {
                    _cancelToken.ThrowIfCancellationRequested();
                    var remoteSaveItem = new ListViewItem(s)
                    {
                        ForeColor = Color.DarkRed
                    };
                    remoteSaveItem.SubItems.Add("Remote");
                    remoteSaveItem.SubItems.Add("N/A");
                    remoteSaveItem.SubItems.Add("On Server");
                    _window.GetSaveFileList().Items.Add(remoteSaveItem);
                }
            }
        }
    }
}