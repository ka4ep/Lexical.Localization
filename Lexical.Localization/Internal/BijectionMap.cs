// -----------------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           1.6.2017
// -----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Bijection Dictionary is a Dictionary that has no values or keys, only 1:1 of values. 
    /// These value/keys will be called with left and right side.
    /// 
    /// Each value can exist only once on a each side
    /// </summary>
    /// <typeparam name="L">Left-side key</typeparam>
    /// <typeparam name="R">Right-side key</typeparam>
    public class BijectionMap<L, R> : ICloneable, IEnumerable<KeyValuePair<L, R>>
    {
        /// <summary>
        /// The keys of tableLeft are left-side-values and values are right-side-values.
        /// </summary>
        protected Dictionary<L, R> tableLeft;

        /// <summary>
        /// The keys of tableRight are right-side-values and values on it are left-side-values.
        /// </summary>
        protected Dictionary<R, L> tableRight;

        /// <summary>
        /// Left key comparer.
        /// </summary>
        public IEqualityComparer<L> LeftComparer { get; protected set; }

        /// <summary>
        /// Right key comparer.
        /// </summary>
        public IEqualityComparer<R> RightComparer { get; protected set; }

        /// <summary>
        /// Index an element with left-side key.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public R this[L left] {
            get => this.GetRight(left);
            set => this.Put(left, value);
        }

        /// <summary>
        /// Create bijection map.
        /// </summary>
        public BijectionMap()
        {
            tableLeft = new Dictionary<L, R>();
            tableRight = new Dictionary<R, L>();
            LeftComparer = EqualityComparer<L>.Default;
            RightComparer = EqualityComparer<R>.Default;
        }

        /// <summary>
        /// Create bijection map with custom comparers.
        /// </summary>
        /// <param name="leftComparer">(optional) comparer</param>
        /// <param name="rightComparer">(optional) comparer</param>
        public BijectionMap(IEqualityComparer<L> leftComparer, IEqualityComparer<R> rightComparer)
        {
            LeftComparer = leftComparer ?? EqualityComparer<L>.Default;
            RightComparer = rightComparer ?? EqualityComparer<R>.Default;
            tableLeft = new Dictionary<L, R>(LeftComparer);
            tableRight = new Dictionary<R, L>(RightComparer);
        }

        /// <summary>
        /// Create bijection map with default comparers, copy initial values from <paramref name="copyFrom"/>.
        /// </summary>
        /// <param name="copyFrom"></param>
        public BijectionMap(BijectionMap<L, R> copyFrom)
        {
            LeftComparer = copyFrom.LeftComparer;
            RightComparer = copyFrom.RightComparer;
            tableLeft = new Dictionary<L, R>(LeftComparer);
            tableRight = new Dictionary<R, L>(RightComparer);
            AddAll(copyFrom);
        }

        /// <summary>
        /// Add contents from another bijection map.
        /// </summary>
        /// <param name="dictionary"></param>
        public void AddAll(BijectionMap<L, R> dictionary)
        {
            foreach (var entry in dictionary.tableLeft)
            {
                tableLeft[entry.Key] = entry.Value;
                tableRight[entry.Value] = entry.Key;
            }
        }

        /// <summary>
        /// Retain all entries whose left side is within <paramref name="values"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RetainAllLeft(ICollection<L> values)
        {
            // Probably not the best of implementations
            bool result = false;
            ICollection<L> remove = new List<L>(Count);
            foreach (L lValue in tableLeft.Keys)
            {

                if (!values.Contains(lValue))
                {
                    remove.Add(lValue);
                    result = true;
                }
            }
            if (remove.Count > 0)
            {
                foreach (L lValue in remove)
                    RemoveWithLeft(lValue);
            }
            return result;
        }

        /// <summary>
        /// Retain all entries whose right side is within <paramref name="values"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RetainAllRight(ICollection<R> values)
        {
            // Probably not the best of implementations
            bool result = false;
            ICollection<R> remove = new List<R>(Count);
            foreach (R rValue in tableRight.Keys)
            {
                if (!values.Contains(rValue))
                {
                    remove.Add(rValue);
                    result = true;
                }
            }
            if (remove.Count > 0)
            {
                foreach (R rValue in remove)
                    RemoveWithRight(rValue);
            }
            return result;
        }

        /// <summary>
        /// Test if left values contain <paramref name="leftValue"/>.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <returns></returns>
        public bool ContainsLeft(L leftValue)
            => tableLeft.ContainsKey(leftValue);

        /// <summary>
        /// Test if right values contain <paramref name="rightValue"/>.
        /// </summary>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public bool ContainsRight(R rightValue)
            => tableRight.ContainsKey(rightValue);

        /// <summary>
        /// Test if contains pair.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public bool Contains(L leftValue, R rightValue)
        {
            if (leftValue == null || rightValue == null) return false;
            R r;
            if (!tableLeft.TryGetValue(leftValue, out r)) return false;
            return RightComparer.Equals(r, rightValue);
        }

        /// <summary>
        /// Put left, right value pair.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        public void Put(L leftValue, R rightValue)
        {
            if (leftValue == null) throw new ArgumentNullException(nameof(leftValue));
            if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));
            // Remove possible old entry
            L oldL;
            R oldR;
            if (tableLeft.TryGetValue(leftValue, out oldR)) tableRight.Remove(oldR);
            if (tableRight.TryGetValue(rightValue, out oldL)) tableLeft.Remove(oldL);

            // Set new values
            tableLeft[leftValue] = rightValue;
            tableRight[rightValue] = leftValue;
        }

        /// <summary>
        /// Remove left, right pair.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public bool Remove(L leftValue, R rightValue)
        {
            if (leftValue == null || rightValue == null) return false;
            L oldL;
            if (!tableRight.TryGetValue(rightValue, out oldL)) return false;
            if (!LeftComparer.Equals(oldL, leftValue)) return false;
            tableRight.Remove(rightValue);
            tableLeft.Remove(oldL);
            return true;
        }

        /// <summary>
        /// Test if is empty.
        /// </summary>
        public bool IsEmpty 
            => tableLeft.Count == 0;

        /// <summary>
        /// Get entry count.
        /// </summary>
        public int Count => tableLeft.Count;

        /// <summary>
        /// Get value on the left with right key.
        /// </summary>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public L GetLeft(R rightValue)
            => tableRight[rightValue];

        /// <summary>
        /// Try get value on the left with right key.
        /// </summary>
        /// <param name="rightValue"></param>
        /// <param name="leftValue"></param>
        /// <returns></returns>
        public bool TryGetLeft(R rightValue, out L leftValue)
            => tableRight.TryGetValue(rightValue, out leftValue);

        /// <summary>
        /// Get value on the right with left key.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <returns></returns>
        public R GetRight(L leftValue)
            => tableLeft[leftValue];

        /// <summary>
        /// Try get value on the right with left key.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public bool TryGetRight(L leftValue, out R rightValue)
            => tableLeft.TryGetValue(leftValue, out rightValue);

        /// <summary>
        /// Remove with left key.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <returns></returns>
        public R RemoveWithLeft(L leftValue)
        {
            R oldR = default(R);
            if (tableLeft.TryGetValue(leftValue, out oldR))
            {
                tableRight.Remove(oldR);
                tableLeft.Remove(leftValue);
            }
            return oldR;
        }

        /// <summary>
        /// Remove with right key.
        /// </summary>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public L RemoveWithRight(R rightValue)
        {
            L oldL = default(L);
            if (tableRight.TryGetValue(rightValue, out oldL))
            {
                tableLeft.Remove(oldL);
                tableRight.Remove(rightValue);
            }
            return oldL;
        }

        /// <summary>
        /// Get set of left values
        /// </summary>
        /// <returns>Left values</returns>
        public ICollection<L> GetLeftSet()
            => tableLeft.Keys;

        /// <summary>
        /// Get set of right values
        /// </summary>
        /// <returns>Right values</returns>
        public ICollection<R> GetRightSet()
            => tableRight.Keys;

        /// <summary>
        /// Get left-to-right Dictionary
        /// </summary>
        /// <returns>left-to-right Dictionary</returns>
        public Dictionary<L, R> GetLeftToRightDictionary()
            => tableLeft;

        /// <summary>
        /// Get right-to-left Dictionary
        /// </summary>
        /// <returns>right-to-left Dictionary</returns>
        public Dictionary<R, L> GetRightToLeftDictionary()
            => tableRight;

        /// <summary>
        /// Clear entriees.
        /// </summary>
        public virtual void Clear()
        {
            tableLeft.Clear();
            tableRight.Clear();
        }

        /// <summary>
        /// Print entries.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var e in tableLeft)
            {
                if (count++ > 0) sb.Append(", ");
                sb.Append(e.Key);
                sb.Append("=");
                sb.Append(e.Value);
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Create clone.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            BijectionMap<L, R> clone = new BijectionMap<L, R>(tableLeft.Comparer, tableRight.Comparer);
            clone.AddAll(this);
            return clone;
        }

        /// <summary>
        /// Enumerate pairs.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<L, R>> GetEnumerator()
            => tableLeft.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => tableLeft.GetEnumerator();

    }
}
