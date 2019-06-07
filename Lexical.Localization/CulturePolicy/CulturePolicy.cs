//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.12.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        /// Source of cultures
        /// </summary>
        protected ICulturePolicy source;

        /// <summary>
        /// Cultures. The first element is active culture, others fallback cultures.
        /// </summary>
        public CultureInfo[] Cultures => source?.Cultures ?? NO_CULTURES;

        /// <summary>
        /// Create new culture policy. 
        /// The initial configuration returns CultureInfo.ActiveCulture and its fallback cultures.
        /// </summary>
        public CulturePolicy()
        {
            source = new CulturePolicyFuncWithFallbacks( ICulturePolicyExtensions.FuncCurrentThreadCulture );
        }

        /// <summary>
        /// Create new culture policy with initial cultures <paramref name="initialCultures"/>.
        /// </summary>
        /// <param name="initialCultures"></param>
        public CulturePolicy(IEnumerable<CultureInfo> initialCultures)
        {
            source = new CulturePolicyArray(initialCultures?.ToArray() ?? NO_CULTURES);
        }

        /// <summary>
        /// Create new culture policy with initial cultures <paramref name="initialCultures"/>.
        /// </summary>
        /// <param name="initialCultures"></param>
        public CulturePolicy(params CultureInfo[] initialCultures)
        {
            source = new CulturePolicyArray(initialCultures ?? NO_CULTURES);
        }

        /// <summary>
        /// Assign new enumerable
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns>this</returns>
        public ICulturePolicyAssignable SetSource(ICulturePolicy culturePolicy)
        {
            this.source = culturePolicy;
            return this;
        }
    }

}
