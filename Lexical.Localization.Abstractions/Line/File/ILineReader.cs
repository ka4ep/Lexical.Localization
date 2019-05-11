//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    #region Interface
    /// <summary>
    /// Signals that file format can read localization files.
    /// 
    /// For reading capability, must implement one of:
    /// <list type="Bullet">
    /// <item><see cref="ILineStreamReader"/></item>
    /// <item><see cref="ILineTreeStreamReader"/></item>
    /// <item><see cref="ILineTextReader"/></item>
    /// <item><see cref="ILineTreeTextReader"/></item>
    /// <item><see cref="ILineStringTextReader"/></item>
    /// <item><see cref="ILineStringStreamReader"/></item>
    /// </list>
    /// </summary>
    public interface ILineReader : ILineFileFormat { }

    /// <summary>
    /// Reader that can read lines from a <see cref="Stream"/>.
    /// </summary>
    public interface ILineStreamReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy. </param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<ILine> ReadKeyLines(Stream stream, ILineFormat namePolicy = default);
    }

    /// <summary>
    /// Reader that can read tree format from a <see cref="Stream"/>.
    /// </summary>
    public interface ILineTreeStreamReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into tree structuer.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        ILineTree ReadLineTree(Stream stream, ILineFormat namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILineTextReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<ILine> ReadKeyLines(TextReader text, ILineFormat namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILineTreeTextReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        ILineTree ReadLineTree(TextReader text, ILineFormat namePolicy = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILineStringTextReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, IFormulationString>> ReadStringLines(TextReader text, ILineFormat namePolicy = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="Stream"/>.
    /// </summary>
    public interface ILineStringStreamReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, IFormulationString>> ReadStringLines(Stream stream, ILineFormat namePolicy = default);
    }
    #endregion Interface
}
