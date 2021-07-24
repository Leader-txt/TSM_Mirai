
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace TSM_Mirai
{
    class Config
    {
        public string Host { get; set; } = "127.0.0.1";
        public short Port { get; set; } = 8080;
        public string AuthKey { get; set; } = "";
        public static long[] BlackLists { get; set; } = new long[0];
        public long BotQQ { get; set; }
        static string configPath = "Config/TSM.json";
        public long[] Admin { get; set; }
        public long[] Groups { get; set; }
        public Server[] Servers { get; set; }
        public static Config GetConfig()
        {
            if (!File.Exists(configPath))
            {
                Directory.CreateDirectory("Config");
                using (StreamWriter wr = new StreamWriter(configPath))
                {
                    wr.Write(JsonConvert.SerializeObject(new Config()
                    {
                        Admin = new long[0],
                        Servers = new Server[1] {new Server()
                        {
                            IP="127.0.0.1",
                            RestPort =7878,
                            Token="",
                            Note =""
                        } }
                    }, Formatting.Indented));
                }
            }
            Config config;
            using (StreamReader reader = new StreamReader(configPath))
            {
                config = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
            }
            return config;
        }
    }
    public class Server
    {
        public string IP { get; set; }
        public int RestPort { get; set; }
        public string Token { get; set; }
        public string Note { get; set; }
        public string Name
        {
            get
            {
                try
                {
                    var info = Utils.GetHttp("http://" + IP + ":" + RestPort + "/status");
                    return (info["name"].ToString() == "" ? info["world"].ToString() : info["name"].ToString() + $"({info["world"]})");
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
