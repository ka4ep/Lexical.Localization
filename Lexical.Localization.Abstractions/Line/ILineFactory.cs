// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Line.Internal;
using Lexical.Localization.Resolver;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    #region Interface
    /// <summary>
    /// Line factory can append new parts to <see cref="ILine"/>s. 
    /// Appended parts are typically immutable, and form a linked list or a trie.
    /// </summary>
    public interface ILineFactory
    {
    }

    /// <summary>
    /// Policy for <see cref="ILineFactoryCollection"/>.
    /// </summary>
    public enum LineFactoryAddPolicy
    {
        /// <summary>
        /// If appender with same key exists, throw <see cref="InvalidOperationException"/>.
        /// </summary>
        ThrowIfExists,

        /// <summary>
        /// If appender with same key exists, overwrite the previous appender.
        /// </summary>
        OverwriteIfExists,

        /// <summary>
        /// If appender with same key exists, ignore the new appender. Don't throw.
        /// </summary>
        IgnoreIfExists
    }

    /// <summary>
    /// Collection of line factories
    /// </summary>
    public interface ILineFactoryCollection
    {
        /// <summary>
        /// Add line factory to collection.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="policy"></param>
        ILineFactoryCollection Add(ILineFactory lineFactory, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists);
    }

    /// <summary>
    /// Enumerable of line factory's component factories.
    /// </summary>
    public interface ILineFactoryEnumerable : IEnumerable<ILineFactory>
    {
    }

    /// <summary>
    /// Zero argument line factory.
    /// </summary>
    /// <typeparam name="Intf">the interface type of the line part that can be appended </typeparam>
    public interface ILineFactory<Intf> : ILineFactory where Intf : ILine
    {
        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="line">create output</param>
        /// <returns>true if part was created</returns>
        /// <exception cref="LineException">If append failed due to unexpected reasons</exception>
        bool TryCreate(ILineFactory factory, ILine previous, out Intf line);
    }

    /// <summary>
    /// One argument line part factory.
    /// </summary>
    /// <typeparam name="Intf">the part type</typeparam>
    /// <typeparam name="A0"></typeparam>
    public interface ILineFactory<Intf, A0> : ILineFactory where Intf : ILine
    {
        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="a0"></param>
        /// <param name="line">create output</param>
        /// <returns>true if part was created</returns>
        /// <exception cref="LineException">If append failed due to unexpected reason</exception>
        bool TryCreate(ILineFactory factory, ILine previous, A0 a0, out Intf line);
    }

    /// <summary>
    /// Two argument line part factory.
    /// </summary>
    /// <typeparam name="Intf">the part type</typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    public interface ILineFactory<Intf, A0, A1> : ILineFactory where Intf : ILine
    {
        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="line">create output</param>
        /// <returns>true if part was created</returns>
        /// <exception cref="LineException">If append failed due to unexpected reason</exception>
        bool TryCreate(ILineFactory factory, ILine previous, A0 a0, A1 a1, out Intf line);
    }

    /// <summary>
    /// Three argument line part factory.
    /// </summary>
    /// <typeparam name="Intf">the part type</typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    /// <typeparam name="A2"></typeparam>
    public interface ILineFactory<Intf, A0, A1, A2> : ILineFactory where Intf : ILine
    {
        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="line">create output</param>
        /// <returns>true if part was created</returns>
        /// <exception cref="LineException">If append failed due to unexpected reason</exception>
        bool TryCreate(ILineFactory factory, ILine previous, A0 a0, A1 a1, A2 a2, out Intf line);
    }

    /// <summary>
    /// Adapts to different <see cref="ILineFactory"/> types.
    /// </summary>
    public interface ILineFactoryCastable : ILineFactory
    {
        /// <summary>
        /// Get factory for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <returns>factory or null</returns>
        ILineFactory<Intf> Cast<Intf>() where Intf : ILine;

        /// <summary>
        /// Get factory for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <returns>factory or null</returns>
        ILineFactory<Intf, A0> Cast<Intf, A0>() where Intf : ILine;

        /// <summary>
        /// Get factory for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <returns>factory or null</returns>
        ILineFactory<Intf, A0, A1> Cast<Intf, A0, A1>() where Intf : ILine;

        /// <summary>
        /// Get factory for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <typeparam name="A2">argument 2 type</typeparam>
        /// <returns>factory or null</returns>
        ILineFactory<Intf, A0, A1, A2> Cast<Intf, A0, A1, A2>() where Intf : ILine;
    }

    /// <summary>
    /// Appender can append new <see cref="ILine"/>s.
    /// </summary>
    public interface ILineFactoryByArgument : ILineFactory
    {
        /// <summary>
        /// Create line (part) with <paramref name="arguments"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="arguments">Line construction arguments</param>
        /// <param name="line">create output</param>
        /// <returns>true if part was created</returns>
        /// <exception cref="LineException">If append failed due to unexpected reason</exception>
        bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line);
    }

    /// <summary>
    /// Line factory that has an assigned resolver.
    /// </summary>
    public interface ILineFactoryResolver : ILineFactory
    {
        /// <summary>
        /// (optional) Type and parameter resolver
        /// </summary>
        IResolver Resolver { get; set; }
    }

    /// <summary>
    /// Line factory that has parameter infos assigned
    /// </summary>
    public interface ILineFactoryParameterInfos : ILineFactory
    {
        /// <summary>
        /// (optional) Associated parameter infos.
        /// </summary>
        IParameterInfos ParameterInfos { get; set; }
    }
    #endregion Interface

    /// <summary></summary>
    public static partial class ILineFactoryExtensions
    {
        /// <summary>
        /// Create line (part) with <paramref name="arguments"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="arguments"></param>
        /// <returns>appended part</returns>
        /// <exception cref="LineException">If append failed due to unexpected reason</exception>
        public static ILine Create(this ILineFactory factory, ILine previous, ILineArguments arguments)
        {
            ILine result = null;
            if (factory.TryCreate(previous, arguments, out result)) return result;
            ILineFactoryByArgument argumentAdapter;
            if (LineFactoryByArgumentAdapter.Default.TryGet(arguments.GetType(), out argumentAdapter) && argumentAdapter.TryCreate(factory, previous, arguments, out result)) return result;
            throw new LineException(arguments, "Could not be appended");
        }

        /// <summary>
        /// Create line (part) with <paramref name="arguments"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="arguments"></param>
        /// <param name="line"></param>
        /// <returns>try if create succeeded</returns>
        /// <exception cref="LineException">If append failed due to unexpected reason</exception>
        public static bool TryCreate(this ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            if (factory is ILineFactoryByArgument argFactory && argFactory.TryCreate(factory, previous, arguments, out line)) return true;
            ILineFactoryByArgument argumentAdapter;
            if (LineFactoryByArgumentAdapter.Default.TryGet(arguments.GetType(), out argumentAdapter) && argumentAdapter.TryCreate(factory, previous, arguments, out line)) return true;
            line = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <typeparam name="Intf"></typeparam>
        /// <returns>Line</returns>
        /// <exception cref="LineException">If part could not append <typeparamref name="Intf"/></exception>
        public static Intf Create<Intf>(this ILineFactory factory, ILine previous) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            if (factory is ILineFactory<Intf> _casted && _casted.TryCreate(factory, previous, out result)) return result;
            ILineFactory<Intf> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf>()) != null && casted.TryCreate(factory, previous, out result)) return result;
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf>(), out result_) && result_ is Intf result__) return result__;
            throw new LineException(previous, $"Appender doesn't have capability to append {nameof(Intf)}.");
        }

        /// <summary>
        /// Try append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <param name="line">result</param>
        /// <typeparam name="Intf"></typeparam>
        /// <returns>true if succeeded</returns>
        public static bool TryCreate<Intf>(this ILineFactory factory, ILine previous, out Intf line) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            if (factory is ILineFactory<Intf> _casted && _casted.TryCreate(factory, previous, out result)) { line = result; return true; }
            ILineFactory<Intf> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf>()) != null && casted.TryCreate(factory, previous, out result)) { line = result; return true; }
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf>(), out result_) && result_ is Intf result__) { line = result__; return true; }
            line = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="a0"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <returns>Line</returns>
        /// <exception cref="LineException">If part could not append <typeparamref name="Intf"/></exception>
        public static Intf Create<Intf, A0>(this ILineFactory factory, ILine previous, A0 a0) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            if (factory is ILineFactory<Intf, A0> _casted && _casted.TryCreate(factory, previous, a0, out result)) return result;
            ILineFactory<Intf, A0> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0>()) != null && casted.TryCreate(factory, previous, a0, out result)) return result;
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf, A0>(a0), out result_) && result_ is Intf result__) return result__;
            throw new LineException(previous, $"Appender doesn't have capability to append {nameof(Intf)}.");
        }

        /// <summary>
        /// Try append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <param name="a0"></param>
        /// <param name="line">result</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <returns>true if succeeded</returns>
        public static bool TryCreate<Intf, A0>(this ILineFactory factory, ILine previous, A0 a0, out Intf line) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            if (factory is ILineFactory<Intf, A0> _casted && _casted.TryCreate(factory, previous, a0, out result)) { line = result; return true; }
            ILineFactory<Intf, A0> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0>()) != null && casted.TryCreate(factory, previous, a0, out result)) { line = result; return true; }
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf, A0>(a0), out result_) && result_ is Intf result__) { line = result__; return true; }
            line = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <returns>Line</returns>
        /// <exception cref="LineException">If part could not append <typeparamref name="Intf"/></exception>
        public static Intf Create<Intf, A0, A1>(this ILineFactory factory, ILine previous, A0 a0, A1 a1) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            ILineFactory<Intf, A0, A1> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0, A1>()) != null && casted.TryCreate(factory, previous, a0, a1, out result)) return result;
            if (factory is ILineFactory<Intf, A0, A1> _casted && _casted.TryCreate(factory, previous, a0, a1, out result)) return result;
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf, A0, A1>(a0, a1), out result_) && result_ is Intf result__) return result__;
            throw new LineException(previous, $"Appender doesn't have capability to append {nameof(Intf)}.");
        }

        /// <summary>
        /// Try append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="line">result</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <returns>true if succeeded</returns>
        public static bool TryCreate<Intf, A0, A1>(this ILineFactory factory, ILine previous, A0 a0, A1 a1, out Intf line) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            if (factory is ILineFactory<Intf, A0, A1> _casted && _casted.TryCreate(factory, previous, a0, a1, out result)) { line = result; return true; }
            ILineFactory<Intf, A0, A1> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0, A1>()) != null && casted.TryCreate(factory, previous, a0, a1, out result)) { line = result; return true; }
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf, A0, A1>(a0, a1), out result_) && result_ is Intf result__) { line = result__; return true; }
            line = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <returns>Line</returns>
        /// <exception cref="LineException">If part could not append <typeparamref name="Intf"/></exception>
        public static Intf Create<Intf, A0, A1, A2>(this ILineFactory factory, ILine previous, A0 a0, A1 a1, A2 a2) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            if (factory is ILineFactory<Intf, A0, A1, A2> _casted && _casted.TryCreate(factory, previous, a0, a1, a2, out result)) return result;
            ILineFactory<Intf, A0, A1, A2> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0, A1, A2>()) != null && casted.TryCreate(factory, previous, a0, a1, a2, out result)) return result;
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf, A0, A1, A2>(a0, a1, a2), out result_) && result_ is Intf result__) return result__;
            throw new LineException(previous, $"Appender doesn't have capability to append {nameof(Intf)}.");
        }

        /// <summary>
        /// Try append new <see cref="ILine"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous">(optional) previous part</param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="line">result</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <returns>true if succeeded</returns>
        public static bool TryCreate<Intf, A0, A1, A2>(this ILineFactory factory, ILine previous, A0 a0, A1 a1, A2 a2, out Intf line) where Intf : ILine
        {
            if (factory == null) throw new LineException(previous, "Appender is not found.");
            Intf result = default;
            if (factory is ILineFactory<Intf, A0, A1, A2> _casted && _casted.TryCreate(factory, previous, a0, a1, a2, out result)) { line = result; return true; }
            ILineFactory<Intf, A0, A1, A2> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0, A1, A2>()) != null && casted.TryCreate(factory, previous, a0, a1, a2, out result)) { line = result; return true; }
            ILine result_ = null;
            if (factory is ILineFactoryByArgument __casted && __casted.TryCreate(factory, previous, LineArguments.Create<Intf, A0, A1, A2>(a0, a1, a2), out result_) && result_ is Intf result__) { line = result__; return true; }
            line = default;
            return false;
        }

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="LineFactoryAddPolicy.ThrowIfExists"/></exception>
        public static ILineFactoryCollection Add<Line>(this ILineFactoryCollection collection, Func<ILineFactory, ILine, Line> func, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists) where Line : ILine
            => collection.Add(new Delegate0<Line>(func), policy);

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="LineFactoryAddPolicy.ThrowIfExists"/></exception>
        public static ILineFactoryCollection Add<Line, A0>(this ILineFactoryCollection collection, Func<ILineFactory, ILine, A0, Line> func, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists) where Line : ILine
            => collection.Add(new Delegate1<Line, A0>(func), policy);

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="LineFactoryAddPolicy.ThrowIfExists"/></exception>
        public static ILineFactoryCollection Add<Line, A0, A1>(this ILineFactoryCollection collection, Func<ILineFactory, ILine, A0, A1, Line> func, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists) where Line : ILine
            => collection.Add(new Delegate2<Line, A0, A1>(func), policy);

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="LineFactoryAddPolicy.ThrowIfExists"/></exception>
        public static ILineFactoryCollection Add<Line, A0, A1, A2>(this ILineFactoryCollection collection, Func<ILineFactory, ILine, A0, A1, A2, Line> func, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists) where Line : ILine
            => collection.Add(new Delegate3<Line, A0, A1, A2>(func), policy);

        /// <summary>
        /// Concatenate <paramref name="right"/> to <paramref name="left"/>.
        /// 
        /// This method can also be used for cloning if <paramref name="left"/> is null.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="left">(optional)</param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="LineException">on append error</exception>
        public static ILine Concat(this ILineFactory factory, ILine left, ILine right)
        {
            ILine result = left;
            StructList16<ILine> args = new StructList16<ILine>();
            for (ILine l = right; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineArguments || l is ILineArgumentsEnumerable) args.Add(l);
            }

            for (int i = args.Count - 1; i >= 0; i--)
            {
                ILine l = args[i];
                if (l is ILineArgumentsEnumerable enumr)
                    foreach (ILineArguments args_ in enumr)
                        result = factory.Create(result, args_);

                if (l is ILineArguments arg)
                    result = factory.Create(result, arg);
            }
            return result;
        }

        /// <summary>
        /// Clone <paramref name="line"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="LineException">on append error</exception>
        public static ILine Clone(this ILineFactory factory, ILine line)
        {
            ILine result = null;
            StructList16<ILine> args = new StructList16<ILine>();
            for (ILine l = line; l != null; l = l.GetPreviousPart()) if (l is ILineArguments || l is ILineArgumentsEnumerable) args.Add(l);
            for (int i = args.Count - 1; i >= 0; i--)
            {
                ILine l = args[i];
                if (l is ILineArgumentsEnumerable enumr)
                    foreach (ILineArguments args_ in enumr)
                        result = factory.Create(result, args_);

                if (l is ILineArguments arg)
                    result = factory.Create(result, arg);
            }
            return result;
        }


        /// <summary>
        /// Try to clone <paramref name="line"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="line"></param>
        /// <param name="clone"></param>
        /// <returns></returns>
        public static bool TryClone(this ILineFactory factory, ILine line, out ILine clone)
        {
            if (factory == null) { clone = default; return false; }
            ILine result = null;
            StructList16<ILine> args = new StructList16<ILine>();
            for (ILine l = line; l != null; l = l.GetPreviousPart()) if (l is ILineArguments || l is ILineArgumentsEnumerable) args.Add(l);
            for (int i = args.Count - 1; i >= 0; i--)
            {
                ILine l = args[i];
                if (l is ILineArgumentsEnumerable enumr)
                    foreach (ILineArguments args_ in enumr)
                        if (!factory.TryCreate(result, args_, out result)) { clone = default; return false; }

                if (l is ILineArguments arg)
                    if (!factory.TryCreate(result, args, out result)) { clone = default; return false; }
            }
            clone = result;
            return true;
        }

        /// <summary>
        /// Append <paramref name="right"/> to <paramref name="left"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="left">part to append to</param>
        /// <param name="right"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="LineException">on append error</exception>
        public static bool TryConcat(this ILineFactory factory, ILine left, ILine right, out ILine result)
        {
            ILine _result = left;
            StructList16<ILine> _args = new StructList16<ILine>();
            for (ILine l = right; l != null; l = l.GetPreviousPart()) if (l is ILineArguments || l is ILineArgumentsEnumerable) _args.Add(l);
            for (int i = _args.Count - 1; i >= 0; i--)
            {
                ILine l = _args[i];
                if (l is ILineArgumentsEnumerable enumr)
                    foreach (ILineArguments args_ in enumr)
                        if (!factory.TryCreate(args_, out _result)) { result = null; return false; }


                if (l is ILineArguments args)
                    if (!factory.TryCreate(args, out _result)) { result = null; return false; }
            }
            result = _result;
            return false;
        }

        /// <summary>
        /// Concatenate <paramref name="right"/> to <paramref name="left"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="left">part to append to</param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="LineException">on append error</exception>
        public static ILine ConcatIfNew(this ILineFactory factory, ILine left, ILine right)
        {
            ILine result = left;
            KeyValuePair<string, string>[] parameters = null;

            StructList16<ILine> _args = new StructList16<ILine>();
            for (ILine l = right; l != null; l = l.GetPreviousPart()) if (l is ILineArguments || l is ILineArgumentsEnumerable) _args.Add(l);
            for (int i = _args.Count - 1; i >= 0; i--)
            {
                ILine l = _args[i];
                if (l is ILineArgumentsEnumerable enumr)
                    foreach (ILineArguments args_ in enumr)
                    {
                        if (args_ is ILineArguments<ILineParameter, string, string> paramArgs && ContainsParameter(paramArgs.Argument0, paramArgs.Argument1)) continue;
                        if (args_ is ILineArguments<ILineNonCanonicalKey, string, string> paramArgs_ && ContainsParameter(paramArgs_.Argument0, paramArgs_.Argument1)) continue;
                        if (args_ is ILineArguments<ILineCanonicalKey, string, string> paramArgs__ && ContainsParameter(paramArgs__.Argument0, paramArgs__.Argument1)) continue;
                        result = factory.Create(result, args_);
                    }

                if (l is ILineArguments args)
                {
                    if (args is ILineArguments<ILineParameter, string, string> paramArgs && ContainsParameter(paramArgs.Argument0, paramArgs.Argument1)) continue;
                    if (args is ILineArguments<ILineNonCanonicalKey, string, string> paramArgs_ && ContainsParameter(paramArgs_.Argument0, paramArgs_.Argument1)) continue;
                    if (args is ILineArguments<ILineCanonicalKey, string, string> paramArgs__ && ContainsParameter(paramArgs__.Argument0, paramArgs__.Argument1)) continue;
                    result = factory.Create(result, args);
                }
            }
            return result;

            bool ContainsParameter(string parameterName, string parameterValue)
            {
                if (parameters == null) parameters = left.GetParameterAsKeyValues();
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    if (p.Key == parameterName && p.Value == parameterValue) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Get associated resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static bool TryGetResolver(this ILineFactory lineFactory, out IResolver resolver)
        {
            if (lineFactory is ILineFactoryResolver lineFactory1 && lineFactory1.Resolver != null) { resolver = lineFactory1.Resolver; return true; }
            resolver = default;
            return false;
        }

        /// <summary>
        /// Get parameter infos
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="parameterInfos"></param>
        /// <returns></returns>
        public static bool TryGetParameterInfos(this ILineFactory lineFactory, out IParameterInfos parameterInfos)
        {
            if (lineFactory is ILineFactoryParameterInfos lineFactory1 && lineFactory1.ParameterInfos != null) { parameterInfos = lineFactory1.ParameterInfos; return true; }
            parameterInfos = default;
            return false;
        }

    }

}
