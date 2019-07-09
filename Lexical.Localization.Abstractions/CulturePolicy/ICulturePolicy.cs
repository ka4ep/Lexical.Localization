// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    // <ICulturePolicy>
    /// <summary>
    /// Interface for policy that returns active culture policy, and fallback cultures.
    /// </summary>
    public interface ICulturePolicy
    {
        /// <summary>
        /// Array property returns the prefered culture as first element.
        /// Other cultures are considered fallback cultures.
        /// 
        /// For example: "en-UK", "en", "".
        /// </summary>
        CultureInfo[] Cultures { get; }
    }
    // </ICulturePolicy>

    // <ICulturePolicyAssignable>
    /// <summary>
    /// Interface for culture policy where culture is assignable.
    /// </summary>
    public interface ICulturePolicyAssignable : ICulturePolicy
    {
        /// <summary>
        /// Set source of cultures. The first element is active culture, others fallback cultures.
        /// </summary>
        /// <param name="cultureSource"></param>
        /// <returns></returns>
        ICulturePolicyAssignable SetSource(ICulturePolicy cultureSource);
    }
    // </ICulturePolicyAssignable>
}
