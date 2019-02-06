// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Asset
{
    /// <summary>
    /// Makes copies of keys. Copies parts that parametrizer recognizes.
    /// </summary>
    public class AssetKeyCloner
    {
        public readonly IAssetKeyParametrizer Parametrizer;

        public readonly AssetKeyProxy Root = new AssetKeyProxy("", "");
        ParameterPartVisitor<AssetKeyProxy> visitor;

        static AssetKeyProxy.Parametrizer proxyParametrizer = AssetKeyProxy.Parametrizer.Default;

        /// <summary>
        /// Create new cloner.
        /// </summary>
        /// <param name="parametrizer">object that reads parameters from <see cref="IAssetKey" /></param>
        public AssetKeyCloner(IAssetKeyParametrizer parametrizer)
        {
            this.Parametrizer = parametrizer ?? throw new ArgumentNullException(nameof(parametrizer));
            this.visitor = Visitor;
        }

        /// <summary>
        /// Create a proxy copy of the key. 
        /// 
        /// Proxy copy doesn't have any of the interface features, just parameter name + value pairs.
        /// Proxy result must be parametrized with <see cref="AssetKeyProxy.Parametrizer"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>copy of every part the parametrizer extracted</returns>
        public IAssetKey Copy(IAssetKey key)
        {
            AssetKeyProxy result = null;
            if (key != null) Parametrizer.VisitParts(key, visitor, ref result);
            return result ?? Root;
        }

        void Visitor(object obj, ref AssetKeyProxy result)
        {
            IAssetKey part = obj as IAssetKey;
            if (part == null) return;

            // Pop from stack in reverse order
            string[] parameterNames = Parametrizer.GetPartParameters(part);
            if (parameterNames == null || parameterNames.Length==0) return;
            foreach (string parameterName in parameterNames)
            {
                string parameterValue = Parametrizer.GetPartValue(part, parameterName);
                if (parameterValue == null) continue;
                AssetKeyProxy newKey = proxyParametrizer.TryCreatePart(result, parameterName, parameterValue, part is IAssetKeyNonCanonicallyCompared == false) as AssetKeyProxy;
                if (newKey != null) result = newKey;
            }
        }

    }
}
