using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
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

        private int[,] m_regionIndex;
        private bool[,] m_border;
        private Dictionary<int, VoroniRegion> m_regions;

        public int Width => this.m_width;

        public int Height => this.m_height;

        public int RegionCount => this.m_regions.Count;

        public VoroniDiagram(int width, int height) {

            this.m_width = width;
            this.m_height = height;
            this.m_render = new Render((uint)width, (uint)height);
            this.m_regionIndex = new int[this.m_width, this.m_height];
            this.m_border = new bool[this.m_width, this.m_height];

        }

        public void GenerateVoroni(HashSet<(int, int)> points, DistanceMethod method) {

            ImmutableArray<(int, int)> pointsImm = points.ToImmutableArray();
            this.m_regions = new Dictionary<int, VoroniRegion>();
            for (int i = 0; i < this.m_regions.Count; i++) {
                this.m_regions.Add(i, null);
            }

            Random random = new Random(pointsImm.Length);
            Dictionary<(int, int), (float, float, float)> colours = new Dictionary<(int, int), (float, float, float)>();
            bool isEuclidean = method == DistanceMethod.Euclidean;

            for (int i = 0; i < pointsImm.Length; i++) {
                (float, float, float) colour = ((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                colours.Add(pointsImm[i], colour);
            }

            for (int x = 0; x < this.m_width; x++) {
                for (int y = 0; y < this.m_height; y++) {

                    double min = double.MaxValue;
                    int index = 0;
                    for (int i = 0; i < pointsImm.Length; i++) {
                        (int, int) p = pointsImm[i];
                        double dis;
                        if (isEuclidean) {
                            dis = Math.Sqrt(Math.Pow(p.Item1 - x, 2) + Math.Pow(p.Item2 - y, 2));
                        } else {
                            dis = Math.Abs(p.Item1 - x) + Math.Abs(p.Item2 - y);
                        }
                        if (dis < min) {
                            min = dis;
                            index = i;
                        }
                    }

                    this.m_regionIndex[x, y] = index;
                    this.m_render.SetPixel(x,y, colours[pointsImm[index]]);

                }
            }

        }

        public void Borderise(bool drawBorders) {

            for (int x = 0; x < this.m_width; x++) {
                for (int y = 0; y < this.m_height; y++) {

                    for (int px = x - 1; px < x + 1; px++) {
                        for (int py = y - 1; py < y + 1; py++) {
                            if (px >= 0 && px < this.m_width - 1 && py >= 0 && py < this.m_height - 1) {
                                if (this.m_regionIndex[x,y] != this.m_regionIndex[px, py]) {
                                    this.m_border[x,y] = true;
                                    goto BORDERCHECKOVER;
                                }
                            } else {
                                this.m_border[x, y] = true;
                                goto BORDERCHECKOVER;
                            }
                        }
                    }

                BORDERCHECKOVER:

                    if (drawBorders) {
                        if (this.m_border[x, y] && x > 0 && y > 0 && x < this.m_width - 1 && y < this.m_height - 1) {
                            this.m_render.SetPixel(x, y, (0.0f, 0.0f, 0.0f));
                        }
                    }

                }
            }

        }

        public void CaptureRegions(bool drawRegions) {

            SortedDictionary<int, List<(int, int)>> regionalPoints = new SortedDictionary<int, List<(int, int)>>();
            SortedDictionary<int, List<(int, int)>> regionalCorners = new SortedDictionary<int, List<(int, int)>>();

            for (int x = 0; x < this.m_width; x++) {
                for (int y = 0; y < this.m_height; y++) {

                    int i = this.m_regionIndex[x, y];

                    if (regionalPoints.ContainsKey(i)) {
                        regionalPoints[i].Add((x, y));
                    } else {
                        regionalPoints.Add(i, new List<(int, int)>() { (x, y) });
                    }

                }
            }

            foreach (KeyValuePair<int, List<(int,int)>> pair in regionalPoints) {
                List<(int, int, (bool,bool,bool,bool))> corners = new List<(int, int, (bool, bool, bool, bool))>();
                for (int i = 0; i < pair.Value.Count; i++) {
                    int x = pair.Value[i].Item1;
                    int y = pair.Value[i].Item2;
                    if (this.m_border[x,y]) {

                        int otherCount = 0;
                        bool top = false;
                        bool bottom = false;
                        bool left = false;
                        bool right = false;

                        if (y - 1 < 0 || this.m_regionIndex[x, y - 1] != pair.Key) {
                            otherCount++;
                            top = true;
                        }

                        if (y + 1 >= this.m_height || this.m_regionIndex[x, y + 1] != pair.Key) {
                            otherCount++;
                            bottom = true;
                        }

                        if (x - 1 < 0 || this.m_regionIndex[x - 1, y] != pair.Key) {
                            otherCount++;
                            left = true;
                        }

                        if (x + 1 >= this.m_width || this.m_regionIndex[x + 1, y] != pair.Key) {
                            otherCount++;
                            right = true;
                        }

                        if (otherCount >= 2) {
                            corners.Add((x, y, (top, bottom, left, right)));
                        }

                    }
                }
                
                Stack<int> redundant = new Stack<int>();
                for (int i = 0; i < corners.Count; i++) {
                    bool bLeft = corners.Contains((corners[i].Item1 - 1, corners[i].Item2 - 1, corners[i].Item3));
                    bool bRight = corners.Contains((corners[i].Item1 - 1, corners[i].Item2 + 1, corners[i].Item3));
                    bool tLeft = corners.Contains((corners[i].Item1 + 1, corners[i].Item2 - 1, corners[i].Item3));
                    bool tRight = corners.Contains((corners[i].Item1 + 1, corners[i].Item2 + 1, corners[i].Item3));
                    if ( (bLeft && tRight) || (tLeft && bRight) ) {
                        redundant.Push(i);
                    }
                }
                
                while(redundant.Count > 0) {
                    corners.RemoveAt(redundant.Pop());
                }

                if (drawRegions) {
                    corners.ForEach(x => this.m_render.SetPixel(x.Item1, x.Item2, (0.5f, 0.5f, 0.5f)));
                }

                this.m_regions[pair.Key] = new VoroniRegion() {
                    Vertices = corners.Select(x => (x.Item1, x.Item2)).ToList()
                };

                this.m_regions[pair.Key].CalculateBoundingBox();

            }

        }

        public void SubDivide(Random random) {

            for (int i = 0; i < this.m_regions.Count; i++) {

                if (this.m_regions[i].Bounding.Height <= 7 || this.m_regions[i].Bounding.Width <= 7) {
                    continue;
                }

                HashSet<(int, int)> points = new HashSet<(int, int)>();
                int genCount = random.Next(3, 6);

                while (points.Count < genCount) {

                    int x = random.Next(this.m_regions[i].Bounding.MinX, this.m_regions[i].Bounding.MaxX);
                    int y = random.Next(this.m_regions[i].Bounding.MinY, this.m_regions[i].Bounding.MaxY);

                    if (this.m_regionIndex[x, y] != i) {
                        continue;
                    }

                    bool any = false;

                    for (int px = x - 4; px < x + 4; px++) {
                        if (!any) {
                            for (int py = y - 4; py < y + 4; py++) {
                                if (px < 0 || py < 0 || px >= this.m_width || py >= this.m_height || this.m_border[px, py]) {
                                    any = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!any) {

                        if (points.Add((x, y))) {

                            this.m_render.SetPixel(x, y, (1.0f, 0.0f, 0.0f));

                        }

                    }

                }

                if (points.Count > 1) {

                    VoroniDiagram subDiagram = new VoroniDiagram(this.m_regions[i].Bounding.Width, this.m_regions[i].Bounding.Height);
                    subDiagram.GenerateVoroni(points, DistanceMethod.Euclidean);
                    subDiagram.SaveToFile($"sub{i}.png");

                }

            }

        }

        public void RemoveRegions(Func<VoroniRegion, VoroniDiagram, bool> predicate) {
            Stack<int> remove = new Stack<int>();
            foreach (var pair in this.m_regions) {
                if (predicate(pair.Value, this)) {
                    int i = pair.Key;
                    for (int x = this.m_regions[i].Bounding.MinX; x < this.m_regions[i].Bounding.MaxX; x++) {
                        for (int y = this.m_regions[i].Bounding.MinY; y < this.m_regions[i].Bounding.MaxY; y++) {

                            if (this.m_regionIndex[x, y] == i) {
                                this.m_render.SetPixel(x, y, (0.0f, 0.0f, 0.0f));
                            }

                        }
                    }

                    remove.Push(pair.Key);

                }
            }
            while(remove.Count > 0) {
                this.m_regions.Remove(remove.Pop());
            }
        }

        public List<VoroniRegion> SelectRegion(Func<VoroniRegion, VoroniDiagram, bool> predicate) {
            List<VoroniRegion> regions = new List<VoroniRegion>();
            foreach (var pair in this.m_regions) {
                if (predicate(pair.Value, this)) {
                    regions.Add(pair.Value);
                }
            }
            return regions;
        }

        public VoroniRegion SelectRegion(int index) {
            if (this.m_regions.TryGetValue(index, out VoroniRegion vr)) {
                return vr;
            } else {
                return null;
            }
        }

        public List<VoroniRegion> SelectNeighbours(VoroniRegion source) {

            HashSet<VoroniRegion> neighbours = new HashSet<VoroniRegion>();

            for (int x = source.Bounding.MinX - 5; x < source.Bounding.MaxX + 5; x++) {
                for (int y = source.Bounding.MinY - 5; y < source.Bounding.MaxY + 5; y++) {
                    if (x >= 0 && y >= 0 && x < this.m_width && y < this.m_height) {
                        int i = this.m_regionIndex[x, y];
                        if (this.m_regions.TryGetValue(i, out VoroniRegion d)) {
                            if (d != source) {
                                neighbours.Add(d);
                            }
                        }
                    }
                }
            }

            return neighbours.ToList();

        }

        public void SaveToFile(string file)
            => m_render.RenderToFile(file);

    }

}
