using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blur;
using System.Threading;

namespace osu_live.Layer
{
    public class L_background : Layer
    {
        // Effect Control
        Image newBackground, oldBackground;
        Color color;
        float x = 0, y = 0;
        float width = 0, height = 0;
        float preX = 0, preY = 0;
        float preWidth = 0, preHeight = 0;

        float Ratio { get => newBackground.Width / (float)newBackground.Height; }

        int fade, fadeSpeed;

        // Status Control
        bool isFadeIn, flag;

        public L_background()
        {
            Bitmap = new Bitmap(CanvasWidth, CanvasHeight);
        }
        public void Initialize(FileInfo MapInfo)
        {
            watch.Restart();

            Graphic = Graphics.FromImage(Bitmap);
            preX = x;
            preY = y;
            preWidth = width;
            preHeight = height;

            ChangeStatus = ChangeStatus.ReadyToChange;

            //oldBackground = newBackground;
            newBackground = SceneListen.GetMapBG(MapInfo);

            if (oldBackground == null)
                oldBackground = newBackground;

            fade = 0;
            isFadeIn = false;
            flag = false;
            fadeSpeed = 80;

            InitializeTime = watch.ElapsedMilliseconds;
            watch.Stop();
        }

        public void Draw()
        {
            watch.Restart();

            color = Color.FromArgb(rnd.Next(0, 50), rnd.Next(0, 50), rnd.Next(0, 50));
            if (!isFadeIn)
            {
                if (!flag)
                {
                    Graphic.Clear(Color.Transparent);
                    flag = true;
                    Graphic.DrawImage(oldBackground, preX, preY, preWidth, preHeight);
                }

                Graphic.FillRectangle(new SolidBrush(Color.FromArgb(fade, color.R, color.G, color.B)), new Rectangle(0, 0, CanvasWidth, CanvasHeight));

                fade += fadeSpeed;
                if (fade >= 255)
                {
                    isFadeIn = true;
                    fade = 255;
                }
            }
            else
            {
                if (flag)
                {
                    Graphic.Clear(Color.Transparent);
                    GetBGSize();
                    flag = false;
                }
                Graphic.DrawImage(newBackground, x, y, width, height);
                Graphic.FillRectangle(new SolidBrush(Color.FromArgb(fade, color.R, color.G, color.B)), new Rectangle(0, 0, CanvasWidth, CanvasHeight));

                if (fade > 0)
                {
                    fade -= fadeSpeed;
                    if (fade < 0)
                        fade = 0;
                }
                else
                {
                    ChangeStatus = ChangeStatus.ChangeFinshed;
                    Graphic.Dispose();

                    Thread t1 = new Thread(Blur);
                    t1.Start();
                    watch.Stop();
                    DrawTime = 0;
                    return;
                }
            }
            DrawTime = watch.ElapsedMilliseconds;
            watch.Stop();
        }

        private void Blur()
        {
            lock (oldBackground)
            {
                if (oldBackground != null)
                {
                    var GB = new GaussianBlur(5);
                    oldBackground = newBackground;
                    try
                    {
                        oldBackground = new Bitmap(GB.ProcessImage(new Bitmap(oldBackground, oldBackground.Width / 2, oldBackground.Height / 2))
                            , oldBackground.Width, oldBackground.Height);
                    }
                    catch { }
                }
            }
        }

        private void GetBGSize()
        {
            // deal with different size of image
            if (Ratio >= Constant.Canvas.Ratio) // more width
            {
                float scale = (float)CanvasHeight / newBackground.Height;
                height = CanvasHeight;
                width = newBackground.Width * scale;
                x = -(width - CanvasWidth) / 2;
                y = Constant.Canvas.Y;
            }
            else if (Ratio < Constant.Canvas.Ratio) // more height
            {
                float scale = (float)CanvasWidth / newBackground.Width;
                width = CanvasWidth;
                height = newBackground.Height * scale;
                x = Constant.Canvas.X;
                y = -(height - CanvasHeight) / 2;
            }
        }
    }
}
