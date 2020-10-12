using System;
using System.Collections.Generic;
using System.Text;
using WorldBuilder.Graphics;
using WorldBuilder.History;
using WorldBuilder.Utility.Functional;
using WorldBuilder.Utility.Maths.Graphs;

namespace WorldBuilder.Geography {
    
    public class World {

        uint m_width, m_height;
        TimeLine m_history;
        List<WorldContinent> m_continents;
        Graph<WorldProvince, DistanceEdge> m_worldGraph;

        public List<WorldContinent> Continents => this.m_continents;

        public TimeLine History { get => this.m_history; set => this.m_history = value; }

        public World(uint width, uint height, double worldScale) {
            this.m_width = width;
            this.m_height = height;
            this.m_history = new TimeLine();
            this.m_continents = new List<WorldContinent>();
            this.m_worldGraph = new Graph<WorldProvince, DistanceEdge>();
        }

        public void RecalculateWorldGraph() {

            this.m_worldGraph = new Graph<WorldProvince, DistanceEdge>();

            for (int i = 0; i < this.m_continents.Count; i++) {
                for (int j = 0; j < this.m_continents[i].Regions.Count; j++) {
                    for (int k = 0; k < this.m_continents[i].Regions[j].Provinces.Count; k++) {
                        this.m_worldGraph.AddVertex(this.m_continents[i].Regions[j].Provinces[k]);
                    }
                }
            }

            for (int i = 0; i < this.m_worldGraph.Vertices.Count; i++) {

                WorldProvince prov = this.m_worldGraph.Vertices[i];

                foreach (WorldProvince n in prov.NeighbourProvinces) {

                    DistanceEdge edge = new DistanceEdge {
                        Distance = (int)Math.Sqrt(Math.Pow(prov.XPos - n.XPos, 2) + Math.Pow(prov.YPos - n.YPos, 2))
                    };

                    this.m_worldGraph.AddEdge(edge, prov, n);

                }

            }

        }

        public void SaveMapToFile(string filename) {

            Render render = new Render(this.m_width, this.m_height);
            render.Clear(0.0f, 0.25f, 0.35f);



            render.RenderToFile(filename);

            this.m_worldGraph.SaveToFile(filename.Replace(".png", "_graph.png"), this.m_width, this.m_height);

        }

        public WorldProvince GetRandomWorldProvince() => this.GetRandomWorldProvince(new Random());

        public WorldProvince GetRandomWorldProvince(Random random) => this.m_continents.Random(random).Regions.Random(random).Provinces.Random(random);

    }

}
