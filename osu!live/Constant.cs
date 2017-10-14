﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live
{
    public static class Constant
    {
        public static _canvas Canvas { get; set; } = new _canvas
        {
            Height = 720,
            Width = 1280,
            X = 0,
            Y = 0
        };

        public class _canvas
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public float Ratio { get => (float)Width / Height; }
        }
    }
}