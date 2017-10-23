using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_live
{
    public partial class Form3 : Form
    {
        Stopwatch ts = new Stopwatch();
        Stopwatch ts2 = new Stopwatch();
        public Form3()
        {
            InitializeComponent();
            Initialize(count);
        }

        Bitmap bitmap;
        Graphics graphic;
        RectangleF[] rec;
        Color[] color;

        int count = 300;

        float[] degree = { 0 }, degree_spd, y_move_spd;
        int canvas_height, canvas_width;

        long timer_delay, compute_delay;

        private void Initialize(int num)
        {
            count = num;

            canvas_height = ClientRectangle.Height;
            canvas_width = ClientRectangle.Width;

            bitmap = new Bitmap(canvas_width, canvas_height);
            graphic = Graphics.FromImage(bitmap);
            graphic.SmoothingMode = SmoothingMode.HighQuality;

            //graphic.CompositingMode = CompositingMode.SourceOver;

            rec = new RectangleF[count];
            degree = new float[count];
            degree_spd = new float[count];
            y_move_spd = new float[count];
            color = new Color[count];
            for (int i = 0; i < count; i++)
            {
                int border = rnd.Next(10, 50);
                rec[i] = new RectangleF(rnd.Next(-20, canvas_width), rnd.Next(0, canvas_height), border, border);
                degree_spd[i] = rnd.Next(-5, 5);
                y_move_spd[i] = rnd.Next(-5, -2);

                color[i] = Color.FromArgb(rnd.Next(100, 150), rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer_delay = ts.ElapsedMilliseconds;
            ts.Restart();
            pictureBox1.Image = bitmap;
        }

        Random rnd = new Random();

        private void Draw()
        {
            try
            {
                graphic.Clear(Color.Transparent);
                for (int i = 0; i < count; i++)
                {
                    if (rec[i].Y + rec[i].Height < 0)
                        rec[i].Y = canvas_height;
                    rec[i].Y += y_move_spd[i];

                    degree[i] += degree_spd[i];

                    GraphicsPath gp = new GraphicsPath();
                    gp.AddRectangle(rec[i]);
                    //gp.AddString("cnbb", new FontFamily("arial"), FontStyle.Regular, 12, rec[i].Location, StringFormatFlags.NoClip);
                    Matrix m = new Matrix();
                    m.Translate(rec[i].Width / 2 + rec[i].Left, rec[i].Height / 2 + rec[i].Top);
                    m.Rotate(degree[i]);
                    m.Translate(-rec[i].Width / 2 - rec[i].Left, -rec[i].Height / 2 - rec[i].Top);
                    gp.Transform(m);
                    graphic.FillPath(new SolidBrush(color[i]), gp);
                    if (checkBox1.Checked) graphic.DrawPath(new Pen(Color.Black), gp);
                }
            }
            catch { }
            //graphic.Dispose();
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer2.Enabled = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Enabled = false;
            Initialize(trackBar1.Value);
            timer1.Enabled = true;
        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Interval = trackBar2.Value;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            graphic.Dispose();
            bitmap.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            ts2.Restart();
            Draw();
            compute_delay = ts2.ElapsedMilliseconds;

            this.Text = string.Format("Particles: {0}, Refresh: {1}ms, Draw: {2}ms. FPS: {3}", count, timer_delay, compute_delay, Math.Round((1000f / timer_delay), 2));
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }
    }
}
