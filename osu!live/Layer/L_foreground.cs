﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace osu_live.Layer
{
    class L_foreground
    {
        public Bitmap Bitmap { get; set; }
        public Graphics Graphics { get; set; }
        public Rectangle Rec_Panel { get; set; }
        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.ReadyToChange;

        int canvas_height = Constant.Canvas.Height;
        int canvas_width = Constant.Canvas.Width;

        string newTitle, oldTitle;
        string newArtist, oldArtist;

        bool isRunned = false;
        bool isFadeIn = false;
        int fade_speed = 25;
        char[] artist_list;
        char[] title_list;

        int artist_x = 25, artist_y = 30;
        int title_x = 20, title_y = 60;
        int shadow_offset = 2;
        float[] artist_x_list_std, title_x_list_std;
        float[] artist_x_list_moving, title_x_list_moving;
        float[] artist_x_list_moving_a = { 15 }, title_x_list_moving_a = { 20 };
        int[] artist_x_list_alpha = { 0 }, title_x_list_alpha = { 0 };

        int action_info_counter = -1;

        public void Initialize(FileInfo MapInfo)
        {
            int font_panel_y = (int)(canvas_height * (600d / 720));
            Rec_Panel = new Rectangle(0, font_panel_y, canvas_width, canvas_height - font_panel_y);
            Bitmap = new Bitmap(Rec_Panel.Width, Rec_Panel.Height);

            Graphics = Graphics.FromImage(Bitmap);
            ChangeStatus = ChangeStatus.ReadyToChange;

            oldTitle = newTitle;
            newTitle = SceneListen.GetMapInfo(@"Files\l_TitleUnicode");
            if (oldTitle == null) oldTitle = newTitle;

            oldArtist = newArtist;
            newArtist = SceneListen.GetMapInfo(@"Files\l_ArtistUnicode");
            if (oldArtist == null) oldArtist = newArtist;

            artist_list = newArtist.ToCharArray();
            title_list = newTitle.ToCharArray();
            artist_x_list_std = new float[newArtist.Length];
            title_x_list_std = new float[newTitle.Length];

            artist_x_list_moving = new float[newArtist.Length];
            title_x_list_moving = new float[newTitle.Length];

            artist_x_list_moving_a = new float[newArtist.Length]; //加速度
            title_x_list_moving_a = new float[newTitle.Length];

            artist_x_list_alpha = new int[newArtist.Length]; //透明度
            title_x_list_alpha = new int[newTitle.Length];

            action_info_counter = -1;
            isRunned = false;

            for (int i = 0; i < artist_x_list_moving_a.Length; i++)
                artist_x_list_moving_a[i] = 11;
            for (int i = 0; i < title_x_list_moving_a.Length; i++)
                title_x_list_moving_a[i] = 13;
        }
        public void Draw()
        {
            Clear();
            Font font_artist, font_title;
            Brush brush;
            float old_width;

            #region more font style
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            //font_layer_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            #endregion

            font_artist = new Font("等线 Light", 18, FontStyle.Regular);
            font_title = new Font("等线 Light", 28, FontStyle.Regular);
            if (!isRunned)
            {
                Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // calculate each character's position
                // new artist_list
                old_width = 0;
                for (int i = 0; i < artist_list.Length; i++)
                {
                    string chara = artist_list[i].ToString();

                    SizeF sizeF = Graphics.MeasureString(chara, font_artist);
                    if (i == 0)
                        artist_x_list_std[i] = artist_x;
                    else
                        artist_x_list_std[i] = artist_x_list_std[i - 1] + old_width;
                    artist_x_list_moving[i] = artist_x_list_std[i] + 70;
                    if (artist_list[i] == ' ')
                        old_width = 6;
                    else
                        old_width = sizeF.Width - 8;
                }

                // new title_list
                old_width = 0;
                for (int i = 0; i < title_list.Length; i++)
                {
                    string chara = title_list[i].ToString();
                    SizeF sizeF = Graphics.MeasureString(chara, font_title);
                    if (i == 0)
                        title_x_list_std[i] = title_x;
                    else
                        title_x_list_std[i] = title_x_list_std[i - 1] + old_width;

                    title_x_list_moving[i] = title_x_list_std[i] + 90;

                    if (title_list[i] == ' ')
                        old_width = 10;
                    else
                        old_width = sizeF.Width - 13;
                }
                isRunned = true;
            }

            //draw artist
            for (int i = 0; i < artist_list.Length; i++)
            {
                if (i > action_info_counter)
                    continue;
                string chara = artist_list[i].ToString();

                brush = new SolidBrush(Color.FromArgb(artist_x_list_alpha[i], 255, 255, 255));
                Graphics.DrawString(chara, font_artist, brush, artist_x_list_moving[i], artist_y);
                if (artist_x_list_alpha[i] < 255) //control the alpha degree
                {
                    artist_x_list_alpha[i] += fade_speed;
                    if (artist_x_list_alpha[i] > 255) artist_x_list_alpha[i] = 255;
                }
                if (artist_x_list_moving[i] > artist_x_list_std[i]) //keep moving until reach target
                    artist_x_list_moving[i] -= artist_x_list_moving_a[i];
                else artist_x_list_moving[i] = artist_x_list_std[i];

                if (artist_x_list_moving_a[i] > 1) artist_x_list_moving_a[i]--; //control the acceleration
            }

            //draw title
            for (int i = 0; i < title_list.Length; i++)
            {
                if (i > action_info_counter)
                    continue;
                string chara = title_list[i].ToString();

                brush = new SolidBrush(Color.FromArgb(title_x_list_alpha[i], 255, 255, 255));
                Graphics.DrawString(chara, font_title, brush, title_x_list_moving[i], title_y);
                if (title_x_list_alpha[i] < 255)
                {
                    title_x_list_alpha[i] += fade_speed;
                    if (title_x_list_alpha[i] > 255) title_x_list_alpha[i] = 255;
                }
                if (title_x_list_moving[i] > title_x_list_std[i])
                    title_x_list_moving[i] -= title_x_list_moving_a[i];
                else title_x_list_moving[i] = title_x_list_std[i];

                if (title_x_list_moving_a[i] > 1) title_x_list_moving_a[i]--;
            }
            action_info_counter++;

            //if finished then redraw
            if ((artist_x_list_std.Length == 0 || artist_x_list_moving[artist_x_list_std.Length - 1] <= artist_x_list_std[artist_x_list_std.Length - 1])
                && (title_x_list_std.Length == 0 || title_x_list_moving[title_x_list_std.Length - 1] <= title_x_list_std[title_x_list_std.Length - 1]))
            {
                Clear();
                for (int i = 0; i < artist_list.Length; i++)
                {
                    brush = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
                    string chara = artist_list[i].ToString();
                    Graphics.DrawString(chara, font_artist, brush, artist_x_list_std[i] + shadow_offset, artist_y + shadow_offset);
                    brush = new SolidBrush(Color.White);
                    Graphics.DrawString(chara, font_artist, brush, artist_x_list_std[i], artist_y);
                }
                for (int i = 0; i < title_list.Length; i++)
                {
                    string chara = title_list[i].ToString();
                    brush = new SolidBrush(Color.FromArgb(70, 0, 0, 0));
                    Graphics.DrawString(chara, font_title, brush, title_x_list_std[i] + shadow_offset, title_y + shadow_offset);
                    brush = new SolidBrush(Color.White);
                    Graphics.DrawString(chara, font_title, brush, title_x_list_std[i], title_y);
                }
                ChangeStatus = ChangeStatus.ChangeFinshed;
            }
        }

        private void Clear()
        {
            Graphics.Clear(Color.FromArgb(80, 0, 0, 0));
        }
    }
}
