using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync.Tests
{
    [TestClass]
    public class DataManagementTests
    {
        private static string dataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            Directory.CreateDirectory(dataDir);
        }

        [TestMethod("Local Save List - Save/Load")]
        public void DataManagementTests_LocalSaveListTests()
        {
            LocalSaveList list = new LocalSaveList();
            var testFile = Path.GetTempFileName();
            list.AddSave("test", testFile);

            // Save to invalid path
            Assert.ThrowsException<Exception>(() => DataManagement.SaveLocalSaveList("invalid_path", list));

            // Load empty data
            Assert.ThrowsException<Exception>(() => DataManagement.GetLocalSaveList(dataDir));

            // Save to data directory
            DataManagement.SaveLocalSaveList(dataDir, list);

            // Load from directory
            var list2 = DataManagement.GetLocalSaveList(dataDir);
            Assert.AreEqual(list.GetSavePath("test"), list2.GetSavePath("test"));
        }

        [TestMethod("Servers - Save/Load")]
        public void DataManagementTests_ServerTests()
        {
            Server server = new DropboxServer("random data", "doesn't matter", DateTime.Now, "we're not testing server functionlity");

            // Save to invalid path
            Assert.ThrowsException<Exception>(() => DataManagement.SaveServerData("invalid_path", server));

            // Load empty data
            Assert.ThrowsException<Exception>(() => DataManagement.GetServerData(dataDir));

            // Save to data directory
            DataManagement.SaveServerData(dataDir, server);

            // Load from directory
            Server server2 = DataManagement.GetServerData(dataDir);
            Assert.AreEqual(server, server2);
        }

        [TestMethod("Settings - Save/Load")]
        public void DataManagementTests_Settings()
        {
            Settings settings = new Settings();

            // Save to invalid path
            Assert.ThrowsException<Exception>(() => DataManagement.SaveSettings("invalid_path", settings));

            // Load empty data
            Assert.ThrowsException<Exception>(() => DataManagement.GetSettings(dataDir));

            // Save to data directory
            DataManagement.SaveSettings(dataDir, settings);

            // Load from directory
            Settings settings2 = DataManagement.GetSettings(dataDir);
            Assert.AreEqual(settings, settings2);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            Directory.Delete(dataDir, true);
        }
    }
}