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
        public readonly IAssetKeyParametrizer ReaderParametrizer;
        public readonly IAssetKeyParametrizer WriterParametrizer;

        public IAssetKey Root = new ParameterKey("", "");
        ParameterPartVisitor<ParameterKey> visitor1;
        ParameterPartVisitor<IAssetKey> visitor2;

        /// <summary>
        /// List of parameters not to clone.
        /// </summary>
        HashSet<string> parameterNamesToExclude = new HashSet<string>();

        /// <summary>
        /// Create new cloner.
        /// </summary>
        /// <param name="readerParametrizer">object that reads parameters from <see cref="IAssetKey" /></param>
        public AssetKeyCloner(IAssetKeyParametrizer readerParametrizer, IAssetKeyParametrizer writerParametrizer)
        {
            this.ReaderParametrizer = readerParametrizer ?? throw new ArgumentNullException(nameof(readerParametrizer));
            this.WriterParametrizer = writerParametrizer ?? throw new ArgumentNullException(nameof(writerParametrizer));
            this.visitor1 = Visitor1;
            this.visitor2 = Visitor2;
        }

        /// <summary>
        /// Set root, might be needed for cloning.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public AssetKeyCloner SetRoot(IAssetKey root)
        {
            this.Root = root;
            return this;
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
        /// Create a proxy copy of the key. 
        /// 
        /// Proxy copy doesn't have any of the interface features, just parameter name + value pairs.
        /// Proxy result must be parametrized with <see cref="ParameterKey.Parametrizer"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>copy of every part the parametrizer extracted</returns>
        public IAssetKey Copy(IAssetKey key)
        {
            if (WriterParametrizer is ParameterKey.Parametrizer proxyWriter)
            {
                ParameterKey result = null;
                if (key != null) ReaderParametrizer.VisitParts<ParameterKey>(key, visitor1, ref result);
                return result ?? Root;
            }
            else
            {
                IAssetKey result = Root;
                if (key != null) ReaderParametrizer.VisitParts<IAssetKey>(key, visitor2, ref result);
                return result ?? Root;
            }
        }

        void Visitor1(object obj, ref ParameterKey result)
        {
            IAssetKey part = obj as IAssetKey;
            if (part == null) return;

            // Pop from stack in reverse order
            string[] parameterNames = ReaderParametrizer.GetPartParameters(part);
            if (parameterNames == null || parameterNames.Length==0) return;
            foreach (string parameterName in parameterNames)
            {
                if (parameterNamesToExclude.Contains(parameterName)) continue;
                string parameterValue = ReaderParametrizer.GetPartValue(part, parameterName);
                if (parameterValue == null) continue;
                ParameterKey newKey = null;

                newKey = ((ParameterKey.Parametrizer)WriterParametrizer).TryCreatePart(result, parameterName, parameterValue, part is IAssetKeyNonCanonicallyCompared == false) as ParameterKey;
                if (newKey != null) result = newKey;
            }
        }

        void Visitor2(object obj, ref IAssetKey result)
        {
            IAssetKey part = obj as IAssetKey;
            if (part == null) return;

            // Pop from stack in reverse order
            string[] parameterNames = ReaderParametrizer.GetPartParameters(part);
            if (parameterNames == null || parameterNames.Length == 0) return;
            foreach (string parameterName in parameterNames)
            {
                if (parameterNamesToExclude.Contains(parameterName)) continue;
                string parameterValue = ReaderParametrizer.GetPartValue(part, parameterName);
                if (parameterValue == null) continue;
                IAssetKey newKey = null;

                newKey = WriterParametrizer.TryCreatePart(result, parameterName, parameterValue) as IAssetKey;
                if (newKey != null) result = newKey;
            }
        }

    }
}
