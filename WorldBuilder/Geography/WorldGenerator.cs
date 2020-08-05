using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldBuilder.Graphics;
using WorldBuilder.Graphics.Draw;
using WorldBuilder.Utility.Functional;
using WorldBuilder.Utility.Maths;

namespace WorldBuilder.Geography {
    
    public class WorldGenerator {

        Range m_pointRange;
        ushort m_width, m_height;
        double m_scale;
        VoroniDiagram.DistanceMethod m_dstMethod;

        int m_continents = 1;

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

        public WorldGenerator SetContinentCount(int continents) {
            this.m_continents = continents;
            return this;
        }

        public WorldGenerator SetDistanceMethod(VoroniDiagram.DistanceMethod method) {
            this.m_dstMethod = method;
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
            diagram.GenerateVoroni(points, this.m_dstMethod);
            diagram.SaveToFile("test_voroni.png");

            Console.WriteLine("-- Voroni generated --");

            diagram.Borderise(false);

            Console.WriteLine("-- Voroni Borderized --");

            diagram.CaptureRegions(false);

            Console.WriteLine("-- Voroni Regions Captured --");
            Console.WriteLine("-- Voroni Removing Border Regions --");

            diagram.RemoveRegions((a, b) => a.Bounding.MinX == 0 || a.Bounding.MinY == 0 || a.Bounding.MaxX == b.Width-1 || a.Bounding.MaxY == b.Height-1);
            diagram.SaveToFile("test_voroni_no_edge_regions.png");

            Console.WriteLine("-- Generating Continents --");

            GenerateContinents(diagram, randomizer, pointCount);

            Console.WriteLine("-- Converting to Grayscale --");

            Render bwDiagram = diagram.Renderer.ToBlackWhite(0.01f);
            bwDiagram.RenderToFile("test_world_a.png");

            CrystalizeFilter crystalizeFlt = new CrystalizeFilter() { Generator = randomizer };
            MedianNoiseFilter medianFlt = new MedianNoiseFilter() { NoiseSize = 5 };

            Console.WriteLine("-- Applying filters --");

            bwDiagram.ApplyFilter(medianFlt);
            bwDiagram.ApplyFilter(crystalizeFlt);
            bwDiagram.ApplyFilter(medianFlt);

            Console.WriteLine();

            bwDiagram.RenderToFile("test_world_b.png");

            return result;

        }

        private void GenerateContinents(VoroniDiagram diagram, Random randomizer, int pointCount) {


            int offset = randomizer.Next(0, 50);
            int xCut = offset;

            Dictionary<int, List<VoroniRegion>> m_continentRegions = new Dictionary<int, List<VoroniRegion>>();

            for (int i = 0; i < this.m_continents; i++) {

                int select = randomizer.Next(5, 8 + (int)((diagram.RegionCount) / (this.m_continents) * 0.675));
                int nullCounter = 0;
                List<VoroniRegion> selected = new List<VoroniRegion>();
                Stack<VoroniRegion> addOrder = new Stack<VoroniRegion>();
                VoroniRegion vr0 = null;

                while (selected.Count < select && nullCounter < 100) {

                    if (vr0 == null) {
                        vr0 = diagram.SelectRegion(randomizer.Next(0, pointCount));
                        if (m_continentRegions.Any(x => x.Value.Contains(vr0))) {
                            vr0 = null;
                            continue;
                        } else {
                            addOrder.Push(vr0);
                            continue;
                        }
                    }

                    List<VoroniRegion> neighbours = diagram.SelectNeighbours(vr0);

                    if (neighbours.Count > 0) {

                        int takeCount = randomizer.Next(1, Math.Min(neighbours.Count, Math.Min(3, select - selected.Count)));
                        if (takeCount > 2) {
                            takeCount = 2;
                        }

                        IEnumerable<(VoroniRegion, int)> p = neighbours
                            .Select(x => (x, (this.m_dstMethod == VoroniDiagram.DistanceMethod.Euclidean)?
                            ((x.Bounding.Centre.Item1 - vr0.Bounding.Centre.Item1) *(x.Bounding.Centre.Item1 - vr0.Bounding.Centre.Item1) +(vr0.Bounding.Centre.Item2) *(vr0.Bounding.Centre.Item2))
                            :Math.Abs(x.Bounding.Centre.Item1 - vr0.Bounding.Centre.Item1) + Math.Abs(x.Bounding.Centre.Item2 - vr0.Bounding.Centre.Item2)))
                            .Where(x => !m_continentRegions.Any(y => y.Value.Contains(x.x)) && !selected.Contains(x.x));

                        IEnumerable<VoroniRegion> n = ((randomizer.Next(0, 10) < 6) ? p.OrderBy(x => x.Item2) : p.OrderByDescending(x => x.Item2)).Select(x => x.Item1);

                        if (n.Count() > 0) {

                            var ls = n.Take(takeCount);

                            foreach (var vr in ls) {
                                addOrder.Push(vr);
                            }

                            var f = ls.FirstOrDefault();
                            if (f != null) {
                                vr0 = f;
                                selected.Add(f);
                            }

                        } else {
                            if (addOrder.Count > 0) {
                                vr0 = addOrder.Pop();
                            } else {
                                vr0 = selected.First();
                                nullCounter++;
                            }
                        }

                    } else {
                        if (addOrder.Count > 0) {
                            vr0 = addOrder.Pop();
                        } else {
                            vr0 = selected.First();
                            nullCounter++;
                        }
                    }

                }

                m_continentRegions.Add(i, selected);

            }

            diagram.RemoveRegions((a, b) => !m_continentRegions.Any(x => x.Value.Contains(a)));
            diagram.SaveToFile("test_voroni_with_continents.png");

        }

    }

}
