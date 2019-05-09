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
    /// Signals that file format can be read localization files.
    /// 
    /// For reading capability, must implement one of:
    /// <list type="Bullet">
    /// <item><see cref="ILocalizationKeyLinesStreamReader"/></item>
    /// <item><see cref="ILocalizationKeyTreeStreamReader"/></item>
    /// <item><see cref="ILocalizationKeyLinesTextReader"/></item>
    /// <item><see cref="ILocalizationKeyTreeTextReader"/></item>
    /// <item><see cref="ILocalizationStringLinesTextReader"/></item>
    /// <item><see cref="ILocalizationStringLinesStreamReader"/></item>
    /// </list>
    /// </summary>
    public interface ILocalizationReader : ILocalizationFileFormat { }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationKeyLinesStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy. </param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<ILine, IFormulationString>> ReadKeyLines(Stream stream, IParameterPolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization into tree format format a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationKeyTreeStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into tree structuer.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        ILineTree ReadKeyTree(Stream stream, IParameterPolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationKeyLinesTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<ILine, IFormulationString>> ReadKeyLines(TextReader text, IParameterPolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationKeyTreeTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        ILineTree ReadKeyTree(TextReader text, IParameterPolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationStringLinesTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, IFormulationString>> ReadStringLines(TextReader text, IParameterPolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationStringLinesStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, IFormulationString>> ReadStringLines(Stream stream, IParameterPolicy namePolicy = default);
    }
    #endregion Interface
}
