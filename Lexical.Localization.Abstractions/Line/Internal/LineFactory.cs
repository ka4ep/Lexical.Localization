// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts delegate to <see cref="ILineFactory{Intf}"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    internal class Delegate0<Intf> : ILineFactory<Intf> where Intf : ILine
    {
        /// <summary>
        /// Delegate
        /// </summary>
        public readonly Func<ILineFactory, ILine, Intf> Func;

        /// <summary>
        /// Create adapter
        /// </summary>
        /// <param name="func"></param>
        public Delegate0(Func<ILineFactory, ILine, Intf> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory appender, ILine previous, out Intf result)
        {
            var _result = Func(appender, previous);
            result = _result;
            return _result != default;
        }
    }

    /// <summary>
    /// Adapts delegate to <see cref="ILineFactory{Intf, A0}"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    internal class Delegate1<Intf, A0> : ILineFactory<Intf, A0> where Intf : ILine
    {
        /// <summary>
        /// Delegate
        /// </summary>
        public readonly Func<ILineFactory, ILine, A0, Intf> Func;

        /// <summary>
        /// Create adapter
        /// </summary>
        /// <param name="func"></param>
        public Delegate1(Func<ILineFactory, ILine, A0, Intf> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory appender, ILine previous, A0 a0, out Intf result)
        {
            var _result = Func(appender, previous, a0);
            result = _result;
            return _result != default;
        }
    }

    /// <summary>
    /// Adapts delegate to <see cref="ILineFactory{Intf, A0, A1}"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    internal class Delegate2<Intf, A0, A1> : ILineFactory<Intf, A0, A1> where Intf : ILine
    {
        /// <summary>
        /// Delegate
        /// </summary>
        public readonly Func<ILineFactory, ILine, A0, A1, Intf> Func;

        /// <summary>
        /// Create adapter
        /// </summary>
        /// <param name="func"></param>
        public Delegate2(Func<ILineFactory, ILine, A0, A1, Intf> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory appender, ILine previous, A0 a0, A1 a1, out Intf result)
        {
            var _result = Func(appender, previous, a0, a1);
            result = _result;
            return _result != default;
        }
    }

    /// <summary>
    /// Adapts delegate to <see cref="ILineFactory{Intf, A0, A1, A2}"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    /// <typeparam name="A2"></typeparam>
    internal class Delegate3<Intf, A0, A1, A2> : ILineFactory<Intf, A0, A1, A2> where Intf : ILine
    {
        /// <summary>
        /// Delegate
        /// </summary>
        public readonly Func<ILineFactory, ILine, A0, A1, A2, Intf> Func;

        /// <summary>
        /// Create adapter
        /// </summary>
        /// <param name="func"></param>
        public Delegate3(Func<ILineFactory, ILine, A0, A1, A2, Intf> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory appender, ILine previous, A0 a0, A1 a1, A2 a2, out Intf result)
        {
            var _result = Func(appender, previous, a0, a1, a2);
            result = _result;
            return _result != default;
        }
    }

}
