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
        public static DateTime stateStart; //Check state length

        //State locks
        public static bool isExitingWaitingDialogs = false, isSettingUpRealm = false;

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
            if (currentState == "realm-select")
            {
                if (SelectRealm.selectedAlternative == null) SelectRealm.selectAlternative(SelectRealm.srv_1);
                else if (SelectRealm.selectedAlternative == SelectRealm.srv_1) SelectRealm.selectAlternative(SelectRealm.srv_2);
                else if (SelectRealm.selectedAlternative == SelectRealm.srv_2) SelectRealm.selectAlternative(SelectRealm.srv_3);
                else if (SelectRealm.selectedAlternative == SelectRealm.srv_3) SelectRealm.selectAlternative(SelectRealm.srv_1);

            }
            else if (currentState== "queue")
            {
                System.Threading.Thread.Sleep(2000);
                SendKeys.SendWait("{ESCAPE}");
                System.Threading.Thread.Sleep(2000);
                
            } else if (currentState == "disconnect-dialog")
            {
                System.Threading.Thread.Sleep(1500);
                SendKeys.SendWait("{ESCAPE}");
                System.Threading.Thread.Sleep(1500);
            }
        }

        private static void TimerCallback(Object o)
        {

            TimeSpan ts = DateTime.Now.Subtract(StateManager.stateStart);
            double secs = ts.TotalSeconds;

            Console.WriteLine("Been in state " + StateManager.currentState + " (prev: " + StateManager.lastState + ") for " + secs + " seconds..");

            //Caluclations

            //Waiting dialogs 
            if (isExitingWaitingDialogs && currentState != "waiting-dialog") isExitingWaitingDialogs = false;
            if (currentState == "waiting-dialog" && secs >= 30 && !isExitingWaitingDialogs)
            {
                isExitingWaitingDialogs = true; 
                SendKeys.SendWait("{ESCAPE}");
                Console.WriteLine("Exiting waiting-dialog (timeout.");
                System.Threading.Thread.Sleep(2000);
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
            if (currentState == "realm-setup" && secs >= 5 && !isSettingUpRealm)
            {
                isSettingUpRealm = true;
                System.Threading.Thread.Sleep(1500);

                Program.gameSetMouse(1144, 336);
                System.Threading.Thread.Sleep(250);
                Program.mouse_event(Program.MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
                Program.mouse_event(Program.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);



            }


            GC.Collect();

            

        }

    }
}
