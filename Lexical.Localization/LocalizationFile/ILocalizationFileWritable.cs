// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.LocalizationFile
{
    /// <summary>
    /// Interface for a container of localization keys.
    /// </summary>
    public interface ILocalizationFileWritable : IDisposable
    {
        /// <summary>
        /// Name policy
        /// </summary>
        IAssetKeyNamePolicy NamePolicy { get; }

        /// <summary>
        /// Write tree completely.
        /// </summary>
        /// <param name="root"></param>
        void Write(TreeNode root);

        /// <summary>
        /// Flush content to source.
        /// </summary>
        void Flush();
    }

    public static class LocalizationFileWriterExtensions
    {
        /// <summary>
        /// Write using <see cref="IAssetKey"/> map.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="keyValues"></param>
        public static void Write(this ILocalizationFileWritable writer, IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
            => writer.Write(TreeNode.Create(keyValues));
        /// <summary>
        /// Converts <see cref="IAssetKey"/> to ParameterKey.
        /// 
        /// Reorders non-canonical parts to be first, and "culture" to be very first.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<object, string>> Convert(IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
        {
            throw new NotImplementedException("Deprecated and to be removed");
        }
    }

}
