using System.Collections.Generic;
using WorldBuilder.Graphics;

namespace WorldBuilder.Utility.Maths.Graphs {
    
    public class Graph<TVertex, TEdge> 
        where TVertex : IGraphVertex 
        where TEdge : IGraphEdge {

        private struct _Edge {
            public TEdge EdgeType;
            public TVertex VA;
            public TVertex VB;
            public _Edge(TEdge e, TVertex a, TVertex b) {
                this.EdgeType = e;
                this.VA = a;
                this.VB = b;
            }
        }

        List<TVertex> m_vertices;
        List<_Edge> m_edges;

        public List<TVertex> Vertices => this.m_vertices;

        public Graph() {
            this.m_vertices = new List<TVertex>();
            this.m_edges = new List<_Edge>();
        }

        public void AddVertex(TVertex vertex) => this.m_vertices.Add(vertex);

        public void AddEdge(TEdge e, TVertex a, TVertex b) => this.m_edges.Add(new _Edge(e, a, b));

        public void Clear() {
            this.m_vertices.Clear();
            this.m_edges.Clear();
        }

        public void SaveToFile(string filepath, uint w, uint h) {

            Render render = new Render(w*2, h*2);
            render.Clear(1.0f, 1.0f, 1.0f);

            (float, float, float) rgb = (0, 0, 0);

            for (int i = 0; i < m_vertices.Count; i++) {
                render.SetPixel(m_vertices[i].XPos, m_vertices[i].YPos, rgb);
            }

            for (int i = 0; i < m_edges.Count; i++) {
                render.DrawLine(m_edges[i].VA.XPos, m_edges[i].VA.YPos, m_edges[i].VB.XPos, m_edges[i].VB.YPos, rgb);
            }

            render.RenderToFile(filepath);

        }

    }

}
