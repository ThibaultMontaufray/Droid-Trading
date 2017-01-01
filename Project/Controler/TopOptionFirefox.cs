namespace Droid_trading
{
    using   Aspose.OCR;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;

    public static class TopOptionFirefox
    {
        #region Struct
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        #endregion

        #region Attribute
        private const int SWRESTORE = 9;

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        #endregion

        #region Properties
        #endregion

        #region Methods public
        public static void OpenFirefox()
        {
            string url = "file:///P:/_DEV/JavaScript/We%27llRocksIt.html";
            System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Mozilla Firefox SG\firefox.exe", url);
        }
        public static void CloseFirefox()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 20, rect.top + 10);
            }
        }
        public static void SetTrade5()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 360, rect.top + 145);
            }
        }
        public static void SetTrade10()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 360, rect.top + 169);
            }
        }
        public static void SetTrade25()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 360, rect.top + 193);
            }
        }
        public static void SetTrade50()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 360, rect.top + 217);
            }
        }
        public static void SetTrade100()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 360, rect.top + 230);
            }
        }
        public static void SetTrade250()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 360, rect.top + 250);
            }
        }
        public static void SetTrade500()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 360, rect.top + 280);
            }
        }
        public static void SetTrade1000()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 360, rect.top + 115);
                LeftClick(rect.right - 365, rect.top + 300);
            }
        }
        public static void TradeDown()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 100, rect.top + 280);
                Thread.Sleep(100);
                LeftClick(rect.right - 100, rect.top + 280);
            }
        }
        public static void TradeUp()
        {
            string procName = "firefox";
            using (Process proc = Process.GetProcessesByName(procName)[0])
            {
                // You need to focus on the application
                SetForegroundWindow(proc.MainWindowHandle);
                ShowWindow(proc.MainWindowHandle, SWRESTORE);

                Thread.Sleep(1000);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                int watchdog = 1000;
                while (error == (IntPtr)0 && watchdog >= 0)
                {
                    error = GetWindowRect(proc.MainWindowHandle, ref rect);
                    watchdog--;
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                LeftClick(rect.right - 100, rect.top + 169);
                Thread.Sleep(100);
                LeftClick(rect.right - 100, rect.top + 280);
            }
        }

        public static double GetMarketCurrentPrice()
        {
            Bitmap bmp = GetScreen();
            string price = PriceBmpToString(bmp);

            double priceFinal;
            if (double.TryParse(price, out priceFinal))
            {
                return priceFinal;
            }
            else
            {
                return double.NaN;
            }
        }
        #endregion

        #region Methods private
        private static void LeftClick(int x, int y)
        {
            Cursor.Position = new System.Drawing.Point(x, y);
            mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
        }
        private static Bitmap CropImg(Bitmap src)
        {
            Rectangle cropRect = new Rectangle(600, 205, 115, 30);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }
            return AdjustContrast(target, 90);
        }
        private static Bitmap AdjustContrast(Bitmap Image, float Value)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }
        private static Bitmap GetScreen()
        {
            try
            {
                string procName = "firefox";
                using (Process proc = Process.GetProcessesByName(procName)[0])
                {
                    // You need to focus on the application
                    SetForegroundWindow(proc.MainWindowHandle);
                    ShowWindow(proc.MainWindowHandle, SWRESTORE);

                    Thread.Sleep(1000);

                    Rect rect = new Rect();
                    IntPtr error = GetWindowRect(proc.MainWindowHandle, ref rect);

                    int watchdog = 1000;
                    while (error == (IntPtr)0 && watchdog >= 0)
                    {
                        error = GetWindowRect(proc.MainWindowHandle, ref rect);
                        watchdog--;
                    }

                    int width = rect.right - rect.left;
                    int height = rect.bottom - rect.top;

                    Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    Graphics.FromImage(bmp).CopyFromScreen(rect.left,
                                                           rect.top,
                                                           0,
                                                           0,
                                                           new Size(width, height),
                                                           CopyPixelOperation.SourceCopy);
                    return bmp;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            return null;
        }
        private static string PriceBmpToString(Bitmap bmp)
        {
            bmp = CropImg(bmp);
            return ExtractTextFromImage(bmp);
        }
        private static string ExtractTextFromImage(Bitmap bt)
        {
            //Initialize an instance of OcrEngine
            OcrEngine ocrEngine = new OcrEngine();

            //Set the Image property by loading the image from file path location or an instance of MemoryStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bt.Save(memoryStream, ImageFormat.Bmp);
                //ocrEngine.Image = ImageStream.FromStream(memoryStream, ImageStreamFormat.Bmp);   // ImageStream.From File(imageFile);
                ocrEngine.Image = ImageStream.FromStream(memoryStream, ImageStreamFormat.Bmp);

                ocrEngine.Config.Whitelist = new char[] { '.', ',', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                //ocrEngine.Config.Whitelist = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                //Process the image
                if (ocrEngine.Process())
                {
                    string retVal = ocrEngine.Text.ToString().Replace(' ', '.');
                    if (retVal.Length > 4)
                    {
                        retVal = retVal.Substring(0, 3) + Regex.Replace(retVal.Substring(3, retVal.Length - 3), @"\.", "");
                        retVal = retVal.Replace("..", ".");
                        retVal = retVal.Substring(retVal.Length - 4, 4);
                        return retVal;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            return string.Empty;
        }
        #endregion
    }
}