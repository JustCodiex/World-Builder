using System.Drawing;

namespace WorldBuilder.Graphics.Draw {
    
    public interface IFilter {

        void Apply(RawBitmap bitmap);

    }

}
