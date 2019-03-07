//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lexical.Localization.Utils;

namespace Lexical.Localization.Internal
{
    public interface ICorrespondence
    {
        object A { get; set; }
        object B { get; set; }
    }

    public interface ICorrespondence<a, b> : ICorrespondence
    {
        new a A { get; set; }
        new b B { get; set; }
    }

    public abstract class CorrespondenceMap
    {
        public abstract CorrespondenceMap Map(object a, object b);

        public abstract IList GetListOfB(object a);
        public abstract IList GetListOfA(object b);
    }

    public class _CorrespondenceContext<a, b> : CorrespondenceMap
    {
        public readonly MapList<a, b> MapA2B = new MapList<a, b>();
        public readonly MapList<b, a> MapB2A = new MapList<b, a>();
        public readonly List<a> UnmappedA = new List<a>();
        public readonly List<b> UnmappedB = new List<b>();

        public override CorrespondenceMap Map(object a, object b)
        {
            MapA2B.Add((a)a, (b)b);
            MapB2A.Add((b)b, (a)a);
            return this;
        }

        public override IList GetListOfB(object a)
            => MapA2B.GetList((a)a);
        public override IList GetListOfA(object b)
            => MapB2A.GetList((b)b);
    }

    public class CorrespondenceMap<a, b> : _CorrespondenceContext<a, b>
    {
        public CorrespondenceMap<a, b> Map(a a, b b)
        {
            MapA2B.Add(a, b);
            MapB2A.Add(b, a);
            return this;
        }

        public IList<b> GetListOfB(a a)
            => MapA2B.GetList((a)a);
        public IList<a> GetListOfA(b b)
            => MapB2A.GetList((b)b);
    }

}
