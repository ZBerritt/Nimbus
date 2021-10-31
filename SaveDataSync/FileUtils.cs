using System;
using System.Collections.Generic;
using System.IO;

namespace SaveDataSync
{
    internal class FileUtils
    {
        public sealed class TemporaryFile : IDisposable
        {
            public TemporaryFile() :
              this(Path.GetTempPath())
            { }

            public TemporaryFile(string directory)
            {
                Create(Path.Combine(directory, Path.GetRandomFileName()));
            }

            ~TemporaryFile()
            {
                Delete();
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

        public static string[] GetFileList(string directory)
        {
            List<string> files = new List<string>();
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
    }
}
