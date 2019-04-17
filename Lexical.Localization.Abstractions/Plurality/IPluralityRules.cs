// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{

    /// <summary>
    /// Map that contains plurality rules.
    /// </summary>
    public interface IPluralityRuleMap : IReadOnlyDictionary<CultureInfo, IPluralityRules>
    {
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
    public interface IPluralityRules : IReadOnlyDictionary<string, IPluralityCategory>
    {

    }

}
