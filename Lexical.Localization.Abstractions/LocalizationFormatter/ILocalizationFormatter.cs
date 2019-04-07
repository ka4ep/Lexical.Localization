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
    /// Adds format arguments into language string and formulates a string
    /// </summary>
    public interface ILocalizationFormatter
    {
        /// <summary>
        /// Formulate language string.
        /// </summary>
        /// <param name="request"></param>
        //void Formulate(ref FormatRequest request);

        LocalizationString ResolveString(IAssetKey key);
        LocalizationString ResolveFormulatedString(IAssetKey key);
    }

    /// <summary>
    /// A formatter where result observer, typically a logger, can be placed into.
    /// </summary>
    public interface ILocalizationFormatterObservable : ILocalizationFormatter, IObservable<LocalizationString>
    {
    }



}
