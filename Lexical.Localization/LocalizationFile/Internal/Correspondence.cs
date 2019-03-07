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

    public class KeyTreeCorrespondence<T> : ICorrespondence<IKeyTree, T>
    {
        object ICorrespondence.A { get => a; set => a = (IKeyTree)value; }
        object ICorrespondence.B { get => b; set => b = (T)value; }
        IKeyTree ICorrespondence<IKeyTree, T>.A { get => a; set => a = (IKeyTree)value; }
        T ICorrespondence<IKeyTree, T>.B { get => b; set => b = (T)value; }
        IKeyTree a;
        T b;
    }

    public class KeyTreeValueCorrespondence<T> : ICorrespondence<KeyTreeValue, T>
    {
        object ICorrespondence.A { get => a; set => a = (KeyTreeValue)value; }
        object ICorrespondence.B { get => b; set => b = (T)value; }
        KeyTreeValue ICorrespondence<KeyTreeValue, T>.A { get => a; set => a = (KeyTreeValue)value; }
        T ICorrespondence<KeyTreeValue, T>.B { get => b; set => b = (T)value; }
        KeyTreeValue a;
        T b;
    }

    /// <summary>
    /// Reference to a value in a <see cref="IKeyTree"/>.
    /// </summary>
    public struct KeyTreeValue // todo hashequals
    {
        public IKeyTree keyTree;
        public string value;
        public int valueIndex;
    }

    public abstract class CorrespondenceContext
    {
        public abstract CorrespondenceContext Map(object a, object b);
        public abstract IList GetListOfB(object a);
        public abstract IList GetListOfA(object b);
    }

    public class _CorrespondenceContext<a, b> : CorrespondenceContext
    {
        public readonly MapList<a, b> A2B = new MapList<a, b>();
        public readonly MapList<b, a> B2A = new MapList<b, a>();

        public override CorrespondenceContext Map(object a, object b)
        {
            A2B.Add((a)a, (b)b);
            B2A.Add((b)b, (a)a);
            return this;
        }

        public override IList GetListOfB(object a)
            => A2B.GetList((a)a);
        public override IList GetListOfA(object b)
            => B2A.GetList((b)b);
    }

    public class CorrespondenceContext<a, b> : _CorrespondenceContext<a, b>
    {
        public CorrespondenceContext<a, b> Map(a a, b b)
        {
            A2B.Add(a, b);
            B2A.Add(b, a);
            return this;
        }

        public IList<b> GetListOfB(a a)
            => A2B.GetList((a)a);
        public IList<a> GetListOfA(b b)
            => B2A.GetList((b)b);
    }

}
