using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;

namespace osu_live_sharpdx
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
        //    var form = new RenderForm("device render");

        //    //this one is for creating DirectWrite Elements
        //    var factoryWrite = new SharpDX.DirectWrite.Factory();

        //    // SwapChain description
        //    var desc = new SwapChainDescription()
        //    {
        //        BufferCount = 1,
        //        ModeDescription = new ModeDescription(
        //            form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
        //        IsWindowed = true,
        //        OutputHandle = form.Handle,
        //        SampleDescription = new SampleDescription(1, 0),
        //        SwapEffect = SwapEffect.Discard,
        //        Usage = Usage.RenderTargetOutput
        //    };

        //    // Create Device and SwapChain
        //    Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport,
        //        new SharpDX.Direct3D.FeatureLevel[] { SharpDX.Direct3D.FeatureLevel.Level_10_0 },
        //        desc, out Device device, out SwapChain swapChain);

        //    var d2dFactory = new SharpDX.Direct2D1.Factory();

        //    int width = form.ClientSize.Width;
        //    int height = form.ClientSize.Height;

        //    var rectangleGeometry = new RoundedRectangleGeometry(d2dFactory, new RoundedRectangle()
        //    {
        //        RadiusX = 32,
        //        RadiusY = 32,
        //        Rect = new RectangleF(128, 128, width - 128 * 2, height - 128 * 2)
        //    });

        //    // Ignore all windows events
        //    Factory factory = swapChain.GetParent<Factory>();
        //    factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

        //    // New RenderTargetView from the backbuffer
        //    Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
        //    var renderView = new RenderTargetView(device, backBuffer);

        //    Surface surface = backBuffer.QueryInterface<Surface>();


        //    var d2dRenderTarget = new RenderTarget(d2dFactory, surface,
        //                                                    new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));

        //    var solidColorBrush = new SolidColorBrush(d2dRenderTarget, Color.White);


        //    //create textformat
        //    var textFormat = new SharpDX.DirectWrite.TextFormat(factoryWrite, "Arial", 36);

        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();

        //    Stopwatch stopwatch2 = new Stopwatch();
        //    stopwatch2.Start();
        //    TimeSpan oldms = stopwatch2.Elapsed;
        //    // Main loop
        //    RenderLoop.Run(form, () =>
        //    {
        //        // Get the elapsed time as a TimeSpan value.
        //        TimeSpan ts = stopwatch2.Elapsed;

        //        d2dRenderTarget.BeginDraw();
        //        d2dRenderTarget.Clear(Color.Black);
        //        solidColorBrush.Color = new Color4(1, 1, 1, (float)Math.Abs(Math.Cos(stopwatch.ElapsedMilliseconds * .001)));
        //        d2dRenderTarget.FillGeometry(rectangleGeometry, solidColorBrush, null);
        //        d2dRenderTarget.DrawText((1000f / (ts.TotalMilliseconds - oldms.TotalMilliseconds)).ToString(), textFormat, new RawRectangleF(0, 0, 400, 200), solidColorBrush);

        //        d2dRenderTarget.EndDraw();

        //        swapChain.Present(0, PresentFlags.None);
        //        oldms = stopwatch2.Elapsed;
        //    });

        //    // Release all resources
        //    renderView.Dispose();
        //    backBuffer.Dispose();
        //    device.ImmediateContext.ClearState();
        //    device.ImmediateContext.Flush();
        //    device.Dispose();
        //    device.Dispose();
        //    swapChain.Dispose();
        //    factory.Dispose();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RenderForm());
        }

    }
}
