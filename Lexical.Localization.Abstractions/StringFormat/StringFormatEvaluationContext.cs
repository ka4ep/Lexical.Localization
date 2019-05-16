// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// String evaluation context
    /// </summary>
    public partial struct StringFormatEvaluationContext
    {
        /// <summary>
        /// (optional) Format Arguments.
        /// Used when string format uses arguments by index, e.g. "Hello, {0}"
        /// </summary>
        public object[] Args;

        /// <summary>
        /// (optional) Format arguments by name
        /// Used when string format uses arguments by name, e.g. "Hello, %s"
        /// </summary>
        public IReadOnlyDictionary<string, object> ArgsByName;

        /// <summary>
        /// Custom format provider, excluding culture.
        /// </summary>
        public IFormatProvider FormatProvider;

        /// <summary>
        /// (optional) Active culture.
        /// </summary>
        public CultureInfo CultureInfo;

        /// <summary>
        /// (optional) Functions.
        /// </summary>
        public IFunctions Functions;

        /// <summary>
        /// Reserved for <see cref="IStringResolver"/> implementation specific. 
        /// </summary>
        public object Reserved;
    }
}
