using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ElysiumAutoQueue.Content
{
    class WowExternalRealmSwitcher
    {

        public static bool isSwitching = false;
        public static SelectRealmAlternative realmChangingTo; 

        public static bool switchRealm(SelectRealmAlternative sra) {

            WowExternalRealmSwitcher.realmChangingTo = sra;

            bool success = true;

            //Make sure proc is killed.
            try {
                Program.wowproc.Kill();
            } catch (Exception e) { }

            System.Threading.Thread.Sleep(2000); //Give it a few seconds..

            //Realm List 
            try {
                WowExternalRealmSwitcher.writeRealmList();
            } catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[WowExternalRealmSwitcher] Unable to write realmlist. Crucial!");
                Console.ForegroundColor = ConsoleColor.White;
                success = false;
            }

            //Clear WDB 
            try
            {
                WowExternalRealmSwitcher.clearWDB();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[WowExternalRealmSwitcher] Unable to clear WDB.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return success; //success is true even if WDB failed. Realmlist is important while WDB is not...

        }

        private static void writeRealmList()
        {
            string realmListContent = "" +
            "SET realmList \"logon.elysium-project.org\"" + Environment.NewLine + 
            "SET realmName \"" + WowExternalRealmSwitcher.realmChangingTo.realmlist_name + "\"" + Environment.NewLine +
            "SET accountName \"" + ProgramConfig.config.login_username + "\"";

            using (StreamWriter sw = new StreamWriter(ProgramConfig.config.path_wow + "./realmlist.wtf"))
            {
                sw.Write(realmListContent);
            }

        }

        private static void clearWDB()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(ProgramConfig.config.path_wow + "./WDB/");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

    }
}
