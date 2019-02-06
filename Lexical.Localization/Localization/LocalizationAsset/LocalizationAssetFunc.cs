using Lexical.Asset;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts delegate into <see cref="IAsset"/>.
    /// </summary>
    public class LocalizationAssetFunc :
        IAsset,
        ILocalizationStringProvider, ILocalizationStringCollection,
        IAssetResourceProvider, IAssetResourceCollection
    {
        public readonly Func<IAsset> Func;

        public LocalizationAssetFunc(Func<IAsset> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        IEnumerable<KeyValuePair<string, string>> ILocalizationStringCollection.GetAllStrings(IAssetKey key)
            => (Func() as ILocalizationStringCollection)?.GetAllStrings(key);

        byte[] IAssetResourceProvider.GetResource(IAssetKey key)
            => (Func() as IAssetResourceProvider)?.GetResource(key);

        IEnumerable<string> IAssetResourceCollection.GetResourceNames(IAssetKey key)
            => (Func() as IAssetResourceCollection)?.GetResourceNames(key);

        string ILocalizationStringProvider.GetString(IAssetKey key)
            => (Func() as ILocalizationStringProvider)?.GetString(key);

        Stream IAssetResourceProvider.OpenStream(IAssetKey key)
            => (Func() as IAssetResourceProvider).OpenStream(key);

        public override string ToString()
            => $"{GetType().Name}()";
    }

    public static partial class LanguageAssetExtensions
    {
        public static IAssetSource ToSource(this Func<IAsset> assetProvider)
            => new AssetSource(new LocalizationAssetFunc(assetProvider));
        public static IAsset ToAsset(this Func<IAsset> assetProvider)
            => new LocalizationAssetFunc(assetProvider);
    }
}
