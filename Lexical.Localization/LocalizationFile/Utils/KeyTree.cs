// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
    [DebuggerDisplay("{DebugPrint()}")]
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
            KeyTree root = new KeyTree(new Key("Root", ""));
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
                //if (value == null) throw new ArgumentNullException(nameof(Key));
                if (key == value) return;

                // Copy key into Key
                Key oldKey = key;
                if (oldKey != null)
                {
                    if (oldKey.Equals(value)) return;

                    // Update parent's lookup table
                    if (Parent != null && oldKey != null && !Key.Comparer.Default.Equals(oldKey, value))
                    {
                        if (oldKey != null && Parent.ChildrenLookup.ContainsKey(oldKey)) Parent.ChildrenLookup.Remove(oldKey, this);
                        Parent.ChildrenLookup.Add(value, this);
                    }
                }
                this.key = value;
            }
        }

        IAssetKey IKeyTree.Key
        {
            get => key;
            set
            {
                //if (value == null) throw new ArgumentNullException(nameof(Key));

                Key oldKey = key;
                if (oldKey != null && oldKey.Equals(value)) return;
                Key newKey = value is Key _key ? _key : Key.CreateFrom(value);
                if (oldKey != null)
                {
                    // Update parent's lookup table
                    if (Parent != null && oldKey != null && !Key.Comparer.Default.Equals(oldKey, newKey))
                    {
                        if (oldKey != null && Parent.ChildrenLookup.ContainsKey(oldKey)) Parent.ChildrenLookup.Remove(oldKey, this);
                        Parent.ChildrenLookup.Add(newKey, this);
                    }
                }
                this.key = newKey;
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

        MapList<IAssetKey, KeyTree> childLookup;

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
        public MapList<IAssetKey, KeyTree> ChildrenLookup => childLookup ?? (childLookup = new MapList<IAssetKey, KeyTree>(AssetKeyComparer.Default).AddRange(Children.Select(c=>new KeyValuePair<IAssetKey, KeyTree>(c.Key, c))));

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
        /// <param name="values">(optional) value to add</param>
        public KeyTree(Key parameter, params string[] values)
        {
            this.Key = parameter;
            if (values != null) this.values = new List<string>(values);
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
            if (key == null) throw new ArgumentNullException(nameof(key));
            List<KeyTree> children;
            if (ChildrenLookup.TryGetValue(key, out children) && children.Count > 0) return children.First();

            KeyTree child = new KeyTree(this, key);
            ChildrenLookup.Add(key, child);
            Children.Add(child);
            return child;
        }

        /// <summary>
        /// Remove self from parent
        /// </summary>
        public void Remove()
        {
            if (Parent == null) throw new InvalidOperationException("Cannot remove root");
            Parent.Children.Remove(this);
            List<KeyTree> lookupTarget;
            if (Parent.childLookup != null && Parent.childLookup.TryGetValue(key, out lookupTarget)) lookupTarget.Remove(this);
            if (Parent.children != null) Parent.children.Remove(this);
        }

        IKeyTree IKeyTree.Parent => Parent;

        IList<string> IKeyTree.Values => this.Values;
        IReadOnlyCollection<IKeyTree> IKeyTree.Children => this.Children;
        IKeyTree IKeyTree.CreateChild() => this.CreateChild();
        static IKeyTree[] empty = new IKeyTree[0];
        IEnumerable<IKeyTree> IKeyTree.GetChildren(IAssetKey key)
        {
            if (!HasChildren) return empty;
            IEnumerable<IKeyTree> children = ChildrenLookup.TryGetList(key);
            return children ?? (IEnumerable<IKeyTree>) empty;
        }

        KeyTree CreateChild()
        {
            KeyTree result = new KeyTree(this, null);
            Children.Add(result);
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (IKeyTree tree in this.VisitFromRoot())
            {
                if (i++ > 0) sb.Append("/");
                IAssetKey key = tree.Key;
                if (key == null) continue;
                ParameterNamePolicy.Instance.PrintKey(key, sb, false);
            }

            if (HasValues)
            {
                sb.Append(" [");
                int ix = 0;
                foreach(string value in Values)
                {
                    if (ix++ > 0) sb.Append(", ");
                    sb.Append(value);
                }
                sb.Append("]");
            }

            return sb.ToString();
        }

        public virtual string DebugPrint()
        {
            StringBuilder sb = new StringBuilder();
            foreach(IKeyTree tree in this.VisitFromRoot())
            {
                if (sb.Length > 0) sb.Append("/");
                IAssetKey key = tree.Key;
                if (key == null) continue;
                ParameterNamePolicy.Instance.PrintKey(key, sb, false);
            }
            return sb.ToString();
        }

    }

}
