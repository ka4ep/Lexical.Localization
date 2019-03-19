// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Internal
{
    public class EqualsComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            bool xnull = object.ReferenceEquals(x, null), ynull = object.ReferenceEquals(y, null);
            if (xnull && ynull) return true;
            if (xnull || ynull) return false;
            return x.Equals(y);
        }
        public int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }

    public class ReferenceComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y) => object.ReferenceEquals(x, y);
        public int GetHashCode(T obj) => obj == null ? 0 : obj.GetHashCode();
    }

    public class ArrayComparer<Element> : IEqualityComparer<Element[]>
    {
        public readonly IEqualityComparer<Element> elementComparer;

        public ArrayComparer(IEqualityComparer<Element> elementComparer)
        {
            this.elementComparer = elementComparer ?? throw new ArgumentNullException(nameof(elementComparer));
        }

        public const int FNVHashBasis = unchecked((int)2166136261);
        public const int FNVHashPrime = 16777619;

        public bool Equals(Element[] x, Element[] y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            int len = x.Length;
            for (int i = 0; i < len; i++)
                if (!elementComparer.Equals(x[i], y[i])) return false;
            return true;
        }

        public int GetHashCode(Element[] array)
        {
            if (array == null) return 0;
            int result = FNVHashBasis;
            foreach (Element e in array)
            {
                if (e != null) result ^= elementComparer.GetHashCode(e);
                result *= FNVHashPrime;
            }
            return result;
        }
    }
}
