// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Makes copies of keys. Copies parts that parametrizer recognizes.
    /// </summary>
    public class AssetKeyCloner
    {
        /// <summary>
        /// Root from where keys are appended. Determines the class of the constructed keys.
        /// </summary>
        public readonly ILinePart Root;

        /// <summary>
        /// Visitor delegate
        /// </summary>
        LinePartVisitor<ILinePart> visitor;

        /// <summary>
        /// List of parameters not to clone.
        /// </summary>
        HashSet<string> parameterNamesToExclude = new HashSet<string>();

        /// <summary>
        /// Create new cloner.
        /// </summary>
        /// <param name="root">root from which parameters are constructed</param>
        public AssetKeyCloner(ILinePart root)
        {
            this.Root = root ?? throw new ArgumentNullException(nameof(root));
            this.visitor = Visitor;
        }

        /// <summary>
        /// Add parameter to a list of parameters not to clone.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public AssetKeyCloner AddParameterToExclude(string parameterName)
        {
            this.parameterNamesToExclude.Add(parameterName);
            return this;
        }

        /// <summary>
        /// Create a copy of the key. 
        /// </summary>
        /// <param name="key">key to copy. If null root is returned</param>
        /// <returns>copy of every part the parametrizer extracted</returns>
        public ILinePart Copy(ILinePart key)
        {
            if (key == null) return Root;
            ILinePart result = Root;
            key.VisitFromRoot(visitor, ref result);
            return result ?? Root;
        }

        void Visitor(ILinePart key, ref ILinePart result)
        {
            string parameterName = key.GetParameterName(), parameterValue = key.GetParameterValue();
            if (!String.IsNullOrEmpty(parameterName) && parameterValue != null)
            {
                if (parameterNamesToExclude.Contains(parameterName)) return;
                result = result.AppendParameter(parameterName, parameterValue);
            }
        }

    }
}
