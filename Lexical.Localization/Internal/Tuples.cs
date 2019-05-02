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
        /// <summary>A</summary>
        public readonly A a;
        public readonly B b;
        public readonly int hashcode;

        /// <summary>
        /// Create Pair
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
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

    /// <summary>
    /// Triple (3-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public struct Triple<A, B, C> : IEquatable<Triple<A, B, C>>, IComparable<Triple<A, B, C>>
    {
        public readonly A a;
        public readonly B b;
        public readonly C c;
        public readonly int hashcode;

        public Triple(A a, B b, C c) { this.a = a; this.b = b; this.c = c; hashcode = (a == null ? 0 : 11 * a.GetHashCode()) + (b == null ? 0 : 13 * b.GetHashCode()) + (c == null ? 0 : 17 * c.GetHashCode()); }

        public override int GetHashCode() => hashcode;
        public override bool Equals(object obj) => obj is Triple<A, B, C> other ? EqualityComparer.Default.Equals(this, other) : false;
        public bool Equals(Triple<A, B, C> other) => EqualityComparer.Default.Equals(this, other);
        public int CompareTo(Triple<A, B, C> other) => Comparer.Default.Compare(this, other);

        public class EqualityComparer : IEqualityComparer<Triple<A, B, C>>
        {
            private static EqualityComparer singleton;
            public static EqualityComparer Default => singleton ?? (singleton = new EqualityComparer(EqualityComparer<A>.Default, EqualityComparer<B>.Default, EqualityComparer<C>.Default));

            public readonly IEqualityComparer<A> aComparer;
            public readonly IEqualityComparer<B> bComparer;
            public readonly IEqualityComparer<C> cComparer;

            public EqualityComparer(IEqualityComparer<A> aComparer, IEqualityComparer<B> bComparer, IEqualityComparer<C> cComparer)
            {
                this.aComparer = aComparer ?? throw new ArgumentNullException(nameof(aComparer));
                this.bComparer = bComparer ?? throw new ArgumentNullException(nameof(bComparer));
                this.cComparer = cComparer ?? throw new ArgumentNullException(nameof(cComparer));
            }

            public bool Equals(Triple<A, B, C> x, Triple<A, B, C> y)
            {
                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                return true;
            }

            public int GetHashCode(Triple<A, B, C> obj)
                => (obj.a == null ? 0 : 11 * obj.a.GetHashCode()) + (obj.b == null ? 0 : 13 * obj.b.GetHashCode()) + (obj.c == null ? 0 : 17 * obj.c.GetHashCode());
        }

        public class Comparer : IComparer<Triple<A, B, C>>
        {
            private static Comparer singleton;
            public static Comparer Default => singleton ?? (singleton = new Comparer(System.Collections.Generic.Comparer<A>.Default, System.Collections.Generic.Comparer<B>.Default, System.Collections.Generic.Comparer<C>.Default));
            public readonly IComparer<A> aComparer;
            public readonly IComparer<B> bComparer;
            public readonly IComparer<C> cComparer;

            public Comparer(IComparer<A> aComparer, IComparer<B> bComparer, IComparer<C> cComparer)
            {
                this.aComparer = aComparer ?? throw new ArgumentNullException(nameof(aComparer));
                this.bComparer = bComparer ?? throw new ArgumentNullException(nameof(bComparer));
                this.cComparer = cComparer ?? throw new ArgumentNullException(nameof(cComparer));
            }
            public int Compare(Triple<A, B, C> x, Triple<A, B, C> y)
            {

                int compare = 0;
                compare = aComparer.Compare(x.a, y.a);
                if (compare != 0) return compare;
                compare = bComparer.Compare(x.b, y.b);
                if (compare != 0) return compare;
                compare = cComparer.Compare(x.c, y.c);
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
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(")");
        }

    }

    /// <summary>
    /// Quad (4-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    public struct Quad<A, B, C, D> : IEquatable<Quad<A, B, C, D>>, IComparable<Quad<A, B, C, D>>
    {
        public readonly A a;
        public readonly B b;
        public readonly C c;
        public readonly D d;
        public readonly int hashcode;

        public Quad(A a, B b, C c, D d) { this.a = a; this.b = b; this.c = c; this.d = d; hashcode = (a == null ? 0 : 11 * a.GetHashCode()) + (b == null ? 0 : 13 * b.GetHashCode()) + (c == null ? 0 : 17 * c.GetHashCode()) + (d == null ? 0 : 19 * d.GetHashCode()); }

        public override int GetHashCode() => hashcode;
        public override bool Equals(object obj) => obj is Quad<A, B, C, D> other ? EqualityComparer.Default.Equals(this, other) : false;
        public bool Equals(Quad<A, B, C, D> other) => EqualityComparer.Default.Equals(this, other);
        public int CompareTo(Quad<A, B, C, D> other) => Comparer.Default.Compare(this, other);

        public class EqualityComparer : IEqualityComparer<Quad<A, B, C, D>>
        {
            private static EqualityComparer singleton;
            public static EqualityComparer Default => singleton ?? (singleton = new EqualityComparer(EqualityComparer<A>.Default, EqualityComparer<B>.Default, EqualityComparer<C>.Default, EqualityComparer<D>.Default));

            public readonly IEqualityComparer<A> aComparer;
            public readonly IEqualityComparer<B> bComparer;
            public readonly IEqualityComparer<C> cComparer;
            public readonly IEqualityComparer<D> dComparer;

            public EqualityComparer(IEqualityComparer<A> aComparer, IEqualityComparer<B> bComparer, IEqualityComparer<C> cComparer, IEqualityComparer<D> dComparer)
            {
                this.aComparer = aComparer ?? throw new ArgumentNullException(nameof(aComparer));
                this.bComparer = bComparer ?? throw new ArgumentNullException(nameof(bComparer));
                this.cComparer = cComparer ?? throw new ArgumentNullException(nameof(cComparer));
                this.dComparer = dComparer ?? throw new ArgumentNullException(nameof(dComparer));
            }

            public bool Equals(Quad<A, B, C, D> x, Quad<A, B, C, D> y)
            {
                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                return true;
            }

            public int GetHashCode(Quad<A, B, C, D> obj)
                => (obj.a == null ? 0 : 11 * obj.a.GetHashCode()) + (obj.b == null ? 0 : 13 * obj.b.GetHashCode()) + (obj.c == null ? 0 : 17 * obj.c.GetHashCode()) + (obj.d == null ? 0 : 19 * obj.d.GetHashCode());
        }

        public class Comparer : IComparer<Quad<A, B, C, D>>
        {
            private static Comparer singleton;
            public static Comparer Default => singleton ?? (singleton = new Comparer(System.Collections.Generic.Comparer<A>.Default, System.Collections.Generic.Comparer<B>.Default, System.Collections.Generic.Comparer<C>.Default, System.Collections.Generic.Comparer<D>.Default));
            public readonly IComparer<A> aComparer;
            public readonly IComparer<B> bComparer;
            public readonly IComparer<C> cComparer;
            public readonly IComparer<D> dComparer;

            public Comparer(IComparer<A> aComparer, IComparer<B> bComparer, IComparer<C> cComparer, IComparer<D> dComparer)
            {
                this.aComparer = aComparer ?? throw new ArgumentNullException(nameof(aComparer));
                this.bComparer = bComparer ?? throw new ArgumentNullException(nameof(bComparer));
                this.cComparer = cComparer ?? throw new ArgumentNullException(nameof(cComparer));
                this.dComparer = dComparer ?? throw new ArgumentNullException(nameof(dComparer));
            }
            public int Compare(Quad<A, B, C, D> x, Quad<A, B, C, D> y)
            {

                int compare = 0;
                compare = aComparer.Compare(x.a, y.a);
                if (compare != 0) return compare;
                compare = bComparer.Compare(x.b, y.b);
                if (compare != 0) return compare;
                compare = cComparer.Compare(x.c, y.c);
                if (compare != 0) return compare;
                compare = dComparer.Compare(x.d, y.d);
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
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(")");
        }

    }

}
