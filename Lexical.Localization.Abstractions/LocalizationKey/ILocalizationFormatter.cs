// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that can be assigned with a <see cref="ILocalizationFormatter"/>.
    /// </summary>
    [Obsolete("Prototype")]
    public interface ILocalizationFormatterAssignable : IAssetKey
    {
        /// <summary>
        /// Create key where a new <paramref name="formatter"/> has been assigned to the key chain.
        /// 
        /// If key has multiple formatters, they are evaluated in order from tail towards root.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns>key that is assigned with <paramref name="formatter"/></returns>
        ILocalizationFormatterAssigned Formatter(ILocalizationFormatter formatter);
    }

    /// <summary>
    /// A key that has been assigned with formatter
    /// </summary>
    [Obsolete("Prototype")]
    public interface ILocalizationFormatterAssigned : IAssetKey
    {
        /// <summary>
        /// (Optional) The assigned formatter.
        /// </summary>
        ILocalizationFormatter Formatter { get; }
    }

    /// <summary>
    /// Adds format arguments into language string and formulates a string
    /// </summary>
    [Obsolete("Prototype")]
    public interface ILocalizationFormatter
    {
        /// <summary>
        /// Formulate language string.
        /// </summary>
        /// <param name="request"></param>
        void Formulate(ref FormatRequest request);
    }

    /// <summary>
    /// Format request that is handled by a chain of <see cref="ILocalizationFormatter"/> until the request
    /// is completely formatted.
    /// </summary>
    public struct FormatRequest
    {
        /// <summary>
        /// Current state
        /// </summary>
        public FormatState State;

        /// <summary>
        /// Culture to apply to the formatting.
        /// </summary>
        public CultureInfo Culture;

        /// <summary>
        /// Language string where parameters are numbered written inside curly brackets "{0}".
        /// </summary>
        public String LanguageString;

        /// <summary>
        /// Format arguments to apply.
        /// </summary>
        public object[] FormatArgs;

        /// <summary>
        /// Key
        /// </summary>
        public IAssetKey Key;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FormatState
    {
        /// <summary>
        /// Formatting has not been applied to the language string.
        /// </summary>
        Unapplied,

        /// <summary>
        /// Some parameters have been formatted, but not all.
        /// </summary>
        PartiallyAppled,

        /// <summary>
        /// Format request is completely formatted.
        /// </summary>
        Completed
    }

    /// <summary>
    /// Event that occurs when a language string formatted or resolved.
    /// </summary>
    //public class FormattingEvent
    //{
    //}

    public static partial class LocalizationKeyExtensions
    {

    }
}
