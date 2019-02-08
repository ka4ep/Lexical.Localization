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
        public T this[int index] { get => Array[index]; set { lock (m_lock) { list[index] = value; cow = null; } } }

        public int Count => Array.Length;

        public virtual bool IsReadOnly => false;

        protected internal object m_lock = new object();

        /// <summary>
        /// Copy-on-write snapshot. 
        /// </summary>
        protected T[] cow;

        /// <summary>
        /// Get-or-create copy-on-write snapshot.
        /// </summary>
        public T[] Array => cow ?? BuildCow();

        protected List<T> list;

        public CopyOnWriteList()
        {
            list = new List<T>();
        }

        public CopyOnWriteList(IEnumerable<T> strsEnumr)
        {
            this.list = new List<T>(strsEnumr);
        }

        /// <summary>
        /// Immutable version of composition where components can only be added in the constructor.
        /// </summary>
        public class Immutable : CopyOnWriteList<T>
        {
            public override bool IsReadOnly => true;
            public Immutable() : base() { }
            public Immutable(IEnumerable<T> strsEnumr) : base(strsEnumr) { }
            public override void Insert(int index, T item) => throw new InvalidOperationException("Immutable");
            public override bool Remove(T item) => throw new InvalidOperationException("Immutable");
            public override void RemoveAt(int index) => throw new InvalidOperationException("Immutable");
            public override void Add(T item) => throw new InvalidOperationException("Immutable");
            public override void Clear() => throw new InvalidOperationException("Immutable");
        }

        protected virtual T[] BuildCow()
        {
            lock (m_lock) return cow = list.ToArray();
        }
        protected virtual void ClearCache()
        {
            this.cow = null;
        }

        public virtual void Add(T item)
        {
            lock (m_lock)
            {
                list.Add(item);
                ClearCache();
            }
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            lock (m_lock)
            {
                list.AddRange(items);
                ClearCache();
            }
        }

        public virtual void Clear()
        {
            lock (m_lock)
            {
                if (list.Count == 0) return;
                list.Clear();
                ClearCache();
            }
        }

        public bool Contains(T item)
        {
            var snapshot = Array;
            for (int ix = 0; ix < snapshot.Length; ix++)
                if (snapshot[ix].Equals(item)) return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var snapshot = Array;
            System.Array.Copy(snapshot, 0, array, arrayIndex, snapshot.Length);
        }

        public int IndexOf(T item)
        {
            var snapshot = Array;
            for (int ix = 0; ix < snapshot.Length; ix++)
                if (snapshot[ix].Equals(item)) return ix;
            return -1;
        }

        public virtual void Insert(int index, T item)
        {
            lock (m_lock)
            {
                list.Insert(index, item);
                ClearCache();
            }
        }

        public void CopyFrom(IEnumerable<T> newContent)
        {
            lock (m_lock)
            {
                list.Clear();
                list.AddRange(newContent);
                ClearCache();
            }
        }

        public virtual bool Remove(T item)
        {
            lock (m_lock)
            {
                bool removed = list.Remove(item);
                if (removed) ClearCache();
                return removed;
            }
        }

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
        IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

        /// <summary>
        /// Returns an enumerator to a copy-on-write list. 
        /// Therefore it can't throw ConcurrentModicationException.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Array).GetEnumerator();

    }
}
