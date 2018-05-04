using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Gma.System.MouseKeyHook;

namespace osu_live.Layer
{
    public class L_DashAim : Layer
    {
        // Effect Control
        Point[] pf = new Point[2];
        const int RED = 255, GREEN = 255, BLUE = 255;
        const int RED2 = 125, GREEN2 = 254, BLUE2 = 254;
        double xRed = RED, xGreen = GREEN, xBlue = BLUE;
        double boostStatus = 0;
        Color boostColor = Color.FromArgb(RED, GREEN, BLUE);

        const int RED_kps = 255, GREEN_kps = 245, BLUE_kps = 181;
        const int RED2_kps = 242, GREEN2_kps = 154, BLUE2_kps = 117;
        Color boostColor_kps = Color.FromArgb(RED_kps, GREEN_kps, BLUE_kps);

        Image a = Image.FromFile("1.png");
        Image kira = Image.FromFile("animate\\progress\\kira.png");
        Image light = Image.FromFile("animate\\progress\\light.png");
        Image light2 = Image.FromFile("animate\\progress\\light2.png");
        Image toum = Image.FromFile("templet\\toum.png");
        Image[] progressImg = new Image[15];

        int kps = 0;
        Stopwatch sw = new Stopwatch();
        List<long> pressTime = new List<long>();
        float justkps = 0;
        // Status Control
        bool isIniting = false;

        // Hook
        List<Keys> currentKey = new List<Keys>();
        Thread t;
        private IKeyboardMouseEvents m_GlobalHook;

        public L_DashAim()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyPress += HookKeyPress;
            //m_GlobalHook.KeyUp += HookKeyUp;
        }

        public void Initialize()
        {
            isIniting = true;
            int x = (int)(CanvasWidth * 0.52),
                y = (int)(CanvasHeight * 0.83),
                w = (int)(CanvasWidth * 0.3),
                h = (int)(toum.Height / (float)toum.Width * w);
            //h = (int)(CanvasHeight * 0.1);
            RecPanel = new Rectangle(x, y, w, h);
            Bitmap = new Bitmap(RecPanel.Width, RecPanel.Height);
            Graphic = Graphics.FromImage(Bitmap);
            Graphic.SmoothingMode = SmoothingMode.HighQuality;
            Graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            for (int i = 0; i < progressImg.Length; i++)
            {
                progressImg[i] = Image.FromFile("animate\\progress\\lightingL-" + i + ".png");
            }

            isIniting = false;

            t = new Thread(CalculateKey);
            t.Start();
            sw.Start();
        }

        public void Draw()
        {
            if (isIniting) return;
            Graphic.Clear(Color.Transparent);

            //Graphic.DrawRectangle(new Pen(Color.Red, 2), 0, 0, Bitmap.Width, Bitmap.Height);
            //Graphic.DrawLine(new Pen(Color.Red, 2), 0, 0, Bitmap.Width, Bitmap.Height);
            //Graphic.DrawLine(new Pen(Color.Red, 2), 0, Bitmap.Height, Bitmap.Width, 0);

            var pos = Cursor.Position;

            for (int i = pf.Length - 1; i > 0; i--)
            {
                pf[i] = pf[i - 1];
            }
            pf[0] = new Point(pos.X, pos.Y);

            float d = (float)Math.Pow(Math.Pow((pf[0].X - pf[1].X), 2) + Math.Pow((pf[0].Y - pf[1].Y), 2), 0.5);
            float d_bili = d / 120;
            int positive = d_bili >= 0.3 ? +1 : -1;
            int ratio = 5;
            double increase = Math.Abs(0.4 - d_bili);
            int positive_r = RED2 > RED ? positive : -positive,
                positive_g = GREEN2 > GREEN ? positive : -positive,
                positive_b = BLUE2 > BLUE ? positive : -positive;
            double new_red = xRed + positive_r * (Math.Abs(RED2 - RED) / 255f) * (1 + ratio * increase),
                        new_green = xGreen + positive_g * (Math.Abs(GREEN2 - GREEN) / 255f) * (1 + ratio * increase),
                        new_blue = xBlue + positive_b * (Math.Abs(BLUE2 - BLUE) / 255f) * (1 + ratio * increase);
            int minR = RED > RED2 ? RED2 : RED,
             maxR = RED < RED2 ? RED2 : RED;
            int minG = GREEN > GREEN2 ? GREEN2 : GREEN,
             maxG = GREEN < GREEN2 ? GREEN2 : GREEN;
            int minB = BLUE > BLUE2 ? BLUE2 : BLUE,
             maxB = BLUE < BLUE2 ? BLUE2 : BLUE;

            if (new_red > maxR) new_red = maxR;
            else if (new_red < minR) new_red = minR;
            if (new_green > maxG) new_green = maxG;
            else if (new_green < minG) new_green = minG;
            if (new_blue > maxB) new_blue = maxB;
            else if (new_blue < minB) new_blue = minB;

            xRed = new_red;
            xGreen = new_green;
            xBlue = new_blue;
            boostColor = Color.FromArgb((int)new_red, (int)new_green, (int)new_blue);
            Color linearColor = Color.FromArgb(60, (int)(new_red * 0.9), (int)(new_green * 0.9), (int)(new_blue * 0.9));
            boostStatus = (new_red - RED) / (RED2 - RED);
            boostStatus = (new_blue - BLUE) / (BLUE2 - BLUE);

            //Graphic.DrawString(boostColor.ToString() + "\r\n" + boostStatus + "\r\n" + d_bili
            //    + "\r\n" + kps.ToString(), new Font("Arial", 11), new SolidBrush(Color.White), 0, 0);
            float pro_width = Bitmap.Width * 0.7f, pro_height = Bitmap.Height * 0.7f;
            float offset_h = 40;
            var progress_rec = new RectangleF(Bitmap.Width / 2f, 0 + offset_h / 2f, (pro_width / 2f - 25) * (float)boostStatus, Bitmap.Height - offset_h);
            //if (progress_rec.Width != 0)
            //    Graphic.FillRectangle(new TextureBrush(a, WrapMode.Tile, progress_rec), progress_rec);

            if (boostStatus > 0.02)
            {
                Point[] _Point = new Point[] {
                new Point((int)(progress_rec.Left),(int)progress_rec.Top),
                new Point((int)(progress_rec.Left), (int)(progress_rec.Top+progress_rec.Height / 2)),
                new Point((int)(progress_rec.Left+ progress_rec.Width), (int)(progress_rec.Top+progress_rec.Height / 2)),
                new Point((int)(progress_rec.Left+ progress_rec.Width), (int)(progress_rec.Top)) };
                //Point[] _Point = new Point[] {
                //    new Point((int)(progress_rec.Left),(int)progress_rec.Top),
                //    new Point((int)(progress_rec.Left+ progress_rec.Width/2), (int)(progress_rec.Top)),
                //    new Point((int)(progress_rec.Left+ progress_rec.Width/2), (int)(progress_rec.Top+progress_rec.Height)),
                //    new Point((int)(progress_rec.Left), (int)(progress_rec.Top+progress_rec.Height)) };

                PathGradientBrush _SetBruhs = new PathGradientBrush(_Point, WrapMode.TileFlipY);
                _SetBruhs.CenterPoint = new PointF(progress_rec.Left, 0);
                _SetBruhs.FocusScales = new PointF(progress_rec.Left + progress_rec.Width, 0);
                _SetBruhs.CenterColor = boostColor;
                _SetBruhs.SurroundColors = new Color[] { linearColor };

                Graphic.FillRectangle(_SetBruhs, progress_rec);


            }
            Graphic.DrawImage(toum, (Bitmap.Width - pro_width) / 2f, (Bitmap.Height - pro_height) / 2f, pro_width, pro_height);
            if (positive > 0)
            {
                Bitmap b = new Bitmap(light.Width, light.Height);
                Graphics g = Graphics.FromImage(b);
                g.TranslateTransform(b.Width / 2f, b.Height / 2f);
                g.RotateTransform(rnd.Next(0, 360));
                g.DrawImage(light, 0 - light.Width / 2f, 0 - light.Height / 2f, b.Width, b.Height);

                int width = 20;
                Graphic.DrawImage(b, progress_rec.Left + progress_rec.Width - width / 2f, 0, width, Bitmap.Height);
                g.Dispose();
                b.Dispose();
            }
            else
            {
                Bitmap b = new Bitmap(light2.Width, light2.Height);
                Graphics g = Graphics.FromImage(b);
                g.TranslateTransform(b.Width / 2f, b.Height / 2f);
                g.RotateTransform(rnd.Next(0, 360));
                g.DrawImage(light2, 0 - light2.Width / 2f, 0 - light2.Height / 2f, b.Width, b.Height);

                int width = 15;
                Graphic.DrawImage(b, progress_rec.Left + progress_rec.Width - width / 2f, 0, width, Bitmap.Height);
                g.Dispose();
                b.Dispose();
            }

            // kps
            const float KPS_MAX = 25;
            float buffer_up = 0.5f;
            float buffer_down = 0.4f;

            if (justkps > kps) justkps -= buffer_down;
            else justkps += buffer_up;
            float justkps_per = justkps / KPS_MAX;
            if (justkps_per > 1) justkps_per = 1;


            float kps_per = kps / KPS_MAX;
            if (kps_per > 1) kps_per = 1;
            float progress_kps_left = Bitmap.Width / 2f - (pro_width / 2f - 25) * justkps_per;
            float progress_kps_width = Bitmap.Width / 2f - progress_kps_left;
            var progress_kps = new RectangleF(progress_kps_left, 0 + offset_h / 2f,
                progress_kps_width > 1 ? progress_kps_width : 1f, Bitmap.Height - offset_h);

            float new_red_kps, new_green_kps, new_blue_kps;
            new_red_kps = RED_kps + justkps_per * (RED2_kps - RED_kps);
            new_green_kps = GREEN_kps + justkps_per * (GREEN2_kps - GREEN_kps);
            new_blue_kps = BLUE_kps + justkps_per * (BLUE2_kps - BLUE_kps);
            boostColor_kps = Color.FromArgb((int)new_red_kps, (int)new_green_kps, (int)new_blue_kps);
            Color linearColor_kps = Color.FromArgb(60, (int)(new_red_kps * 0.9), (int)(new_green_kps * 0.9), (int)(new_blue_kps * 0.9));

            Point[] _point_kps = new Point[] {
                new Point((int)(progress_kps.Left),(int)progress_kps.Top),
                new Point((int)(progress_kps.Left), (int)(progress_kps.Top + progress_kps.Height / 2)),
                new Point((int)(progress_kps.Left+ progress_kps.Width), (int)(progress_kps.Top+progress_kps.Height / 2)),
                new Point((int)(progress_kps.Left+ progress_kps.Width), (int)(progress_kps.Top)) };

            PathGradientBrush _setBruhs_kps = new PathGradientBrush(_point_kps, WrapMode.TileFlipY);
            _setBruhs_kps.CenterPoint = new PointF(progress_kps.Left, 0);
            _setBruhs_kps.FocusScales = new PointF(progress_kps.Left + progress_kps.Width, 0);
            _setBruhs_kps.CenterColor = boostColor_kps;
            _setBruhs_kps.SurroundColors = new Color[] { linearColor_kps };

            Graphic.FillRectangle(_setBruhs_kps, progress_kps);

            Bitmap b2;
            Graphics g2;
            b2 = new Bitmap(kira.Width, kira.Height);
            g2 = Graphics.FromImage(b2);
            g2.TranslateTransform(b2.Width / 2f, b2.Height / 2f);
            g2.RotateTransform(rnd.Next(0, 360));
            g2.DrawImage(kira, 0 - kira.Width / 2f, 0 - kira.Height / 2f, b2.Width, b2.Height);
            int ww = 10;
            Graphic.DrawImage(b2, progress_kps.Left - ww / 2f, 0, ww, Bitmap.Height);
            g2.Dispose();
            b2.Dispose();
        }

        public void Close()
        {
            if (t.IsAlive) t.Abort();
            m_GlobalHook.KeyPress -= HookKeyPress;
            //m_GlobalHook.KeyUp -= HookKeyUp;
            m_GlobalHook.Dispose();
        }

        private void CalculateKey()
        {
            while (true)
            {
                Thread.Sleep(1);
                pressTime.RemoveAll(x => sw.ElapsedMilliseconds - x > 1000);
                kps = pressTime.Count;
                if (kps == 0) sw.Restart();
            }
        }

        private void HookKeyPress(object sender, KeyPressEventArgs e)
        {
            //if (currentKey.Contains(e.KeyCode)) return;
            pressTime.Add(sw.ElapsedMilliseconds);
            //currentKey.Add(e.KeyCode);
        }
        private void HookKeyUp(object sender, KeyEventArgs e)
        {
            //currentKey.Remove(e.KeyCode);
        }
    }
}
