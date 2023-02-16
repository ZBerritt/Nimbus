using Xunit;
using static SaveDataSync.Utils.FileUtils;

namespace SaveDataSync.Tests
{
    public class SaveDataSyncEngineTests : IDisposable, IAsyncLifetime
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SaveDataSyncEngine _sut;
        private TemporaryFile dataFile;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public async Task InitializeAsync()
        {
            dataFile = new TemporaryFile();
            _sut = await SaveDataSyncEngine.Start(dataFile.FilePath);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
        public void Dispose() => dataFile.Dispose();

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
        public void DataFileShouldExistOnLoad()
        {
            Assert.True(File.Exists(_sut.DataFile));
        }

        [Fact]
        public async void DataShouldSaveToDataFile()
        {
            // Make changes
            var server = new TestServer();
            await _sut.SetServer(server);
            await _sut.AddSave("testing", "test/test");

            // Save to file
            string oldData = File.ReadAllText(_sut.DataFile);
            await _sut.Save();
            string newData = File.ReadAllText(_sut.DataFile);
            Assert.NotEqual(oldData, newData);
        }

        [Fact]
        public async void LoadShouldLoadFromFile()
        {
            // Make changes
            var oldData = File.ReadAllBytes(_sut.DataFile);
            var server = new TestServer();
            await _sut.SetServer(server);
            await _sut.AddSave("testing", "test/test");

            // Load older version
            File.WriteAllBytes(_sut.DataFile, oldData); // Write old data after it has been overritten
            await _sut.Load();
            Assert.Null(_sut.Server);
            Assert.False(_sut.LocalSaveList.HasSave("testing"));
        }

        // TODO: Add more tests
    }
}
