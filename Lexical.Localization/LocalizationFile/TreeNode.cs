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

namespace Lexical.Localization.LocalizationFile
{
    /// <summary>
    /// Class where key values are organized in tree structure.
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// Create tree structure from source of flat key values.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="parametrizer"></param>
        /// <returns>tree root ""</returns>
        public static TreeNode Create(IEnumerable<KeyValuePair<object, string>> keyValues, IAssetKeyParametrizer parametrizer)
        {
            TreeNode root = new TreeNode(new Key.NonCanonical("root", ""), null);
            root.AddRange(parametrizer, keyValues);
            return root;
        }

        /// <summary>
        /// Create tree structure from source of flat key values.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static TreeNode Create(IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
        {
            TreeNode root = new TreeNode(new Key.NonCanonical("root", ""), null);
            root.AddRange(AssetKeyParametrizer.Singleton, keyValues.Select(kp=>new KeyValuePair<object, string>(kp.Key, kp.Value)));
            return root;
        }

        /// <summary>
        /// Parent node, unless is root then null.
        /// </summary>
        public readonly TreeNode Parent;

        /// <summary>
        /// 
        /// </summary>
        public readonly Key Parameter;

        string[] parameters;
        List<string> values;
        Dictionary<string, TreeNode> children;

        public string ParameterName => Parameter.Name;
        public string ParameterValue => Parameter.Value;
        public string[] ParameterNames => parameters ?? (parameters = new string[] { ParameterName });

        Key treeKey;
        public Key TreeKey => treeKey ?? (treeKey = (Parent == null ? Key.Root : Parent.TreeKey).Append(ParameterName, ParameterValue));

        public bool HasChildren => children != null && children.Count > 0;
        public Dictionary<string, TreeNode> Children => children ?? (children = new Dictionary<string, TreeNode>(StringComparer.InvariantCulture));

        public bool HasValues => values != null && values.Count > 0;
        public List<string> Values => values ?? (values = new List<string>(1));

        public TreeNode(Key parameter, TreeNode parent = null)
        {
            this.Parameter = parameter;
            this.Parent = parent;
        }

        public TreeNode AddRange(IAssetKeyParametrizer parametrizer, IEnumerable<KeyValuePair<object, string>> keyValues)
        {
            // Create composite paramerizer
            IAssetKeyParametrizer compositeParametrizer = parametrizer is TreeNode.Parametrizer ? parametrizer :
                new AssetKeyParametrizerComposite(parametrizer, TreeNode.Parametrizer.Instance);
            // Create comparer that can compare LocalizationKeyTree and argument's keys
            ParametrizedComparer comparer = ParametrizedComparer.Instance;

            foreach (var kp in keyValues)
            {
                // Break key into parts
                IEnumerable<object> key_parts_enumr = parametrizer.Break(kp.Key);

                // LocalizationKeyTree is an intermediate model for writing text files
                // Reorganize parts so that non-canonicals are first, and in particular "root", then "culture", then others.
                IEnumerable<object> non_canonical = key_parts_enumr.Where(p => !parametrizer.IsCanonical(p)).OrderBy(p => parametrizer.GetPartParameters(p), PartComparer.Default);

                // Filter out keys that are same as this (root)
                non_canonical = non_canonical.Where(part=>!comparer.Equals(this.Parameter, (IAssetKey)part));

                // Add non-canonical parts
                IEnumerable<object> canonical = key_parts_enumr.Where(p => parametrizer.IsCanonical(p));

                // Into array
                object[] key_parts = non_canonical.Concat(canonical).ToArray();

                // Add recursively
                _add(key_parts, 0, parametrizer, kp.Value);
            }

            return this;
        }


        void _add(object[] key_parts, int part_index, IAssetKeyParametrizer parametrizer, string value)
        {
            // Error
            if (part_index >= key_parts.Length) return;

            // Key part
            object key_part = key_parts[part_index];

            // Read parametrs
            string[] parameters = parametrizer.GetPartParameters(key_part);

            // Iterate parameters
            if (parameters == null) return;
            foreach (string parameterName in parameters)
            {
                // Parameter value
                string parameterValue = parametrizer.GetPartValue(key_part, parameterName);
                // Add-or-get section
                TreeNode subsection = getOrCreateChild(parameterName, parameterValue);
                // Add key=value
                if (part_index == key_parts.Length - 1) subsection.Values.Add(value);
                // Recurse
                subsection._add(key_parts, part_index + 1, parametrizer, value);
            }

        }

        TreeNode getOrCreateChild(string parameterName, string parameterValue)
        {
            TreeNode subsection;
            if (!Children.TryGetValue(parameterValue, out subsection)) Children[parameterValue] = subsection = new TreeNode(new Key(this.Parameter, parameterName, parameterValue), this);
            return subsection;
        }

        public override string ToString()
            => $"{GetType().Name}({Parameter.Name}, {ParameterValue})";

        public class Parametrizer : IAssetKeyParametrizer
        {
            static private Parametrizer instance = new Parametrizer();
            public static Parametrizer Instance => instance;

            public IEnumerable<object> Break(object obj)
            {
                TreeNode key = obj as TreeNode;
                if (key == null) return null;

                // Count
                int count = 0;
                for (TreeNode k = key; k != null; k = k.Parent)
                    count++;

                // Array from root to tail
                object[] result = new object[count];
                int ix = 0;
                for (TreeNode k = key; k != null; k = k.Parent)
                    result[ix++] = k;
                return result;
            }

            public object GetPreviousPart(object part)
                => part is TreeNode tree ? tree.Parent : null;

            public object TryCreatePart(object obj, string parameterName, string parameterValue)
                => (obj as TreeNode)?.getOrCreateChild(parameterName, parameterValue);

            public bool IsCanonical(object part, string parameterName)
                => part is TreeNode tree ? tree.Parameter is Key.NonCanonical == false : false;
            public bool IsNonCanonical(object part, string parameterName)
                => part is TreeNode tree ? tree.Parameter is Key.NonCanonical : false;

            static string[] empty = new string[0];
            public string[] GetPartParameters(object obj)
                => obj is TreeNode tree ? tree.ParameterNames : empty;

            public string GetPartValue(object obj, string parameter)
                => obj is TreeNode tree && tree.Parameter!=null && tree.Parameter.Name==parameter ? tree.Parameter.Value : null;

            public void VisitParts<T>(object obj, ParameterPartVisitor<T> visitor, ref T data)
            {
                TreeNode key = (obj as TreeNode);
                if (key == null) return;

                // Push to stack
                TreeNode prevKey = key.Parent;
                if (prevKey != null) VisitParts(prevKey, visitor, ref data);

                // Pop from stack in reverse order
                visitor(key, ref data);
            }

        }

        // LocalizationKeyTree is an intermediate model for writing text files
        // 
        // Add non-canonical parts

        /// <summary>
        /// LocalizationKeyTree is an intermediate model for writing text files
        /// 
        /// Reorganize parts so that non-canonicals parts, so that "root" is first, then "culture", and then others by parameter name.
        /// </summary>
        class PartComparer : IComparer<string[]>
        {
            private static readonly PartComparer instance = new PartComparer();
            public static PartComparer Default => instance;

            public int Compare(string[] x_parameters, string[] y_parameters)
            {
                if (x_parameters == null && y_parameters == null) return 0;
                if (x_parameters == null) return -1;
                if (y_parameters == null) return 1;

                // Search 
                bool x_root = false, x_culture = false, y_root = false, y_culture = false;
                foreach (string x_parameter in x_parameters)
                {
                    x_root = x_parameter == "root";
                    x_culture = x_parameter == "culture";
                }
                foreach (string y_parameter in y_parameters)
                {
                    y_root = y_parameter == "root";
                    y_culture = y_parameter == "culture";
                }
                if (x_root && y_root) return 0;
                if (x_root && !y_root) return -1;
                if (y_root && !x_root) return 1;
                if (x_culture && y_culture) return 0;
                if (x_culture && !y_culture) return -1;
                if (y_culture && !x_culture) return 1;

                int c = Math.Min(x_parameters.Length, y_parameters.Length);
                for(int i=0; i<c; i++)
                {
                    int cc = string.Compare(x_parameters[i], y_parameters[i], StringComparison.InvariantCulture);
                    if (cc != 0) return cc;
                }

                return x_parameters.Length < y_parameters.Length ? -1 : x_parameters.Length > y_parameters.Length ? 1 : 0;
            }
        }

    }

}
