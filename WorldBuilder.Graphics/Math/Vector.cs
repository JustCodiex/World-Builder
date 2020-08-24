using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldBuilder.Graphics.Math {
    
    public struct Vector<T> where T : IComparable {

        private T[] m_dim;

        public float Length => (float)System.Math.Sqrt(this.m_dim.Select(x => System.Math.Pow((dynamic)x, 2)).Aggregate(0.0f, (a, b) => (float)(a + b)));

        public int Size => m_dim.Length;

        public Vector(int length) {
            this.m_dim = new T[length];
        }

        public Vector(params T[] vars) {
            this.m_dim = vars;
        }

        public T this[int d] {
            get => m_dim[d];
            set => m_dim[d] = value;
        }

        public float DistanceTo(Vector<T> v) {
            if (v.m_dim.Length == this.m_dim.Length) {
                return (float)System.Math.Sqrt(this.m_dim.Zip(v.m_dim).Select(x => System.Math.Pow((dynamic)((dynamic)x.First - (dynamic)x.Second), 2)).Aggregate(0.0f, (a, b) => (float)(a + b)));
            } else {
                throw new ArgumentOutOfRangeException();
            }
        }

        public Vector<T> Normalize() {
            float len = this.Length;
            Vector<T> v = new Vector<T>(m_dim.Length);
            for (int i = 0; i < m_dim.Length; i++) {
                v[i] = (T)((dynamic)m_dim[i] / len);
            }
            return v;
        }

        public static Vector<T> operator*(Vector<T> v, float t) {
            Vector<T> r = new Vector<T>(v.m_dim.Length);
            for (int i = 0; i < r.m_dim.Length; i++) {
                r[i] = (T)((dynamic)v.m_dim[i] * t);
            }
            return r;
        }

        public static Vector<T> operator +(Vector<T> a, Vector<T> b) {
            if (a.m_dim.Length != b.m_dim.Length) {
                throw new ArgumentOutOfRangeException();
            }
            Vector<T> r = new Vector<T>(a.m_dim.Length);
            for (int i = 0; i < r.m_dim.Length; i++) {
                r[i] = (T)((dynamic)a.m_dim[i] + (dynamic)b.m_dim[i]);
            }
            return r;
        }

        public static Vector<T> operator -(Vector<T> a, Vector<T> b) {
            if (a.m_dim.Length != b.m_dim.Length) {
                throw new ArgumentOutOfRangeException();
            }
            Vector<T> r = new Vector<T>(a.m_dim.Length);
            for (int i = 0; i < r.m_dim.Length; i++) {
                r[i] = (T)((dynamic)b.m_dim[i] - (dynamic)a.m_dim[i]);
            }
            return r;
        }

        public static implicit operator Vector<T>(T[] a) => new Vector<T>(a);

        public override string ToString() => $"({string.Join(", ", m_dim)})";

    }

}
