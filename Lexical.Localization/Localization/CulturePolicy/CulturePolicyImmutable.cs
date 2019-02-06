//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.12.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// <see cref="ICulturePolicy"/> implementation where culture enumerable is immutable.
    /// Note that the source of the enumerable is modifiable.
    /// </summary>
    public class CulturePolicyImmutable : ICulturePolicy
    {
        /// <summary>
        /// Culture 
        /// </summary>
        public IEnumerable<CultureInfo> Cultures { get; }

        /// <summary>
        /// Create culture policy with non-changeable enumerable.
        /// </summary>
        /// <param name="cultures"></param>
        public CulturePolicyImmutable(IEnumerable<CultureInfo> cultures)
        {
            this.Cultures = cultures ?? throw new ArgumentNullException(nameof(cultures));
        }
    }
}
