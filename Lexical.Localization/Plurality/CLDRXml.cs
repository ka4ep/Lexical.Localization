// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Unicode CLDR v35 Plurality Rules reader.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    public class CLDRXml : IEnumerable<IPluralRule>
    {
        /// <summary>
        /// RuleSet name
        /// </summary>
        public readonly string RuleSet;

        /// <summary>
        /// Document reader
        /// </summary>
        protected IEnumerable<IPluralRule> ruleReader;

        /// <summary>
        /// Create cldr reader that (re-)reads from embedded resources.
        /// </summary>
        public CLDRXml(string ruleSet, Assembly assembly, string pluralsEmbeddedResource, string ordinalsEmbeddedResource)
        {
            this.RuleSet = ruleSet ?? throw new ArgumentNullException(nameof(ruleSet));
            this.ruleReader = Read(new EmbeddedXmlReader(assembly, pluralsEmbeddedResource), RuleSet, true, true)
                    .Concat(Read(new EmbeddedXmlReader(assembly, ordinalsEmbeddedResource), RuleSet, false, true));
        }

        /// <summary>
        /// Create cldr reader that (re-)reads from external resources.
        /// </summary>
        /// <param name="ruleSet">"RuleSet" parameter for <see cref="PluralRuleInfo"/></param>
        /// <param name="ordinalsFile"></param>
        /// <param name="pluralsFile"></param>
        public CLDRXml(string ruleSet, string pluralsFile, string ordinalsFile)
        {
            ruleReader = Read(new FileXmlReader(pluralsFile), RuleSet, true, true)
                    .Concat(Read(new FileXmlReader(ordinalsFile), RuleSet, false, true));
        }

        /// <summary>
        /// Load rules from <paramref name="rootElements"/>.
        /// </summary>
        /// <param name="rootElements"></param>
        /// <param name="ruleset">The "RuleSet" parameter that is added to every instantiated <see cref="IPluralRule"/></param>
        /// <param name="addOptionalZero">If true, then always adds "zero" case (as optional) if it didn't exist.</param>
        /// <param name="addOptionalOne">If true, then always adds "one" case (as optional) if it didn't exist.</param>
        /// <returns></returns>
        public static IEnumerable<IPluralRule> Read(IEnumerable<XElement> rootElements, string ruleset, bool addOptionalZero, bool addOptionalOne)
        {
            List<(string, IPluralRuleExpression)> list = new List<(string, IPluralRuleExpression)>();
            foreach (XElement rootElement in rootElements)
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
                        int otherCaseIx = -1, zeroCaseIx = -1, oneCaseIx = -1;
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
                                if (@case == "one") oneCaseIx = list.Count;
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
                            // Add optional "one" rule, if doesn't exist.
                            if (addOptionalOne && oneCaseIx < 0)
                            {
                                PluralRuleInfo info = new PluralRuleInfo(ruleset, category, culture, "one", 1);
                                yield return new PluralRule.One(info);
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

        /// <summary>
        /// Read rules from embedded file.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IPluralRule> GetEnumerator()
            => ruleReader.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ruleReader.GetEnumerator();

    }

    /// <summary>
    /// Embedded resource xml document (root) reader.
    /// </summary>
    public class EmbeddedXmlReader : IEnumerable<XElement>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly Assembly Assembly;

        /// <summary>
        /// 
        /// </summary>
        public readonly string ResourceName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        public EmbeddedXmlReader(Assembly assembly, string resourceName)
        {
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        }

        /// <summary>
        /// Read xml document's root
        /// </summary>
        /// <returns></returns>
        public IEnumerator<XElement> GetEnumerator()
        {
            using (Stream s = Assembly.GetManifestResourceStream(ResourceName))
            {
                if (s == null) throw new InvalidOperationException($"Could not find {ResourceName}");
                yield return XDocument.Load(s).Root;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = Assembly.GetManifestResourceStream(ResourceName))
            {
                if (s == null) throw new InvalidOperationException($"Could not find {ResourceName}");
                yield return XDocument.Load(s).Root;
            }
        }
    }

    /// <summary>
    /// Embedded resource xml document (root) reader.
    /// </summary>
    public class FileXmlReader : IEnumerable<XElement>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly String FilePath;

        /// <summary>
        /// 
        /// </summary>
        public readonly string ResourceName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public FileXmlReader(string filePath)
        {
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        /// <summary>
        /// Read xml document's root.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<XElement> GetEnumerator()
        {
            yield return XDocument.Load(FilePath).Root;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return XDocument.Load(FilePath).Root;
        }
    }


}
