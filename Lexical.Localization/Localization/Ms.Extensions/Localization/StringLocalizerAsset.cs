// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lexical.Localization.Ms.Extensions
{
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
        ILocalizationStringCollection,  // Doesn't work
        IAssetReloadable
    {
        public readonly IStringLocalizer stringLocalizer;
        public readonly CultureInfo culture;
        protected ConcurrentDictionary<CultureInfo, StringLocalizerAsset> culture_map;
        protected Func<CultureInfo, StringLocalizerAsset> createFunc;

        public StringLocalizerAsset(IStringLocalizer stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            this.culture_map = new ConcurrentDictionary<CultureInfo, StringLocalizerAsset>();
            this.createFunc = ci => new StringLocalizerAsset(stringLocalizer.WithCulture(ci), ci);
        }
        public StringLocalizerAsset(IStringLocalizer stringLocalizer, CultureInfo culture)
        {
            this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            this.createFunc = ci => new StringLocalizerAsset(stringLocalizer.WithCulture(ci), ci);
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
            public Type(IStringLocalizer stringLocalizer, System.Type type, CultureInfo culture) : base(stringLocalizer, culture)
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
            public Location(IStringLocalizer stringLocalizer, string basename, string location, CultureInfo culture) : base(stringLocalizer, culture)
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
        StringLocalizerAsset FindStringLocalizer(IAssetKey key, CultureInfo key_culture)
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

        public string GetString(IAssetKey key)
        {
            CultureInfo key_culture = key.FindCulture();
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
            IAssetKeyAssemblySection asmSectionToStrip;
            IAssetKeyResourceSection resSectionToStrip;
            IAssetKeyTypeSection typeSectionToStrip;
            int x = key.FindResourceSections(out asmSectionToStrip, out resSectionToStrip, out typeSectionToStrip);

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
                    if (!asmSectionToStrip.Name.Equals(location_assigned.location) || (!resSectionToStrip.Name.Equals(location_assigned.basename))) return null;
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
            for(IAssetKey k = key; k!=null; k=k.GetPreviousKey())
            {
                if (k == typeSectionToStrip || k == asmSectionToStrip || k == resSectionToStrip) break;
                if (k is ILocalizationKeyCultured || k is IAssetKeyNonCanonicallyCompared || string.IsNullOrEmpty(k.Name)) continue;
                if (length > 0) length++;
                length += k.Name.Length;
            }
            char[] chars = new char[length];
            int ix = length;
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
            {
                if (k == typeSectionToStrip || k == asmSectionToStrip || k == resSectionToStrip) break;
                if (k is ILocalizationKeyCultured || k is IAssetKeyNonCanonicallyCompared || string.IsNullOrEmpty(k.Name)) continue;
                if (ix < length) chars[--ix] = '.';
                ix -= k.Name.Length;
                k.Name.CopyTo(0, chars, ix, k.Name.Length);
            }
            string id = new String(chars);

            LocalizedString str = stringLocalizer[id];

            if (str.ResourceNotFound) return null;
            return str.Value;
        }
        
        public IEnumerable<KeyValuePair<string, string>> GetAllStrings(IAssetKey key = null)
        {
            CultureInfo key_culture = key?.FindCulture();
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
                    ?.Select(str => new KeyValuePair<string, string>(prefix == null ? str.Name : prefix + str.Name, str.Value))
                    ?.Where(kp => kp.Value != null)
                    ?.ToArray();
            } catch (Exception)
            {
                return null;
            }
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
    }
}

