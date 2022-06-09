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
    internal static class MainWindowData
    {
        public static async Task<ServerStatus> GetServerStatus(SaveDataSyncEngine engine)
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
                var serverOnline = await server.ServerOnline();
                try
                {
                    ServerStatus = serverOnline ? "Online" : "Offline";
                }
                catch (Exception)
                {
                    ServerStatus = "Error";
                }
            }

            return new ServerStatus(ServerStatus, ServerHost, ServerType);
        }

        // TODO: Somehow someway someplace figure out how to get the sync status of each individually and seperate
        public static async Task<List<ListViewItem>> GetLocalServerList(SaveDataSyncEngine engine)
        {
            /* Get a list of the data for the table */
            var saves = engine.LocalSaves.Saves;
            var server = engine.Server;
            var serverOnline = server is not null && await server.ServerOnline();
            var saveList = new List<ListViewItem>();
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
                    var localHash = await engine.GetLocalHash(save.Key);
                    var remoteHash = await engine.GetRemoteHash(save.Key);
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

            return saveList;
        }

        public static async Task<List<ListViewItem>> GetRemoteServerList(SaveDataSyncEngine engine)
        {
            /* Add remote saves to the list */
            var server = engine.Server;
            var serverOnline = await server.ServerOnline();
            var saveList = new List<ListViewItem>();
            if (server is not null && serverOnline)
            {
                var remoteSaveNames = await server.SaveNames(); // THIS TOO!!!
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

            return saveList;
        }
    }

    public class ServerStatus
    {
        public string Status { get; private set; }
        public string Host { get; private set; }
        public string Type { get; private set; }

        public Color Color
        {
            get
            {
                return Status switch
                {
                    "Online" => Color.Green,
                    "Offline" => Color.DarkGoldenrod,
                    "Error" => Color.Red,
                    _ => Color.Black,
                };
            }
        }

        internal ServerStatus(string status, string host, string type)
        {
            Status = status;
            Host = host;
            Type = type;
        }
    }
}