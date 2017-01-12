using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElysiumAutoQueue.Content
{
    class StateManager
    {
        public static System.Threading.Timer stateWatcherTimer = new System.Threading.Timer(StateManager.TimerCallback, null, 1500, 1500);

        public static string lastState = null, currentState = null;
        public static string latest_reported_state; //used for standby on null etc...

        public static DateTime stateStart; //Check state length


        //State locks
        public static int waitDialogTimeouts = 0;
        public static bool isExitingWaitingDialogs = false, isSettingUpRealm = false, isSwitchingRealmFromCharacterScreen = false;


        //OVERALL LOGIC ADMIN 
        #region MASTER LOGIC 

        public static SelectRealmAlternative current_sra = null;
        public static bool isSwitchingRealm = false;

        public static SelectRealmAlternative getNextSRA(SelectRealmAlternative sra)
        {
            //Fallback
            current_sra = SelectRealm.elysium_pvp;

            //Find realm
            if (sra == SelectRealm.elysium_pvp) current_sra = SelectRealm.nostalrius_pvp;
            if (sra == SelectRealm.nostalrius_pvp) current_sra = SelectRealm.nostalrius_pve;
            if (sra == SelectRealm.nostalrius_pve) current_sra = SelectRealm.elysium_pvp;

            Console.WriteLine("[StateManager] Switching realm to " + current_sra.realmlist_name);

            return current_sra; 
        }


        #endregion 


        public static void updateState(string newState)
        {
            if (currentState != newState)
            {
                StateManager.stateStart = DateTime.Now;
                Console.WriteLine("State has changed. Last state: " + lastState + " new state: " + newState + " (" + stateStart.ToString() + ")");
                lastState = "" + currentState + "";
            }

            //Change state vars 
            
            currentState = "" + newState + "";

            StateManager.stateUpdated();

        }

        public static void stateUpdated()
        {
            if (latest_reported_state == null) return; //Waiting while null

            if (currentState == "realm-select")
            {
                if (SelectRealm.selectedAlternative == null) SelectRealm.selectAlternative(SelectRealm.srv_1);
                else if (SelectRealm.selectedAlternative == SelectRealm.srv_1) SelectRealm.selectAlternative(SelectRealm.srv_2);
                else if (SelectRealm.selectedAlternative == SelectRealm.srv_2) SelectRealm.selectAlternative(SelectRealm.srv_3);
                else if (SelectRealm.selectedAlternative == SelectRealm.srv_3) SelectRealm.selectAlternative(SelectRealm.srv_1);

            }
            else if (currentState== "queue")
            {
                //TODO: Something
                //System.Threading.Thread.Sleep(2000);
                //SendKeys.SendWait("{ESCAPE}");
                //System.Threading.Thread.Sleep(2000);
                
            } else if (currentState == "disconnect-dialog")
            {
                //If disconnect from waiting, report incident
                if (lastState == "waiting-dialog")
                {
                    WaitingIncidentMonitor.addIncident();
                }

                System.Threading.Thread.Sleep(1500);
                SendKeys.SendWait("{ESCAPE}");
                System.Threading.Thread.Sleep(1500);
            }
        }

        private static void TimerCallback(Object o)
        {
            if (latest_reported_state == null) return; //Waiting while null

            //DEBUG
            OutConfig.export();

            TimeSpan ts = DateTime.Now.Subtract(StateManager.stateStart);
            int secs = Convert.ToInt32(Math.Floor(((double)ts.TotalSeconds)));

            if (secs % 5 == 0) Console.WriteLine("Been in state " + StateManager.currentState + " (prev: " + StateManager.lastState + ") for " + secs + " seconds..");

            //Caluclations

            //Queue
            if (currentState == "queue" && secs >= 30 && !isSwitchingRealm)
            {
                isSwitchingRealm = true;

                try
                {
                    Program.wowproc.Kill();
                }
                catch (Exception e) { }

                //Sleep two secs
                System.Threading.Thread.Sleep(2000);

                WoWStart ws = new WoWStart(StateManager.getNextSRA(current_sra));
                ws.start();

            }

            //Waiting dialogs 
            if (isExitingWaitingDialogs && currentState != "waiting-dialog") isExitingWaitingDialogs = false;
            if (currentState == "waiting-dialog" && secs >= 30 && !isExitingWaitingDialogs)
            {
                WaitingIncidentMonitor.addIncident();

                waitDialogTimeouts++;
                isExitingWaitingDialogs = true; 
                SendKeys.SendWait("{ESCAPE}");
                Console.WriteLine("Exiting waiting-dialog (timeout.");
                System.Threading.Thread.Sleep(2000);

                if (waitDialogTimeouts >= 5)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    waitDialogTimeouts = 0;
                    Console.WriteLine("Exiting to login (too many waiting-dialog timeout (prev state: " + lastState + ")).");
                    System.Threading.Thread.Sleep(1000);

                    Program.wowproc.Kill();
                    System.Threading.Thread.Sleep(1000);

                    Console.WriteLine("Restarting wow..");

                    Console.ResetColor();

                    Program.Main(null);

                }

            }

            //Login stuff 
            if (WoWLogin.loginRunning && currentState != "login")
            {
                WoWLogin.loginRunning = false; //Login must have been success?
            }

            if (currentState == "login" && secs >= 10 && !WoWLogin.loginRunning)
            {
                WoWLogin.DoLogin();
                Console.WriteLine("Doing login..");
            }

            //Realm setup....
            if (isSettingUpRealm && currentState != "realm-setup") isSettingUpRealm = false; 
            if (currentState == "realm-setup" && secs >= 8 && !isSettingUpRealm)
            {
                isSettingUpRealm = true;
                WoWRealmSetup.doSetup();
            }

            //Character-select 
            if (currentState == "char-select" && secs >= 20 && !isSwitchingRealm)
            {
                isSwitchingRealm = true;

                current_sra.server.update(0, true);
                
                try
                {
                    Program.wowproc.Kill();
                }
                catch (Exception e) { }

                //Sleep two secs
                System.Threading.Thread.Sleep(2000);

                WoWStart ws = new WoWStart(StateManager.getNextSRA(current_sra));
                ws.start();

            }

            //--- RESTART IF BUGGED ---
            if (secs >= 100 && !isSwitchingRealm) //more than 2 minutes in same state...
            {
                //Report that the realm is bugged...
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[StateManager] State is bugged. (>= 120 secs). Restarting.");
                Console.ForegroundColor = ConsoleColor.White;

                //Set to switching
                isSwitchingRealm = true;

                try
                {
                    Program.wowproc.Kill();
                }
                catch (Exception e) { }

                //Sleep two secs
                System.Threading.Thread.Sleep(2000);

                WoWStart ws = new WoWStart(current_sra); //Retry with same SRA
                ws.start();

            }


            GC.Collect();

            

        }

    }
}
