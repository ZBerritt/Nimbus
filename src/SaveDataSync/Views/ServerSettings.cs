using SaveDataSync.Controllers;
using SaveDataSync.Models.Servers;
using SaveDataSync.Servers;
using SaveDataSync.Utils;
using System;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    internal partial class ServerSettings : Form
    {
        public SaveDataSyncEngine engine;
        public Server server;
        public bool ShouldReload { get; private set; } = false;

        private DropboxServer _dropboxServer;
        private WebDAVServer _webDAVServer;

        public ServerSettings(SaveDataSyncEngine engine)
        {
            this.engine = engine;
            server = engine.Server;
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
                    case "WebDAV":
                        _webDAVServer = server as WebDAVServer;
                        WebDAVReloadUI();
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
                        if (!serverOnline)
                        {
                            PopupDialog.ErrorPopup("Server cannot be found or is not online!");
                            return;
                        }
                        await engine.SetServer(_dropboxServer);
                        break;

                    case "webdav":
                        try
                        {
                            var args = new string[] {
                                webDavUrlInput.Text, webDavUsernameInput.Text, webDavPasswordInput.Text };
                            _webDAVServer = await Server.Create<WebDAVServer>(args) as WebDAVServer;
                            var canConnect = await _webDAVServer.GetOnlineStatus();
                            if (!canConnect)
                            {
                                PopupDialog.ErrorPopup("Cannot connect or authenticate to WebDAV server.");
                                return;
                            }

                            await engine.SetServer(_webDAVServer);
                        }
                        catch (Exception ee)
                        {
                            PopupDialog.ErrorPopup("The server could not be added." +
                                " Please validate your inputs before trying again.\n" +
                                $"Message: {ee.Message}");
                            return;
                        }
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
            var dropboxServer = await Server.Create<DropboxServer>(Array.Empty<string>()) as DropboxServer;
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

        private void WebDAVReloadUI()
        {
            if (_webDAVServer is not null)
            {
                webDavUsernameInput.Text = _webDAVServer.Username.ToString();
                webDavUrlInput.Text = _webDAVServer.Uri.ToString();
            }
        }
    }
}