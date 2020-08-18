using System;
using System.Collections.Generic;

namespace WorldBuilder.Utility.Algorithms {
    
    public static class DistancePattern {
    
        public static U MinDist<U, V>(this IEnumerable<U> enumerable, V max, U seed, Func<U, U, V> func) where V : IComparable {

            V best = max;
            U bestU = default;
            var itt = enumerable.GetEnumerator();
            while (itt.MoveNext()) {
                V v = func(seed, itt.Current);
                if (v.CompareTo(best) < 0) {
                    best = v;
                    bestU = itt.Current;
                }
            }

            return bestU;

        }

    }

}
