using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ElysiumAutoQueue.Content
{
    class WoWQueue
    {

        

        public static WowServer elysium_pvp = new WowServer("Elysium", "elysium_pvp");
        public static WowServer nostalrius_pvp = new WowServer("Anathema", "anathema_pvp");
        public static WowServer nostalrius_pve = new WowServer("Darrowshire", "darrowshire_pve");
        public static WowServer zethkur = new WowServer("Zeth'Kur", "zethkur_pvp");

        public static List<WowServer> servers = new List<WowServer>() { elysium_pvp, nostalrius_pvp, nostalrius_pve, zethkur};

        public static WowServer findServerByName(string name)
        {

            foreach (WowServer server in servers)
            {
                if (server.name.ToLower().Equals(name.ToLower())) return server;
            }

            return null;

        }

        public static void update(string data)
        {

            bool prerequisites = (data.Contains(" is Full") && data.Contains("Position in queue:") && data.Contains("Estimated time"));
            if (!prerequisites)
            {
                Console.WriteLine("Unable to parse data for WowQueue. Missing crucial data.");
                using (StreamWriter sw = new StreamWriter("./queue_data_invalid.txt"))
                {
                    sw.Write(data);
                }
                return;
            }

            WowServer server = null;

            try {

                string serverName = data.Substring(0, data.IndexOf(" is Full")).Trim();
                server = WoWQueue.findServerByName(serverName);
                if (server == null)
                {
                    Console.WriteLine("Unable to find WoWServer for WowQueue. Input name: " + serverName + ".");
                    StateManager.current_sra.server.update(0, false);
                    return;
                }

                string delim = "Position in queue: ";

                int pos_start = data.IndexOf(delim) + delim.Length;
                int pos_end = data.IndexOf("Estimated time");
                int queueNum = Convert.ToInt32((data.Substring(pos_start, (pos_end - pos_start))));

                Console.WriteLine("Queue number @ '" + serverName + "': " + queueNum);
                server.update(queueNum, true);

            } catch (Exception e)
            {
                if (server != null) server.update(0, false);

                Console.WriteLine("Exception when parsing data for WowQueue.");
                StateManager.current_sra.server.update(0, false);
                return;
            }

            //end update
        }

    }
}
