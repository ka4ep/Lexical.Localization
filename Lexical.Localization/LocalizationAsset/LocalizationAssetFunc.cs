using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts delegate into <see cref="IAsset"/>.
    /// </summary>
    public class LocalizationAssetFunc :
        IAsset,
        ILocalizationStringProvider, ILocalizationStringLinesEnumerable, ILocalizationKeyLinesEnumerable,
        IAssetResourceProvider, IAssetResourceNamesEnumerable, IAssetResourceKeysEnumerable
    {
        /// <summary>
        /// Delegate that reads asset.
        /// </summary>
        public readonly Func<IAsset> Func;

        /// <summary>
        /// Create asset that acquires asset from <paramref name="func"/>.
        /// </summary>
        /// <param name="func"></param>
        public LocalizationAssetFunc(Func<IAsset> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        IEnumerable<KeyValuePair<string, IString>> ILocalizationStringLinesEnumerable.GetAllStringLines(ILine key)
            => (Func() as ILocalizationStringLinesEnumerable)?.GetAllStringLines(key);

        byte[] IAssetResourceProvider.GetResource(ILine key)
            => (Func() as IAssetResourceProvider)?.GetResource(key);

        IEnumerable<string> IAssetResourceNamesEnumerable.GetResourceNames(ILine key)
            => (Func() as IAssetResourceNamesEnumerable)?.GetResourceNames(key);

        ILine ILocalizationStringProvider.GetString(ILine key)
            => (Func() as ILocalizationStringProvider)?.GetString(key);

        Stream IAssetResourceProvider.OpenStream(ILine key)
            => (Func() as IAssetResourceProvider).OpenStream(key);

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
            => (Func() as ILocalizationStringProvider)?.GetStringLines(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetLines(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetLines(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllLines(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllLines(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllResourceNames(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllResourceNames(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetResourceKeys(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetResourceKeys(filterKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllResourceKeys(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllResourceKeys(filterKey);
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
            => new AssetInstanceSource(new LocalizationAssetFunc(assetProvider));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetProvider"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this Func<IAsset> assetProvider)
            => new LocalizationAssetFunc(assetProvider);
    }
}
