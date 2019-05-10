// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Reflection;

namespace Lexical.Localization
{
    /// <summary>
    /// Resolves string format class name to string format.
    /// </summary>
    public class StringFormatResolver : TypeResolver<IStringFormat>, IStringFormatResolver
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly Lazy<StringFormatResolver> instance = new Lazy<StringFormatResolver>();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static StringFormatResolver Instance => instance.Value;

        /// <summary>
        /// Create type resolver with default settings.
        /// 
        /// Parses expressions and instantiates types that are found in the app domain.
        /// Does not load external dll files.
        /// </summary>
        public StringFormatResolver() : this(DefaultAssemblyResolver, DefaultTypeResolver)
        {
        }

        /// <summary>
        /// Create type resolver.
        /// </summary>
        /// <param name="assemblyLoader">(optional) function that reads assembly from file.</param>
        /// <param name="typeResolver">(optional) Function that resolves type name into <see cref="Type"/>.</param>
        public StringFormatResolver(Func<AssemblyName, Assembly> assemblyLoader, Func<Assembly, string, bool, Type> typeResolver) : base(assemblyLoader, typeResolver)
        {
        }

    }
}
