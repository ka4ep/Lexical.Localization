﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Extended version of <see cref="ICustomFormatter"/>. 
    /// 
    /// <see cref="ILocalizationArgumentFormatter"/> is provided with <see cref="IFormatProvider"/>.
    /// </summary>
    public interface ILocalizationArgumentFormatter
    {
        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information <paramref name="formatProvider"/>.
        /// </summary>
        /// <param name="functionName">function name that is extracted from formulation string</param>
        /// <param name="format">format paramters</param>
        /// <param name="arg">argument value</param>
        /// <param name="formatProvider">culture-specific formatting information</param>
        /// <returns>The string representation of the value of arg, formatted as specified by <paramref name="format"/>, <paramref name="functionName"/> and <paramref name="formatProvider"/>, or null if was unable to format</returns>
        string Format(string functionName, string format, object arg, IFormatProvider formatProvider);
    }
}
