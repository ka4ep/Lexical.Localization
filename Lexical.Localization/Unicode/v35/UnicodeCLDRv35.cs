// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Unicode CLDR v35 Plurality Rules.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    public class UnicodeCLDRv35 : PluralRulesCached
    {
        /// <summary>
        /// Version 35 lazy loader.
        /// </summary>
        private static readonly Lazy<UnicodeCLDRv35> instance = new Lazy<UnicodeCLDRv35>();

        /// <summary>
        /// Unicode CLDR v35.
        /// 
        /// Reads embedded CLDR plural data files.
        /// Data files are licensed under <see href="https://unicode.org/repos/cldr/tags/release-35/unicode-license.txt"/>.
        /// </summary>
        public static UnicodeCLDRv35 Instance => instance.Value;

        /// <summary>
        /// Read "plurals.xml"
        /// </summary>
        /// <returns></returns>
        public static XDocument ReadPlurals()
            => UnicodeCLDRXml.ReadEmbedded(typeof(UnicodeCLDRv35).Assembly, "Lexical.Localization.Unicode.v35.plurals.xml");

        /// <summary>
        /// Read "ordinals.xml"
        /// </summary>
        /// <returns></returns>
        public static XDocument ReadOrdinals()
            => UnicodeCLDRXml.ReadEmbedded(typeof(UnicodeCLDRv35).Assembly, "Lexical.Localization.Unicode.v35.ordinals.xml");

        /// <summary>
        /// RuleSet name
        /// </summary>
        public const string RuleSet = "Unicode.CLDRv35";

        /// <summary>
        /// Load embedded files and convert into <see cref="IPluralRules"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IPluralRule> ListRules()
            => UnicodeCLDRXml.Read(ReadPlurals().Root, RuleSet, true)
               .Concat(UnicodeCLDRXml.Read(ReadOrdinals().Root, RuleSet, false));

        /// <summary>
        /// Create unicode rules
        /// </summary>
        public UnicodeCLDRv35() : base(ListRules())
        {
        }
    }
}
