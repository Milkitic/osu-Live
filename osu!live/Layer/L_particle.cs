using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live.Layer
{
    //效率就是一坨翔，我要退坑了

    public class L_particle
    {
        //public Bitmap Bitmap { get; set; }
        public Bitmap[] Bitmap { get; set; }
        public Graphics[] Graphic { get; set; }
        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.ReadyToChange;

        RectangleF[] rec;
        float[] degree = { 0 }, degree_spd, y_move_spd;
        int canvas_height = Constant.Canvas.Height, canvas_width = Constant.Canvas.Width;
        Random rnd = new Random();
        public void Initialize()
        {
            Graphic = new Graphics[0];
            Bitmap = new Bitmap[Graphic.Length];
            rec = new RectangleF[Graphic.Length];
            degree = new float[Graphic.Length];
            degree_spd = new float[Graphic.Length];
            y_move_spd = new float[Graphic.Length];

            //Bitmap = new Bitmap(canvas_width, canvas_height);
            //↓严重低下部分
            for (int i = 0; i < Graphic.Length; i++)
            {
                Bitmap[i] = new Bitmap(canvas_width, canvas_height);
                Graphic[i] = Graphics.FromImage(Bitmap[i]);
                Graphic[i].SmoothingMode = SmoothingMode.AntiAlias;
                Graphic[i].CompositingQuality = CompositingQuality.HighSpeed;

                int border = rnd.Next(10, 50);
                rec[i] = new RectangleF(rnd.Next(0, 1280), rnd.Next(0, 720), border, border);
                degree_spd[i] = rnd.Next(-10, 10);
                y_move_spd[i] = rnd.Next(-10, -3);
            }
        }

        public void Draw()
        {
            return;
            for (int i = 0; i < Graphic.Length; i++)
            {
                if (rec[i].Y + rec[i].Height < 0)
                    rec[i].Y = canvas_height;
                rec[i].Y += y_move_spd[i];
                degree[i] += degree_spd[i];

                try
                {
                    //Graphic[i] = Graphics.FromImage(Bitmap[i]);
                    Graphic[i].Clear(Color.Transparent);

                    Graphic[i].TranslateTransform(rec[i].Width / 2 + rec[i].Left, rec[i].Height / 2 + rec[i].Top);
                    Graphic[i].RotateTransform(degree[i]);
                    Graphic[i].TranslateTransform(-rec[i].Width / 2 - rec[i].Left, -rec[i].Height / 2 - rec[i].Top);

                    Graphic[i].FillRectangle(new SolidBrush(Color.FromArgb(64, 123, 53, 230)), rec[i]);
                    Graphic[i].ResetTransform();
                    //Graphic[i].Dispose();
                }
                catch (InvalidOperationException ex)
                {

                }
            }
            //degree -= 3;
            ////rec.Y -= 2;
            ////rec.X += 1;
            //Graphic.Clear(Color.Transparent);
            //if (rec.Y + rec.Height < 0) rec.Y = canvas_height;
            //rec.Y -= 3;

            //Graphic.TranslateTransform(rec.Width / 2 + rec.Left, rec.Height / 2 + rec.Top);
            //Graphic.RotateTransform(degree);
            //Graphic.TranslateTransform(-rec.Width / 2 - rec.Left, -rec.Height / 2 - rec.Top);

            //Graphic.FillRectangle(new SolidBrush(Color.FromArgb(64, 123, 53, 230)), rec);
            //Graphic.ResetTransform();
        }
    }
}
