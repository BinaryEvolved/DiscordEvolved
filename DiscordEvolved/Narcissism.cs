using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DiscordEvolved
{
    //This is where I stick annoying self pointing data because of static/non-static constraints
    public class Narcissism
    {
        public string DiscordToken { get; private set; }

        public void LoadData()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            // next, let's load the values from that file
            // to our client's configuration
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            DiscordToken = cfgjson.Token;
        }



        // this structure will hold data from config.json
        public struct ConfigJson
        {
            [JsonProperty("token")]
            public string Token { get; private set; }
        }
    }
}
