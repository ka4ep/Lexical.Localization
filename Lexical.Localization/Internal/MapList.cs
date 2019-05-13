// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           13.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// MapList is a dictionary that has multiple values per key.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class MapList<Key, Value> : Dictionary<Key, List<Value>>, IEnumerable<Value>
    {
        /// <summary>
        /// Create map list.
        /// </summary>
        public MapList() : base() { }

        /// <summary>
        /// Create map list with custom comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public MapList(IEqualityComparer<Key> comparer) : base(comparer) { }

        /// <summary>
        /// Create map list with initial values.
        /// </summary>
        /// <param name="enumr"></param>
        public MapList(IEnumerable<KeyValuePair<Key, Value>> enumr) : base() { AddRange(enumr); }

        /// <summary>
        /// Create map list with initial values.
        /// </summary>
        /// <param name="enumr"></param>
        public MapList(IEnumerable<KeyValuePair<Key, List<Value>>> enumr) : base() { AddRange(enumr); }

        static Value[] empty_value_array = new Value[0];

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MapList<Key, Value> Add(Key key, Value value)
        {
            List<Value> list = null;
            if (!TryGetValue(key, out list)) this[key] = list = new List<Value>(1);
            list.Add(value);
            return this;
        }

        /// <summary>
        /// Remove value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MapList<Key, Value> Remove(Key key, Value value)
        {
            List<Value> list = null;
            if (TryGetValue(key, out list))
            {
                list.Remove(value);
                if (list.Count == 0) Remove(key);
            }
            return this;
        }

        /// <summary>
        /// Get values for a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>values</returns>
        public IEnumerable<Value> GetEnumerable(Key key)
        {
            List<Value> values;
            if (TryGetValue(key, out values)) return values;
            return empty_value_array;
        }

        /// <summary>
        /// Try get internal values list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Value> TryGetList(Key key)
        {
            List<Value> result;
            if (TryGetValue(key, out result)) return result;
            return null;
        }

        /// <summary>
        /// Test if contains key,value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(Key key, Value value)
        {
            List<Value> values;
            return TryGetValue(key, out values) && values.Contains(value);
        }

        /// <summary>
        /// Get internal value list for <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Value> GetList(Key key)
            => this[key];

        /// <summary>
        /// Get-or-create value list for <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Value> GetOrCreateList(Key key)
        {
            List<Value> list = null;
            if (!TryGetValue(key, out list)) this[key] = list = new List<Value>(1);
            return list;
        }

        /// <summary>
        /// Add range of entries.
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        public MapList<Key, Value> AddRange(IEnumerable<KeyValuePair<Key, List<Value>>> toAdd)
        {
            foreach (var pair in toAdd)
            {
                List<Value> values = GetOrCreateList(pair.Key);
                values.AddRange(pair.Value);
            }
            return this;
        }

        /// <summary>
        /// Add range of entries.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public MapList<Key, Value> AddRange(IEnumerable<KeyValuePair<Key, Value>> values)
        {
            foreach (var pair in values)
                GetOrCreateList(pair.Key).Add(pair.Value);
            return this;
        }

        /// <summary>
        /// Enumerate entries.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<Key, Value>> Lines()
        {
            foreach (var line in this)
                foreach (var item in line.Value)
                    yield return new KeyValuePair<Key, Value>(line.Key, item);
        }

        /// <summary>
        /// Get all values
        /// </summary>
        /// <returns></returns>
        IEnumerator<Value> IEnumerable<Value>.GetEnumerator()
        {
            foreach (var line in this)
                foreach (var _value in line.Value)
                    yield return _value;
        }
    }

    /// <summary></summary>
    public static class MapListExtensions
    {
        /// <summary>
        /// Convert to map list.
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Value"></typeparam>
        /// <param name="enumr"></param>
        /// <returns></returns>
        public static MapList<Key, Value> ToMapList<Key, Value>(this IEnumerable<KeyValuePair<Key, Value>> enumr)
            => new MapList<Key, Value>(enumr);

        /// <summary>
        /// Convert to map list.
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Value"></typeparam>
        /// <param name="enumr"></param>
        /// <returns></returns>
        public static MapList<Key, Value> ToMapList<Key, Value>(this IEnumerable<KeyValuePair<Key, List<Value>>> enumr)
            => new MapList<Key, Value>(enumr);
    }

}
