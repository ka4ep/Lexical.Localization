// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Interface for validating whether a <see cref="ILine"/> matches a criteria.
    /// </summary>
    public interface ILineFilter
    {
        /// <summary>
        /// Filter <paramref name="key"/> against the filter rules.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if <paramref name="key"/> passes the filter, containing all required parameters</returns>
        bool Filter(ILine key);

        /// <summary>
        /// Filters lines against filter rules.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        IEnumerable<ILine> Filter(IEnumerable<ILine> line);

        /// <summary>
        /// Filters lines against filter rules.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        //IEnumerable<ILine> Filter(IEnumerable<ILine> lines);
    }
}
