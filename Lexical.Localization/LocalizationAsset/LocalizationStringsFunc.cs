// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Line provider that reads from a delegate.
    /// </summary>
    public class LocalizationStringsFunc : ILocalizationStringProvider
    {
        /// <summary>
        /// Function that resolve key to a language string.
        /// </summary>
        public readonly Func<ILine, ILine> ResolverFunc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolverFunc"></param>
        public LocalizationStringsFunc(Func<ILine, ILine> resolverFunc)
        {
            this.ResolverFunc = resolverFunc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ILine GetString(ILine key) => ResolverFunc(key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GetType().Name}()";
    }

    public static partial class LocalizationAssetExtensions_
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringFunc"></param>
        /// <returns></returns>
        public static IAssetSource ToSource(this Func<ILine, ILine> stringFunc)
            => new AssetInstanceSource(new LocalizationStringsFunc(stringFunc));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringFunc"></param>
        /// <returns></returns>
        public static ILocalizationStringProvider ToAsset(this Func<ILine, ILine> stringFunc)
            => new LocalizationStringsFunc(stringFunc);

        /// <summary>
        /// Adapts <see cref="Delegate"/> to <see cref="ILocalizationStringProvider"/> and adds to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="resolverFunc"></param>
        /// <returns>composition</returns>
        public static IAssetComposition AddSourceFunc(this IAssetComposition composition, Func<ILine, ILine> resolverFunc)
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
        public static IAssetBuilder AddSourceFunc(this IAssetBuilder builder, Func<ILine, ILine> resolver)
        {
            builder.Sources.Add(new AssetInstanceSource(new LocalizationStringsFunc(resolver)));
            return builder;
        }
    }
}
