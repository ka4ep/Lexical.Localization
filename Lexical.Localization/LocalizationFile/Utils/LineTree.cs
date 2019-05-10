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
    /// Lines organized into a tree structure.
    /// 
    /// The root key has single key Key("", "").
    /// 
    /// Sub-nodes have one or more keys. For example an .ini file
    /// <code>
    /// [Culture:en:Type:MyController]
    /// key:Success = success
    /// </code>
    /// 
    /// Would be parsed into a tree of following structure.
    ///   LineTree(Key("Root", ""))
    ///       LineTree(Key("Culture", "en).Append("Type", "MyController"))
    ///           LineTree(Key("Key", "Success")
    ///           
    /// </summary>
    [DebuggerDisplay("{DebugPrint()}")]
    public class LineTree : ILineTree
    {
        /// <summary>
        /// Create tree structure from source of flat key values.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="groupingPolicy"></param>
        /// <returns>tree root ""</returns>
        public static LineTree Create(IEnumerable<KeyValuePair<ILine, IFormulationString>> keyValues, ILinePattern groupingPolicy)
        {
            LineTree root = new LineTree(new LinePart(LineAppender.Default, null));
            root.AddRange(keyValues, groupingPolicy);
            return root;
        }

        /// <summary>
        /// Parent node, unless is root then null.
        /// </summary>
        public readonly LineTree Parent;

        /// <summary>
        /// Key of this tree node.
        /// </summary>
        protected ILine key;

        /// <summary>
        /// Associated parameters contained in an instance of Key.
        /// </summary>
        public ILine Key
        {
            get => key;
            set
            {
                //if (value == null) throw new ArgumentNullException(nameof(Key));
                if (key == value) return;

                // Copy key into Key
                ILine oldKey = key;
                if (oldKey != null)
                {
                    if (oldKey.Equals(value)) return;

                    // Update parent's lookup table
                    if (Parent != null && oldKey != null && !LineComparer.Parameters.Equals(oldKey, value))
                    {
                        if (oldKey != null && Parent.ChildrenLookup.ContainsKey(oldKey)) Parent.ChildrenLookup.Remove(oldKey, this);
                        Parent.ChildrenLookup.Add(value, this);
                    }
                }
                this.key = value;
            }
        }

        ILine ILineTree.Key
        {
            get => key;
            set
            {
                //if (value == null) throw new ArgumentNullException(nameof(Key));

                ILine oldKey = key;
                if (oldKey != null && oldKey.Equals(value)) return;
                ILine newKey = value is ILine _key ? _key : value.CloneKey();
                // Add to parent's lookup table
                if (oldKey == null)
                {
                    if (Parent != null) Parent.ChildrenLookup.Add(newKey, this);
                }
                // Update parent's lookup table
                else
                {
                    if (Parent != null && oldKey != null && !LineComparer.Parameters.Equals(oldKey, newKey))
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
        List<IFormulationString> values;

        /// <summary>
        /// Child nodes
        /// </summary>
        List<LineTree> children;

        MapList<ILine, LineTree> childLookup;

        /// <summary>
        /// Test if has child nodes.
        /// </summary>
        public bool HasChildren => children != null && children.Count > 0;

        /// <summary>
        /// Get-or-create child nodes
        /// </summary>
        public List<LineTree> Children => children ?? (children = new List<LineTree>());

        /// <summary>
        /// Get-or-create child nodes
        /// </summary>
        public MapList<ILine, LineTree> ChildrenLookup => childLookup ?? (childLookup = new MapList<ILine, LineTree>(LineComparer.Parameters).AddRange(Children.Where(c=>c.Key!=null).Select(c=>new KeyValuePair<ILine, LineTree>(c.Key, c))));

        /// <summary>
        /// Test if has values.
        /// </summary>
        public bool HasValues => values != null && values.Count > 0;

        /// <summary>
        /// Get-or-create values list.
        /// </summary>
        public List<IFormulationString> Values => values ?? (values = new List<IFormulationString>(1));

        /// <summary>
        /// Create new root.
        /// </summary>
        public LineTree()
        {
            this.Key = new LinePart(LineAppender.Default, null);
        }

        /// <summary>
        /// Create new key tree node.
        /// </summary>
        /// <param name="parameters"></param>
        public LineTree(ILine parameters)
        {
            this.Key = parameters;
        }

        /// <summary>
        /// Create new key tree node.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="values">(optional) value to add</param>
        public LineTree(ILine parameter, params IFormulationString[] values)
        {
            this.Key = parameter;
            if (values != null) this.values = new List<IFormulationString>(values);
        }

        /// <summary>
        /// Create new key tree node.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parameter"></param>
        public LineTree(LineTree parent, ILine parameter)
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
        public LineTree(LineTree parent, ILine parameter, params IFormulationString[] values)
        {
            this.Parent = parent;
            this.Key = parameter;
            this.values = new List<IFormulationString>(values);
        }

        /// <summary>
        /// Get-or-create immediate child.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineTree GetOrCreateChild(ILine key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            List<LineTree> children;
            if (ChildrenLookup.TryGetValue(key, out children) && children.Count > 0) return children.First();

            LineTree child = new LineTree(this, key);
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
            List<LineTree> lookupTarget;
            if (Parent.childLookup != null && Parent.childLookup.TryGetValue(key, out lookupTarget)) lookupTarget.Remove(this);
            if (Parent.children != null) Parent.children.Remove(this);
        }

        ILineTree ILineTree.Parent => Parent;

        IList<IFormulationString> ILineTree.Values => this.Values;
        IReadOnlyCollection<ILineTree> ILineTree.Children => this.Children;
        ILineTree ILineTree.CreateChild() => this.CreateChild();
        static ILineTree[] empty = new ILineTree[0];
        IEnumerable<ILineTree> ILineTree.GetChildren(ILine key)
        {
            if (!HasChildren) return empty;
            IEnumerable<ILineTree> children = ChildrenLookup.TryGetList(key);
            return children ?? (IEnumerable<ILineTree>) empty;
        }

        LineTree CreateChild()
        {
            LineTree result = new LineTree(this, null);
            Children.Add(result);
            return result;
        }

        /// <summary>
        /// Print tree info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (ILineTree tree in this.VisitFromRoot())
            {
                if (i++ > 0) sb.Append("/");
                ILine key = tree.Key;
                if (key == null) continue;
                ParameterParser.Instance.Print(key, sb);
            }

            if (HasValues)
            {
                sb.Append(" [");
                int ix = 0;
                foreach(IFormulationString value in Values)
                {
                    if (ix++ > 0) sb.Append(", ");
                    sb.Append(value.Text);
                }
                sb.Append("]");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Print debug info
        /// </summary>
        /// <returns></returns>
        public virtual string DebugPrint()
        {
            StringBuilder sb = new StringBuilder();
            foreach(ILineTree tree in this.VisitFromRoot())
            {
                if (sb.Length > 0) sb.Append("/");
                ILine key = tree.Key;
                if (key == null) continue;
                ParameterParser.Instance.Print(key, sb);
            }
            return sb.ToString();
        }

    }

}
