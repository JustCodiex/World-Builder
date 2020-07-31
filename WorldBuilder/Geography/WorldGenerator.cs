using System;
using System.Collections.Generic;
using System.Text;
using WorldBuilder.Utility.Maths;

namespace WorldBuilder.Geography {
    
    public class WorldGenerator {

        Range m_pointRange;
        ushort m_width, m_height;
        double m_scale;

        public WorldGenerator SetSize(ushort w, ushort h) {
            this.m_width = w;
            this.m_height = h;
            return this;
        }

        public WorldGenerator SetScale(double scale) {
            this.m_scale = scale;
            return this;
        }

        public WorldGenerator SetPointRange(Range range) {
            this.m_pointRange = range;
            return this;
        }

        public World Generate(int seed) {

            Random randomizer = new Random(seed);
            World result = new World(this.m_width, this.m_height, this.m_scale);

            int pointCount = randomizer.Next(m_pointRange.Start.Value, m_pointRange.End.Value);
            HashSet<(int, int)> points = new HashSet<(int, int)>();

            // Generate random points
            for (int i = 0; i < pointCount; i++) {
                points.Add((randomizer.Next(0, this.m_width), randomizer.Next(0, this.m_height)));
            }

            VoroniDiagram diagram = new VoroniDiagram(this.m_width, this.m_height);
            diagram.GenerateVoroni(points, VoroniDiagram.DistanceMethod.Manhattan);
            diagram.SaveToFile("test_voroni.png");

            return result;

        }

    }

}
