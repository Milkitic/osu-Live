using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using DX = SharpDX;
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
using DW = SharpDX.DirectWrite;
using DXGI = SharpDX.DXGI;
using Mathe = SharpDX.Mathematics.Interop;
using DXIO = SharpDX.IO;

using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace osu_live_sharpdx.Layer
{
    class Background : ILayer
    {
        Stopwatch sw;
        FileInfo MapInfo;
        string Root { get; set; } = null;
        string RootOld { get; set; }

        bool startChange = false;
        bool isChanging = false;

        Thread tStatusCheck;

        D2D.Bitmap newBack, oldBack;
        float Ratio { get => newBack.Size.Width / newBack.Size.Height; }
        private List<D2D.Bitmap> BGList { get; set; } = new List<D2D.Bitmap>();

        Random rnd;

        float x = 0, y = 0;
        float width = 0, height = 0;
        float preX = 0, preY = 0;
        float preWidth = 0, preHeight = 0;
        float opacity1 = 1;
        float opacity2 = 0;

        // Text formats
        DW.TextFormat textFormat;
        DW.TextFormat textFormatInfo;

        int checkTimes = 0;
        // Brushes
        D2D.Brush whiteBrush, redBrush;

        public Background()
        {
            // Create brushes
            whiteBrush = new D2D.SolidColorBrush(RenderForm.RenderTarget, new Mathe.RawColor4(1, 1, 1, 1));
            redBrush = new D2D.SolidColorBrush(RenderForm.RenderTarget, new Mathe.RawColor4(0.99f, 0.16f, 0.3f, 1));

            // Create text formats
            textFormat = new DW.TextFormat(RenderForm.FactoryWrite, "Arial", 36);
            textFormatInfo = new DW.TextFormat(RenderForm.FactoryWrite, "Arial", 12);

            sw = new Stopwatch();

            rnd = new Random();

            // Some other things
            LoadBG();

            tStatusCheck = new Thread(StatusCheck);
            tStatusCheck.Start();

            newBack = BGList[0];
        }

        public void Measure()
        {
            float timing1 = 150;
            if (isChanging)
            {
                if (startChange)
                {
                    startChange = false;
                }

                opacity1 = (timing1 - sw.ElapsedMilliseconds) / timing1;

                opacity2 = (sw.ElapsedMilliseconds - timing1) / timing1;

                if (opacity2 >= 1)
                {
                    sw.Stop();
                    isChanging = false;
                }
            }
        }

        public void Draw()
        {
            Measure();

            // Draw text
            if (oldBack != null)
                lock (oldBack)
                {
                    if (opacity1 > 0) RenderForm.RenderTarget.DrawBitmap(oldBack,
                        new Mathe.RawRectangleF(preX, preY, preX + preWidth, preY + preHeight),
                        opacity1, D2D.BitmapInterpolationMode.Linear);
                }
            lock (newBack)
            {
                if (opacity2 > 0) RenderForm.RenderTarget.DrawBitmap(newBack,
                    new Mathe.RawRectangleF(x, y, x + width, y + height),
                    opacity2, D2D.BitmapInterpolationMode.Linear);
            }

            RenderForm.RenderTarget.DrawText("Background Layer", textFormat, new Mathe.RawRectangleF(0, 0, 400, 200), redBrush);
            RenderForm.RenderTarget.DrawText(sw.ElapsedMilliseconds.ToString(), textFormat, new Mathe.RawRectangleF(0, 50, 400, 200), whiteBrush);

            //RenderForm.RenderTarget.DrawText(DateTime.Now.ToLongTimeString() + "." + DateTime.Now.Millisecond.ToString("000"),
            //    textFormat, new Mathe.RawRectangleF(0, 100, 400, 200), whiteBrush);
            if (Root != null) RenderForm.RenderTarget.DrawText(Root,
                    textFormatInfo, new Mathe.RawRectangleF(0, 150, RenderForm.Width, 200), redBrush);
            RenderForm.RenderTarget.DrawText(checkTimes.ToString(),
                textFormatInfo, new Mathe.RawRectangleF(0, 180, 400, 200), whiteBrush);
            RenderForm.RenderTarget.DrawText(opacity1.ToString() + "," + opacity2.ToString(),
               textFormatInfo, new Mathe.RawRectangleF(0, 210, 400, 200), whiteBrush);
        }

        public void Dispose()
        {
            whiteBrush.Dispose();
            redBrush.Dispose();
            textFormat.Dispose();
            if (tStatusCheck.IsAlive)
                tStatusCheck.Abort();
        }

        private void StatusCheck()
        {
            while (true)
            {
                Thread.Sleep(100);
                checkTimes++;
                FileInfo tmp = new FileInfo(@"stream\Files\a_OsuFileLocation");
                if (MapInfo != null && tmp.LastWriteTime == MapInfo.LastWriteTime)
                    continue;
                try
                {
                    MapInfo = tmp;
                    string tmproot = File.ReadAllText(MapInfo.FullName);
                    RootOld = Root;
                    Root = tmproot;
                }
                catch
                {
                    continue;

                }
                if (Root.Trim() != "")
                {
                    if (Root == RootOld)
                        continue;

                    opacity1 = 1;
                    opacity2 = 0;
                    preX = x;
                    preY = y;
                    preWidth = width;
                    preHeight = height;

                    if (oldBack != null)
                        lock (oldBack)
                        {
                            oldBack = newBack;
                        }
                    lock (newBack)
                    {
                        newBack = GetMapBG(new FileInfo(Root));

                        if (oldBack == null) oldBack = newBack;
                        SetBGSize();
                    }
                    sw.Restart();
                    startChange = true;
                    isChanging = true;
                }
            }
        }

        private void LoadBG()
        {
            DirectoryInfo di = new DirectoryInfo("BG");
            FileInfo[] files = di.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension.ToLower() == ".png" || file.Extension.ToLower() == ".jpg" || file.Extension.ToLower() == ".jpeg")
                {
                    BGList.Add(LoadFromFile(RenderForm.RenderTarget, file.FullName));
                }
            }

        }

        private D2D.Bitmap GetMapBG(FileInfo fi)
        {
            string root = fi.FullName;
            string bgName;
            try
            {
                StreamReader sr;
                sr = new StreamReader(root);
                string line = sr.ReadLine();
                while (line != null && line != @"//Background and Video events")
                {
                    line = sr.ReadLine();
                }
                string nextLine = sr.ReadLine();
                if (nextLine.Substring(0, 2) != "//")
                {
                    bgName = nextLine.Split('"')[1];
                    string[] tmp = bgName.Split('.');
                    if (tmp[tmp.Length - 1] != "png" && tmp[tmp.Length - 1] != "jpg")
                    {
                        nextLine = sr.ReadLine();
                        if (nextLine.Substring(0, 2) == "//")
                            throw new Exception();
                        bgName = nextLine.Split('"')[1];
                        string[] tmp2 = bgName.Split('.');
                        if ((tmp2[tmp2.Length - 1] != "png" && tmp2[tmp2.Length - 1] != "jpg"))
                            throw new Exception();
                    }

                }
                else
                    bgName = null;
                sr.Close();
            }
            catch
            {
                bgName = null;
            }

            if (bgName != null)
                try
                {
                    return LoadFromFile(RenderForm.RenderTarget, new FileInfo(root).DirectoryName + "\\" + bgName);
                }
                catch
                {
                    newBack = null;
                }
            else newBack = null;
            if (newBack == null)
            {
                if (BGList.Count > 1) newBack = BGList[rnd.Next(0, BGList.Count)];
                else newBack = BGList[0];
            }
            return newBack;
        }

        public static D2D.Bitmap LoadFromFile(D2D.RenderTarget renderTarget, string filePath)
        {
            WIC.ImagingFactory imagingFactory = new WIC.ImagingFactory();
            DXIO.NativeFileStream fileStream = new DXIO.NativeFileStream(filePath,
                DXIO.NativeFileMode.Open, DXIO.NativeFileAccess.Read);

            WIC.BitmapDecoder bitmapDecoder = new WIC.BitmapDecoder(imagingFactory, fileStream, WIC.DecodeOptions.CacheOnDemand);
            WIC.BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

            WIC.FormatConverter converter = new WIC.FormatConverter(imagingFactory);
            converter.Initialize(frame, WIC.PixelFormat.Format32bppPRGBA);

            return D2D.Bitmap.FromWicBitmap(RenderForm.RenderTarget, converter);
        }

        private void SetBGSize()
        {
            // deal with different size of image
            if (Ratio >= 16 / 9f) // more width
            {
                float scale = RenderForm.Height / newBack.Size.Height;
                height = RenderForm.Height;
                width = newBack.Size.Width * scale;
                x = -(width - RenderForm.Width) / 2;
                y = 0;
            }
            else if (Ratio < 16 / 9f) // more height
            {
                float scale = RenderForm.Width / newBack.Size.Width;
                width = RenderForm.Width;
                height = newBack.Size.Height * scale;
                x = 0;
                y = -(height - RenderForm.Height) / 2;
            }
        }
    }

}
