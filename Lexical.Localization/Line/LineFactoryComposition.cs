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
    /// Collection of <see cref="ILineFactory"/>s.
    /// </summary>
    public class LineFactoryComposition : ILineFactoryCastable, ILineFactoryByArgument, ILineFactoryEnumerable, ILineFactoryCollection
    {
        List<ILineFactoryByArgument> argFactories = new List<ILineFactoryByArgument>();
        List<ILineFactoryCastable> castables = new List<ILineFactoryCastable>();
        Dictionary<Type, ILineFactory> factories0 = new Dictionary<Type, ILineFactory>();
        Dictionary<Pair<Type, Type>, ILineFactory> factories1 = new Dictionary<Pair<Type, Type>, ILineFactory>(Pair<Type, Type>.EqualityComparer.Default);
        Dictionary<Triple<Type, Type, Type>, ILineFactory> factories2 = new Dictionary<Triple<Type, Type, Type>, ILineFactory>(Triple<Type, Type, Type>.EqualityComparer.Default);
        Dictionary<Quad<Type, Type, Type, Type>, ILineFactory> factories3 = new Dictionary<Quad<Type, Type, Type, Type>, ILineFactory>(Quad<Type, Type, Type, Type>.EqualityComparer.Default);
        bool immutable;

        /// <summary>
        /// Create line part appender collection
        /// </summary>
        public LineFactoryComposition()
        {
            PostConstruction();
            PostConstruction2();
        }

        /// <summary>
        /// Create line part appender collection
        /// </summary>
        public LineFactoryComposition(IEnumerable<ILineFactory> initialAppenders)
        {
            AddRange(initialAppenders);
            PostConstruction();
            PostConstruction2();
        }

        /// <summary>
        /// Override this to add post construction actions.
        /// </summary>
        protected virtual void PostConstruction() { }

        /// <summary>
        /// Override this to add post construction actions.
        /// </summary>
        protected virtual void PostConstruction2() { }

        /// <summary>
        /// Add range of appenders
        /// </summary>
        /// <param name="appenders"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="LineFactoryAddPolicy.ThrowIfExists"/></exception>
        public LineFactoryComposition AddRange(IEnumerable<ILineFactory> appenders, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists)
        {
            foreach (var appender in appenders)
                Add(appender, policy);
            return this;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <returns>appender or null</returns>
        public virtual ILineFactory<Intf> Cast<Intf>() where Intf : ILine
        {
            ILineFactory result;
            if (factories0.TryGetValue(typeof(Intf), out result)) return result as ILineFactory<Intf>;
            foreach (var adapter in castables) { var appender = adapter.Cast<Intf>(); if (appender != null) return appender; }
            if (this is ILineFactory<Intf> casted) return casted;
            return default;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <returns>appender or null</returns>
        public virtual ILineFactory<Intf, A0> Cast<Intf, A0>() where Intf : ILine
        {
            ILineFactory result;
            if (factories1.TryGetValue(new Pair<Type, Type>(typeof(Intf), typeof(A0)), out result)) return result as ILineFactory<Intf, A0>;
            foreach (var adapter in castables) { var appender = adapter.Cast<Intf, A0>(); if (appender != null) return appender; }
            if (this is ILineFactory<Intf, A0> casted) return casted;
            return default;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <returns>appender or null</returns>
        public virtual ILineFactory<Intf, A0, A1> Cast<Intf, A0, A1>() where Intf : ILine
        {
            ILineFactory result;
            if (factories2.TryGetValue(new Triple<Type, Type, Type>(typeof(Intf), typeof(A0), typeof(A1)), out result)) return result as ILineFactory<Intf, A0, A1>;
            foreach (var adapter in castables) { var appender = adapter.Cast<Intf, A0, A1>(); if (appender != null) return appender; }
            if (this is ILineFactory<Intf, A0, A1> casted) return casted;
            return default;
        }

        /// <summary>
        /// Get appender for part type <typeparamref name="Intf"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <typeparam name="A2">argument 2 type</typeparam>
        /// <returns>appender or null</returns>
        public virtual ILineFactory<Intf, A0, A1, A2> Cast<Intf, A0, A1, A2>() where Intf : ILine
        {
            ILineFactory result;
            if (factories3.TryGetValue(new Quad<Type, Type, Type, Type>(typeof(Intf), typeof(A0), typeof(A1), typeof(A2)), out result)) return result as ILineFactory<Intf, A0, A1, A2>;
            foreach (var adapter in castables) { var appender = adapter.Cast<Intf, A0, A1, A2>(); if (appender != null) return appender; }
            if (this is ILineFactory<Intf, A0, A1, A2> casted) return casted;
            return default;
        }

        /// <summary>
        /// Add appender to the appender collection.
        /// </summary>
        /// <param name="appender">
        ///     Appender that implements one or more of the following interfaces:
        ///     <list type="bullet">
        ///         <item><see cref="IEnumerable{ILineFactory}"/></item>
        ///         <item><see cref="ILineFactory{Line}"/></item>
        ///         <item><see cref="ILineFactory{Line, A0}"/></item>
        ///         <item><see cref="ILineFactory{Line, A0, A1}"/></item>
        ///         <item><see cref="ILineFactory{Line, A0, A1, A2}"/></item>
        ///     </list>
        /// </param>
        /// <param name="policy"></param>
        /// <exception cref="InvalidOperationException">if key already exists and <paramref name="policy"/> is <see cref="LineFactoryAddPolicy.ThrowIfExists"/></exception>
        /// <returns></returns>
        public ILineFactoryCollection Add(ILineFactory appender, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists)
        {
            if (immutable) throw new InvalidOperationException("Appender is in read-only state.");
            if (appender == null) throw new ArgumentNullException(nameof(appender));

            // Append components recursively
            if (appender is IEnumerable<ILineFactory> enumr)
            {
                foreach (var component in enumr)
                    Add(component, policy);
            }

            // Add argument factory
            if (appender is ILineFactoryByArgument argFactory)
            {
                argFactories.Add(argFactory);
            }

            // Iterate each interface
            Type[] intfs = appender.GetType().GetInterfaces();

            // Add ILineFactory0 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILineFactory<>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = paramTypes[0];
                if (itype.IsAssignableFrom(GetType()) || factories0.ContainsKey(key))
                {
                    if (policy == LineFactoryAddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == LineFactoryAddPolicy.IgnoreIfExists) continue;
                }
                factories0[key] = appender;
            }

            // Add ILineFactory1 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILineFactory<,>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = new Pair<Type, Type>(paramTypes[0], paramTypes[1]);
                if (itype.IsAssignableFrom(GetType()) || factories1.ContainsKey(key))
                {
                    if (policy == LineFactoryAddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == LineFactoryAddPolicy.IgnoreIfExists) continue;
                }
                factories1[key] = appender;
            }

            // Add ILineFactory2 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILineFactory<,,>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = new Triple<Type, Type, Type>(paramTypes[0], paramTypes[1], paramTypes[2]);
                if (itype.IsAssignableFrom(GetType()) || factories2.ContainsKey(key))
                {
                    if (policy == LineFactoryAddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == LineFactoryAddPolicy.IgnoreIfExists) continue;
                }
                factories2[key] = appender;
            }

            // Add ILineFactory3 interfaces
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(ILineFactory<,,,>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var key = new Quad<Type, Type, Type, Type>(paramTypes[0], paramTypes[1], paramTypes[2], paramTypes[3]);
                if (itype.IsAssignableFrom(GetType()) || factories3.ContainsKey(key))
                {
                    if (policy == LineFactoryAddPolicy.ThrowIfExists) throw new InvalidOperationException($"Already contains appender for {nameof(itype)}.");
                    else if (policy == LineFactoryAddPolicy.IgnoreIfExists) continue;
                }
                factories3[key] = appender;
            }

            // Add as ILineFactoryAdapter
            if (appender is ILineFactoryCastable adapter) castables.Add(adapter);

            return this;
        }

        /// <summary>
        /// Change appender into read-only state.
        /// </summary>
        /// <returns></returns>
        public LineFactoryComposition ReadOnly()
        {
            this.immutable = true;
            return this;
        }

        /// <summary>
        /// Create clone
        /// </summary>
        /// <returns></returns>
        public virtual LineFactoryComposition Clone()
            => new LineFactoryComposition().AddRange(this, LineFactoryAddPolicy.IgnoreIfExists);

        /// <summary>
        /// List appenders
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ILineFactory> GetEnumerator()
        {
            foreach (var a in castables) yield return a;
            foreach (var a in factories0) yield return a.Value;
            foreach (var a in factories1) yield return a.Value;
            foreach (var a in factories2) yield return a.Value;
            foreach (var a in factories3) yield return a.Value;
            foreach (var a in argFactories) yield return a;
        }

        /// <summary>
        /// List appenders
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var a in castables) yield return a;
            foreach (var a in factories0) yield return a.Value;
            foreach (var a in factories1) yield return a.Value;
            foreach (var a in factories2) yield return a.Value;
            foreach (var a in factories3) yield return a.Value;
            foreach (var a in argFactories) yield return a;
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
        public bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
        {
            foreach (var argFactory in argFactories)
            {
                if (argFactory.TryCreate(factory, previous, arguments, out line)) return true;
            }
            line = default;
            return false;
        }

    }

    /// <summary></summary>
    public static partial class LineLineExtensions
    {
        /// <summary>
        /// Create a clone of <paramref name="previous"/>'s appender, and add new components to it. 
        /// </summary>
        /// <param name="previous">previous part</param>
        /// <param name="appender">appender to add</param>
        /// <param name="policy">add policy</param>
        /// <returns>part with another appender</returns>
        public static ILine AddAppender(this ILine previous, ILineFactory appender, LineFactoryAddPolicy policy = LineFactoryAddPolicy.OverwriteIfExists)
        {
            ILineFactory previousAppender;
            if (previous.TryGetAppender(out previousAppender))
            {
                ILineFactory newAppender = previousAppender == null ? appender : (ILineFactory)new LineFactoryComposition().Add(previousAppender, policy).Add(appender, policy);
                return newAppender.Create<ILinePart>(previous);
            }
            else
            {
                return appender.Create<ILinePart>(previous);
            }
        }
    }

}
