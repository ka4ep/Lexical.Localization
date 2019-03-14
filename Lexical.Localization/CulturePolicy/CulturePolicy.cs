//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.12.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// <see cref="ICulturePolicy"/> implementation that is reconfigurable.
    /// </summary>
    public class CulturePolicy : ICulturePolicyAssignable
    {
        /// <summary>
        /// No cultures array.
        /// </summary>
        public static CultureInfo[] NO_CULTURES = new CultureInfo[0];

        /// <summary>
        /// Cultures. The first element is active culture, others fallback cultures.
        /// </summary>
        public IEnumerable<CultureInfo> Cultures { get; protected set; }

        /// <summary>
        /// Create new culture policy. 
        /// The initial configuration returns CultureInfo.ActiveCulture and its fallback cultures.
        /// </summary>
        public CulturePolicy()
        {
            CulturePolicyExtensions.SetToCurrentThreadCulture(this);
        }

        /// <summary>
        /// Create new culture policy with initial cultures <paramref name="initialCultures"/>.
        /// </summary>
        /// <param name="initialCultures"></param>
        public CulturePolicy(IEnumerable<CultureInfo> initialCultures)
        {
            Cultures = initialCultures ?? NO_CULTURES;
        }

        /// <summary>
        /// Assign new enumerable
        /// </summary>
        /// <param name="cultureEnumerable"></param>
        /// <returns>this</returns>
        public ICulturePolicyAssignable SetCultures(IEnumerable<CultureInfo> cultureEnumerable)
        {
            this.Cultures = cultureEnumerable;
            return this;
        }
    }    
}
