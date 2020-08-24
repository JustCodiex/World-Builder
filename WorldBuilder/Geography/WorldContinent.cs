using System;
using System.Collections.Generic;

namespace WorldBuilder.Geography {
    
    public class WorldContinent {
    
        public List<WorldRegion> Regions { get; set; }

        public WorldContinent() {
            this.Regions = new List<WorldRegion>();
        }

    }

}
