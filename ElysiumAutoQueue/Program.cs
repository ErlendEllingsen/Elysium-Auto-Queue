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
using Tesseract;

using ElysiumAutoQueue.Content;

namespace ElysiumAutoQueue
{
    class Program
    {

        #region Win-Calls

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
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;



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
        #endregion

        #region Vars

        //Crucial vars

        public static IntPtr wow_handle;
        public static RECT wow_rect;
        public static Process wowproc;

        public static IntPtr console_handle;

        //Application vars 
        public enum ApplicationModes { normal, cordscan, dev, screenshot };
        public static ApplicationModes applicationMode = ApplicationModes.normal;

        public static List<GameState> gameStates = new List<GameState>();
        public static GameState queueBox, loginScreen, realmSelect, charSelect, waitingDialog, disconnectDialog;

        #endregion

        #region Frequently used functions 

        #region GameFunctions 
        public static bool gameIsGameState(GameState gs)
        {
            //Go through the criterias...
            bool criteriasBroken = false; 

            foreach (GameStateCriteria gsc in gs.criterias)
            {
                //Fetch color from game
                string colorFromGame = gameGetColor(gsc.gameX, gsc.gameY);

                //Check MOUSE criteria
                if (gsc.requiresMouseHover)
                {
                    gameSetMouse(gsc.gameX, gsc.gameY);
                    Point p = gameGetMouse();
                    System.Threading.Thread.Sleep(250);
                    colorFromGame = gameGetColor(p.X, p.Y);
                }

                //Find matching color 
                bool foundCorrectColor = false;
 
                //Loop through accepted colors
                foreach (string s in gsc.acceptedColors)
                {
                    if (ColorLike(hexToColor(s), hexToColor(colorFromGame)))
                    {
                        foundCorrectColor = true;
                        break;
                    }
                }

                string listResult = string.Join(",", gsc.acceptedColors);
                Console.WriteLine(gsc.name + " : " + foundCorrectColor + " (Expected: " + listResult + " Got: " + colorFromGame);

                //Correct color found?
                if (!foundCorrectColor)
                {
                    criteriasBroken = true;
                    break;
                }

                //end criteria loop
            }

            return !criteriasBroken; 

            //end gameIsGameState
        }

        public static Image gameGetImage(int left, int top, int right, int bottom)
        {
            right = (left + (right - left));
            bottom = (top + (bottom - top));

            //Adjust accordingly to the game..
            left = Program.wow_rect.Left + left;
            top = Program.wow_rect.Top + top;
            right = Program.wow_rect.Left + right;
            bottom = Program.wow_rect.Top + bottom;

            RECT rct = new RECT();
            rct.Left = left;
            rct.Top = top;
            rct.Right = right;
            rct.Bottom = bottom;

            return getClientImage(rct);

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
        #endregion

        #region Other freq. functions
        static Image getClientImage(RECT rect)
        {
            Rectangle rct = new Rectangle(rect.Left, rect.Top, (rect.Right - rect.Left), (rect.Bottom - rect.Top));
            Bitmap bmp = new Bitmap(rct.Width, rct.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rct.Left, rct.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            bmp.Save("test.jpeg", System.Drawing.Imaging.ImageFormat.Png);
            return bmp;
        }

        public static Color hexToColor(string s)
        {
            Color color = System.Drawing.ColorTranslator.FromHtml("#" + s);
            return color; 
        }

        public static string colorToHex(Color c)
        {
            string s = c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            return s;
        }

        public static void updateWowRect()
        {
            GetWindowRect(Program.wow_handle, out Program.wow_rect);
        }

        public static string ColorClassify(Color c)
        {
            float hue = c.GetHue();
            float sat = c.GetSaturation();
            float lgt = c.GetBrightness();

            if (lgt < 0.2) return "Blacks";
            if (lgt > 0.8) return "Whites";

            if (sat < 0.25) return "Grays";

            if (hue < 30) return "Reds";
            if (hue < 90) return "Yellows";
            if (hue < 150) return "Greens";
            if (hue < 210) return "Cyans";
            if (hue < 270) return "Blues";
            if (hue < 330) return "Magentas";
            return "Reds";
        }

        public static bool ColorLike(Color a, Color b)
        {
            //h, s, l
            double limit = 0.1; // 10% equality rate
            double minPos = 1 + limit;
            double minNeg = 1 - limit;

            double h = (a.GetHue() / b.GetHue());
            double s = (a.GetSaturation() / b.GetSaturation());
            double l = (a.GetBrightness() / b.GetBrightness());

            bool resH = (h >= minNeg && h <= minPos);
            bool resS = (s >= minNeg && s <= minPos);
            bool resL = (l >= minNeg && l <= minPos);

            //Check for blacks (can't divide by 0).
            if (a.GetHue() == 0 && b.GetHue() == 0) resH = true;
            if (a.GetSaturation() == 0 && b.GetSaturation() == 0) resS = true;
            if (a.GetBrightness() == 0 && b.GetBrightness() == 0) resL = true;

            return (resH && resS && resL);

        }

        #endregion

        #endregion

        #region Main functions 


        public static System.Threading.Timer t;


        static string findGameState()
        {

            //Queuebox
            bool isWaitingDialog = Processes.isWaitingDialog();
            if (isWaitingDialog) return "waiting-dialog";

            bool isDisconnectDialog = Processes.isDisconnected();
            if (isDisconnectDialog) return "disconnect-dialog";

            bool isRealmSelect = Processes.isRealmSelector();
            if (isRealmSelect) return "realm-select";

            bool isQueueBox = Processes.isQueueBox();
            if (isQueueBox) return "queue";

            bool isCharacterSelect = Processes.isCharacterSelector();
            if (isCharacterSelect) return "char-select";
            
            bool isLogin = Processes.isLogin();
            if (isLogin) return "login";

            bool isRealmSetup = Processes.isRealmSetup();
            if (isRealmSetup) return "realm-setup";

            return null;
        }

        #endregion


        public static void Main(string[] args)
        {
            applicationMode = ApplicationModes.dev;

            OutConfig.export();

            IntPtr thisHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;


            wowproc = new Process();
            wowproc.StartInfo = new ProcessStartInfo("D:\\World of Warcraft Classic\\WoW.exe");
            wowproc.Start();

            System.Threading.Thread.Sleep(8000);

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


            t = new System.Threading.Timer(TimerCallback, null, 0, 15000);


            if (applicationMode == ApplicationModes.screenshot)
            {
                Bitmap b = (Bitmap)gameGetImage(0, 0, 1465, 910);
                b.Save("screenshot.png", System.Drawing.Imaging.ImageFormat.Png);
                Console.ReadKey();
                Environment.Exit(0);
            }

            

            if (applicationMode == (ApplicationModes.cordscan))
            {
                while (true)
                {
                    Point p = gameGetMouse();
                    Console.WriteLine(p.X + "," + p.Y + " - " + gameGetColor(p.X, p.Y));
                    System.Threading.Thread.Sleep(1000);
                }
            }





            //705, 574


            //705, 485

            /*
            getClientImage(rect);

            Color c = GetColorAt(50, 0);
            Console.WriteLine("R: " + c.R + " G: " + c.G + " B: " + c.B);

            Console.WriteLine(rect.Left.ToString());
            */

            Console.ReadLine();
            wowproc.Kill();

            //RECT rect = new RECT();
            //GetWindowRect(GetForegroundWindow(), out rect);
        }

        private static void TimerCallback(Object o)
        {
            
            t.Dispose();
            
            if (applicationMode == ApplicationModes.dev)
            {

                string state = findGameState();
                if (state == null)
                {
                    Console.WriteLine("GameState is null");
                }
                else
                {
                    //Console.WriteLine("GameState is " + state);
                    StateManager.updateState(state);
                }

                

                /*
                EXAMPLE OF CAPABILITIES
                if (state == "realm-select")
                {
                    SendKeys.SendWait("{ESC}");
                }
                */


            }

            // Display the date/time when this method got called.
            
            // Force a garbage collection to occur for this demo.
            GC.Collect();

            t = new System.Threading.Timer(TimerCallback, null, 1500, 15000);

        }

    }
}
