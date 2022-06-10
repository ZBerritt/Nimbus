using Newtonsoft.Json.Linq;

namespace SaveDataSync
{
    public class Settings
    {
        public Settings()
        {
        }

        public Settings(JObject json)
        {
        }

        public Settings Clone()
        {
            return MemberwiseClone() as Settings;
        }

        public JObject ToJSON()
        {
            var json = new JObject();
            return json;
        }
    }
}