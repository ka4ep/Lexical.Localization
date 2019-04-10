// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Unicode rules.
    /// </summary>
    public class UnicodePluralityRules : PluralityRuleMap
    {
        /// <summary>
        /// Version 35 lazy loader.
        /// </summary>
        private static readonly Lazy<IReadOnlyDictionary<string, IPluralityRules>> v35 =
            new Lazy<IReadOnlyDictionary<string, IPluralityRules>>(
                () => new UnicodePluralityRules(
                        ReadEmbedded("Lexical.Localization.Unicode.v35.plurals.xml"),
                        ReadEmbedded("Lexical.Localization.Unicode.v35.ordinals.xml")));

        /// <summary>
        /// Read embedded resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>documents</returns>
        static XDocument ReadEmbedded(string resource)
        {
            using (Stream s = typeof(UnicodePluralityRules).Assembly.GetManifestResourceStream(resource))
            {
                if (s == null) throw new InvalidOperationException($"Could not find {resource}");
                return XDocument.Load(s);
            }
        }

        /// <summary>
        /// Unicode CLDR v35 
        /// </summary>
        public static IReadOnlyDictionary<string, IPluralityRules> V35 => v35.Value;

        /// <summary>
        /// Newest Unicode CLDR
        /// </summary>
        public static IReadOnlyDictionary<string, IPluralityRules> Instance => v35.Value;

        /// <summary>
        /// Create rules
        /// </summary>
        /// <param name="pluralRules"></param>
        /// <param name="ordinalRules"></param>
        public UnicodePluralityRules(XDocument pluralRules, XDocument ordinalRules)
        {
            // TODO
        }

    }
}
