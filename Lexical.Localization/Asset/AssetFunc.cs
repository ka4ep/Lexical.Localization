using Lexical.Localization.Binary;
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
        IStringAsset, IStringAssetUnformedLinesEnumerable, IStringAssetLinesEnumerable,
        IBinaryAsset, IBinaryAssetNamesEnumerable, IBinaryAssetKeysEnumerable
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

        IEnumerable<KeyValuePair<string, IString>> IStringAssetUnformedLinesEnumerable.GetAllUnformedLines(ILine key)
            => (Func() as IStringAssetUnformedLinesEnumerable)?.GetAllUnformedLines(key);

        LineBinaryBytes IBinaryAsset.GetBytes(ILine key)
            => Func() is IBinaryAsset resourceAsset ? resourceAsset.GetBytes(key) : new LineBinaryBytes(key, (Exception)null, LineStatus.ResolveFailedNoResult);

        IEnumerable<string> IBinaryAssetNamesEnumerable.GetBinaryNames(ILine key)
            => (Func() as IBinaryAssetNamesEnumerable)?.GetBinaryNames(key);

        ILine IStringAsset.GetLine(ILine key)
            => (Func() as IStringAsset)?.GetLine(key);

        LineBinaryStream IBinaryAsset.GetStream(ILine key)
            => Func() is IBinaryAsset resourceAsset ? resourceAsset.GetStream(key) : new LineBinaryStream(key, (Exception)null, LineStatus.ResolveFailedNoResult);

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
        public IEnumerable<KeyValuePair<string, IString>> GetUnformedLines(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetUnformedLines(filterKey);

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
        public IEnumerable<string> GetAllBinaryNames(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetAllBinaryNames(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetBinaryKeys(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetBinaryKeys(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllBinaryKeys(ILine filterKey = null)
            => (Func() as IStringAsset)?.GetAllBinaryKeys(filterKey);
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
            => new AssetFactory(new AssetFunc(assetProvider));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetProvider"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this Func<IAsset> assetProvider)
            => new AssetFunc(assetProvider);
    }
}
