// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Line's <see cref="IAsset"/> assignment.
    /// </summary>
    public interface ILineAsset : ILine
    {
        /// <summary>
        /// Object that contains localization assets.
        /// </summary>
        IAsset Asset { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append localizatoin asset.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="asset"></param>
        /// <returns>part with asset</returns>
        public static ILineAsset Asset(this ILine line, IAsset asset)
            => line.Append<ILineAsset, IAsset>(asset);

        /// <summary>
        /// Get IAsset. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>asset</returns>
        /// <exception cref="LineException">If asset is not found</exception>
        public static IAsset GetAsset(this ILine line)
        {
            for (; line != null; line = line.GetPreviousPart())
                if (line is ILineAsset lineAsset && lineAsset.Asset != null)
                    return lineAsset.Asset;
            throw new LineException(line, $"Could not find {nameof(IAsset)}");
        }

        /// <summary>
        /// Search for IAsset. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>asset or null</returns>
        public static IAsset FindAsset(this ILine line)
        {
            for (; line != null; line = line.GetPreviousPart())
                if (line is ILineAsset lineAsset && lineAsset.Asset != null)
                    return lineAsset.Asset;
            return null;
        }
    }

}
