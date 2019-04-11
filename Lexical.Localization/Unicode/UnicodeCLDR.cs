// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Unicode CLDR Plurality Rules.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    public class UnicodeCLDR : PluralityRuleMap
    {
        /// <summary>
        /// Version 35 lazy loader.
        /// </summary>
        private static readonly Lazy<IReadOnlyDictionary<CultureInfo, IPluralityRules>> v35 =
            new Lazy<IReadOnlyDictionary<CultureInfo, IPluralityRules>>(
                () => new UnicodeCLDR()
                      .Load(ReadEmbedded("Lexical.Localization.Unicode.v35.plurals.xml").Root)
                      .Load(ReadEmbedded("Lexical.Localization.Unicode.v35.ordinals.xml").Root));

        /// <summary>
        /// Read embedded resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>documents</returns>
        static XDocument ReadEmbedded(string resource)
        {
            using (Stream s = typeof(UnicodeCLDR).Assembly.GetManifestResourceStream(resource))
            {
                if (s == null) throw new InvalidOperationException($"Could not find {resource}");
                return XDocument.Load(s);
            }
        }

        /// <summary>
        /// Unicode CLDR v35.
        /// 
        /// Reads embedded cldr data files.
        /// Data files are licensed under <see href="https://unicode.org/repos/cldr/tags/release-35/unicode-license.txt"/>.
        /// </summary>
        public static IReadOnlyDictionary<CultureInfo, IPluralityRules> V35 => v35.Value;

        /// <summary>
        /// Newest Unicode CLDR available
        /// </summary>
        public static IReadOnlyDictionary<CultureInfo, IPluralityRules> Instance => v35.Value;

        /// <summary>
        /// Create rules
        /// </summary>
        /// <param name="rules"></param>
        public UnicodeCLDR()
        {
        }

        /// <summary>
        /// Load rules from <paramref name="rootElement"/>.
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public UnicodeCLDR Load(XElement rootElement)
        {
            foreach (XElement pluralsElement in rootElement.Elements("plurals"))
            {
                string type = pluralsElement.Attribute("type")?.Value;
                if (type == null) throw new ArgumentException($"Xml element {pluralsElement} does not have expected attribute \"type\".");
                foreach (XElement pluralRulesElement in pluralsElement.Elements("pluralRules"))
                {
                    string localesText = pluralRulesElement.Attribute("locales")?.Value;
                    if (localesText == null) throw new ArgumentException($"Xml element {pluralRulesElement} does not have expected attribute \"locales\".");
                    string[] locales = localesText.Split(' ').Select(l => l.Replace('_', '-')).ToArray();

                    PluralRules pluralRules = new PluralRules();
                    foreach (XElement pluralRuleElement in pluralRulesElement.Elements("pluralRule"))
                    {
                        string countText = pluralRuleElement.Attribute("count")?.Value;
                        if (countText == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected attribute \"count\".");
                        string text = pluralRuleElement.Value;
                        if (text == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected text.");

                        PluralRule pluralRule = new PluralRule(countText, text);
                    }

                }
            }
            return this;
        }

    }
}
