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
    public class FunctionsMap : Dictionary<string, IFunction>, IFunctionsQueryable, IFunctionsEnumerable, IFunctionsMap
    {
        /// <summary>
        /// Create table of functions.
        /// </summary>
        public FunctionsMap() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcs"></param>
        public FunctionsMap(IEnumerable<IFunctions> srcs) : base()
        {
            foreach (var funcSrc in srcs)
            {
                if (funcSrc is IFunctionsEnumerable enumr)
                {
                    foreach (IFunction f in enumr)
                    {
                        this[f.Name] = f;
                    }
                }
            }
        }

        IEnumerator<IFunction> IEnumerable<IFunction>.GetEnumerator()
            => Values.GetEnumerator();

        /// <summary>
        /// Add function to table.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public FunctionsMap Add(IFunction function)
        {
            this[function.Name] = function;
            return this;
        }
    }
}
