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

        public override void Clear()
        {
            base.Clear();
            UnmappedL.Clear();
            UnmappedR.Clear();
        }
    }

    /// <summary>
    /// Correspondence between <see cref="IKeyTree"/> and <see cref="XDocument"/>.
    /// </summary>
    public class XmlCorrespondence
    {
        public readonly Correspondence<IKeyTree, XElement> Nodes = new Correspondence<IKeyTree, XElement>();
        public readonly Dictionary<KeyTreeValue, XText> Values = new Dictionary<KeyTreeValue, XText>(new KeyValueTreeComparer());
    }

    public class IniCorrespondence
    {
        public readonly Correspondence<IKeyTree, IniToken> Nodes = new Correspondence<IKeyTree, IniToken>();
        public readonly Dictionary<KeyTreeValue, IniToken> Values = new Dictionary<KeyTreeValue, IniToken>(new KeyValueTreeComparer());
    }

    public class ResXCorrespondence
    {
        public readonly Correspondence<string, XElement> Nodes = new Correspondence<string, XElement>();
        public readonly Dictionary<KeyValuePair<string, IFormulationString>, XElement> Values = new Dictionary<KeyValuePair<string, IFormulationString>, XElement>(KeyValuePairEqualityComparer<string, IFormulationString>.Default);
    }

    /// <summary>
    /// Reference to a value in a <see cref="IKeyTree"/>.
    /// </summary>
    public struct KeyTreeValue : IEquatable<KeyTreeValue>
    {
        public readonly IKeyTree tree;
        public readonly IFormulationString value;
        public readonly int valueIndex;

        public KeyTreeValue(IKeyTree tree, IFormulationString value, int valueIndex)
        {
            this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
            this.value = value;
            this.valueIndex = valueIndex;
        }

        public override bool Equals(object obj)
            => obj is KeyTreeValue other ? tree == other.tree && FormulationStringComparer.Instance.Equals(value, other.value) && valueIndex == other.valueIndex : false;

        public bool Equals(KeyTreeValue other)
            => tree == other.tree && FormulationStringComparer.Instance.Equals(value, other.value) && valueIndex == other.valueIndex;

        public override int GetHashCode()
        {
            int hash = -2128831035;
            hash ^= tree.GetHashCode();
            hash *= 0x1000193;
            hash ^= FormulationStringComparer.Instance.GetHashCode(value);
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
            => x.tree == y.tree && FormulationStringComparer.Instance.Equals(x.value, y.value) && x.valueIndex == y.valueIndex;

        public int GetHashCode(KeyTreeValue obj)
            => obj.GetHashCode();
    }

}

namespace Lexical.Localization.Internal
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JsonCorrespondence
    {
        public readonly Correspondence<IKeyTree, JToken> Nodes = new Correspondence<IKeyTree, JToken>();
        public readonly Dictionary<KeyTreeValue, JValue> Values = new Dictionary<KeyTreeValue, JValue>(new KeyValueTreeComparer());
    }

}