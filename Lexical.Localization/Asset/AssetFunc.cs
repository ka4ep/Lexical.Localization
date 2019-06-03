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

        byte[] IResourceAsset.GetResourceBytes(ILine key)
            => (Func() as IResourceAsset)?.GetResourceBytes(key);

        IEnumerable<string> IResourceAssetNamesEnumerable.GetResourceNames(ILine key)
            => (Func() as IResourceAssetNamesEnumerable)?.GetResourceNames(key);

        ILine IStringAsset.GetString(ILine key)
            => (Func() as IStringAsset)?.GetString(key);

        Stream IResourceAsset.GetResourceStream(ILine key)
            => (Func() as IResourceAsset).GetResourceStream(key);

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
