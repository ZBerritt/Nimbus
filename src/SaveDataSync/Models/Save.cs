using SaveDataSync.Utils;

namespace SaveDataSync.Models
{
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
