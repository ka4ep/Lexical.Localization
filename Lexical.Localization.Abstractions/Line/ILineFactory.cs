// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lexical.Localization
{
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
    /// Zero argument count line factory.
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
    /// One argument count line part factory.
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
    /// Two argument count line part factory.
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
    /// Two argument count line part factory.
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

            // TODO Fix reflection //

            try
            {
                Type[] intfs = arguments.GetType().GetInterfaces();
                foreach (Type intf in intfs)
                {
                    Type intfGeneric = intf.GetGenericTypeDefinition();
                    if (intfGeneric == null) continue;
                    Type[] typeArgs = intf.GetGenericArguments();

                    // ILineArguments<>
                    if (intfGeneric == typeof(ILineArguments<>))
                    {
                        Type factoryIntfType = typeof(ILineFactory<>).MakeGenericType(typeArgs);
                        if (factoryIntfType.IsAssignableFrom(factory.GetType()))
                        {
                            object[] parameters = new object[] { factory, previous, null };
                            object boolResult = factoryIntfType.GetMethod("TryCreate").Invoke(factory, parameters);
                            if ((bool)boolResult)
                            {
                                line = parameters[parameters.Length - 1] as ILine;
                                return true;
                            }
                        }

                        if (factory is ILineFactoryCastable castable)
                        {
                            MethodInfo mi = typeof(ILineFactoryCastable).GetMethods().Where(m => m.Name == "Cast" && m.GetGenericArguments() != null && m.GetGenericArguments().Length == 1).FirstOrDefault();
                            mi = mi.MakeGenericMethod(typeArgs);
                            object lineResult = mi.Invoke(factory, new object[0]);
                            if (lineResult is ILine _result)
                            {
                                line = _result;
                                return true;
                            }
                        }
                    }

                    // ILineArguments<,>
                    if (intfGeneric == typeof(ILineArguments<,>))
                    {
                        Type factoryIntfType = typeof(ILineFactory<,>).MakeGenericType(typeArgs);
                        if (factoryIntfType.IsAssignableFrom(factory.GetType()))
                        {
                            object[] parameters = new object[] { factory, previous, null };
                            object boolResult = factoryIntfType.GetMethod("TryCreate").Invoke(factory, parameters);
                            if ((bool)boolResult)
                            {
                                line = parameters[parameters.Length - 1] as ILine;
                                return true;
                            }
                        }

                        if (factory is ILineFactoryCastable castable)
                        {
                            MethodInfo mi = typeof(ILineFactoryCastable).GetMethods().Where(m => m.Name == "Cast" && m.GetGenericArguments() != null && m.GetGenericArguments().Length == 2).FirstOrDefault();
                            mi = mi.MakeGenericMethod(typeArgs);
                            object lineResult = mi.Invoke(factory, new object[0]);
                            if (lineResult is ILine _result)
                            {
                                line = _result;
                                return true;
                            }
                        }
                    }

                    // ILineArguments<,,>
                    if (intfGeneric == typeof(ILineArguments<,,>))
                    {
                        Type factoryIntfType = typeof(ILineFactory<,,>).MakeGenericType(typeArgs);
                        if (factoryIntfType.IsAssignableFrom(factory.GetType()))
                        {
                            object[] parameters = new object[] { factory, previous, null };
                            object boolResult = factoryIntfType.GetMethod("TryCreate").Invoke(factory, parameters);
                            if ((bool)boolResult)
                            {
                                line = parameters[parameters.Length - 1] as ILine;
                                return true;
                            }
                        }

                        if (factory is ILineFactoryCastable castable)
                        {
                            MethodInfo mi = typeof(ILineFactoryCastable).GetMethods().Where(m => m.Name == "Cast" && m.GetGenericArguments() != null && m.GetGenericArguments().Length == 3).FirstOrDefault();
                            mi = mi.MakeGenericMethod(typeArgs);
                            object lineResult = mi.Invoke(factory, new object[0]);
                            if (lineResult is ILine _result)
                            {
                                line = _result;
                                return true;
                            }
                        }
                    }

                    // ILineArguments<,,,>
                    if (intfGeneric == typeof(ILineArguments<,,,>))
                    {
                        Type factoryIntfType = typeof(ILineFactory<,,,>).MakeGenericType(typeArgs);
                        if (factoryIntfType.IsAssignableFrom(factory.GetType()))
                        {
                            object[] parameters = new object[] { factory, previous, null };
                            object boolResult = factoryIntfType.GetMethod("TryCreate").Invoke(factory, parameters);
                            if ((bool)boolResult)
                            {
                                line = parameters[parameters.Length - 1] as ILine;
                                return true;
                            }
                        }

                        if (factory is ILineFactoryCastable castable)
                        {
                            MethodInfo mi = typeof(ILineFactoryCastable).GetMethods().Where(m => m.Name == "Cast" && m.GetGenericArguments() != null && m.GetGenericArguments().Length == 4).FirstOrDefault();
                            mi = mi.MakeGenericMethod(typeArgs);
                            object lineResult = mi.Invoke(factory, new object[0]);
                            if (lineResult is ILine _result)
                            {
                                line = _result;
                                return true;
                            }
                        }
                    }

                }

                line = default;
                return false;
            } catch (Exception e)
            {
                throw new LineException(previous, e.Message, e);
            }
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
            ILineFactory<Intf> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf>()) != null && casted.TryCreate(factory, previous, out result)) return result;
            if (factory is ILineFactory<Intf> _casted && _casted.TryCreate(factory, previous, out result)) return result;
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
            ILineFactory<Intf> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf>()) != null && casted.TryCreate(factory, previous, out result)) { line = result; return true; }
            if (factory is ILineFactory<Intf> _casted && _casted.TryCreate(factory, previous, out result)) { line = result; return true; }
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
            ILineFactory<Intf, A0> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0>()) != null && casted.TryCreate(factory, previous, a0, out result)) return result;
            if (factory is ILineFactory<Intf, A0> _casted && _casted.TryCreate(factory, previous, a0, out result)) return result;
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
            ILineFactory<Intf, A0> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0>()) != null && casted.TryCreate(factory, previous, a0, out result)) { line = result; return true; }
            if (factory is ILineFactory<Intf, A0> _casted && _casted.TryCreate(factory, previous, a0, out result)) { line = result; return true; }
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
            ILineFactory<Intf, A0, A1> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0, A1>()) != null && casted.TryCreate(factory, previous, a0, a1, out result)) { line = result; return true; }
            if (factory is ILineFactory<Intf, A0, A1> _casted && _casted.TryCreate(factory, previous, a0, a1, out result)) { line = result; return true; }
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
            ILineFactory<Intf, A0, A1, A2> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0, A1, A2>()) != null && casted.TryCreate(factory, previous, a0, a1, a2, out result)) return result;
            if (factory is ILineFactory<Intf, A0, A1, A2> _casted && _casted.TryCreate(factory, previous, a0, a1, a2, out result)) return result;
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
            ILineFactory<Intf, A0, A1, A2> casted;
            if (factory is ILineFactoryCastable castable && (casted = castable.Cast<Intf, A0, A1, A2>()) != null && casted.TryCreate(factory, previous, a0, a1, a2, out result)) { line = result; return true; }
            if (factory is ILineFactory<Intf, A0, A1, A2> _casted && _casted.TryCreate(factory, previous, a0, a1, a2, out result)) { line = result; return true; }
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
    }

}
