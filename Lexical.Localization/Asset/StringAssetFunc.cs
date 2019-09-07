// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Line provider that reads from a delegate.
    /// </summary>
    public class StringAssetFunc : IStringAsset
    {
        /// <summary>
        /// Function that resolve key to a language string.
        /// </summary>
        public readonly Func<ILine, ILine> ResolverFunc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolverFunc"></param>
        public StringAssetFunc(Func<ILine, ILine> resolverFunc)
        {
            this.ResolverFunc = resolverFunc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ILine GetLine(ILine key) => ResolverFunc(key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GetType().Name}()";
    }

    public static partial class StringAssetExtensions_
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringFunc"></param>
        /// <returns></returns>
        public static IAssetSource ToSource(this Func<ILine, ILine> stringFunc)
            => new AssetFactory(new StringAssetFunc(stringFunc));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringFunc"></param>
        /// <returns></returns>
        public static IStringAsset ToAsset(this Func<ILine, ILine> stringFunc)
            => new StringAssetFunc(stringFunc);

        /// <summary>
        /// Adapts <see cref="Delegate"/> to <see cref="IStringAsset"/> and adds to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="resolverFunc"></param>
        /// <returns>composition</returns>
        public static IAssetComposition AddSourceFunc(this IAssetComposition composition, Func<ILine, ILine> resolverFunc)
        {
            composition.Add(new StringAssetFunc(resolverFunc));
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
            builder.Sources.Add(new AssetFactory(new StringAssetFunc(resolver)));
            return builder;
        }
    }
}
