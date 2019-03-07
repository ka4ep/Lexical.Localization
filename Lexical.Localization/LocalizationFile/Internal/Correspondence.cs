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

    public class CorrespondenceMap<a, b> : _CorrespondenceContext<a, b> where a : class where b : class
    {
        public CorrespondenceMap<a, b> Map(a a, b b)
        {
            MapA2B.Add(a, b);
            MapB2A.Add(b, a);
            return this;
        }

        static b[] no_b = new b[0];
        static a[] no_a = new a[0];

        public IList<b> GetListOfB(a a)
            => MapA2B.GetList((a)a) ?? (IList<b>)no_b;
        public IList<a> GetListOfA(b b)
            => MapB2A.GetList((b)b) ?? (IList<a>)no_a;
        public bool IsAMappedToB(a a, b b)
        {
            List<b> b_list = MapA2B.GetList(a);
            if (b_list == null) return false;
            return b_list.Contains(b);
        }

        /// <summary>
        /// Test if <paramref name="a"/>'s mappings list is empty, or that contains only <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if <paramref name="a"/>'s mappings is empty, or contains only <paramref name="b"/></returns>
        public bool HasNoMappingOfAOrOnlyToB(a a, b b)
        {
            List<b> b_list = MapA2B.GetList(a);
            if (b_list == null) return true;
            foreach (b _b in b_list)
                if (_b != b) return false;
            return true;
        }

        /// <summary>
        /// Test if <paramref name="b"/>'s mapping list is empty, or that contains only <paramref name="a"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if <paramref name="b"/>'s mappings is empty, or contains only <paramref name="a"/></returns>
        public bool HasNoMappingOfBOrOnlyToA(a a, b b)
        {
            List<b> b_list = MapA2B.GetList(a);
            if (b_list == null) return true;
            foreach (b _b in b_list)
                if (_b != b) return false;
            return true;
        }
    }

}
