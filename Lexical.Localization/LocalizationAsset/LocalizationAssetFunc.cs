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
        public readonly Func<IAsset> Func;

        public LocalizationAssetFunc(Func<IAsset> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        IEnumerable<KeyValuePair<string, string>> ILocalizationStringLinesEnumerable.GetAllStringLines(IAssetKey key)
            => (Func() as ILocalizationStringLinesEnumerable)?.GetAllStringLines(key);

        byte[] IAssetResourceProvider.GetResource(IAssetKey key)
            => (Func() as IAssetResourceProvider)?.GetResource(key);

        IEnumerable<string> IAssetResourceNamesEnumerable.GetResourceNames(IAssetKey key)
            => (Func() as IAssetResourceNamesEnumerable)?.GetResourceNames(key);

        string ILocalizationStringProvider.GetString(IAssetKey key)
            => (Func() as ILocalizationStringProvider)?.GetString(key);

        Stream IAssetResourceProvider.OpenStream(IAssetKey key)
            => (Func() as IAssetResourceProvider).OpenStream(key);

        public override string ToString()
            => $"{GetType().Name}()";

        public IEnumerable<KeyValuePair<string, string>> GetStringLines(IAssetKey filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetStringLines(filterKey);

        public IEnumerable<KeyValuePair<IAssetKey, string>> GetKeyLines(IAssetKey filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetKeyLines(filterKey);

        public IEnumerable<KeyValuePair<IAssetKey, string>> GetAllKeyLines(IAssetKey filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllKeyLines(filterKey);

        public IEnumerable<string> GetAllResourceNames(IAssetKey filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllResourceNames(filterKey);

        public IEnumerable<IAssetKey> GetResourceKeys(IAssetKey filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetResourceKeys(filterKey);

        public IEnumerable<IAssetKey> GetAllResourceKeys(IAssetKey filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllResourceKeys(filterKey);
    }

    public static partial class LanguageAssetExtensions
    {
        public static IAssetSource ToSource(this Func<IAsset> assetProvider)
            => new AssetSource(new LocalizationAssetFunc(assetProvider));
        public static IAsset ToAsset(this Func<IAsset> assetProvider)
            => new LocalizationAssetFunc(assetProvider);
    }
}
