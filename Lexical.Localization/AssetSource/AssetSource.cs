// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Base asset source class.
    /// 
    /// List of known sub-classes.
    /// <list type="table">
    ///     
    ///     <item><see cref="FileAssetSource"/></item>
    ///     <item><see cref="FileAssetSource.Line.String"/></item>
    ///     <item><see cref="FileSystemUnformedLineAssetSource.UnformedLine.String"/></item>
    ///     <item><see cref="FileAssetSource.LineTree.String"/></item>
    ///     <item><see cref="FileAssetSource.Line.Binary"/></item>
    ///     <item><see cref="FileSystemUnformedLineAssetSource.UnformedLine.Binary"/></item>
    ///     <item><see cref="FileAssetSource.LineTree.Binary"/></item>
    ///     
    ///     <item><see cref="FileSystemPatternAssetSource"/></item>
    ///     <item><see cref="FileSystemPatternAssetSource.String"/></item>
    ///     <item><see cref="FileSystemPatternAssetSource.Binary"/></item>
    ///     
    ///     <item><see cref="EmbeddedFileAssetSource"/></item>
    ///     <item><see cref="EmbeddedFileAssetSource.Line.String"/></item>
    ///     <item><see cref="EmbeddedFileUnformedLineAssetSource.UnformedLine.String"/></item>
    ///     <item><see cref="EmbeddedFileAssetSource.LineTree.String"/></item>
    ///     <item><see cref="EmbeddedFileAssetSource.Line.Binary"/></item>
    ///     <item><see cref="EmbeddedFileUnformedLineAssetSource.UnformedLine.Binary"/></item>
    ///     <item><see cref="EmbeddedFileAssetSource.LineTree.Binary"/></item>
    ///     
    ///     <item><see cref="EmbeddedFilePatternAssetSource"/></item>
    ///     <item><see cref="EmbeddedFilePatternAssetSource.String"/></item>
    ///     <item><see cref="EmbeddedFilePatternAssetSource.Binary"/></item>
    ///     
    ///     <item><see cref="FileAssetSource"/></item>
    ///     <item><see cref="FileAssetSource.Line.String"/></item>
    ///     <item><see cref="FileUnformedLineAssetSource.UnformedLine.String"/></item>
    ///     <item><see cref="FileAssetSource.LineTree.String"/></item>
    ///     <item><see cref="FileAssetSource.Line.Binary"/></item>
    ///     <item><see cref="FileUnformedLineAssetSource.UnformedLine.Binary"/></item>
    ///     <item><see cref="FileAssetSource.LineTree.Binary"/></item>
    ///     
    ///     <item><see cref="FilePatternAssetSource"/></item>
    ///     <item><see cref="FilePatternAssetSource.String"/></item>
    ///     <item><see cref="FilePatternAssetSource.Binary"/></item>
    ///     
    ///     <item><see cref="FileProviderAssetSource"/></item>
    ///     <item><see cref="FileProviderAssetSource.Line.String"/></item>
    ///     <item><see cref="FileProviderUnformedLineAssetSource.UnformedLine.String"/></item>
    ///     <item><see cref="FileProviderAssetSource.LineTree.String"/></item>
    ///     <item><see cref="FileProviderAssetSource.Line.Binary"/></item>
    ///     <item><see cref="FileProviderUnformedLineAssetSource.UnformedLine.Binary"/></item>
    ///     <item><see cref="FileProviderAssetSource.LineTree.Binary"/></item>
    ///     
    ///     <item><see cref="FileProviderPatternAssetSource"/></item>
    ///     <item><see cref="FileProviderPatternAssetSource.String"/></item>
    ///     <item><see cref="FileProviderPatternAssetSource.Binary"/></item>
    ///     
    ///     <item><see cref=""/></item>
    ///     <item><see cref=""/></item>
    ///     <item><see cref=""/></item>
    ///     <item><see cref=""/></item>
    /// </list>
    /// </summary>
    public abstract class AssetSource : IAssetSource
    {
    }
}
