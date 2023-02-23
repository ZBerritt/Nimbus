using NimbusApp.Utils;

namespace NimbusApp.Models
{
    /// <summary>
    /// Represents a single local save file or folder
    /// </summary>
    public class Save
    {
        private string _name;
        private string _location;

        public string Name { get => _name; set => _name = value; }
        public string Location { 
            get => _location;
            set => _location = FileUtils.Normalize(value); }

        public Save(string name, string location)
        {
            Name = name;
            Location = location;
        }
    }
}
