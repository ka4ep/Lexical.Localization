// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Utility that parse Unicode CLDR plurality XML-files into <see cref="IPluralRules"/>.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    public class UnicodeCLDRXml
    {
        /// <summary>
        /// Read embedded resource.
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="resource"></param>
        /// <returns>documents</returns>
        public static XDocument ReadEmbedded(Assembly asm, string resource)
        {
            if (asm == null) asm = typeof(UnicodeCLDRXml).Assembly;
            using (Stream s = asm.GetManifestResourceStream(resource))
            {
                if (s == null) throw new InvalidOperationException($"Could not find {resource}");
                return XDocument.Load(s);
            }
        }

        /// <summary>
        /// Load rules from <paramref name="rootElement"/>.
        /// </summary>
        /// <param name="rootElement"></param>
        /// <param name="ruleset">The "RuleSet" parameter that is added to every instantiated <see cref="IPluralRule"/></param>
        /// <param name="addOptionalZero">If true, then always adds "zero" case (as optional) if it didn't exist.</param>
        /// <returns></returns>
        public static IEnumerable<IPluralRule> Read(XElement rootElement, string ruleset, bool addOptionalZero)
        {
            List<(string, IPluralRuleExpression)> list = new List<(string, IPluralRuleExpression)>();
            foreach (XElement pluralsElement in rootElement.Elements("plurals"))
            {
                string category = pluralsElement.Attribute("type")?.Value;
                if (category == null) throw new ArgumentException($"Xml element {pluralsElement} does not have expected attribute \"type\".");
                foreach (XElement pluralRulesElement in pluralsElement.Elements("pluralRules"))
                {
                    string culturesText = pluralRulesElement.Attribute("locales")?.Value;
                    if (culturesText == null) throw new ArgumentException($"Xml element {pluralRulesElement} does not have expected attribute \"locales\".");
                    string[] cultures = culturesText.Split(' ').Select(l => l.Replace('_', '-')).ToArray();

                    // Read expressions into list
                    list.Clear();
                    int otherCaseIx = -1, zeroCaseIx = -1;
                    foreach (XElement pluralRuleElement in pluralRulesElement.Elements("pluralRule"))
                    {
                        string @case = pluralRuleElement.Attribute("count")?.Value;
                        if (@case == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected attribute \"count\".");
                        string text = pluralRuleElement.Value;
                        if (text == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected text.");

                        IEnumerable<IPluralRuleExpression> parser = PluralRuleExpressionParser.CreateParser(text);
                        if (parser == null) throw new ArgumentException($"Could not parse expression \"{text}\"");
                        int expCountInText = 0;
                        foreach (IPluralRuleExpression exp in parser)
                        {
                            list.Add((@case, exp));
                            if (@case == "other") otherCaseIx = list.Count;
                            if (@case == "zero") zeroCaseIx = list.Count;
                            expCountInText++;
                        }
                        if (expCountInText == 0) list.Add((@case, null));
                        if (expCountInText > 1) throw new ArgumentException($"Got more than one expression for case \"{@case}\"");
                    }

                    // Move "other" rule to as last index
                    if (otherCaseIx >= 0 && otherCaseIx < list.Count - 1)
                    {
                        var otherCase = list[otherCaseIx];
                        list.RemoveAt(otherCaseIx);
                        list.Add(otherCase);
                    }

                    // Now instantiate rules for each culture
                    foreach (string culture in cultures)
                    {
                        // Add optional "zero" rule, if doesn't exist.
                        if (addOptionalZero && zeroCaseIx < 0)
                        {
                            PluralRuleInfo info = new PluralRuleInfo(ruleset, category, culture, "zero", 1);
                            yield return new PluralRule.Zero(info);
                        }
                        // Add mandatory rules
                        foreach ((string @case, IPluralRuleExpression exp) in list)
                        {
                            // Info
                            PluralRuleInfo info = new PluralRuleInfo(ruleset, category, culture, @case, 0);
                            // Return "other" case as a class that is easier to evaluate (always true)
                            if (exp.Rule == null && @case == "other") yield return new PluralRule.TrueWithExpression(info, exp.Infos, exp.Rule, exp.Samples);
                            // Return rule that evaluates expression
                            else yield return new PluralRule.Expression(info, exp.Infos, exp.Rule, exp.Samples);
                        }
                    }
                }
            }
        }

    }
}
