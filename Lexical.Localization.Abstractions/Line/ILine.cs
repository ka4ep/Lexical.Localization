// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    /// <summary>
    /// Line that can contain resolve hints, be comparable as key, and have a string value.
    /// 
    /// The implementing class should implement ToString() to return the value of <see cref="ILineExtensions.ResolveString(ILine)"/>.Value.
    /// </summary>
    public interface ILine
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Make clone. <paramref name="line"/> must have an appender.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Clone(this ILine line)
            => line.GetAppender().Clone(line);

        /// <summary>
        /// Try to create clone
        /// </summary>
        /// <param name="line"></param>
        /// <param name="clone"></param>
        /// <returns></returns>
        public static bool TryClone(this ILine line, out ILine clone)
        {
            ILineFactory appender;
            if (line.TryGetAppender(out appender) && appender.TryClone(line, out clone)) return true;
            clone = default;
            return false;
        }

        /// <summary>
        /// Prune out arguments that are disqualified by <paramref name="qualifier"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="qualifier">Argument qualifier that is used for determining which parts to keep in the line</param>
        /// <param name="lineFactory">(optional) extra line factory</param>
        /// <returns>a modified <paramref name="line"/></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Prune(this ILine line, ILineQualifier qualifier, ILineFactory lineFactory = null)
        {
            // Qualified parts to append. Order: tail to root. 
            StructList12<ILineArgument> list = new StructList12<ILineArgument>();

            ILineArgumentQualifier lineArgumentQualifier = qualifier as ILineArgumentQualifier;

            // Earliest qualified line part. The start tail, where to start appending qualified parts
            ILine startTail = line;

            // Line part's arguments. Order: root to tail
            StructList8<ILineArgument> tmp = new StructList8<ILineArgument>();
            // Start tail buffered args. Order: root to tail
            StructList8<ILineArgument> startTailArgsBuffer = new StructList8<ILineArgument>();
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

                    bool argumentQualifies = true;
                    if (lineArgumentQualifier != null)
                    {
                        // Qualify as an argument.
                        if (!a.IsNonCanonicalKey()) argumentQualifies = lineArgumentQualifier.QualifyArgument(a);
                        // Qualify as non-canonical parameter
                        else if (a.TryGetParameter(out parameterName, out parameterValue))
                        {
                            // Calculate occurance index
                            int occIx = -1;
                            if (lineArgumentQualifier.NeedsOccuranceIndex)
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
                            argumentQualifies = lineArgumentQualifier.QualifyArgument(a, occIx);
                        }
                    }
                    if (!argumentQualifies) tmp.RemoveAt(i);
                    linePartQualifies &= argumentQualifies;
                }

                // This part didn't qualify
                if (!linePartQualifies)
                {
                    // Append previous start tail to append args
                    if (startTailArgsBuffer.Count > 0)
                    {
                        for (int i = 0; i < startTailArgsBuffer.Count; i++) list.Add(startTailArgsBuffer[i]);
                        startTailArgsBuffer.Clear();
                        startTail = null;
                    }
                    // Add parts that did qualify to append list
                    for (int i = 0; i < tmp.Count; i++) list.Add(tmp[i]);
                    // preceding part might be better for start tail
                    startTail = l.GetPreviousPart();
                }
                else
                // This part qualified
                {
                    // Add to start tail buffer, in case preceding startTail fails qualifications
                    for (int i = 0; i < tmp.Count; i++) startTailArgsBuffer.Add(tmp[i]);
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
            for (int i = list.Count - 1; i >= 0; i--)
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
