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

        // status
        IdleStatus idleStatus = IdleStatus.Listening;

        // var
        string root = null;

        Bitmap display;
        Graphics display_g;
        FileInfo map_changed_info;

        L_background l_BG = new L_background();
        L_foreground l_FG = new L_foreground();
        L_particle l_PA = new L_particle();


        string root_old;
        public Form1()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //开启双缓冲
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {

            Height = (int)((Width - 16) / 1280d * 720) + 39;
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

        private void action_display_Tick(object sender, EventArgs e)
        {
            //canvas.Image = bg_layer;
            if (display != null) display.Dispose();
            display = new Bitmap(canvas_width, canvas_height);
            display_g = Graphics.FromImage(display);
            display_g.Clear(Color.Transparent);
            display_g.DrawImage(l_BG.Bitmap, 0, 0);
            display_g.DrawImage(l_BG.Bitmap_c, 0, 0);
            display_g.DrawImage(l_PA.Bitmap, 0, 0);
            display_g.DrawImage(l_FG.Bitmap, l_FG.Rec_Panel);

            display_g.Dispose();
            canvas.Image = display;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //map_changed_info = new FileInfo(@"Files\l_OsuFileLocation");
            Form2 fm2 = new Form2();
            fm2.Show();
            Size = new Size(1296, 759);
            Size = new Size(800, 600);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Text = ClientRectangle.Width.ToString() + "," + ClientRectangle.Height.ToString();

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
                idleStatus = IdleStatus.Playing;
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
                    l_BG.Graphics_c.Dispose();
                    l_BG.Graphics.Dispose();
                    l_FG.Graphics.Dispose();
                }
                l_BG.Initialize(map_changed_info);
                l_FG.Initialize(map_changed_info);

                if (action_change_info.Enabled) action_change_info.Enabled = false;
                //
            }
            if (!timer_status_change.Enabled) timer_status_change.Enabled = true;
            if (!action_particle.Enabled) action_particle.Enabled = true;
            if (!action_display.Enabled) action_display.Enabled = true;
        }
    }
}
