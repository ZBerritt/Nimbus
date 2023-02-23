using NimbusApp.Utils;

namespace NimbusApp.Models
{
    /// <summary>
    /// Represents a single local save file or folder
    /// </summary>
    public class Save
    {
        public string Name { get; set; }
        public string Location { get => Location; set => FileUtils.Normalize(value); }

        public Save(string name, string location)
        {
            Name = name;
            Location = location;
        }
    }
}
