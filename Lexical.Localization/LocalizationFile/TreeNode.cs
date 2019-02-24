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
        public static TreeNode Create(IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
        {
            TreeNode root = new TreeNode(new Key.NonCanonical("root", ""), null);
            root.AddRange(keyValues);
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

        public TreeNode AddRange(IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
        {
            throw new NotImplementedException("Deprecated and to be removed");
        }

        TreeNode getOrCreateChild(string parameterName, string parameterValue)
        {
            TreeNode subsection;
            if (!Children.TryGetValue(parameterValue, out subsection)) Children[parameterValue] = subsection = new TreeNode(new Key(this.Parameter, parameterName, parameterValue), this);
            return subsection;
        }

        public override string ToString()
            => $"{GetType().Name}({Parameter.Name}, {ParameterValue})";

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
