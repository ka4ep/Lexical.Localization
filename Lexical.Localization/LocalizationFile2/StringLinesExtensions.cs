﻿//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lexical.Localization.LocalizationFile2;
using Lexical.Localization.Internal;

namespace Lexical.Localization.LocalizationFile2
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public static class StringLinesExtensions
    {
        /// <summary>
        /// Parse string key of each line into <see cref="IAssetKey"/> by using <paramref name="policy"/>.
        /// 
        /// If parse fails, then skips the key, doesn't throw exception.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="policy"><see cref="IAssetKeyNameParser"/> implementation used for parsing.</param>
        /// <returns>lines with <see cref="IAssetKey"/> keys</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ToKeyLines(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKeyNamePolicy policy)
        {
            foreach (var line in lines)
            {
                IAssetKey kk;
                if (policy.TryParse(line.Key, out kk))
                    yield return new KeyValuePair<IAssetKey, string>(kk, line.Value);
            }
        }

        /// <summary>
        /// Parse string key of each line and put into <see cref="IKeyTree"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="policy"><see cref="IAssetKeyNameParser"/> implementation used for parsing.</param>
        /// <returns></returns>
        public static IKeyTree ToKeyTree(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKeyNamePolicy policy)
            => KeyTree.Create(lines.ToKeyLines(policy));

        /// <summary>
        /// Convert <paramref name="lines"/> to asset.
        /// 
        /// Lines are reloaded into the asset if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IEnumerable<KeyValuePair<string, string>> lines)
            => new LocalizationStringAsset(lines);
    }
}
