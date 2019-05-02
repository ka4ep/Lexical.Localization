// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts Delegate to <see cref="ILocalizationStringProvider"/>.
    /// </summary>
    public class LocalizationStringsFunc : ILocalizationStringProvider
    {
        /// <summary>
        /// Function that resolve key to a language string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved string or null</returns>
        public readonly Func<ILinePart, IFormulationString> ResolverFunc;

        public LocalizationStringsFunc(Func<ILinePart, IFormulationString> resolverFunc)
        {
            this.ResolverFunc = resolverFunc;
        }

        public IFormulationString GetString(ILinePart key) => ResolverFunc(key);
        public override string ToString() => $"{GetType().Name}()";
    }

    public static partial class LocalizationAssetExtensions_
    {
        public static IAssetSource ToSource(this Func<ILinePart, IFormulationString> stringFunc)
            => new AssetInstanceSource(new LocalizationStringsFunc(stringFunc));
        public static ILocalizationStringProvider ToAsset(this Func<ILinePart, IFormulationString> stringFunc)
            => new LocalizationStringsFunc(stringFunc);

        /// <summary>
        /// Adapts <see cref="Delegate"/> to <see cref="ILocalizationStringProvider"/> and adds to composition.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resolverFunc"></param>
        /// <returns>composition</returns>
        public static IAssetComposition AddSourceFunc(this IAssetComposition composition, Func<ILinePart, IFormulationString> resolverFunc)
        {
            composition.Add(new LocalizationStringsFunc(resolverFunc));
            return composition;
        }


        /// <summary>
        /// Adapts <see cref="Delegate"/> to <see cref="IAssetSource"/> and adds to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resolver"></param>
        /// <returns>builder</returns>
        public static IAssetBuilder AddSourceFunc(this IAssetBuilder builder, Func<ILinePart, IFormulationString> resolver)
        {
            builder.Sources.Add(new AssetInstanceSource(new LocalizationStringsFunc(resolver)));
            return builder;
        }
    }
}
