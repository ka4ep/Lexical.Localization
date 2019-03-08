//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
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
        public readonly Correspondence<KeyTreeValue, XText> Values = new Correspondence<KeyTreeValue, XText>();
    }

    /// <summary>
    /// Reference to a value in a <see cref="IKeyTree"/>.
    /// </summary>
    public class KeyTreeValue // todo hashequals
    {
        public IKeyTree keyTree;
        public string value;
        public int valueIndex;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
