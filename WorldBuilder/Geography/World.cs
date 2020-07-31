using System;
using System.Collections.Generic;
using System.Text;
using WorldBuilder.Graphics;

namespace WorldBuilder.Geography {
    
    public class World {

        uint m_width, m_height;

        public World(uint width, uint height, double worldScale) {
            this.m_width = width;
            this.m_height = height;
        }

        public void SaveMapToFile(string filename) {

            Render render = new Render(this.m_width, this.m_height);
            render.Clear(0.0f, 0.25f, 0.35f);



            render.RenderToFile(filename);

        }

    }

}
