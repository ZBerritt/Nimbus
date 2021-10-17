using System;
using System.IO;

namespace SaveDataSync
{
    public class Save
    {
        string Name { get; set; }
        string Location { get; set; }

        public Save(string name, string path)
        {
            // Validate name to prevent errors
            if (name == null) throw new ArgumentNullException("name");
            if (name.Length == 0) throw new ArgumentException(name, "name");
            if (path == null) throw new ArgumentNullException("path");
            if (path.Length == 0) throw new ArgumentException(path, "path");
            Name = name;
            Location = Path.GetFullPath(path);
        }


    }
}
