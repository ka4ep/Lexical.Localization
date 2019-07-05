// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.StringFormat;
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
        /// Append localization asset.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="asset"></param>
        /// <returns>part with asset</returns>
        public static ILineAsset Asset(this ILine line, IAsset asset)
            => line.Append<ILineAsset, IAsset>(asset);

        /// <summary>
        /// Append localization asset.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="asset"></param>
        /// <returns>part with asset</returns>
        public static ILineAsset Asset(this ILineFactory lineFactory, IAsset asset)
            => lineFactory.Create<ILineAsset, IAsset>(null, asset);

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
        /// Try get IAsset. 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="asset"></param>
        /// <returns>true if asset was returned</returns>
        public static bool TryGetAsset(this ILine line, out IAsset asset)
        {
            for (; line != null; line = line.GetPreviousPart())
                if (line is ILineAsset lineAsset && lineAsset.Asset != null)
                    { asset = lineAsset.Asset; return true; }
            asset = default; return false;
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

        /// <summary>
        /// Get key-matching value from associated <see cref="IAsset"/>.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveFormatString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>format string</returns>
        /// <exception cref="LineException">If resolving failed or resolver was not found</exception>
        public static IString GetAssetValue(this ILine key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new LineException(key, "String resolver was not found.");
            ILine line = asset.GetLine(key);
            if (line == null) throw new LineException(key, "String was not found.");
            return line.GetString();
        }

        /// <summary>
        /// Try get key-matching value from associated <see cref="IAsset"/>.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveFormatString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns>format string, or null if format string was not found, or if resolver was not found</returns>
        public static bool TryGetAssetValue(this ILine key, out IString result)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) { result = null; return false; }
            ILine line = asset.GetLine(key);
            if (line == null) { result = null; return false; }
            IString str = line.GetString();
            result = str;
            return str.Text != null;
        }
    }

}
