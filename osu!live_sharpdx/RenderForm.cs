using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DX = SharpDX;
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
using DW = SharpDX.DirectWrite;
using DXGI = SharpDX.DXGI;
using Mathe = SharpDX.Mathematics.Interop;
using System.Threading;
using System.Diagnostics;

namespace osu_live_sharpdx
{
    public partial class RenderForm : Form
    {
        Layer.Clock layerClock;
        Layer.Background layerBack;
        Layer.Particles layerParticle;

        Process streamProc;

        // Factory for creating 2D elements
        public static D2D.Factory Factory { get; set; } = new D2D.Factory();
        // For creating DirectWrite Elements
        public static DW.Factory FactoryWrite { get; set; } = new DW.Factory();
        // Target of rendering
        public static D2D.RenderTarget RenderTarget { get; set; }

        // Backcolor
        Mathe.RawColor4 colorBack;

        public static new int Left { get; private set; }
        public static new int Top { get; private set; }
        public static new int Right { get; private set; }
        public static new int Bottom { get; private set; }
        public static new int Width { get; private set; }
        public static new int Height { get; private set; }

        private int drawCount = 0;

        public RenderForm()
        {
            InitializeComponent();

            Load += LoadTarget;
            this.ClientSize = new Size(1280, 720);

            Width = ClientSize.Width;
            Height = ClientSize.Height;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ProcessStart();

        }

        private void LoadTarget(object sender, EventArgs e)
        {

            var pixelFormat = new D2D.PixelFormat(DXGI.Format.B8G8R8A8_UNorm, D2D.AlphaMode.Premultiplied);

            var winProp = new D2D.HwndRenderTargetProperties
            {
                Hwnd = this.Handle,
                PixelSize = new DX.Size2(this.ClientSize.Width, this.ClientSize.Height),
                PresentOptions = D2D.PresentOptions.Immediately
            };

            var renderProp = new D2D.RenderTargetProperties(D2D.RenderTargetType.Default, pixelFormat,
                   96, 96, D2D.RenderTargetUsage.None, D2D.FeatureLevel.Level_DEFAULT);

            RenderTarget = new D2D.WindowRenderTarget(Factory, renderProp, winProp)
            {
                AntialiasMode = D2D.AntialiasMode.PerPrimitive,
                TextAntialiasMode = D2D.TextAntialiasMode.Grayscale,
                Transform = new Mathe.RawMatrix3x2
                {
                    M11 = 1,
                    M12 = 0,
                    M21 = 0,
                    M22 = 1,
                    M31 = 0,
                    M32 = 0
                }
            };

            // Create colors
            colorBack = new Mathe.RawColor4(0, 0, 0, 1);

            // Initialize layers
            layerClock = new Layer.Clock(this.ClientSize);
            layerParticle = new Layer.Particles(500, 200);
            layerBack = new Layer.Background();

            // Avoid artifacts
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            if (RenderTarget.IsDisposed) return;
            drawCount++;
            if (drawCount == 10000)
            {
                GC.Collect();
                drawCount = 0;
            }
            //Thread.Sleep(45);
            // Begin rendering
            RenderTarget.BeginDraw();
            RenderTarget.Clear(colorBack);

            layerBack.Draw();
            layerParticle.Draw();
            layerClock.Draw();

            // End drawing
            RenderTarget.EndDraw();

            //Left = (int)(Location.X + (Width - ClientSize.Width) / 2f);
            //Right = Left + ClientSize.Width;
            //Top = (int)(Location.Y + (Height - ClientSize.Height) - 8f);
            //Bottom = Top + ClientSize.Height;

            this.Invalidate();
        }

        private void RenderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (streamProc != null)
            {
                streamProc.Exited -= Process_Exited;
                streamProc.Kill();
            }

            RenderTarget.Dispose();

            layerClock.Dispose();
            layerBack.Dispose();

            FactoryWrite.Dispose();
            Factory.Dispose();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (Width > Height) ClientSize = new Size(ClientSize.Width, (int)(ClientSize.Width * 9d / 16));
            else if (Width < Height) ClientSize = new Size((int)(ClientSize.Height / 9d * 16), ClientSize.Height);
        }

        private void ProcessStart()
        {
            try
            {
                //proc.StartInfo.UseShellExecute = false;
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //proc.StartInfo.FileName = "stream\\osu!StreamCompanion.exe";
                streamProc = Process.Start("stream\\osu!StreamCompanion.exe");

                streamProc.EnableRaisingEvents = true;
                streamProc.Exited += Process_Exited;
            }
            catch
            {
            }
        }
        private void Process_Exited(object sender, EventArgs e)
        {
            ProcessStart();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
