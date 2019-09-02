//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    // <Interface>
    /// <summary>
    /// Flags for write operation to localization files.
    /// </summary>
    [Flags]
    public enum LineFileWriteFlags : UInt32
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0,

        /// <summary>
        /// Permission to add new entries to the container.
        /// </summary>
        Add = 1,

        /// <summary>
        /// Permission to remove entires that no longer exist. Remove is recursive.
        /// If entry contains unrecognized attributes, comments, elements, then 
        /// entry will be removed regardless.
        /// </summary>
        Remove = 2,

        /// <summary>
        /// Permission to remove entires that no longer exist, but be cautious.
        /// If entry contains unrecognized comments, elements, then remove will not proceed.
        /// </summary>
        RemoveCautious = 4,

        /// <summary>
        /// Permission to modify values of existing entries.
        /// </summary>
        Modify = 8,

        /// <summary>
        /// Overwrite contents of previous file.
        ///   <see cref="Add"/> must be set with this flag, or else an empty document is created.
        /// 
        /// Flags <see cref="Remove"/> and <see cref="Modify"/> have no effect with this flag.
        /// </summary>
        Overwrite = 16,

        /// <summary>
        /// Matches when old and new files have different tree structure. 
        /// 
        /// Matches effective key of each entry to the effective key of new entry.
        /// 
        /// For example if file had previously following node 
        /// <![CDATA[ <Line Type="ConsoleApp1.MyController" Key="Success" Culture="de">Erfolg</Line> ]]>
        /// 
        /// And the code wants to write the following structure
        /// <![CDATA[ <Type:ConsoleApp1.MyController><Key:Success Culture="de">Erfolg !!</Key:Success></Type:ConsoleApp1.MyController> ]]>
        /// 
        /// Then these nodes are considered corresponding as their keys are effectively same (though structurally differnet), and the previous node gets updated.
        /// 
        /// If this <see cref="EffectiveKeyMatching"/> is false, then previous node is removed and new node is created, provided those flags are enabled.
        /// </summary>
        EffectiveKeyMatching = 32,

    }

    /// <summary>
    /// Signals that file format class can write localization files.
    /// 
    /// Must implement atleast one of the sub classes.
    /// <list type="Bullet">
    /// <item><see cref="ILineTextWriter"/></item>
    /// <item><see cref="ILineStreamWriter"/></item>
    /// <item><see cref="IUnformedLineTextWriter"/></item>
    /// <item><see cref="IUnformedLineStreamWriter"/></item>
    /// <item><see cref="ILineTreeTextWriter"/></item>
    /// <item><see cref="ILineTreeStreamWriter"/></item>
    /// </list>
    /// </summary>
    public interface ILineWriter : ILineFileFormat { }

    /// <summary>
    /// Writer that can write localization key-values with text writers.
    /// </summary>
    public interface ILineTextWriter : ILineWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcText">(optional) source text, used if previous content are updated.</param>
        /// <param name="dstText"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteLines(IEnumerable<ILine> lines, TextReader srcText, TextWriter dstText, ILineFormat lineFormat, LineFileWriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization key-values to streams.
    /// </summary>
    public interface ILineStreamWriter : ILineWriter
    {
        /// <summary>
        /// Write <paramref name="lines"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// 
        /// The implementation must not close either <paramref name="srcStream"/> or <paramref name="dstStream"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream">stream to write to.</param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteLines(IEnumerable<ILine> lines, Stream srcStream, Stream dstStream, ILineFormat lineFormat, LineFileWriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization tree structure to streams.
    /// </summary>
    public interface ILineTreeStreamWriter : ILineWriter
    {
        /// <summary>
        /// Write <paramref name="tree"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// 
        /// The implementation must not close either <paramref name="srcStream"/> or <paramref name="dstStream"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteLineTree(ILineTree tree, Stream srcStream, Stream dstStream, ILineFormat lineFormat, LineFileWriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization tree structure to text writer.
    /// </summary>
    public interface ILineTreeTextWriter : ILineWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="srcText">(optional) source text, used if previous content is updated.</param>
        /// <param name="dstText"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteLineTree(ILineTree tree, TextReader srcText, TextWriter dstText, ILineFormat lineFormat, LineFileWriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization key-values with text writers.
    /// 
    /// This interfaces writes lines from sources which have keys that are plain strings and are unstructured so that <see cref="ILine"/> parts cannot be extracted.
    /// </summary>
    public interface IUnformedLineTextWriter : ILineWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// 
        /// The implementation must not close either <paramref name="srcText"/> or <paramref name="dstText"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcText">(optional) source text, used if previous content are updated.</param>
        /// <param name="dstText"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteStringLines(IEnumerable<KeyValuePair<string, IString>> lines, TextReader srcText, TextWriter dstText, ILineFormat lineFormat, LineFileWriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization key-values to streams.
    /// 
    /// This interfaces writes lines from sources which have keys that are plain strings and are unstructured so that <see cref="ILine"/> parts cannot be extracted.
    /// </summary>
    public interface IUnformedLineStreamWriter : ILineWriter
    {
        /// <summary>
        /// Write <paramref name="lines"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// 
        /// The implementation must not close either <paramref name="srcStream"/> or <paramref name="dstStream"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream">stream to write to.</param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteStringLines(IEnumerable<KeyValuePair<string, IString>> lines, Stream srcStream, Stream dstStream, ILineFormat lineFormat, LineFileWriteFlags flags);
    }
    // </Interface>
}
