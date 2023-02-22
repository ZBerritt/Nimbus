using NimbusApp.Models;
using NimbusApp.Utils;
using System.Text;
using Xunit;

namespace NimbusApp.Tests
{
    public class LocalSaveListTests
    {
        private readonly LocalSaveList _sut;
        public LocalSaveListTests()
        {
            _sut = new LocalSaveList();
            _sut.AddSave("test", "test/dir");
        }

        [Theory]
        [InlineData("\\invalid characters\\", "test_directory")]
        [InlineData("/invalid characters/", "test_directory")]
        [InlineData("The missile knows where it is at all times. It knows this because it knows where it isn't.",
            "test_directory")]
        [InlineData("test", "test_directory")]
        [InlineData("test2", "test/dir")]
        [InlineData("test2", "test")]
        [InlineData("test2", "test/dir/der")]
        public void InvalidSaveShouldNotAddAndThrowError(string saveName, string saveLocation)
        {
            Assert.Throws<InvalidSaveException>(() => _sut.AddSave(saveName, saveLocation));
            Assert.False(!saveName.Equals("test") && _sut.HasSave(saveName));
        }

        [Fact]
        public void GetSaveShouldRetrieveSaveWithSameName()
        {
            var save = _sut.GetSave("test");
            Assert.True(save.Name.Equals("test"));
        }

        [Fact]
        public void SaveLocationShouldBeNormalized()
        {
            var save = _sut.GetSave("test");
            Assert.True(save.Location.Equals(FileUtils.Normalize("test/dir")));
        }

        [Fact]
        public void GetSaveShouldReturnNullWithInvalidSave()
        {
            Assert.Null(_sut.GetSave("test2"));
        }

        [Fact]
        public void HasSaveShouldReturnTrueIfSaveIsPresent()
        {
            Assert.True(_sut.HasSave("test"));
            Assert.False(_sut.HasSave("test2"));
        }

        [Fact]
        public void RemoveSaveShouldRemoveSave()
        {
            _sut.RemoveSave("test");
            Assert.False(_sut.HasSave("test"));
        }

        [Fact]
        public void SaveListEnumeratorShouldLoopThroughAllSaves()
        {
            _sut.AddSave("test2", "test2/dir");
            _sut.AddSave("test3", "test3/dir");
            _sut.AddSave("test4", "test4/dir");
            _sut.AddSave("test5", "test5/dir");
            int count = 0;
            foreach (Save save in _sut.GetSaveList())
            {
                count++;
            }

            Assert.Equal(5, count);
        }

        [Fact]
        public async Task ArchivedFolderShouldBeDeterministic()
        {
            // Setup the folder structure
            using var saveFolder = new FileUtils.TemporaryFolder();
            var folder1 = Directory.CreateDirectory(Path.Combine(saveFolder.FolderPath, "folder1"));
            var folder2 = Directory.CreateDirectory(Path.Combine(saveFolder.FolderPath, "folder2"));
            var file0 = File.Open(Path.Combine(saveFolder.FolderPath, "file0"), FileMode.Create, FileAccess.Write);
            await file0.WriteAsync(Encoding.ASCII.GetBytes("test_text-0"));
            file0.Close();
            var file1 = File.Open(Path.Combine(folder1.FullName, "file1"), FileMode.Create, FileAccess.Write);
            await file1.WriteAsync(Encoding.ASCII.GetBytes("test_text-1"));
            file1.Close();
            var file2 = File.Open(Path.Combine(folder2.FullName, "file2"), FileMode.Create, FileAccess.Write);
            await file2.WriteAsync(Encoding.ASCII.GetBytes("test_text-2"));
            file2.Close();

            // Add save to list
            _sut.AddSave("test_save", saveFolder.FolderPath);

            // Create the first archive
            using var archiveFile1 = new FileUtils.TemporaryFile();
            await _sut.ArchiveSaveData("test_save", archiveFile1.FilePath);
            Assert.True(FileUtils.GetSize(archiveFile1.FilePath) > 0); // Should be greater than 0 since 0 will always equal 0

            // Clear save folder to prepare for extraction
            var di = new DirectoryInfo(saveFolder.FolderPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            // Extract back to the folder
            await _sut.ExtractSaveData("test_save", archiveFile1.FilePath);

            // Check if all files are the same
            var file0Data = await File.ReadAllTextAsync(file0.Name);
            Assert.Equal("test_text-0", file0Data);
            var file1Data = await File.ReadAllTextAsync(file1.Name);
            Assert.Equal("test_text-1", file1Data);
            var file2Data = await File.ReadAllTextAsync(file2.Name);
            Assert.Equal("test_text-2", file2Data);

            // Archive number 2
            Thread.Sleep(2000); // Sleep in order to force the timestamp to change
            using var archiveFile2 = new FileUtils.TemporaryFile();
            await _sut.ArchiveSaveData("test_save", archiveFile2.FilePath);
            Assert.True(FileUtils.GetSize(archiveFile2.FilePath) > 0);

            // Confirm both zip files are identical
            var archive1Text = File.ReadAllText(archiveFile1.FilePath);
            var archive2Text = File.ReadAllText(archiveFile2.FilePath);
            Assert.Equal(archive1Text, archive2Text);
            var archive1Bytes = File.ReadAllBytes(archiveFile1.FilePath);
            var archive2Bytes = File.ReadAllBytes(archiveFile2.FilePath);
            Assert.Equal(archive1Bytes, archive2Bytes);
        }

        [Fact]
        public async Task ArchivedFileShouldBeDeterministic()
        {
            // Setup the folder structure
            using var saveFile = new FileUtils.TemporaryFile();
            var saveFileStream = File.Open(saveFile.FilePath, FileMode.Create, FileAccess.Write);
            await saveFileStream.WriteAsync(Encoding.ASCII.GetBytes("test_text"));
            saveFileStream.Close();

            // Add save to list
            _sut.AddSave("test_save", saveFile.FilePath);

            // Create the first archive
            using var archiveFile1 = new FileUtils.TemporaryFile();
            await _sut.ArchiveSaveData("test_save", archiveFile1.FilePath);
            Assert.True(FileUtils.GetSize(archiveFile1.FilePath) > 0); // Should be greater than 0 since 0 will always equal 0

            // Delete the original file
            File.Delete(saveFile.FilePath);

            // Extract back to the folder
            await _sut.ExtractSaveData("test_save", archiveFile1.FilePath);

            // Check if file is the same
            var fileData = await File.ReadAllTextAsync(saveFile.FilePath);
            Assert.Equal("test_text", fileData);

            // Archive number 2
            Thread.Sleep(2000); // Sleep in order to force the timestamp to change
            using var archiveFile2 = new FileUtils.TemporaryFile();
            File.WriteAllText(archiveFile2.FilePath, "test text");
            await _sut.ArchiveSaveData("test_save", archiveFile2.FilePath);
            Assert.True(FileUtils.GetSize(archiveFile2.FilePath) > 0);

            // Confirm both zip files are identical
            var archive1Text = File.ReadAllText(archiveFile1.FilePath);
            var archive2Text = File.ReadAllText(archiveFile2.FilePath);
            Assert.Equal(archive1Text, archive2Text);
            var archive1Bytes = File.ReadAllBytes(archiveFile1.FilePath);
            var archive2Bytes = File.ReadAllBytes(archiveFile2.FilePath);
            Assert.Equal(archive1Bytes, archive2Bytes);
        }

        // TODO: Possibly write more tests
    }
}