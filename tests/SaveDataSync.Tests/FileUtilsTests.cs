using Xunit;
using static SaveDataSync.Utils.FileUtils;

namespace SaveDataSync.Tests
{
    public class FileUtilsTests
    {
        // Root directory used for normalizing
        private static readonly string root = AppContext.BaseDirectory;
        public FileUtilsTests()
        {
        }

        [Theory]
        [MemberData(nameof(NormalizeTestData))]
        public void NormalizeShouldNormalizeFilePath(string input, string expected)
        {
            var normalized = Normalize(input);
            Assert.Equal(expected, normalized);
        }

        public static IEnumerable<object[]> NormalizeTestData()
        {
            yield return new object[] { "test", $"{root}test\\" };
            yield return new object[] { "test/test", $"{root}test\\test\\" };
            // TODO: Add more cases
        }

        [Fact]
        public void NotAFileShouldReturnTrueWhenDirectory()
        {
            Assert.True(NotAFile(root));
        }

        [Fact]
        public void NotAFileShouldReturnFalseWhenFile()
        {
            using var file = new TemporaryFile();
            Assert.False(NotAFile(file.FilePath));
            file.Dispose();
        }

        [Fact]
        public void NotAFileShouldReturnTrueWhenNothing()
        {
            Assert.True(NotAFile("fnknoibwoefibfoewiwf"));
        }

        [Fact]
        public void TemporaryFileShouldDeleteOnDispose()
        {
            using var file = new TemporaryFile();
            var filePath = file.FilePath;
            Assert.True(File.Exists(filePath));
            file.Dispose();
            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void TemporaryFolderShouldDeleteOnDispose()
        {
            using var folder = new TemporaryFolder();
            var folderPath = folder.FolderPath;
            Assert.True(Directory.Exists(folderPath));
            folder.Dispose();
            Assert.False(Directory.Exists(folderPath));
        }

        [Fact]
        public void GetFileListShouldRecursivelyRetrieveFilesInFolder()
        {
            // Setup
            using var folder = new TemporaryFolder();
            Directory.CreateDirectory(Path.Combine(folder.FolderPath, "test_folder1"));
            Directory.CreateDirectory(Path.Combine(folder.FolderPath, "test_folder2"));
            using var file0 = File.Create(Path.Combine(folder.FolderPath, "file0"));
            using var file00 = File.Create(Path.Combine(folder.FolderPath, "file00"));
            using var file1 = File.Create(Path.Combine(folder.FolderPath, "test_folder1", "file1"));
            using var file11 = File.Create(Path.Combine(folder.FolderPath, "test_folder1", "file11"));
            using var file2 = File.Create(Path.Combine(folder.FolderPath, "test_folder2", "file2"));
            using var file22 = File.Create(Path.Combine(folder.FolderPath, "test_folder2", "file22"));

            // Assertions
            Assert.Equal(6, GetFileList(folder.FolderPath).ToList().Count);
            // TODO: Possibly check each file
        }

        [Theory]
        [InlineData(10, "10 B")]
        [InlineData(1023, "1023 B")]
        [InlineData(1024, "1 kB")]
        [InlineData(69 * 1024, "69 kB")]
        [InlineData(1024 * 1024, "1 MB")]
        [InlineData(1000 * 1024 * 1024, "1000 MB")]
        [InlineData(1024 * 1024 * 1024, "1 GB")]
        public void ReadableFileSizeConvertsToShortenedFileSizes(long fileSize, string expected)
        {
            string readable = ReadableFileSize(fileSize);
            Assert.Equal(expected, readable);
        }

        [Fact]
        public void GetSizeOfFileGetsTotalFileSize()
        {
            // Setup 
            var file = new TemporaryFile();
            File.WriteAllBytes(file.FilePath, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            long fileSize = GetSize(file.FilePath);
            Assert.Equal(10, fileSize);
        }

        [Fact]
        public void GetSizeOfDirectoryGetsTotalSizeOfAllFiles()
        {
            // Setup
            using var folder = new TemporaryFolder();
            Directory.CreateDirectory(Path.Combine(folder.FolderPath, "test_folder1"));
            Directory.CreateDirectory(Path.Combine(folder.FolderPath, "test_folder2"));
            using (var file0 = File.Create(Path.Combine(folder.FolderPath, "file0")))
            using (var file1 = File.Create(Path.Combine(folder.FolderPath, "test_folder1", "file1")))
            using (var file2 = File.Create(Path.Combine(folder.FolderPath, "test_folder2", "file2")))
            {
                for (int i = 0; i < 10; i++) // All files have 10 bytes written, 30 total
                {
                    file0.WriteByte(1);
                    file1.WriteByte(1);
                    file2.WriteByte(1);
                }
            }
            long folderSize = GetSize(folder.FolderPath);
            Assert.Equal(30, folderSize);
        }

        [Fact]
        public void GetSizeOfEmptyFolderShouldBeZero()
        {
            using var folder = new TemporaryFolder();
            long folderSize = GetSize(folder.FolderPath);
            Assert.Equal(0, folderSize);
        }

        [Fact]
        public void GetSizeOfEmptyFileShouldBeZero()
        {
            using var file = new TemporaryFile();
            long fileSize = GetSize(file.FilePath);
            Assert.Equal(0, fileSize);
        }
    }
}
