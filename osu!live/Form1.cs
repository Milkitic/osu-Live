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
        Stopwatch ts2 = new Stopwatch();
        double fps = 0;
        int fps_count;
        private void action_display_Tick(object sender, EventArgs e)
        {
            if (fps_count == 0)
            {
                fps = 1000f / (ts_fps.ElapsedMilliseconds);
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

            display_g.SmoothingMode = SmoothingMode.AntiAlias;
            display_g.CompositingQuality = CompositingQuality.Invalid;
            display_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            display_g.Clear(Color.Transparent);
            display_g.DrawImage(l_BG.Bitmap, 0, 0);
            display_g.DrawImage(l_PA.Bitmap, 0, 0);

            //display_g.DrawImage(l_PA.Bitmap, 0, 0);
            display_g.DrawImage(l_FG.Bitmap, l_FG.Rec_Panel);

            Color a;
            if (fps >= 60)
                a = Color.FromArgb(172, 220, 25);
            else if (fps >= 45)
                a = Color.FromArgb(255, 204, 34);
            else
                a = Color.FromArgb(255, 149, 24);

            display_g.FillRectangle(new SolidBrush(a), new RectangleF(1196 * zoom, 698 * zoom, canvas_width, canvas_height));
            //display_g.DrawString(Math.Round(fps) + " FPS", new Font("Consolas", 12 * zoom), new SolidBrush(Color.Black), canvas_width - 80 * zoom, canvas_height - 20 * zoom);
            display_g.DrawString(string.Format("{0:0.0}", fps > 60 ? 60 : fps) + " FPS", new Font("Consolas", 12 * zoom), new SolidBrush(Color.Black), canvas_width - 80 * zoom, canvas_height - 20 * zoom);


            display_g.Dispose();
            canvas.Image = display;
            Form1_Resize(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //map_changed_info = new FileInfo(@"Files\l_OsuFileLocation");
            // Form2 fm2 = new Form2();
            //fm2.Show();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Text = string.Format("{0}, {1} (resolution: {2}, {3}) Background: {4}ms{7} Foreground: {5}ms{8}, Particle: {6}ms{9}",
                  ClientRectangle.Width, ClientRectangle.Height, canvas_width, canvas_height, l_BG.DrawTime, l_FG.DrawTime, l_PA.DrawTime,
                    " (INIT " + l_BG.InitializeTime + "ms)",
                    " (INIT " + l_FG.InitializeTime + "ms)",
                    " (INIT " + l_PA.InitializeTime + "ms)");
            //l_BG.InitializeTime != 0 ? " (INIT " + l_BG.InitializeTime + "ms)" : "",
            //      l_FG.InitializeTime != 0 ? " (INIT " + l_FG.InitializeTime + "ms)" : "",
            //      l_PA.InitializeTime != 0 ? " (INIT " + l_PA.InitializeTime + "ms)" : "");

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
                    return;
                }
                //idleStatus = IdleStatus.Playing;
                //todo
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
