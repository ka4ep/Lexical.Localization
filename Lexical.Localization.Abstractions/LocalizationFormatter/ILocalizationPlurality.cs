﻿// --------------------------------------------------------
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

        public const string plurality = "plurality";
        public const string ordinal = "ordinal";
        public const string range = "range";
        public const string optional = "optional";

    }

    /// <summary>
    /// Map that contains plurality rules.
    /// </summary>
    public interface IPluralityFunctionMap : IEnumerable<(string, IPluralityFunction)>
    {
        /// <summary>
        /// Try get plurality function.
        /// </summary>
        /// <param name="languageCode">The key is ISO 639-1 (two character) or ISO 639-2 (three character) language code</param>
        /// <param name="functionName">Function name, such as "plurality", "ordinal"</param>
        /// <returns>function or null</returns>
        IPluralityFunction TryGet(string languageCode, string functionName);
    }

    /// <summary>
    /// Plurality rules when conjugating and declinating sentences that conform to one argument.
    /// 
    /// The instance is language specific.
    /// 
    /// "plural"
    ///      Case "":         "{plurality:0} car(s)"
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
    public interface IPluralityFunction
    {
        /// <summary>
        /// Function name, e.g. "plurality" "ordinal", "optional"
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Possible cases.
        /// </summary>
        IPluralityCase[] Cases { get; }

        /// <summary>
        /// Tests whether case(s) matches to the value. 
        /// 
        /// Sometimes there are optional cases. They are returned first, the mandatory cases are last indices of the result array.
        /// 
        /// For example, "{plurality:0} cars" for value '0' returns cases "Zero" and "Other".
        /// Zero is optional, and the translator can choose whether to use "Zero" string or not.
        ///    "N0:Zero"="No cars"
        ///    "N0:Other="{0} cars"
        ///    
        /// If "N0:Zero" is not found, then "N0:Other" is used.
        /// </summary>
        /// <param name="value">numeric value, boxed in object</param>
        /// <param name="formatted"><paramref name="value"/> formatted in its string presentation</param>
        /// <returns>Applicable cases, first ones are Optional, last is non-Optional</returns>
        IPluralityCase[] Evaluate(object value, string formatted);
    }

    /// <summary>
    /// A single case of a <see cref="IPluralityFunction"/>.
    /// </summary>
    public interface IPluralityCase
    {
        /// <summary>
        /// The function this case is part of
        /// </summary>
        IPluralityFunction Function { get; }

        /// <summary>
        /// Index in <see cref="IPluralityFunction.Cases"/>.
        /// </summary>
        int CaseIndex { get; }

        /// <summary>
        /// Name of the case, e.g. "Zero", "One"
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Is case optional.
        /// </summary>
        bool Optional { get; }

        /// <summary>
        /// Evaluate whether case applies to <paramref name="value"/> and <paramref name="formatted"/>
        /// </summary>
        /// <param name="value">numeric value, boxed in object</param>
        /// <param name="formatted"><paramref name="value"/> formatted in its string presentation</param>
        /// <returns>true or false</returns>
        bool Evaluate(object value, string formatted);
    }

}
