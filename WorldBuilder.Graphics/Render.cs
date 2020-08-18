using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using WorldBuilder.Graphics.Draw;

namespace WorldBuilder.Graphics {
    
    public class Render {

        Bitmap m_bmp;
        RawBitmap m_raw;

        public RawBitmap Raw => this.m_raw;

        public Render(uint width, uint height) {
            this.m_bmp = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
            this.m_raw = new RawBitmap(this.m_bmp);
        }

        public void RenderToFile(string file) {
            this.m_raw.Write(this.m_bmp);
            this.m_bmp.Save(file, ImageFormat.Png);
        }

        public void Clear(float r, float g, float b)
            => this.m_raw.Clear((byte)(255.0 * r), (byte)(255.0 * g), (byte)(255.0 * b));
        
        public void SetPixel(int x, int y, (float,float,float) rgb)
            => this.m_raw.SetPixel((uint)x, (uint)y, (byte)(255 * rgb.Item1), (byte)(255 * rgb.Item2), (byte)(255 * rgb.Item3));

        public (float, float, float) GetPixel(int x, int y) {
            var col = this.m_raw.GetPixelVector(x, y);
            return (col[0], col[1], col[2]);
        }

        public Render ToBlackWhite(float cut) {

            Render render = new Render((uint)this.m_bmp.Width, (uint)this.m_bmp.Height);
            render.m_raw.Copy(this.m_raw);

            for (int x = 0; x < this.m_bmp.Width; x++) {
                for (int y = 0; y < this.m_bmp.Height; y++) {
                    Color colour = this.m_raw.GetPixel(x, y);
                    if ((colour.R + colour.G + colour.B) / 3 > cut) {
                        colour = Color.White;
                    } else {
                        colour = Color.Black;
                    }
                    render.m_raw.SetPixel((uint)x, (uint)y, colour);
                }
            }

            return render;

        }

        public void ApplyFilter<T>() where T : IFilter {
            IFilter f = Activator.CreateInstance<T>();
            f.Apply(this.m_raw);
        }

        public void ApplyFilter(IFilter filter)
            => filter.Apply(this.m_raw);

    }

}
