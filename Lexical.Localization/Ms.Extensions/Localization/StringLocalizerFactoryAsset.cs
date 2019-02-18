// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// Adapts <see cref="IStringLocalizerFactory"/> into <see cref="IAsset"/>.
    /// </summary>
    public class StringLocalizerFactoryAsset : ILocalizationStringProvider, IAssetReloadable
    {
        protected readonly IStringLocalizerFactory stringLocalizerFactory;

        protected ConcurrentDictionary<Type, StringLocalizerAsset.Type> map_by_type;
        protected ConcurrentDictionary<Pair<string, string>, StringLocalizerAsset.Location> map_by_location;

        Func<Type, StringLocalizerAsset.Type> createByTypeFunc;
        Func<Pair<string, string>, StringLocalizerAsset.Location> createByLocationFunc;

        public StringLocalizerFactoryAsset(IStringLocalizerFactory stringLocalizerFactory)
        {
            this.stringLocalizerFactory = stringLocalizerFactory ?? throw new ArgumentNullException(nameof(stringLocalizerFactory));
            this.map_by_type = new ConcurrentDictionary<Type, StringLocalizerAsset.Type>();
            this.map_by_location = new ConcurrentDictionary<Pair<String, String>, StringLocalizerAsset.Location>();
            createByTypeFunc = type => new StringLocalizerAsset.Type(stringLocalizerFactory.Create(type), type, null);
            createByLocationFunc = location => new StringLocalizerAsset.Location(stringLocalizerFactory.Create(location.a, location.b), location.a, location.b, null);
        }

        public IAsset Create(Type type, CultureInfo culture = null)
            => new StringLocalizerAsset.Type(stringLocalizerFactory.Create(type), type, culture);
        public IAsset Create(string basename, string location, CultureInfo culture = null)
            => new StringLocalizerAsset.Location(stringLocalizerFactory.Create(basename, location), basename, location, culture);

        public StringLocalizerAsset GetHandlingAsset(IAssetKey key)
        {
            IAssetKeyAssemblySection asmSection;
            IAssetKeyResourceSection resSection;
            IAssetKeyTypeSection typeSection;
            int x = key.FindResourceSections(out asmSection, out resSection, out typeSection);
            return
                x == 1 ? map_by_type.GetOrAdd(typeSection.Type, createByTypeFunc) :
                x == 2 ? (StringLocalizerAsset)map_by_location.GetOrAdd(new Pair<string, string>(resSection.Name, asmSection.Name), createByLocationFunc) :
                null;
        }

        public string GetString(IAssetKey key)
            => GetHandlingAsset(key)?.GetString(key);

        public IAsset Reload()
        {
            map_by_type.Clear();
            map_by_location.Clear();
            return this;
        }
    }

    public static partial class MsLocalizationExtensions
    {
        /// <summary>
        /// Adapts <see cref="IStringLocalizerFactory"/> into an <see cref="IAsset" />.
        /// 
        /// Notice, that using asset that is converted this way, requres that keys have 
        /// <see cref="IAssetKeyTypeSection"/> hint, or
        /// <see cref="IAssetKeyAssemblySection"/>+<see cref="IAssetKeyResourceSection"/> hints.
        /// </summary>
        /// <param name="stringLocalizerFactory"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IStringLocalizerFactory stringLocalizerFactory)
            => new StringLocalizerFactoryAsset(stringLocalizerFactory);

        /// <summary>
        /// Adapts <see cref="IStringLocalizerFactory"/> into an <see cref="IAssetSource" />.
        /// 
        /// Notice, that using asset that is converted this way, requres that keys have 
        /// <see cref="IAssetKeyTypeSection"/> hint, or
        /// <see cref="IAssetKeyAssemblySection"/>+<see cref="IAssetKeyResourceSection"/> hints.
        /// </summary>
        /// <param name="stringLocalizerFactory"></param>
        /// <returns></returns>
        public static IAssetSource ToSource(this IStringLocalizerFactory stringLocalizerFactory)
            => new StringLocalizerFactoryAsset(stringLocalizerFactory).ToSource();

        public static StringLocalizerFactoryAsset ToAssetFactory(this IStringLocalizerFactory stringLocalizerFactory)
            => new StringLocalizerFactoryAsset(stringLocalizerFactory);
    }
}
