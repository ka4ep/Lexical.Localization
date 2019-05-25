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
        public readonly Func<IAsset> Func;

        public LocalizationAssetFunc(Func<IAsset> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        IEnumerable<KeyValuePair<string, IFormatString>> ILocalizationStringLinesEnumerable.GetAllStringLines(ILine key)
            => (Func() as ILocalizationStringLinesEnumerable)?.GetAllStringLines(key);

        byte[] IAssetResourceProvider.GetResource(ILine key)
            => (Func() as IAssetResourceProvider)?.GetResource(key);

        IEnumerable<string> IAssetResourceNamesEnumerable.GetResourceNames(ILine key)
            => (Func() as IAssetResourceNamesEnumerable)?.GetResourceNames(key);

        IFormatString ILocalizationStringProvider.GetString(ILine key)
            => (Func() as ILocalizationStringProvider)?.GetString(key);

        Stream IAssetResourceProvider.OpenStream(ILine key)
            => (Func() as IAssetResourceProvider).OpenStream(key);

        public override string ToString()
            => $"{GetType().Name}()";

        public IEnumerable<KeyValuePair<string, IFormatString>> GetStringLines(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetStringLines(filterKey);

        public IEnumerable<ILine> GetLines(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetLines(filterKey);

        public IEnumerable<ILine> GetAllLines(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllLines(filterKey);

        public IEnumerable<string> GetAllResourceNames(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllResourceNames(filterKey);

        public IEnumerable<ILine> GetResourceKeys(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetResourceKeys(filterKey);

        public IEnumerable<ILine> GetAllResourceKeys(ILine filterKey = null)
            => (Func() as ILocalizationStringProvider)?.GetAllResourceKeys(filterKey);
    }

    public static partial class LanguageAssetExtensions
    {
        public static IAssetSource ToSource(this Func<IAsset> assetProvider)
            => new AssetInstanceSource(new LocalizationAssetFunc(assetProvider));
        public static IAsset ToAsset(this Func<IAsset> assetProvider)
            => new LocalizationAssetFunc(assetProvider);
    }
}
