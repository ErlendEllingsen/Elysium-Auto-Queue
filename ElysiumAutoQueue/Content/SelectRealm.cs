using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElysiumAutoQueue.Content
{
    class SelectRealm
    {

        public static SelectRealmAlternative srv_1 = new SelectRealmAlternative("Server 1", "Elysium PvP", WoWQueue.elysium_pvp, 605, 250);
        public static SelectRealmAlternative srv_2 = new SelectRealmAlternative("Server 2", "Nostalrius PvP", WoWQueue.nostalrius_pvp, 605, 275);
        public static SelectRealmAlternative srv_3 = new SelectRealmAlternative("Server 3", "Nostalrius PvE", WoWQueue.nostalrius_pve, 605, 295);

        //Create references...
        public static SelectRealmAlternative elysium_pvp = srv_1, nostalrius_pvp = srv_2, nostalrius_pve = srv_3; 

        public static SelectRealmAlternative selectedAlternative = null;

        public static void selectAlternative(SelectRealmAlternative sra)
        {
            Console.WriteLine("Selecting alternative " + sra.name);

            SelectRealm.selectedAlternative = sra;

            Program.gameSetMouse(sra.x, sra.y); //Username field
            System.Threading.Thread.Sleep(150);

            Program.mouse_event(Program.MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            System.Threading.Thread.Sleep(50);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);

        }

    }


    class SelectRealmAlternative
    {
        public string name;
        public string realmlist_name;

        public WowServer server;
         
        public int x;
        public int y;

        public SelectRealmAlternative(string name, string realmlist_name, WowServer server, int x, int y)
        {
            this.name = name;
            this.realmlist_name = realmlist_name;

            this.server = server;

            this.x = x;
            this.y = y;
        }

    }

}
