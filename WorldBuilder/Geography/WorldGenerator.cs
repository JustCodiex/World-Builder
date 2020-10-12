using System;
using System.Collections.Generic;
using System.Linq;

using WorldBuilder.Graphics;
using WorldBuilder.Graphics.Draw;
using WorldBuilder.History;
using WorldBuilder.Utility.Algorithms;
using WorldBuilder.Utility.Functional;
using WorldBuilder.Utility.Maths;

namespace WorldBuilder.Geography {
    
    public class WorldGenerator {

        Range m_pointRange;
        ushort m_width, m_height;
        double m_scale;
        VoroniDiagram.DistanceMethod m_dstMethod;

        int m_continents = 1;

        public bool GenerateHistory { get; set; } = true;

        public int StartYear { get; set; } = -10000;

        public int SimStartYear { get; set; } = 1700;

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

            diagram.RemoveRegions((a, b) => a.Bounding.MinX <= 30 || a.Bounding.MinY <= 30 || a.Bounding.MaxX >= b.Width-30 || a.Bounding.MaxY >= b.Height-30);
            diagram.SaveToFile("test_voroni_no_edge_regions.png");

            Console.WriteLine("-- Generating Continents --");

            Dictionary<int, List<VoroniRegion>> contininents = GenerateContinents(diagram, randomizer, pointCount); // TODO: Save regions

            Console.WriteLine("-- Converting to Grayscale --");

            Render bwDiagram = diagram.Renderer.ToBlackWhite(0.01f);
            bwDiagram.RenderToFile("test_world_a.png");

            CrystalizeFilter crystalizeFlt = new CrystalizeFilter() { Generator = randomizer };
            MedianNoiseFilter medianFlt = new MedianNoiseFilter() { NoiseSize = 5 };

            Console.WriteLine("-- Applying filters --");

            bwDiagram.ApplyFilter(medianFlt);
            bwDiagram.ApplyFilter(crystalizeFlt);
            bwDiagram.ApplyFilter(medianFlt);

            bwDiagram.RenderToFile("test_world_b.png");

            Console.WriteLine("-- Filters applied --");

            // Create continental points
            HashSet<(int, int)> continentalPoints = new HashSet<(int, int)>();
            int counter = 0;
            int pCountNew = (int)(pointCount * 4.5);
            while (counter < pCountNew) {
                (int x, int y) = (randomizer.Next(0, this.m_width), randomizer.Next(0, this.m_height));
                if (diagram.IsValidRegion(x, y)) {
                    if (continentalPoints.Add((x, y))) {
                        counter++;
                    }
                }
            }

            VoroniDiagram continentalDiagram = VoroniDiagram.FromRender(bwDiagram, continentalPoints, VoroniDiagram.DistanceMethod.Euclidean, (0.0f, 0.0f, 0.0f));
            continentalDiagram.Borderise(false);
            continentalDiagram.CaptureRegions(false);
            continentalDiagram.RemoveRegions((a, b) => a.Bounding.MinX == 0 && a.Bounding.MinY == 0 && a.Bounding.MaxX == b.Width - 1 && a.Bounding.MaxY == b.Height - 1);
            continentalDiagram.SaveToFile("continental.png");

            Console.WriteLine("-- Generating height map --");

            var continentVoroniRegions = this.MapContinents(contininents, diagram, continentalDiagram);
            this.GenerateHeightmap(bwDiagram, continentVoroniRegions, continentalDiagram, randomizer, out Render heightmap, out Render colourmap);

            Console.WriteLine("-- Genereating details map --");
            this.GenerateDetails(bwDiagram);

            Console.WriteLine("-- Finished generating world terrain --");

            if (GenerateHistory) {

                Console.WriteLine("-- Converting terrain into world data");

                this.ConvertToSimulationData(continentVoroniRegions, continentalDiagram, heightmap, colourmap, out List<WorldContinent> wContinents, randomizer);
                result.Continents.AddRange(wContinents);
                result.RecalculateWorldGraph();

                Console.WriteLine("-- Generating history --");

                HistoryGenerator hisGen = new HistoryGenerator(randomizer);

                hisGen.GenerateHistory(result, StartYear, SimStartYear);

                result.History.SaveToFile("history.txt");

            }

            Console.WriteLine();

            // Force the GC to clean up
            GC.Collect();

            return result;

        }

        private Dictionary<int, List<VoroniRegion>> GenerateContinents(VoroniDiagram diagram, Random randomizer, int pointCount) {

            int offset = randomizer.Next(0, 50);
            int xCut = offset;

            Dictionary<int, List<VoroniRegion>> continents = new Dictionary<int, List<VoroniRegion>>();

            for (int i = 0; i < this.m_continents; i++) {

                int select = randomizer.Next(5, 8 + (int)((diagram.RegionCount) / (this.m_continents) * 0.6));
                int nullCounter = 0;
                List<VoroniRegion> selected = new List<VoroniRegion>();
                Stack<VoroniRegion> addOrder = new Stack<VoroniRegion>();
                VoroniRegion vr0 = null;

                while (selected.Count < select && nullCounter < 100) {

                    if (vr0 == null) {
                        vr0 = diagram.SelectRegion(randomizer.Next(0, pointCount));
                        if (continents.Any(x => x.Value.Contains(vr0))) {
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
                            .Where(x => !continents.Any(y => y.Value.Contains(x.x)) && !selected.Contains(x.x));

                        IEnumerable<VoroniRegion> n = ((randomizer.Next(0, 11) <= 5) ? p.OrderBy(x => x.Item2) : p.OrderByDescending(x => x.Item2)).Select(x => x.Item1);

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

                continents.Add(i, selected);

            }

            diagram.RemoveRegions((a, b) => !continents.Any(x => x.Value.Contains(a)));
            diagram.SaveToFile("test_voroni_with_continents.png");

            return continents;

        }

        private void GenerateHeightmap(Render currentRender, Dictionary<int, List<VoroniRegion>> continents, VoroniDiagram voroni, Random randomizer, out Render heightmap, out Render colourmap) {

            heightmap = new Render((uint)currentRender.Raw.Width, (uint)currentRender.Raw.Height);
            heightmap.Clear(0, 0, 0);

            colourmap = new Render((uint)currentRender.Raw.Width, (uint)currentRender.Raw.Height);
            colourmap.Clear(0, 0, 0);

            foreach (var continent in continents) {

                foreach (var region in continent.Value) {

                    var neighbourRegions = voroni.SelectNeighbours(region);
                    List<VoroniRegion> nonContinentalNeighbours = new List<VoroniRegion>();

                    foreach (var neighbour in neighbourRegions) {
                        if (!continent.Value.Contains(neighbour)) {
                            nonContinentalNeighbours.Add(neighbour);
                            break;
                        }
                    }

                    bool isCoastalRegion = false;

                    for (int x = region.Bounding.MinX - 1; x < region.Bounding.MaxX + 1 && !isCoastalRegion; x++) {
                        for (int y = region.Bounding.MinY - 1; y < region.Bounding.MaxY + 1 && !isCoastalRegion; y++) {
                            if (!voroni.IsValidRegion(x, y)) {
                                isCoastalRegion = true;
                            }
                        }
                    }

                    bool isContinentalBorderRegion = nonContinentalNeighbours.Count > 0;
                    bool isSouthernRegion = region.Bounding.Centre.Item2 > currentRender.Raw.Height * (3.5/4.0);
                    bool isEquatorialRegion = !isSouthernRegion && region.Bounding.Centre.Item2 > currentRender.Raw.Height * (1.5 / 4.0);
                    bool isNorthernRegion = !isSouthernRegion && !isEquatorialRegion;

                    /*if (isNorthernRegion) {
                        DrawContinentalZone(voroni, heightmap, colourmap, region, (1.0f, 0.0f, 0.0f));
                    } else if (isEquatorialRegion) {
                        DrawContinentalZone(voroni, heightmap, colourmap, region, (0.0f, 1.0f, 0.0f));
                    } else {
                        DrawContinentalZone(voroni, heightmap, colourmap, region, (0.0f, 0.0f, 1.0f));
                    }*/

                    if (isContinentalBorderRegion) {
                        if (isCoastalRegion) {
                            if (isNorthernRegion) {
                                if (randomizer.Next(0,10) <= 8) { 
                                    GenerateForest(voroni, heightmap, colourmap, region, randomizer); 
                                } else {
                                    GenerateMountainous(voroni, heightmap, colourmap, region, randomizer);
                                }
                            } else if (isEquatorialRegion) {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateForest(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateMountainous(voroni, heightmap, colourmap, region, randomizer);
                                }
                            } else {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateForest(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateMountainous(voroni, heightmap, colourmap, region, randomizer);
                                }
                            }
                        } else {
                            if (isNorthernRegion) {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateForest(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateMountainous(voroni, heightmap, colourmap, region, randomizer);
                                }
                            } else if (isEquatorialRegion) {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateTropical(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateMountainous(voroni, heightmap, colourmap, region, randomizer);
                                }
                            } else {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateTropical(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateMountainous(voroni, heightmap, colourmap, region, randomizer);
                                }
                            }
                        }
                    } else {
                        if (isCoastalRegion) {
                            if (isNorthernRegion) {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateForest(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateFlatlands(voroni, heightmap, colourmap, region, randomizer);
                                }
                            } else if (isEquatorialRegion) {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateFlatlands(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateDesert(voroni, heightmap, colourmap, region, randomizer);
                                }
                            } else {
                                if (randomizer.Next(0, 10) <= 8) {
                                    GenerateFlatlands(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateTropical(voroni, heightmap, colourmap, region, randomizer);
                                }
                            }
                        } else {
                            if (isNorthernRegion) {
                                if (randomizer.Next(0, 10) <= 5) {
                                    GenerateForest(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateFlatlands(voroni, heightmap, colourmap, region, randomizer);
                                }
                            } else if (isEquatorialRegion) {
                                GenerateDesert(voroni, heightmap, colourmap, region, randomizer);
                            } else {
                                if (randomizer.Next(0, 10) <= 5) {
                                    GenerateTropical(voroni, heightmap, colourmap, region, randomizer);
                                } else {
                                    GenerateDesert(voroni, heightmap, colourmap, region, randomizer);
                                }
                            }
                        }
                    }//*/

                }

            }

            heightmap.RenderToFile("test_world_c.png");
            colourmap.RenderToFile("test_world_d.png");

        }

        private Dictionary<int, List<VoroniRegion>> MapContinents(Dictionary<int, List<VoroniRegion>> source, VoroniDiagram diagram, VoroniDiagram continentalDiagram) {

            Dictionary<int, List<VoroniRegion>> result = new Dictionary<int, List<VoroniRegion>>();
            var oldregions = diagram.SelectRegion((a, b) => true);

            var regions = continentalDiagram.SelectRegion((a, b) => true);

            foreach (var pair in source) {
                result.Add(pair.Key, new List<VoroniRegion>());
            }

            foreach (var reg in regions) {

                List<VoroniRegion> boundMatches = new List<VoroniRegion>();

                foreach (var reg2 in oldregions) {
                    if (diagram.IsValidRegion(reg2.Bounding.Centre.Item1, reg2.Bounding.Centre.Item2) && reg2.Bounding.Expand(1.5f).Contains(reg.Bounding.Centre.Item1, reg.Bounding.Centre.Item2)) {
                        boundMatches.Add(reg2);
                    }
                }

                if (boundMatches.Count > 0) {

                    VoroniRegion nearest = boundMatches.MinDist(double.MaxValue, reg,
                        (a, b) => (Math.Pow(a.Bounding.Centre.Item1 - b.Bounding.Centre.Item1, 2)) + Math.Pow(a.Bounding.Centre.Item2 - b.Bounding.Centre.Item2, 2));

                    foreach (var pair in source) {
                        if (pair.Value.Contains(nearest)) {
                            result[pair.Key].Add(reg);
                            break;
                        }
                    }

                }

            }
            return result;

        }

        private static (float r, float g, float b) TropicalTreeColour = (0.035f, 0.18f, 0.054f);
        private static (float r, float g, float b) FlatlandsColour = (0.83f, 0.71f, 0.54f);
        private static (float r, float g, float b) DesertColour = (0.125f, 0.54f, 0.1f);
        private static (float r, float g, float b) ForestColour = (0.49f, 0.54f, 0.1f);
        private static (float r, float g, float b) MountainColour = (0.97f, 0.97f, 0.97f);
        //private static (float r, float g, float b) BeachColour = (0.125f, 0.54f, 0.1f);

        private void DrawContinentalZone(VoroniDiagram diagram, Render heightmap, Render colourmap, VoroniRegion region, (float,float,float) colour) {

            for (int x = region.Bounding.MinX; x <= region.Bounding.MaxX; x++) {
                for (int y = region.Bounding.MinY; y <= region.Bounding.MaxY; y++) {

                    if (diagram.IsValidRegion(x, y) && diagram.SelectRegion(x, y) == region) {
                        heightmap.SetPixel(x, y, (0.35f, 0.35f, 0.35f));
                        colourmap.SetPixel(x, y, colour);
                    }

                }
            }

        }

        private void GenerateFlatlands(VoroniDiagram diagram, Render heightmap, Render colourmap, VoroniRegion region, Random random) {

            for (int x = region.Bounding.MinX; x <= region.Bounding.MaxX; x++) {
                for (int y = region.Bounding.MinY; y <= region.Bounding.MaxY; y++) {

                    if (diagram.IsValidRegion(x, y) && diagram.SelectRegion(x, y) == region) {
                        heightmap.SetPixel(x, y, (0.35f, 0.35f, 0.35f));
                        colourmap.SetPixel(x, y, FlatlandsColour);
                    }

                }
            }

        }

        private void GenerateDesert(VoroniDiagram diagram, Render heightmap, Render colourmap, VoroniRegion region, Random random) {

            for (int x = region.Bounding.MinX; x <= region.Bounding.MaxX; x++) {
                for (int y = region.Bounding.MinY; y <= region.Bounding.MaxY; y++) {

                    if (diagram.IsValidRegion(x, y) && diagram.SelectRegion(x, y) == region) {
                        heightmap.SetPixel(x, y, (0.35f, 0.35f, 0.35f));
                        colourmap.SetPixel(x, y, DesertColour);
                    }

                }
            }

        }

        private void GenerateForest(VoroniDiagram diagram, Render heightmap, Render colourmap, VoroniRegion region, Random random) {

            for (int x = region.Bounding.MinX; x <= region.Bounding.MaxX; x++) {
                for (int y = region.Bounding.MinY; y <= region.Bounding.MaxY; y++) {

                    if (diagram.IsValidRegion(x, y) && diagram.SelectRegion(x, y) == region) {
                        heightmap.SetPixel(x, y, (0.35f, 0.35f, 0.35f));
                        colourmap.SetPixel(x, y, ForestColour);
                    }

                }
            }

        }

        private void GenerateMountainous(VoroniDiagram diagram, Render heightmap, Render colourmap, VoroniRegion region, Random random) {

            for (int x = region.Bounding.MinX; x <= region.Bounding.MaxX; x++) {
                for (int y = region.Bounding.MinY; y <= region.Bounding.MaxY; y++) {

                    if (diagram.IsValidRegion(x, y) && diagram.SelectRegion(x, y) == region) {
                        heightmap.SetPixel(x, y, (0.35f, 0.35f, 0.35f));
                        colourmap.SetPixel(x, y, MountainColour);
                    }

                }
            }

        }

        private void GenerateTropical(VoroniDiagram diagram, Render heightmap, Render colourmap, VoroniRegion region, Random random) {

            for (int x = region.Bounding.MinX; x <= region.Bounding.MaxX; x++) {
                for (int y = region.Bounding.MinY; y <= region.Bounding.MaxY; y++) {

                    if (diagram.IsValidRegion(x, y) && diagram.SelectRegion(x, y) == region) {
                        heightmap.SetPixel(x, y, (0.35f, 0.35f, 0.35f));
                        colourmap.SetPixel(x,y, TropicalTreeColour);
                    }

                }
            }

        }

        private void GenerateDetails(Render currentRender) {

            currentRender.RenderToFile("test_world_e.png");

        }

        private void ConvertToSimulationData(Dictionary<int, List<VoroniRegion>> vorodiDiagrams, VoroniDiagram diagram, Render heightmap, Render colourmap, out List<WorldContinent> continents, Random random) {

            continents = new List<WorldContinent>();
            List<WorldRegion> regions = new List<WorldRegion>();
            List<WorldProvince> provinces = new List<WorldProvince>();

            foreach (var continent in vorodiDiagrams) {
                if (continent.Value.Count > 0) {

                    var wContinent = new WorldContinent();
                    var continentProvinces = new Dictionary<WorldProvince, VoroniRegion>();
                    var continentRegions = new List<WorldRegion>();

                    foreach (var province in continent.Value) {

                        WorldProvince wProvince = new WorldProvince() {
                            XPos = province.Bounding.Centre.Item1,
                            YPos = province.Bounding.Centre.Item2,
                        };

                        continentProvinces.Add(wProvince, province);

                    }

                    foreach (var pair in continentProvinces) {
                        var neighbours = diagram.SelectNeighbours(pair.Value);
                        for (int i = 0; i < neighbours.Count; i++) {
                            if (neighbours[i] != null) {
                                WorldProvince p = continentProvinces.FirstOrDefault(x => x.Value == neighbours[i]).Key;
                                if (p != null) {
                                    pair.Key.NeighbourProvinces.Add(p);
                                }
                            }
                        }
                    }

                    int regionMaxCount = continentProvinces.Count / random.Next(4, 8);
                    int regionMaxSize = (continentProvinces.Count / regionMaxCount) + 1;
                    var provincePool = continentProvinces.Select(x => x.Key).ToList();

                    while (continentRegions.Count < regionMaxCount && provincePool.Count > 0) {

                        WorldProvince province = provincePool.Random(random);
                        WorldRegion reg = new WorldRegion();

                        if (province.NeighbourProvinces.Count > 0) {
                            reg.Provinces.Add(province);
                            int picked = 1;
                            int attempts = 0;
                            int thisRegionMaxSize = regionMaxSize + random.Next(-1, 1);
                            while (picked < thisRegionMaxSize && attempts < regionMaxSize * 4) {
                                WorldProvince neighbour = province.NeighbourProvinces.Random(random);
                                if (provincePool.Contains(neighbour)) {
                                    provincePool.Remove(neighbour);
                                    picked++;
                                    reg.Provinces.Add(neighbour);
                                    if (random.NextDouble() <= 0.45) {
                                        province = neighbour;
                                    }
                                } // else - find some other province in region and use that
                                attempts++;
                            }
                        } else { // island region-ish
                            reg.Provinces.Add(province);
                        }

                        provincePool.Remove(province);
                        continentRegions.Add(reg);

                    }

                    if (continentRegions.Count < regionMaxCount) {
                        // pickup the remaining
                    } else if (provincePool.Count > 0) {
                        while (provincePool.Count > 0) {
                            WorldProvince province = provincePool[0];
                            if (province.NeighbourProvinces.Count > 0) {
                                foreach (var neighbour in province.NeighbourProvinces) {
                                    WorldRegion reg = continentRegions.FirstOrDefault(x => x.Provinces.Contains(neighbour));
                                    if (reg != null) {
                                        reg.Provinces.Add(province);
                                        break;
                                    }
                                }
                                if (provincePool.Count > 0 && provincePool[0] == province) {
                                    WorldRegion reg = new WorldRegion();
                                    reg.Provinces.Add(province);
                                    continentRegions.Add(reg);
                                }
                            } else {
                                WorldRegion reg = new WorldRegion();
                                reg.Provinces.Add(province);
                                continentRegions.Add(reg);
                            }
                            provincePool.RemoveAt(0);
                        }
                    }

                    wContinent.Regions.AddRange(continentRegions);

                    continents.Add(wContinent);

                }
            }

        }

    }

}
