using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using WorldBuilder.Graphics.Math;

namespace WorldBuilder.Graphics {
    
    public unsafe class RawBitmap {

        Vector<byte>[] m_buffer;
        uint m_pCount;
        uint m_width;
        uint m_height;

        public int Width => (int)this.m_width;

        public int Height => (int)this.m_height;

        public RawBitmap(uint width, uint height) {
            this.m_buffer = new Vector<byte>[width * height];
            this.m_width = width;
            this.m_height = height;
            this.m_pCount = width * height;
        }

        public RawBitmap(Bitmap bitmap) {
            this.m_buffer = new Vector<byte>[bitmap.Width * bitmap.Height];
            this.m_width = (uint)bitmap.Width;
            this.m_height = (uint)bitmap.Height;
            this.m_pCount = this.m_width * this.m_height;
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, (int)this.m_width, (int)this.m_height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] buffer = new byte[this.m_pCount * 4];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            int p = 0;
            for (int i = 0; i < buffer.Length; i += 4, p++) {
                m_buffer[p] = new Vector<byte>(buffer[i], buffer[i + 1], buffer[i + 2], buffer[i + 3]);
            }
            bitmap.UnlockBits(data);
        }

        public RawBitmap(RawBitmap bitmap) {
            this.m_buffer = new Vector<byte>[bitmap.m_buffer.Length];
            this.m_width = bitmap.m_width;
            this.m_height = bitmap.m_height;
            this.m_pCount = bitmap.m_pCount;
            this.Copy(bitmap);
        }

        public void Write(Bitmap bmp) {

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)this.m_width, (int)this.m_height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] buffer = new byte[m_pCount * 4];
            fixed(byte* argb = buffer) {
                for (int i = 0; i < this.m_buffer.Length; i++) {
                    byte* pixel = argb + (i << 2);
                    pixel[0] = m_buffer[i][2];
                    pixel[1] = m_buffer[i][1];
                    pixel[2] = m_buffer[i][0];
                    pixel[3] = m_buffer[i][3];
                }
            }
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            bmp.UnlockBits(data);

        }

        public void SetPixel(uint x, uint y, byte brightness)
            => this.SetPixel(x, y, brightness, brightness, brightness, 255);

        public void SetPixel(uint x, uint y, byte r, byte g, byte b)
            => this.SetPixel(x, y, r, g, b, 255);

        public void SetPixel(uint x, uint y, Color colour)
            => this.SetPixel(x, y, colour.R, colour.G, colour.B, colour.A);

        public void SetPixel(uint x, uint y, byte r, byte g, byte b, byte a) 
            => this.m_buffer[(y * this.m_width) + x] = new byte[] { r, g, b, a };

        public void Copy(RawBitmap other) {
            if (other.m_pCount == this.m_pCount) {
                Array.Copy(other.m_buffer, this.m_buffer, this.m_buffer.Length);
            } else {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void Clear(Color colour)
            => this.Clear(colour.R, colour.G, colour.B, colour.A);

        public void Clear(byte r, byte g, byte b)
            => this.Clear(r, g, b, 255);

        public void Clear(byte r, byte g, byte b, byte a) {

            Vector<byte> colour = new Vector<byte>(r, g, b, a);

            for (int i = 0; i < m_pCount; i++) {
                this.m_buffer[i] = colour;
            }

        }

        public Color GetPixel(int x, int y) => this.GetPixel((uint)x, (uint)y);

        public Color GetPixel(uint x, uint y) {
            Vector<byte> colour = this.m_buffer[(y * this.m_width) + x];
            return Color.FromArgb(colour[3], colour[0], colour[1], colour[2]);
        }

        public Vector<byte> GetPixelVector(int x, int y) => this.GetPixelVector((uint)x, (uint)y);

        public Vector<byte> GetPixelVector(uint x, uint y) => this.m_buffer[(y * this.m_width) + x];

    }

}
