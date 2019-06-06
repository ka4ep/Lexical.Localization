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

namespace Lexical.Localization.Asset
{
    using Lexical.Localization.StringFormat;
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
        IStringAsset, 
        IStringAssetStringLinesEnumerable,  // Doesn't work
        IAssetReloadable
    {
        /// <summary>
        /// Source string localizer
        /// </summary>
        public readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// Selected culture of localizer
        /// </summary>
        public readonly CultureInfo culture;

        /// <summary>
        /// Value string parser
        /// </summary>
        public readonly IStringFormatParser ValueParser;

        /// <summary>
        /// 
        /// </summary>
        protected ConcurrentDictionary<CultureInfo, StringLocalizerAsset> culture_map;

        /// <summary>
        /// 
        /// </summary>
        protected Func<CultureInfo, StringLocalizerAsset> createFunc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringLocalizer"></param>
        /// <param name="valueParser"></param>
        public StringLocalizerAsset(IStringLocalizer stringLocalizer, IStringFormatParser valueParser = default)
        {
            this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            this.culture_map = new ConcurrentDictionary<CultureInfo, StringLocalizerAsset>();
            this.createFunc = ci => new StringLocalizerAsset(stringLocalizer.WithCulture(ci), ci);
            this.ValueParser = ValueParser ?? CSharpFormat.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringLocalizer"></param>
        /// <param name="culture"></param>
        /// <param name="valueParser"></param>
        public StringLocalizerAsset(IStringLocalizer stringLocalizer, CultureInfo culture, IStringFormatParser valueParser = default)
        {
            this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            this.createFunc = ci => new StringLocalizerAsset(stringLocalizer.WithCulture(ci), ci);
            this.ValueParser = ValueParser ?? CSharpFormat.Default;
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
            /// <summary>
            /// The type this is string localizer for.
            /// </summary>
            public readonly System.Type type;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="stringLocalizer"></param>
            /// <param name="type"></param>
            /// <param name="culture"></param>
            /// <param name="valueParser"></param>
            public Type(IStringLocalizer stringLocalizer, System.Type type, CultureInfo culture, IStringFormatParser valueParser = default) : base(stringLocalizer, culture, valueParser)
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
            /// <summary>
            /// Location or assembly
            /// </summary>
            public readonly string location;

            /// <summary>
            /// Basename (embedded resource prefix)
            /// </summary>
            public readonly string basename;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="stringLocalizer"></param>
            /// <param name="basename">Embed location, e.g. "Resources" folder in an assembly</param>
            /// <param name="location">Assembly name</param>
            /// <param name="culture"></param>
            /// <param name="valueParser"></param>
            public Location(IStringLocalizer stringLocalizer, string basename, string location, CultureInfo culture, IStringFormatParser valueParser = default) : base(stringLocalizer, culture, valueParser)
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
        /// <param name="key_culture"></param>
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

        /// <summary>
        /// Match <paramref name="key"/> to content in <see cref="IStringLocalizer"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ILine GetLine(ILine key)
        {
            CultureInfo key_culture;
            if (key.TryGetCultureInfo(out key_culture))
            {
                // Get-or-create culture specific adapter, use that
                if (this.culture == null)
                {
                    var localizer = FindStringLocalizer(key, key_culture);
                    if (localizer == null) return null;
                    if (localizer != this) return localizer.GetLine(key);
                }

                // Culture mismatch
                if (this.culture!=null && !this.culture.Equals(key_culture)) return null;
            }

            // Parse basename/location/type from key
            string _location, _basename, _type;
            int x = key.FindResourceInfos(out _type, out _basename, out _location);

            // If key has type, and this handler is also assigned to type, but they mismatch, don't strip the type name from key (below)
            if (x == 1)
            {
                if (this is Type type_assigned)
                {
                    // This adapter is assigned to a specific type.
                    // If a request is made to another type, if might be because it is needed in the name.
                    // So lets not strip the type part from the name we will build (below)
                    if (!_type.Equals(type_assigned.type.FullName)) _type = null;
                }
                else
                {
                    // This adapter is not assigned to any Type. 
                    // If key has type hint, we can try to fetch for that full name.
                    // Therefore lets not strip type part from the name we will build (below)
                    _type = null;
                }
            }

            // If key has basename/location, and its different than this handler, then return null.
            if (x == 2)
            {
                if (this is Location location_assigned)
                {
                    // This adapter is assigned for specific assembly/embed_basename.
                    // If key has these hints, and they mismatch, we cant retrieve for the key.
                    if (!_location.Equals(location_assigned.location) || (!_basename.Equals(location_assigned.basename))) return null;
                } else
                {
                    // This adapter is not assigned for any specific assembly/embed_basename.
                    // If key has these hints, we can try to fetch for that full name.
                    // Therefore lets not strip location part from the name (below).
                    _location = null;
                    _basename = null;
                }
            }

            // Build id
            // Strip: culture, and our typesection/asm section
            int length = 0;
            for(ILine k = key; k!=null; k=k.GetPreviousPart())
            {
                if (k is ILineParameterEnumerable lineParameters)
                    foreach(var lineParameter in lineParameters)
                    {
                        if (lineParameter.ParameterValue == _type || lineParameter.ParameterValue == _location || lineParameter.ParameterValue == _basename) continue; // break; ?
                        string value = lineParameter.ParameterValue;
                        if (lineParameter.ParameterName == "Culture" || string.IsNullOrEmpty(value)) continue;
                        if (length > 0) length++;
                        length += value.Length;
                    }
                if (k is ILineParameter parameter)
                {
                    if (parameter.ParameterValue == _type || parameter.ParameterValue == _location || parameter.ParameterValue == _basename) continue; // break; ?
                    string value = parameter.ParameterValue;
                    if (parameter.ParameterName == "Culture" || string.IsNullOrEmpty(value)) continue;
                    if (length > 0) length++;
                    length += value.Length;
                }
            }
            char[] chars = new char[length];
            int ix = length;
            for (ILine k = key; k != null; k = k.GetPreviousPart())
            {
                if (k is ILineParameterEnumerable lineParameters)
                    foreach (var lineParameter in lineParameters)
                    {
                        if (lineParameter.ParameterValue == _type || lineParameter.ParameterValue == _location || lineParameter.ParameterValue == _basename) continue; // break; ?
                        string value = lineParameter.ParameterValue;
                        if (lineParameter.ParameterName == "Culture" || string.IsNullOrEmpty(value)) continue;
                        if (ix < length) chars[--ix] = '.';
                        ix -= value.Length;
                        value.CopyTo(0, chars, ix, value.Length);
                    }
                if (k is ILineParameter parameter)
                {
                    if (parameter.ParameterValue == _type || parameter.ParameterValue == _location || parameter.ParameterValue == _basename) continue; // break; ?
                    string value = parameter.ParameterValue;
                    if (parameter.ParameterName == "Culture" || string.IsNullOrEmpty(value)) continue;
                    if (ix < length) chars[--ix] = '.';
                    ix -= value.Length;
                    value.CopyTo(0, chars, ix, value.Length);
                }
            }
            string id = new String(chars);

            LocalizedString str = stringLocalizer[id];

            if (str.ResourceNotFound) return null;
            return key.String(ValueParser.Parse(str.Value));
        }

        /// <summary>
        /// Get all strings in the asset
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> GetStringLines(ILine key = null)
        {
            CultureInfo key_culture = null;
            key.TryGetCultureInfo(out key_culture);
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
                    ?.Select(str => new KeyValuePair<string, IString>(prefix == null ? str.Name : prefix + str.Name, ValueParser.Parse(str.Value)))
                    ?.Where(kp => kp.Value != null)
                    ?.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get all strings in the asset
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> GetAllStringLines(ILine key = null)
        {
            return GetStringLines(key);
        }

        /// <summary>
        /// Flush cache.
        /// </summary>
        /// <returns></returns>
        public IAsset Reload()
        {
            culture_map.Clear();
            return this;
        }

    }

    public static partial class MsLocalizationExtensions
    {
        /// <summary>
        /// Convert to asset
        /// </summary>
        /// <param name="stringLocalizer"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IStringLocalizer stringLocalizer)
            => new StringLocalizerAsset(stringLocalizer);

        /// <summary>
        /// Convert to asset source.
        /// </summary>
        /// <param name="stringLocalizer"></param>
        /// <returns></returns>
        public static IAssetSource ToSource(this IStringLocalizer stringLocalizer)
            => new StringLocalizerAsset(stringLocalizer).ToSource();


        /// <summary>
        /// Searches key for either:
        /// <list type="bullet">
        ///     <item><see cref="ILineType"/></item>
        ///     <item>"BaseName" and "Location"</item>
        ///     <item>"BaseName" and "Assembly"</item>
        /// </list>
        /// </summary>
        /// <returns>
        ///     0 found nothing
        ///     1 found type
        ///     2 found basename + location
        /// </returns>
        public static int FindResourceInfos(this ILine line, out string type, out string baseName, out string location)
        {
            string _location = null, _basename = null;
            string _type = null;
            for (ILine l = line; l != null; l=l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                    foreach(var parameter in lineParameters)
                    {
                        if (parameter is ILineType lineType && lineType.Type != null) _type = lineType.Type.FullName;
                        else if (parameter.ParameterName == "Type" && parameter.ParameterValue != null) _type = parameter.ParameterValue;
                        else if ((parameter.ParameterName == "Assembly" || parameter.ParameterName == "Location") && parameter.ParameterValue!= null) _location = parameter.ParameterValue;
                        else if (parameter.ParameterName == "BaseName" && parameter.ParameterValue != null) _basename = parameter.ParameterValue;
                    }
                else if (l is ILineParameter parameter)
                {
                    if (parameter is ILineType lineType && lineType.Type != null) _type = lineType.Type.FullName;
                    else if (parameter.ParameterName == "Type" && parameter.ParameterValue != null) _type = parameter.ParameterValue;
                    else if ((parameter.ParameterName == "Assembly" || parameter.ParameterName == "Location") && parameter.ParameterValue != null) _location = parameter.ParameterValue;
                    else if (parameter.ParameterName == "BaseName" && parameter.ParameterValue != null) _basename = parameter.ParameterValue;
                }
            }


            if (_location != null && _basename != null)
            {
                location = _location;
                baseName = _basename;
                type = null;
                return 2;
            }
            if (_type != null)
            {
                location = null;
                baseName = null;
                type = _type;
                return 1;
            }

            location = null;
            baseName = null;
            type = null;
            return 0;
        }
    }
}


