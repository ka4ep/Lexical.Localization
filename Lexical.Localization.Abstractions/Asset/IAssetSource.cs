// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Asset
{
    // <doc>
    /// <summary>
    /// Source of assets. Adds resources to builder's list.
    /// 
    /// The implementation of this class must use one of the more specific sub-interfaces:
    /// <list type="table">
    ///     <item><see cref="ILineAssetSource"/></item>
    ///     <item><see cref="IUnformedLineAssetSource"/></item>
    ///     <item><see cref="ILineTreeAssetSource"/></item>
    ///     <item><see cref="IFileAssetSource"/></item>
    ///     <item><see cref="IFilePatternAssetSource"/></item>
    ///     <item><see cref="IStringAssetSource"/></item>
    ///     <item><see cref="IBinaryAssetSource"/></item>
    ///     <item><see cref="IAssetSourceFileSystem"/></item>
    ///     <item><see cref="IAssetSourceObservePolicy"/></item>
    ///     <item><see cref="IObservableAssetSource"/></item>
    ///     <item><see cref="IAssetLoaderSource"/></item>
    /// </list>
    /// 
    /// IAssetSource should implement hash-equals comparisons to distinguish similar sources.
    /// If sources are references, for instance file path, then hash-equals comparer should
    /// regard them as equal.
    /// </summary>
    public interface IAssetSource
    {
    }

    /// <summary>
    /// Provides a specific <see cref="IFileSystem"/> to be used with the <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSourceFileSystem : IAssetSource
    {
        /// <summary>
        /// Specific <see cref="IFileSystem"/> to load the asset source from.
        /// 
        /// If null, then file-system is unspecified, and the caller have a reference to known file-system.
        /// 
        /// The reference is immutable and must not be modified after construction.
        /// </summary>
        IFileSystem FileSystem { get; }
    }

    /// <summary>
    /// Provides a specific <see cref="IAssetObservePolicy"/> for the <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSourceObservePolicy : IAssetSource
    {
        /// <summary>
        /// Reference for asset file observe policy for the asset source.
        /// 
        /// If null, then the asset source does not specify whether it should be observed or not.
        /// 
        /// The reference is immutable and must not be modified after construction.
        /// </summary>
        IAssetObservePolicy ObservePolicy { get; }
    }

    /// <summary>
    /// Asset source that originates from one specific file.
    /// 
    /// The implementing class can implement <see cref="IAssetSourceFileSystem"/> which 
    /// gives a reference to <see cref="IFileSystem"/> from which the file is to be loaded.
    /// 
    /// The implementing class can implement <see cref="IStringAssetSource"/> or <see cref="IBinaryAssetSource"/> to signal
    /// the content type of the asset file.
    /// </summary>
    public interface IFileAssetSource : IAssetSource
    {
        /// <summary>
        /// Reference to an asset file. Used within <see cref="IFileSystem"/>. Directory separator is '/'. Root doesn't use separator.
        /// 
        /// Example: "resources/localization.xml".
        /// </summary>
        string FilePath { get; }
    }

    /// <summary>
    /// Asset source that referers to a pattern of filenames.
    /// 
    /// The implementing class can implement <see cref="IAssetSourceFileSystem"/> which 
    /// gives a reference to <see cref="IFileSystem"/> from which the file is to be loaded.
    /// 
    /// The implementing class can implement <see cref="IStringAssetSource"/> or <see cref="IBinaryAssetSource"/> to signal
    /// the content type of the asset file.
    /// </summary>
    public interface IFilePatternAssetSource : IAssetSource
    {
        /// <summary>
        /// Reference to a pattern of asset files. Used within <see cref="IFileSystem"/>.
        /// 
        /// Separator character is '/'. Root doesn't use separator.
        /// Example: "resources/{Culture/}localization.xml".
        /// </summary>
        ILinePattern FilePattern { get; }
    }

    /// <summary>
    /// Signals that the implementing <see cref="IAssetSource"/> constructs a <see cref="IStringAsset"/>.
    /// </summary>
    public interface IStringAssetSource : IAssetSource
    {
        /// <summary>
        /// File format to use to read the file. 
        /// 
        /// If null, then file format is not available.
        /// </summary>
        ILineFileFormat FileFormat { get; }

        /// <summary>
        /// Information whether the asset contains binary lines.
        /// </summary>
        bool IsStringSource { get; }
    }

    /// <summary>
    /// Signals that the implementing <see cref="IAssetSource"/> constructs a <see cref="IBinaryAsset"/>.
    /// </summary>
    public interface IBinaryAssetSource : IAssetSource
    {
        /// <summary>
        /// Information whether the asset contains binary lines.
        /// </summary>
        bool ContainsBinaryLines { get; }
    }

    /// <summary>
    /// Asset source where lines can be enumerated.
    /// 
    /// The implementation of this class must use one of the more specific sub-interfaces:
    /// <list type="table">
    ///     <item><see cref="ILineAssetSource"/></item>
    ///     <item><see cref="IUnformedLineAssetSource"/></item>
    ///     <item><see cref="ILineTreeAssetSource"/></item>
    /// </list>
    /// 
    /// <see cref="IEnumerable.GetEnumerator"/> may throw exception such as <see cref="IOException"/>
    /// if the source file is not available.
    /// </summary>
    public interface IEnumerableAssetSource : IAssetSource
    {
    }

    /// <summary>
    /// Asset source that reads lines every time <see cref="IEnumerable{T}.GetEnumerator"/> is called.
    /// </summary>
    public interface ILineAssetSource : IEnumerableAssetSource, IEnumerable<ILine>
    {
        /// <summary>
        /// Information whether asset source can enumerate <see cref="ILine"/>s.
        /// </summary>
        bool EnumeratesLines { get; }
    }

    /// <summary>
    /// Asset source that reads lines every time <see cref="IEnumerable{T}.GetEnumerator"/> is called.
    /// </summary>
    public interface IUnformedLineAssetSource : IEnumerableAssetSource, IEnumerable<KeyValuePair<string, IString>>
    {
        /// <summary>
        /// Information whether asset source can enumerate unformed lines KeyValuePair&lt;<see cref="string"/>, <see cref="IString"/>&gt;
        /// </summary>
        bool EnumeratesUnformedLines { get; }
    }

    /// <summary>
    /// Asset source that reads lines every time <see cref="IEnumerable{T}.GetEnumerator"/> is called.
    /// </summary>
    public interface ILineTreeAssetSource : IEnumerableAssetSource, IEnumerable<ILineTree>
    {
        /// <summary>
        /// Information whether asset source can enumerate as <see cref="ILineTree"/>.
        /// </summary>
        bool EnumeratesLineTree { get; }
    }

    /// <summary>
    /// Enumerable asset source that can be observed for changes.
    /// </summary>
    public interface IObservableAssetSource : IEnumerableAssetSource, IObservable<IAssetSourceEvent>
    {
        /// <summary>
        /// If returns true, the asset source is observable.
        /// </summary>
        bool IsObservable { get; }
    }

    /// <summary>
    /// Asset source event.
    /// </summary>
    public interface IAssetSourceEvent
    {
        /// <summary>
        /// The asset source that is being observed
        /// </summary>
        IAssetSource AssetSource { get; }

        /// <summary>
        /// Change events
        /// </summary>
        WatcherChangeTypes ChangeEvents { get; }
    }

    /// <summary>
    /// Asset loader loads assets based on key parts in a <see cref="ILine"/>.
    /// </summary>
    public interface IAssetLoaderSource : IAssetSource
    {
        /// <summary>
        /// Match the key parts in <paramref name="line"/> to <see cref="IAssetSource"/>.
        /// 
        /// If keys didn't match the criteria of the loader, then returns null.
        /// 
        /// The implementation should always return same array reference for each line match.
        /// Unless the source is changed. The caller uses the reference for caching.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>matching asset sources or null if <paramref name="line"/> didn't match</returns>
        IAssetSource[] Match(ILine line);

        /// <summary>
        /// List asset sources of all resources represented by this loader.
        /// </summary>
        /// <returns>asset sources or null if the loader cannot list sources</returns>
        IEnumerable<IAssetSource> List();
    }

    /// <summary>
    /// Used with dependency injection as a service type of multiple <see cref="IAssetSource"/>s.
    /// 
    /// This is a workaround to the problem, where service provider implementation doesn't 
    /// concatenate <see cref="IAssetSource"/> and <see cref="IEnumerable{IAssetSource}"/>s together.
    /// And supplying an <see cref="IEnumerable{IAssetSource}"/> messes up the service descriptions
    /// of <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSources : IEnumerable<IAssetSource> { }
    // </doc>
}
