//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using Lexical.Localization.LocalizationFile2;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for <see cref="IKeyTree"/>.
    /// </summary>
    public static class KeyTreeExtensions_
    {
        /// <summary>
        /// Flatten <paramref name="keyTree"/> to string lines.
        /// </summary>
        /// <param name="keyTree"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ToStringLines(this IKeyTree keyTree, IAssetKeyNamePolicy policy)
            => keyTree.ToKeyLines(true).ToStringLines(policy);

        /// <summary>
        /// Flatten <paramref name="node"/> to key lines.
        /// </summary>
        /// <param name="skipRoot"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ToKeyLines(this IKeyTree node, bool skipRoot = true)
        {
            Queue<(IKeyTree, IAssetKey)> queue = new Queue<(IKeyTree, IAssetKey)>();
            queue.Enqueue((node, skipRoot && node.Key.Name == "root" ? null : node.Key));
            while (queue.Count > 0)
            {
                // Next element
                (IKeyTree, IAssetKey) current = queue.Dequeue();

                // Yield values
                if (current.Item2 != null && current.Item1.HasValues)
                {
                    foreach (string value in current.Item1.Values)
                        yield return new KeyValuePair<IAssetKey, string>(current.Item2, value);
                }

                // Enqueue children
                if (current.Item1.HasChildren)
                {
                    foreach (IKeyTree child in current.Item1.Children)
                    {
                        IAssetKey childKey = current.Item2 == null ? child.Key : current.Item2.Concat(child.Key);
                        queue.Enqueue((child, childKey));
                    }
                }
            }
        }

        /// <summary>
        /// Create an asset that uses <paramref name="tree"/>.
        /// 
        /// Trees are reloaded into the asset if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="hintSource"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IKeyTree tree, string hintSource = null)
            => new LoadableLocalizationAsset().AddKeyTreeSource(tree, hintSource).Load();

        /// <summary>
        /// Create an asset that uses <paramref name="trees"/>.
        /// 
        /// Trees are reloaded into the asset if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="trees"></param>
        /// <param name="hintSource"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IEnumerable<IKeyTree> trees, string hintSource = null)
            => new LoadableLocalizationAsset().AddKeyTreeSource(trees, hintSource).Load();

    }
}
