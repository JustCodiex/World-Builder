using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace WorldBuilder.Graphics {
    
    public class Render {

        Bitmap m_bmp;
        System.Drawing.Graphics m_graphics;

        public Render(uint width, uint height) {
            this.m_bmp = new Bitmap((int)width, (int)height);
            this.m_graphics = System.Drawing.Graphics.FromImage(this.m_bmp);
        }

        public void RenderToFile(string file) {
            this.m_graphics.Flush();
            this.m_bmp.Save(file, ImageFormat.Png);
        }

        public void Clear(float r, float g, float b)
            => this.m_graphics.Clear(Color.FromArgb((int)(255.0 * r), (int)(255.0 * g), (int)(255.0 * b)));
        
        public void SetPixel(int x, int y, (float,float,float) rgb) {
            this.m_graphics.DrawRectangle(new Pen(Color.FromArgb((int)(255 * rgb.Item1), (int)(255 * rgb.Item2), (int)(255 * rgb.Item3))), new Rectangle(x, y, 1, 1));
        }

    }

}
