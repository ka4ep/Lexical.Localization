// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Plurality constants.
    /// </summary>
    public static class Plurality 
    {
        public const int MAX_NUMERIC_ARGUMENTS_TO_PERMUTATE = 2;

        public const string Zero = "Zero";
        public const string One = "One";
        public const string Plural = "Plural";

        public const string plural = "plural";
        public const string ordinal = "ordinal";
        public const string range = "range";
        public const string optional = "optional";

    }

    /// <summary>
    /// Map that contains plurality rules.
    /// 
    /// The key is ISO 639-1 (two character) or ISO 639-2 (three character) language code.
    /// </summary>
    public interface IPluralizationRuleSetMap : IReadOnlyDictionary<string, IPluralityRuleSet>
    {
    }

    /// <summary>
    /// A set of plurality rules
    /// </summary>
    public interface IPluralityRuleSet
    {
        /// <summary>
        /// Get rules by plurality category. 
        /// 
        /// Known categories:
        /// <list type="table">
        /// <item>plural</item>
        /// <item>ordinal</item>
        /// <item>range</item>
        /// <item>optional</item>
        /// </list>
        /// </summary>
        /// <param name="pluralityCategory"></param>
        /// <returns>rules or null if not found</returns>
        IPluralityRules this[string pluralityCategory] { get; }
    }

    /// <summary>
    /// Root interface for classes that provide plurality rules. 
    /// 
    /// Classes implement one or more sub-interface:
    /// <list type="bull">
    /// <item><see cref="IPluralityRulesOneArgument"/></item>
    /// <item><see cref="IPluralityRulesTwoArguments"/></item>
    /// </list>
    /// </summary>
    public interface IPluralityRules
    {
        /// <summary>
        /// Category name, e.g. "plural" "ordinal", "range", "optional"
        /// </summary>
        String Category { get; }
    }

    /// <summary>
    /// Plurality rules when conjugating and declinating sentences that conform to one argument.
    /// 
    /// The instance is language specific.
    /// 
    /// "plural"
    ///      Case "":         "{plural:0} car(s)"
    ///      Case "N0:One":   "a car"                       when {0}=1
    ///      Case "N0:Other": "{0} cars"                    when {0}=5
    ///      
    /// "ordinal" Plurality rules for languages where the sentences is conjugated according to ordinality number (e.g. 1st)
    ///      Case "":         "Ta {ordinal:0}:a svängen till höger"
    ///      Case "N0:One":   "Ta {0}:a svängen till höger" when {0}=1
    ///      Case "N0:Other": "Ta {1}:e svängen till höger" when {0}=3
    /// 
    /// "optional" plurality rules for formulating sentences for possibility of null value.
    ///      Case "":        "{optional:0} cars"
    ///      Case "N0:Null": "No cars"                      when {0}=null
    /// </summary>
    public interface IPluralityRulesOneArgument : IPluralityRules
    {
        /// <summary>
        /// A group of cses that must all be supplied with language string when plurality is supported.
        /// 
        /// Or null if this plurality rules cannot provide plural rules.
        /// 
        /// Cases are language specific, for example english language has "One" and "Other".
        /// </summary>
        /// <returns>rules</returns>
        IPluralityCase[] Cases { get; }
    }

    /// <summary>
    /// Plurality rules for languages where the sentence is conjugaed based on two numbers.
    /// 
    /// "range" Plurality rules for languages where the sentence is conjugaed according to range number (e.g. 1-2)
    /// Case "":                  "{range:0}-{range:1} நாட்கள்"
    ///      "N0:One:N1:Other":   "{0}–{1} நாட்கள்" when {0}=1, {1}=2
    ///      "N0:Other:N1:One":   "{0}–{1} நாள்"    when {0}=0, {1}=1
    ///      "N0:Other:N1:Other": "{0}–{1} நாட்கள்" when {0}=0, {1}=2
    /// </summary>
    public interface IPluralityRulesTwoArguments : IPluralityRules
    {
        /// <summary>
        /// Two argument plurality cases.
        /// </summary>
        /// <returns>rules</returns>
        IPluralityCaseTwoArguments[] Cases { get; }
    }

    /// <summary>
    /// A category checker for one culture and one category.
    /// </summary>
    public interface IPluralityCase
    {
        /// <summary>
        /// Case name this rule applies to (e.g. "Other", "Zero", "One")
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Tests whether case matches to the value.
        /// </summary>
        /// <param name="value">numeric value, boxed in object</param>
        /// <param name="formatted"><paramref name="value"/> formatted in its string presentation</param>
        /// <returns></returns>
        bool Match(object value, string formatted);
    }

    /// <summary>
    /// A case for a pair of values, such as a range. 
    /// </summary>
    public interface IPluralityCaseTwoArguments
    {
        /// <summary>
        /// Case name this rule applies to (e.g. "Other", "Zero", "One")
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Tests whether category rule matches to the value.
        /// </summary>
        /// <param name="from">numeric value, boxed in object</param>
        /// <param name="to">numeric value, boxed in object</param>
        /// <param name="fromFormatted"><paramref name="from"/> formatted in its string presentation</param>
        /// <param name="toFormatted"><paramref name="to"/> formatted in its string presentation</param>
        /// <returns></returns>
        bool Match(object from, object to, string fromFormatted, string toFormatted);
    }

}
