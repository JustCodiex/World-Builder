using System;
using System.Collections.Generic;
using System.Text;
using WorldBuilder.Graphics;

namespace WorldBuilder.Utility.Maths.Graphs {
    
    public class Graph<TVertex, TEdge> 
        where TVertex : IGraphVertex 
        where TEdge : IGraphEdge {

        private struct _Edge {
            TEdge EdgeType;
            TVertex VA;
            TVertex VB;
            public _Edge(TEdge e, TVertex a, TVertex b) {
                this.EdgeType = e;
                this.VA = a;
                this.VB = b;
            }
        }

        List<TVertex> m_vertices;
        List<_Edge> m_edges;

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

            Render render = new Render(w, h);

            for (int i = 0; i < m_vertices.Count; i++) {
                render.SetPixel(m_vertices[i].XPos, m_vertices[i].YPos, (0, 0, 0));
            }

            for (int i = 0; i < m_edges.Count; i++) {

            }

            render.RenderToFile(filepath);

        }

    }

}
