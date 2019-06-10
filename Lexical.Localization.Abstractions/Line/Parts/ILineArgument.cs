// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that appending arguments can be copied from this line part.
    /// </summary>
    public interface ILineArgument : ILine
    {
    }

    /// <summary>
    /// Enumerable of line arguments for multiple parts.
    /// </summary>
    public interface ILineArgumentEnumerable : ILine, IEnumerable<ILineArgument>
    {
    }

    /// <summary>
    /// Construction arguments of <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    public interface ILineArgument<Intf> : ILineArgument
    {
    }

    /// <summary>
    /// Construction arguments of <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    public interface ILineArgument<Intf, A0> : ILineArgument
    {
        /// <summary>
        /// <see cref="ILineFactory"/> argument.
        /// </summary>
        A0 Argument0 { get; }
    }

    /// <summary>
    /// Construction arguments of <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    public interface ILineArgument<Intf, A0, A1> : ILineArgument
    {
        /// <summary>
        /// <see cref="ILineFactory"/> argument.
        /// </summary>
        A0 Argument0 { get; }

        /// <summary>
        /// <see cref="ILineFactory"/> argument.
        /// </summary>
        A1 Argument1 { get; }
    }

    /// <summary>
    /// Construction arguments of <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    /// <typeparam name="A2"></typeparam>
    public interface ILineArgument<Intf, A0, A1, A2> : ILineArgument
    {
        /// <summary>
        /// <see cref="ILineFactory"/> argument.
        /// </summary>
        A0 Argument0 { get; }

        /// <summary>
        /// <see cref="ILineFactory"/> argument.
        /// </summary>
        A1 Argument1 { get; }

        /// <summary>
        /// <see cref="ILineFactory"/> argument.
        /// </summary>
        A2 Argument2 { get; }
    }

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Put all argument parts to <paramref name="list"/> with parameter occurance.
        /// 
        /// If <paramref name="argumentQualifier"/> is provided, then filters out non-qualified arguments.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <param name="list">list to add parts in order of from tail to root</param>
        /// <param name="argumentQualifier">(optional) parameter qualifier that validates each parameter</param>
        /// <param name="pruneIneffectiveParameters">(optional) If true, removes parameters that are not effetive: a null value, or reoccurance of non-canonical key</param>
        /// <param name="parameterInfos">(optional) extra parameter infos, used by <paramref name="pruneIneffectiveParameters"/> feature to detect which parameters are non-canonical keys.</param>
        /// <returns>array of parameters</returns>
        public static void GetArgumentPartsWithOccurance<LIST>(this ILine line, ref LIST list, ILineQualifier argumentQualifier = null, bool pruneIneffectiveParameters = false, IParameterInfos parameterInfos = null) where LIST : IList<(ILineArgument, int)>
        {
            ILineArgumentQualifier q = argumentQualifier as ILineArgumentQualifier;
            bool qualifyNow = q != null && !q.NeedsOccuranceIndex;

            // Read argument parts
            StructList8<ILineArgument> tmp = new StructList8<ILineArgument>();
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineArgumentEnumerable lineArguments)
                {
                    tmp.Clear();
                    foreach (ILineArgument argument in lineArguments)
                    {
                        if (qualifyNow && !q.QualifyArgument(argument)) continue;
                        tmp.Add(argument);
                    }
                    for (int i = tmp.Count - 1; i >= 0; i--)
                        list.Add((tmp[i], -1));
                }
                if (l is ILineArgument lineArgument)
                {
                    if (qualifyNow && !q.QualifyArgument(lineArgument)) continue;
                    list.Add((lineArgument, -1));
                }
            }

            // Calculate occurance
            for (int i=list.Count-1; i>=0; i--)
            {
                ILineArgument a = list[i].Item1;
                string parameterName, parameterValue;
                if (a.TryGetParameter(out parameterName, out parameterValue))
                {
                    // Calculate occurance index
                    int occIx = 0;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        ILineArgument b = list[j].Item1;
                        string parameterName2, parameterValue2;
                        if (b.TryGetParameter(out parameterName2, out parameterValue2)) continue;
                        if (parameterValue2 != null && parameterName == parameterName2) occIx++;
                    }

                    list[i] = (a, occIx);
                }
            }
        }

        /// <summary>
        /// Put all argument parts to <paramref name="list"/>.
        /// 
        /// If <paramref name="argumentQualifier"/> is provided, then filters out non-qualified arguments.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <param name="list">list to add parts in order of from tail to root</param>
        /// <param name="argumentQualifier">(optional) parameter qualifier that validates each parameter</param>
        /// <param name="pruneIneffectiveParameters">(optional) If true, removes parameters that are not effetive: a null value, or reoccurance of non-canonical key</param>
        /// <param name="parameterInfos">(optional) extra parameter infos, used by <paramref name="pruneIneffectiveParameters"/> feature to detect which parameters are non-canonical keys.</param>
        /// <returns>array of parameters</returns>
        public static void GetArgumentParts<LIST>(this ILine line, ref LIST list, ILineQualifier argumentQualifier = null, bool pruneIneffectiveParameters = false, IParameterInfos parameterInfos = null) where LIST : IList<ILineArgument>
        {
            ILineArgumentQualifier q = argumentQualifier as ILineArgumentQualifier;
            bool qualifyNow = q != null && !q.NeedsOccuranceIndex;

            // Read argument parts
            StructList8<ILineArgument> tmp = new StructList8<ILineArgument>();
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineArgumentEnumerable lineArguments)
                {
                    tmp.Clear();
                    foreach (ILineArgument argument in lineArguments)
                    {
                        if (qualifyNow && !q.QualifyArgument(argument)) continue;
                        tmp.Add(argument);
                    }
                    for (int i = tmp.Count - 1; i >= 0; i--) list.Add(tmp[i]);
                }
                if (l is ILineArgument lineArgument)
                {
                    if (qualifyNow && !q.QualifyArgument(lineArgument)) continue;
                    list.Add(lineArgument);
                }
            }

            // Filter arguments with occurance
            if (q != null && q.NeedsOccuranceIndex)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ILineArgument a = list[i];

                    // Get parameter
                    string parameterName, parameterValue;
                    if (!a.TryGetParameter(out parameterName, out parameterValue) || parameterValue == null)
                    {
                        // Qualify as an argument.
                        if (!q.QualifyArgument(a)) list.RemoveAt(i);
                        continue;
                    }

                    // Calculate occurance index
                    int occIx = 0;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        ILineArgument b = list[j];
                        string parameterName2, parameterValue2;
                        if (b.TryGetParameter(out parameterName2, out parameterValue2)) continue;
                        if (parameterValue2 != null && parameterName == parameterName2) occIx++;
                    }

                    // Qualify parameter with occurance index.
                    if (!q.QualifyArgument(a, occIx)) list.RemoveAt(i);
                }
            }

            if (pruneIneffectiveParameters)
            {
                IParameterInfos parameterInfos2 = null;
                ILineFactory f;
                if (line.TryGetAppender(out f)) f.TryGetParameterInfos(out parameterInfos2);

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ILineArgument a = list[i];

                    // Get parameter
                    string parameterName, parameterValue;
                    if (!a.TryGetParameter(out parameterName, out parameterValue)) continue;

                    // Value == null is ineffective part
                    if (parameterValue == null)
                    {
                        list.RemoveAt(i);
                        continue;
                    }

                    // Is non-canonical
                    if (a.IsNonCanonicalKey(parameterInfos, parameterInfos2))
                    {
                        // Calculate occurance index
                        int occIx = 0;
                        for (int j = i - 1; j >= 0; j--)
                        {
                            ILineArgument b = list[j];
                            string parameterName2, parameterValue2;
                            if (b.TryGetParameter(out parameterName2, out parameterValue2)) continue;
                            if (parameterValue2 != null && parameterName == parameterName2) occIx++;
                        }

                        // Only first occurance is effective
                        if (occIx > 0)
                        {
                            list.RemoveAt(i);
                            continue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Try to read parameter from <paramref name="argument"/>.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public static bool TryGetParameter(this ILineArgument argument, out string parameterName, out string parameterValue)
        {
            if (argument is ILineArgument<ILineParameter, string, string> lineParameter) { parameterName = lineParameter.Argument0; parameterValue = lineParameter.Argument1; return true; }
            if (argument is ILineArgument<ILineHint, string, string> lineHint) { parameterName = lineHint.Argument0; parameterValue = lineHint.Argument1; return true; }
            if (argument is ILineArgument<ILineCanonicalKey, string, string> lineCanonicalKey) { parameterName = lineCanonicalKey.Argument0; parameterValue = lineCanonicalKey.Argument1; return true; }
            if (argument is ILineArgument<ILineNonCanonicalKey, string, string> lineNonCanonicalKey) { parameterName = lineNonCanonicalKey.Argument0; parameterValue = lineNonCanonicalKey.Argument1; return true; }
            parameterName = default; parameterValue = default; return false;
        }

        /// <summary>
        /// Test if <paramref name="argument"/> is non-canonical key.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="parameterInfos1"></param>
        /// <param name="parameterInfos2"></param>
        /// <returns></returns>
        public static bool IsNonCanonicalKey(this ILineArgument argument, IParameterInfos parameterInfos1 = null, IParameterInfos parameterInfos2 = null)
        {
            if (argument is ILineArgument<ILineNonCanonicalKey, string, string>) return true;
            if (argument is ILineArgument<ILineHint, string, string>) return false;
            if (argument is ILineArgument<ILineCanonicalKey, string, string>) return false;
            if (argument is ILineArgument<ILineParameter, string, string> lineParameter) {
                IParameterInfo pi;
                if (parameterInfos1 != null && parameterInfos1.TryGetValue(lineParameter.Argument0, out pi)) return typeof(ILineNonCanonicalKey).Equals(pi.InterfaceType);
                if (parameterInfos2 != null && parameterInfos2.TryGetValue(lineParameter.Argument0, out pi)) return typeof(ILineNonCanonicalKey).Equals(pi.InterfaceType);
            }
            return false;
        }

    }

    /// <summary>
    /// Class that carries line arguments.
    /// </summary>
    public class LineArgument
    {
        /// <summary>
        /// Convert <paramref name="linePart"/> to <see cref="ILineArgument"/>.
        /// </summary>
        /// <param name="linePart"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="linePart"/> is null.</exception>
        /// <exception cref="LineException">If conversion fails.</exception>
        public static ILineArgument ToArgument(ILine linePart)
        {
            if (linePart == null) throw new ArgumentNullException(nameof(linePart));
            if (linePart is ILineArgument arg) return arg;
            if (linePart is ILineHint hint) return new LineArgument<ILineHint, string, string>(hint.ParameterName, hint.ParameterValue);
            if (linePart is ILineCanonicalKey canonicalKey) return new LineArgument<ILineCanonicalKey, string, string>(canonicalKey.ParameterName, canonicalKey.ParameterValue);
            if (linePart is ILineNonCanonicalKey nonCanonicalKey) return new LineArgument<ILineNonCanonicalKey, string, string>(nonCanonicalKey.ParameterName, nonCanonicalKey.ParameterValue);
            throw new LineException(linePart, $"Failed to convert {linePart.GetType().FullName}:{linePart.ToString()} to {nameof(ILineArgument)}.");
        }

        /// <summary>
        /// Create line arguments with only interface argument.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <returns></returns>
        public static ILineArgument<Intf> Create<Intf>() => new LineArgument<Intf>();

        /// <summary>
        /// Create one argument <see cref="ILineArgument"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <param name="a0"></param>
        /// <returns></returns>
        public static ILineArgument<Intf, A0> Create<Intf, A0>(A0 a0) => new LineArgument<Intf, A0>(a0);

        /// <summary>
        /// Create two argument <see cref="ILineArgument"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <returns></returns>
        public static ILineArgument<Intf, A0, A1> Create<Intf, A0, A1>(A0 a0, A1 a1) => new LineArgument<Intf, A0, A1>(a0, a1);

        /// <summary>
        /// Create three argument <see cref="ILineArgument"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static ILineArgument<Intf, A0, A1, A2> Create<Intf, A0, A1, A2>(A0 a0, A1 a1, A2 a2) => new LineArgument<Intf, A0, A1, A2>(a0, a1, a2);
    }

}
