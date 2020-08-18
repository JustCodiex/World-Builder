using System;

namespace WorldBuilder.Graphics.Draw {

    public class CrystalizeFilter : IFilter {

        public int CellSize { get; set; }

        public Random Generator { get; set; }

        public CrystalizeFilter() {
            this.CellSize = 5;
            this.Generator = new Random();
        }

        public void Apply(RawBitmap bitmap) {

            RawBitmap f = new RawBitmap(bitmap);

            int cellW = bitmap.Width / this.CellSize;
            int cellH = bitmap.Height / this.CellSize;

            (int, int)[,] points = new (int, int)[cellW, cellH];
            int[,] pColour = new int[cellW, cellH];

            int gx = 0;
            for (int x = 0; x < bitmap.Width - this.CellSize; x += this.CellSize, gx++) {
                int gy = 0;
                for (int y = 0; y < bitmap.Height - this.CellSize; y += this.CellSize, gy++) {
                    points[gx, gy] = (this.Generator.Next(x, x + this.CellSize), this.Generator.Next(y, y + this.CellSize));
                    pColour[gx, gy] = bitmap.GetPixelVector(points[gx,gy].Item1, points[gx, gy].Item2)[0];
                }
            }

            for (int x = 0; x < bitmap.Width; x++) {
                for (int y = 0; y < bitmap.Height; y++) {

                    int cx = x / this.CellSize;
                    int cy = y / this.CellSize;
                    int maxD = int.MaxValue;
                    int bx = 0;
                    int by = 0;

                    for (int ccx = cx - 3; ccx < cx + 3; ccx++) {
                        for (int ccy = cy - 3; ccy < cy + 3; ccy++) {
                            if (ccx >= 0 && ccy >= 0 && ccx < cellW && ccy < cellH) {
                                (int w, int h) = points[ccx, ccy];
                                int d = (w - x) * (w - x) + (h - y) * (h - y);
                                if (d < maxD) {
                                    maxD = d;
                                    bx = ccx;
                                    by = ccy;
                                }
                            }
                        }
                    }

                    f.SetPixel((uint)x, (uint)y, (byte)pColour[bx, by]);

                }
            }

            bitmap.Copy(f);

        }

    }

}
