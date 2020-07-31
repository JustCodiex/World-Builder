using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldBuilder.Graphics;

namespace WorldBuilder.Utility.Maths {
    
    public class VoroniDiagram {

        public enum DistanceMethod {
            Euclidean,
            Manhattan,
        }

        private Render m_render;
        private int m_width;
        private int m_height;

        public VoroniDiagram(int width, int height) {

            this.m_width = width;
            this.m_height = height;
            this.m_render = new Render((uint)width, (uint)height);

        }

        public void GenerateVoroni(HashSet<(int, int)> points, DistanceMethod methood) {

            Func<(int, int), (int, int), double> func = null;
            if (methood == DistanceMethod.Euclidean) {
                func = this.EDis;
            } else { 
                func = this.MDis; 
            }

            Random random = new Random(points.Count);
            Dictionary<(int, int), (float, float, float)> colours = new Dictionary<(int, int), (float, float, float)>();

            for (int x = 0; x < this.m_width; x++) {
                for (int y = 0; y < this.m_height; y++) {

                    double min = double.MaxValue;
                    (int, int) point = (-1,-1);
                    for (int i = 0; i < points.Count; i++) {
                        double dis = func((x, y), points.ElementAt(i));
                        if (dis < min) {
                            min = dis;
                            point = points.ElementAt(i);
                        }
                    }

                    if (!colours.TryGetValue(point, out (float, float, float) colour)) {
                        colour = ((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                        colours.Add(point, colour);
                    }

                    this.m_render.SetPixel(x,y, colour);

                }
            }

        }

        private double EDis((int, int) a, (int, int) b) => Math.Sqrt(Math.Pow(a.Item1 - b.Item1, 2) + Math.Pow(a.Item2 - b.Item2, 2));

        private double MDis((int, int) a, (int, int) b) => Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);

        public void SaveToFile(string file)
            => m_render.RenderToFile(file);

    }

}
