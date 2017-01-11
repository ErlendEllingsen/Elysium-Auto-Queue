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

        public static SelectRealmAlternative srv_1 = new SelectRealmAlternative("Server 1", 605, 250);
        public static SelectRealmAlternative srv_2 = new SelectRealmAlternative("Server 2", 605, 275);
        public static SelectRealmAlternative srv_3 = new SelectRealmAlternative("Server 3", 605, 295);

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
        public int x;
        public int y;

        public SelectRealmAlternative(string name, int x, int y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
        }

    }

}
