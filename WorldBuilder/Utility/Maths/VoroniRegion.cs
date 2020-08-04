using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldBuilder.Graphics.Math;

namespace WorldBuilder.Utility.Maths {
    
    public class VoroniRegion {
    
        public List<(int,int)> Vertices { get; set; }

        public Rect Bounding { get; set; }

        public void CalculateBoundingBox() {
            (int x1, int x2) = (this.Vertices.Min(p => p.Item1), this.Vertices.Max(p => p.Item1));
            (int y1, int y2) = (this.Vertices.Min(p => p.Item2), this.Vertices.Max(p => p.Item2));
            this.Bounding = new Rect(x1, y1, x2, y2);
        }

    }

}
