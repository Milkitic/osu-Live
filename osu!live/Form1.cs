using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using osu_live.Layer;

namespace osu_live
{
    public partial class Form1 : Form
    {
        // const
        int canvas_width = Constant.Canvas.Width,
            canvas_height = Constant.Canvas.Height;
        float zoom = (float)Constant.Canvas.Zoom;
        bool showFPS = false;

        // status
        public static IdleStatus idleStatus = IdleStatus.Stopped;

        // var
        string root = null;

        Bitmap display;
        Graphics display_g;
        FileInfo map_changed_info;
        FileInfo diff_playing_info;

        public static L_background l_BG = new L_background();
        public static L_foreground l_FG = new L_foreground();
        public static L_particle l_PA = new L_particle();

        long refreshDelay;

        string root_old, diff;
        public Form1()
        {
            InitializeComponent();
            Size = new Size((int)(1280 * Constant.Canvas.Zoom) + 16,
             (int)(720 * Constant.Canvas.Zoom) + 39);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //开启双缓冲
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void DrawFPS()
        {
            Color a;
            if (fps >= 60)
                a = Color.FromArgb(172, 220, 25);
            else if (fps >= 45)
                a = Color.FromArgb(255, 204, 34);
            else
                a = Color.FromArgb(255, 149, 24);

            display_g.FillRectangle(new SolidBrush(a), new RectangleF(1196 * zoom, 698 * zoom, canvas_width, canvas_height));
            display_g.DrawString(string.Format("{0:0.0}", fps > 60 ? 60 : fps) + " FPS", new Font("Consolas", 12 * zoom), new SolidBrush(Color.Black), canvas_width - 80 * zoom, canvas_height - 20 * zoom);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {


        }

        private void timer_status_change_Tick(object sender, EventArgs e)
        {
            if (idleStatus == IdleStatus.Listening && l_BG.ChangeStatus == ChangeStatus.ReadyToChange)
            {
                l_BG.ChangeStatus = ChangeStatus.Changing;
                l_FG.ChangeStatus = ChangeStatus.Changing;
                action_change_bg.Enabled = true;
                action_change_info.Enabled = true;
            }
        }

        private void action_change_info_Tick(object sender, EventArgs e)
        {
            if (l_FG.ChangeStatus != ChangeStatus.Changing)
            {
                action_change_info.Enabled = false;
                return;
            }
            l_FG.Draw();
        }
        private void action_particle_Tick(object sender, EventArgs e)
        {
            l_PA.Draw();
        }

        private void action_change_bg_Tick(object sender, EventArgs e)
        {
            if (l_BG.ChangeStatus != ChangeStatus.Changing)
            {
                action_change_bg.Enabled = false;
                return;
            }
            l_BG.Draw();
        }
        Stopwatch ts_fps = new Stopwatch();
        double fps = 0;
        int fps_count;
        float angle = 0;
        float scale = 0, scale_r;
        private void action_display_Tick(object sender, EventArgs e)
        {
            if (fps_count == 0)
            {
                refreshDelay = ts_fps.ElapsedMilliseconds;
                fps = 1000f / (refreshDelay);
                fps_count++;
            }
            else if (fps_count == 1)
                fps_count = 0;
            else
                fps_count++;

            ts_fps.Restart();

            if (display != null) display.Dispose();
            display = new Bitmap(canvas_width, canvas_height);
            display_g = Graphics.FromImage(display);

            display_g.SmoothingMode = SmoothingMode.HighQuality;
            display_g.CompositingQuality = CompositingQuality.Invalid;
            display_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            scale_r = (float)(Math.Cos(scale * Math.PI / 180) * 0.25 + 0.75);
            display_g.ScaleTransform(scale_r, scale_r);
            display_g.TranslateTransform((canvas_width - canvas_width * scale_r) / 2, (canvas_height - canvas_height * scale_r) / 2);

            display_g.TranslateTransform(canvas_width / 2, canvas_height / 2);
            display_g.RotateTransform(angle);
            display_g.TranslateTransform(-canvas_width / 2, -canvas_height / 2);

            display_g.Clear(Color.Transparent);
            display_g.DrawImage(l_BG.Bitmap, 0, 0);
            display_g.DrawImage(l_PA.Bitmap, 0, 0);
            display_g.DrawImage(l_FG.Bitmap, l_FG.Rec_Panel);
            if (showFPS) DrawFPS();

            display_g.Dispose();
            canvas.Image = display;
            Form1_Resize(sender, e);
            angle += 0.5f;
            scale += 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //map_changed_info = new FileInfo(@"Files\l_OsuFileLocation");
            // Form2 fm2 = new Form2();
            //fm2.Show();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Text = string.Format("({0}, {1}) Background: {2}ms{5} Foreground: {3}ms{6}, Particle: {4}ms{7}. Refresh: {8}ms",
                   canvas_width, canvas_height, l_BG.DrawTime, l_FG.DrawTime, l_PA.DrawTime,
                   l_BG.InitializeTime != 0 ? " (INIT " + l_BG.InitializeTime + "ms)" : "",
                   l_FG.InitializeTime != 0 ? " (INIT " + l_FG.InitializeTime + "ms)" : "",
                   l_PA.InitializeTime != 0 ? " (INIT " + l_PA.InitializeTime + "ms)" : "",
                   refreshDelay);
        }

        private void timer_status_check_Tick(object sender, EventArgs e)
        {
            FileInfo tmp = new FileInfo(@"Files\l_OsuFileLocation");
            if (map_changed_info != null && tmp.LastWriteTime == map_changed_info.LastWriteTime)
                return;
            map_changed_info = tmp;
            root_old = root;
            try
            {
                root = File.ReadAllText(map_changed_info.FullName);
            }
            catch
            {
                return;
            }
            if (root.Trim() == "")
            {
                FileInfo tmp2 = new FileInfo(@"Files\l_DiffName");
                if (diff_playing_info != null && tmp2.LastWriteTime == diff_playing_info.LastWriteTime)
                    return;
                diff_playing_info = tmp2;
                try
                {
                    diff = File.ReadAllText(diff_playing_info.FullName);
                }
                catch
                {
                    return;
                }
                if (diff.Trim() != "")
                {
                    idleStatus = IdleStatus.Playing;
                    //todo
                    return;
                }
            }
            else
            {
                idleStatus = IdleStatus.Listening;

                if (root == root_old) return;

                /// Initialize
                if (!timer_status_change.Enabled)
                {
                    //first run
                    l_PA.Initialize();
                }
                else
                {
                    l_BG.Graphics.Dispose();
                    l_FG.Graphics.Dispose();
                }
                l_BG.Initialize(map_changed_info);
                l_FG.Initialize(map_changed_info);

                if (action_change_info.Enabled) action_change_info.Enabled = false;
                //
            }
            if (idleStatus != IdleStatus.Stopped)
            {
                if (!timer_status_change.Enabled) timer_status_change.Enabled = true;
                if (!action_particle.Enabled) action_particle.Enabled = true;
                if (!action_display.Enabled) action_display.Enabled = true;
            }
        }
    }
}
