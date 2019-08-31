// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Asset
{
    // <doc>
    /// <summary>
    /// Source of assets. Adds resources to builder's list.
    /// 
    /// The implementation of this class must use one of the more specific sub-interfaces:
    /// <list type="table">
    /// <item><see cref="ILinesSource"/></item>
    /// <item><see cref="IResourcesSource"/></item>
    /// </list>
    /// </summary>
    /// </summary>
    public interface IAssetSource
    {
        /// <summary>
        /// Source adds its <see cref="IAsset"/>s to list.
        /// </summary>
        /// <param name="list">list to add provider(s) to</param>
        /// <returns>self</returns>
        void Build(IList<IAsset> list);
    }
    // </doc>

    // <doc2>
    /// <summary>
    /// Signals that <see cref="IAssetSource"/> constructs lines asset.
    /// 
    /// The implementation of this class must use one of the more specific sub-interfaces:
    /// <list type="table">
    /// <item><see cref="IStringLinesSource"/></item>
    /// <item><see cref="IKeyLinesSource"/></item>
    /// <item><see cref="ITreeLinesSource"/></item>
    /// </list>
    /// </summary>
    public interface ILinesSource : IAssetSource
    {
    }

    /// <summary>
    /// Source that provides string based key-value lines.
    /// 
    /// String line source is used for file types such as .resx and .resources, that do not carry
    /// information about key parts within the file itself. 
    /// 
    /// <see cref="ILineFormat"/> is used to translate the string into <see cref="ILinePart"/>s.
    /// 
    /// For instance, if a string follows pattern "{Culture.}{Type.}[Key]" the <see cref="LineFormat"/> would 
    /// convert lines such as "En.MyController.Success" into <see cref="ILine"/>.
    /// </summary>
    public interface IStringLinesSource : ILinesSource, IEnumerable<KeyValuePair<string, IString>>
    {
        /// <summary>
        /// Format that is used for converting string to <see cref="ILine"/>.
        /// </summary>
        ILineFormat LineFormat { get; }
    }

    /// <summary>
    /// Source that provides <see cref="ILine"/> based key-value lines.
    /// </summary>
    public interface IKeyLinesSource : ILinesSource, IEnumerable<ILine>
    {
    }

    /// <summary>
    /// Source that provides <see cref="ILineTree"/> based key-value lines.
    /// </summary>
    public interface ITreeLinesSource : ILinesSource, IEnumerable<ILineTree>
    {
    }
    // </doc2>

    // <doc3>
    /// <summary>
    /// Signals that <see cref="IAssetSource"/> constructs resources asset.
    /// </summary>
    public interface IResourcesSource : IAssetSource
    {
    }
    // </doc3>


    // <attribute>
    /// <summary>
    /// Used with dependency injection as a service type of multiple <see cref="IAssetSource"/>s.
    /// 
    /// This is a workaround to the problem, where service provider implementation doesn't 
    /// concatenate <see cref="IAssetSource"/> and <see cref="IEnumerable{IAssetSource}"/>s together.
    /// And supplying an <see cref="IEnumerable{IAssetSource}"/> messes up the service descriptions
    /// of <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSources : IEnumerable<IAssetSource> { }

    /// <summary>
    /// This interface signals that the implementing class provides
    /// asset sources for the class library.
    /// </summary>
    public interface ILibraryAssetSources : IAssetSources { }
    // </attribute>
}
