//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lexical.Localization.LocalizationFile2
{
    /// <summary>
    /// Represents a localization file format.
    /// </summary>
    public interface ILocalizationFileFormat
    {
        /// <summary>
        /// Extension of the file format without separator. e.g. "xml".
        /// </summary>
        string Extension { get; }
    }

    /// <summary>
    /// Signals that file format can be read localization files.
    /// </summary>
    public interface ILocalizationReader : ILocalizationFileFormat { }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationLinesStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy. </param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<IAssetKey, string>> ReadLines(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization into tree format format a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationTreeStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into tree structuer.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        IKeyTree ReadTree(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can open a read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationLinesTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<IAssetKey, string>> ReadLines(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can open a read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationTreeTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        IKeyTree ReadTree(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    [Flags]
    public enum WriteFlags {
        /// <summary>
        /// Adds entries that didn't exist
        /// </summary>
        UpdateAdd = 1,
        /// <summary>
        /// Modifies entries that existed previously
        /// </summary>
        UpdateModify = 2,
        /// <summary>
        /// Removes entires that no longer exist.
        /// </summary>
        UpdateRemove = 4,
        
        /// <summary>
        /// Replace previous content.
        /// </summary>
        Overwrite = UpdateAdd | UpdateModify | UpdateRemove,
    }

    /// <summary>
    /// Signals that file format can write localization files.
    /// </summary>
    public interface ILocalizationWriter : ILocalizationFileFormat { }

    /// <summary>
    /// Writer that can write localization key-values with text writers.
    /// </summary>
    public interface ILocalizationLinesTextWriter : ILocalizationWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcText">(optional) source text, used if previous content are updated.</param>
        /// <param name="dstText"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteLines(IEnumerable<KeyValuePair<IAssetKey, string>> lines, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization key-values to streams.
    /// </summary>
    public interface ILocalizationLinesStreamWriter : ILocalizationWriter
    {
        /// <summary>
        /// Write <paramref name="lines"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream">stream to write to.</param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteLines(IEnumerable<KeyValuePair<IAssetKey, string>> lines, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }
    /// <summary>
    /// Writer that can write localization tree structure to streams.
    /// </summary>
    public interface ILocalizationTreeStreamWriter : ILocalizationWriter
    {
        /// <summary>
        /// Write <paramref name="tree"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteTree(IKeyTree tree, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization tree structure to text writer.
    /// </summary>
    public interface ILocalizationTreeTextWriter : ILocalizationWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="srcText">(optional) source text, used if previous content is updated.</param>
        /// <param name="dstText"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteTree(IKeyTree tree, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    public static class LocalizationFileFormatExtensions
    {

    }

}
