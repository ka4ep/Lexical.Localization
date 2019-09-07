//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    // <doc>
    /// <summary>
    /// Signals that file format can read localization files.
    /// 
    /// For reading capability, must implement one of:
    /// <list type="Bullet">
    /// <item><see cref="ILineStreamReader"/></item>
    /// <item><see cref="ILineTreeStreamReader"/></item>
    /// <item><see cref="ILineTextReader"/></item>
    /// <item><see cref="ILineTreeTextReader"/></item>
    /// <item><see cref="IUnformedLineTextReader"/></item>
    /// <item><see cref="IUnformedLineStreamReader"/></item>
    /// </list>
    /// </summary>
    public interface ILineReader : ILineFileFormat { }

    /// <summary>
    /// Reader that can read lines from a <see cref="Stream"/>.
    /// 
    /// This interfaces reads lines from sources that have keys which are formulated so that <see cref="ILine"/> parts can be extracted.
    /// The source is context-free of information that the caller must know. It doesn't need <see cref="ILineFormat"/>.
    /// </summary>
    public interface ILineStreamReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<ILine> ReadLines(Stream stream, ILineFormat lineFormat = default);
    }

    /// <summary>
    /// Reader that can read tree format from a <see cref="Stream"/>.
    /// 
    /// This interfaces reads lines from sources that have keys which are formulated so that <see cref="ILine"/> parts can be extracted.
    /// The source is context-free of information that the caller must know. It doesn't need <see cref="ILineFormat"/>.
    /// </summary>
    public interface ILineTreeStreamReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into tree structure.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        ILineTree ReadLineTree(Stream stream, ILineFormat lineFormat = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// 
    /// This interfaces reads lines from sources that have keys which are formulated so that <see cref="ILine"/> parts can be extracted.
    /// The source is context-free of information that the caller must know. It doesn't need <see cref="ILineFormat"/>.
    /// </summary>
    public interface ILineTextReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into tree structure.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<ILine> ReadLines(TextReader text, ILineFormat lineFormat = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// 
    /// This interfaces reads lines from sources that have keys which are formulated so that <see cref="ILine"/> parts can be extracted.
    /// The source is context-free of information that the caller must know. It doesn't need <see cref="ILineFormat"/>.
    /// </summary>
    public interface ILineTreeTextReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        ILineTree ReadLineTree(TextReader text, ILineFormat lineFormat = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="TextReader"/>.
    /// 
    /// This interfaces reads lines from sources which keys which are plain strings and are unstructured so that <see cref="ILine"/> parts cannot be extracted.
    /// The source is context-dependent of information that the caller must know. <see cref="ILineFormat"/> is needed to formulate <see cref="ILine"/>.
    /// </summary>
    public interface IUnformedLineTextReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, IString>> ReadUnformedLines(TextReader text, ILineFormat lineFormat = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="Stream"/>.
    /// 
    /// This interfaces reads lines from sources that have keys which are plain strings and unstructured so that <see cref="ILine"/> parts cannot be extracted.
    /// The source is context-dependent of information that the caller must know. <see cref="ILineFormat"/> is needed to formulate <see cref="ILine"/>.
    /// </summary>
    public interface IUnformedLineStreamReader : ILineReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, IString>> ReadUnformedLines(Stream stream, ILineFormat lineFormat = default);
    }
    // </doc>
}
