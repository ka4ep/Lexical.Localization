// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    #region ICulturePolicy
    /// <summary>
    /// Interface for policy that returns active culture policy, and fallback cultures.
    /// </summary>
    public interface ICulturePolicy
    {
        /// <summary>
        /// Enumerable that returns first the active culture, and then fallback cultures.
        /// 
        /// For example: "en-UK", "en", "".
        /// </summary>
        CultureInfo[] Cultures { get; }
    }
    #endregion ICulturePolicy

    #region ICulturePolicyAssignable
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
    #endregion ICulturePolicyAssignable
}
