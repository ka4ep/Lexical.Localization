// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolves name to <see cref="IFunctions"/>.
    /// </summary>
    public interface IFunctionsResolver
    {
        /// <summary>
        /// Resolve <paramref name="name"/> to <see cref="IFunctions"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>functions or null</returns>
        IFunctions Resolve(string name);
    }
}
