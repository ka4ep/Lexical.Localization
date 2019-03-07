//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Lexical.Localization.Utils;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Correspondence between <see cref="IKeyTree"/> and <see cref="XDocument"/>.
    /// </summary>
    public class KeyTreeXmlCorrespondence
    {
        public readonly CorrespondenceMap<IKeyTree, XElement> Nodes = new CorrespondenceMap<IKeyTree, XElement>();
        public readonly CorrespondenceMap<KeyTreeValue, XText> Values = new CorrespondenceMap<KeyTreeValue, XText>();
    }

    /// <summary>
    /// Reference to a value in a <see cref="IKeyTree"/>.
    /// </summary>
    public struct KeyTreeValue // todo hashequals
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
