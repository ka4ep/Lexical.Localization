// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries a localization string value.
    /// </summary>
    public interface ILineLocalizationString : ILocalizationKey, ILinePart
    {
        /// <summary>
        /// Localization string value.
        /// </summary>
        IFormulationString LocalizationString { get; }
    }

    /// <summary></summary>
    public static partial class LineLocalizationStringExtensions
    {
        /// <summary>
        /// Append <see cref="ILineLocalizationString"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="localizationString"></param>
        /// <returns></returns>
        public static IAssetKey LocalizationString(this ILinePart part, IFormulationString localizationString)
            => part.Append<ILineLocalizationString, IFormulationString>(localizationString);
    }


}

    