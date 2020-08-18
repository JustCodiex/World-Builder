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

        public bool Intersects(Rect bounding) {

            bool xIntersection = (this.MinX <= bounding.MinX && this.MaxX >= bounding.MaxX) || (this.MinX <= bounding.MaxX && this.MaxX >= bounding.MaxX)
                || (bounding.MinX <= this.MinX && bounding.MaxX >= this.MaxX) || (bounding.MinX <= this.MaxX && bounding.MaxX >= this.MaxX);

            bool yIntersection = (this.MinY <= bounding.MinY && this.MaxY >= bounding.MaxY) || (this.MinY <= bounding.MaxY && this.MaxY >= bounding.MaxY)
                || (bounding.MinY <= this.MinY && bounding.MaxY >= this.MaxY) || (bounding.MinY <= this.MaxY && bounding.MaxY >= this.MaxY);

            return xIntersection && yIntersection;

        }

        public Rect Expand(float modifier) {
            float minx = this.MinX - (this.MinX * modifier);
            float miny = this.MinY - (this.MinY * modifier);
            float maxx = this.MaxX + (this.MaxX * modifier);
            float maxy = this.MaxY + (this.MaxY * modifier);
            return new Rect((int)minx, (int)miny, (int)maxx, (int)maxy);
        }

        public bool Contains(int x, int y) => this.MinX <= x && x <= this.MaxX && this.MinY <= y && y <= this.MaxY;

    }

}
