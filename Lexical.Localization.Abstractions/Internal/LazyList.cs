// -----------------------------------------------------------------
// Copyright:      Toni Kalajainen (toni.kalajainen iki.fi)
// Date:           1.6.2017
// -----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// A cheap structure that is initially reserved from stack with one slot for one slot.
    /// If more slots are needed then allocates a list from heap.
    /// 
    /// Cannot hold null as a meaningful value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct LazyList<T> : IEnumerable<T>
    {
        /// <summary>
        /// Count the number of items. 
        /// </summary>
        public int Count { get { if (itemOrList == null) return 0; List<T> list = itemOrList as List<T>; return list == null ? 1 : list.Count; } }

        /// <summary>
        /// Tests if list is empty
        /// </summary>
        public bool IsEmpty => Count == 0;

        public T FirstItem => itemOrList == null ? default(T) : itemOrList is List<T> ? ((List<T>)itemOrList)[0] : (T)itemOrList;

        /// <summary>
        /// Get item at an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (itemOrList == null) throw new ArgumentOutOfRangeException();
                if (itemOrList is List<T> list) return list[index];
                if (index == 0) return (T)itemOrList;
                throw new ArgumentOutOfRangeException();
            }
            set
            {
                if (itemOrList == null) throw new ArgumentOutOfRangeException();
                if (itemOrList is List<T> list) list[index] = value;
                else if (index == 0) itemOrList = value;
                else new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// First item is assigned as it. Further instances are assigned to a <see cref="List{T}"/>
        /// </summary>
        object itemOrList;

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException("Cannot hold null.");

            // Add first item
            if (itemOrList == null) { itemOrList = item; return; }

            List<T> list = itemOrList as List<T>;

            // Create List
            if (list == null)
            {
                list = new List<T>(3);
                list.Add((T)itemOrList);
                itemOrList = list;
            }

            // Add item
            list.Add(item);
        }

        public void AddRange(ref LazyList<T> items)
        {
            int count = items.Count();
            if (count == 0) return;
            if (items.itemOrList is T value)
            {
                Add(value);
            }
            else if (items.itemOrList is T[] array)
            {
                AddRange(array);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("Cannot hold null.");

            if (items is T[] array)
            {
                if (array.Length == 0) return;
                if (array.Length == 1) { Add(array[0]); return; }
            }
            else if (items is LazyList<T> ll)
            {
                if (ll.Count == 0) return;
                if (ll.Count == 1) { Add(ll.FirstItem); return; }
            }
            else
            {
                int count = items.Count();
                if (count == 0) return;
                if (count == 1) { Add(items.First()); return; }
            }

            // Add many
            if (itemOrList == null)
            {
                itemOrList = new List<T>(items);
            }
            else
            {
                List<T> list = itemOrList as List<T>;
                list.AddRange(items);
            }
        }

        public LazyList<T> Append(T item)
        {
            if (item == null) throw new ArgumentNullException("Cannot hold null.");
            if (itemOrList == null) return new LazyList<T> { itemOrList = item };
            List<T> list = itemOrList as List<T>;
            if (list == null)
            {
                List<T> newList = new List<T>(3);
                newList.Add((T)item);
                return new LazyList<T> { itemOrList = newList };
            }
            else
            {
                List<T> newList = new List<T>(list.Count + 1);
                for (int i = 0; i < list.Count; i++) newList[i] = list[i];
                newList.Add((T)item);
                return new LazyList<T> { itemOrList = newList };
            }
        }

        public void AddIfNew(T item)
        {
            if (item == null) throw new ArgumentNullException("Cannot hold null.");

            // Add first item
            if (itemOrList == null) { itemOrList = item; return; }

            List<T> list = itemOrList as List<T>;

            // Create List
            if (list == null)
            {
                // item already exists
                if (itemOrList.Equals(item)) return;

                list = new List<T>(3);
                list.Add((T)itemOrList);
                itemOrList = list;
                list.Add(item);
                return;
            }

            // Add item
            if (list.Contains(item)) return;
            list.Add(item);
        }

        /// <summary>
        /// Remove last element
        /// </summary>
        /// <returns>true if last element was removed</returns>
        public bool RemoveLast()
        {
            if (itemOrList == null) return false;
            if (itemOrList is T) { itemOrList = null; return true; }
            List<T> list = itemOrList as List<T>;
            if (list != null && list.Count > 0)
            {
                list.RemoveAt(list.Count - 1);
                return true;
            }
            return false;
        }

        public bool Contains(T value)
        {
            if (itemOrList == null) return false;
            if (itemOrList is T _value) return value!=null&&value.Equals(_value);
            if (itemOrList is List<T> list) return list.Contains(value);
            return false;
        }

        public void Clear()
        {
            itemOrList = null;
        }

        /// <summary>
        /// Clear lazy list and export to list
        /// </summary>
        /// <returns></returns>
        public List<T> ClearToList()
        {
            if (itemOrList == null) return new List<T>(0);
            if (itemOrList is T value) return new List<T>() { value };
            if (itemOrList is List<T> list) return list;
            return null; // Unexpected
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (itemOrList == null) return EmptyEnumerable<T>.Instance;
            List<T> list = itemOrList as List<T>;
            return list == null ? new SingleEnumerator<T>((T)itemOrList) : ((IEnumerable<T>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (itemOrList == null) return EmptyEnumerable<T>.Instance;
            List<T> list = itemOrList as List<T>;
            return list == null ? new SingleEnumerator<T>((T)itemOrList) : (IEnumerator)list.GetEnumerator();
        }

    }
    public struct EmptyEnumerable<T> : IEnumerator<T>, IEnumerable<T>
    {
        public static EmptyEnumerable<T> Instance = new EmptyEnumerable<T>();
        public T Current => default(T);
        object IEnumerator.Current => default(T);
        public void Dispose() { }
        public IEnumerator<T> GetEnumerator() => this;
        public bool MoveNext() { return false; }
        public void Reset() { }
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
    public struct SingleEnumerable<T> : IEnumerable<T>
    {
        public T value;
        public SingleEnumerable(T a_value) { this.value = a_value; }
        public SingleEnumerator<T> GetEnumerator() => new SingleEnumerator<T>(value);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new SingleEnumerator<T>(value);
        IEnumerator IEnumerable.GetEnumerator() => new SingleEnumerator<T>(value);
    }

    public struct SingleEnumerator<T> : IEnumerator<T>
    {
        public T value;
        int index;
        public T Current => index == 1 ? value : default(T);
        object IEnumerator.Current => index == 1 ? value : default(T);
        public SingleEnumerator(T a_value) { this.value = a_value; this.index = 0; }
        public void Dispose() { }
        public bool MoveNext() => ++index == 1;
        public void Reset() { index = 0; }
    }
}
