﻿//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Internal
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
        public static IAsset ToAsset(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKeyNamePolicy policy)
            => new LoadableLocalizationStringAsset(policy).AddLineStringSource(lines).Load();

        /// <summary>
        /// Convert <paramref name="lines"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="policy"></param>
        /// <param name="sourceHint">(optional)</param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKeyNamePolicy policy, string sourceHint = null)
            => new LocalizationStringLinesSource(lines, policy, sourceHint);

        /// <summary>
        /// Add prefix parameters to each key.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> AddKeyPrefix(this IEnumerable<KeyValuePair<string, string>> lines, string prefix)
        {
            if (string.IsNullOrEmpty(prefix) || lines == null) return lines;
            return lines.Select(line => new KeyValuePair<string, string>(prefix + line.Key, line.Value));
        }

        /// <summary>
        /// Add prefix parameters to each key.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="key"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> AddKeyPrefix(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKey key, IAssetKeyNamePolicy policy)
        {
            if (lines == null || key == null || key.GetParameterCount() == 0) return lines;
            if (policy == null) policy = AssetKeyNameProvider.Default;
            return lines.Select(line => {
                IAssetKey lineKey;
                if (policy.TryParse(line.Key, out lineKey)) return new KeyValuePair<string, string>(policy.BuildName(key.ConcatIfNew(lineKey)), line.Value);
                return new KeyValuePair<string, string>(policy.BuildName(key) + line.Key, line.Value);
            });
        }

        /// <summary>
        /// Add suffix parameters to each key.
        /// </summary>
        /// <param name="trees"></param>
        /// <param name="key">(optional)</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> AddKeySuffix(this IEnumerable<KeyValuePair<string, string>> lines, string suffix)
        {
            if (string.IsNullOrEmpty(suffix) || lines == null) return lines;
            return lines.Select(line => new KeyValuePair<string, string>(line.Key+suffix, line.Value));
        }

        /// <summary>
        /// Add suffix parameters to each key.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="key"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> AddKeySuffix(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKey key, IAssetKeyNamePolicy policy)
        {
            if (lines == null || key == null || key.GetParameterCount()==0) return lines;
            if (policy == null) policy = AssetKeyNameProvider.Default;
            return lines.Select(line => {
                IAssetKey lineKey;
                if (policy.TryParse(line.Key, out lineKey)) return new KeyValuePair<string, string>(policy.BuildName(lineKey.ConcatIfNew(key)), line.Value);
                return new KeyValuePair<string, string>(line.Key+policy.BuildName(key), line.Value);
            });
        }

    }
}
