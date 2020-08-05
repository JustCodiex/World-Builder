using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldBuilder.Utility.Functional {
    
    public static class IFunctionalEnumerable {

        public static T Random<T>(this IEnumerable<T> enumerable)
            => enumerable.Random(new Random());

        public static T Random<T>(this IEnumerable<T> enumerable, Random random)
            => enumerable.ElementAtOrDefault(random.Next(0, enumerable.Count()));

    }

}
