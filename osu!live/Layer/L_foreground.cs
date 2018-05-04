using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace osu_live.Layer
{
    public class L_foreground : Layer
    {
        string newTitle, oldTitle;
        string newArtist, oldArtist;

        bool isRunned = false;
        int fade_speed = 25;
        char[] artist_list;
        char[] title_list;

        float artistX = 25, artistY = 30;
        float titleX = 20, titleY = 60;
        float shadowOffset = 2;
        float[] artistListX, titleListX;
        float[] artistMovListX, titleMovListX;
        float[] artistMovListX_A = { 15 }, titleMovListX_A = { 20 };
        int[] artistAlphaListX = { 0 }, titleAlphaListX = { 0 };

        float actionCount = -1;

        public void Initialize(FileInfo MapInfo)
        {
            watch.Restart();

            #region INIT
            artistX = 25 * zoom;
            artistY = 30 * zoom;
            titleX = 20 * zoom;
            titleY = 60 * zoom;
            shadowOffset = 2 * zoom;

            int font_panel_y = (int)(CanvasHeight * (600d / 720));
            RecPanel = new Rectangle(-1, font_panel_y, CanvasWidth + 1, CanvasHeight - font_panel_y);
            Bitmap = new Bitmap(RecPanel.Width, RecPanel.Height);

            Graphic = Graphics.FromImage(Bitmap);
            ChangeStatus = ChangeStatus.ReadyToChange;

            oldTitle = newTitle;
            newTitle = SceneListen.GetMapInfo(@"stream\Files\l_TitleUnicode");
            if (oldTitle == null) oldTitle = newTitle;

            oldArtist = newArtist;
            newArtist = SceneListen.GetMapInfo(@"stream\Files\l_ArtistUnicode");
            if (oldArtist == null) oldArtist = newArtist;

            artist_list = newArtist.ToCharArray();
            title_list = newTitle.ToCharArray();
            artistListX = new float[newArtist.Length];
            titleListX = new float[newTitle.Length];

            artistMovListX = new float[newArtist.Length];
            titleMovListX = new float[newTitle.Length];

            artistMovListX_A = new float[newArtist.Length]; //加速度
            titleMovListX_A = new float[newTitle.Length];

            artistAlphaListX = new int[newArtist.Length]; //透明度
            titleAlphaListX = new int[newTitle.Length];

            actionCount = -1;
            isRunned = false;

            for (int i = 0; i < artistMovListX_A.Length; i++)
                artistMovListX_A[i] = 11 * zoom;
            for (int i = 0; i < titleMovListX_A.Length; i++)
                titleMovListX_A[i] = 13 * zoom;
            #endregion

            watch.Stop();
            InitializeTime = watch.ElapsedMilliseconds;
        }
        public void Draw()
        {
            watch.Restart();

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

            font_artist = new Font("等线 Light", 18 * zoom, FontStyle.Regular);
            font_title = new Font("等线 Light", 28 * zoom, FontStyle.Regular);
            if (!isRunned)
            {
                Graphic.SmoothingMode = SmoothingMode.HighQuality;
                Graphic.CompositingQuality = CompositingQuality.HighQuality;
                Graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // calculate each character's position
                // new artist_list
                old_width = 0;
                for (int i = 0; i < artist_list.Length; i++)
                {
                    string chara = artist_list[i].ToString();

                    SizeF sizeF = Graphic.MeasureString(chara, font_artist);
                    if (i == 0)
                        artistListX[i] = artistX;
                    else
                        artistListX[i] = artistListX[i - 1] + old_width;
                    artistMovListX[i] = artistListX[i] + 70 * zoom;
                    if (artist_list[i] == ' ')
                        old_width = 6 * zoom;
                    else
                        old_width = sizeF.Width - 8 * zoom;
                }

                // new title_list
                old_width = 0;
                for (int i = 0; i < title_list.Length; i++)
                {
                    string chara = title_list[i].ToString();
                    SizeF sizeF = Graphic.MeasureString(chara, font_title);
                    if (i == 0)
                        titleListX[i] = titleX;
                    else
                        titleListX[i] = titleListX[i - 1] + old_width;

                    titleMovListX[i] = titleListX[i] + 90 * zoom;

                    if (title_list[i] == ' ')
                        old_width = 10 * zoom;
                    else
                        old_width = sizeF.Width - 13 * zoom;
                }
                isRunned = true;
            }

            //draw artist
            for (int i = 0; i < artist_list.Length; i++)
            {
                if (i > actionCount)
                    continue;
                string chara = artist_list[i].ToString();

                brush = new SolidBrush(Color.FromArgb(artistAlphaListX[i], 255, 255, 255));
                Graphic.DrawString(chara, font_artist, brush, artistMovListX[i], artistY);
                if (artistAlphaListX[i] < 255) //control the alpha degree
                {
                    artistAlphaListX[i] += fade_speed;
                    if (artistAlphaListX[i] > 255) artistAlphaListX[i] = 255;
                }
                if (artistMovListX[i] > artistListX[i]) //keep moving until reach target
                    artistMovListX[i] -= artistMovListX_A[i];
                else artistMovListX[i] = artistListX[i];

                if (artistMovListX_A[i] > 1) artistMovListX_A[i] -= zoom; //control the acceleration
            }

            //draw title
            for (int i = 0; i < title_list.Length; i++)
            {
                if (i > actionCount)
                    continue;
                string chara = title_list[i].ToString();

                brush = new SolidBrush(Color.FromArgb(titleAlphaListX[i], 255, 255, 255));
                Graphic.DrawString(chara, font_title, brush, titleMovListX[i], titleY);
                if (titleAlphaListX[i] < 255)
                {
                    titleAlphaListX[i] += fade_speed;
                    if (titleAlphaListX[i] > 255) titleAlphaListX[i] = 255;
                }
                if (titleMovListX[i] > titleListX[i])
                    titleMovListX[i] -= titleMovListX_A[i];
                else titleMovListX[i] = titleListX[i];

                if (titleMovListX_A[i] > 1) titleMovListX_A[i] -= zoom;
            }
            actionCount++;

            //if finished then redraw
            if ((artistListX.Length == 0 || artistMovListX[artistListX.Length - 1] <= artistListX[artistListX.Length - 1])
                && (titleListX.Length == 0 || titleMovListX[titleListX.Length - 1] <= titleListX[titleListX.Length - 1]))
            {
                Clear();
                for (int i = 0; i < artist_list.Length; i++)
                {
                    brush = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
                    string chara = artist_list[i].ToString();
                    Graphic.DrawString(chara, font_artist, brush, artistListX[i] + shadowOffset, artistY + shadowOffset);
                    brush = new SolidBrush(Color.White);
                    Graphic.DrawString(chara, font_artist, brush, artistListX[i], artistY);
                }
                for (int i = 0; i < title_list.Length; i++)
                {
                    string chara = title_list[i].ToString();
                    brush = new SolidBrush(Color.FromArgb(70, 0, 0, 0));
                    Graphic.DrawString(chara, font_title, brush, titleListX[i] + shadowOffset, titleY + shadowOffset);
                    brush = new SolidBrush(Color.White);
                    Graphic.DrawString(chara, font_title, brush, titleListX[i], titleY);
                }
                ChangeStatus = ChangeStatus.ChangeFinshed;
                Graphic.Dispose();
                watch.Stop();
                DrawTime = 0;
                return;
            }

            DrawTime = watch.ElapsedMilliseconds;
            watch.Stop();
        }

        private void Clear()
        {
            Graphic.Clear(Color.FromArgb(0, 0, 0, 0));
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(0, 0), new Point(0, RecPanel.Height), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(200, 0, 0, 0));
            Graphic.FillRectangle(linGrBrush, 0, 0, RecPanel.Width, RecPanel.Height);
        }
    }
}
