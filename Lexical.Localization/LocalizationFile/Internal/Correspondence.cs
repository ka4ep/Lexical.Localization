//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Lexical.Localization.Internal
{
    public class Correspondence<L, R> : BijectionMap<L, R>
    {
        public List<L> UnmappedL = new List<L>();
        public List<R> UnmappedR = new List<R>();
        public Correspondence(IEqualityComparer<L> leftComparer = default, IEqualityComparer<R> rightComparer = default) : base(leftComparer, rightComparer) { }
    }
    /// <summary>
    /// Correspondence between <see cref="IKeyTree"/> and <see cref="XDocument"/>.
    /// </summary>
    public class KeyTreeXmlCorrespondence
    {
        public readonly Correspondence<IKeyTree, XElement> Nodes = new Correspondence<IKeyTree, XElement>();
        public readonly Correspondence<KeyTreeValue, XText> Values = new Correspondence<KeyTreeValue, XText>(new KeyValueTreeComparer());
    }

    /// <summary>
    /// Reference to a value in a <see cref="IKeyTree"/>.
    /// </summary>
    public struct KeyTreeValue : IEquatable<KeyTreeValue>
    {
        public readonly IKeyTree tree;
        public readonly string value;
        public readonly int valueIndex;

        public KeyTreeValue(IKeyTree tree, string value, int valueIndex)
        {
            this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
            this.value = value;
            this.valueIndex = valueIndex;
        }

        public override bool Equals(object obj)
            => obj is KeyTreeValue other ? tree == other.tree && value == other.value && valueIndex == other.valueIndex : false;
        public bool Equals(KeyTreeValue other)
            => tree == other.tree && value == other.value && valueIndex == other.valueIndex;
        public override int GetHashCode()
        {
            int hash = -2128831035;
            hash ^= tree.GetHashCode();
            hash *= 0x1000193;
            hash ^= value == null ? 0 : value.GetHashCode();
            hash *= 0x1000193;
            hash ^= valueIndex;
            hash *= 0x1000193;
            return hash;
        }
        public override string ToString()
            => $"{tree}[{valueIndex}]={value}";
    }

    public class KeyValueTreeComparer : IEqualityComparer<KeyTreeValue>
    {
        public bool Equals(KeyTreeValue x, KeyTreeValue y)
            => x.tree == y.tree && x.value == y.value && x.valueIndex == y.valueIndex;

        public int GetHashCode(KeyTreeValue obj)
            => obj.GetHashCode();
    }

}
