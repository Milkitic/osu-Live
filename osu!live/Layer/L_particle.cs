using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live.Layer
{
    class L_particle
    {
        public Bitmap Bitmap { get; set; }
        public Graphics Graphic { get; set; }
        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.ReadyToChange;

        int canvas_height = Constant.Canvas.Height, canvas_width = Constant.Canvas.Width;

        public void Initialize()
        {
            Bitmap = new Bitmap(canvas_width, canvas_height);
            Graphic = Graphics.FromImage(Bitmap);
            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
            Graphic.CompositingQuality = CompositingQuality.HighQuality;

            rec = new RectangleF(640, 400, 60, 60);
            //rec = new RectangleF(-0, -0, 20, 20);
            path = new GraphicsPath();
            path.AddRectangle(rec);
        }

        RectangleF rec;
        GraphicsPath path;
        float degree = 0;
        float y_move = -1;
        public void Draw()
        {
            degree -= 30;
            //rec.Y -= 2;
            //rec.X += 1;
            Graphic.Clear(Color.Transparent);
            Graphic.FillRectangle(new SolidBrush(Color.FromArgb(64, 123, 53, 230)), rec);
            Graphic.ResetTransform();
            if (rec.Y < 0) rec.Y = canvas_height;
            rec.Y -= 3;
            y_move--;

            //Graphic.TranslateTransform(rec.Left, +rec.Top);
            //Graphic.RotateTransform(degree);
            //Graphic.TranslateTransform(-rec.Left, -rec.Top);

            Graphic.TranslateTransform(rec.Width / 2 + rec.Left, rec.Height / 2 + rec.Top);
            Graphic.RotateTransform(degree);
            Graphic.TranslateTransform(-rec.Width / 2 - rec.Left, -rec.Height / 2 - rec.Top);

           //path.Transform(m);
            //path.Transform(m2);
            //rec.Y -= 10 * (float)(Math.Sin(Math.PI / 180 * degree));
            //rec.X -= 10 * (float)(Math.Cos(Math.PI / 180 * degree));
            //Graphics.FillPath(new SolidBrush(Color.FromArgb(64, 123, 53, 230)), path);

        }
    }
}
