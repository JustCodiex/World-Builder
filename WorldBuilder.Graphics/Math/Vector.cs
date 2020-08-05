using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Graphics.Math {
    
    public struct Vector<T> where T : IComparable {

        private T[] m_dim;

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

        public static implicit operator Vector<T>(T[] a) => new Vector<T>(a);

    }

}
