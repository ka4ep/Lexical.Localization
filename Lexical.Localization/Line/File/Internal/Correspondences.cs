//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Correspondence of two sets of entities (left and right).
    /// 
    /// Contains mapping of entities (L - R) and lists of unmapped entiries (L and R).
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <typeparam name="R"></typeparam>
    public class Correspondence<L, R> : BijectionMap<L, R>
    {
        /// <summary>
        /// List of entities on the L side that are unmapped.
        /// </summary>
        public List<L> UnmappedL = new List<L>();

        /// <summary>
        /// List of entities on the R side that are unmapped.
        /// </summary>
        public List<R> UnmappedR = new List<R>();

        /// <summary>
        /// Create new container for correspondences.
        /// </summary>
        /// <param name="leftComparer"></param>
        /// <param name="rightComparer"></param>
        public Correspondence(IEqualityComparer<L> leftComparer = default, IEqualityComparer<R> rightComparer = default) : base(leftComparer, rightComparer) { }

        /// <summary>
        /// Clear correspondence container.
        /// </summary>
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
        /// <summary>
        /// Node correspondences.
        /// </summary>
        public readonly Correspondence<ILineTree, XElement> Nodes = new Correspondence<ILineTree, XElement>();

        /// <summary>
        /// Value correspondences
        /// </summary>
        public readonly Dictionary<LineTreeValue, XText> Values = new Dictionary<LineTreeValue, XText>(new KeyValueTreeComparer());
    }

    /// <summary>
    /// Correspondences between <see cref="ILineTree"/> and <see cref="IniToken"/>.
    /// </summary>
    public class IniCorrespondence
    {
        /// <summary>
        /// Node correspondences
        /// </summary>
        public readonly Correspondence<ILineTree, IniToken> Nodes = new Correspondence<ILineTree, IniToken>();

        /// <summary>
        /// Value correspondences
        /// </summary>
        public readonly Dictionary<LineTreeValue, IniToken> Values = new Dictionary<LineTreeValue, IniToken>(new KeyValueTreeComparer());
    }

    /// <summary>
    /// Correspondences for .resx file.
    /// </summary>
    public class ResXCorrespondence
    {
        /// <summary>
        /// Node correspondences
        /// </summary>
        public readonly Correspondence<string, XElement> Nodes = new Correspondence<string, XElement>();

        /// <summary>
        /// Value correspondences
        /// </summary>
        public readonly Dictionary<KeyValuePair<string, IString>, XElement> Values = new Dictionary<KeyValuePair<string, IString>, XElement>(KeyValuePairEqualityComparer<string, IString>.Default);
    }

    /// <summary>
    /// Reference to a value in a <see cref="ILineTree"/>.
    /// </summary>
    public struct LineTreeValue : IEquatable<LineTreeValue>
    {
        /// <summary>
        /// Tree
        /// </summary>
        public readonly ILineTree tree;

        /// <summary>
        /// Value
        /// </summary>
        public readonly ILine value;

        /// <summary>
        /// Value index in the <see cref="tree"/>.
        /// </summary>
        public readonly int valueIndex;

        /// <summary>
        /// Line value tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="value"></param>
        /// <param name="valueIndex"></param>
        public LineTreeValue(ILineTree tree, ILine value, int valueIndex)
        {
            this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
            this.value = value;
            this.valueIndex = valueIndex;
        }

        /// <summary>
        /// Compare for quality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is LineTreeValue other ? tree == other.tree && LineComparer.String.Equals(value, other.value) && valueIndex == other.valueIndex : false;

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LineTreeValue other)
            => tree == other.tree && LineComparer.String.Equals(value, other.value) && valueIndex == other.valueIndex;

        /// <summary>
        /// Calculate hash-code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = -2128831035;
            hash ^= tree.GetHashCode();
            hash *= 0x1000193;
            hash ^= LineComparer.String.GetHashCode(value);
            hash *= 0x1000193;
            hash ^= valueIndex;
            hash *= 0x1000193;
            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{tree}[{valueIndex}]={value}";
    }

    /// <summary>
    /// Comparer class
    /// </summary>
    public class KeyValueTreeComparer : IEqualityComparer<LineTreeValue>
    {
        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(LineTreeValue x, LineTreeValue y)
            => x.tree == y.tree && LineComparer.String.Equals(x.value, y.value) && x.valueIndex == y.valueIndex;

        /// <summary>
        /// Calculate hash-code
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(LineTreeValue obj)
            => obj.GetHashCode();
    }

}

namespace Lexical.Localization.Internal
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// .Json file correspondences
    /// </summary>
    public class JsonCorrespondence
    {
        /// <summary>
        /// Node correspondences
        /// </summary>
        public readonly Correspondence<ILineTree, JToken> Nodes = new Correspondence<ILineTree, JToken>();

        /// <summary>
        /// Value correspondences
        /// </summary>
        public readonly Dictionary<LineTreeValue, JValue> Values = new Dictionary<LineTreeValue, JValue>(new KeyValueTreeComparer());
    }

}