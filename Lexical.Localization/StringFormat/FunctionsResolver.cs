// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Reflection;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolves function class name to <see cref="IFunctions"/>.
    /// </summary>
    public class FunctionsResolver : TypeResolver<IFunctions>, IParameterResolver<IFunctions>
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        static readonly Lazy<FunctionsResolver> instance = new Lazy<FunctionsResolver>();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static FunctionsResolver Default => instance.Value;

        /// <summary>
        /// Parameter Name
        /// </summary>
        public string ParameterName => "Functions";

        /// <summary>
        /// Create type resolver with default settings.
        /// 
        /// Parses expressions and instantiates types that are found in the app domain.
        /// Does not load external dll files.
        /// </summary>
        public FunctionsResolver() : this(DefaultAssemblyResolver, DefaultTypeResolver)
        {
        }

        /// <summary>
        /// Create type resolver.
        /// </summary>
        /// <param name="assemblyLoader">(optional) function that reads assembly from file.</param>
        /// <param name="typeResolver">(optional) Function that resolves type name into <see cref="Type"/>.</param>
        public FunctionsResolver(Func<AssemblyName, Assembly> assemblyLoader, Func<Assembly, string, bool, Type> typeResolver) : base(assemblyLoader, typeResolver)
        {
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
            } else
            {
                // Continue disposing
                base.Dispose();
            }
        }
    }
}
