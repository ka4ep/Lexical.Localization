// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// A visitor delegate that is used when part is visited.
    /// Visitation starts from tail and proceeds towards root.
    /// Visitation is stack allocated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="part"></param>
    /// <param name="data"></param>
    public delegate void LinePartVisitor<T>(ILinePart part, ref T data);

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Enumerate linked list towards root.
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static IEnumerable<ILinePart> EnumerateToRoot(this ILinePart part)
        {
            while (part != null)
            {
                yield return part;
                part = part.PreviousPart;
            }
        }

        /// <summary>
        /// Visit part chain from root towards part
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="part"></param>
        /// <param name="visitor"></param>
        /// <param name="data"></param>
        public static void VisitFromRoot<T>(this ILinePart part, LinePartVisitor<T> visitor, ref T data)
        {
            // Push to stack
            ILinePart prevKey = part.PreviousPart;
            if (prevKey != null) VisitFromRoot(prevKey, visitor, ref data);

            // Pop from stack in reverse order
            visitor(part, ref data);
        }

        /// <summary>
        /// Return an array of parts from root.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="includeNonCanonical">include all parts that implement ILocalizationKeyNonCanonicallyCompared</param>
        /// <returns>array of parts</returns>
        public static ILinePart[] ArrayFromRoot(this ILinePart part, bool includeNonCanonical=true)
        {
            // Count the number of parts
            int count = 0;
            if (includeNonCanonical)
                for (ILinePart p = part; p != null; p = p.PreviousPart) count++;
            else
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineKeyNonCanonicallyCompared == false) count++;

            // Create result
            ILinePart[] result = new ILinePart[count];
            int ix = count - 1;
            if (includeNonCanonical)
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    result[ix--] = p;
            else
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineKeyNonCanonicallyCompared == false)
                        result[ix--] = p;

            return result;
        }


        /// <summary>
        /// Get the first part in the linked list.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>part or first part</returns>
        public static ILinePart GetFirstKey(this ILinePart part)
        {
            while (true)
            {
                ILinePart prevKey = part.PreviousPart;
                if (prevKey == null) return part;
                part = prevKey;
            }
        }

        /// <summary>
        /// Finds part that implements T when walking towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="part"></param>
        /// <returns>T or null</returns>
        public static T Find<T>(this ILinePart part) where T : ILinePart
        {
            for (; part != null; part = part.PreviousPart)
                if (part is T casted) return casted;
            return default;
        }

        /// <summary>
        /// Finds part that implements T when walking towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="part"></param>
        /// <returns>T</returns>
        /// <exception cref="LineException">if T is not found</exception>
        public static T Get<T>(this ILinePart part) where T : ILinePart
        {
            for (; part != null; part = part.PreviousPart)
                if (part is T casted) return casted;
            throw new LineException(part, $"{typeof(T).FullName} is not found.");
        }

        /// <summary>
        /// Finds part that implements T when walking towards root, start from previous part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="part"></param>
        /// <returns>T or null</returns>
        public static T FindPrev<T>(this ILinePart part) where T : ILinePart
        {
            for (ILinePart k = part.PreviousPart; k != null; k = k.PreviousPart)
                if (k is T casted) return casted;
            return default;
        }

        /// <summary>
        /// Scan part towards root, returns <paramref name="index"/>th part from tail.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="index">the index of part to return starting from tail.</param>
        /// <returns>part</returns>
        /// <exception cref="IndexOutOfRangeException">if <paramref name="index"/> goes over root</exception>
        public static ILinePart GetAt(this ILinePart part, int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            for (int i = 0; i < index; i++)
                part = part.PreviousPart;
            if (part == null) throw new IndexOutOfRangeException();
            return part;
        }

    }
}
