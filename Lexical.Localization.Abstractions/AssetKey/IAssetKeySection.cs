using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// This interface signals that this key represents a section.
    /// </summary>
    public interface IAssetKeySection : IAssetKey
    {
    }

    /// <summary>
    /// Key has capability of "Section" parameter assignment.
    /// 
    /// Regular section as a folder is used when loading assets from files, embedded resources, and withint language string dictionaries.
    /// 
    /// Consumers of this interface should use the extension method <see cref="AssetKeyExtensions.Section(IAssetKey, string)"/>.
    /// </summary>
    public interface IAssetKeySectionAssignable : IAssetKey
    {
        /// <summary>
        /// Create section.
        /// </summary>
        /// <param name="sectionName">Name section.</param>
        /// <returns>new key</returns>
        IAssetKeySectionAssigned Section(string sectionName);
    }

    /// <summary>
    /// Key (may have) has been "Section" parameter assignment.
    /// 
    /// Regular section as a folder is used when loading assets from files, embedded resources, and withint language string dictionaries.
    /// </summary>
    public interface IAssetKeySectionAssigned : IAssetKeySection, IAssetKey
    {
    }

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Create section
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ISectionAssignableLocalizationKey</exception>
        public static IAssetKeySectionAssigned Section(this IAssetKey key, String name)
        {
            if (key is IAssetKeySectionAssignable casted) return casted.Section(name);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeySectionAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeySectionAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeySectionAssigned TryAddSection(this IAssetKey key, string name)
        {
            if (key is IAssetKeyTypeAssignable casted) return casted.Section(name);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeySection"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>key or null</returns>
        public static IAssetKeySection FindSection(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeySection sectionKey && !string.IsNullOrEmpty(key.Name)) return sectionKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeySectionAssigned"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>key or null</returns>
        public static IAssetKeySectionAssigned FindSectionKey(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeySectionAssigned sectionKey && !string.IsNullOrEmpty(key.Name)) return sectionKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

    }

}
