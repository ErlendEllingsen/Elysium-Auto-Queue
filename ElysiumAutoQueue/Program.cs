using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ElysiumAutoQueue
{
    class Program
    {

        //WIN: Finding rect of process...
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        //WIN: Fetching pixels from screen 
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        //WIN: MOVE WINDOW 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        //WIN: Set program to front
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //WIN: SET CURSOR POS
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        //WIN: MOUSE ACTIONS
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;



        static public System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        static IntPtr getName(string wName)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.ProcessName.ToLower().Contains(wName)) hWnd = pList.MainWindowHandle;

            }
            return hWnd;

        }

        #region Vars

        public static IntPtr wow_handle;
        public static RECT wow_rect;

        public static IntPtr console_handle;
         
        #endregion 


        public static string colorToHex(Color c)
        {
            string s = c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            return s;
        }

        static Image getClientImage(RECT rect)
        {
            Rectangle rct = new Rectangle(rect.Left, rect.Top, (rect.Right - rect.Left), (rect.Bottom - rect.Top));
            Bitmap bmp = new Bitmap(rct.Width, rct.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rct.Left, rct.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            bmp.Save("test.jpeg", ImageFormat.Jpeg);
            return bmp;
        }

        public static string gameGetColor(int X, int Y)
        {
            return colorToHex(GetPixelColor(Program.wow_rect.Left + X, Program.wow_rect.Top + Y));
        }

        public static void gameSetMouse(int X, int Y)
        {
            SetCursorPos(Program.wow_rect.Left + X, Program.wow_rect.Top + Y);
        }

        public static Point gameGetMouse()
        {
            Point p = new Point(Cursor.Position.X, Cursor.Position.Y);
            return p;
        }

        
        

        public static void updateWowRect()
        {
            GetWindowRect(Program.wow_handle, out Program.wow_rect);
        }

        static void Main(string[] args)
        {
            IntPtr thisHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            SetCursorPos(1000, 900);
            //Screen.PrimaryScreen.Bounds.Width;
            //Screen.PrimaryScreen.Bounds.Height;
            //Screen.PrimaryScreen.Bounds.Size;

            System.Threading.Thread.Sleep(250);
            MoveWindow(thisHandle, 0, Screen.PrimaryScreen.Bounds.Height - 250, Screen.PrimaryScreen.Bounds.Width, 200, true);

            IntPtr proc = getName("wow");
            if (proc == IntPtr.Zero)
            {
                Console.WriteLine("Did not find process World of Warcraft");
                Console.ReadLine();
                Environment.Exit(0);
            }

            Program.wow_handle = proc;

            Console.WriteLine("Found process World of Warcraft");

            updateWowRect();

            //SET FOREGROUND
            SetForegroundWindow(proc);
            System.Threading.Thread.Sleep(250);


            //RESIZE THE WINDOW 
            MoveWindow(proc, 0, 0, 1465, 910, true);
            //System.Threading.Thread.Sleep(250);

            //FETCH POSITION AGAIN 
            updateWowRect();

            /*
            while (true)
            {
                Point p = gameGetMouse();
                Console.WriteLine(p.X + "," + p.Y + " - " + gameGetColor(p.X, p.Y));
                System.Threading.Thread.Sleep(1000);
            }
            */

            //--- LOGIN PROCESS ---
            
            //-- USERNAME -- 
            gameSetMouse(705, 485); //Username field
            System.Threading.Thread.Sleep(150);
            mouse_event(MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);

            //Empty if content is there..
            SendKeys.SendWait("{BACKSPACE}");

            System.Threading.Thread.Sleep(150);
            SendKeys.SendWait("erlendellingsen");

            System.Threading.Thread.Sleep(500);

            //Switch from username to password-field
            SendKeys.SendWait("{TAB}");

            //-- PASSWORD --
            gameSetMouse(705, 574);
            System.Threading.Thread.Sleep(150);

            mouse_event(MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);

            SendKeys.SendWait("{BACKSPACE}");
            System.Threading.Thread.Sleep(150);
            SendKeys.SendWait("lol123");

            //Login button
            gameSetMouse(705, 643);
            System.Threading.Thread.Sleep(150);
            mouse_event(MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);

            //705, 574


            //705, 485

            /*
            getClientImage(rect);

            Color c = GetColorAt(50, 0);
            Console.WriteLine("R: " + c.R + " G: " + c.G + " B: " + c.B);

            Console.WriteLine(rect.Left.ToString());
            */

            Console.ReadLine();

            //RECT rect = new RECT();
            //GetWindowRect(GetForegroundWindow(), out rect);
        }
    }
}
