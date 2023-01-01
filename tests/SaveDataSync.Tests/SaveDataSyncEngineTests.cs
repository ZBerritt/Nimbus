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
        private readonly TemporaryFile dataFile;

        public SaveDataSyncEngineTests() {
            dataFile = new TemporaryFile();
            _sut = SaveDataSyncEngine.Start(dataFile.FilePath).Result; // Asynchronous method needs to be called in constructor
        }

        [Fact(Timeout = 5000)]
        public void IntanceShouldEqualCurrentEngine()
        {
            Assert.Equal(_sut, SaveDataSyncEngine.Instance);
        }

        [Fact(Timeout = 5000)]
        public async Task SetLocalSaveListShouldChangeSaveList()
        {
            var list = new LocalSaveList();
            await _sut.SetLocalSaveList(list);
            Assert.Equal(list, _sut.LocalSaveList);
        }

        [Fact(Timeout = 5000)]
        public async Task SetSettingsShouldChangeSettings()
        {
            var settings = new Settings();
            await _sut.SetSettings(settings);
            Assert.Equal(settings, _sut.Settings);
        }

        [Fact(Timeout = 5000)]
        public async Task SetServerShouldChangeServer()
        {
            var server = new TestServer();
            await _sut.SetServer(server);
            Assert.Equal(server, _sut.Server);
        }

        [Fact(Timeout = 5000)]
        public void DataFileShouldExistOnLoad()
        {
            Assert.True(File.Exists(_sut.DataFile));
            Assert.True(File.ReadAllText(_sut.DataFile).Length > 0);
        }

        [Fact(Timeout = 5000)]
        public async void DataShouldSaveToDataFile()
        {
            // Make changes
            var server = new TestServer();
            await _sut.SetServer(server);
            await _sut.AddSave("testing", "test/test");

            // Save to file
            string oldData = File.ReadAllText(_sut.DataFile);
            await _sut.Save();
            string newData =  File.ReadAllText(_sut.DataFile);
            Assert.NotEqual(oldData, newData);
        }

        [Fact(Timeout = 5000)]
        public async void LoadShouldLoadFromFile()
        {
            // Make changes
            var server = new TestServer();
            await _sut.SetServer(server);
            await _sut.AddSave("testing", "test/test");

            // Load older version
            await _sut.Load();
            Assert.Null(_sut.Server);
            Assert.False(_sut.LocalSaveList.HasSave("testing"));
        }

        // TODO: Add more tests

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dataFile.Dispose();
        }
    }
}
