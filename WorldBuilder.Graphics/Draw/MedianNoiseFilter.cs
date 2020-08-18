using System;

namespace WorldBuilder.Graphics.Draw {

    public class MedianNoiseFilter : IFilter {

        public int NoiseSize { get; set; }

        public MedianNoiseFilter() {
            this.NoiseSize = 3;
        }

        public void Apply(RawBitmap bitmap) {

            int ns = NoiseSize * NoiseSize;

            RawBitmap f = new RawBitmap(bitmap);

            int eX = (int)System.Math.Floor(NoiseSize / 2.0);
            int eY = (int)System.Math.Floor(NoiseSize / 2.0);

            byte[] window = new byte[ns];

            for (int x = eX; x < bitmap.Width - eX; x++) {
                for (int y = eY; y < bitmap.Height - eY; y++) {

                    int i = 0;

                    for (int fx = 0; fx < NoiseSize; fx++) {
                        for (int fy = 0; fy < NoiseSize; fy++) {
                            window[i] = bitmap.GetPixel(x + fx - eX, y + fy - eY).R;
                            i++;
                        }
                    }

                    Array.Sort(window);

                    f.SetPixel((uint)x, (uint)y, window[ns / 2]);

                }
            }

            bitmap.Copy(f);

        }

    }

}
