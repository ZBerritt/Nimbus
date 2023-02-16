using SaveDataSync.Controllers;
using SaveDataSync.Models.Servers;
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    [SupportedOSPlatform("windows7.0")]
    internal partial class ServerSettings : Form
    {
        public SaveDataSyncEngine engine;
        public Server server;
        public bool ShouldReload { get; private set; } = false;

        private DropboxServer _dropboxServer;

        public ServerSettings(SaveDataSyncEngine engine)
        {
            this.engine = engine;
            this.server = engine.Server;
            InitializeComponent();
            if (server is not null)
            {
                var serverName = server.Type;
                switch (serverName)
                {
                    case "Dropbox":
                        _dropboxServer = server as DropboxServer;
                        DropboxReloadUI();
                        break;
                }
            }
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                switch (settingsTabs.SelectedTab.AccessibleName) // Active tab settings will be saved
                {
                    case "dropbox":
                        var serverOnline = await _dropboxServer.GetOnlineStatus();
                        if (!serverOnline) throw new Exception("Server cannot be found or is not online!");
                        await engine.SetServer(_dropboxServer);
                        break;
                }
                ShouldReload = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void loginWithDropboxButton_Click(object sender, EventArgs e)
        {
            var dropboxServer = new DropboxServer();
            await dropboxServer.Build();
            _dropboxServer = dropboxServer;
            DropboxReloadUI();
        }

        private void DropboxReloadUI()
        {
            if (_dropboxServer is null)
            {
                dropboxLoginNotice.Text = "❌ Not logged into Dropbox";
            }
            else
            {
                dropboxLoginNotice.Text = $"✔️ Logged in as user: {_dropboxServer.Uid}";
            }
        }
    }
}