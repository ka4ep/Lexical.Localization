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
    /// Correspondence between <see cref="ILineTree"/> and <see cref="XDocument"/>.
    /// </summary>
    public class XmlCorrespondence
    {
        public readonly Correspondence<ILineTree, XElement> Nodes = new Correspondence<ILineTree, XElement>();
        public readonly Dictionary<LineTreeValue, XText> Values = new Dictionary<LineTreeValue, XText>(new KeyValueTreeComparer());
    }

    public class IniCorrespondence
    {
        public readonly Correspondence<ILineTree, IniToken> Nodes = new Correspondence<ILineTree, IniToken>();
        public readonly Dictionary<LineTreeValue, IniToken> Values = new Dictionary<LineTreeValue, IniToken>(new KeyValueTreeComparer());
    }

    public class ResXCorrespondence
    {
        public readonly Correspondence<string, XElement> Nodes = new Correspondence<string, XElement>();
        public readonly Dictionary<KeyValuePair<string, IFormulationString>, XElement> Values = new Dictionary<KeyValuePair<string, IFormulationString>, XElement>(KeyValuePairEqualityComparer<string, IFormulationString>.Default);
    }

    /// <summary>
    /// Reference to a value in a <see cref="ILineTree"/>.
    /// </summary>
    public struct LineTreeValue : IEquatable<LineTreeValue>
    {
        public readonly ILineTree tree;
        public readonly IFormulationString value;
        public readonly int valueIndex;

        public LineTreeValue(ILineTree tree, IFormulationString value, int valueIndex)
        {
            this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
            this.value = value;
            this.valueIndex = valueIndex;
        }

        public override bool Equals(object obj)
            => obj is LineTreeValue other ? tree == other.tree && FormulationStringComparer.Instance.Equals(value, other.value) && valueIndex == other.valueIndex : false;

        public bool Equals(LineTreeValue other)
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

    public class KeyValueTreeComparer : IEqualityComparer<LineTreeValue>
    {
        public bool Equals(LineTreeValue x, LineTreeValue y)
            => x.tree == y.tree && FormulationStringComparer.Instance.Equals(x.value, y.value) && x.valueIndex == y.valueIndex;

        public int GetHashCode(LineTreeValue obj)
            => obj.GetHashCode();
    }

}

namespace Lexical.Localization.Internal
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JsonCorrespondence
    {
        public readonly Correspondence<ILineTree, JToken> Nodes = new Correspondence<ILineTree, JToken>();
        public readonly Dictionary<LineTreeValue, JValue> Values = new Dictionary<LineTreeValue, JValue>(new KeyValueTreeComparer());
    }

}