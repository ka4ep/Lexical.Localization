﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <returns>new child</returns>
        IKeyTree CreateChild();

        /// <summary>
        /// Search child by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>child node or null if was not found</returns>
        IKeyTree GetChild(IAssetKey key);

        /// <summary>
        /// Remove self from parent.
        /// </summary>
        void Remove();
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

        /// <summary>
        /// List parameters from root to tail.
        /// </summary>
        /// <param name="skipRoot">should "root" parameter be skipped</param>
        /// <returns>parameters</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetConcatenatedParameters(this IKeyTree node, bool skipRoot)
        {
            IEnumerable<KeyValuePair<string, string>> result = null;
            if (node.Parent != null) result = node.Parent.GetConcatenatedParameters(skipRoot);
            IEnumerable<KeyValuePair<string, string>> key = node.Key.GetParameters(skipRoot);
            result = result == null ? key : result.Concat(key);
            return result;
        }

        /// <summary>
        /// Get concatenated key from root to <paramref name="node"/>.
        /// </summary>
        /// <returns>key</returns>
        public static IAssetKey GetConcatenatedKey(this IKeyTree node)
             => node.Parent != null ? node.Parent.GetConcatenatedKey().Concat(node.Key) : node.Key;

        /// <summary>
        /// Search decendents for tree nodes that have matching key. 
        /// </summary>
        /// <param name="node">node to start search.</param>
        /// <param name="searchKey">key to search</param>
        /// <returns>nodes that have matching key</returns>
        public static IEnumerable<IKeyTree> Search(this IKeyTree node, IAssetKey searchKey)
        {
            List<IKeyTree> result = new List<IKeyTree>();
            _search(node, searchKey, node.Key, result);
            return result;
        }

        static void _search(IKeyTree node, IAssetKey searchKey, IAssetKey concatenatedKeyOfNode, List<IKeyTree> result)
        {
            // Check for mismatch
            for(IAssetKey k = concatenatedKeyOfNode; k!=null; k=k.GetPreviousKey())
            {
                string parameterName = k.GetParameterName();
                if (parameterName == null || parameterName == "root") continue;
                string parameterValue = k.Name;

                bool parameterDetectedInSearchKey = false;
                for (IAssetKey sk = searchKey; sk != null; sk = sk.GetPreviousKey())
                {
                    string _parameterName = sk.GetParameterName();
                    if (_parameterName == null || _parameterName == "root") continue;
                    string _parameterValue = sk.Name;
                    parameterDetectedInSearchKey |= _parameterValue == parameterValue;
                    if (parameterDetectedInSearchKey) break;
                }

                // node has a parameter that was not found in search key. This is wrong branch
                if (!parameterDetectedInSearchKey) return;
            }

            // Key match
            if (AssetKeyComparer.Default.Equals(concatenatedKeyOfNode, searchKey)) { result.Add(node); return; }

            // Recurse
            if (node.HasChildren)
            {
                foreach(IKeyTree child in node.Children)
                {
                    IAssetKey newConcatenatedKey = concatenatedKeyOfNode.Concat(child.Key);
                    _search(child, searchKey, newConcatenatedKey, result);
                }
            }
        }
    }
}
