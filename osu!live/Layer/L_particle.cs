using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live.Layer
{
    public class L_particle : Layer
    {
        public bool Rotate { get; set; } = true;
        public bool Border { get; set; } = false;

        // Effect Control
        RectangleF[] rec;
        Color[] color;
        float[] degree = { 0 }, degSpeed, yMovSpeed;
        int ParticleCount { get; set; } = 50;

        // Status Control
        bool isIniting = false;

        public void Initialize(int count)
        {
            isIniting = true;
            watch.Restart();

            #region INIT
            ParticleCount = count;

            rec = new RectangleF[count];
            degree = new float[count];
            degSpeed = new float[count];
            yMovSpeed = new float[count];
            color = new Color[count];

            // Init all rectengles
            for (int i = 0; i < count; i++)
            {
                int border = rnd.Next(10, 50);
                rec[i] = new RectangleF(rnd.Next(-20, CanvasWidth), rnd.Next(0, CanvasHeight), border * zoom, border * zoom);
                degSpeed[i] = rnd.Next(-5, 5);
                yMovSpeed[i] = rnd.Next(-5, -2) * zoom;

                color[i] = Color.FromArgb(rnd.Next(50, 150), rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            }
            #endregion

            watch.Stop();
            InitializeTime = watch.ElapsedMilliseconds;
            isIniting = false;
        }

        public void Draw()
        {
            if (isIniting) return;
            watch.Restart();

            #region DRAW
            Graphic.Clear(Color.Transparent);
            for (int i = 0; i < ParticleCount; i++)
            {
                if (rec[i].Y + rec[i].Height < 0)
                    rec[i].Y = CanvasHeight;
                rec[i].Y += yMovSpeed[i];
                degree[i] += degSpeed[i];

                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(rec[i]);

                if (Rotate)
                {
                    Matrix m = new Matrix();
                    m.Translate(rec[i].Width / 2 + rec[i].Left, rec[i].Height / 2 + rec[i].Top);
                    m.Rotate(degree[i]);
                    m.Translate(-rec[i].Width / 2 - rec[i].Left, -rec[i].Height / 2 - rec[i].Top);
                    gp.Transform(m);
                }
                Graphic.FillPath(new SolidBrush(color[i]), gp);
                if (Border) Graphic.DrawPath(new Pen(Color.Black), gp);
            }
            #endregion

            watch.Stop();
            DrawTime = watch.ElapsedMilliseconds;
        }
    }
}
