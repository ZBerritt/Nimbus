using NimbusApp.Controllers;
using NimbusApp.Models.Servers;
using NimbusApp.Servers;
using NimbusApp.Utils;
using System;
using System.Windows.Forms;

namespace NimbusApp.UI
{
    internal partial class ServerSettings : Form
    {
        public NimbusAppEngine engine;
        public Server server;
        public bool ShouldReload { get; private set; } = false;

        private DropboxServer _dropboxServer;
        private WebDAVServer _webDAVServer;
        private FileServer _fileServer;

        public ServerSettings(NimbusAppEngine engine)
        {
            this.engine = engine;
            server = engine.Server;
            InitializeComponent();
            if (server is not null)
            {
                switch (server)
                {
                    case DropboxServer:
                        _dropboxServer = server as DropboxServer;
                        DropboxReloadUI();
                        break;
                    case WebDAVServer:
                        _webDAVServer = server as WebDAVServer;
                        WebDAVReloadUI();
                        break;
                    case FileServer:
                        _fileServer = server as FileServer;
                        FileReloadUI();
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
                        engine.Server = _dropboxServer;
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

                            engine.Server = _webDAVServer;
                        }
                        catch (Exception ee)
                        {
                            PopupDialog.ErrorPopup("The server could not be added." +
                                " Please validate your inputs before trying again.\n" +
                                $"Message: {ee.Message}");
                            return;
                        }
                        break;
                    case "file":
                        if (localDirectoryTextBox.Text.Length < 0) {
                            PopupDialog.ErrorPopup("Please specify a directory for the server!");
                            return;
                        }
                        var fileArgs = new string[]
                        {
                            localDirectoryTextBox.Text
                        };
                        _fileServer = await Server.Create<FileServer>(fileArgs) as FileServer;
                        var canAccess = await _fileServer.GetOnlineStatus();
                        if (!canAccess)
                        {
                            PopupDialog.ErrorPopup("The file system directory cannot be accessed and a local file server cannot be created!");
                            return;
                        }
                        engine.Server = _fileServer;
                        break;

                }
                await engine.Save();
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

        private void FileReloadUI()
        {
            if (_fileServer is not null)
            {
                localDirectoryTextBox.Text = _fileServer.Location;
            }
        }

        private void localBrowseButton_Click(object sender, EventArgs e)
        {
            if (localFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                localDirectoryTextBox.Text = localFolderBrowser.SelectedPath;
            }
        }
    }
}