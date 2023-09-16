using NimbusApp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

// TODO: Handle large files
namespace NimbusApp.Models
{
    /// <summary>
    /// Represents all stored and managed local saves
    /// </summary>
    public class LocalSaveList
    {
        private static readonly int BUFFER_SIZE = 4096;
        private static readonly int MAX_FILE_SIZE = 1024 * 1024 * 128; // 128 mb
        public Dictionary<string, Save> Saves { get; set; }

        public LocalSaveList()
        {
            Saves = new Dictionary<string, Save>();
        }

        public List<Save> GetSaveList() => Saves.Values.ToList();

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
                throw new InvalidSaveException("Save names must be shorter than 32 characters.");

            // Duplicate names
            if (Saves.ContainsKey(name))
                throw new InvalidSaveException($"Save with name {name} already exists.");

            foreach (var save in Saves.Values)
            {
                var loc = save.Location;
                var locNormalizedPath = FileUtils.Normalize(loc);

                // Same path exists
                if (locNormalizedPath.Equals(normalizedPath))
                    throw new InvalidSaveException($"Save game with name {location} already exists.");

                // Path contains one another
                if (FileUtils.NotAFile(normalizedPath) && locNormalizedPath.Contains(normalizedPath)
                    || FileUtils.NotAFile(locNormalizedPath) && normalizedPath.Contains(locNormalizedPath))
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
            var found = Saves.TryGetValue(name, out Save save);
            if (!found)
            {
                return null;
            }

            return save;
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
                FileAccess.Write, FileShare.None, bufferSize: BUFFER_SIZE, useAsync: true);
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
                    FileAccess.Read, FileShare.Read, bufferSize: BUFFER_SIZE, useAsync: true);
                await fileStream.CopyToAsync(entryStream);
            }

            // Add comment declaring archive type
            archive.Comment = FileUtils.IsDirectory(location) ? "folder" : "file";
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
                FileAccess.Read, FileShare.None, bufferSize: BUFFER_SIZE, useAsync: true);
            using var archive = new ZipArchive(arciveStream, ZipArchiveMode.Read);

            // Determine file type
            var isFolder = archive.Comment != null
                ? archive.Comment == "folder" // Auto match, works best if the user doesn't do anything stupid
                : destination.EndsWith("\\"); // Use legacy match, might be less reliable but it works

            // Some error checking in case tampering was done. May be useless but its better to have
            if (!isFolder && archive.Entries.Count > 1)
            {
                throw new Exception("Trying to extract file archive with multiple entries. " +
                    "If you're seeing this and didn't tamper with the remote files, something went wrong.");
            }

            if (isFolder)
            {
                foreach (var entry in archive.Entries)
                {
                    var entryFileName = entry.FullName[(name.Length + 1)..];
                    var entryDestination = Path.Combine(destination, entryFileName);
                    await ExtractEntry(entryDestination, entry);
                }

                return;
            }

            await ExtractEntry(destination, archive.Entries[0]); // File should have 1 entry
        }

        /// <summary>
        /// Extracts a zip entry to the desired folder
        /// </summary>
        /// <param name="destination">The base destination folder</param>
        /// <param name="zipArchive">The zip archive</param>
        /// <param name="zipEntry">The zip entry</param>
        /// <returns>A task for the asynchronous operation</returns>
        private static async Task ExtractEntry(string destination, ZipArchiveEntry zipEntry)
        {
            // Handle as directory
            if (zipEntry.FullName.EndsWith("/") && !Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
                return;
            }

            // Create directory just in case
            Directory.CreateDirectory(Path.GetDirectoryName(destination));

            // Write to file
            using var destinationStream = File.Open(destination, FileMode.OpenOrCreate, FileAccess.Write);
            await using var entryStream = zipEntry.Open();
            await entryStream.CopyToAsync(destinationStream);

        }
    }
}