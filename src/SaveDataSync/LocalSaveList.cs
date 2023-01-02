using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveDataSync.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// TODO: Handle large files
namespace SaveDataSync
{
    /// <summary>
    /// Represents all stored and managed local saves
    /// </summary>
    public class LocalSaveList : IEnumerable<Save>
    {
        private static readonly int MAX_FILE_SIZE = 1024 * 1024 * 128; // 128 mb
        Dictionary<string, Save> Saves { get; } = new Dictionary<string, Save>();

        /// <summary>
        /// Adds a save to the list if the save is valid
        /// </summary>
        /// <param name="name">The name of the save</param>
        /// <param name="location">The location of the save</param>
        /// <exception cref="InvalidSaveException">The save is invalid and cannot be added</exception>
        /// <exception cref="SaveTooLargeException">The save file is too large and cannot be added</exception>
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

            foreach (var save in Saves.Values)
            {
                var loc = save.Location;
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

            Saves[name] = new Save(name, location); // Always add the save using the normalized path to avoid errors
        }

        /// <summary>
        /// Gets a save file of the specified name
        /// </summary>
        /// <param name="name">The name of the save to get</param>
        /// <returns>The save with the given name</returns>
        public Save GetSave(string name)
        {
            return Saves.GetValueOrDefault(name);
        }

        /// <summary>
        /// Determines if the list contains a save of a given name
        /// </summary>
        /// <param name="name">The name of the save</param>
        /// <returns>A boolean representing whether the list contains the save</returns>
        public bool HasSave(string name)
        {
            return Saves.ContainsKey(name);
        }

        /// <summary>
        /// Removes a save with a given name
        /// </summary>
        /// <param name="name">The name of the save</param>
        public void RemoveSave(string name)
        {
            if (!Saves.ContainsKey(name)) return;
            Saves.Remove(name);
        }

        // TODO: Store original permissions/date and put them back when extracting
        /// <summary>
        /// Archies a given save data to zip format
        /// </summary>
        /// <param name="name">The name of the save to arcive</param>
        /// <param name="destinationFile">The file to write the data to</param>
        /// <returns>Task representing asynchronous operation</returns>
        /// <exception cref="Exception">Throws if archive cannot be created</exception>
        public async Task ArchiveSaveData(string name, string destinationFile)
        {
            if (!HasSave(name)) throw new Exception("Cannot archive save data: save does not exist");
            string location = GetSave(name).Location;

            // This entire mess basically just zips the file into what we need. We need to do it manually isntead of using fastzip

            using var outputStream = File.OpenWrite(destinationFile);
            using var zipOutputStream = new ZipOutputStream(outputStream);

            FileAttributes attr = File.GetAttributes(location);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                string[] files = FileUtils.GetFileList(location); // Recursively get all files
                foreach (string file in files)
                {
                    string fileEntryName = Path.Combine(name, file[location.Length..]);
                    await FileUtils.AddToArchive(file, fileEntryName, zipOutputStream); // Adds all entries to archive
                }
                return;
            }

            // Single file - create archive of one entry
            string entryName = Path.GetFileName(location);
            await FileUtils.AddToArchive(location, entryName, zipOutputStream);
        }

        /// <summary>
        /// Extracts save data from zip file and writes to save location
        /// </summary>
        /// <param name="name">The name of the save</param>
        /// <param name="source">The location of the source zip file</param>
        /// <returns>Task representing asynchronous operation</returns>
        /// <exception cref="Exception">Throws if archive cannot be extracted</exception>
        public async Task ExtractSaveData(string name, string source)
        {
            if (!File.Exists(source)) throw new Exception("Source file does not exist.");
            if (!HasSave(name)) return;
            var destination = GetSave(name).Location;

            using var fileInputStream = File.OpenRead(source);
            using var zipInputStream = new ZipInputStream(fileInputStream);

            while (zipInputStream.GetNextEntry() is ZipEntry zipEntry)
            {
                if (destination.EndsWith("\\")) // Temporary fix for splitting files and folders. May not work...
                {
                    var entryFileName = zipEntry.Name[(name.Length + 1)..];
                    var entryDestination = Path.Combine(destination, entryFileName);
                    await FileUtils.Extract(entryDestination, zipInputStream, zipEntry);
                    continue;
                }

                // File -> single entry -> file IS the destination
                await FileUtils.Extract(destination, zipInputStream, zipEntry);
            }
        }

        /// <summary>
        /// Converts the save list to JSON format
        /// </summary>
        /// <returns>The JSON represenation of the save list</returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(Saves.Values);
        }

        /// <summary>
        /// Builds a local save list from a json string
        /// </summary>
        /// <param name="json">The serialized JSON string</param>
        /// <returns>A deserialized local save list</returns>
        public static LocalSaveList Deserialize(string json)
        {
            var deserializedJson = JsonConvert.DeserializeObject<List<Save>>(json);
            var list = new LocalSaveList();
            var failed = new List<string>();
            foreach (var save in deserializedJson)
            {
                try
                {
                    list.AddSave(save.Name, save.Location);
                }
                catch (InvalidSaveException)
                {
                    failed.Add(save.Name);
                }
            }

            if (failed.Count > 0)
            {
                var message = "The following saves were deemed invalid and were removed:";
                failed.ForEach(x => message += "\n• " + x);
                PopupDialog.ErrorPopup(message);
            }

            return list;
        }

        public IEnumerator<Save> GetEnumerator()
        {
            return Saves.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}