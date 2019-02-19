// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains a single parameter name and value. Is also a linked list.
    /// </summary>
    public class ParameterKey : IAssetKey, IAssetKeyLinked, IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Parameter name, e.g. "culture"
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Parameter value.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Link to previous key in a linked list.
        /// </summary>
        public ParameterKey Previous;

        IAssetKey IAssetKeyLinked.PreviousKey => Previous;
        string IAssetKey.Name => Name;

        /// <summary>
        /// Cached parameters array.
        /// </summary>
        string[] parameters;

        static string[] empty = new string[0];

        /// <summary>
        /// Parameters array. Contains one or no elements.
        /// </summary>
        public string[] Parameters => parameters ?? (parameters = String.IsNullOrEmpty(Name) ? empty : new string[] { Name });

        /// <summary>
        /// Create proxy root implementation of <see cref="IAssetKey"/>. Contains one parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public ParameterKey(string parameterName, string parameterValue)
        {
            this.Name = parameterName;
            this.Value = parameterValue;
        }

        /// <summary>
        /// Create proxy implementation of <see cref="IAssetKey"/>. Contains one parameter.
        /// </summary>
        /// <param name="previous">(optional) previous link</param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public ParameterKey(ParameterKey previous, string parameterName, string parameterValue)
        {
            this.Name = parameterName;
            this.Value = parameterValue;
            this.Previous = previous;
        }

        /// <summary>
        /// Create a new link in a new reference.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name=""></param>
        /// <returns>new reference with a new key</returns>
        public ParameterKey Append(string parameterName, string parameterValue)
            => new ParameterKey(this, parameterName, parameterValue);

        /// <summary>
        /// Proxy implementation of non-canonical parameter. Implements <see cref="IAssetKeyNonCanonicallyCompared"/>.
        /// </summary>
        public class NonCanonical : ParameterKey, IAssetKeyNonCanonicallyCompared
        {
            public NonCanonical(string parameterName, string parameterValue) : base(parameterName, parameterValue) { }
            public NonCanonical(ParameterKey previous, string parameterName, string parameterValue) : base(previous, parameterName, parameterValue) { }
        }

        public override string ToString()
            => AssetKeyNameProvider.Default.BuildName(this, Parametrizer.Default);

        /// <summary>
        /// Asset key comparer.
        /// </summary>
        public class Comparer : IEqualityComparer<ParameterKey>, IComparer<ParameterKey>
        {
            private static Comparer instance = new Comparer();
            private static IEqualityComparer<ParameterKey[]> arrayComparer = new ArrayComparer<ParameterKey>(new Comparer());

            public static Comparer Default => instance;
            public static IEqualityComparer<ParameterKey[]> Array => arrayComparer;

            public readonly IComparer<string> parameterNameComparer;
            public readonly IComparer<string> parameterValueComparer;

            public Comparer(IComparer<string> parameterNameComparer = default, IComparer<string> parameterValueComparer = default)
            {
                this.parameterNameComparer = parameterNameComparer ?? StringComparer.InvariantCultureIgnoreCase;
                this.parameterValueComparer = parameterValueComparer ?? AlphaNumericComparer.Default;
            }

            public int Compare(ParameterKey x, ParameterKey y)
            {
                string x_comparand = x.Name, y_comparand = y.Name;
                int o = parameterNameComparer.Compare(x_comparand, y_comparand);
                if (o != 0) return -o;
                o = parameterValueComparer.Compare(x.Value, y.Value);
                return o;
            }

            public bool Equals(ParameterKey x, ParameterKey y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Name == y.Name && x.Value == y.Value;
            }

            public int GetHashCode(ParameterKey obj)
            {
                int hash = 24342;
                if (obj.Value != null) { hash ^= obj.Value.GetHashCode(); hash *= 137; }
                if (obj.Name != null) { hash ^= obj.Name.GetHashCode(); hash *= 137; }
                return hash;
            }
        }

        public class Parametrizer : IAssetKeyParametrizer
        {
            private static Parametrizer instance = new Parametrizer();
            public static Parametrizer Default => instance;

            public IEnumerable<object> Break(object obj)
            {
                ParameterKey key = obj as ParameterKey;
                if (key == null) return null;

                // Count
                int count = 0;
                for (ParameterKey k = key; k != null; k = k.Previous)
                    count++;

                // Array from root to tail
                object[] result = new object[count];
                int ix = count;
                for (ParameterKey k = key; k != null; k = k.Previous)
                    result[--ix] = k;

                return result;
            }            

            public string[] GetPartParameters(object obj)
            {
                ParameterKey part = obj as ParameterKey;
                if (part == null) return null;
                return part.Parameters;
            }

            public string GetPartValue(object obj, string parameter)
            {
                ParameterKey part = obj as ParameterKey;
                if (part == null) return null;

                return parameter == part.Name ? part.Value : null;
            }

            public object GetPreviousPart(object part)
                => part is ParameterKey proxy ? proxy.Previous : null;

            public bool IsCanonical(object part, string parameterName)
                => part is ParameterKey proxy && part is ParameterKey.NonCanonical == false;
            public bool IsNonCanonical(object part, string parameterName)
                => part is ParameterKey proxy && part is ParameterKey.NonCanonical;

            public object TryCreatePart(object obj, string parameterName, string parameterValue)
            {
                ParameterKey key = obj as ParameterKey;
                if (key == null) return null;

                return (parameterName != "culture" && parameterName != "root") ?
                    new ParameterKey(key, parameterName, parameterValue) :
                    new ParameterKey.NonCanonical(key, parameterName, parameterValue);
            }

            public object TryCreatePart(object obj, string parameterName, string parameterValue, bool canonical)
            {
                ParameterKey key = obj as ParameterKey;
                if (key == null && obj != null) return null;

                return canonical ?
                    new ParameterKey(key, parameterName, parameterValue) :
                    new ParameterKey.NonCanonical(key, parameterName, parameterValue);
            }

            public void VisitParts<T>(object obj, ParameterPartVisitor<T> visitor, ref T data)
            {
                ParameterKey key = obj as ParameterKey;
                if (key == null) return;

                // Push to stack
                ParameterKey prevKey = key.Previous;
                if (prevKey != null) VisitParts(prevKey, visitor, ref data);

                // Pop from stack in reverse order
                visitor(key, ref data);
            }
        }

        /// <summary>
        /// Create an array of parameters from head towards tail.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includeNonCanonical">include all keys that implement ILocalizationKeyNonCanonicallyCompared</param>
        /// <returns>array of keys</returns>
        public ParameterKey[] ToArray(bool includeNonCanonical = true)
        {
            // Count the number of keys
            int count = 0;
            if (includeNonCanonical)
                for (ParameterKey k = this; k != null; k = k.Previous) count++;
            else
                for (ParameterKey k = this; k != null; k = k.Previous)
                    if (k is IAssetKeyNonCanonicallyCompared == false) count++;

            // Create result
            ParameterKey[] result = new ParameterKey[count];
            int ix = count - 1;
            if (includeNonCanonical)
                for (ParameterKey k = this; k != null; k = k.Previous)
                    result[ix--] = k;
            else
                for (ParameterKey k = this; k != null; k = k.Previous)
                    if (k is IAssetKeyNonCanonicallyCompared == false)
                        result[ix--] = k;

            return result;
        }

        /// <summary>
        /// Enumerate from tail towards head.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => new Enumerator(this);

        /// <summary>
        /// Enumerate from tail towards head.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => new Enumerator(this);

        class Enumerator : IEnumerator<KeyValuePair<string, string>>, IEnumerator
        {
            ParameterKey start, current;
            internal Enumerator(ParameterKey start)
            {
                this.start = start;
            }
            object IEnumerator.Current
                => current == null ? new KeyValuePair<string, string>() : new KeyValuePair<string, string>(current.Name, current.Value);
            KeyValuePair<string, string> IEnumerator<KeyValuePair<string, string>>.Current 
                => current == null ? new KeyValuePair<string, string>() : new KeyValuePair<string, string>(current.Name, current.Value);
            public void Dispose()
                => start = current = null;
            public bool MoveNext()
                => (current = current?.Previous) != null;
            public void Reset()
                => current = start;
        }

    }

}
