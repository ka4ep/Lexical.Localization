using System;
using System.Collections.Generic;
using System.Linq;
using Lexical.Localization.Internal;
using Lexical.Localization.LocalizationFile2;

namespace Lexical.Localization
{
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

    }

    /// <summary>
    /// TreeNode is an intermediate model for writing text files
    /// 
    /// Reorganize parts so that non-canonicals parts, so that "root" is first, then "culture", and then others by parameter name.
    /// </summary>
    internal class PartComparer : IComparer<PartComparer.Part>
    {
        private static readonly PartComparer instance = new PartComparer().AddParametersToSortOrder("root", "culture");
        public static PartComparer Default => instance;

        public readonly List<string> order = new List<string>();

        public PartComparer()
        {
        }

        public PartComparer AddParametersToSortOrder(IEnumerable<string> parameters)
        {
            this.order.AddRange(parameters);
            return this;
        }

        public PartComparer AddParametersToSortOrder(params string[] parameters)
        {
            this.order.AddRange(parameters);
            return this;
        }

        public int Compare(Part x, Part y)
        {
            // canonical parts cannot be reordered between themselves.
            if (x.isCanonical || y.isCanonical) return 0;
            int xix = order.IndexOf(x.name);
            int yix = order.IndexOf(y.name);
            if (xix == yix) return 0;
            if (xix < 0) xix = Int32.MaxValue;
            if (yix < 0) yix = Int32.MaxValue;
            return xix - yix;
        }

        public struct Part
        {
            public string name;
            public string value;
            public bool isCanonical;
            public bool isNonCanonical;

            public Part(string name, string value, bool isCanonical, bool isNonCanonical)
            {
                this.name = name;
                this.value = value;
                this.isCanonical = isCanonical;
                this.isNonCanonical = isNonCanonical;
            }

            public Key CreateKey(Key prev = default)
                => isCanonical ? new Key.Canonical(prev, name, value) :
                   isNonCanonical ? new Key.NonCanonical(prev, name, value) :
                   new Key(prev, name, value);
        }
    }



}
