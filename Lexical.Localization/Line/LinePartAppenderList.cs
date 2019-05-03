// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Collection of <see cref="ILinePartAppender"/>s.
    /// </summary>
    public class LinePartAppenderList : ILinePartAppenderAdapter, IEnumerable<ILinePartAppender>
    {
        Dictionary<Type, ILinePartAppender0> appenders0 = new Dictionary<Type, ILinePartAppender0>();
        Dictionary<Pair<Type, Type>, ILinePartAppender1> appenders1 = new Dictionary<Pair<Type, Type>, ILinePartAppender1>(Pair<Type, Type>.EqualityComparer.Default);
        Dictionary<Triple<Type, Type, Type>, ILinePartAppender2> appenders2 = new Dictionary<Triple<Type, Type, Type>, ILinePartAppender2>(Triple<Type, Type, Type>.EqualityComparer.Default);
        Dictionary<Quad<Type, Type, Type, Type>, ILinePartAppender3> appenders3 = new Dictionary<Quad<Type, Type, Type, Type>, ILinePartAppender3>(Quad<Type, Type, Type, Type>.EqualityComparer.Default);
        bool immutable;

        /// <summary>
        /// Create line part appender collection
        /// </summary>
        public LinePartAppenderList()
        {
        }

        /// <summary>
        /// Create line part appender collection
        /// </summary>
        public LinePartAppenderList(IEnumerable<ILinePartAppender> initialAppenders)
        {
            AddRange(initialAppenders);
        }

        /// <summary>
        /// Add range of appenders
        /// </summary>
        /// <param name="appenders"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="AddPolicy.ThrowIfExists"/></exception>
        public LinePartAppenderList AddRange(IEnumerable<ILinePartAppender> appenders, AddPolicy policy = AddPolicy.ThrowIfExists)
        {
            foreach (var appender in appenders)
                Add(appender, policy);
            return this;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <returns>appender or null</returns>
        public ILinePartAppender0<Part> Cast<Part>() where Part : ILinePart
        {
            ILinePartAppender0 result;
            if (appenders0.TryGetValue(typeof(Part), out result)) return result as ILinePartAppender0<Part>;
            return default;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <returns>appender or null</returns>
        public ILinePartAppender1<Part, A0> Cast<Part, A0>() where Part : ILinePart
        {
            ILinePartAppender1 result;
            if (appenders1.TryGetValue(new Pair<Type, Type>(typeof(Part), typeof(A0)), out result)) return result as ILinePartAppender1<Part, A0>;
            return default;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <returns>appender or null</returns>
        public ILinePartAppender2<Part, A0, A1> Cast<Part, A0, A1>() where Part : ILinePart
        {
            ILinePartAppender2 result;
            if (appenders2.TryGetValue(new Triple<Type, Type, Type>(typeof(Part), typeof(A0), typeof(A1)), out result)) return result as ILinePartAppender2<Part, A0, A1>;
            return default;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <typeparam name="A2">argument 2 type</typeparam>
        /// <returns>appender or null</returns>
        public ILinePartAppender3<Part, A0, A1, A2> Cast<Part, A0, A1, A2>() where Part : ILinePart
        {
            ILinePartAppender3 result;
            if (appenders3.TryGetValue(new Quad<Type, Type, Type, Type>(typeof(Part), typeof(A0), typeof(A1), typeof(A2)), out result)) return result as ILinePartAppender3<Part, A0, A1, A2>;
            return default;
        }

        /// <summary>
        /// Policy for <see cref="Add(ILinePartAppender, AddPolicy)"/>
        /// </summary>
        public enum AddPolicy
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
        /// Add appender to the appender collection.
        /// </summary>
        /// <param name="appender">
        ///     Appender that implements one or more of the following interfaces:
        ///     <list type="bullet">
        ///         <item><see cref="IEnumerable{ILinePartAppender}"/></item>
        ///         <item><see cref="ILinePartAppender0{Part}"/></item>
        ///         <item><see cref="ILinePartAppender1{Part, A0}"/></item>
        ///         <item><see cref="ILinePartAppender2{Part, A0, A1}"/></item>
        ///         <item><see cref="ILinePartAppender3{Part, A0, A1, A2}"/></item>
        ///     </list>
        /// </param>
        /// <param name="policy"></param>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="AddPolicy.ThrowIfExists"/></exception>
        /// <returns></returns>
        public LinePartAppenderList Add(ILinePartAppender appender, AddPolicy policy = AddPolicy.ThrowIfExists)
        {
            if (immutable) throw new InvalidOperationException("Appender is in read-only state.");
            if (appender == null) throw new ArgumentNullException(nameof(appender));

            // Append components recursively
            if (appender is IEnumerable<ILinePartAppender> enumr)
            {
                foreach (var component in enumr)
                    Add(component, policy);
            }

            // Iterate each interface
            Type[] intfs = appender.GetType().GetInterfaces();

            // Add ILinePartAppender0 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILinePartAppender0<>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = paramTypes[0];
                if (itype.IsAssignableFrom(GetType()) || appenders0.ContainsKey(key))
                {
                    if (policy == AddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == AddPolicy.IgnoreIfExists) continue;
                }
                appenders0[key] = (ILinePartAppender0)appender;
            }

            // Add ILinePartAppender1 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILinePartAppender1<,>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = new Pair<Type, Type>(paramTypes[0], paramTypes[1]);
                if (itype.IsAssignableFrom(GetType()) || appenders1.ContainsKey(key))
                {
                    if (policy == AddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == AddPolicy.IgnoreIfExists) continue;
                }
                appenders1[key] = (ILinePartAppender1)appender;
            }

            // Add ILinePartAppender2 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILinePartAppender2<,,>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = new Triple<Type, Type, Type>(paramTypes[0], paramTypes[1], paramTypes[2]);
                if (itype.IsAssignableFrom(GetType()) || appenders2.ContainsKey(key))
                {
                    if (policy == AddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == AddPolicy.IgnoreIfExists) continue;
                }
                appenders2[key] = (ILinePartAppender2)appender;
            }

            // Add ILinePartAppender3 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILinePartAppender3<,,,>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = new Quad<Type, Type, Type, Type>(paramTypes[0], paramTypes[1], paramTypes[2], paramTypes[3]);
                if (itype.IsAssignableFrom(GetType()) || appenders3.ContainsKey(key))
                {
                    if (policy == AddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == AddPolicy.IgnoreIfExists) continue;
                }
                appenders3[key] = (ILinePartAppender3)appender;
            }

            return this;
        }

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="AddPolicy.ThrowIfExists"/></exception>
        public LinePartAppenderList Add<Part>(Func<ILinePartAppender, ILinePart, Part> func, AddPolicy policy = AddPolicy.ThrowIfExists) where Part : ILinePart
            => Add(new Delegate0<Part>(this, func), policy);

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="AddPolicy.ThrowIfExists"/></exception>
        public LinePartAppenderList Add<Part, A0>(Func<ILinePartAppender, ILinePart, A0, Part> func, AddPolicy policy = AddPolicy.ThrowIfExists) where Part : ILinePart
            => Add(new Delegate1<Part, A0>(this, func), policy);

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="AddPolicy.ThrowIfExists"/></exception>
        public LinePartAppenderList Add<Part, A0, A1>(Func<ILinePartAppender, ILinePart, A0, A1, Part> func, AddPolicy policy = AddPolicy.ThrowIfExists) where Part : ILinePart
            => Add(new Delegate2<Part, A0, A1>(this, func), policy);

        /// <summary>
        /// Add constructor delegate
        /// </summary>
        /// <param name="func"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="AddPolicy.ThrowIfExists"/></exception>
        public LinePartAppenderList Add<Part, A0, A1, A2>(Func<ILinePartAppender, ILinePart, A0, A1, A2, Part> func, AddPolicy policy = AddPolicy.ThrowIfExists) where Part : ILinePart
            => Add(new Delegate3<Part, A0, A1, A2>(this, func), policy);

        /// <summary>
        /// Change appender into read-only state.
        /// </summary>
        /// <returns></returns>
        public LinePartAppenderList ReadOnly()
        {
            this.immutable = true;
            return this;
        }

        /// <summary>
        /// Create clone
        /// </summary>
        /// <returns></returns>
        public virtual LinePartAppenderList Clone()
            => new LinePartAppenderList().AddRange(this, AddPolicy.IgnoreIfExists);

        /// <summary>
        /// List appenders
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ILinePartAppender> GetEnumerator()
        {
            foreach (var a in appenders0) yield return a.Value;
            foreach (var a in appenders1) yield return a.Value;
            foreach (var a in appenders2) yield return a.Value;
            foreach (var a in appenders3) yield return a.Value;
        }

        /// <summary>
        /// List appenders
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var a in appenders0) yield return a.Value;
            foreach (var a in appenders1) yield return a.Value;
            foreach (var a in appenders2) yield return a.Value;
            foreach (var a in appenders3) yield return a.Value;
        }

        /// <summary>
        /// Adapts delegate to <see cref="ILinePartAppender0{Part}"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        public class Delegate0<Part> : ILinePartAppender0<Part> where Part : ILinePart
        {
            /// <summary>
            /// Delegate
            /// </summary>
            public readonly Func<ILinePartAppender, ILinePart, Part> Func;

            /// <summary>
            /// The appender instance
            /// </summary>
            public readonly ILinePartAppender Appender;

            /// <summary>
            /// Create adapter
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="func"></param>
            public Delegate0(ILinePartAppender appender, Func<ILinePartAppender, ILinePart, Part> func)
            {
                Appender = appender ?? throw new ArgumentNullException(nameof(appender));
                Func = func ?? throw new ArgumentNullException(nameof(func));
            }

            /// <summary>
            /// Append
            /// </summary>
            /// <param name="previous"></param>
            /// <returns></returns>
            public Part Append(ILinePart previous)
                => Func(Appender, previous);
        }

        /// <summary>
        /// Adapts delegate to <see cref="ILinePartAppender1{Part, A0}"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        public class Delegate1<Part, A0> : ILinePartAppender1<Part, A0> where Part : ILinePart
        {
            /// <summary>
            /// Delegate
            /// </summary>
            public readonly Func<ILinePartAppender, ILinePart, A0, Part> Func;

            /// <summary>
            /// The appender instance
            /// </summary>
            public readonly ILinePartAppender Appender;

            /// <summary>
            /// Create adapter
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="func"></param>
            public Delegate1(ILinePartAppender appender, Func<ILinePartAppender, ILinePart, A0, Part> func)
            {
                Appender = appender ?? throw new ArgumentNullException(nameof(appender));
                Func = func ?? throw new ArgumentNullException(nameof(func));
            }

            /// <summary>
            /// Append
            /// </summary>
            /// <param name="previous"></param>
            /// <param name="a0"></param>
            /// <returns></returns>
            public Part Append(ILinePart previous, A0 a0)
                => Func(Appender, previous, a0);
        }

        /// <summary>
        /// Adapts delegate to <see cref="ILinePartAppender2{Part, A0, A1}"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        public class Delegate2<Part, A0, A1> : ILinePartAppender2<Part, A0, A1> where Part : ILinePart
        {
            /// <summary>
            /// Delegate
            /// </summary>
            public readonly Func<ILinePartAppender, ILinePart, A0, A1, Part> Func;

            /// <summary>
            /// The appender instance
            /// </summary>
            public readonly ILinePartAppender Appender;

            /// <summary>
            /// Create adapter
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="func"></param>
            public Delegate2(ILinePartAppender appender, Func<ILinePartAppender, ILinePart, A0, A1, Part> func)
            {
                Appender = appender ?? throw new ArgumentNullException(nameof(appender));
                Func = func ?? throw new ArgumentNullException(nameof(func));
            }

            /// <summary>
            /// Append
            /// </summary>
            /// <param name="previous"></param>
            /// <param name="a0"></param>
            /// <param name="a1"></param>
            /// <returns></returns>
            public Part Append(ILinePart previous, A0 a0, A1 a1)
                => Func(Appender, previous, a0, a1);
        }

        /// <summary>
        /// Adapts delegate to <see cref="ILinePartAppender3{Part, A0, A1, A2}"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        public class Delegate3<Part, A0, A1, A2> : ILinePartAppender3<Part, A0, A1, A2> where Part : ILinePart
        {
            /// <summary>
            /// Delegate
            /// </summary>
            public readonly Func<ILinePartAppender, ILinePart, A0, A1, A2, Part> Func;

            /// <summary>
            /// The appender instance
            /// </summary>
            public readonly ILinePartAppender Appender;

            /// <summary>
            /// Create adapter
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="func"></param>
            public Delegate3(ILinePartAppender appender, Func<ILinePartAppender, ILinePart, A0, A1, A2, Part> func)
            {
                Appender = appender ?? throw new ArgumentNullException(nameof(appender));
                Func = func ?? throw new ArgumentNullException(nameof(func));
            }

            /// <summary>
            /// Append
            /// </summary>
            /// <param name="previous"></param>
            /// <param name="a0"></param>
            /// <param name="a1"></param>
            /// <param name="a2"></param>
            /// <returns></returns>
            public Part Append(ILinePart previous, A0 a0, A1 a1, A2 a2)
                => Func(Appender, previous, a0, a1, a2);
        }
    }

    /// <summary></summary>
    public static partial class LinePartExtensions
    {
        /// <summary>
        /// Create a clone of <paramref name="previous"/>'s appender, and add new components to it. 
        /// </summary>
        /// <param name="previous">previous part</param>
        /// <param name="appender">appender to add</param>
        /// <param name="policy">add policy</param>
        /// <returns>part with another appender</returns>
        public static ILinePart AddAppender(this ILinePart previous, ILinePartAppender appender, LinePartAppenderList.AddPolicy policy = LinePartAppenderList.AddPolicy.OverwriteIfExists)
        {
            ILinePartAppender previousAppender = previous.GetAppender();
            ILinePartAppender newAppender = previousAppender == null ? appender : new LinePartAppenderList().Add(previousAppender, policy).Add(appender, policy);
            return newAppender.Append<ILinePart>(previous);
        }
    }

}
