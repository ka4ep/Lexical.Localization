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

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// Keys organized into a tree structure.
    /// 
    /// The root key has single key Key("Root", "").
    /// 
    /// Sub-nodes have one or more keys. For example an .ini file
    /// <code>
    /// [culture:en:type:MyController]
    /// key:Success = success
    /// </code>
    /// 
    /// Would be parsed into a tree of following structure.
    ///   KeyTree(Key("Root", ""))
    ///       KeyTree(Key("Culture", "en).Append("Type", "MyController"))
    ///           KeyTree(Key("Key", "Success")
    ///           
    /// </summary>
    public class KeyTree : IKeyTree
    {
        /// <summary>
        /// Create tree structure from source of flat key values.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="parametrizer"></param>
        /// <returns>tree root ""</returns>
        public static KeyTree Create(IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
        {
            KeyTree root = new KeyTree(new Key("Root", ""), null);
            root.AddRange(keyValues);
            return root;
        }

        /// <summary>
        /// Parent node, unless is root then null.
        /// </summary>
        public readonly KeyTree Parent;

        protected Key key;

        /// <summary>
        /// Associated parameters contained in an instance of Key.
        /// </summary>
        public Key Key
        {
            get => key;
            set
            {
                if (key == value) return;

                // Copy key into Key
                Key oldKey = key;
                key = Key.CreateFrom(value);

                // Update parent's lookup table
                if (Parent != null && oldKey != null && !Key.Comparer.Default.Equals(oldKey, value))
                {
                    if (oldKey != null && Parent.ChildLookup.ContainsKey(oldKey)) Parent.ChildLookup.Remove(oldKey);
                    Parent.ChildLookup[this.Key] = this;
                }
            }
        }

        /// <summary>
        /// Associated values.
        /// </summary>
        List<string> values;

        /// <summary>
        /// Child nodes
        /// </summary>
        List<KeyTree> children;

        Dictionary<IAssetKey, KeyTree> childLookup;

        /// <summary>
        /// Test if has child nodes.
        /// </summary>
        public bool HasChildren => children != null && children.Count > 0;

        /// <summary>
        /// Get-or-create child nodes
        /// </summary>
        public List<KeyTree> Children => children ?? (children = new List<KeyTree>());

        /// <summary>
        /// Get-or-create child nodes
        /// </summary>
        public Dictionary<IAssetKey, KeyTree> ChildLookup => childLookup ?? (childLookup = new Dictionary<IAssetKey, KeyTree>(AssetKeyComparer.Default));

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
        /// Get-or-create immediate child.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public KeyTree GetOrCreateChild(Key key)
        {
            KeyTree child;
            if (!ChildLookup.TryGetValue(key, out child))
            {
                child = child = new KeyTree(this, key);
                if (!ChildLookup.ContainsKey(key)) ChildLookup[Key] = child;
                Children.Add(child);
            }
            return child;
        }

        /// <summary>
        /// Remove self from parent
        /// </summary>
        public void Remove()
        {
            if (Parent == null) throw new InvalidOperationException("Cannot remove root");
            Parent.Children.Remove(this);
            KeyTree lookupTarget;
            if (Parent.childLookup != null && Parent.childLookup.TryGetValue(key, out lookupTarget) && lookupTarget == this)
            {
                // Find new target
                lookupTarget = Parent.Children.Where(c => c.Key.Equals(this.Key)).FirstOrDefault();
                // Update lookup table
                Parent.childLookup[this.Key] = lookupTarget;
            }
        }

        IKeyTree IKeyTree.Parent => Parent;
        IAssetKey IKeyTree.Key
        {
            get => key;
            set
            {
                if (this.key == value) return;
                Key oldKey = this.key;
                // Copy key into Key
                this.key = Key.CreateFrom(value);
                // Update parent's lookup table
                if (Parent != null && !AssetKeyComparer.Default.Equals(oldKey, value))
                {
                    if (oldKey != null && Parent.ChildLookup.ContainsKey(oldKey)) Parent.ChildLookup.Remove(oldKey);
                    Parent.ChildLookup[this.Key] = this;
                }
            }
        }
        ICollection<string> IKeyTree.Values => this.Values;
        IReadOnlyCollection<IKeyTree> IKeyTree.Children => this.Children;
        IKeyTree IKeyTree.CreateChild() => this.CreateChild();
        IKeyTree IKeyTree.GetChild(IAssetKey key) => this.GetChild(key);

        KeyTree CreateChild()
        {
            KeyTree result = new KeyTree(this, null);
            Children.Add(result);
            return result;
        }

        KeyTree GetChild(IAssetKey key)
        {
            if (!HasChildren) return null;
            KeyTree child;
            if (ChildLookup.TryGetValue(key, out child)) return child;
            return null;
        }

        public override string ToString()
        {
            if (HasValues)
            {
                if (Values.Count == 1)
                    return $"{GetType().Name}({Key}={Values.First()})";
                else
                    return $"{GetType().Name}({Key}={String.Join(",", Values)})";
            }
            else
            {
                return $"{GetType().Name}({Key})";
            }
        }

    }

}
