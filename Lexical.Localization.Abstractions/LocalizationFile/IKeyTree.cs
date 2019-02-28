// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    public interface IKeyTree
    {
        /// <summary>
        /// Parent node, unless is root then null.
        /// </summary>
        IKeyTree Parent { get; }

        /// <summary>
        /// Parameters that are associated with this particular node.
        /// </summary>
        IAssetKey Key { get; set; }

        /// <summary>
        /// Associated values.
        /// </summary>
        ICollection<string> Values { get; }

        /// <summary>
        /// Test if has child nodes.
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Child nodes.
        /// </summary>
        IReadOnlyCollection<IKeyTree> Children { get; }

        /// <summary>
        /// Test if has values.
        /// </summary>
        bool HasValues { get; }

        /// <summary>
        /// Add new child to the key tree.
        /// </summary>
        /// <returns></returns>
        IKeyTree CreateChild();

        /// <summary>
        /// Search child by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>child node or null if was not found</returns>
        IKeyTree GetChild(IAssetKey key);
    }

    public static class KeyTreeExtensions
    {
        /// <summary>
        /// Get or create node by parameters in <paramref name="key"/>.
        /// 
        /// If <paramref name="key"/> contains no parameters, returns <paramref name="node"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key">key that contains parameters</param>
        /// <returns>existing child, new child, or <paramref name="node"/> if no parameters were provided</returns>
        public static IKeyTree GetOrCreate(this IKeyTree node, IAssetKey key)
        {
            // No parameters
            if (key.GetParameterCount() == 0) return node;

            // Get existing child
            IKeyTree child = node.GetChild(key);
            if (child != null) return child;
            
            // Create child
            child = node.CreateChild();
            child.Key = key;
            return child;
        }

        /// <summary>
        /// Adds key and/or value.
        /// 
        /// If argument <paramref name="key"/> is given, then get-or-creates child node, otherwise uses <paramref name="node"/>.
        /// If argument <paramref name="value"/> is given, then adds value.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key">(optional) possible initial key to set.</param>
        /// <param name="value">(optional) possible initial value to add</param>
        /// <returns><param name="node"></returns>
        public static IKeyTree Add(this IKeyTree node, IAssetKey key, string value)
        {
            IKeyTree n = node;
            if (key != null) n = n.GetChild(key);
            if (value != null && !n.Values.Contains(value)) n.Values.Add(value);
            return node;
        }

        /// <summary>
        /// Add key-value recursively
        /// 
        /// If argument <paramref name="key_parts"/> is given, then get-or-creates decendent node, otherwise uses <paramref name="node"/>.
        /// If argument <paramref name="value"/> is given then adds value.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key_parts">(optional) possible initial key to set.</param>
        /// <param name="value">(optional) possible initial value to add</param>
        /// <returns><param name="node"></returns>
        public static IKeyTree AddRecursive(this IKeyTree node, IEnumerable<IAssetKey> key_parts, string value)
        {
            IKeyTree n = node;
            // Add-or-get section
            foreach (IAssetKey key in key_parts)
                n = n.GetOrCreate(key);
            if (value != null && !n.Values.Contains(value)) node.Values.Add(value);
            return node;
        }

        /// <summary>
        /// Visit every decendent node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<IKeyTree> Decendents(this IKeyTree node)
        {
            if (node == null) yield break;

            Queue<IKeyTree> queue = new Queue<IKeyTree>();
            queue.Enqueue(node);
            while(queue.Count>0)
            {
                IKeyTree n = queue.Dequeue();
                if (n != node) yield return n;

                if (n.HasChildren)
                    foreach (IKeyTree child in n.Children)
                        queue.Enqueue(child);
            }
        }
    }
}
