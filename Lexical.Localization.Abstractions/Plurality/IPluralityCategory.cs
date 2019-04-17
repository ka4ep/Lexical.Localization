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
    /// Plurality constants.
    /// </summary>
    public static class Plurality_
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
    /// Plurality category is function that can be attached to an argument to describe possible
    /// cases of declination of sentences.
    /// 
    /// For example formulation string "There are {plural:0} cats", describes that there will be
    /// different declination cases of the sentence depending on the number in the argument.
    /// </summary>
    public interface IPluralityCategory
    {
        /// <summary>
        /// Category name, e.g. "plural" "ordinal", "optional"
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
        /// For example, "{plural:0} cars" for value '0' returns cases "Zero" and "Other".
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


}
