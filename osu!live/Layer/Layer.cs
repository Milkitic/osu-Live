using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live.Layer
{
    public abstract class Layer
    {
        public Bitmap Bitmap { get; set; }
        public Graphics Graphic { get; set; }
        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.ReadyToChange;
        public long InitializeTime { get; set; }
        protected Random rnd = new Random();

        protected int CanvasHeight { get; set; } = Constant.Canvas.Height;
        protected int CanvasWidth { get; set; } = Constant.Canvas.Width;
        protected float zoom = (float)Constant.Canvas.Zoom;
        public Rectangle RecPanel { get; set; }

        // Debug
        protected Stopwatch watch = new Stopwatch();
        public long DrawTime { get; set; }

        public Layer()
        {
            Bitmap = new Bitmap(CanvasWidth, CanvasHeight);
            Graphic = Graphics.FromImage(Bitmap);
            Graphic.SmoothingMode = SmoothingMode.HighQuality;
        }
    }
}
