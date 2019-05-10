// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Resolves class name to instnace of class.
    /// Caches the instances.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypeResolver<T> : IDisposable
    {
        /// <summary>
        /// Assembly resolver that looks into AppDomain, but will not load external file.
        /// </summary>
        public static Func<AssemblyName, Assembly> DefaultAssemblyResolver = asmName => Assembly.Load(asmName);

        /// <summary>
        /// Assembly resolver that searches dlls from application directory.
        /// </summary>
        public static Func<AssemblyName, Assembly> FileAssemblyResolver = asmName =>
        {
            try
            {
                Assembly a = Assembly.Load(asmName);
                if (a != null) return a;
            }
            catch (Exception)
            {
            }

            string dir = typeof(TypeResolver<>).Assembly.Location;
            if (dir != null)
            {
                string dllName = asmName.Name + ".dll";
                string dllPath = Path.Combine(dir, asmName.Name + ".dll");
                if (!File.Exists(dllPath)) throw new FileNotFoundException(dllName);
                return Assembly.LoadFile(dllPath);
            }
            return null;
        };

        /// <summary>
        /// Default type resolver that does following name mapping.
        /// </summary>
        public static Func<Assembly, string, bool, Type> DefaultTypeResolver = (Assembly a, string typename, bool throwOnError) =>
        {
            // Transform "Unicode.CLDR35", "Unicode.CLDR35,Lexical.Localization" -> "Lexical.Localization.Unicode.CLDR35"
            if ((a == null || a.GetName().Name == "Lexical.Localization") && typename.StartsWith("Unicode.CLDR")) typename = "Lexical.Localization." + typename;
            // Assembly name was specified and it was resolved into Assembly, now try to load the Type from there
            if (a != null) return a.GetType(typename);
            // There was no assembly name specified in the type name
            else return Type.GetType(typename);
        };

        /// <summary>
        /// 0 - not disposed
        /// 1 - disposing
        /// 2 - disposed
        /// </summary>
        long disposed = 0L;

        /// <summary>
        /// Function that resolves type name into <see cref="Type"/>.
        /// </summary>
        protected Func<Assembly, string, bool, Type> typeResolver;

        /// <summary>
        /// Function that reads assembly from file.
        /// </summary>
        protected Func<AssemblyName, Assembly> assemblyResolver;

        /// <summary>
        /// Cache of resolved rules.
        /// </summary>
        protected ConcurrentDictionary<string, ResultLine> cache = new ConcurrentDictionary<string, ResultLine>();

        /// <summary>
        /// Function that resolves rules.
        /// </summary>
        protected Func<string, ResultLine> resolveFunc;

        /// <summary>
        /// Set to disposed and clear cached instances.
        /// </summary>
        public void Dispose()
        {
            // Start disposing
            if (Interlocked.CompareExchange(ref disposed, 1L, 0L) != 0L) return;
            // Release cache
            cache = null;
            // Mark disposed
            Interlocked.CompareExchange(ref disposed, 2L, 1L);
        }

        /// <summary>
        /// Create type resolver with default settings.
        /// 
        /// Parses expressions and instantiates types that are found in the app domain.
        /// Does not load external dll files.
        /// </summary>
        public TypeResolver() : this(DefaultAssemblyResolver, DefaultTypeResolver)
        {
        }

        /// <summary>
        /// Create type resolver.
        /// </summary>
        /// <param name="assemblyLoader">(optional) function that reads assembly from file.</param>
        /// <param name="typeResolver">(optional) Function that resolves type name into <see cref="Type"/>.</param>
        public TypeResolver(Func<AssemblyName, Assembly> assemblyLoader, Func<Assembly, string, bool, Type> typeResolver)
        {
            this.assemblyResolver = assemblyLoader;
            this.typeResolver = typeResolver;
            this.resolveFunc = (string typeName) =>
            {
                try
                {
                    // Empty 
                    if (String.IsNullOrEmpty(typeName)) return new ResultLine { Value = default, Error = null };

                    // Assert assemblyResolver is not null
                    if (assemblyResolver == null) throw new InvalidOperationException($"{nameof(assemblyResolver)} is null");
                    // Assert typeResolver is not null
                    if (typeResolver == null) throw new InvalidOperationException($"{nameof(typeResolver)} is null");
                    // Try get type
                    Type type = Type.GetType(typeName, assemblyResolver, typeResolver, true);
                    // Assert type was loaded
                    if (type == null) throw new TypeLoadException($"Could not resolve Type {typeName}.");
                    // Assert type is assignable
                    if (!typeof(IEnumerable<T>).IsAssignableFrom(type)) throw new InvalidCastException($"{typeName} doesn't implement {nameof(T)}");
                    // Instantiate type
                    object obj = Activator.CreateInstance(type);
                    // Cast
                    if (obj is T casted)
                    {
                        return new ResultLine { Value = casted };
                    } else
                    {
                        return new ResultLine { Error = null, Value = default };
                    }
                }
                catch (Exception e)
                {
                    return new ResultLine { Error = e };
                }
            };
        }

        /// <summary>
        /// Resolve into value.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns>value</returns>
        /// <exception cref="LocalizationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public T Resolve(string typeName)
        {
            var _cache = cache;
            if (_cache == null || Interlocked.Read(ref disposed) != 0L) throw new ObjectDisposedException(GetType().FullName);
            ResultLine line = _cache.GetOrAdd(typeName, resolveFunc);
            if (line.Error != null) throw new LocalizationException(line.Error.Message, line.Error);
            return line.Value;
        }

        /// <summary>
        /// Try resolve into value.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="result"></param>
        /// <returns>value</returns>
        /// <exception cref="LocalizationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public bool TryResolve(string typeName, out T result)
        {
            var _cache = cache;
            if (_cache == null || Interlocked.Read(ref disposed) != 0L) throw new ObjectDisposedException(GetType().FullName);
            ResultLine line = _cache.GetOrAdd(typeName, resolveFunc);
            if (line.Error != null) { result = default; return false; }
            if (line.Value == default) { result = default; return false; }
            result = line.Value;
            return true;
        }

        /// <summary>
        /// Record that contains either result or an error.
        /// </summary>
        public struct ResultLine
        {
            /// <summary>
            /// Error
            /// </summary>
            public Exception Error;

            /// <summary>
            /// Result
            /// </summary>
            public T Value;
        }

    }
}
