using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static SaveDataSync.Utils.FileUtils;

namespace SaveDataSync.Tests
{
    public class SaveDataSyncEngineTests : IDisposable
    {
        private readonly SaveDataSyncEngine _sut;
        private readonly TemporaryFolder dataFolder;

        public SaveDataSyncEngineTests() {
            dataFolder = new TemporaryFolder();
            _sut = SaveDataSyncEngine.Start(dataFolder.FolderPath).Result; // Asynchronous method needs to be called in constructor
        }

        [Fact]
        public void IntanceShouldEqualCurrentEngine()
        {
            Assert.Equal(_sut, SaveDataSyncEngine.Instance);
        }

        [Fact]
        public async Task SetLocalSaveListShouldChangeSaveList()
        {
            var list = new LocalSaveList();
            await _sut.SetLocalSaveList(list);
            Assert.Equal(list, _sut.LocalSaveList);
        }

        [Fact]
        public async Task SetSettingsShouldChangeSettings()
        {
            var settings = new Settings();
            await _sut.SetSettings(settings);
            Assert.Equal(settings, _sut.Settings);
        }

        [Fact]
        public async Task SetServerShouldChangeServer()
        {
            var server = new TestServer();
            await _sut.SetServer(server);
            Assert.Equal(server, _sut.Server);
        }

        [Fact]
        public void DataFolderShouldBePopulated()
        {
            var folder = dataFolder.FolderPath;
            var dataManager = new DataManager(folder); // Just used to get file paths
            Assert.True(File.Exists(dataManager.LocalSavesFile));
            Assert.True(File.Exists(dataManager.SettingsFile));
            Assert.False(File.Exists(dataManager.ServerFile)); // No server
        }

        [Fact]
        public async Task ServerFileShouldExistWhenAdded()
        {
            var server = new TestServer();
            await _sut.SetServer(server);
            var dataManager = new DataManager(dataFolder.FolderPath);
            Assert.True(File.Exists(dataManager.ServerFile));
        }

        // TODO: Add more tests

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dataFolder.Dispose();
        }
    }
}
