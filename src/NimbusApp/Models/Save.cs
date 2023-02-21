using NimbusApp.Utils;

namespace NimbusApp.Models
{
    /// <summary>
    /// Represents a single local save file or folder
    /// </summary>
    public class Save
    {
        public string Name { get; }
        public string Location { get; }

        public Save(string name, string location)
        {
            Name = name;
            Location = FileUtils.Normalize(location);
        }
    }
}
