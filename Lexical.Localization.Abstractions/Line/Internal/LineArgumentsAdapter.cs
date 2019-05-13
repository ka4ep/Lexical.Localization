// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           13.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Concurrent;

namespace Lexical.Localization.Line.Internal
{
    /// <summary>
    /// Adapts <see cref="ILineFactoryByArgument"/> so that it forwards <see cref="ILineArguments"/> to call <see cref="ILineFactory{Intf}"/> and <see cref="ILineFactoryCastable"/> implementations without reflection.
    /// </summary>
    public class LineFactoryByArgumentAdapter
    {
        /// <summary>
        /// Default instance
        /// </summary>
        static LineFactoryByArgumentAdapter instance = new LineFactoryByArgumentAdapter();

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static LineFactoryByArgumentAdapter Instance => instance;

        /// <summary>
        /// Cache of factories by implementing class type.
        /// </summary>
        ConcurrentDictionary<Type, ILineFactoryByArgument> cache = new ConcurrentDictionary<Type, ILineFactoryByArgument>();

        /// <summary>
        /// Crates factory.
        /// </summary>
        Func<Type, ILineFactoryByArgument> valueFactory;

        /// <summary>
        /// Create builder
        /// </summary>
        public LineFactoryByArgumentAdapter()
        {
            valueFactory = Build;
        }

        /// <summary>
        /// Get-or-create adapter.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>factory or null</returns>
        /// <exception cref="LineException">If adapter construction fails</exception>
        public ILineFactoryByArgument Get(Type type)
            => cache.GetOrAdd(type, valueFactory);

        /// <summary>
        /// Get-or-Create adapter
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        /// <exception cref="LineException">If adapter construction fails</exception>
        public bool TryGet(Type type, out ILineFactoryByArgument factory)
        {
            ILineFactoryByArgument f = cache.GetOrAdd(type, valueFactory);
            factory = f;
            return f != null;
        }

        /// <summary>
        /// Creates a line factory that is specialized for handling <see cref="ILineArguments"/> of specific implementing <paramref name="classType"/>.
        /// </summary>
        /// <param name="classType">type that implements <see cref="ILineArguments"/> once or more than once</param>
        /// <returns>factory or null</returns>
        /// <exception cref="LineException">If adapter construction fails</exception>
        public ILineFactoryByArgument Build(Type classType)
        {
            try
            {
                StructList8<ILineFactoryByArgument> factories = new StructList8<ILineFactoryByArgument>();

                Type[] intfs = classType.GetInterfaces();
                foreach (Type intf in intfs)
                {
                    Type intfGeneric = intf.GetGenericTypeDefinition();
                    if (intfGeneric == null) continue;
                    Type[] typeArgs = intf.GetGenericArguments();

                    // ILineArguments<>
                    if (intfGeneric == typeof(ILineArguments<>)) factories.Add(Activator.CreateInstance(typeof(Adapter<>).MakeGenericType(typeArgs)) as ILineFactoryByArgument);
                    // ILineArguments<,>
                    else if (intfGeneric == typeof(ILineArguments<,>)) factories.Add(Activator.CreateInstance(typeof(Adapter<,>).MakeGenericType(typeArgs)) as ILineFactoryByArgument);
                    // ILineArguments<,,>
                    else if (intfGeneric == typeof(ILineArguments<,,>)) factories.Add(Activator.CreateInstance(typeof(Adapter<,,>).MakeGenericType(typeArgs)) as ILineFactoryByArgument);
                    // ILineArguments<,,,>
                    else if (intfGeneric == typeof(ILineArguments<,,,>)) factories.Add(Activator.CreateInstance(typeof(Adapter<,,>).MakeGenericType(typeArgs)) as ILineFactoryByArgument);
                }

                if (factories.Count == 0) return null;
                if (factories.Count == 1) return factories[0];
                return new Factories(factories.ToArray());
            }
            catch (Exception e)
            {
                throw new LineException(null, e.Message, e);
            }
        }

        class Factories : ILineFactoryByArgument
        {
            ILineFactoryByArgument[] factories;

            public Factories(ILineFactoryByArgument[] factories)
            {
                this.factories = factories ?? throw new ArgumentNullException(nameof(factories));
            }

            public bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
            {
                ILine result = previous;
                foreach (var f in factories)
                    if (!f.TryCreate(factory, result, arguments, out result)) { line = default; return false; }
                line = result;
                return true;
            }
        }

        class Adapter<Intf> : ILineFactoryByArgument where Intf : ILine
        {
            public bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
            {
                if (arguments is ILineArguments<Intf>)
                {
                    Intf result;
                    if (factory is ILineFactory<Intf> casted && casted.TryCreate<Intf>(previous, out result)) { line = result; return true; }
                    ILineFactory<Intf> _casted;
                    if (factory is ILineFactoryCastable castable && ((_casted = castable.Cast<Intf>()) != null) && _casted.TryCreate(factory, previous, out result)) { line = result; return true; }
                }
                line = default;
                return false;
            }
        }

        class Adapter<Intf, A0> : ILineFactoryByArgument where Intf : ILine
        {
            public bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
            {
                if (arguments is ILineArguments<Intf, A0> args)
                {
                    Intf result;
                    if (factory is ILineFactory<Intf> casted && casted.TryCreate<Intf, A0>(previous, args.Argument0, out result)) { line = result; return true; }
                    ILineFactory<Intf, A0> _casted;
                    if (factory is ILineFactoryCastable castable && ((_casted = castable.Cast<Intf, A0>()) != null) && _casted.TryCreate(factory, previous, args.Argument0, out result)) { line = result; return true; }
                }
                line = default;
                return false;
            }
        }

        class Adapter<Intf, A0, A1> : ILineFactoryByArgument where Intf : ILine
        {
            public bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
            {
                if (arguments is ILineArguments<Intf, A0, A1> args)
                {
                    Intf result;
                    if (factory is ILineFactory<Intf> casted && casted.TryCreate<Intf, A0, A1>(previous, args.Argument0, args.Argument1, out result)) { line = result; return true; }
                    ILineFactory<Intf, A0, A1> _casted;
                    if (factory is ILineFactoryCastable castable && ((_casted = castable.Cast<Intf, A0, A1>()) != null) && _casted.TryCreate(factory, previous, args.Argument0, args.Argument1, out result)) { line = result; return true; }
                }
                line = default;
                return false;
            }
        }

        class Adapter<Intf, A0, A1, A2> : ILineFactoryByArgument where Intf : ILine
        {
            public bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
            {
                if (arguments is ILineArguments<Intf, A0, A1, A2> args)
                {
                    Intf result;
                    if (factory is ILineFactory<Intf> casted && casted.TryCreate<Intf, A0, A1, A2>(previous, args.Argument0, args.Argument1, args.Argument2, out result)) { line = result; return true; }
                    ILineFactory<Intf, A0, A1, A2> _casted;
                    if (factory is ILineFactoryCastable castable && ((_casted = castable.Cast<Intf, A0, A1, A2>()) != null) && _casted.TryCreate(factory, previous, args.Argument0, args.Argument1, args.Argument2, out result)) { line = result; return true; }
                }
                line = default;
                return false;
            }
        }

    }
}
