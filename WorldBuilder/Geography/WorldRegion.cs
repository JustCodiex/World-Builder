using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Geography {

    public class WorldRegion {
    
        public List<WorldProvince> Provinces { get; set; }

        public WorldRegion() {
            this.Provinces = new List<WorldProvince>();
        }

    }

}
