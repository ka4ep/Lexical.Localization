// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that appending arguments can be copied from this line part.
    /// </summary>
    public interface ILineArguments : ILine
    {
    }

    /// <summary>
    /// Enumerable of line arguments for multiple parts.
    /// </summary>
    public interface ILineArgumentsEnumerable : ILine, IEnumerable<ILineArguments>
    {
    }

    /// <summary>
    /// Construction arguments of <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    public interface ILineArguments<Intf> : ILineArguments
    {
    }

    /// <summary>
    /// Construction arguments of <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    public interface ILineArguments<Intf, A0> : ILineArguments
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
    public interface ILineArguments<Intf, A0, A1> : ILineArguments
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
    public interface ILineArguments<Intf, A0, A1, A2> : ILineArguments
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
    public class LineArguments
    {
        /// <summary>
        /// Create line arguments with only interface argument.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <returns></returns>
        public static ILineArguments<Intf> Create<Intf>() => new LineArguments<Intf>();

        /// <summary>
        /// Create one argument <see cref="ILineArguments"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <param name="a0"></param>
        /// <returns></returns>
        public static ILineArguments<Intf, A0> Create<Intf, A0>(A0 a0) => new LineArguments<Intf, A0>(a0);

        /// <summary>
        /// Create two argument <see cref="ILineArguments"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <returns></returns>
        public static ILineArguments<Intf, A0, A1> Create<Intf, A0, A1>(A0 a0, A1 a1) => new LineArguments<Intf, A0, A1>(a0, a1);

        /// <summary>
        /// Create three argument <see cref="ILineArguments"/>.
        /// </summary>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static ILineArguments<Intf, A0, A1, A2> Create<Intf, A0, A1, A2>(A0 a0, A1 a1, A2 a2) => new LineArguments<Intf, A0, A1, A2>(a0, a1, a2);
    }

}
