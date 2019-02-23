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
        public static void Write(this ILocalizationFileWritable writer, IEnumerable<KeyValuePair<object, string>> keyValues, IAssetKeyParametrizer parametrizer = default)
            => writer.Write( TreeNode.Create(keyValues, parametrizer) );

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
        /// <param name="parametrizer">(optional) code that extracts sections from keys</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<object, string>> Convert(IEnumerable<KeyValuePair<IAssetKey, string>> keyValues, IAssetKeyParametrizer parametrizer = default)
        {
            if (parametrizer == null) parametrizer = AssetKeyParametrizer.Singleton;
            List<Key> list = new List<Key>();
            foreach (var kp in keyValues)
            {
                // Arrange
                list.Clear();
                object[] parts = parametrizer.Break(kp.Key)?.ToArray();
                if (parts == null || parts.Length == 0) continue;

                // Add non-canonical parts
                foreach (var key_part in parts.Where(part=>part is IAssetKeyNonCanonicallyCompared))
                {
                    string[] parameters = parametrizer.GetPartParameters(key_part);
                    if (parameters == null || parameters.Length == 0) continue;
                    foreach (string parameter in parameters)
                    {
                        string value = parametrizer.GetPartValue(key_part, parameter);
                        if (value == null) continue;
                        list.Add(new Key.NonCanonical(parameter, value));
                    }
                }
                // Sort non-canonicals by parameter names
                list.Sort(Key.Comparer.Default);

                // Add canonical parts
                foreach (var key_part in parts.Where(part => part is IAssetKeyNonCanonicallyCompared == false))
                {
                    string[] parameters = parametrizer.GetPartParameters(key_part);
                    if (parameters == null || parameters.Length == 0) continue;
                    foreach (string parameter in parameters)
                    {
                        string value = parametrizer.GetPartValue(key_part, parameter);
                        if (value == null) continue;
                        list.Add(new Key(parameter, value));
                    }
                }

                // Fix links
                for (int i = 1; i < list.Count; i++)
                    list[i].Previous = list[i - 1];

                // Result
                yield return new KeyValuePair<object, string>(list[list.Count-1], kp.Value);
            }
        }
    }

}
