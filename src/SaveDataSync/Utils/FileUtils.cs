using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SaveDataSync.Utils
{
    public class FileUtils
    {

        /// <sumary>
        /// Normalizes a path string by:
        ///  - Getting the full path
        ///  - Using back slashes
        ///  - Adding a back slash to the end of any directories
        /// </sumary>
        public static string Normalize(string path)
        {
            string fullPath = Path.GetFullPath(path.Replace('/', '\\'));
            if (!NotAFile(fullPath))
            {
                return fullPath;
            }

            if (!fullPath.EndsWith("\\"))
            {
                return fullPath + "\\";
            }

            return fullPath;

        }

        // Returns true if either a directory or file exists
        public static bool PathExists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        // Returns true if the path leads to anything but a file (directory or doesn't exist)
        public static bool NotAFile(string path)
        {
            return !PathExists(path)
                || (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public sealed class TemporaryFile : IDisposable
        {
            public TemporaryFile() :
              this(Path.GetTempPath())
            { }

            public TemporaryFile(string directory)
            {
                Create(Path.Combine(directory, Path.GetRandomFileName()));
            }

            public void Dispose()
            {
                Delete();
                GC.SuppressFinalize(this);
            }

            public string FilePath { get; private set; }

            private void Create(string path)
            {
                FilePath = path;
                using (File.Create(FilePath)) { };
            }

            private void Delete()
            {
                if (FilePath == null) return;
                File.Delete(FilePath);
                FilePath = null;
            }
        }

        public sealed class TemporaryFolder : IDisposable
        {
            public TemporaryFolder() :
              this(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()))
            { }

            public TemporaryFolder(string directory)
            {
                Create(Path.Combine(directory, Path.GetRandomFileName()));
            }

            public void Dispose()
            {
                Delete();
                GC.SuppressFinalize(this);
            }

            public string FolderPath { get; private set; }

            private void Create(string path)
            {
                FolderPath = path;
                Directory.CreateDirectory(FolderPath);
            }

            private void Delete()
            {
                if (FolderPath == null) return;
                Directory.Delete(FolderPath, true);
                FolderPath = null;
            }
        }

        public static string[] GetFileList(string directory)
        {
            var files = new List<string>();
            foreach (string f in Directory.GetFiles(directory))
            {
                files.Add(f);
            }
            foreach (string dir in Directory.GetDirectories(directory))
            {
                files.AddRange(GetFileList(dir));
            }

            return files.ToArray();
        }

        // Human readable file sizes (10 B, 30 MB, etc)
        public static string ReadableFileSize(long size)
        {
            string[] sizes = { "B", "kB", "MB", "GB", "TB" };
            int order = 0;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return string.Format("{0:0.##} {1}", size, sizes[order]);
        }

        // Gets the size of a file or recursive directory
        public static long GetSize(string location)
        {
            if (!PathExists(location)) return 0;
            return File.GetAttributes(location).HasFlag(FileAttributes.Directory)
                        ? GetFileList(location).Sum(fi => new FileInfo(fi).Length)
                        : new FileInfo(location).Length;
        }
    }
}