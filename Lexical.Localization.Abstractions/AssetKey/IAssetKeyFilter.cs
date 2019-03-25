﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Interface for validating whether a <see cref="IAssetKey"/> matches a criteria.
    /// </summary>
    public interface IAssetKeyFilter
    {
        /// <summary>
        /// Filter <paramref name="key"/> against the filter rules.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if <paramref name="key"/> passes the filter, containing all required parameters</returns>
        bool Filter(IAssetKey key);

        /// <summary>
        /// Filters lines against filter rules.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        IEnumerable<IAssetKey> Filter(IEnumerable<IAssetKey> keys);

        /// <summary>
        /// Filters lines against filter rules.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<IAssetKey, string>> Filter(IEnumerable<KeyValuePair<IAssetKey, string>> lines);
    }
}
