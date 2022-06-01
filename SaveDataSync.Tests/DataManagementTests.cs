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

            // Save to data directory
            DataManagement.SaveLocalSaveList(dataDir, list);

            // Load from directory
            var list2 = DataManagement.GetLocalSaveList(dataDir);
            Assert.AreEqual(list.GetSavePath("test"), list2.GetSavePath("test"));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            Directory.Delete(dataDir, true);
        }
    }
}