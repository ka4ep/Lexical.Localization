// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace Lexical.Localization
{
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// Adapts <see cref="IStringLocalizerFactory"/> into <see cref="IAsset"/>.
    /// </summary>
    public class StringLocalizerFactoryAsset : ILocalizationStringProvider, IAssetReloadable
    {
        /// <summary>
        /// Source factory
        /// </summary>
        protected readonly IStringLocalizerFactory stringLocalizerFactory;

        /// <summary>
        /// Value converter
        /// </summary>
        public readonly IStringFormatParser ValueParser;

        /// <summary>
        /// Cache of converted assets
        /// </summary>
        protected ConcurrentDictionary<String, StringLocalizerAsset.Type> map_by_type;

        /// <summary>
        /// Cache of converted assets
        /// </summary>
        protected ConcurrentDictionary<Pair<string, string>, StringLocalizerAsset.Location> map_by_location;

        Func<String, StringLocalizerAsset.Type> createByTypeFunc;
        Func<Pair<string, string>, StringLocalizerAsset.Location> createByLocationFunc;

        /// <summary>
        /// Create factory asset.
        /// </summary>
        /// <param name="stringLocalizerFactory"></param>
        /// <param name="valueParser"></param>
        public StringLocalizerFactoryAsset(IStringLocalizerFactory stringLocalizerFactory, IStringFormatParser valueParser = default)
        {
            this.stringLocalizerFactory = stringLocalizerFactory ?? throw new ArgumentNullException(nameof(stringLocalizerFactory));
            this.map_by_type = new ConcurrentDictionary<String, StringLocalizerAsset.Type>();
            this.map_by_location = new ConcurrentDictionary<Pair<String, String>, StringLocalizerAsset.Location>();
            createByTypeFunc = typeName =>
            {
                Type type = Type.GetType(typeName, throwOnError: true);
                return new StringLocalizerAsset.Type(stringLocalizerFactory.Create(type), type, null);
            };
            createByLocationFunc = location => new StringLocalizerAsset.Location(stringLocalizerFactory.Create(location.a, location.b), location.a, location.b, null);
            this.ValueParser = ValueParser ?? CSharpFormat.Instance;
        }

        /// <summary>
        /// Create string localizer for type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public IAsset Create(Type type, CultureInfo culture = null)
            => new StringLocalizerAsset.Type(stringLocalizerFactory.Create(type), type, culture, ValueParser);

        /// <summary>
        /// Create string localizer for location and basename.
        /// </summary>
        /// <param name="basename"></param>
        /// <param name="location"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public IAsset Create(string basename, string location, CultureInfo culture = null)
            => new StringLocalizerAsset.Location(stringLocalizerFactory.Create(basename, location), basename, location, culture, ValueParser);

        /// <summary>
        /// Get-or-create string localizer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public StringLocalizerAsset GetHandlingAsset(ILine key)
        {
            string _location;
            string _basename;
            string _type;
            int x = key.FindResourceInfos(out _type, out _location, out _basename);
            return
                x == 1 ? map_by_type.GetOrAdd(_type, createByTypeFunc) :
                x == 2 ? (StringLocalizerAsset)map_by_location.GetOrAdd(new Pair<string, string>(_basename, _location), createByLocationFunc) :
                null;
        }

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IFormulationString GetString(ILine key)
            => GetHandlingAsset(key)?.GetString(key);

        /// <summary>
        /// Clear caches
        /// </summary>
        /// <returns></returns>
        public IAsset Reload()
        {
            map_by_type.Clear();
            map_by_location.Clear();
            return this;
        }
    }

    /// <summary></summary>
    public static partial class MsLocalizationExtensions
    {
        /// <summary>
        /// Adapts <see cref="IStringLocalizerFactory"/> into an <see cref="IAsset" />.
        /// 
        /// Notice, that using asset that is converted this way, requres that keys have 
        /// "Type" parameter, or "BaseName" and "Location" parameters.
        /// </summary>
        /// <param name="stringLocalizerFactory"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IStringLocalizerFactory stringLocalizerFactory)
            => new StringLocalizerFactoryAsset(stringLocalizerFactory);

        /// <summary>
        /// Adapts <see cref="IStringLocalizerFactory"/> into an <see cref="IAssetSource" />.
        /// 
        /// Notice, that using asset that is converted this way, requres that keys have 
        /// "Type" parameter, or "BaseName" and "Location" parameters.
        /// </summary>
        /// <param name="stringLocalizerFactory"></param>
        /// <returns></returns>
        public static IAssetSource ToSource(this IStringLocalizerFactory stringLocalizerFactory)
            => new StringLocalizerFactoryAsset(stringLocalizerFactory).ToSource();

        /// <summary>
        /// Adapt to asset.
        /// </summary>
        /// <param name="stringLocalizerFactory"></param>
        /// <returns></returns>
        public static StringLocalizerFactoryAsset ToAssetFactory(this IStringLocalizerFactory stringLocalizerFactory)
            => new StringLocalizerFactoryAsset(stringLocalizerFactory);
    }
}
