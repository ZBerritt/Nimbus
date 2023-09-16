using NimbusApp.Controllers;
using NimbusApp.Server;
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
            _sut = await NimbusAppEngine.Deserialize(dataFile.FilePath);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
        public void Dispose() => dataFile.Dispose();

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
            _sut.Server = server;
            _sut.AddSave("testing", "test/test");
            await _sut.Serialize(dataFile.FilePath);

            // Save to file
            string oldData = File.ReadAllText(dataFile.FilePath);
            await _sut.Serialize(dataFile.FilePath);
            string newData = File.ReadAllText(dataFile.FilePath);
            Assert.NotEqual(oldData, newData);
        }

        [Fact]
        public async void LoadShouldLoadFromFile()
        {
            // Make changes
            var oldData = File.ReadAllBytes(dataFile.FilePath);
            var server = new TestServer();
            _sut.Server = server;
            _sut.AddSave("testing", "test/test");
            await _sut.Serialize(dataFile.FilePath);

            // Load older version
            File.WriteAllBytes(dataFile.FilePath, oldData); // Write old data after it has been overritten
            _sut = await NimbusAppEngine.Deserialize(dataFile.FilePath);
            Assert.Null(_sut.Server);
            Assert.False(_sut.LocalSaveList.HasSave("testing"));
        }

        // TODO: Add more tests
    }
}
