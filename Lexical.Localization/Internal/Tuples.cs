// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Pair (2-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public struct Pair<A, B> : IEquatable<Pair<A, B>>, IComparable<Pair<A, B>>
    {
        public readonly A a;
        public readonly B b;
        public readonly int hashcode;

        public Pair(A a, B b) { this.a = a; this.b = b; hashcode = (a == null ? 0 : 11 * a.GetHashCode()) + (b == null ? 0 : 13 * b.GetHashCode()); }

        public override int GetHashCode() => hashcode;
        public override bool Equals(object obj) => obj is Pair<A, B> other ? EqualityComparer.Default.Equals(this, other) : false;
        public bool Equals(Pair<A, B> other) => EqualityComparer.Default.Equals(this, other);
        public int CompareTo(Pair<A, B> other) => Comparer.Default.Compare(this, other);

        public class EqualityComparer : IEqualityComparer<Pair<A, B>>
        {
            private static EqualityComparer instance;
            public static EqualityComparer Default => instance ?? (instance = new EqualityComparer(EqualityComparer<A>.Default, EqualityComparer<B>.Default));

            public readonly IEqualityComparer<A> aComparer;
            public readonly IEqualityComparer<B> bComparer;

            public EqualityComparer(IEqualityComparer<A> aComparer, IEqualityComparer<B> bComparer)
            {
                this.aComparer = aComparer ?? throw new ArgumentNullException(nameof(aComparer));
                this.bComparer = bComparer ?? throw new ArgumentNullException(nameof(bComparer));
            }

            public bool Equals(Pair<A, B> x, Pair<A, B> y)
            {
                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                return true;
            }

            public int GetHashCode(Pair<A, B> obj)
                => (obj.a == null ? 0 : 11 * obj.a.GetHashCode()) + (obj.b == null ? 0 : 13 * obj.b.GetHashCode());
        }

        public class Comparer : IComparer<Pair<A, B>>
        {
            private static Comparer instance;
            public static Comparer Default => instance ?? (instance = new Comparer(System.Collections.Generic.Comparer<A>.Default, System.Collections.Generic.Comparer<B>.Default));
            public readonly IComparer<A> aComparer;
            public readonly IComparer<B> bComparer;

            public Comparer(IComparer<A> aComparer, IComparer<B> bComparer)
            {
                this.aComparer = aComparer ?? throw new ArgumentNullException(nameof(aComparer));
                this.bComparer = bComparer ?? throw new ArgumentNullException(nameof(bComparer));
            }
            public int Compare(Pair<A, B> x, Pair<A, B> y)
            {

                int compare = 0;
                compare = aComparer.Compare(x.a, y.a);
                if (compare != 0) return compare;
                compare = bComparer.Compare(x.b, y.b);
                if (compare != 0) return compare;
                return 0;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        public void AppendTo(StringBuilder sb)
        {
            sb.Append("Pair(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(")");
        }

    }
}
