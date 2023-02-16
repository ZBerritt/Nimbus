using Newtonsoft.Json;
using SaveDataSync.Models;
using SaveDataSync.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

// TODO: Handle large files
namespace SaveDataSync.Controllers
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
                if (FileUtils.NotAFile(normalizedPath) && locNormalizedPath.Contains(normalizedPath) || FileUtils.NotAFile(locNormalizedPath) && normalizedPath.Contains(locNormalizedPath))
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

            await using var archiveStream = new FileStream(destinationFile, FileMode.Create,
                FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true);
            var pathBase = FileUtils.IsDirectory(location) ? name : "";
            foreach (var file in FileUtils.GetFileList(location))
            {
                string fileEntryName = Path.Combine(pathBase, Path.GetRelativePath(location, file));
                var entry = archive.CreateEntry(fileEntryName, CompressionLevel.Optimal);

                // Make sure the files write times are equal
                entry.LastWriteTime = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);

                using var entryStream = entry.Open();
                await using var fileStream = new FileStream(file, FileMode.Open,
                    FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                await fileStream.CopyToAsync(entryStream);
            }
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

            using var arciveStream = new FileStream(source, FileMode.Open,
                FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
            using var archive = new ZipArchive(arciveStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                if (destination.EndsWith("\\")) // Temporary fix for splitting files and folders. May not work...
                {
                    var entryFileName = entry.FullName[(name.Length + 1)..];
                    var entryDestination = Path.Combine(destination, entryFileName);
                    await FileUtils.ExtractEntry(entryDestination, archive, entry);
                    continue;
                }

                // File -> single entry -> file IS the destination
                await FileUtils.ExtractEntry(destination, archive, entry);
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