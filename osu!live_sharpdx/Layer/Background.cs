using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DX = SharpDX;
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
using DW = SharpDX.DirectWrite;
using DXGI = SharpDX.DXGI;
using Mathe = SharpDX.Mathematics.Interop;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace osu_live_sharpdx.Layer
{
    class Background : ILayer
    {
        Stopwatch sw;
        // Text formats
        DW.TextFormat textFormat;

        // Brushes
        D2D.Brush whiteBrush, redBrush, blueBrush1, blueBrush2, blueBrush3;

        D2D.StrokeStyle strokeStyle;
        D2D.StrokeStyleProperties strokeProperties;

        D2D.StrokeStyle strokeStyle2;
        D2D.StrokeStyleProperties strokeProperties2;
        Random rnd = new Random();

        private Size ClientSize { get; set; }
        PointF g_center, g_mili, g_sec, g_min, g_hour;
        public Background(Size clientSize)
        {
            ClientSize = clientSize;

            // Create brushes
            whiteBrush = new D2D.SolidColorBrush(RenderForm.RenderTarget, new Mathe.RawColor4(1, 1, 1, 1));
            redBrush = new D2D.SolidColorBrush(RenderForm.RenderTarget, new Mathe.RawColor4(0.99f, 0.16f, 0.3f, 1));
            blueBrush1 = new D2D.SolidColorBrush(RenderForm.RenderTarget, new Mathe.RawColor4(0.3f, 0.4f, 0.9f, 1));
            blueBrush2 = new D2D.SolidColorBrush(RenderForm.RenderTarget, new Mathe.RawColor4(0.45f, 0.6f, 0.95f, 1));
            blueBrush3 = new D2D.SolidColorBrush(RenderForm.RenderTarget, new Mathe.RawColor4(0.6f, 0.8f, 1f, 1));

            // Create text formats
            textFormat = new DW.TextFormat(RenderForm.FactoryWrite, "Arial", 36);

            g_center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

            strokeProperties = new D2D.StrokeStyleProperties
            {
                StartCap = D2D.CapStyle.Round,
                EndCap = D2D.CapStyle.Triangle
            };
            strokeStyle = new D2D.StrokeStyle(RenderForm.Factory, strokeProperties);

            strokeProperties2 = new D2D.StrokeStyleProperties
            {
                StartCap = D2D.CapStyle.Round,
                EndCap = D2D.CapStyle.Round,
                DashStyle = D2D.DashStyle.Dash
            };
            strokeStyle2 = new D2D.StrokeStyle(RenderForm.Factory, strokeProperties2);

            sw = new Stopwatch();
            sw.Start();
        }

        public void Measure()
        {
            int rSec = 100, rMin = 85, rHour = 70;
            float degMili, degSec, degMin, degHour;
            float radMili, radSec, radMin, radHour;
            degMili = (DateTime.Now.Millisecond / 1000f * 360 - 90);
            degSec = (DateTime.Now.Second / 60f * 360 - 90) + degMili / 60f;
            degMin = (DateTime.Now.Minute / 60f * 360 - 90) + degSec / 360f * 6;
            degHour = (DateTime.Now.Hour / 12f * 360 - 90) + degMin / 360f * 30;
            radMili = degMili / 180 * (float)Math.PI;
            radSec = degSec / 180 * (float)Math.PI;
            radMin = degMin / 180 * (float)Math.PI;
            radHour = degHour / 180 * (float)Math.PI;

            g_mili = new PointF(g_center.X + (float)Math.Cos(radMili) * rSec, g_center.Y + (float)Math.Sin(radMili) * rSec);
            g_sec = new PointF(g_center.X + (float)Math.Cos(radSec) * rSec, g_center.Y + (float)Math.Sin(radSec) * rSec);
            g_min = new PointF(g_center.X + (float)Math.Cos(radMin) * rMin, g_center.Y + (float)Math.Sin(radMin) * rMin);
            g_hour = new PointF(g_center.X + (float)Math.Cos(radHour) * rHour, g_center.Y + (float)Math.Sin(radHour) * rHour);
        }

        public void Draw()
        {
            //System.Threading.Thread.Sleep(100);

            Measure();
            // Begin rendering
            RenderForm.RenderTarget.BeginDraw();
            RenderForm.RenderTarget.Clear(new Mathe.RawColor4(0, 0, 0, 1));


            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(g_center.X, g_center.Y),
                new Mathe.RawVector2(g_hour.X, g_hour.Y), blueBrush1, 10, strokeStyle);
            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(g_center.X, g_center.Y),
                new Mathe.RawVector2(g_min.X, g_min.Y), blueBrush2, 5, strokeStyle);
            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(g_center.X, g_center.Y),
                new Mathe.RawVector2(g_sec.X, g_sec.Y), blueBrush3, 2, strokeStyle);
            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(g_center.X, g_center.Y),
                new Mathe.RawVector2(g_mili.X, g_mili.Y), blueBrush3, 1, strokeStyle);

            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(Cursor.Position.X - RenderForm.Left, Cursor.Position.Y - RenderForm.Top),
                new Mathe.RawVector2(g_mili.X, g_mili.Y), blueBrush3, 1, strokeStyle2);
            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(Cursor.Position.X - RenderForm.Left, Cursor.Position.Y - RenderForm.Top),
                new Mathe.RawVector2(g_sec.X, g_sec.Y), blueBrush3, 1, strokeStyle2);
            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(Cursor.Position.X - RenderForm.Left, Cursor.Position.Y - RenderForm.Top),
                new Mathe.RawVector2(g_min.X, g_min.Y), blueBrush3, 1, strokeStyle2);
            RenderForm.RenderTarget.DrawLine(new Mathe.RawVector2(Cursor.Position.X - RenderForm.Left, Cursor.Position.Y - RenderForm.Top),
                new Mathe.RawVector2(g_hour.X, g_hour.Y), blueBrush3, 1, strokeStyle2);


            // Draw text
            RenderForm.RenderTarget.DrawText("Direct2D Test", textFormat, new Mathe.RawRectangleF(0, 0, 400, 200), whiteBrush);
            RenderForm.RenderTarget.DrawText(sw.ElapsedMilliseconds.ToString(), textFormat, new Mathe.RawRectangleF(0, 50, 400, 200), whiteBrush);

            RenderForm.RenderTarget.DrawText(DateTime.Now.ToLongTimeString() + "." + DateTime.Now.Millisecond.ToString("000"),
                textFormat, new Mathe.RawRectangleF(0, 100, 400, 200), redBrush);



            // End drawing
            RenderForm.RenderTarget.EndDraw();
        }
    }
}
