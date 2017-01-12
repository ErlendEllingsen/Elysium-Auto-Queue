using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ElysiumAutoQueue.Content
{
    class ProgramConfig
    {

        public static ProgramConfig config = new ProgramConfig();

        public string login_username = "";
        public string login_password = "";
        public string password_autoqueue = "";

        public string path_wow = "";

        public string endpoint_live = "";
        public string endpoint_dev = "";
        public bool is_production;


        public static void loadConfig()
        {
            try {

                using (StreamReader sr = new StreamReader("./config.json"))
                {
                    string json = sr.ReadToEnd();
                    ProgramConfig.config = JsonConvert.DeserializeObject<ProgramConfig>(json);
                }

                ProgramConfig.configLoaded();

            } catch (IOException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ProgramConfig] Unable to load config.json. Exiting");
                Console.ForegroundColor = ConsoleColor.White;
                Environment.Exit(0);
            }

        }

        public static void configLoaded()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ProgramConfig] Config loaded.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string getEndpoint()
        {
            return (is_production ? endpoint_live : endpoint_dev);
        }

    }

}
