using Newtonsoft.Json.Linq;

namespace SaveDataSync
{
    internal class Settings
    {
        public Settings()
        {

        }

        public Settings(JObject json)
        {

        }

        public Settings Clone()
        {
            return (Settings)MemberwiseClone();
        }

        public JObject ToJSON()
        {
            var json = new JObject();
            return json;
        }
    }
}
