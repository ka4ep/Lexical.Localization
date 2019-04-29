// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           13.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Internal
{
    public class MapList<Key, Value> : Dictionary<Key, List<Value>>
    {
        public MapList() : base() { }
        public MapList(IEqualityComparer<Key> comparer) : base(comparer) { }
        public MapList(IEnumerable<KeyValuePair<Key, Value>> enumr) : base() { AddRange(enumr); }
        public MapList(IEnumerable<KeyValuePair<Key, List<Value>>> enumr) : base() { AddRange(enumr); }

        static Value[] empty_value_array = new Value[0];

        public MapList<Key, Value> Add(Key key, Value value)
        {
            List<Value> list = null;
            if (!TryGetValue(key, out list)) this[key] = list = new List<Value>(1);
            list.Add(value);
            return this;
        }

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

        public IEnumerable<Value> GetEnumerable(Key key)
        {
            List<Value> values;
            if (TryGetValue(key, out values)) return values;
            return empty_value_array;
        }

        public List<Value> TryGetList(Key key)
        {
            List<Value> result;
            if (TryGetValue(key, out result)) return result;
            return null;
        }

        public bool Contains(Key key, Value value)
        {
            List<Value> values;
            return TryGetValue(key, out values) && values.Contains(value);
        }

        public List<Value> GetList(Key key)
            => this[key];

        public List<Value> GetOrCreateList(Key key)
        {
            List<Value> list = null;
            if (!TryGetValue(key, out list)) this[key] = list = new List<Value>(1);
            return list;
        }

        public MapList<Key, Value> AddRange(IEnumerable<KeyValuePair<Key, List<Value>>> toAdd)
        {
            foreach (var pair in toAdd)
            {
                List<Value> values = GetOrCreateList(pair.Key);
                values.AddRange(pair.Value);
            }
            return this;
        }

        public MapList<Key, Value> AddRange(IEnumerable<KeyValuePair<Key, Value>> values)
        {
            foreach(var pair in values)
                GetOrCreateList(pair.Key).Add(pair.Value);
            return this;
        }

        public IEnumerable<KeyValuePair<Key, Value>> Lines()
        {
            foreach (var line in this)
                foreach (var item in line.Value)
                    yield return new KeyValuePair<Key, Value>(line.Key, item);
        }
    }

    public static class MapListExtensions
    {
        public static MapList<Key, Value> ToMapList<Key, Value>(this IEnumerable<KeyValuePair<Key, Value>> enumr)
            => new MapList<Key, Value>(enumr);
        public static MapList<Key, Value> ToMapList<Key, Value>(this IEnumerable<KeyValuePair<Key, List<Value>>> enumr)
            => new MapList<Key, Value>(enumr);
    }

}
