using Lexical.Localization.Resource;
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Adapts delegate into <see cref="IAsset"/>.
    /// </summary>
    public class AssetFunc :
        IAsset,
        IStringAsset, IStringAssetStringLinesEnumerable, IStringAssetLinesEnumerable,
        IResourceAsset, IResourceAssetNamesEnumerable, IResourceAssetKeysEnumerable
    {
        /// <summary>
        /// Delegate that reads asset.
        /// </summary>
        public readonly Func<IAsset> Func;

        /// <summary>
        /// Create asset that acquires asset from <paramref name="func"/>.
        /// </summary>
        /// <param name="func"></param>
        public AssetFunc(Func<IAsset> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        IEnumerable<KeyValuePair<string, IString>> IStringAssetStringLinesEnumerable.GetAllStringLines(ILine key)
            => (Func() as IStringAssetStringLinesEnumerable)?.GetAllStringLines(key);

        LineResourceBytes IResourceAsset.GetResourceBytes(ILine key)
            => Func() is IResourceAsset resourceAsset ? resourceAsset.GetResourceBytes(key) : new LineResourceBytes(key, (Exception)null, LineStatus.ResolveFailedNoResult);

        IEnumerable<string> IResourceAssetNamesEnumerable.GetResourceNames(ILine key)
            => (Func() as IResourceAssetNamesEnumerable)?.GetResourceNames(key);

        ILine IStringAsset.GetLine(ILine key)
            => (Func() as IStringAsset)?.GetLine(key);

        LineResourceStream IResourceAsset.GetResourceStream(ILine key)
            => Func() is IResourceAsset resourceAsset ? resourceAsset.GetResourceStream(key) : new LineResourceStream(key, (Exception)null, LineStatus.ResolveFailedNoResult);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}()";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> GetStringLines(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetStringLines(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetLines(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetLines(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllLines(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetAllLines(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllResourceNames(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetAllResourceNames(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetResourceKeys(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetResourceKeys(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllResourceKeys(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetAllResourceKeys(filterKey);
    }

    /// <summary></summary>
    public static partial class LanguageAssetExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetProvider"></param>
        /// <returns></returns>
        public static IAssetSource ToSource(this Func<IAsset> assetProvider)
            => new AssetInstanceSource(new AssetFunc(assetProvider));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetProvider"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this Func<IAsset> assetProvider)
            => new AssetFunc(assetProvider);
    }
}
