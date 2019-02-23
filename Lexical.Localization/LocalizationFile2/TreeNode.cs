// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.LocalizationFile2
{
    /// <summary>
    /// Keys organized into a tree structure.
    /// 
    /// The root key has single key Key("root", "").
    /// 
    /// Sub-nodes have one or more keys. For example an .ini file
    /// <code>
    /// [culture:en:type:MyController]
    /// key:Success = success
    /// </code>
    /// 
    /// Would be parsed into a tree of following structure.
    ///   KeyTree(Key("root", ""))
    ///       KeyTree(Key("culture", "en).Append("type", "MyController"))
    ///           KeyTree(Key("key", "Success")
    ///           
    /// </summary>
    public class KeyTree
    {
        /// <summary>
        /// Create tree structure from source of flat key values.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="parametrizer"></param>
        /// <returns>tree root ""</returns>
        public static KeyTree Create(IEnumerable<KeyValuePair<object, string>> keyValues, IAssetKeyParametrizer parametrizer)
        {
            KeyTree root = new KeyTree(new Key.NonCanonical("root", ""), null);
            root.AddRange(parametrizer, keyValues);
            return root;
        }

        /// <summary>
        /// Create tree structure from source of flat key values.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static KeyTree Create(IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
        {
            KeyTree root = new KeyTree(new Key.NonCanonical("root", ""), null);
            root.AddRange(AssetKeyParametrizer.Singleton, keyValues.Select(kp=>new KeyValuePair<object, string>(kp.Key, kp.Value)));
            return root;
        }

        /// <summary>
        /// Parent node, unless is root then null.
        /// </summary>
        public readonly KeyTree Parent;

        /// <summary>
        /// Associated parameters contained in an instance of Key.
        /// </summary>
        public readonly Key Key;

        /// <summary>
        /// Associated values.
        /// </summary>
        List<string> values;

        /// <summary>
        /// Child nodes
        /// </summary>
        Dictionary<Key, KeyTree> children;

        /// <summary>
        /// Test if has child nodes.
        /// </summary>
        public bool HasChildren => children != null && children.Count > 0;

        /// <summary>
        /// Get-or-create child nodes
        /// </summary>
        public Dictionary<Key, KeyTree> Children => children ?? (children = new Dictionary<Key, KeyTree>(Key.ChainComparer));

        /// <summary>
        /// Test if has values.
        /// </summary>
        public bool HasValues => values != null && values.Count > 0;

        /// <summary>
        /// Get-or-create values list.
        /// </summary>
        public List<string> Values => values ?? (values = new List<string>(1));

        /// <summary>
        /// Create new key tree node.
        /// </summary>
        /// <param name="parameter"></param>
        public KeyTree(Key parameter)
        {
            this.Key = parameter;
        }

        /// <summary>
        /// Create new key tree node.
        /// </summary>
        /// <param name="parameter"></param>
        public KeyTree(Key parameter, params string[] values)
        {
            this.Key = parameter;
            this.values = new List<string>(values);
        }

        /// <summary>
        /// Create new key tree node.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parameter"></param>
        public KeyTree(KeyTree parent, Key parameter)
        {
            this.Parent = parent;
            this.Key = parameter;
        }

        /// <summary>
        /// Create new key tree node.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parameter"></param>
        /// <param name="values"></param>
        public KeyTree(KeyTree parent, Key parameter, params string[] values)
        {
            this.Parent = parent;
            this.Key = parameter;
            this.values = new List<string>(values);
        }

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// </summary>
        /// <param name="parametrizer"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public KeyTree AddRange(IAssetKeyParametrizer parametrizer, IEnumerable<KeyValuePair<object, string>> keyValues)
            => AddRange(parametrizer, keyValues, groupingRule: null);

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// </summary>
        /// <param name="parametrizer"></param>
        /// <param name="keyValues"></param>
        /// <param name="groupingPatternText"></param>
        /// <returns></returns>
        public KeyTree AddRange(IAssetKeyParametrizer parametrizer, IEnumerable<KeyValuePair<object, string>> keyValues, string groupingPatternText)
            => AddRange(parametrizer, keyValues, groupingRule: new AssetNamePattern(groupingPatternText));

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// 
        /// If <paramref name="groupingRule"/> the nodes are laid out in the order occurance of name pattern parts.
        /// 
        /// For example grouping pattern "{type}/{culture}{anysection}{key}" would order nodes as following:
        /// <code>
        ///  "type:MyController": {
        ///      "key:Success": "Success",
        ///      "culture:en:key:Success": "Success",
        ///      "culture:fi:key:Success": "Onnistui"
        ///  }
        /// </code>
        /// 
        /// Non-capture parts such as "/" in pattern "{section}/{culture}", specify separator of tree node levels.
        /// 
        /// </summary>
        /// <param name="parametrizer"></param>
        /// <param name="keyValues"></param>
        /// <param name="groupingRule">(optional)</param>
        /// <returns></returns>
        public KeyTree AddRange(IAssetKeyParametrizer parametrizer, IEnumerable<KeyValuePair<object, string>> keyValues, IAssetNamePattern groupingRule)
        {
            // Create composite paramerizer
            IAssetKeyParametrizer compositeParametrizer = parametrizer is KeyTree.Parametrizer ? parametrizer : new AssetKeyParametrizerComposite(parametrizer, KeyTree.Parametrizer.Instance);
            // Create comparer that can compare TreeNode and argument's keys
            ParametrizedComparer comparer = new ParametrizedComparer();
            // Create orderer
            PartComparer partComparer = new PartComparer().AddParametersToSortOrder("root");
            if (groupingRule != null)
            {
                foreach (IAssetNamePatternPart part in groupingRule.CaptureParts)
                    partComparer.AddParametersToSortOrder(part.ParameterName);
            } else
            {
                partComparer.AddParametersToSortOrder("culture");
            }

            List<PartComparer.Part> partList = new List<PartComparer.Part>(10);
            List<Key> key_parts = new List<Key>();
            foreach (var kp in keyValues)
            {
                // Break key into parts
                IEnumerable<object> parts = parametrizer.Break(kp.Key);
                if (parts != null)
                {
                    foreach (object part in parts)
                    {
                        bool isCanonical = parametrizer.IsCanonical(part);
                        foreach (string parameterName in parametrizer.GetPartParameters(part))
                        {
                            string value = parametrizer.GetPartValue(part, parameterName);
                            if (value == null) continue;
                            bool isNonCanonical = parametrizer.IsNonCanonical(part, parameterName);
                            partList.Add(new PartComparer.Part(parameterName, value, isCanonical, isNonCanonical));
                        }
                    }
                }
                // Reorder parts according to grouping rule
                partList.Sort(partComparer);

                // Segment according to grouping rule
                if (groupingRule == null)
                {
                    key_parts.AddRange(partList.Select(p => new Key(p.name, p.value)));
                } else
                {
                    int part_ix = 0;
                    Key constructedKey = null;
                    foreach(var part in groupingRule.AllParts)
                    {
                        // yield constructedKey into the array due to separator
                        if (constructedKey != null && part.PrefixSeparator.Contains("/")) { key_parts.Add(constructedKey); constructedKey = null; }

                        // Is not a capture part
                        if (part.CaptureIndex<0) { if (constructedKey != null) { key_parts.Add(constructedKey); constructedKey = null; } continue; }

                        // Look ahead to see if there is part for this parameter name
                        int ixx = -1;
                        for(int ix=part_ix; ix<partList.Count; ix++)
                        {
                            // Detected part
                            if (partList[ix].name == part.ParameterName) { ixx = ix; break; }
                        }

                        // Detected part for parameter name in the grouping rule
                        if (ixx>=0) for (;part_ix<=ixx;part_ix++) constructedKey = new Key(constructedKey, partList[part_ix].name, partList[part_ix].value);

                        // yield constructedKey into the array due to separator
                        if (constructedKey != null && part.PrefixSeparator.Contains("/")) { key_parts.Add(constructedKey); constructedKey = null; }
                    }

                    // yield constructedKey into the array.
                    if (constructedKey != null) { key_parts.Add(constructedKey); constructedKey = null; }

                    // Add rest of the keys
                    for (; part_ix <= partList.Count; part_ix++)
                        key_parts.Add(new Key(constructedKey, partList[part_ix].name, partList[part_ix].value));
                }

                // Add recursively
                Add(key_parts, kp.Value);

                // Clear
                partList.Clear();
                key_parts.Clear();
            }

            return this;
        }


        public KeyTree Add(Key key, string value)
        {
            KeyTree node = this;
            if (key != null) node = node.GetOrCreateChild(key);
            if (value != null) node.Values.Add(value);
            return node;
        }

        public KeyTree Add(IEnumerable<Key> key_parts, string value)
        {
            KeyTree node = this;
            // Add-or-get section
            foreach (Key key in key_parts)
                node = node.GetOrCreateChild(key);
            if (value!=null) node.Values.Add(value);
            return node;
        }

        /// <summary>
        /// Get-or-create immediate child.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public KeyTree GetOrCreateChild(Key key)
        {
            KeyTree subsection;
            if (!Children.TryGetValue(key, out subsection))
                Children[key] = subsection = new KeyTree(this, key);
            return subsection;
        }

        public IEnumerable<KeyValuePair<Key, string>> ToKeyValues(bool skipRoot)
        {
            Queue<(KeyTree, Key)> queue = new Queue<(KeyTree, Key)>();
            queue.Enqueue((this, skipRoot&&this.Key.Name=="root"?null:this.Key));
            while(queue.Count>0)
            {
                // Dequue next element
                (KeyTree, Key) current = queue.Dequeue();

                // Yield values
                if (current.Item2 != null && current.Item1.HasValues)
                {
                    foreach (string value in current.Item1.Values)
                        yield return new KeyValuePair<Key, string>(current.Item2, value);
                }

                // Enqueue children
                if (current.Item1.HasChildren)
                {
                    foreach(KeyTree child in current.Item1.Children.Values)
                    {
                        Key childKey = current.Item2 == null ? child.Key : current.Item2.Concat(child.Key);
                        queue.Enqueue((child, childKey));
                    }
                }
            }
        }

        public override string ToString()
        {
            if (HasValues)
            {
                if (Values.Count == 1)
                    return $"{GetType().Name}({Key}={Values.First()})";
                else
                    return $"{GetType().Name}({Key}={String.Join(",", Values)})";
            } else
            {
                return $"{GetType().Name}({Key})";
            }
        }

        public class Parametrizer : IAssetKeyParametrizer
        {            
            static private Parametrizer instance = new Parametrizer();
            public static Parametrizer Instance => instance;

            public IEnumerable<object> Break(object obj)
            {
                KeyTree node = obj as KeyTree;
                if (node == null) return null;

                // Count
                int count = 0;
                for (KeyTree n = node; n != null; n = n.Parent)
                    count++;

                // Array from root to tail
                object[] result = new object[count];
                int ix = 0;
                for (KeyTree k = node; k != null; k = k.Parent)
                    result[ix++] = k;
                return result;
            }

            public object GetPreviousPart(object part)
                => part is KeyTree tree ? tree.Parent : null;

            public object TryCreatePart(object obj, string parameterName, string parameterValue)
                => (obj as KeyTree)?.GetOrCreateChild(new Key(parameterName, parameterValue));

            public bool IsCanonical(object part, string parameterName)
            {
                if (part is KeyTree node)
                {
                    for (Key k = node.Key; k != null; k = k.Previous)
                        if (k.Name == parameterName && k is Key.NonCanonical) return true;
                }
                return false;
            }
            public bool IsNonCanonical(object part, string parameterName)
            {
                if (part is KeyTree node)
                {
                    for (Key k = node.Key; k != null; k = k.Previous)
                        if (k.Name == parameterName && k is Key) return true;
                }
                return false;
            }

            static string[] empty = new string[0];
            public string[] GetPartParameters(object obj)
                => obj is KeyTree tree ? tree.Key.ToKeyArray() : empty;

            public string GetPartValue(object obj, string parameter)
            {
                if (obj is KeyTree node)
                {
                    for (Key k = node.Key; k != null; k = k.Previous)
                        if (k.Name == parameter) return k.Value;
                }
                return null;
            }

            public void VisitParts<T>(object obj, ParameterPartVisitor<T> visitor, ref T data)
            {
                KeyTree key = (obj as KeyTree);
                if (key == null) return;

                // Push to stack
                KeyTree prevKey = key.Parent;
                if (prevKey != null) VisitParts(prevKey, visitor, ref data);

                // Pop from stack in reverse order
                visitor(key, ref data);
            }

        }

        /// <summary>
        /// TreeNode is an intermediate model for writing text files
        /// 
        /// Reorganize parts so that non-canonicals parts, so that "root" is first, then "culture", and then others by parameter name.
        /// </summary>
        class PartComparer : IComparer<PartComparer.Part>
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
            }
        }


    }

}
