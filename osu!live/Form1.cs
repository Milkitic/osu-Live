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
        // CAMERA & CANVAS
        int CanvasWidth { get; set; } = Constant.Canvas.Width;
        int CanvasHeight { get; set; } = Constant.Canvas.Height;
        float CanvasZoom { get; set; } = (float)Constant.Canvas.Zoom;

        float CameraDeg { get; set; } = 0;
        float CameraSca { get; set; } = 0;
        float CameraDegR { get; set; }
        float CameraScaR { get; set; }

        // FPS
        Stopwatch FpsWatch { get; set; } = new Stopwatch();
        double Fps { get; set; } = 0;
        public static bool ShowFps { get; set; } = false;
        long RefreshDelay { get; set; }
        int fpsCount;

        // STATUS
        public static IdleStatus IdleStatus { get; set; } = IdleStatus.Stopped;

        // IO
        string Root { get; set; } = null;
        string RootOld { get; set; }
        FileInfo MapInfo { get; set; }
        FileInfo PlayInfo { get; set; }
        string PlayDiff;

        // GDI+
        Bitmap bitmap;
        Graphics graphic;
        public static L_background layerBack = new L_background();
        public static L_foreground layerFore = new L_foreground();
        //public static L_particle layerParticle = new L_particle();
        public static L_DashAim layerDashAim = new L_DashAim();

        public Form1()
        {
            InitializeComponent();
            // Set default size
            Size = new Size((int)(1280 * Constant.Canvas.Zoom) + 16,
             (int)(720 * Constant.Canvas.Zoom) + 39);

            //开启双缓冲
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private void DrawFPS()
        {
            Color color;
            if (Fps >= 60)
                color = Color.FromArgb(172, 220, 25);
            else if (Fps >= 45)
                color = Color.FromArgb(255, 204, 34);
            else
                color = Color.FromArgb(255, 149, 24);

            graphic.FillRectangle(new SolidBrush(color), new RectangleF(1196 * CanvasZoom, 698 * CanvasZoom, CanvasWidth, CanvasHeight));
            graphic.DrawString(string.Format("{0:0.0}", Fps > 60 ? 60 : Fps) + " FPS", new Font("Consolas", 12 * CanvasZoom), new SolidBrush(Color.Black), CanvasWidth - 80 * CanvasZoom, CanvasHeight - 20 * CanvasZoom);
        }

        private void timer_status_change_Tick(object sender, EventArgs e)
        {
            if (IdleStatus == IdleStatus.Listening && layerBack.ChangeStatus == ChangeStatus.ReadyToChange)
            {
                layerBack.ChangeStatus = ChangeStatus.Changing;
                layerFore.ChangeStatus = ChangeStatus.Changing;
                action_change_bg.Enabled = true;
                action_change_info.Enabled = true;
            }
        }

        private void action_change_info_Tick(object sender, EventArgs e)
        {
            if (layerFore.ChangeStatus != ChangeStatus.Changing)
            {
                action_change_info.Enabled = false;
                return;
            }
            layerFore.Draw();
        }
        private void action_particle_Tick(object sender, EventArgs e)
        {
            //layerParticle.Draw();
            layerDashAim.Draw();
        }

        private void action_change_bg_Tick(object sender, EventArgs e)
        {
            if (layerBack.ChangeStatus != ChangeStatus.Changing)
            {
                action_change_bg.Enabled = false;
                return;
            }
            layerBack.Draw();
        }

        Random rnd = new Random();
        private void action_display_Tick(object sender, EventArgs e)
        {
            if (fpsCount == 0)
            {
                RefreshDelay = FpsWatch.ElapsedMilliseconds;
                Fps = 1000f / (RefreshDelay);
                fpsCount++;
            }
            else if (fpsCount == 1)
                fpsCount = 0;
            else
                fpsCount++;

            FpsWatch.Restart();

            if (bitmap != null) bitmap.Dispose();
            bitmap = new Bitmap(CanvasWidth, CanvasHeight);
            graphic = Graphics.FromImage(bitmap);
            //display_g = Graphics.FromHwnd(canvas.Handle);
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.Invalid;
            graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            #region 一个五毛特效
            //CameraScaR = (float)(Math.Cos(CameraSca * Math.PI / 180) * 0.01 + 1.013);
            //CameraDegR = (float)(Math.Cos(CameraDeg * Math.PI / 180) * 0.2);
            //graphic.ScaleTransform(CameraScaR, CameraScaR);
            //graphic.TranslateTransform((CanvasWidth - CanvasWidth * CameraScaR) / 2, (CanvasHeight - CanvasHeight * CameraScaR) / 2);

            //graphic.TranslateTransform(CanvasWidth / 2, CanvasHeight / 2);
            //graphic.RotateTransform(CameraDegR);
            //graphic.TranslateTransform(-CanvasWidth / 2, -CanvasHeight / 2);
            #endregion

            graphic.Clear(Color.Transparent);
            graphic.DrawImage(layerBack.Bitmap, 0, 0);

            //graphic.DrawImage(layerParticle.Bitmap, 0, 0);
            graphic.DrawImage(layerFore.Bitmap, layerFore.RecPanel);
            //graphic.DrawImage(layerDashAim.Bitmap, layerDashAim.RecPanel.X, layerDashAim.RecPanel.Y);
            graphic.DrawImage(layerDashAim.Bitmap, layerDashAim.RecPanel);
            graphic.CompositingQuality = CompositingQuality.GammaCorrected;

            if (ShowFps) DrawFPS();

            graphic.Dispose();
            canvas.Image = bitmap;
            Form1_Resize(sender, e);
            CameraDeg += (float)(1 + rnd.NextDouble() * 2);
            CameraSca += (float)(0.5 + rnd.NextDouble() * 0.5);
        }

        private void canvas_DoubleClick(object sender, EventArgs e)
        {
            Form2 fm2 = new Form2();
            fm2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ProcessStart();
            SceneListen.LoadBG();
            //avoid artifacts
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            //map_changed_info = new FileInfo(@"Files\l_OsuFileLocation");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (proc == null) return;
            proc.Exited -= new EventHandler(Process_Exited);
            proc.Kill();
            layerDashAim.Close();
        }
        Process proc;
        void ProcessStart()
        {
            try
            {
                //proc.StartInfo.UseShellExecute = false;
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //proc.StartInfo.FileName = "stream\\osu!StreamCompanion.exe";
                proc = Process.Start("stream\\osu!StreamCompanion.exe");

                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(Process_Exited);
            }
            catch
            {
            }
        }
        void Process_Exited(object sender, EventArgs e)
        {
            ProcessStart();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            //Text = string.Format("({0}, {1}) Background: {2}ms{5} Foreground: {3}ms{6}, Particle: {4}ms{7}. Refresh: {8}ms",
            //       CanvasWidth, CanvasHeight, layerBack.DrawTime, layerFore.DrawTime, layerParticle.DrawTime,
            //       layerBack.InitializeTime != 0 ? " (INIT " + layerBack.InitializeTime + "ms)" : "",
            //       layerFore.InitializeTime != 0 ? " (INIT " + layerFore.InitializeTime + "ms)" : "",
            //       layerParticle.InitializeTime != 0 ? " (INIT " + layerParticle.InitializeTime + "ms)" : "",
            //       RefreshDelay);
        }

        private void timer_status_check_Tick(object sender, EventArgs e)
        {
            FileInfo tmp = new FileInfo(@"stream\Files\l_OsuFileLocation");
            if (MapInfo != null && tmp.LastWriteTime == MapInfo.LastWriteTime)
                return;
            MapInfo = tmp;
            RootOld = Root;
            try
            {
                Root = File.ReadAllText(MapInfo.FullName);
            }
            catch
            {
                return;
            }
            if (Root.Trim() == "")
            {
                FileInfo playDiff = new FileInfo(@"stream\Files\l_DiffName");
                if (PlayInfo != null && playDiff.LastWriteTime == PlayInfo.LastWriteTime)
                    return;
                PlayInfo = playDiff;
                try
                {
                    PlayDiff = File.ReadAllText(PlayInfo.FullName);
                }
                catch
                {
                    return;
                }
                if (PlayDiff.Trim() != "")
                {
                    IdleStatus = IdleStatus.Playing;
                    //todo
                    return;
                }
            }
            else
            {
                IdleStatus = IdleStatus.Listening;

                if (Root == RootOld) return;

                /// Initialize
                if (!timer_status_change.Enabled)
                {
                    //first run
                    //layerParticle.Initialize(10);
                    layerDashAim.Initialize();
                }
                else
                {
                    layerBack.Graphic.Dispose();
                    layerFore.Graphic.Dispose();
                    string sentence = "214,3215,663";
                    string[] word = sentence.Split(',');
                }
                layerBack.Initialize(MapInfo);
                layerFore.Initialize(MapInfo);

                if (action_change_info.Enabled) action_change_info.Enabled = false;
                //
            }
            if (IdleStatus != IdleStatus.Stopped)
            {
                if (!timer_status_change.Enabled) timer_status_change.Enabled = true;
                if (!action_particle.Enabled) action_particle.Enabled = true;
                if (!action_display.Enabled) action_display.Enabled = true;
            }
        }
    }
}
