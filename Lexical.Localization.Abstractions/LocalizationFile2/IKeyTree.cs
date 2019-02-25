﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.LocalizationFile2
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
        /// Flatten tree as lines of key-value pairs.
        /// </summary>
        /// <param name="skipRoot"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ToLines(this IKeyTree node, bool skipRoot)
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
}