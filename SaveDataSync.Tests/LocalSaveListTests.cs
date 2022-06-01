using System.Security.Cryptography;

namespace SaveDataSync.Tests
{
    [TestClass]
    public class LocalSaveListTests
    {
        private static LocalSaveList localSaves = new LocalSaveList();
        private static string testPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            // Create test directory
            Directory.CreateDirectory(testPath);

            // Add single test files
            var testFile1 = Path.GetTempFileName();
            File.WriteAllText(testFile1, "foo");
            var testFile2 = Path.GetTempFileName();
            File.WriteAllText(testFile1, "bar");
            localSaves.AddSave("test_file1", testFile1);
            localSaves.AddSave("test_file2", testFile2);

            // Add test folders
            var testFolder1 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(testFolder1);
            var tmpTestFile1_1 = Path.Combine(testFolder1, Path.GetRandomFileName());
            File.WriteAllText(tmpTestFile1_1, "Hello");
            var tmpTestFile1_2 = Path.Combine(testFolder1, Path.GetRandomFileName());
            File.WriteAllText(tmpTestFile1_2, "World");
            var testFolder2 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(testFolder2);
            var tmpTestFile2_1 = Path.Combine(testFolder2, Path.GetRandomFileName());
            File.WriteAllText(tmpTestFile2_1, "Hi");
            var tmpTestFile2_2 = Path.Combine(testFolder2, Path.GetRandomFileName());
            File.WriteAllText(tmpTestFile2_2, "Mom");
            localSaves.AddSave("test_folder1", testFolder1);
            localSaves.AddSave("test_folder2", testFolder2);
        }

        [TestMethod("Name Validation Functionality")]
        public void LocalSaveList_NameValidationWorks()
        {
            // Dupe file name
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("test_file1", testPath));
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("test_folder1", testPath));

            // Dupe folder
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("test_file3", localSaves.GetSavePath("test_file1")));
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("test_folder3", localSaves.GetSavePath("test_folder1")));

            // Contains current folder
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("temp_directory", Path.GetTempPath()));

            // Illegal characters
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("\\ /", testPath));

            // Path that doesn't exist
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("fake_folder", "not/a/path"));

            // Longer than 32 chars
            Assert.ThrowsException<Exception>(() => localSaves.AddSave("Lorem ipsum dolor sit amet fusce.", testPath));
        }

        [TestMethod("Save Management Tests")]
        public void LocalSaveList_SaveManagementTests()
        {
            localSaves.AddSave("testing", testPath);
            var saves = localSaves.GetSaves();
            Assert.AreEqual(saves["testing"], testPath);
            localSaves.RemoveSave("testing");

            // Delete a file that doesn't exist
            Assert.ThrowsException<Exception>(() => localSaves.RemoveSave("testing"));

            // Get save path of a file that doesn't exist
            Assert.ThrowsException<Exception>(() => localSaves.GetSavePath("testing"));

            // Get zip data of a file that doesn't exist
            Assert.ThrowsException<Exception>(() => localSaves.GetSaveZipData("testing"));
        }

        [TestMethod("Json Test")]
        public void LocalSaveList_JsonTest()
        {
            var json = localSaves.ToJson();
            var fromJson = LocalSaveList.FromJson(json);
            var json2 = fromJson.ToJson();
            Assert.AreEqual(json, json2);
        }

        [TestMethod("Zip is Deterministic")]
        public void LocalSaveList_ZipIsDeterministic()
        {
            var sha256 = SHA256.Create();

            // File test
            var data1 = localSaves.GetSaveZipData("test_file1");
            var hash1 = sha256.ComputeHash(data1);
            var hex1 = BitConverter.ToString(hash1, 0, hash1.Length).Replace("-", "").ToLower();
            Thread.Sleep(2000);
            var data2 = localSaves.GetSaveZipData("test_file1");
            var hash2 = sha256.ComputeHash(data2);
            var hex2 = BitConverter.ToString(hash2, 0, hash2.Length).Replace("-", "").ToLower();
            Assert.AreEqual(hex1, hex2);

            // Folder test
            var data3 = localSaves.GetSaveZipData("test_folder1");
            var hash3 = sha256.ComputeHash(data3);
            var hex3 = BitConverter.ToString(hash3, 0, hash3.Length).Replace("-", "").ToLower();
            Thread.Sleep(2000);
            var data4 = localSaves.GetSaveZipData("test_folder1");
            var hash4 = sha256.ComputeHash(data4);
            var hex4 = BitConverter.ToString(hash4, 0, hash4.Length).Replace("-", "").ToLower();
            Assert.AreEqual(hex3, hex4);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var saveLocations = localSaves.GetSaves().Values.ToList();
            foreach (var location in saveLocations)
            {
                FileAttributes attr = File.GetAttributes(location);
                bool isDirectory = attr.HasFlag(FileAttributes.Directory);
                if (isDirectory)
                {
                    Directory.Delete(location, true);
                }
                else
                {
                    File.Delete(location);
                }
            }

            Directory.Delete(testPath, true);
        }
    }
}