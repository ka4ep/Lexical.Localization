// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /*
    /// <summary>
    /// Unicode CLDR v35 Plurality Rules.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    public class UnicodeCLDRv35 : PluralRuleSet
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
        /// Load embedded files and convert into <see cref="IPluralRules"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IPluralRules> LoadRules()
            => UnicodeCLDR.Load(UnicodeCLDR.ReadEmbedded("Lexical.Localization.Unicode.v35.plurals.xml").Root)
               .Concat(UnicodeCLDR.Load(UnicodeCLDR.ReadEmbedded("Lexical.Localization.Unicode.v35.ordinals.xml").Root));

        /// <summary>
        /// Create v35 of Unicode CLDR Plural rules.
        /// </summary>
        public UnicodeCLDRv35() : base(typeof(UnicodeCLDRv35).FullName, LoadRules())
        {
        }
    }

    /// <summary>
    /// Unicode CLDR latest version of plurality Rules.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    public class UnicodeCLDR : PluralRuleSet
    {
        /// <summary>
        /// Read embedded resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>documents</returns>
        public static XDocument ReadEmbedded(string resource)
        {
            using (Stream s = typeof(UnicodeCLDR).Assembly.GetManifestResourceStream(resource))
            {
                if (s == null) throw new InvalidOperationException($"Could not find {resource}");
                return XDocument.Load(s);
            }
        }

        /// <summary>
        /// Version 35 lazy loader.
        /// </summary>
        private static readonly Lazy<UnicodeCLDR> instance = new Lazy<UnicodeCLDR>();

        /// <summary>
        /// Unicode CLDR v35.
        /// 
        /// Reads embedded CLDR plural data files.
        /// Data files are licensed under <see href="https://unicode.org/repos/cldr/tags/release-35/unicode-license.txt"/>.
        /// </summary>
        public static UnicodeCLDR Instance => instance.Value;

        /// <summary>
        /// Create rules
        /// </summary>
        public UnicodeCLDR() : base(typeof(UnicodeCLDR).FullName)
        {
        }

        /// <summary>
        /// Load rules from <paramref name="rootElement"/>.
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static IEnumerable<IPluralRules> Load(XElement rootElement)
        {
            List<IPluralRule> list = new List<IPluralRule>();
            foreach (XElement pluralsElement in rootElement.Elements("plurals"))
            {
                string rulesName = pluralsElement.Attribute("type")?.Value;
                if (rulesName == null) throw new ArgumentException($"Xml element {pluralsElement} does not have expected attribute \"type\".");
                foreach (XElement pluralRulesElement in pluralsElement.Elements("pluralRules"))
                {
                    string localesText = pluralRulesElement.Attribute("locales")?.Value;
                    if (localesText == null) throw new ArgumentException($"Xml element {pluralRulesElement} does not have expected attribute \"locales\".");
                    string[] locales = localesText.Split(' ').Select(l => l.Replace('_', '-')).ToArray();

                    list.Clear();
                    int otherCaseIx = -1, zeroCaseIx = -1;
                    foreach (XElement pluralRuleElement in pluralRulesElement.Elements("pluralRule"))
                    {
                        string ruleName = pluralRuleElement.Attribute("count")?.Value;
                        if (ruleName == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected attribute \"count\".");
                        string text = pluralRuleElement.Value;
                        if (text == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected text.");

                        IPluralRuleExpression exp = PluralRuleParser.Parse<IPluralRuleExpression>(text);
                        IPluralRule rule;
                        if (exp.Rule == null && ruleName == "other") rule = PluralCase.True.other;
                        else rule = new PluralExpressionCase(ruleName, false, exp);
                        if (ruleName == "other") otherCaseIx = list.Count;
                        if (ruleName == "zero") zeroCaseIx = list.Count;
                        list.Add(rule);
                    }

                    // Move "other" rule as last
                    if (otherCaseIx>=0 && otherCaseIx<list.Count-1)
                    {
                        IPluralRule otherCase = list[otherCaseIx];
                        list.RemoveAt(otherCaseIx);
                        list.Add(otherCase);
                    }
                    // Add optional "zero" rule, if doesn't exist.
                    if (zeroCaseIx < 0) list.Insert(0, PluralCase.Zero._zero);

                    // Create rules
                    yield return new PluralRulesEvaluatable(list);
                }
            }
        }

    }*/
}
