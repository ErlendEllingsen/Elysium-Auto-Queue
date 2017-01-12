using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElysiumAutoQueue.Content
{
    class WoWStart
    {

        public SelectRealmAlternative startRealm;

        public WoWStart(SelectRealmAlternative startRealm)
        {
            this.startRealm = startRealm;
        }

        public void start()
        {
            WowExternalRealmSwitcher.switchRealm(this.startRealm);
            this.startProcess();
        }

        private void startProcess()
        {
            //START: WOW 
            Program.wowproc = new Process();
            Program.wowproc.StartInfo = new ProcessStartInfo(ProgramConfig.config.path_wow + "./WoW.exe");
            Program.wowproc.Start();

            System.Threading.Thread.Sleep(8000);

            Program.wow_handle_proc = Program.getName("wow");
            if (Program.wow_handle_proc == IntPtr.Zero)
            {
                Console.WriteLine("Did not find process World of Warcraft");
                Console.ReadLine();
                Environment.Exit(0);
            }

            Program.wow_handle = Program.wow_handle_proc;

            Console.WriteLine("Found process World of Warcraft");

            Program.updateWowRect();

            //SET FOREGROUND
            Program.SetForegroundWindow(Program.wow_handle_proc);
            System.Threading.Thread.Sleep(250);


            //RESIZE THE WINDOW 
            Program.MoveWindow(Program.wow_handle_proc, 0, 0, 1465, 910, true);
            System.Threading.Thread.Sleep(250);

            //FETCH POSITION AGAIN 
            Program.updateWowRect();

            //Is no longer switching realm...
            StateManager.isSwitchingRealm = false; 

        }

    }
}
