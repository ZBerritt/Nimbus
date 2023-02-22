using NimbusApp.Controllers;
using NimbusApp.Models;
using NimbusApp.Models.Servers;
using Xunit;
using static NimbusApp.Utils.FileUtils;

namespace NimbusApp.Tests
{
    public class NimbusAppEngineTests : IDisposable, IAsyncLifetime
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private NimbusAppEngine _sut;
        private TemporaryFile dataFile;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public async Task InitializeAsync()
        {
            dataFile = new TemporaryFile();
            _sut = await NimbusAppEngine.Load(dataFile.FilePath);
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
            Assert.True(File.Exists(dataFile.FilePath));
        }

        [Fact]
        public async void DataShouldSaveToDataFile()
        {
            // Make changes
            var server = new TestServer();
            await _sut.SetServer(server);
            await _sut.AddSave("testing", "test/test");

            // Save to file
            string oldData = File.ReadAllText(dataFile.FilePath);
            await _sut.Save(dataFile.FilePath);
            string newData = File.ReadAllText(dataFile.FilePath);
            Assert.NotEqual(oldData, newData);
        }

        [Fact]
        public async void LoadShouldLoadFromFile()
        {
            // Make changes
            var oldData = File.ReadAllBytes(dataFile.FilePath);
            var server = new TestServer();
            await _sut.SetServer(server);
            await _sut.AddSave("testing", "test/test");

            // Load older version
            File.WriteAllBytes(dataFile.FilePath, oldData); // Write old data after it has been overritten
            _sut = await NimbusAppEngine.Load(dataFile.FilePath);
            Assert.Null(_sut.Server);
            Assert.False(_sut.LocalSaveList.HasSave("testing"));
        }

        // TODO: Add more tests
    }
}
