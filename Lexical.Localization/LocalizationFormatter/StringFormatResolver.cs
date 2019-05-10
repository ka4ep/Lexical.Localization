// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Resolves string format class name to string format.
    /// </summary>
    public class StringFormatResolver : IStringFormatResolver
    {
        private static IStringFormatResolver instance = new StringFormatResolver();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IStringFormatResolver Default => instance;

        public IStringFormat Resolve(string name)
        {
            throw new NotImplementedException();
        }
    }
}
