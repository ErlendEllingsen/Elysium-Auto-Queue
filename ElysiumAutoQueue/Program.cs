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
        #endregion

        #region Vars

        //Crucial vars

        public static IntPtr wow_handle;
        public static RECT wow_rect;

        public static IntPtr console_handle;

        //Application vars 
        public enum ApplicationModes { normal, cordscan, dev };
        public static ApplicationModes applicationMode = ApplicationModes.normal;

        public static List<GameState> gameStates = new List<GameState>();
        public static GameState queueBox;

        #endregion

        #region Frequently used functions 

        #region GameFunctions 
        static bool gameIsGameState(GameState gs)
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
                    if (s == colorFromGame)
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

        static Image gameGetImage(int left, int top, int right, int bottom)
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
            bmp.Save("test.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return bmp;
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

        #endregion

        #endregion

        #region Main functions 
        static void setupGameStates()
        {

            //Login GameState 

            //Queue box GameState
            queueBox = new GameState("QueueBox");
            queueBox.criterias.Add(new GameStateCriteria("BoxTopLeft", 440, 375, "474744,8C5422,8D5422,8B5422"));
            queueBox.criterias.Add(new GameStateCriteria("BoxBottomRight", 1026, 553, "5E5C5A,151210"));
            queueBox.criterias.Add(new GameStateCriteria("BtnChangeRealm", 654, 521, "B20C04,B20D04,300200", true));





        }
        #endregion


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

            setupGameStates();

            applicationMode = ApplicationModes.dev;

            if (applicationMode == ApplicationModes.dev)
            {
                bool gameIsQueueBox = gameIsGameState(queueBox);
                Console.WriteLine("Is QueueBox?: " + gameIsQueueBox);

                if (gameIsQueueBox)
                {
                    Bitmap bmp = (Bitmap)gameGetImage(440, 375, 1026, 553);
                    Bitmap bmp2 = new Bitmap(bmp);

                    

                    //Scan the image 
                    for (var x = 1; x < bmp.Width; x++)
                    {
                        for (var y = 1; y < bmp.Height; y++)
                        {

                            Color fromOrig = bmp.GetPixel(x, y);
                            string result = ColorClassify(fromOrig);

                            if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && fromOrig.GetBrightness() >= 0.3)
                            {
                                Console.WriteLine(fromOrig.GetBrightness());
                                bmp2.SetPixel(x, y, Color.White);
                            } else
                            {
                                bmp2.SetPixel(x, y, Color.Black);
                            }

                        }
                    }

                    bmp2.Save("testttt.jpeg");

                    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile("./testttt.jpeg"))
                        {
                            using (var page = engine.Process(img))
                            {
                                var text = page.GetText();
                                Console.WriteLine(text);
                            }
                        }
                    }

                                

                }

                

                Console.ReadLine();
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
