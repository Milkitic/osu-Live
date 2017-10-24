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
        public Bitmap Bitmap { get; set; }
        public Graphics Graphic { get; set; }
        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.ReadyToChange;

        int canvas_height = Constant.Canvas.Height, canvas_width = Constant.Canvas.Width;
        float zoom = (float)Constant.Canvas.Zoom;

        RectangleF[] rec;
        Color[] color;
        float[] degree = { 0 }, degree_spd, y_move_spd;
        Random rnd = new Random();

        int count = 150;
        bool rotate = true;
        bool border = false;
        
        public void Initialize()
        {
            Bitmap = new Bitmap(canvas_width, canvas_height);
            Graphic = Graphics.FromImage(Bitmap);
            Graphic.SmoothingMode = SmoothingMode.HighQuality;

            //graphic.CompositingMode = CompositingMode.SourceOver;

            rec = new RectangleF[count];
            degree = new float[count];
            degree_spd = new float[count];
            y_move_spd = new float[count];
            color = new Color[count];
            for (int i = 0; i < count; i++)
            {
                int border = rnd.Next(10, 50);
                rec[i] = new RectangleF(rnd.Next(-20, canvas_width), rnd.Next(0, canvas_height), border * zoom, border * zoom);
                degree_spd[i] = rnd.Next(-5, 5);
                y_move_spd[i] = rnd.Next(-5, -2) * zoom;

                color[i] = Color.FromArgb(rnd.Next(50, 150), rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            }
        }

        public void Draw()
        {
            Graphic.Clear(Color.Transparent);
            for (int i = 0; i < count; i++)
            {
                if (rec[i].Y + rec[i].Height < 0)
                    rec[i].Y = canvas_height;
                rec[i].Y += y_move_spd[i];
               degree[i] += degree_spd[i];

                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(rec[i]);
                //gp.AddString("cnbb", new FontFamily("arial"), FontStyle.Regular, 12, rec[i].Location, StringFormatFlags.NoClip);
                if (rotate)
                {
                    Matrix m = new Matrix();
                    m.Translate(rec[i].Width / 2 + rec[i].Left, rec[i].Height / 2 + rec[i].Top);
                    m.Rotate(degree[i]);
                    m.Translate(-rec[i].Width / 2 - rec[i].Left, -rec[i].Height / 2 - rec[i].Top);
                    gp.Transform(m);
                }
                Graphic.FillPath(new SolidBrush(color[i]), gp);
                if (border) Graphic.DrawPath(new Pen(Color.Black), gp);
            }

        }
    }
}
