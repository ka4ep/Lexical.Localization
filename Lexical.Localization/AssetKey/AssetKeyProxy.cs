// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Proxy implementation of asset key. Contains single parameter name and value.
    /// </summary>
    public class AssetKeyProxy : IAssetKey, IAssetKeyLinked
    {
        /// <summary>
        /// Parameter name, e.g. "culture"
        /// </summary>
        public readonly string ParameterName;

        /// <summary>
        /// Parameter value.
        /// </summary>
        public readonly string ParameterValue;

        /// <summary>
        /// Link to previous key in a linked list.
        /// </summary>
        public AssetKeyProxy Previous;
        public IAssetKey PreviousKey => Previous;
        public string Name => ParameterName;

        /// <summary>
        /// Cached parameters array.
        /// </summary>
        string[] parameters;

        static string[] empty = new string[0];

        /// <summary>
        /// Parameters array. Contains one or no elements.
        /// </summary>
        public string[] Parameters => parameters ?? (parameters = String.IsNullOrEmpty(ParameterName) ? empty : new string[] { ParameterName });

        /// <summary>
        /// Create proxy root implementation of <see cref="IAssetKey"/>. Contains one parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public AssetKeyProxy(string parameterName, string parameterValue)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
        }

        /// <summary>
        /// Create proxy implementation of <see cref="IAssetKey"/>. Contains one parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public AssetKeyProxy(AssetKeyProxy previous, string parameterName, string parameterValue)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
            this.Previous = previous;
        }

        /// <summary>
        /// Proxy implementation of non-canonical parameter. Implements <see cref="IAssetKeyNonCanonicallyCompared"/>.
        /// </summary>
        public class NonCanonical : AssetKeyProxy, IAssetKeyNonCanonicallyCompared
        {
            public NonCanonical(string parameterName, string parameterValue) : base(parameterName, parameterValue) { }
            public NonCanonical(AssetKeyProxy previous, string parameterName, string parameterValue) : base(previous, parameterName, parameterValue) { }
        }

        public override string ToString()
            => AssetKeyNameProvider.Default.BuildName(this, Parametrizer.Default);

        /// <summary>
        /// Asset key comparer.
        /// </summary>
        public class Comparer : IEqualityComparer<AssetKeyProxy>, IComparer<AssetKeyProxy>
        {
            private static Comparer instance = new Comparer();
            private static IEqualityComparer<AssetKeyProxy[]> arrayComparer = new ArrayComparer<AssetKeyProxy>(new Comparer());

            public static Comparer Default => instance;
            public static IEqualityComparer<AssetKeyProxy[]> Array => arrayComparer;

            public readonly IComparer<string> parameterNameComparer;
            public readonly IComparer<string> parameterValueComparer;

            public Comparer(IComparer<string> parameterNameComparer = default, IComparer<string> parameterValueComparer = default)
            {
                this.parameterNameComparer = parameterNameComparer ?? StringComparer.InvariantCultureIgnoreCase;
                this.parameterValueComparer = parameterValueComparer ?? AlphaNumericComparer.Default;
            }

            public int Compare(AssetKeyProxy x, AssetKeyProxy y)
            {
                string x_comparand = x.ParameterName, y_comparand = y.ParameterName;
                int o = parameterNameComparer.Compare(x_comparand, y_comparand);
                if (o != 0) return -o;
                o = parameterValueComparer.Compare(x.ParameterValue, y.ParameterValue);
                return o;
            }

            public bool Equals(AssetKeyProxy x, AssetKeyProxy y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.ParameterName == y.ParameterName && x.ParameterValue == y.ParameterValue;
            }

            public int GetHashCode(AssetKeyProxy obj)
            {
                int hash = 24342;
                if (obj.ParameterValue != null) { hash ^= obj.ParameterValue.GetHashCode(); hash *= 137; }
                if (obj.ParameterName != null) { hash ^= obj.ParameterName.GetHashCode(); hash *= 137; }
                return hash;
            }
        }

        public class Parametrizer : IAssetKeyParametrizer
        {
            private static Parametrizer instance = new Parametrizer();
            public static Parametrizer Default => instance;

            public IEnumerable<object> Break(object obj)
            {
                AssetKeyProxy key = obj as AssetKeyProxy;
                if (key == null) return null;

                // Count
                int count = 0;
                for (AssetKeyProxy k = key; k != null; k = k.Previous)
                    count++;

                // Array from root to tail
                object[] result = new object[count];
                int ix = count;
                for (AssetKeyProxy k = key; k != null; k = k.Previous)
                    result[--ix] = k;

                return result;
            }            

            public string[] GetPartParameters(object obj)
            {
                AssetKeyProxy part = obj as AssetKeyProxy;
                if (part == null) return null;
                return part.Parameters;
            }

            public string GetPartValue(object obj, string parameter)
            {
                AssetKeyProxy part = obj as AssetKeyProxy;
                if (part == null) return null;

                return parameter == part.ParameterName ? part.ParameterValue : null;
            }

            public object GetPreviousPart(object part)
                => part is AssetKeyProxy proxy ? proxy.Previous : null;

            public bool IsCanonical(object part, string parameterName)
                => part is AssetKeyProxy proxy && part is AssetKeyProxy.NonCanonical == false;
            public bool IsNonCanonical(object part, string parameterName)
                => part is AssetKeyProxy proxy && part is AssetKeyProxy.NonCanonical;

            public object TryCreatePart(object obj, string parameterName, string parameterValue)
            {
                AssetKeyProxy key = obj as AssetKeyProxy;
                if (key == null) return null;

                return new AssetKeyProxy(key, parameterName, parameterValue);
            }

            public object TryCreatePart(object obj, string parameterName, string parameterValue, bool canonical)
            {
                AssetKeyProxy key = obj as AssetKeyProxy;
                if (key == null && obj != null) return null;

                return canonical ?
                    new AssetKeyProxy(key, parameterName, parameterValue) :
                    new AssetKeyProxy.NonCanonical(key, parameterName, parameterValue);
            }

            public void VisitParts<T>(object obj, ParameterPartVisitor<T> visitor, ref T data)
            {
                AssetKeyProxy key = obj as AssetKeyProxy;
                if (key == null) return;

                // Push to stack
                AssetKeyProxy prevKey = key.Previous;
                if (prevKey != null) VisitParts(prevKey, visitor, ref data);

                // Pop from stack in reverse order
                visitor(key, ref data);
            }
        }

    }

}
