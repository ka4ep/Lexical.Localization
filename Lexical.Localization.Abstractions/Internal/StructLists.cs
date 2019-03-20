// -----------------------------------------------------------------
// Copyright:      Toni Kalajainen 
// Date:           19.3.2019
// -----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// A list where first 4 elements are struct elements. Rest are allocated from heap if needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct StructList4<T> : IList<T>
    {
        /// <summary>
        /// The number of elements that are stack allocated.
        /// </summary>
        const int StackCount = 4;

        /// <summary>
        /// Number of elements
        /// </summary>
        int count;

        /// <summary>
        /// First elements
        /// </summary>
        T _0, _1, _2, _3;

        /// <summary>
        /// Elements after <see cref="StackCount"/>.
        /// </summary>
        List<T> rest;

        /// <summary>
        /// Element comparer
        /// </summary>
        IEqualityComparer<T> elementComparer;

        /// <summary>
        /// Construct lazy list.
        /// </summary>
        /// <param name="elementComparer"></param>
        public StructList4(IEqualityComparer<T> elementComparer = default)
        {
            this.elementComparer = elementComparer;
            count = 0;
            _0 = default;
            _1 = default;
            _2 = default;
            _3 = default;
            rest = null;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList4`1.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: return _0;
                    case 1: return _1;
                    case 2: return _2;
                    case 3: return _3;
                    default: return rest[index - StackCount];
                }
            }
            set
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: _0 = value; return;
                    case 1: _1 = value; return;
                    case 2: _2 = value; return;
                    case 3: _3 = value; return;
                    default: rest[index - StackCount] = value; return;
                }
            }
        }

        /// <summary>
        /// Number of elements in the list
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Is list readonly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the StructList4`1.
        /// </summary>
        /// <param name="item">The object to add to the StructList4`1.</param>
        /// <exception cref="System.NotSupportedException">The StructList4`1 is read-only.</exception>
        public void Add(T item)
        {
            switch (count)
            {
                case 0: _0 = item; count++; return;
                case 1: _1 = item; count++; return;
                case 2: _2 = item; count++; return;
                case 3: _3 = item; count++; return;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    count++;
                    return;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the StructList4`1.
        /// </summary>
        /// <param name="item">The object to remove from the StructList4`1.</param>
        /// <returns>true if item was successfully removed from the StructList4`1; otherwise, false. This method also returns false if item is not found in the original StructList4`1.</returns>
        public bool Remove(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;

            if (count == 0) return false;
            if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
            if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
            if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
            if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }

            if (rest == null) return false;
            bool removed = rest.Remove(item);
            if (removed) count--;
            return removed;
        }

        /// <summary>
        /// Removes the StructList4`1 item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList4`1.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            if (index <= 0 && count > 1) _0 = _1;
            if (index <= 1 && count > 2) _1 = _2;
            if (index <= 2 && count > 3) _2 = _3;
            if (index <= 3 && count > 4) { _3 = rest[0]; rest.RemoveAt(0); }
            if (index >= StackCount) rest.RemoveAt(index - StackCount);
            count--;
        }

        /// <summary>
        /// Removes and returns the element at the end of the list.
        /// </summary>
        /// <returns>the last element</returns>
        /// <exception cref="InvalidOperationException">If list is empty</exception>
        public T Dequeue()
        {
            if (count == 0) throw new InvalidOperationException();
            int ix = count - 1;
            T result = this[ix];
            RemoveAt(ix);
            return result;
        }

        /// <summary>
        /// Removes all items from the StructList4`1.
        /// </summary>
        /// <exception cref="System.NotSupportedException">The StructList4`1 is read-only.</exception>
        public void Clear()
        {
            if (count >= 1) _0 = default;
            if (count >= 2) _1 = default;
            if (count >= 3) _2 = default;
            if (count >= 4) _3 = default;
            if (rest != null) rest.Clear();
            count = 0;
        }

        /// <summary>
        /// Determines whether the StructList4`1 contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the StructList4`1.</param>
        /// <returns>true if item is found in the StructList4`1; otherwise, false.</returns>
        public bool Contains(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return true;
            if (count >= 2 && comparer.Equals(_1, item)) return true;
            if (count >= 3 && comparer.Equals(_2, item)) return true;
            if (count >= 4 && comparer.Equals(_3, item)) return true;
            if (rest != null) return rest.Contains(item);
            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the StructList4`1.
        /// </summary>
        /// <param name="item">The object to locate in the StructList4`1.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return 0;
            if (count >= 2 && comparer.Equals(_1, item)) return 1;
            if (count >= 3 && comparer.Equals(_2, item)) return 2;
            if (count >= 4 && comparer.Equals(_3, item)) return 3;
            if (rest != null) return rest.IndexOf(item) - StackCount;
            return -1;
        }

        /// <summary>
        /// Inserts an item to the StructList4`1 at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the StructList4`1.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList4`1.</exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
            if (index >= 4) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
            if (index <= 3 && count >= 4) { if (rest == null) rest = new List<T>(); rest.Insert(0, _3); }
            if (index <= 2 && count >= 3) _3 = _2;
            if (index <= 1 && count >= 2) _2 = _1;
            if (index <= 0 && count >= 1) _1 = _0;

            count++;
            this[index] = item;
        }

        /// <summary>
        /// Copies the elements of the StructList4`1 to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from StructList4`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">The number of elements in the source StructList4`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (count > array.Length + arrayIndex) throw new ArgumentException();

            if (count >= 1) array[arrayIndex++] = _0;
            if (count >= 2) array[arrayIndex++] = _1;
            if (count >= 3) array[arrayIndex++] = _2;
            if (count >= 4) array[arrayIndex++] = _3;
            if (rest != null) rest.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

    }

    /// <summary>
    /// A list where first 8 elements are struct elements. Rest are allocated from heap if needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct StructList8<T> : IList<T>
    {
        /// <summary>
        /// The number of elements that are stack allocated.
        /// </summary>
        const int StackCount = 8;

        /// <summary>
        /// Number of elements
        /// </summary>
        int count;

        /// <summary>
        /// First elements
        /// </summary>
        T _0, _1, _2, _3, _4, _5, _6, _7;

        /// <summary>
        /// Elements after <see cref="StackCount"/>.
        /// </summary>
        List<T> rest;

        /// <summary>
        /// Element comparer
        /// </summary>
        IEqualityComparer<T> elementComparer;

        /// <summary>
        /// Construct lazy list.
        /// </summary>
        /// <param name="elementComparer"></param>
        public StructList8(IEqualityComparer<T> elementComparer = default)
        {
            this.elementComparer = elementComparer;
            count = 0;
            _0 = default;
            _1 = default;
            _2 = default;
            _3 = default;
            _4 = default;
            _5 = default;
            _6 = default;
            _7 = default;
            rest = null;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList8`1.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: return _0;
                    case 1: return _1;
                    case 2: return _2;
                    case 3: return _3;
                    case 4: return _4;
                    case 5: return _5;
                    case 6: return _6;
                    case 7: return _7;
                    default: return rest[index - StackCount];
                }
            }
            set
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: _0 = value; return;
                    case 1: _1 = value; return;
                    case 2: _2 = value; return;
                    case 3: _3 = value; return;
                    case 4: _4 = value; return;
                    case 5: _5 = value; return;
                    case 6: _6 = value; return;
                    case 7: _7 = value; return;
                    default: rest[index - StackCount] = value; return;
                }
            }
        }

        /// <summary>
        /// Number of elements in the list
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Is list readonly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the StructList8`1.
        /// </summary>
        /// <param name="item">The object to add to the StructList8`1.</param>
        /// <exception cref="System.NotSupportedException">The StructList8`1 is read-only.</exception>
        public void Add(T item)
        {
            switch (count)
            {
                case 0: _0 = item; count++; return;
                case 1: _1 = item; count++; return;
                case 2: _2 = item; count++; return;
                case 3: _3 = item; count++; return;
                case 4: _4 = item; count++; return;
                case 5: _5 = item; count++; return;
                case 6: _6 = item; count++; return;
                case 7: _7 = item; count++; return;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    count++;
                    return;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the StructList8`1.
        /// </summary>
        /// <param name="item">The object to remove from the StructList8`1.</param>
        /// <returns>true if item was successfully removed from the StructList8`1; otherwise, false. This method also returns false if item is not found in the original StructList8`1.</returns>
        public bool Remove(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;

            if (count == 0) return false;
            if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
            if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
            if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
            if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
            if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
            if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
            if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
            if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }

            if (rest == null) return false;
            bool removed = rest.Remove(item);
            if (removed) count--;
            return removed;
        }

        /// <summary>
        /// Removes the StructList8`1 item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList8`1.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            if (index <= 0 && count > 1) _0 = _1;
            if (index <= 1 && count > 2) _1 = _2;
            if (index <= 2 && count > 3) _2 = _3;
            if (index <= 3 && count > 4) _3 = _4;
            if (index <= 4 && count > 5) _4 = _5;
            if (index <= 5 && count > 6) _5 = _6;
            if (index <= 6 && count > 7) _6 = _7;
            if (index <= 7 && count > 8) { _7 = rest[0]; rest.RemoveAt(0); }
            if (index >= StackCount) rest.RemoveAt(index - StackCount);
            count--;
        }

        /// <summary>
        /// Removes and returns the element at the end of the list.
        /// </summary>
        /// <returns>the last element</returns>
        /// <exception cref="InvalidOperationException">If list is empty</exception>
        public T Dequeue()
        {
            if (count == 0) throw new InvalidOperationException();
            int ix = count - 1;
            T result = this[ix];
            RemoveAt(ix);
            return result;
        }

        /// <summary>
        /// Removes all items from the StructList8`1.
        /// </summary>
        /// <exception cref="System.NotSupportedException">The StructList8`1 is read-only.</exception>
        public void Clear()
        {
            if (count >= 1) _0 = default;
            if (count >= 2) _1 = default;
            if (count >= 3) _2 = default;
            if (count >= 4) _3 = default;
            if (count >= 5) _4 = default;
            if (count >= 6) _5 = default;
            if (count >= 7) _6 = default;
            if (count >= 8) _7 = default;
            if (rest != null) rest.Clear();
            count = 0;
        }

        /// <summary>
        /// Determines whether the StructList8`1 contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the StructList8`1.</param>
        /// <returns>true if item is found in the StructList8`1; otherwise, false.</returns>
        public bool Contains(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return true;
            if (count >= 2 && comparer.Equals(_1, item)) return true;
            if (count >= 3 && comparer.Equals(_2, item)) return true;
            if (count >= 4 && comparer.Equals(_3, item)) return true;
            if (count >= 5 && comparer.Equals(_4, item)) return true;
            if (count >= 6 && comparer.Equals(_5, item)) return true;
            if (count >= 7 && comparer.Equals(_6, item)) return true;
            if (count >= 8 && comparer.Equals(_7, item)) return true;
            if (rest != null) return rest.Contains(item);
            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the StructList8`1.
        /// </summary>
        /// <param name="item">The object to locate in the StructList8`1.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return 0;
            if (count >= 2 && comparer.Equals(_1, item)) return 1;
            if (count >= 3 && comparer.Equals(_2, item)) return 2;
            if (count >= 4 && comparer.Equals(_3, item)) return 3;
            if (count >= 5 && comparer.Equals(_4, item)) return 4;
            if (count >= 6 && comparer.Equals(_5, item)) return 5;
            if (count >= 7 && comparer.Equals(_6, item)) return 6;
            if (count >= 8 && comparer.Equals(_7, item)) return 7;
            if (rest != null) return rest.IndexOf(item) - StackCount;
            return -1;
        }

        /// <summary>
        /// Inserts an item to the StructList8`1 at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the StructList8`1.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList8`1.</exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
            if (index >= 8) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
            if (index <= 7 && count >= 8) { if (rest == null) rest = new List<T>(); rest.Insert(0, _7); }
            if (index <= 6 && count >= 7) _7 = _6;
            if (index <= 5 && count >= 6) _6 = _5;
            if (index <= 4 && count >= 5) _5 = _4;
            if (index <= 3 && count >= 4) _4 = _3;
            if (index <= 2 && count >= 3) _3 = _2;
            if (index <= 1 && count >= 2) _2 = _1;
            if (index <= 0 && count >= 1) _1 = _0;

            count++;
            this[index] = item;
        }

        /// <summary>
        /// Copies the elements of the StructList8`1 to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from StructList8`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">The number of elements in the source StructList8`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (count > array.Length + arrayIndex) throw new ArgumentException();

            if (count >= 1) array[arrayIndex++] = _0;
            if (count >= 2) array[arrayIndex++] = _1;
            if (count >= 3) array[arrayIndex++] = _2;
            if (count >= 4) array[arrayIndex++] = _3;
            if (count >= 5) array[arrayIndex++] = _4;
            if (count >= 6) array[arrayIndex++] = _5;
            if (count >= 7) array[arrayIndex++] = _6;
            if (count >= 8) array[arrayIndex++] = _7;
            if (rest != null) rest.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (count > 4) yield return _4;
            if (count > 5) yield return _5;
            if (count > 6) yield return _6;
            if (count > 7) yield return _7;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (count > 4) yield return _4;
            if (count > 5) yield return _5;
            if (count > 6) yield return _6;
            if (count > 7) yield return _7;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

    }

    /// <summary>
    /// A list where first 12 elements are struct elements. Rest are allocated from heap if needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct StructList12<T> : IList<T>
    {
        /// <summary>
        /// The number of elements that are stack allocated.
        /// </summary>
        const int StackCount = 12;

        /// <summary>
        /// Number of elements
        /// </summary>
        int count;

        /// <summary>
        /// First elements
        /// </summary>
        T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11;

        /// <summary>
        /// Elements after <see cref="StackCount"/>.
        /// </summary>
        List<T> rest;

        /// <summary>
        /// Element comparer
        /// </summary>
        IEqualityComparer<T> elementComparer;

        /// <summary>
        /// Construct lazy list.
        /// </summary>
        /// <param name="elementComparer"></param>
        public StructList12(IEqualityComparer<T> elementComparer = default)
        {
            this.elementComparer = elementComparer;
            count = 0;
            _0 = default;
            _1 = default;
            _2 = default;
            _3 = default;
            _4 = default;
            _5 = default;
            _6 = default;
            _7 = default;
            _8 = default;
            _9 = default;
            _10 = default;
            _11 = default;
            rest = null;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList12`1.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: return _0;
                    case 1: return _1;
                    case 2: return _2;
                    case 3: return _3;
                    case 4: return _4;
                    case 5: return _5;
                    case 6: return _6;
                    case 7: return _7;
                    case 8: return _8;
                    case 9: return _9;
                    case 10: return _10;
                    case 11: return _11;
                    default: return rest[index - StackCount];
                }
            }
            set
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: _0 = value; return;
                    case 1: _1 = value; return;
                    case 2: _2 = value; return;
                    case 3: _3 = value; return;
                    case 4: _4 = value; return;
                    case 5: _5 = value; return;
                    case 6: _6 = value; return;
                    case 7: _7 = value; return;
                    case 8: _8 = value; return;
                    case 9: _9 = value; return;
                    case 10: _10 = value; return;
                    case 11: _11 = value; return;
                    default: rest[index - StackCount] = value; return;
                }
            }
        }

        /// <summary>
        /// Number of elements in the list
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Is list readonly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the StructList12`1.
        /// </summary>
        /// <param name="item">The object to add to the StructList12`1.</param>
        /// <exception cref="System.NotSupportedException">The StructList12`1 is read-only.</exception>
        public void Add(T item)
        {
            switch (count)
            {
                case 0: _0 = item; count++; return;
                case 1: _1 = item; count++; return;
                case 2: _2 = item; count++; return;
                case 3: _3 = item; count++; return;
                case 4: _4 = item; count++; return;
                case 5: _5 = item; count++; return;
                case 6: _6 = item; count++; return;
                case 7: _7 = item; count++; return;
                case 8: _8 = item; count++; return;
                case 9: _9 = item; count++; return;
                case 10: _10 = item; count++; return;
                case 11: _11 = item; count++; return;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    count++;
                    return;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the StructList12`1.
        /// </summary>
        /// <param name="item">The object to remove from the StructList12`1.</param>
        /// <returns>true if item was successfully removed from the StructList12`1; otherwise, false. This method also returns false if item is not found in the original StructList12`1.</returns>
        public bool Remove(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;

            if (count == 0) return false;
            if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
            if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
            if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
            if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
            if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
            if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
            if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
            if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
            if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
            if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }
            if (count >= 11 && comparer.Equals(_10, item)) { RemoveAt(10); return true; }
            if (count >= 12 && comparer.Equals(_11, item)) { RemoveAt(11); return true; }

            if (rest == null) return false;
            bool removed = rest.Remove(item);
            if (removed) count--;
            return removed;
        }

        /// <summary>
        /// Removes the StructList12`1 item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList12`1.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            if (index <= 0 && count > 1) _0 = _1;
            if (index <= 1 && count > 2) _1 = _2;
            if (index <= 2 && count > 3) _2 = _3;
            if (index <= 3 && count > 4) _3 = _4;
            if (index <= 4 && count > 5) _4 = _5;
            if (index <= 5 && count > 6) _5 = _6;
            if (index <= 6 && count > 7) _6 = _7;
            if (index <= 7 && count > 8) _7 = _8;
            if (index <= 8 && count > 9) _8 = _9;
            if (index <= 9 && count > 10) _9 = _10;
            if (index <= 10 && count > 11) _10 = _11;
            if (index <= 11 && count > 12) { _11 = rest[0]; rest.RemoveAt(0); }
            if (index >= StackCount) rest.RemoveAt(index - StackCount);
            count--;
        }

        /// <summary>
        /// Removes and returns the element at the end of the list.
        /// </summary>
        /// <returns>the last element</returns>
        /// <exception cref="InvalidOperationException">If list is empty</exception>
        public T Dequeue()
        {
            if (count == 0) throw new InvalidOperationException();
            int ix = count - 1;
            T result = this[ix];
            RemoveAt(ix);
            return result;
        }

        /// <summary>
        /// Removes all items from the StructList12`1.
        /// </summary>
        /// <exception cref="System.NotSupportedException">The StructList12`1 is read-only.</exception>
        public void Clear()
        {
            if (count >= 1) _0 = default;
            if (count >= 2) _1 = default;
            if (count >= 3) _2 = default;
            if (count >= 4) _3 = default;
            if (count >= 5) _4 = default;
            if (count >= 6) _5 = default;
            if (count >= 7) _6 = default;
            if (count >= 8) _7 = default;
            if (count >= 9) _8 = default;
            if (count >= 10) _9 = default;
            if (count >= 11) _10 = default;
            if (count >= 12) _11 = default;
            if (rest != null) rest.Clear();
            count = 0;
        }

        /// <summary>
        /// Determines whether the StructList12`1 contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the StructList12`1.</param>
        /// <returns>true if item is found in the StructList12`1; otherwise, false.</returns>
        public bool Contains(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return true;
            if (count >= 2 && comparer.Equals(_1, item)) return true;
            if (count >= 3 && comparer.Equals(_2, item)) return true;
            if (count >= 4 && comparer.Equals(_3, item)) return true;
            if (count >= 5 && comparer.Equals(_4, item)) return true;
            if (count >= 6 && comparer.Equals(_5, item)) return true;
            if (count >= 7 && comparer.Equals(_6, item)) return true;
            if (count >= 8 && comparer.Equals(_7, item)) return true;
            if (count >= 9 && comparer.Equals(_8, item)) return true;
            if (count >= 10 && comparer.Equals(_9, item)) return true;
            if (count >= 11 && comparer.Equals(_10, item)) return true;
            if (count >= 12 && comparer.Equals(_11, item)) return true;
            if (rest != null) return rest.Contains(item);
            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the StructList12`1.
        /// </summary>
        /// <param name="item">The object to locate in the StructList12`1.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return 0;
            if (count >= 2 && comparer.Equals(_1, item)) return 1;
            if (count >= 3 && comparer.Equals(_2, item)) return 2;
            if (count >= 4 && comparer.Equals(_3, item)) return 3;
            if (count >= 5 && comparer.Equals(_4, item)) return 4;
            if (count >= 6 && comparer.Equals(_5, item)) return 5;
            if (count >= 7 && comparer.Equals(_6, item)) return 6;
            if (count >= 8 && comparer.Equals(_7, item)) return 7;
            if (count >= 9 && comparer.Equals(_8, item)) return 8;
            if (count >= 10 && comparer.Equals(_9, item)) return 9;
            if (count >= 11 && comparer.Equals(_10, item)) return 10;
            if (count >= 12 && comparer.Equals(_11, item)) return 11;
            if (rest != null) return rest.IndexOf(item) - StackCount;
            return -1;
        }

        /// <summary>
        /// Inserts an item to the StructList12`1 at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the StructList12`1.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList12`1.</exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
            if (index >= 12) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
            if (index <= 11 && count >= 12) { if (rest == null) rest = new List<T>(); rest.Insert(0, _11); }
            if (index <= 10 && count >= 11) _11 = _10;
            if (index <= 9 && count >= 10) _10 = _9;
            if (index <= 8 && count >= 9) _9 = _8;
            if (index <= 7 && count >= 8) _8 = _7;
            if (index <= 6 && count >= 7) _7 = _6;
            if (index <= 5 && count >= 6) _6 = _5;
            if (index <= 4 && count >= 5) _5 = _4;
            if (index <= 3 && count >= 4) _4 = _3;
            if (index <= 2 && count >= 3) _3 = _2;
            if (index <= 1 && count >= 2) _2 = _1;
            if (index <= 0 && count >= 1) _1 = _0;

            count++;
            this[index] = item;
        }

        /// <summary>
        /// Copies the elements of the StructList12`1 to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from StructList12`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">The number of elements in the source StructList12`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (count > array.Length + arrayIndex) throw new ArgumentException();

            if (count >= 1) array[arrayIndex++] = _0;
            if (count >= 2) array[arrayIndex++] = _1;
            if (count >= 3) array[arrayIndex++] = _2;
            if (count >= 4) array[arrayIndex++] = _3;
            if (count >= 5) array[arrayIndex++] = _4;
            if (count >= 6) array[arrayIndex++] = _5;
            if (count >= 7) array[arrayIndex++] = _6;
            if (count >= 8) array[arrayIndex++] = _7;
            if (count >= 9) array[arrayIndex++] = _8;
            if (count >= 10) array[arrayIndex++] = _9;
            if (count >= 11) array[arrayIndex++] = _10;
            if (count >= 12) array[arrayIndex++] = _11;
            if (rest != null) rest.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (count > 4) yield return _4;
            if (count > 5) yield return _5;
            if (count > 6) yield return _6;
            if (count > 7) yield return _7;
            if (count > 8) yield return _8;
            if (count > 9) yield return _9;
            if (count > 10) yield return _10;
            if (count > 11) yield return _11;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (count > 4) yield return _4;
            if (count > 5) yield return _5;
            if (count > 6) yield return _6;
            if (count > 7) yield return _7;
            if (count > 8) yield return _8;
            if (count > 9) yield return _9;
            if (count > 10) yield return _10;
            if (count > 11) yield return _11;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

    }

    /// <summary>
    /// A list where first 16 elements are struct elements. Rest are allocated from heap if needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct StructList16<T> : IList<T>
    {
        /// <summary>
        /// The number of elements that are stack allocated.
        /// </summary>
        const int StackCount = 16;

        /// <summary>
        /// Number of elements
        /// </summary>
        int count;

        /// <summary>
        /// First elements
        /// </summary>
        T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15;

        /// <summary>
        /// Elements after <see cref="StackCount"/>.
        /// </summary>
        List<T> rest;

        /// <summary>
        /// Element comparer
        /// </summary>
        IEqualityComparer<T> elementComparer;

        /// <summary>
        /// Construct lazy list.
        /// </summary>
        /// <param name="elementComparer"></param>
        public StructList16(IEqualityComparer<T> elementComparer = default)
        {
            this.elementComparer = elementComparer;
            count = 0;
            _0 = default;
            _1 = default;
            _2 = default;
            _3 = default;
            _4 = default;
            _5 = default;
            _6 = default;
            _7 = default;
            _8 = default;
            _9 = default;
            _10 = default;
            _11 = default;
            _12 = default;
            _13 = default;
            _14 = default;
            _15 = default;
            rest = null;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList16`1.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: return _0;
                    case 1: return _1;
                    case 2: return _2;
                    case 3: return _3;
                    case 4: return _4;
                    case 5: return _5;
                    case 6: return _6;
                    case 7: return _7;
                    case 8: return _8;
                    case 9: return _9;
                    case 10: return _10;
                    case 11: return _11;
                    case 12: return _12;
                    case 13: return _13;
                    case 14: return _14;
                    case 15: return _15;
                    default: return rest[index - StackCount];
                }
            }
            set
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
                switch (index)
                {
                    case 0: _0 = value; return;
                    case 1: _1 = value; return;
                    case 2: _2 = value; return;
                    case 3: _3 = value; return;
                    case 4: _4 = value; return;
                    case 5: _5 = value; return;
                    case 6: _6 = value; return;
                    case 7: _7 = value; return;
                    case 8: _8 = value; return;
                    case 9: _9 = value; return;
                    case 10: _10 = value; return;
                    case 11: _11 = value; return;
                    case 12: _12 = value; return;
                    case 13: _13 = value; return;
                    case 14: _14 = value; return;
                    case 15: _15 = value; return;
                    default: rest[index - StackCount] = value; return;
                }
            }
        }

        /// <summary>
        /// Number of elements in the list
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Is list readonly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the StructList16`1.
        /// </summary>
        /// <param name="item">The object to add to the StructList16`1.</param>
        /// <exception cref="System.NotSupportedException">The StructList16`1 is read-only.</exception>
        public void Add(T item)
        {
            switch (count)
            {
                case 0: _0 = item; count++; return;
                case 1: _1 = item; count++; return;
                case 2: _2 = item; count++; return;
                case 3: _3 = item; count++; return;
                case 4: _4 = item; count++; return;
                case 5: _5 = item; count++; return;
                case 6: _6 = item; count++; return;
                case 7: _7 = item; count++; return;
                case 8: _8 = item; count++; return;
                case 9: _9 = item; count++; return;
                case 10: _10 = item; count++; return;
                case 11: _11 = item; count++; return;
                case 12: _12 = item; count++; return;
                case 13: _13 = item; count++; return;
                case 14: _14 = item; count++; return;
                case 15: _15 = item; count++; return;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    count++;
                    return;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the StructList16`1.
        /// </summary>
        /// <param name="item">The object to remove from the StructList16`1.</param>
        /// <returns>true if item was successfully removed from the StructList16`1; otherwise, false. This method also returns false if item is not found in the original StructList16`1.</returns>
        public bool Remove(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;

            if (count == 0) return false;
            if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
            if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
            if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
            if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
            if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
            if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
            if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
            if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
            if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
            if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }
            if (count >= 11 && comparer.Equals(_10, item)) { RemoveAt(10); return true; }
            if (count >= 12 && comparer.Equals(_11, item)) { RemoveAt(11); return true; }
            if (count >= 13 && comparer.Equals(_12, item)) { RemoveAt(12); return true; }
            if (count >= 14 && comparer.Equals(_13, item)) { RemoveAt(13); return true; }
            if (count >= 15 && comparer.Equals(_14, item)) { RemoveAt(14); return true; }
            if (count >= 16 && comparer.Equals(_15, item)) { RemoveAt(15); return true; }

            if (rest == null) return false;
            bool removed = rest.Remove(item);
            if (removed) count--;
            return removed;
        }

        /// <summary>
        /// Removes the StructList16`1 item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList16`1.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            if (index <= 0 && count > 1) _0 = _1;
            if (index <= 1 && count > 2) _1 = _2;
            if (index <= 2 && count > 3) _2 = _3;
            if (index <= 3 && count > 4) _3 = _4;
            if (index <= 4 && count > 5) _4 = _5;
            if (index <= 5 && count > 6) _5 = _6;
            if (index <= 6 && count > 7) _6 = _7;
            if (index <= 7 && count > 8) _7 = _8;
            if (index <= 8 && count > 9) _8 = _9;
            if (index <= 9 && count > 10) _9 = _10;
            if (index <= 10 && count > 11) _10 = _11;
            if (index <= 11 && count > 12) _11 = _12;
            if (index <= 12 && count > 13) _12 = _13;
            if (index <= 13 && count > 14) _13 = _14;
            if (index <= 14 && count > 15) _14 = _15;
            if (index <= 15 && count > 16) { _15 = rest[0]; rest.RemoveAt(0); }
            if (index >= StackCount) rest.RemoveAt(index - StackCount);
            count--;
        }

        /// <summary>
        /// Removes and returns the element at the end of the list.
        /// </summary>
        /// <returns>the last element</returns>
        /// <exception cref="InvalidOperationException">If list is empty</exception>
        public T Dequeue()
        {
            if (count == 0) throw new InvalidOperationException();
            int ix = count - 1;
            T result = this[ix];
            RemoveAt(ix);
            return result;
        }

        /// <summary>
        /// Removes all items from the StructList16`1.
        /// </summary>
        /// <exception cref="System.NotSupportedException">The StructList16`1 is read-only.</exception>
        public void Clear()
        {
            if (count >= 1) _0 = default;
            if (count >= 2) _1 = default;
            if (count >= 3) _2 = default;
            if (count >= 4) _3 = default;
            if (count >= 5) _4 = default;
            if (count >= 6) _5 = default;
            if (count >= 7) _6 = default;
            if (count >= 8) _7 = default;
            if (count >= 9) _8 = default;
            if (count >= 10) _9 = default;
            if (count >= 11) _10 = default;
            if (count >= 12) _11 = default;
            if (count >= 13) _12 = default;
            if (count >= 14) _13 = default;
            if (count >= 15) _14 = default;
            if (count >= 16) _15 = default;
            if (rest != null) rest.Clear();
            count = 0;
        }

        /// <summary>
        /// Determines whether the StructList16`1 contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the StructList16`1.</param>
        /// <returns>true if item is found in the StructList16`1; otherwise, false.</returns>
        public bool Contains(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return true;
            if (count >= 2 && comparer.Equals(_1, item)) return true;
            if (count >= 3 && comparer.Equals(_2, item)) return true;
            if (count >= 4 && comparer.Equals(_3, item)) return true;
            if (count >= 5 && comparer.Equals(_4, item)) return true;
            if (count >= 6 && comparer.Equals(_5, item)) return true;
            if (count >= 7 && comparer.Equals(_6, item)) return true;
            if (count >= 8 && comparer.Equals(_7, item)) return true;
            if (count >= 9 && comparer.Equals(_8, item)) return true;
            if (count >= 10 && comparer.Equals(_9, item)) return true;
            if (count >= 11 && comparer.Equals(_10, item)) return true;
            if (count >= 12 && comparer.Equals(_11, item)) return true;
            if (count >= 13 && comparer.Equals(_12, item)) return true;
            if (count >= 14 && comparer.Equals(_13, item)) return true;
            if (count >= 15 && comparer.Equals(_14, item)) return true;
            if (count >= 16 && comparer.Equals(_15, item)) return true;
            if (rest != null) return rest.Contains(item);
            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the StructList16`1.
        /// </summary>
        /// <param name="item">The object to locate in the StructList16`1.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            IEqualityComparer<T> comparer = elementComparer ?? EqualityComparer<T>.Default;
            if (count >= 1 && comparer.Equals(_0, item)) return 0;
            if (count >= 2 && comparer.Equals(_1, item)) return 1;
            if (count >= 3 && comparer.Equals(_2, item)) return 2;
            if (count >= 4 && comparer.Equals(_3, item)) return 3;
            if (count >= 5 && comparer.Equals(_4, item)) return 4;
            if (count >= 6 && comparer.Equals(_5, item)) return 5;
            if (count >= 7 && comparer.Equals(_6, item)) return 6;
            if (count >= 8 && comparer.Equals(_7, item)) return 7;
            if (count >= 9 && comparer.Equals(_8, item)) return 8;
            if (count >= 10 && comparer.Equals(_9, item)) return 9;
            if (count >= 11 && comparer.Equals(_10, item)) return 10;
            if (count >= 12 && comparer.Equals(_11, item)) return 11;
            if (count >= 13 && comparer.Equals(_12, item)) return 12;
            if (count >= 14 && comparer.Equals(_13, item)) return 13;
            if (count >= 15 && comparer.Equals(_14, item)) return 14;
            if (count >= 16 && comparer.Equals(_15, item)) return 15;
            if (rest != null) return rest.IndexOf(item) - StackCount;
            return -1;
        }

        /// <summary>
        /// Inserts an item to the StructList16`1 at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the StructList16`1.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList16`1.</exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
            if (index >= 16) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
            if (index <= 15 && count >= 16) { if (rest == null) rest = new List<T>(); rest.Insert(0, _15); }
            if (index <= 14 && count >= 15) _15 = _14;
            if (index <= 13 && count >= 14) _14 = _13;
            if (index <= 12 && count >= 13) _13 = _12;
            if (index <= 11 && count >= 12) _12 = _11;
            if (index <= 10 && count >= 11) _11 = _10;
            if (index <= 9 && count >= 10) _10 = _9;
            if (index <= 8 && count >= 9) _9 = _8;
            if (index <= 7 && count >= 8) _8 = _7;
            if (index <= 6 && count >= 7) _7 = _6;
            if (index <= 5 && count >= 6) _6 = _5;
            if (index <= 4 && count >= 5) _5 = _4;
            if (index <= 3 && count >= 4) _4 = _3;
            if (index <= 2 && count >= 3) _3 = _2;
            if (index <= 1 && count >= 2) _2 = _1;
            if (index <= 0 && count >= 1) _1 = _0;

            count++;
            this[index] = item;
        }

        /// <summary>
        /// Copies the elements of the StructList16`1 to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from StructList16`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">The number of elements in the source StructList16`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (count > array.Length + arrayIndex) throw new ArgumentException();

            if (count >= 1) array[arrayIndex++] = _0;
            if (count >= 2) array[arrayIndex++] = _1;
            if (count >= 3) array[arrayIndex++] = _2;
            if (count >= 4) array[arrayIndex++] = _3;
            if (count >= 5) array[arrayIndex++] = _4;
            if (count >= 6) array[arrayIndex++] = _5;
            if (count >= 7) array[arrayIndex++] = _6;
            if (count >= 8) array[arrayIndex++] = _7;
            if (count >= 9) array[arrayIndex++] = _8;
            if (count >= 10) array[arrayIndex++] = _9;
            if (count >= 11) array[arrayIndex++] = _10;
            if (count >= 12) array[arrayIndex++] = _11;
            if (count >= 13) array[arrayIndex++] = _12;
            if (count >= 14) array[arrayIndex++] = _13;
            if (count >= 15) array[arrayIndex++] = _14;
            if (count >= 16) array[arrayIndex++] = _15;
            if (rest != null) rest.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (count > 4) yield return _4;
            if (count > 5) yield return _5;
            if (count > 6) yield return _6;
            if (count > 7) yield return _7;
            if (count > 8) yield return _8;
            if (count > 9) yield return _9;
            if (count > 10) yield return _10;
            if (count > 11) yield return _11;
            if (count > 12) yield return _12;
            if (count > 13) yield return _13;
            if (count > 14) yield return _14;
            if (count > 15) yield return _15;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (count > 0) yield return _0;
            if (count > 1) yield return _1;
            if (count > 2) yield return _2;
            if (count > 3) yield return _3;
            if (count > 4) yield return _4;
            if (count > 5) yield return _5;
            if (count > 6) yield return _6;
            if (count > 7) yield return _7;
            if (count > 8) yield return _8;
            if (count > 9) yield return _9;
            if (count > 10) yield return _10;
            if (count > 11) yield return _11;
            if (count > 12) yield return _12;
            if (count > 13) yield return _13;
            if (count > 14) yield return _14;
            if (count > 15) yield return _15;
            if (rest != null)
            {
                IEnumerator<T> restEtor = rest.GetEnumerator();
                while (restEtor.MoveNext())
                    yield return restEtor.Current;
            }
        }

    }

    /// <summary>
    /// Inplace sorter specifically for struct based lists, but works on any <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="List"></typeparam>
    /// <typeparam name="Element"></typeparam>
    public struct StructListSorter<List, Element> where List : IList<Element>
    {
        IComparer<Element> comparer;

        /// <summary>
        /// Create sorter
        /// </summary>
        /// <param name="comparer"></param>
        public StructListSorter(IComparer<Element> comparer)
        {
            this.comparer = comparer ?? Comparer<Element>.Default;
        }

        /// <summary>
        /// Sort elements of list
        /// </summary>
        /// <param name="list"></param>
        public void Sort(ref List list)
            => QuickSort(ref list, 0, list.Count - 1);

        private void QuickSort(ref List list, int left, int right)
        {
            if (left < right)
            {
                int pivot = Partition(ref list, left, right);

                if (pivot > 1)
                {
                    QuickSort(ref list, left, pivot - 1);
                }
                if (pivot + 1 < right)
                {
                    QuickSort(ref list, pivot + 1, right);
                }
            }
        }

        private int Partition(ref List list, int left, int right)
        {
            if (left > right) return -1;
            int end = left;
            Element pivot = list[right];
            for (int i = left; i < right; i++)
            {
                int c = comparer.Compare(list[i], pivot);
                if (c < 0)
                {
                    // Swap list[i] and list[end]
                    Element tmp = list[i];
                    list[i] = list[end];
                    list[end] = tmp;
                    end++;
                }
            }
            // Swap list[end] and list[right]
            {
                Element tmp = list[end];
                list[end] = list[right];
                list[right] = tmp;
            }
            return end;
        }

    }

}
