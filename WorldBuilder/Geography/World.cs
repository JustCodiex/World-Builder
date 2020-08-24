using System;
using System.Collections.Generic;
using System.Text;
using WorldBuilder.Graphics;
using WorldBuilder.History;
using WorldBuilder.Utility.Maths.Graphs;

namespace WorldBuilder.Geography {
    
    public class World {

        uint m_width, m_height;
        TimeLine m_history;
        List<WorldContinent> m_continents;
        Graph<WorldProvince, DistanceEdge> m_worldGraph;

        public List<WorldContinent> Continents => this.m_continents;

        public World(uint width, uint height, double worldScale) {
            this.m_width = width;
            this.m_height = height;
            this.m_history = new TimeLine();
            this.m_continents = new List<WorldContinent>();
            this.m_worldGraph = new Graph<WorldProvince, DistanceEdge>();
        }

        public void RecalculateWorldGraph() {



        }

        public void SaveMapToFile(string filename) {

            Render render = new Render(this.m_width, this.m_height);
            render.Clear(0.0f, 0.25f, 0.35f);



            render.RenderToFile(filename);

            this.m_worldGraph.SaveToFile(filename.Replace(".png", "_graph.png"), this.m_width, this.m_height);

        }

    }

}
