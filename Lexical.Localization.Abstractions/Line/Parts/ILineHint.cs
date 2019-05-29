// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that the parameter is not to be used as key (hash-equals comparisons).
    /// </summary>
    public interface ILineHint : ILineParameter
    {
    }

    /// <summary>
    /// Enumerable of <see cref="ILineHint"/> parameters.
    /// </summary>
    public interface ILineHintEnumerable : ILine, IEnumerable<ILineHint>
    {
    }

    public static partial class ILineExtensions
    {
    }
}
