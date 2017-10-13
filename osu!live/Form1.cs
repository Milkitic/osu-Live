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

namespace osu_live
{
    public partial class Form1 : Form
    {
        // const
        int canvas_width = 1280,
            canvas_height = 720;
        //int canvas_width = 1024,
        //  canvas_height = 576;
        float bg_pos_x = 0,
           bg_pos_y = 0;
        float bg_width, bg_height;

        float bg_pos_x_old = 0,
          bg_pos_y_old = 0;
        float bg_width_old, bg_height_old;

        Rectangle font_panel;

        // status
        //IdleStatus idleStatus_old = IdleStatus.Listening;
        IdleStatus idleStatus = IdleStatus.Listening;
        ChangeStatus bg_changeStatus = ChangeStatus.ReadyToChange;
        ChangeStatus font_changeStatus = ChangeStatus.ReadyToChange;

        // var
        string root = null;

        Bitmap display;
        Graphics display_g;
        FileInfo map_changed_info;
        Image current_bg_old, current_bg;
        string current_title, current_title_old;
        string current_artist, current_artist_old;
        // bg_layer
        Bitmap bg_layer;
        Graphics bg_layer_g;
        Bitmap bg_layer_cover;
        Graphics bg_layer_cover_g;

        Bitmap bg_layer_particle;
        Graphics bg_layer_particle_g;

        // font_layer
        Bitmap font_layer;
        Graphics font_layer_g;

        string root_old;
        public Form1()
        {
            InitializeComponent();
            //  TODO:  在  InitComponent  调用后添加任何初始化 
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //开启双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {

            Height = (int)((Width - 16) / 1280d * 720) + 39;
        }

        private void timer_status_change_Tick(object sender, EventArgs e)
        {
            if (idleStatus == IdleStatus.Listening && bg_changeStatus == ChangeStatus.ReadyToChange)
            {
                bg_changeStatus = ChangeStatus.Changing;
                font_changeStatus = ChangeStatus.Changing;
                bg_width = 0;
                bg_height = 0;
                action_change_bg.Enabled = true;
                action_change_info.Enabled = true;
            }
        }

        bool fore_fade_flag = false;
        bool fore_fade_in = false;
        int fore_fade_speed = 80;
        char[] current_artist_list;
        char[] current_title_list;

        int artist_x = 25, artist_y = 30;
        int title_x = 20, title_y = 60;
        int shadow_offset = 2;
        float[] artist_x_list, title_x_list;
        float[] artist_x_list_moving, title_x_list_moving;
        float[] artist_x_list_moving_a = { 15 }, title_x_list_moving_a = { 20 };
        int[] artist_x_list_alpha = { 0 }, title_x_list_alpha = { 0 };
        int action_info_counter = -1;
        private void action_change_info_Tick(object sender, EventArgs e)
        {
            if (font_changeStatus != ChangeStatus.Changing)
            {
                //font_layer_g.Dispose();
                action_change_info.Enabled = false;
                return;
            }
            font_layer_g.Clear(Color.FromArgb(80, 0, 0, 0));

            Font font_artist, font_title;
            Brush brush;

            //font_artist = new Font("等线 Light", 18, FontStyle.Regular);
            //font_title = new Font("等线 Light", 28, FontStyle.Regular);

            if (!fore_fade_in)
            {
                font_layer_g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                font_layer_g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            float pre_width = 0;
            //if (!fore_fade_flag)
            //{
            //    for (int i = 0; i < current_artist_list.Length; i++)
            //    {
            //        string chara = current_artist_list[i].ToString();

            //        SizeF sizeF = font_layer_g.MeasureString(chara, font_artist);
            //        pre_width = sizeF.Width;
            //        artist_x = artist_x + pre_width;
            //    }

            //}
            #region more style
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            #endregion


            font_artist = new Font("等线 Light", 18, FontStyle.Regular);
            font_title = new Font("等线 Light", 28, FontStyle.Regular);
            if (!fore_fade_flag)
            {
                // new artist_list position
                pre_width = 0;
                for (int i = 0; i < current_artist_list.Length; i++)
                {
                    string chara = current_artist_list[i].ToString();

                    SizeF sizeF = font_layer_g.MeasureString(chara, font_artist);
                    if (i == 0)
                    {
                        artist_x_list[i] = artist_x;
                    }
                    else
                    {
                        artist_x_list[i] = artist_x_list[i - 1] + pre_width;
                    }
                    artist_x_list_moving[i] = artist_x_list[i] + 70;
                    if (current_artist_list[i] == ' ')
                        pre_width = 6;
                    else
                        pre_width = sizeF.Width - 8;
                }

                // new title_list position
                pre_width = 0;
                for (int i = 0; i < current_title_list.Length; i++)
                {
                    string chara = current_title_list[i].ToString();

                    SizeF sizeF = font_layer_g.MeasureString(chara, font_title);
                    if (i == 0)
                    {
                        title_x_list[i] = title_x;
                    }
                    else
                    {
                        title_x_list[i] = title_x_list[i - 1] + pre_width;
                    }
                    title_x_list_moving[i] = title_x_list[i] + 90;

                    if (current_title_list[i] == ' ')
                        pre_width = 10;
                    else
                        pre_width = sizeF.Width - 13;
                }
                fore_fade_flag = true;
            }

            //font_layer_g.DrawString(current_artist, font_artist, brush, artist_x, artist_y);
            for (int i = 0; i < current_artist_list.Length; i++)
            {
                if (i > action_info_counter)
                    continue;
                string chara = current_artist_list[i].ToString();

                brush = new SolidBrush(Color.FromArgb(artist_x_list_alpha[i], 255, 255, 255));
                font_layer_g.DrawString(chara, font_artist, brush, artist_x_list_moving[i], artist_y);
                if (artist_x_list_alpha[i] < 255) //控制透明度
                {
                    artist_x_list_alpha[i] += 25;
                    if (artist_x_list_alpha[i] > 255) artist_x_list_alpha[i] = 255;
                }
                if (artist_x_list_moving[i] > artist_x_list[i]) //控制一直移动，直到到目标位置
                    artist_x_list_moving[i] -= artist_x_list_moving_a[i];
                else artist_x_list_moving[i] = artist_x_list[i];

                if (artist_x_list_moving_a[i] > 1) artist_x_list_moving_a[i]--; //控制加速度
            }
            for (int i = 0; i < current_title_list.Length; i++)
            {
                if (i > action_info_counter)
                    continue;
                string chara = current_title_list[i].ToString();

                brush = new SolidBrush(Color.FromArgb(title_x_list_alpha[i], 255, 255, 255));
                font_layer_g.DrawString(chara, font_title, brush, title_x_list_moving[i], title_y);
                if (title_x_list_alpha[i] < 255)
                {
                    title_x_list_alpha[i] += 25;
                    if (title_x_list_alpha[i] > 255) title_x_list_alpha[i] = 255;
                }
                if (title_x_list_moving[i] > title_x_list[i])
                    title_x_list_moving[i] -= title_x_list_moving_a[i];
                else title_x_list_moving[i] = title_x_list[i];

                if (title_x_list_moving_a[i] > 1) title_x_list_moving_a[i]--;
            }

            action_info_counter++;
            if ((artist_x_list.Length == 0 || artist_x_list_moving[artist_x_list.Length - 1] <= artist_x_list[artist_x_list.Length - 1])
                && (title_x_list.Length == 0 || title_x_list_moving[title_x_list.Length - 1] <= title_x_list[title_x_list.Length - 1]))
            {
                font_layer_g.Clear(Color.FromArgb(80, 0, 0, 0));
                for (int i = 0; i < current_artist_list.Length; i++)
                {
                    brush = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
                    string chara = current_artist_list[i].ToString();
                    font_layer_g.DrawString(chara, font_artist, brush, artist_x_list[i] + shadow_offset, artist_y + shadow_offset);
                    brush = new SolidBrush(Color.White);
                    font_layer_g.DrawString(chara, font_artist, brush, artist_x_list[i], artist_y);
                }
                for (int i = 0; i < current_title_list.Length; i++)
                {
                    string chara = current_title_list[i].ToString();
                    brush = new SolidBrush(Color.FromArgb(70, 0, 0, 0));
                    font_layer_g.DrawString(chara, font_title, brush, title_x_list[i] + shadow_offset, title_y + shadow_offset);
                    brush = new SolidBrush(Color.White);
                    font_layer_g.DrawString(chara, font_title, brush, title_x_list[i], title_y);

                }
                font_changeStatus = ChangeStatus.ChangeFinshed;
            }
        }
        RectangleF rec;
        private void action_particle_Tick(object sender, EventArgs e)
        {
            rec.Y -= 2;
            //rec.X += 1;
            if (rec.Y < 0) rec.Y = canvas.Height;
            //rec.Height -= 1;
            try
            {
                bg_layer_particle_g.Clear(Color.Transparent);
                // rec.Y -= 1;
                //rec.Height -= 1;
                //bg_layer_particle_g.DrawRectangle(new Pen(Color.FromArgb(255, 255, 255, 255)), rec);
                bg_layer_particle_g.FillRectangle(new SolidBrush(Color.FromArgb(64, 123, 53, 230)), rec);
            }
            catch { }
        }

        int bg_fade = 0;
        bool bg_fade_in = false;
        bool bg_fade_flag = false;
        int bg_fade_speed = 80;
        private void action_change_bg_Tick(object sender, EventArgs e)
        {
            if (bg_changeStatus != ChangeStatus.Changing)
            {
                //bg_layer_g.Dispose();
                //bg_layer_cover_g.Dispose();
                action_change_bg.Enabled = false;
                return;
            }
            if (bg_width == 0)
            {
                // deal with different size of image
                if ((float)current_bg.Width / current_bg.Height > 16 / 9f) // more width
                {
                    float scale = (float)canvas_height / current_bg.Height;
                    bg_height = canvas_height;
                    bg_width = current_bg.Width * scale;
                    bg_pos_x = -(bg_width - canvas_width) / 2;
                    bg_pos_y = 0;
                }
                else if ((float)current_bg.Width / current_bg.Height < 16 / 9f) // more height
                {
                    float scale = (float)canvas_width / current_bg.Width;
                    bg_width = canvas_width;
                    bg_height = current_bg.Height * scale;
                    bg_pos_x = 0;
                    bg_pos_y = -(bg_height - canvas_height) / 2;
                }
                else
                {
                    bg_width = canvas_width;
                    bg_height = canvas_height;
                    bg_pos_x = 0;
                    bg_pos_y = 0;
                }
            }
            bg_layer_cover_g.Clear(Color.Transparent);
            if (!bg_fade_in)
            {
                if (bg_fade_flag == false)
                {
                    bg_layer_g.DrawImage(current_bg_old, bg_pos_x_old, bg_pos_y_old, bg_width_old, bg_height_old);
                    bg_fade_flag = true;
                }

                bg_layer_cover_g.FillRectangle(new SolidBrush(Color.FromArgb(bg_fade, 0, 0, 0)), new Rectangle(0, 0, canvas_width, canvas_height));

                bg_fade += bg_fade_speed;
                if (bg_fade >= 255)
                    bg_fade = 255;
            }
            else
            {
                if (bg_fade_flag == true)
                {
                    bg_layer_g.DrawImage(current_bg, bg_pos_x, bg_pos_y, bg_width, bg_height);
                    bg_fade_flag = false;
                }

                bg_layer_cover_g.FillRectangle(new SolidBrush(Color.FromArgb(bg_fade, 0, 0, 0)), new Rectangle(0, 0, canvas_width, canvas_height));
                bg_fade -= bg_fade_speed;
                if (bg_fade <= 0) bg_fade = 0;
            }
            if (bg_fade >= 255)
            {
                bg_fade_in = true;
            }
            else if (bg_fade == 0) bg_changeStatus = ChangeStatus.ChangeFinshed;
        }

        private void action_display_Tick(object sender, EventArgs e)
        {
            //canvas.Image = bg_layer;
            if (display != null) display.Dispose();
            display = new Bitmap(canvas_width, canvas_height);
            display_g = Graphics.FromImage(display);
            display_g.Clear(Color.Transparent);
            display_g.DrawImage(bg_layer, 0, 0);
            display_g.DrawImage(bg_layer_cover, 0, 0);
            display_g.DrawImage(bg_layer_particle, 0, 0);
            display_g.DrawImage(font_layer, font_panel);

            display_g.Dispose();
            canvas.Image = display;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            map_changed_info = new FileInfo(@"Files\l_OsuFileLocation");
        }

        private void timer_status_check_Tick(object sender, EventArgs e)
        {
            FileInfo tmp = new FileInfo(@"Files\l_OsuFileLocation");
            if (tmp.LastWriteTime == map_changed_info.LastWriteTime)
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
            if (root.Trim() == "") idleStatus = IdleStatus.Playing;
            else if (root == root_old) return;
            else
            {
                //idleStatus_old = idleStatus;
                idleStatus = IdleStatus.Listening;
                //if (idleStatus_old == idleStatus) return;

                /// Initialize
                if (!timer_status_change.Enabled)
                {
                    bg_layer = new Bitmap(canvas_width, canvas_height);
                    bg_layer_cover = new Bitmap(canvas_width, canvas_height);
                    int font_panel_y = (int)(canvas_height * (600d / 720));
                    font_panel = new Rectangle(0, font_panel_y, canvas_width, canvas_height - font_panel_y);
                    font_layer = new Bitmap(font_panel.Width, font_panel.Height);

                    bg_layer_particle = new Bitmap(canvas_width, canvas_height);
                    bg_layer_particle_g = Graphics.FromImage(bg_layer_particle);
                    bg_layer_particle_g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    bg_layer_particle_g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    rec = new RectangleF(640, 400, 20, 20);
                }
                else
                {
                    bg_layer_cover_g.Dispose();
                    bg_layer_g.Dispose();
                    font_layer_g.Dispose();
                    //bg_layer_particle_g.Dispose();
                }

                // bgLayer
                bg_layer_cover_g = Graphics.FromImage(bg_layer_cover);
                bg_layer_g = Graphics.FromImage(bg_layer);

                bg_pos_x_old = bg_pos_x;
                bg_pos_y_old = bg_pos_y;
                bg_width_old = bg_width;
                bg_height_old = bg_height;

                bg_changeStatus = ChangeStatus.ReadyToChange;
                bg_fade = 0;
                bg_fade_in = false;

                current_bg_old = current_bg;
                current_bg = SceneListen.GetMapBG(map_changed_info);
                if (current_bg_old == null) current_bg_old = current_bg;

                // fontLayer
                font_layer_g = Graphics.FromImage(font_layer);
                font_changeStatus = ChangeStatus.ReadyToChange;

                current_title_old = current_title;
                current_title = SceneListen.GetMapInfo(@"Files\l_TitleUnicode");
                if (current_title_old == null) current_title_old = current_title;

                current_artist_old = current_artist;
                current_artist = SceneListen.GetMapInfo(@"Files\l_ArtistUnicode");
                if (current_artist_old == null) current_artist_old = current_artist;

                current_artist_list = current_artist.ToCharArray();
                current_title_list = current_title.ToCharArray();
                artist_x_list = new float[current_artist.Length];
                title_x_list = new float[current_title.Length];

                artist_x_list_moving = new float[current_artist.Length];
                title_x_list_moving = new float[current_title.Length];

                artist_x_list_moving_a = new float[current_artist.Length]; //加速度
                title_x_list_moving_a = new float[current_title.Length];

                artist_x_list_alpha = new int[current_artist.Length]; //透明度
                title_x_list_alpha = new int[current_title.Length];

                action_info_counter = -1;
                fore_fade_flag = false;

                for (int i = 0; i < artist_x_list_moving_a.Length; i++)
                    artist_x_list_moving_a[i] = 11;
                for (int i = 0; i < title_x_list_moving_a.Length; i++)
                    title_x_list_moving_a[i] = 13;
                if (action_change_info.Enabled) action_change_info.Enabled = false;
                //
                if (!timer_status_change.Enabled) timer_status_change.Enabled = true;
                if (!action_display.Enabled) action_display.Enabled = true;
                if (!action_particle.Enabled) action_particle.Enabled = true;
            }
        }
    }
}
