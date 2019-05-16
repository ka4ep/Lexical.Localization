// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Table of functions.
    /// </summary>
    public class Functions : Dictionary<string, IFunction>, IFunctionsQueryable, IFunctionsEnumerable, IFunctionsTable
    {
        /// <summary>
        /// Create table of functions.
        /// </summary>
        public Functions() : base()
        {
        }

        IEnumerator<IFunction> IEnumerable<IFunction>.GetEnumerator()
            => Values.GetEnumerator();

        /// <summary>
        /// Add function to table.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public Functions Add(IFunction function)
        {
            this[function.Name] = function;
            return this;
        }
    }
}
