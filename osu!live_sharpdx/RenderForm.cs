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

namespace osu_live_sharpdx
{
    public partial class RenderForm : Form
    {
        Layer.Background layerBack;

        // Factory for creating 2D elements
        public static D2D.Factory Factory { get; set; } = new D2D.Factory();
        // For creating DirectWrite Elements
        public static DW.Factory FactoryWrite { get; set; } = new DW.Factory();
        // Target of rendering
        public static D2D.RenderTarget RenderTarget { get; set; }

        float g_width, g_height;

        public static new int Left { get; private set; }
        public static new int Top { get; private set; }
        public static new int Right { get; private set; }
        public static new int Bottom { get; private set; }

        public RenderForm()
        {
            InitializeComponent();
            Load += LoadTarget;
            this.ClientSize = new Size(1270, 720);

            g_width = ClientSize.Width;
            g_height = ClientSize.Height;
        }

        private void Form1_Load(object sender, EventArgs e)
        {


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
                AntialiasMode = D2D.AntialiasMode.PerPrimitive
            };

            // Initialize layers
            layerBack = new Layer.Background(this.ClientSize);

            // Avoid artifacts
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            layerBack.Draw();
            int width = Screen.PrimaryScreen.Bounds.Width,
                height = Screen.PrimaryScreen.Bounds.Height;
            Left = (int)(Location.X + (Width - ClientSize.Width) / 2f);
            Right = Left + ClientSize.Width;
            Top = (int)(Location.Y + (Height - ClientSize.Height) - 8);
            Bottom = Top + ClientSize.Height;

            this.Invalidate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (RenderTarget != null)
            {
                var abc = new Mathe.RawMatrix3x2
                {
                    M11 = g_width / ClientSize.Width,
                    M12 = 0,
                    M21 = 0,
                    M22 = g_height / ClientSize.Height,
                    M31 = 0,
                    M32 = 0
                };
                RenderTarget.Transform = abc;
            }
        }
    }
}
