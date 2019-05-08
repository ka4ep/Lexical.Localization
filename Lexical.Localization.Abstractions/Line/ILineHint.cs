// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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
