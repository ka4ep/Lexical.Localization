// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization
{
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// Adapts <see cref="IStringLocalizer"/> into <see cref="IAsset"/>.
    /// 
    /// Can be constructed in three kinds:
    ///   Unassigned - adapter is not assigned to any location or type.
    ///   Type - adapter is assigned for specific type
    ///   Location - adapter is assigned to specific location
    /// 
    /// If <see cref="IStringLocalizer.WithCulture(CultureInfo)"/> is called, then creates a new 
    /// adapter with WithCulture is assigned.
    /// </summary>
    public class StringLocalizerAsset : 
        ILocalizationStringProvider, 
        ILocalizationStringLinesEnumerable,  // Doesn't work
        IAssetReloadable
    {
        public readonly IStringLocalizer stringLocalizer;
        public readonly CultureInfo culture;
        public readonly ILocalizationStringFormatParser ValueParser;

        protected ConcurrentDictionary<CultureInfo, StringLocalizerAsset> culture_map;
        protected Func<CultureInfo, StringLocalizerAsset> createFunc;

        public StringLocalizerAsset(IStringLocalizer stringLocalizer, ILocalizationStringFormatParser valueParser = default)
        {
            this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            this.culture_map = new ConcurrentDictionary<CultureInfo, StringLocalizerAsset>();
            this.createFunc = ci => new StringLocalizerAsset(stringLocalizer.WithCulture(ci), ci);
            this.ValueParser = ValueParser ?? LexicalStringFormat.Instance;
        }
        public StringLocalizerAsset(IStringLocalizer stringLocalizer, CultureInfo culture, ILocalizationStringFormatParser valueParser = default)
        {
            this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            this.createFunc = ci => new StringLocalizerAsset(stringLocalizer.WithCulture(ci), ci);
            this.ValueParser = ValueParser ?? LexicalStringFormat.Instance;
            if (culture == null || culture.Name == "")
            {
                this.culture_map = new ConcurrentDictionary<CultureInfo, StringLocalizerAsset>();
                this.culture = null;
            } else
            {
                this.culture = culture;
            }
        }

        /// <summary>
        /// StringLocalizer adapter that is assigned to a specific Type
        /// </summary>
        public class Type : StringLocalizerAsset
        {
            public readonly System.Type type;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="stringLocalizer"></param>
            /// <param name="basename">Embed location, e.g. "Resources" folder in an assembly</param>
            /// <param name="location">Assembly name</param>
            /// <param name="culture"></param>
            public Type(IStringLocalizer stringLocalizer, System.Type type, CultureInfo culture, ILocalizationStringFormatParser valueParser = default) : base(stringLocalizer, culture, valueParser)
            {
                this.type = type ?? throw new ArgumentNullException(nameof(type));
                this.createFunc = ci => new Type(stringLocalizer.WithCulture(ci), type, ci);
            }
        }

        /// <summary>
        /// StringLocalizer adapter that is assigned to a specific Assembly + Embed location
        /// </summary>
        public class Location : StringLocalizerAsset
        {
            public readonly string location;
            public readonly string basename;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="stringLocalizer"></param>
            /// <param name="basename">Embed location, e.g. "Resources" folder in an assembly</param>
            /// <param name="location">Assembly name</param>
            /// <param name="culture"></param>
            public Location(IStringLocalizer stringLocalizer, string basename, string location, CultureInfo culture, ILocalizationStringFormatParser valueParser = default) : base(stringLocalizer, culture, valueParser)
            {
                this.basename = basename ?? throw new ArgumentNullException(nameof(basename));
                this.location = location ?? throw new ArgumentNullException(nameof(location));
                this.createFunc = ci => new Location(stringLocalizer.WithCulture(ci), basename, location, ci);
            }
        }

        /// <summary>
        /// Find a localizer that matches the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>localizer or null</returns>
        StringLocalizerAsset FindStringLocalizer(ILine key, CultureInfo key_culture)
        {
            // This adapter is not assigned with a culture
            if (culture == null)
            {
                if (key_culture != null && key_culture.Name != "") return culture_map.GetOrAdd(key_culture, createFunc);
                return this;
            }

            // Key's culture matches the assigned culture
            if (this.culture.Equals(key_culture)) return this;

            // Culture doesn't match
            return null;
        }

        public IFormulationString GetString(ILine key)
        {
            CultureInfo key_culture = key.GetCultureInfo();
            if (key_culture != null)
            {
                // Get-or-create culture specific adapter, use that
                if (this.culture == null)
                {
                    var localizer = FindStringLocalizer(key, key_culture);
                    if (localizer == null) return null;
                    if (localizer != this) return localizer.GetString(key);
                }

                // Culture mismatch
                if (this.culture!=null && !this.culture.Equals(key_culture)) return null;
            }

            // Parse basename/location/type from key
            ILineAssembly asmSectionToStrip;
            IAssetKeyResourceAssigned resSectionToStrip;
            ILineKeyType typeSectionToStrip;
            int x = key.FindResourceKeys(out asmSectionToStrip, out resSectionToStrip, out typeSectionToStrip);

            // If key has type, and this handler is also assigned to type, but they mismatch, don't strip the type name from key (below)
            if (x == 1)
            {
                if (this is Type type_assigned)
                {
                    // This adapter is assigned to a specific type.
                    // If a request is made to another type, if might be because it is needed in the name.
                    // So lets not strip the type part from the name we will build (below)
                    if (!typeSectionToStrip.Type.Equals(type_assigned.type)) typeSectionToStrip = null;
                }
                else
                {
                    // This adapter is not assigned to any Type. 
                    // If key has type hint, we can try to fetch for that full name.
                    // Therefore lets not strip type part from the name we will build (below)
                    typeSectionToStrip = null;
                }
            }

            // If key has basename/location, and its different than this handler, then return null.
            if (x == 2)
            {
                if (this is Location location_assigned)
                {
                    // This adapter is assigned for specific assembly/embed_basename.
                    // If key has these hints, and they mismatch, we cant retrieve for the key.
                    if (!asmSectionToStrip.GetParameterValue().Equals(location_assigned.location) || (!resSectionToStrip.GetParameterValue().Equals(location_assigned.basename))) return null;
                } else
                {
                    // This adapter is not assigned for any specific assembly/embed_basename.
                    // If key has these hints, we can try to fetch for that full name.
                    // Therefore lets not strip location part from the name (below).
                    asmSectionToStrip = null;
                    resSectionToStrip = null;
                }
            }

            // Build id
            // Strip: culture, and our typesection/asm section
            int length = 0;
            for(ILine k = key; k!=null; k=k.GetPreviousPart())
            {
                string value = k.GetParameterValue();
                if (k == typeSectionToStrip || k == asmSectionToStrip || k == resSectionToStrip) break;
                if (k is ILineCulture || string.IsNullOrEmpty(value)) continue;
                if (length > 0) length++;
                length += value.Length;
            }
            char[] chars = new char[length];
            int ix = length;
            for (ILine k = key; k != null; k = k.GetPreviousPart())
            {
                string value = k.GetParameterValue();
                if (k == typeSectionToStrip || k == asmSectionToStrip || k == resSectionToStrip) break;
                if (k is ILineCulture || string.IsNullOrEmpty(value)) continue;
                if (ix < length) chars[--ix] = '.';
                ix -= value.Length;
                value.CopyTo(0, chars, ix, value.Length);
            }
            string id = new String(chars);

            LocalizedString str = stringLocalizer[id];

            if (str.ResourceNotFound) return null;
            return ValueParser.Parse(str.Value);
        }

        public IEnumerable<KeyValuePair<string, IFormulationString>> GetStringLines(ILine key = null)
        {
            CultureInfo key_culture = key?.GetCultureInfo();
            IStringLocalizer localizer = key == null ? stringLocalizer : FindStringLocalizer(key, key_culture).stringLocalizer;
            if (localizer == null) return null;

            string prefix = null;
            if (this is Type type) prefix = type.type.FullName + ".";
            if (this is Location location) prefix = location.location + "." + location.basename;
            try
            {
                return localizer
                    .GetAllStrings(false)
                    ?.Where(str => !str.ResourceNotFound)
                    ?.Select(str => new KeyValuePair<string, IFormulationString>(prefix == null ? str.Name : prefix + str.Name, ValueParser.Parse(str.Value)))
                    ?.Where(kp => kp.Value != null)
                    ?.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<KeyValuePair<string, IFormulationString>> GetAllStringLines(ILine key = null)
        {
            return GetStringLines(key);
        }

        public IAsset Reload()
        {
            culture_map.Clear();
            return this;
        }

    }

    public static partial class MsLocalizationExtensions
    {
        public static IAsset ToAsset(this IStringLocalizer stringLocalizer)
            => new StringLocalizerAsset(stringLocalizer);
        public static IAssetSource ToSource(this IStringLocalizer stringLocalizer)
            => new StringLocalizerAsset(stringLocalizer).ToSource();


        /// <summary>
        /// Searches key for either 
        ///    <see cref="ILineKeyType"/> with type
        ///    <see cref="ILineAssembly"/> and <see cref="IAssetKeyResourceAssigned"/> with string, not type.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="basename"></param>
        /// <param name="location"></param>
        /// <returns>
        ///     0 found nothing
        ///     1 found type
        ///     2 found basename + location</returns>
        public static int FindResourceInfos(this ILine key, out Type type, out string basename, out string location)
        {
            ILineAssembly asmSection;
            IAssetKeyResourceAssigned resSection;
            ILineKeyType typeSection;
            int x = key.FindResourceKeys(out asmSection, out resSection, out typeSection);
            if (x == 1) { type = typeSection.Type; basename = null; location = null; }
            if (x == 2) { type = null; basename = typeSection.GetParameterValue(); location = asmSection.GetParameterValue(); }
            type = null; basename = null; location = null;
            return 0;
        }
        /// <summary>
        /// Searches key for either 
        ///    <see cref="ILineKeyType"/> with type
        ///    <see cref="ILineAssembly"/> and <see cref="IAssetKeyResourceAssigned"/> with string, not type.
        /// </summary>
        /// <returns>
        ///     0 found nothing
        ///     1 found typeSection
        ///     2 found asmSection + typeSection</returns>
        public static int FindResourceKeys(this ILine key, out ILineAssembly asmSection, out IAssetKeyResourceAssigned resSection, out ILineKeyType typeSection)
        {
            ILineAssembly _asmSection = null;
            IAssetKeyResourceAssigned _resSection = null;
            ILineKeyType _typeSection = null;
            for (ILine k = key; k != null; k = k.GetPreviousPart())
            {
                if (k is ILineAssembly __asmSection) _asmSection = __asmSection;
                else if (k is IAssetKeyResourceAssigned __resSection) _resSection = __resSection;
                else if (k is ILineKeyType __typeSection) _typeSection = __typeSection;
            }

            if (_asmSection != null && _resSection != null)
            {
                asmSection = _asmSection;
                resSection = _resSection;
                typeSection = null;
                return 2;
            }
            if (_typeSection != null && _typeSection.Type != null)
            {
                asmSection = null;
                resSection = null;
                typeSection = _typeSection;
                return 1;
            }

            asmSection = null;
            resSection = null;
            typeSection = null;
            return 0;
        }
    }
}

