// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Parses arguments from formulation strings.
    /// 
    /// For example "You received {cardinal:0} coin(s)." is a formulation string
    /// that parsed into argument and non-argument sections.
    /// </summary>
    public interface ILocalizationArgumentFormatter
    {
        /// <summary>
        /// Try to parse formulation string into <see cref="IFormulationString"/>.
        /// </summary>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="formulationString"/> is null</exception>
        IFormulationString Parse(string formulationString);
    }
}
