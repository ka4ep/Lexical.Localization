// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resolver;
using System;
using System.Reflection;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolves parameter "Type" to <see cref="Type"/>.
    /// </summary>
    public class TypeResolver : ParameterResolver<ILineType, Type>
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        static readonly Lazy<TypeResolver> instance = new Lazy<TypeResolver>();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static TypeResolver Default => instance.Value;

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
        public TypeResolver(Func<AssemblyName, Assembly> assemblyLoader, Func<Assembly, string, bool, Type> typeResolver) : base("Type", assemblyLoader, typeResolver)
        {
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
                    if (type != null)
                    {
                        return new ResultLine { Value = type };
                    }
                    else
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
        /// Dispose or clear resolver.
        /// </summary>
        public override void Dispose()
        {
            if (this == Default)
            {
                // Don't dispose the global static instance, but clear its cache.
                cache.Clear();
            }
            else
            {
                // Continue disposing
                base.Dispose();
            }
        }
    }
}
