// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections.Generic;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// See subinterfaces:
    /// <list type="bullet">
    /// <item><see cref="IPluralRulesEvaluatable"/>Evaluates number</item>
    /// <item><see cref="IPluralRulesCaseMap"/>Cases in map</item>
    /// <item><see cref="IPluralCategory"/>Lists cases</item>
    /// </list>
    /// </summary>
    public interface IPluralRules
    {
    }

    /// <summary>
    /// Plurality category is function that can be attached to an argument to describe possible
    /// cases of declination of sentences.
    /// 
    /// For example formulation string "There are {plural:0} cats", describes that there will be
    /// different declination cases of the sentence depending on the number in the argument.
    /// </summary>
    public interface IPluralCategory : IPluralRules
    {
        /// <summary>
        /// (Optional) Category name, e.g. "plural" "ordinal", "optional"
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Possible cases.
        /// </summary>
        IPluralCase[] Cases { get; }
    }

    /// <summary>
    /// Plurality rules when conjugating and declinating sentences that conform to one argument.
    /// 
    /// The instance is language specific.
    /// 
    /// "plural"
    ///      Case "":         "{cardinal:0} car(s)"
    ///      Case "N0:one":   "a car"                       when {0}=1
    ///      Case "N0:other": "{0} cars"                    when {0}=5
    ///      
    /// "ordinal" Plurality rules for languages where the sentences is conjugated according to ordinality number (e.g. 1st)
    ///      Case "":         "Ta {ordinal:0}:a svängen till höger"
    ///      Case "N0:one":   "Ta {0}:a svängen till höger" when {0}=1
    ///      Case "N0:other": "Ta {1}:e svängen till höger" when {0}=3
    /// 
    /// "optional" plurality rules for formulating sentences for possibility of null value.
    ///      Case "":        "{optional:0} cars"
    ///      Case "N0:null": "No cars"                      when {0}=null
    /// </summary>
    public interface IPluralRulesCaseMap : IPluralRules, IReadOnlyDictionary<string, IPluralRule>
    {
    }

    /// <summary>
    /// Evaluates a <see cref="IPluralNumber"/> 
    /// </summary>
    public interface IPluralRulesEvaluatable : IPluralRules
    {
        /// <summary>
        /// Tests whether case(s) matches to the value. 
        /// 
        /// Sometimes there are optional cases. They are returned first, the mandatory cases are last indices of the result array.
        /// 
        /// For example, "{plural:0} cars" for value '0' returns cases "Zero" and "Other".
        /// Zero is optional, and the translator can choose whether to use "Zero" string or not.
        ///    "N0:Zero"="No cars"
        ///    "N0:Other="{0} cars"
        ///    
        /// If "N0:Zero" is not found, then "N0:Other" is used.
        /// </summary>
        /// <param name="number">numeric and text representation of numberic value</param>
        /// <returns>Matching cases, first ones are optional, the last one is always mandatory (and only mandatory). If null, then rule is not completely configured.</returns>
        IPluralRule[] Evaluate(IPluralNumber number);
    }

    /// <summary>
    /// Key where an explicit <see cref="IPluralRules"/> can be assigned.
    /// </summary>
    public interface IPluralRulesAssignableKey : ILocalizationKey
    {
        /// <summary>
        /// Assign explicit rules.
        /// </summary>
        /// <param name="ruleSets"></param>
        /// <returns></returns>
        IPluralRulesAssignedKey PluralityRules(IPluralRules ruleSets);
    }

    /// <summary>
    /// A key where an explicit <see cref="IPluralRules"/> is assigned.
    /// </summary>
    public interface IPluralRulesAssignedKey : ILocalizationKey
    {
        /// <summary>
        /// Get (possibly) assigned rules.
        /// </summary>
        IPluralRules PluralityRules { get; }
    }

    /// <summary>
    /// Key where an name of <see cref="IPluralRules"/> can be assigned. 
    /// 
    /// The <see cref="IPluralRules"/> is resolved from <see cref="IPluralRuleSet"/> by the assigned name.
    /// </summary>
    public interface IPluralRulesNameAssignableKey : ILocalizationKey
    {
        /// <summary>
        /// Assign rule-sets.
        /// </summary>
        /// <param name="ruleSets"></param>
        /// <returns></returns>
        IPluralRulesNameAssignedKey PluralityRules(String ruleSets);
    }
    
    /// <summary>
    /// A key where an explicit <see cref="IPluralRules"/> is assigned.
    /// 
    /// The parameter name of the key is "PluralityRules", and the example values are "fi", "en" for culture names.
    /// </summary>
    public interface IPluralRulesNameAssignedKey : ILocalizationKey, IAssetKeyParameterAssigned
    {
        /// <summary>
        /// Get (possibly) assigned rule-sets.
        /// </summary>
        String PluralityRules { get; }
    }

    /// <summary>
    /// Plural rules expression.
    /// 
    /// e.g. "§one i = 1 and v = 0 @integer 1 §other @integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …".
    /// 
    /// or 
    /// "#en #fi #sw §one i = 1 and v = 0 @integer 1 §other @integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …".
    /// </summary>
    public interface IPluralRulesExpression : IExpression, IPluralRules
    {
        /// <summary>
        /// (Optional) Culture names
        /// 
        /// e.g. "#en #fi #sw"
        /// </summary>
        String[] Names { get; }

        /// <summary>
        /// Rules
        /// </summary>
        IPluralRuleExpression[] Rules { get; }
    }

}
