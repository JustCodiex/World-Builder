using System;
using System.Collections.Generic;
using WorldBuilder.Utility.Maths.Graphs;

namespace WorldBuilder.Geography {

    public class WorldProvince : IGraphVertex {
    
        public int XPos { get; set; }
        
        public int YPos { get; set; }

        public string Name { get; set; }

        public List<WorldProvince> NeighbourProvinces { get; set; }
    
        public WorldProvince() {
            this.NeighbourProvinces = new List<WorldProvince>();
        }

    }

}
