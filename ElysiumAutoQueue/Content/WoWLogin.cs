﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ElysiumAutoQueue.Content
{
    class WoWLogin
    {
        public static bool loginRunning = false; 

        public static void DoLogin()
        {
            loginRunning = true;
            //--- LOGIN PROCESS ---

            //Fetch username and password
            string username = ProgramConfig.config.login_username;
            string password = ProgramConfig.config.login_password;


            //-- USERNAME -- 
            Program.gameSetMouse(705, 485); //Username field
            System.Threading.Thread.Sleep(150);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);

            //Empty if content is there..
            SendKeys.SendWait("{BACKSPACE}");

            System.Threading.Thread.Sleep(500);
            SendKeys.SendWait(username);

            System.Threading.Thread.Sleep(500);

            //Switch from username to password-field
            SendKeys.SendWait("{TAB}");

            //-- PASSWORD --
            Program.gameSetMouse(705, 574);
            System.Threading.Thread.Sleep(500);

            Program.mouse_event(Program.MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);

            SendKeys.SendWait("{BACKSPACE}");
            System.Threading.Thread.Sleep(500);
            SendKeys.SendWait(password);

            //Login button
            Program.gameSetMouse(705, 643);
            System.Threading.Thread.Sleep(500);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            Program.mouse_event(Program.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
        }

    }
}
