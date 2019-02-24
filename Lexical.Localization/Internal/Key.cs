﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// This class is a context-free implementation of <see cref="IAssetKey"/>. 
    /// It can be used as a reference, but not as a provider of localization content.
    /// It is used for purposes of persisting and comparing keys.
    /// 
    /// This class has one parameter name and a value, and it can carry a link to previous node.
    /// </summary>
    public partial class Key : IAssetKey, IAssetKeyLinked, IAssetKeyParametrized, IAssetKeyParameterAssignable, IEnumerable<KeyValuePair<string, string>>, IEquatable<Key>
    {
        private static readonly Key root = new Key("root", "");
        public static Key Root => root;

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
        public Key Previous;

        IAssetKey IAssetKeyLinked.PreviousKey => Previous;
        string IAssetKey.Name => Value;
        public string ParameterName => Name;

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
        public Key(string parameterName, string parameterValue)
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
        public Key(Key previous, string parameterName, string parameterValue)
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
        public Key Append(string parameterName, string parameterValue)
            => (Key)AppendParameter(parameterName, parameterValue);

        public IAssetKeyParametrized AppendParameter(string parameterName, string parameterValue)
        {
            switch(parameterName)
            {
                case "root": return new Key(this, parameterName, parameterValue);
                case "culture": 
                case "assembly": return new Key.NonCanonical(this, parameterName, parameterValue);
                default: return new Key.Canonical(this, parameterName, parameterValue);
            }
        }

        /// <summary>
        /// Concatenate two keys.
        /// </summary>
        /// <param name="anotherKey"></param>
        /// <returns></returns>
        public Key Concat(Key anotherKey)
        {
            Key result = this;
            if (anotherKey != null)
            {
                foreach (Key k in anotherKey.ToArray())
                    result = Key.Parametrizer.Default.CreatePart(result, k.Name, k.Value) as Key;
            }
            return result;
        }

        /// <summary>
        /// Proxy implementation of non-canonical parameter. Implements <see cref="IAssetKeyNonCanonicallyCompared"/>.
        /// </summary>
        public class NonCanonical : Key, IAssetKeyNonCanonicallyCompared
        {
            public NonCanonical(string parameterName, string parameterValue) : base(parameterName, parameterValue) { }
            public NonCanonical(Key previous, string parameterName, string parameterValue) : base(previous, parameterName, parameterValue) { }
        }

        /// <summary>
        /// Proxy implementation of non-canonical parameter. Implements <see cref="IAssetKeyCanonicallyCompared"/>.
        /// </summary>
        public class Canonical : Key, IAssetKeyCanonicallyCompared
        {
            public Canonical(string parameterName, string parameterValue) : base(parameterName, parameterValue) { }
            public Canonical(Key previous, string parameterName, string parameterValue) : base(previous, parameterName, parameterValue) { }
        }

        public static AssetKeyComparer ChainComparer = new AssetKeyComparer().AddCanonicalParametrizedComparer().AddNonCanonicalParametrizedComparer();

        bool IEquatable<Key>.Equals(Key other)
            => ChainComparer.Equals(this, other);

        public override int GetHashCode()
            => ChainComparer.GetHashCode(this);

        public override bool Equals(object obj)
            => obj is Key other ? ChainComparer.Equals(this, other) : false;

        /// <summary>
        /// Prints the key in "parameterName:parameterValue:..." format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => ParameterNamePolicy.Instance.PrintParameters(this);

        /// <summary>
        /// Comparer that compares the node only, not the whole chain.
        /// </summary>
        public class Comparer : IEqualityComparer<Key>, IComparer<Key>
        {
            private static Comparer instance = new Comparer();
            private static IEqualityComparer<Key[]> arrayComparer = new ArrayComparer<Key>(new Comparer());

            public static Comparer Default => instance;
            public static IEqualityComparer<Key[]> Array => arrayComparer;

            public readonly IComparer<string> parameterNameComparer;
            public readonly IComparer<string> parameterValueComparer;

            public Comparer(IComparer<string> parameterNameComparer = default, IComparer<string> parameterValueComparer = default)
            {
                this.parameterNameComparer = parameterNameComparer ?? StringComparer.InvariantCultureIgnoreCase;
                this.parameterValueComparer = parameterValueComparer ?? AlphaNumericComparer.Default;
            }

            public int Compare(Key x, Key y)
            {
                string x_comparand = x.Name, y_comparand = y.Name;
                int o = parameterNameComparer.Compare(x_comparand, y_comparand);
                if (o != 0) return -o;
                o = parameterValueComparer.Compare(x.Value, y.Value);
                return o;
            }

            public bool Equals(Key x, Key y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Name == y.Name && x.Value == y.Value;
            }

            public int GetHashCode(Key obj)
            {
                int hash = 24342;
                if (obj.Value != null) { hash ^= obj.Value.GetHashCode(); hash *= 137; }
                if (obj.Name != null) { hash ^= obj.Name.GetHashCode(); hash *= 137; }
                return hash;
            }
        }

        public class Parametrizer : IAssetKeyParametrizer
        {
            private static Parametrizer instance = new Parametrizer().AddCanonicalParameterName("root").AddCanonicalParameterName("culture");
            public static Parametrizer Default => instance;

            HashSet<string> nonCanonicalParameterNames = new HashSet<string>();

            public Parametrizer AddCanonicalParameterName(string parameterName)
            {
                nonCanonicalParameterNames.Add(parameterName);
                return this;
            }

            public IEnumerable<object> Break(object obj)
            {
                Key key = obj as Key;
                if (key == null) return null;

                // Count
                int count = 0;
                for (Key k = key; k != null; k = k.Previous)
                    count++;

                // Array from root to tail
                object[] result = new object[count];
                int ix = count;
                for (Key k = key; k != null; k = k.Previous)
                    result[--ix] = k;

                return result;
            }            

            public string[] GetPartParameters(object obj)
            {
                Key part = obj as Key;
                if (part == null) return null;
                return part.Parameters;
            }

            public string GetPartValue(object obj, string parameter)
            {
                Key part = obj as Key;
                if (part == null) return null;

                return parameter == part.Name ? part.Value : null;
            }

            public object GetPreviousPart(object part)
                => part is Key proxy ? proxy.Previous : null;

            public bool IsCanonical(object part, string parameterName)
                => part is Key proxy && part is Key.Canonical == false;
            public bool IsNonCanonical(object part, string parameterName)
                => part is Key proxy && part is Key.NonCanonical;

            public object TryCreatePart(object obj, string parameterName, string parameterValue)
            {
                switch (parameterName)
                {
                    case "root": return new Key(obj as Key, parameterName, parameterValue);
                    case "culture": 
                    case "assembly": return new Key.NonCanonical(obj as Key, parameterName, parameterValue);
                    default: return new Key.Canonical(obj as Key, parameterName, parameterValue);
                }
            }

            public object TryCreatePart(object obj, string parameterName, string parameterValue, bool canonical)
            {
                Key key = obj as Key;
                //if (key == null && obj != null) return null;

                return canonical ?
                    new Key(key, parameterName, parameterValue) :
                    new Key.NonCanonical(key, parameterName, parameterValue);
            }

            public void VisitParts<T>(object obj, ParameterPartVisitor<T> visitor, ref T data)
            {
                Key key = obj as Key;
                if (key == null) return;

                // Push to stack
                Key prevKey = key.Previous;
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
        public Key[] ToArray(bool includeNonCanonical = true)
        {
            // Count the number of keys
            int count = 0;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is IAssetKeyNonCanonicallyCompared) || k is IAssetKeyCanonicallyCompared)
                    count++;
            }

            // Create result
            Key[] result = new Key[count];
            int ix = count;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is IAssetKeyNonCanonicallyCompared) || k is IAssetKeyCanonicallyCompared)
                    result[--ix] = k;
            }

            return result;
        }

        /// <summary>
        /// Enumerate from head towards tail.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, string>>)ToKeyValueArray()).GetEnumerator();

        /// <summary>
        /// Enumerate from head towards tail.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => ToKeyValueArray().GetEnumerator();

        string[] cachedKeyArray;

        public string[] ToKeyArray()
            => cachedKeyArray ?? (cachedKeyArray = _toKeyArray(true));

        public string[] ToKeyArray(bool includeNonCanonical)
            => includeNonCanonical ? ToKeyArray() : _toKeyArray(includeNonCanonical);

        string[] _toKeyArray(bool includeNonCanonical)
        {
            // Count the number of keys
            int count = 0;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is IAssetKeyNonCanonicallyCompared) || k is IAssetKeyCanonicallyCompared)
                    count++;
            }

            if (count == 0) return empty;

            // Create result
            string[] result = new string[count];
            int ix = count;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is IAssetKeyNonCanonicallyCompared) || k is IAssetKeyCanonicallyCompared)
                    result[--ix] = k.Name;
            }

            return result;
        }

        KeyValuePair<string, string>[] cachedKeyValueArray;

        public KeyValuePair<string, string>[] ToKeyValueArray()
            => cachedKeyValueArray ?? (cachedKeyValueArray = _toKeyValueArray(true));

        public KeyValuePair<string, string>[] ToKeyValueArray(bool includeNonCanonical)
            => includeNonCanonical ? ToKeyValueArray() : _toKeyValueArray(includeNonCanonical);

        KeyValuePair<string, string>[] _toKeyValueArray(bool includeNonCanonical)
        {
            // Count the number of keys
            int count = 0;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is IAssetKeyNonCanonicallyCompared) || k is IAssetKeyCanonicallyCompared)
                    count++;
            }

            // Create result
            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[count];
            int ix = count;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is IAssetKeyNonCanonicallyCompared) || k is IAssetKeyCanonicallyCompared)
                    result[--ix] = new KeyValuePair<string, string>(k.Name, k.Value);
            }

            return result;
        }
    }

}