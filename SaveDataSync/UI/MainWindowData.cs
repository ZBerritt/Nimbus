using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    /// <summary>
    /// Represents the data for the main window UI.
    /// </summary>
    internal class MainWindowData
    {
        // Server Status
        public string ServerType { get; set; } = "None";

        public string ServerStatus { get; set; } = "N/A";

        public Color ServerColor
        {
            get
            {
                return ServerStatus switch
                {
                    "Online" => Color.Green,
                    "Offline" => Color.DarkGoldenrod,
                    "Error" => Color.Red,
                    _ => Color.Black,
                };
            }
        }

        public string ServerHost { get; set; } = "N/A";

        // Save list

        public List<ListViewItem> saveList = new();

        public MainWindowData(SaveDataSyncEngine engine)
        {
            /* Check server status */
            var server = engine.Server;
            var serverOnline = false;
            if (server is not null)
            {
                ServerType = server.Name;
                ServerHost = server.Host;
                serverOnline = server.ServerOnline();
                try
                {
                    ServerStatus = serverOnline ? "Online" : "Offline";
                }
                catch (Exception)
                {
                    ServerStatus = "Error";
                }
            }

            /* Get a list of the data for the table */
            var saves = engine.LocalSaves.Saves;
            foreach (var save in saves)
            {
                var saveItem = new ListViewItem(save.Key)
                {
                    UseItemStyleForSubItems = false
                };
                saveItem.SubItems.Add(save.Value);

                // Get file size
                var fileSize = File.Exists(save.Value) || Directory.Exists(save.Value)
                    ? FileUtils.ReadableFileSize(FileUtils.GetSize(save.Value))
                    : "N/A";
                saveItem.SubItems.Add(fileSize);

                // Get file sync status
                var statusItem = new ListViewItem.ListViewSubItem(saveItem, "");
                if (serverOnline && (File.Exists(save.Value) || Directory.Exists(save.Value)))
                {
                    var localHash = engine.GetLocalHash(save.Key);
                    var remoteHash = engine.GetRemoteHash(save.Key);
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
                else if (!File.Exists(save.Value) && !Directory.Exists(save.Value))
                {
                    statusItem.Text = "No Local Save";
                    statusItem.ForeColor = Color.Gray;
                }
                else if (server is not null)
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

                // Add to the list
                saveList.Add(saveItem);
            }

            /* Add remote saves to the list */
            if (server is not null && serverOnline)
            {
                var remoteSaveNames = server.SaveNames();
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
                    saveList.Add(remoteSaveItem);
                }
            }
        }

        public static MainWindowData GetMainWindowData(SaveDataSyncEngine engine)
        {
            MainWindowData data;
            data = new MainWindowData(engine);
            return data;
        }
    }
}