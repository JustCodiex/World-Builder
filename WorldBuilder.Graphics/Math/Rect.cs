using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Graphics.Math {
    
    public readonly struct Rect {
    
        public int MinX { get; }
        public int MinY { get; }
        public int MaxX { get; }
        public int MaxY { get; }

        public int Width => MaxX - MinX;

        public int Height => MaxY - MinY;

        public (int, int) Centre => (MinX + MaxX / 2, MinY + MaxY / 2);

        public Rect(int minx, int miny, int maxx, int maxy) {
            this.MinX = minx;
            this.MinY = miny;
            this.MaxX = maxx;
            this.MaxY = maxy;
        }

    }

}
