// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Line parts form a linked list or trie of a <see cref="ILine"/>.
    /// </summary>
    public interface ILinePart : ILine
    {
        /// <summary>
        /// (Optional) Previous part.
        /// </summary>
        ILine PreviousPart { get; set; }
    }

    /// <summary>
    /// A visitor delegate that is used when part is visited.
    /// Visitation starts from tail and proceeds towards root.
    /// Visitation is stack allocated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="part"></param>
    /// <param name="data"></param>
    public delegate void LinePartVisitor<T>(ILine part, ref T data);

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Get previous part
        /// </summary>
        /// <param name="line"></param>
        /// <returns>part or null</returns>
        public static ILine GetPreviousPart(this ILine line)
            => line is ILinePart part ? part.PreviousPart : null;

        /// <summary>
        /// Enumerate from tail to root.
        /// </summary>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static IEnumerable<ILine> EnumerateToRoot(this ILine tail)
        {
            for (ILine l = tail; l != null; l = l.GetPreviousPart())
                yield return l;
        }

        /// <summary>
        /// Visit line parts from root towards tail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tail"></param>
        /// <param name="visitor"></param>
        /// <param name="data"></param>
        public static void VisitFromRoot<T>(this ILine tail, LinePartVisitor<T> visitor, ref T data)
        {
            // Push to stack
            ILine prevPart = tail.GetPreviousPart();
            if (prevPart != null) VisitFromRoot(prevPart, visitor, ref data);
            // Pop from stack in reverse order
            visitor(tail, ref data);
        }

        /// <summary>
        /// Array of <see cref="ILine"/> parts from root towards tail.
        /// </summary>
        /// <param name="tail"></param>
        /// <param name="whereFilter">(optional) where filter</param>
        /// <returns>array of parts</returns>
        public static ILine[] ToArray(this ILine tail, Func<ILine, bool> whereFilter = null)
        {
            // Count the number of parts
            int count = 0;
            if (whereFilter != null)
            {
                for (ILine p = tail; p != null; p = p.GetPreviousPart())
                    if (whereFilter(p)) count++;
            }
            else
            {
                for (ILine p = tail; p != null; p = p.GetPreviousPart()) count++;
            }

            // Create result
            ILine[] result = new ILine[count];
            int ix = count - 1;
            if (whereFilter != null)
                for (ILine p = tail; p != null; p = p.GetPreviousPart())
                    result[ix--] = p;
            else
            {
                for (ILine p = tail; p != null; p = p.GetPreviousPart())
                    if (whereFilter(p))
                        result[ix--] = p;
            }

            return result;
        }

        /// <summary>
        /// Finds part that implements T when walking from tail towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <returns>T or null</returns>
        public static T Find<T>(this ILine line) where T : ILine
        {
            for (; line != null; line = line.GetPreviousPart())
                if (line is T casted) return casted;
            return default;
        }

        /// <summary>
        /// Finds part that implements T when walking from tail towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <returns>T</returns>
        /// <exception cref="LineException">if T is not found</exception>
        public static T Get<T>(this ILine line) where T : ILine
        {
            for (; line != null; line = line.GetPreviousPart())
                if (line is T casted) return casted;
            throw new LineException(line, $"{typeof(T).FullName} is not found.");
        }

        /// <summary>
        /// Finds part that implements T when walking towards root, start from previous part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <returns>T or null</returns>
        public static T FindPrev<T>(this ILine line) where T : ILine
        {
            for (ILine k = line.GetPreviousPart(); k != null; k = k.GetPreviousPart())
                if (k is T casted) return casted;
            return default;
        }

        /// <summary>
        /// Scan part towards root, returns <paramref name="index"/>th part from tail (0=tail, count-1=root)
        /// </summary>
        /// <param name="tail"></param>
        /// <param name="index">the index of part to return starting from tail.</param>
        /// <returns>part</returns>
        /// <exception cref="IndexOutOfRangeException">if <paramref name="index"/> goes over root</exception>
        public static ILine GetAt(this ILine tail, int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            for (int i = 0; i < index; i++)
                tail = tail.GetPreviousPart();
            if (tail == null) throw new IndexOutOfRangeException();
            return tail;
        }

        /// <summary>
        /// Prune parameters that are disqualified by <paramref name="qualifier"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="qualifier">Argument qualifier that is used for determining which parts to keep in the line</param>
        /// <param name="lineFactory">(optional) extra line factory</param>
        /// <returns>a modified <paramref name="line"/></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Prune(this ILine line, ILineQualifier qualifier, ILineFactory lineFactory = null)
        {
            // Qualified parts to append (to append in order of from appendIx to 0)
            StructList12<ILineArgument> list = new StructList12<ILineArgument>();

            // Earliest qualified line part. The start tail, where to start appending qualified parts
            ILine startTail = null;
            // Index up until to append
            int appendIx = -1;

            StructList8<ILineArgument> tmp = new StructList8<ILineArgument>();
            // Add parts
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                tmp.Clear();
                if (l is ILineArgumentEnumerable lineArguments)
                {
                    foreach (ILineArgument lineArgument in lineArguments) tmp.Add(lineArgument);
                }
                if (l is ILineArgument argument) tmp.Add(argument);

                // Now qualify
                bool linePartQualifies = true;
                string parameterName, parameterValue;
                for (int i = tmp.Count - 1; i >= 0; i--)
                {
                    ILineArgument a = tmp[i];
                    list.Add(a);
                    // Qualify as an argument.
                    if (!a.IsNonCanonicalKey()) linePartQualifies &= qualifier.QualifyArgument(a);
                    // Qualify as non-canonical parameter
                    else if (a.TryGetParameter(out parameterName, out parameterValue))
                    {
                        // Calculate occurance index
                        int occIx = -1;
                        if (qualifier.NeedsOccuranceIndex())
                        {
                            occIx = 0;
                            for (int j = i - 1; j >= 0; j--)
                            {
                                ILineArgument b = list[j];
                                string parameterName2, parameterValue2;
                                if (b.TryGetParameter(out parameterName2, out parameterValue2)) continue;
                                if (parameterValue2 != null && parameterName == parameterName2) occIx++;
                            }
                        }
                        linePartQualifies &= qualifier.QualifyArgument(a, occIx);
                    }
                    if (!linePartQualifies) break;
                }

                if (!linePartQualifies)
                {
                    startTail = l.GetPreviousPart();
                    appendIx = list.Count - 1;
                }
            }


            // Append qualified parts.
            ILineFactory appender1 = null;
            line.TryGetAppender(out appender1);

            // Nothing qualified, no start, create dummy
            if (startTail == null && list.Count == 0)
            {
                // Create dummy
                ILineFactory appender2 = null;
                line.TryGetAppender(out appender2);
                ILinePart dummy = null;
                if (lineFactory == null || !lineFactory.TryCreate(null, out dummy))
                    if (appender2 == null || !appender2.TryCreate(null, out dummy))
                        throw new LineException(line, $"LineFactory doesn't have capability to create {nameof(ILinePart)}");
                return dummy;
            }

            // Append parts
            ILine result = startTail;
            for (int i = appendIx; i >= 0; i--)
            {
                ILineArgument arg = list[i];
                if (lineFactory == null || !lineFactory.TryCreate(result, arg, out result))
                    if (appender1 == null || !appender1.TryCreate(result, arg, out result))
                        throw new LineException(line, $"LineFactory doesn't have capability to concat {arg}");
            }
            return result;
        }


    }
}
