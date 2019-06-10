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
            if (linePart is ILineArgument args) return args;
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
