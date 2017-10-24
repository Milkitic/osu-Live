using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live.Layer
{
    public class L_background
    {
        public Bitmap Bitmap { get; set; }
        public Graphics Graphics { get; set; }
        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.ReadyToChange;

        float x = 0, y = 0;
        float width = 0, height = 0;
        float preX = 0, preY = 0;
        float preWidth = 0, preHeight = 0;

        Image newBackground, oldBackground;
        float Ratio { get => newBackground.Width / (float)newBackground.Height; }

        int canvas_height = Constant.Canvas.Height;
        int canvas_width = Constant.Canvas.Width;

        int fade, fadeSpeed;
        bool isFadeIn, flag;

        public L_background()
        {
            Bitmap = new Bitmap(canvas_width, canvas_height);
        }
        public void Initialize(FileInfo MapInfo)
        {
            Graphics = Graphics.FromImage(Bitmap);
            preX = x;
            preY = y;
            preWidth = width;
            preHeight = height;

            ChangeStatus = ChangeStatus.ReadyToChange;

            oldBackground = newBackground;
            newBackground = SceneListen.GetMapBG(MapInfo);
            if (oldBackground == null)
                oldBackground = newBackground;

            fade = 0;
            isFadeIn = false;
            flag = false;
            fadeSpeed = 80;
        }

        public void Draw()
        {
           if (!isFadeIn)
            {
                if (!flag)
                {
                    Graphics.Clear(Color.Transparent);
                    flag = true;
                    Graphics.DrawImage(oldBackground, preX, preY, preWidth, preHeight);
                }
                
                Graphics.FillRectangle(new SolidBrush(Color.FromArgb(fade, 255, 255, 255)), new Rectangle(0, 0, canvas_width, canvas_height));

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
                    Graphics.Clear(Color.Transparent);
                    GetBGSize();
                    flag = false;
                }
                Graphics.DrawImage(newBackground, x, y, width, height);
                Graphics.FillRectangle(new SolidBrush(Color.FromArgb(fade, 255, 255, 255)), new Rectangle(0, 0, canvas_width, canvas_height));

                if (fade > 0)
                {
                    fade -= fadeSpeed;
                    if (fade < 0)
                        fade = 0;
                }
                else
                    ChangeStatus = ChangeStatus.ChangeFinshed;
            }
        }

        private void GetBGSize()
        {
            // deal with different size of image
            if (Ratio >= Constant.Canvas.Ratio) // more width
            {
                float scale = (float)canvas_height / newBackground.Height;
                height = canvas_height;
                width = newBackground.Width * scale;
                x = -(width - canvas_width) / 2;
                y = Constant.Canvas.Y;
            }
            else if (Ratio < Constant.Canvas.Ratio) // more height
            {
                float scale = (float)canvas_width / newBackground.Width;
                width = canvas_width;
                height = newBackground.Height * scale;
                x = Constant.Canvas.X;
                y = -(height - canvas_height) / 2;
            }
        }
    }
}
