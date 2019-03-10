//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Utils;
using System.Linq;
using System.Collections.Generic;
using Lexical.Localization.Internal;

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
            queue.Enqueue((node, skipRoot && node.Key.Name == "Root" ? null : node.Key));
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

        /// <summary>
        /// Convert <paramref name="tree"/> to <see cref="IAssetSource"/>..
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="sourceHint">(optional)</param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(IKeyTree tree, string sourceHint = null)
            => new LocalizationKeyLinesSource(tree.ToKeyLines(), sourceHint);

        /// <summary>
        /// Convert <paramref name="trees"/> to <see cref="IAssetSource"/>..
        /// </summary>
        /// <param name="trees"></param>
        /// <param name="sourceHint">(optional)</param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(this IEnumerable<IKeyTree> trees, string sourceHint = null)
            => new LocalizationKeyLinesSource(trees.SelectMany(tree => tree.ToKeyLines()), sourceHint);

        /// <summary>
        /// Add a prefix to the key of each first level node.
        /// 
        /// If <paramref name="left"/> is null or contains no parameters, then original <paramref name="trees"/> are returned.
        /// </summary>
        /// <param name="trees"></param>
        /// <param name="left">(optional)</param>
        /// <returns></returns>
        public static IEnumerable<IKeyTree> AddKeyPrefix(this IEnumerable<IKeyTree> trees, IAssetKey left)
        {
            Key key = Key.CreateFrom(left);
            if (key == null) return trees;
            return _AddKeyPrefix(trees, key);
        }

        static IEnumerable<IKeyTree> _AddKeyPrefix(this IEnumerable<IKeyTree> trees, Key left)
        {
            Key _left = null;
            foreach (var parameter in left.GetParameters())
                _left = Key.Create(parameter.Key, parameter.Value);

            foreach (IKeyTree tree in trees)
            {
                // Transform tree
                if (_left != null)
                {
                    foreach (IKeyTree node in tree.Children)
                        node.Key = _left.ConcatIfNew(node.Key);
                }

                // Return tree
                yield return tree;
            }
        }

        /// <summary>
        /// Add suffix to the key of each value-level node.
        /// 
        /// If <paramref name="right"/> is null or contains no parameters, then original <paramref name="trees"/> are returned.
        /// </summary>
        /// <param name="trees"></param>
        /// <param name="right">(optional)</param>
        /// <returns></returns>
        public static IEnumerable<IKeyTree> AddKeySuffix(this IEnumerable<IKeyTree> trees, IAssetKey right)
        {
            Key key = Key.CreateFrom(right);
            if (key == null) return trees;
            return _AddKeySuffix(trees, key);
        }

        static IEnumerable<IKeyTree> _AddKeySuffix(this IEnumerable<IKeyTree> trees, Key right)
        {
            Key _right = null;
            foreach (var parameter in right.GetParameters())
                _right = Key.Create(parameter.Key, parameter.Value);

            foreach (IKeyTree tree in trees)
            {
                // Transform tree
                if (_right != null)
                {
                    foreach (IKeyTree node in tree.Decendents())
                        if (node.HasValues)
                            node.Key = node.Key.ConcatIfNew(_right);
                }

                // Return tree
                yield return tree;
            }
        }

    }
}
