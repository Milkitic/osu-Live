using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live.Layer
{
    class L_background
    {
        public Bitmap Bitmap { get; set; }
        public Graphics Graphics { get; set; }
        public Bitmap Bitmap_c { get; set; }
        public Graphics Graphics_c { get; set; }

        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Width { get; set; } = 0;
        public float Height { get; set; } = 0;
        public float Ratio { get => Background.Width / (float)Background.Height; }

        public float PreX { get; set; } = 0;
        public float PreY { get; set; } = 0;
        public float PreWidth { get; set; } = 0;
        public float PreHeight { get; set; } = 0;

        public Image Background { get; set; }
        public Image PreBackground { get; set; }

        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.ReadyToChange;

        private int canvas_height = Constant.Canvas.Height;
        private int canvas_width = Constant.Canvas.Width;

        public L_background()
        {
            Bitmap = new Bitmap(canvas_width, canvas_height);
            Bitmap_c = new Bitmap(canvas_width, canvas_height);
        }
        public void Initialize(FileInfo MapInfo)
        {
            Graphics = Graphics.FromImage(Bitmap);
            Graphics_c = Graphics.FromImage(Bitmap_c);
            PreX = X;
            PreY = Y;
            PreWidth = Width;
            PreHeight = Height;

            ChangeStatus = ChangeStatus.ReadyToChange;

            PreBackground = Background;
            Background = SceneListen.GetMapBG(MapInfo);
            if (PreBackground == null)
                PreBackground = Background;

            fade = 0;
            isFadeIn = false;
            flag = false;
            fadeSpeed = 80;
        }

        int fade;
        bool isFadeIn;
        bool flag;
        int fadeSpeed;

        public void Draw()
        {
            Graphics_c.Clear(Color.Transparent);
            if (!isFadeIn)
            {
                if (flag == false)
                {
                    Graphics.DrawImage(PreBackground, PreX, PreY, PreWidth, PreHeight);
                    flag = true;
                }

                Graphics_c.FillRectangle(new SolidBrush(Color.FromArgb(fade, 0, 0, 0)), new Rectangle(0, 0, canvas_width, canvas_height));

                fade += fadeSpeed;
                if (fade >= 255)
                {
                    isFadeIn = true;
                    fade = 255;
                }
            }
            else
            {
                if (flag == true)
                {
                    GetBGSize();
                    Graphics.Clear(Color.Transparent);
                    Graphics.DrawImage(Background, X, Y, Width, Height);
                    flag = false;
                }

                Graphics_c.FillRectangle(new SolidBrush(Color.FromArgb(fade, 0, 0, 0)), new Rectangle(0, 0, canvas_width, canvas_height));
                fade -= fadeSpeed;
                if (fade <= 0)
                {
                    ChangeStatus = ChangeStatus.ChangeFinshed;
                    //fade = 0;
                }
            }
        }

        private void GetBGSize()
        {
            // deal with different size of image
            if (Ratio >= Constant.Canvas.Ratio) // more width
            {
                float scale = (float)canvas_height / Background.Height;
                Height = canvas_height;
                Width = Background.Width * scale;
                X = -(Width - canvas_width) / 2;
                Y = Constant.Canvas.Y;
            }
            else if (Ratio < Constant.Canvas.Ratio) // more height
            {
                float scale = (float)canvas_width / Background.Width;
                Width = canvas_width;
                Height = Background.Height * scale;
                X = Constant.Canvas.X;
                Y = -(Height - canvas_height) / 2;
            }
        }
    }
}
