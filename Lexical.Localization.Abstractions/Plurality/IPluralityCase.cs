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
    /// Plurality case.
    /// 
    /// Subinterfaces
    /// <list type="bullet">
    ///     <list><see cref="IPluralityCaseInfo"/></list>
    ///     <list><see cref="IPluralityCategoryPart"/></list>
    ///     <list><see cref="IPluralityCaseEvaluator"/></list>
    /// </list>
    /// </summary>
    public interface IPluralityCase
    {
    }

    /// <summary>
    /// Basic info of a plurality case.
    /// </summary>
    public interface IPluralityCaseInfo
    {
        /// <summary>
        /// Name of the case, e.g. "Zero", "One"
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Is case optional.
        /// </summary>
        bool Optional { get; }
    }

    /// <summary>
    /// A single case of a <see cref="IPluralityCategory"/>.
    /// </summary>
    public interface IPluralityCategoryPart
    {
        /// <summary>
        /// The the category this case is part of.
        /// </summary>
        IPluralityCategory Category { get; }

        /// <summary>
        /// Index in <see cref="IPluralityCategory.Cases"/>.
        /// </summary>
        int CaseIndex { get; }
    }

    /// <summary>
    /// Interface for classes that evaluate whether argument value and text matches the plurality case.
    /// </summary>
    public interface IPluralityCaseEvaluator
    {
        /// <summary>
        /// Evaluate whether the case applies to <paramref name="value"/> and <paramref name="formattedValue"/>
        /// </summary>
        /// <param name="value">numeric value, boxed in object</param>
        /// <param name="formattedValue"><paramref name="value"/> formatted in its string presentation</param>
        /// <param name="numberFormat">Format info containing decimal separators (aquired from <see cref="CultureInfo"></see></param>
        /// <returns>true or false</returns>
        bool Evaluate(object value, string formattedValue, NumberFormatInfo numberFormat);
    }



}
