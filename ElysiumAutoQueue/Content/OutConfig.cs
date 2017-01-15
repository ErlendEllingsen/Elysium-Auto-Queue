using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ElysiumAutoQueue.Content
{



    class OutConfig
    {

        public static OutConfigData config = new OutConfigData();
        public static string outputJson = null;

        public static string export()
        {

            //Set time
            config.export_time = DateTime.Now;

            //Is login server unreliable? 
            config.loginServerUnreliable = WaitingIncidentMonitor.isLogonUnstable();
            config.waiting_incidents = WaitingIncidentMonitor.incidents;

            //Prepare config
            config.prepare();

            outputJson = JsonConvert.SerializeObject(config);

            try {
                //Write final
                using (StreamWriter sw = new StreamWriter("./out.json"))
                {
                    sw.Write(outputJson);
                }
            } catch (IOException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("IO failure Outconfig.export");
                Console.ForegroundColor = ConsoleColor.White;
            }

            //Return output
            return outputJson;
        }
        

    }

    class OutConfigData
    {

        public DateTime export_time;

        public bool loginServerUnreliable = false;
        public List<DateTime> waiting_incidents = new List<DateTime>();

        public Dictionary<string, WowServer> servers = new Dictionary<string, WowServer>();

        public void prepare()
        {
            this.servers.Clear();
                
            foreach (WowServer ws in WoWQueue.servers)
            {
                servers.Add(ws.elysiumStatusIdentifier, ws);
            }

        }

    }
}
