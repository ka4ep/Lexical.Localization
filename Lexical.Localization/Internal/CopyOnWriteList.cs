// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// This class is a simple list of T. Modifications are done under a lock. 
    /// 
    /// There is an internal copy-on-write array that is recreated on reading.
    /// Reading is done from the copy-on-write snapshot.
    /// Reading while writing is synchronous in acid sense.
    /// </summary>
    public class CopyOnWriteList<T> : IList<T>
    {
        /// <summary>
        /// Get or set an element at <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] {
            get => Array[index];
            set { lock (m_lock) { list[index] = value; snapshot = null; } }
        }

        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get
            {
                var _snaphost = snapshot;
                if (_snaphost != null) return _snaphost.Length;
                lock (m_lock) return list.Count;
            }
        }

        /// <summary>
        /// Is in readonly state
        /// </summary>
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Synchronize object
        /// </summary>
        protected internal object m_lock = new object();

        /// <summary>
        /// a snapshot. 
        /// </summary>
        protected T[] snapshot;

        /// <summary>
        /// Get-or-create a snapshot. Make a new copy if write has occured.
        /// </summary>
        public T[] Array 
            => snapshot ?? BuildCow();

        /// <summary>
        /// List of elements
        /// </summary>
        protected List<T> list;

        /// <summary>
        /// Create copy-on-write list
        /// </summary>
        public CopyOnWriteList()
        {
            list = new List<T>();
        }

        /// <summary>
        /// Create copy-on-write list.
        /// </summary>
        /// <param name="strsEnumr"></param>
        public CopyOnWriteList(IEnumerable<T> strsEnumr)
        {
            this.list = new List<T>(strsEnumr);
        }

        /// <summary>
        /// Immutable version of composition where components can only be added in the constructor.
        /// </summary>
        public class Immutable : CopyOnWriteList<T>
        {
            /// <summary>
            /// Is read-only
            /// </summary>
            public override bool IsReadOnly => true;

            /// <summary>
            /// Create immutable version of copy-on-write-list.
            /// </summary>
            public Immutable() : base() { }

            /// <summary>
            /// Create immutable version of copy-on-write-list.
            /// </summary>
            /// <param name="strsEnumr"></param>
            public Immutable(IEnumerable<T> strsEnumr) : base(strsEnumr) { }

            /// <summary>
            /// Insert
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public override void Insert(int index, T item) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// Remove
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public override bool Remove(T item) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// Remove At
            /// </summary>
            /// <param name="index"></param>
            public override void RemoveAt(int index) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// Add
            /// </summary>
            /// <param name="item"></param>
            public override void Add(T item) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// Clear
            /// </summary>
            public override void Clear() => throw new InvalidOperationException("Immutable");
        }

        /// <summary>
        /// Construct array
        /// </summary>
        /// <returns></returns>
        protected virtual T[] BuildCow()
        {
            lock (m_lock) return snapshot = list.ToArray();
        }

        /// <summary>
        /// Clear last array
        /// </summary>
        protected virtual void ClearCache()
        {
            this.snapshot = null;
        }

        /// <summary>
        /// Add element
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item)
        {
            lock (m_lock)
            {
                list.Add(item);
                ClearCache();
            }
        }

        /// <summary>
        /// Add elements
        /// </summary>
        /// <param name="items"></param>
        public virtual void AddRange(IEnumerable<T> items)
        {
            lock (m_lock)
            {
                list.AddRange(items);
                ClearCache();
            }
        }

        /// <summary>
        /// Clear elements
        /// </summary>
        public virtual void Clear()
        {
            lock (m_lock)
            {
                if (list.Count == 0) return;
                list.Clear();
                ClearCache();
            }
        }

        /// <summary>
        /// Test if contains element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            var snapshot = Array;
            for (int ix = 0; ix < snapshot.Length; ix++)
                if (snapshot[ix].Equals(item)) return true;
            return false;
        }

        /// <summary>
        /// Copy elements to array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            var snapshot = Array;
            System.Array.Copy(snapshot, 0, array, arrayIndex, snapshot.Length);
        }

        /// <summary>
        /// Index of element.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            var snapshot = Array;
            for (int ix = 0; ix < snapshot.Length; ix++)
                if (snapshot[ix].Equals(item)) return ix;
            return -1;
        }

        /// <summary>
        /// Insert element
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public virtual void Insert(int index, T item)
        {
            lock (m_lock)
            {
                list.Insert(index, item);
                ClearCache();
            }
        }

        /// <summary>
        /// Copy elements from <paramref name="newContent"/>.
        /// </summary>
        /// <param name="newContent"></param>
        public void CopyFrom(IEnumerable<T> newContent)
        {
            lock (m_lock)
            {
                list.Clear();
                list.AddRange(newContent);
                ClearCache();
            }
        }

        /// <summary>
        /// Remove element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Remove(T item)
        {
            lock (m_lock)
            {
                bool removed = list.Remove(item);
                if (removed) ClearCache();
                return removed;
            }
        }

        /// <summary>
        /// Remove element at index.
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveAt(int index)
        {
            lock (m_lock)
            {
                list.RemoveAt(index);
                ClearCache();
            }
        }

        /// <summary>
        /// Returns an enumerator to a copy-on-write list. 
        /// Therefore it can't throw ConcurrentModicationException.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() 
            => Array.GetEnumerator();

        /// <summary>
        /// Returns an enumerator to a copy-on-write list. 
        /// Therefore it can't throw ConcurrentModicationException.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() 
            => ((IEnumerable<T>)Array).GetEnumerator();

    }
}
