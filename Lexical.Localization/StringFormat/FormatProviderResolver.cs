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
    /// Resolves function class name to <see cref="IFormatProvider"/>.
    /// </summary>
    public class FormatProviderResolver : ParameterResolver<IFormatProvider>, IResolver<IFormatProvider>
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        static readonly Lazy<FormatProviderResolver> instance = new Lazy<FormatProviderResolver>();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static FormatProviderResolver Default => instance.Value;

        /// <summary>
        /// Create type resolver with default settings.
        /// 
        /// Parses expressions and instantiates types that are found in the app domain.
        /// Does not load external dll files.
        /// </summary>
        public FormatProviderResolver() : base("FormatProvider", DefaultAssemblyResolver, DefaultTypeResolver)
        {
        }

        /// <summary>
        /// Create type resolver.
        /// </summary>
        /// <param name="assemblyLoader">(optional) function that reads assembly from file.</param>
        /// <param name="typeResolver">(optional) Function that resolves type name into <see cref="Type"/>.</param>
        public FormatProviderResolver(Func<AssemblyName, Assembly> assemblyLoader, Func<Assembly, string, bool, Type> typeResolver) : base("FormatProvider", assemblyLoader, typeResolver)
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
