// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// The default formatter that uses <see cref="string.Format(string, object)/>
    /// </summary>
    [Obsolete("Prototype")]
    public class LocalizationFormatter : ILocalizationFormatter
    {
        private static LocalizationFormatter instance = new LocalizationFormatter();
        public static LocalizationFormatter Default => instance;

        public void Formulate(ref FormatRequest request)
        {
        }
    }
}
