using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace DiscordEvolved
{
    //This is where I stick annoying self pointing data because of static/non-static constraints
    public class Narcissism
    {
        public string DiscordToken { get; private set; }
        public ulong OwnerId { get; private set; }


        public Narcissism()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            // next, let's load the values from that file
            // to our client's configuration
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            DiscordToken = cfgjson.Token;
            OwnerId = cfgjson.OwnerId;
        }


        // this structure will hold data from config.json
        public struct ConfigJson
        {
            [JsonProperty("token")]
            public string Token { get; private set; }

            [JsonProperty("OwnerID")]
            public ulong OwnerId { get; private set; }
        }
    }
}
