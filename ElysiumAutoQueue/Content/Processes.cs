using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElysiumAutoQueue;
using Tesseract;

namespace ElysiumAutoQueue.Content
{
    public class Processes
    {
        public static bool isLogin()
        {
            Bitmap bmp = (Bitmap)Program.gameGetImage(635, 440, 850, 685);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && (fromOrig.GetBrightness() >= 0.3 && fromOrig.GetBrightness() <= 0.5))
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbow_loginscreen.png", System.Drawing.Imaging.ImageFormat.Png);

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbow_loginscreen.png"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        if (text.ToLower().Contains("account") || text.ToLower().Contains("pass") || text.ToLower().Contains("name")) return true;
                        return false;
                    }
                }
            }

            return false; //Unexpected result

            //end isLogin
        }


        public static bool isQueueBox()
        {
            
            

            Bitmap bmp = (Bitmap)Program.gameGetImage(440, 375, 1026, 553);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && fromOrig.GetBrightness() >= 0.3)
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbox_queuescreen.jpeg");

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbox_queuescreen.jpeg"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        
                        if (text.ToLower().Contains("position in queue"))
                        {
                            WoWQueue.update(text);
                            return true;
                        }                        
                        return false;
                    }
                }
            }

            return false; //Unexpected result

            //end isQueueBox
        }

        public static bool isQueueBoxCalc()
        {



            Bitmap bmp = (Bitmap)Program.gameGetImage(472, 390, 999, 548);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && fromOrig.GetBrightness() >= 0.3)
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbox_queuescreencalc.jpeg");

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbox_queuescreencalc.jpeg"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();

                        if (text.ToLower().Contains("position in queue"))
                        {
                            WoWQueue.update(text);
                            return true;
                        }
                        return false;
                    }
                }
            }

            return false; //Unexpected result

            //end isQueueBoxCalc
        }

        public static bool isRealmSelector()
        {
            Bitmap bmp = (Bitmap)Program.gameGetImage(785, 685, 1100, 760);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && (fromOrig.GetBrightness() >= 0.3 && fromOrig.GetBrightness() <= 0.5))
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbox_realmselect.png", System.Drawing.Imaging.ImageFormat.Png);

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbox_realmselect.png"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        
                        if (text.ToLower().Contains("okay") || text.ToLower().Contains("cancel")) return true;
                        return false;
                    }
                }
            }

            return false; //Unexpected result

            //end isLogin
        }

        public static bool isCharacterSelector()
        {
            Bitmap bmp = (Bitmap)Program.gameGetImage(615, 805, 860, 855);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && (fromOrig.GetBrightness() >= 0.3 && fromOrig.GetBrightness() <= 0.5))
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbox_charselect.png", System.Drawing.Imaging.ImageFormat.Png);

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbox_charselect.png"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        if (text.ToLower().Contains("enter") || text.ToLower().Contains("world")) return true;
                        return false;
                    }
                }
            }

            return false; //Unexpected result

            //end isLogin
        }

        public static bool isWaitingDialog()
        {
            Bitmap bmp = (Bitmap)Program.gameGetImage(623, 461, 845, 500);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && (fromOrig.GetBrightness() >= 0.3 && fromOrig.GetBrightness() <= 0.5))
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbox_waiting.png", System.Drawing.Imaging.ImageFormat.Png);

           
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbox_waiting.png"))
                {
                    using (var page = engine.Process(img))
                    {

                        var text = page.GetText();

                        if (text.ToLower().Contains("cancel")) return true;
                        return false;
                    }
                }
            }
            

            return false; //Unexpected result

            //end isLogin
        }

        public static bool isDisconnected()
        {
            Bitmap bmp = (Bitmap)Program.gameGetImage(600, 432, 874, 454);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && (fromOrig.GetBrightness() >= 0.3 && fromOrig.GetBrightness() <= 0.5))
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbox_disconnected.png", System.Drawing.Imaging.ImageFormat.Png);

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbox_disconnected.png"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        if (text.ToLower().Contains("disconnected") || text.ToLower().Contains("from") || text.ToLower().Contains("server")) return true;
                        return false;
                    }
                }
            }

            return false; //Unexpected result

            //end isLogin
        }

        public static bool isRealmSetup()
        {
            Bitmap bmp = (Bitmap)Program.gameGetImage(38, 188, 240, 214);
            Bitmap bmp2 = new Bitmap(bmp);

            //Scan the image 
            for (var x = 1; x < bmp.Width; x++)
            {
                for (var y = 1; y < bmp.Height; y++)
                {

                    Color fromOrig = bmp.GetPixel(x, y);
                    string result = Program.ColorClassify(fromOrig);

                    if (result == "Yellows" && fromOrig.GetSaturation() >= 0.92 && (fromOrig.GetBrightness() >= 0.3 && fromOrig.GetBrightness() <= 0.5))
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.Black);
                    }

                }
            }

            bmp2.Save("workbox_realmsetup.png", System.Drawing.Imaging.ImageFormat.Png);

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("./workbox_realmsetup.png"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        if (text.ToLower().Contains("choosing") || text.ToLower().Contains("realm")) return true;
                        return false;
                    }
                }
            }

            return false; //Unexpected result

            //end isLogin
        }




    }
}
