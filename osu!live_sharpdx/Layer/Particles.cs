using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DX = SharpDX;
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
using DW = SharpDX.DirectWrite;
using DXGI = SharpDX.DXGI;
using Mathe = SharpDX.Mathematics.Interop;
using DXIO = SharpDX.IO;
using System.Drawing;
using System.IO;

namespace osu_live_sharpdx.Layer
{
    class Particles : ILayer
    {
        // Brushes
        D2D.Brush brush1;
        D2D.Brush brush2;

        // Positions
        Mathe.RawVector2[] startPos;
        Mathe.RawVector2[] nowPos;
        // Bitmap
        D2D.Bitmap[] oriBitmaps;
        D2D.Bitmap[] bitmaps;

        // Speeds (n px/ms)
        float[] speeds;

        // Timing (n ms)
        float[] timings;

        // radius
        float[] r;

        // fade
        float[] startF;
        float[] f;

        // shake
        float[] shakeOffset;
        float[] shakeCycle;

        int panelHeight;

        Stopwatch sw;

        int particleCount;
        Random rnd = new Random();

        public Particles(int count, int panelHeight)
        {
            particleCount = count;
            this.panelHeight = panelHeight;
            // Load bitmap
            FileInfo[] fis = new DirectoryInfo("element").GetFiles("*.png", SearchOption.TopDirectoryOnly);
            oriBitmaps = new D2D.Bitmap[fis.Length];
            for (int i = 0; i < oriBitmaps.Length; i++)
            {
                oriBitmaps[i] = LoadFromFile(RenderForm.RenderTarget, fis[i].FullName);
            }

            startPos = new Mathe.RawVector2[count];
            nowPos = new Mathe.RawVector2[count];
            speeds = new float[count];
            timings = new float[count];
            bitmaps = new D2D.Bitmap[count];

            r = new float[count];
            startF = new float[count];
            f = new float[count];
            shakeOffset = new float[count];
            shakeCycle = new float[count];

            brush1 = new D2D.LinearGradientBrush(RenderForm.RenderTarget, new D2D.LinearGradientBrushProperties()
            {
                StartPoint = new Mathe.RawVector2(0, 0),
                EndPoint = new Mathe.RawVector2(0, panelHeight + 100),
            },
                new D2D.GradientStopCollection(RenderForm.RenderTarget, new D2D.GradientStop[]
                    {
                        new D2D.GradientStop()
                        {
                            Color = new Mathe.RawColor4(0,0,0,0),
                            Position = 1,
                        },
                        new D2D.GradientStop()
                        {
                            Color = new Mathe.RawColor4(0,0,0,0.1f),
                            Position = 0,
                        }
                    }));

            brush2 = new D2D.LinearGradientBrush(RenderForm.RenderTarget, new D2D.LinearGradientBrushProperties()
            {
                StartPoint = new Mathe.RawVector2(0, RenderForm.Height - panelHeight),
                EndPoint = new Mathe.RawVector2(0, RenderForm.Height),
            },
                new D2D.GradientStopCollection(RenderForm.RenderTarget, new D2D.GradientStop[]
                    {
                        new D2D.GradientStop()
                        {
                            Color = new Mathe.RawColor4(0,0,0,0),
                            Position = 0,
                        },
                        new D2D.GradientStop()
                        {
                            Color = new Mathe.RawColor4(0,0,0,0.7f),
                            Position = 1,
                        }
                    }));

            for (int i = 0; i < count; i++)
            {
                bitmaps[i] = oriBitmaps[rnd.Next(0, oriBitmaps.Length)];
                r[i] = (float)(rnd.NextDouble() * 20);
                startF[i] = (float)rnd.NextDouble();
                speeds[i] = r[i] * r[i] * 0.0005f;
                //speeds[i] = (float)(rnd.NextDouble() * 0.07 + 0.02);
                timings[i] = 1 / speeds[i] * 1000;
                startPos[i] = new Mathe.RawVector2(rnd.Next(0, RenderForm.Width), RenderForm.Height + rnd.Next(40, 50));
                shakeCycle[i] = 1 / speeds[i] * 500;
                shakeOffset[i] = (float)rnd.NextDouble() * shakeCycle[i];
            }

            sw = new Stopwatch();
            sw.Restart();
        }

        public void Measure()
        {
            for (int i = 0; i < particleCount; i++)
            {
                float ratio = (sw.ElapsedMilliseconds % timings[i]) / timings[i];

                float ratio2 = ((sw.ElapsedMilliseconds + shakeOffset[i]) % shakeCycle[i]) / shakeCycle[i];
                f[i] = startF[i] - ratio * startF[i];
                nowPos[i] = new Mathe.RawVector2(startPos[i].X + (float)Math.Sin(ratio2 * 2 * Math.PI) * 10, startPos[i].Y - sw.ElapsedMilliseconds % timings[i] * speeds[i]);
            }
        }

        public void Draw()
        {
            Measure();

            RenderForm.RenderTarget.FillRectangle(new Mathe.RawRectangleF(0, RenderForm.Height - panelHeight, RenderForm.Width, RenderForm.Height), brush2);
            RenderForm.RenderTarget.FillRectangle(new Mathe.RawRectangleF(0, 0, RenderForm.Width, panelHeight + 100), brush1);


            for (int i = 0; i < particleCount; i++)
            {
                if (nowPos[i].Y < RenderForm.Height + 10 && nowPos[i].Y > -10)
                    RenderForm.RenderTarget.DrawBitmap(bitmaps[i],
                        new Mathe.RawRectangleF(nowPos[i].X, nowPos[i].Y, nowPos[i].X + r[i], nowPos[i].Y + r[i]),
                        f[i], D2D.BitmapInterpolationMode.NearestNeighbor);
            }
        }

        public void Dispose()
        {

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
    }
}
