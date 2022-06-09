using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

// TODO: Handle large files
namespace SaveDataSync
{
    public class LocalSaves
    {
        private static readonly int MAX_FILE_SIZE = 1024 * 1024 * 128; // 128 mb
        public Dictionary<string, string> Saves { get; } = new Dictionary<string, string>();

        public void AddSave(string name, string location)
        {
            // Get normalized path to file
            var normalizedPath = FileUtils.Normalize(location);

            // No slashes
            if (name.Contains('/') || name.Contains('\\'))
                throw new InvalidSaveException("Invalid characters detected! Please do not use slashes (\\ or /).");

            // Name cannot be longer than 32 characters
            if (name.Length > 32)
                throw new InvalidSaveException("Save file names must be shorter than 32 characters.");

            // Duplicate names
            if (Saves.ContainsKey(name))
                throw new InvalidSaveException("Save game with name " + name + " already exists.");

            foreach (var loc in Saves.Values)
            {
                var locNormalizedPath = FileUtils.Normalize(loc);

                // Same path exists
                if (locNormalizedPath.Equals(normalizedPath))
                    throw new InvalidSaveException("Save game with location " + location + " already exists.");

                // Path contains one another
                if ((FileUtils.NotAFile(normalizedPath) && locNormalizedPath.Contains(normalizedPath)) || (FileUtils.NotAFile(locNormalizedPath) && normalizedPath.Contains(locNormalizedPath)))
                    throw new InvalidSaveException("Save locations cannot contain each other.");
            }

            // Save file exceeds max size
            if (FileUtils.GetSize(normalizedPath) > MAX_FILE_SIZE)
                throw new SaveTooLargeException();

            Saves[name] = normalizedPath; // Always add the save using the normalized path to avoid errors
        }

        public void RemoveSave(string name)
        {
            if (!Saves.ContainsKey(name)) throw new Exception("Game list does not contain the save " + name);
            Saves.Remove(name);
        }

        public string GetSavePath(string name)
        {
            if (!Saves.ContainsKey(name)) throw new Exception("Save file with the name " + name + " does not exist.");
            return Saves[name];
        }

        // TODO: Store original permissions/date and put them back when extracting
        public async Task ArchiveSaveData(string name, string destinationFile)
        {
            string location = GetSavePath(name);

            // This entire mess basically just zips the file into what we need. We need to do it manually isntead of using fastzip

            using var outputStream = File.OpenWrite(destinationFile);
            using var zipOutputStream = new ZipOutputStream(outputStream);
            byte[] buffer = new byte[4096];

            FileAttributes attr = File.GetAttributes(location);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                string[] files = FileUtils.GetFileList(location); // Recursively get all files
                foreach (string file in files)
                {
                    using var fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    string entryName = Path.Combine(name, file[location.Length..]);
                    var fileEntry = new ZipEntry(entryName)
                    {
                        DateTime = File.GetCreationTime(file), // Date time uses creation time
                        Size = fileStream.Length
                    };
                    zipOutputStream.PutNextEntry(fileEntry);

                    var fcount = fileStream.Read(buffer, 0, buffer.Length);
                    while (fcount > 0)
                    {
                        await zipOutputStream.WriteAsync(buffer.AsMemory(0, fcount));
                        fcount = fileStream.Read(buffer, 0, buffer.Length);
                    }
                }
                return;
            }

            using var stream = File.Open(location, FileMode.Open, FileAccess.Read, FileShare.Read);
            var entry = new ZipEntry(Path.GetFileName(location))
            {
                DateTime = File.GetCreationTime(location), // Date time uses creation time
                Size = stream.Length
            };
            zipOutputStream.PutNextEntry(entry);

            var count = stream.Read(buffer, 0, buffer.Length);
            while (count > 0)
            {
                await zipOutputStream.WriteAsync(buffer.AsMemory(0, count));
                count = stream.Read(buffer, 0, buffer.Length);
            }
        }

        public async Task ExtractSaveData(string name, string source)
        {
            if (!File.Exists(source)) throw new Exception("Source folder does not exist.");
            var destination = GetSavePath(name);

            // Extract to temporary folder
            using var tmpDir = new FileUtils.TemporaryFolder();
            var tempDir = tmpDir.FolderPath;

            var fastZip = new FastZip();
            fastZip.ExtractZip(source, tempDir, null);

            string[] content = Directory.GetFiles(tempDir, "*.*", SearchOption.TopDirectoryOnly);
            if (content.Length == 0) content = Directory.GetDirectories(tempDir, "*.*", SearchOption.TopDirectoryOnly);
            var saveContent = content[0]; // There should be only one output (file or folder)

            FileAttributes attr = File.GetAttributes(saveContent);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                await ExtractFolder(saveContent, destination);
                return;
            }

            using var inputStream = File.Open(saveContent, FileMode.Open);
            using var outputStream = File.OpenWrite(destination);
            await inputStream.CopyToAsync(outputStream);
        }

        private static async Task ExtractFolder(string source, string destination)
        {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir.Replace(source, destination)); // Add all dirs
            }

            foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                using var inputStream = File.Open(file, FileMode.Open);
                using var outputStream = File.OpenWrite(file.Replace(source, destination));
                await inputStream.CopyToAsync(outputStream);
            }
        }

        public string Serialize()
        {
            var json = new JObject();
            var saves = new JArray();
            foreach (var pair in Saves)
            {
                var saveObject = new JObject
                {
                    { "name", pair.Key },
                    { "location", pair.Value }
                };
                saves.Add(saveObject);
            }
            json.Add("saves", saves);
            return json.ToString();
        }

        public static LocalSaves Deserialize(string json)
        {
            var deserializedJson = JsonConvert.DeserializeObject<JObject>(json);
            var list = new LocalSaves();
            var saves = deserializedJson.GetValue("saves") as JArray;
            foreach (JObject save in saves)
            {
                string name = save.GetValue("name").ToString();
                string location = save.GetValue("location").ToString();

                try
                {
                    list.AddSave(name, location);
                }
                catch (SaveTooLargeException ex)
                {
                    // TODO: Handle large save files from load differently
                    MessageBox.Show($"{ex.Message} Save will be removed locally.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (InvalidSaveException ex)
                {
                    MessageBox.Show($"{ex.Message} Save will be removed locally.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return list;
        }
    }
}