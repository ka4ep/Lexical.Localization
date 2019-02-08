// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "location" parameter assignment.
    /// 
    /// Location is hint that adds physical directory to search assets from to a given key.
    /// <see cref="IAsset"/> implementation may use this hint to search assets from. 
    /// This hint can be provided multiple times, for example: key = key.Location("Assets/").Location("Icons/");
    /// If <see cref="IAsset"/> implementation uses name pattern, the hint in key reflects to "location_0", "location_1", etc§ parts in name pattern. Part "location" reflects to the last hint.
    /// For example, location hints would reflect to respective parts in the following name pattern: "{location_0/}{location_1/}{type/}{section.}{key}.ini".
    /// 
    /// Consumers of this interface should call the extension method <see cref="AssetKeyExtensions.Location(IAssetKey, string)"/>.
    /// </summary>
    public interface IAssetKeyLocationSectionAssignable : IAssetKey
    {
        /// <summary>
        /// Add location to the key.
        /// 
        /// Location location is a hint that describes the location embedded locations of localization files.
        /// </summary>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        [AssetKeyConstructor(parameterName: "location")]
        IAssetKeyLocationSection Location(string location);
    }

    /// <summary>
    /// Key that (may have) has been assigned with a "location" parameter.
    /// 
    /// Location means physical directory to search assets from.
    /// 
    /// For example, key.Location("Assets/") would be put in place of "location" in following filen name pattern "{location/}{type/}{section.}{key}.ini".
    /// </summary>
    [AssetKeyParameter(parameterName: "location")]
    public interface IAssetKeyLocationSection : IAssetKeySection
    {
    }

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyLocationSection"/> section.
        /// 
        /// Location section is a hint that points to folder where asset is to be loaded.
        /// For example adding "Icons" location section, would mean that when key is matched to file assets, only "Icons" folder is used.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ITypeAssignableLocalizationKey</exception>
        public static IAssetKeyLocationSection Location(this IAssetKey key, string location)
        {
            if (key is IAssetKeyLocationSectionAssignable casted) return casted.Location(location);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyLocationSectionAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyLocationSection"/> section.
        /// 
        /// Location section is a hint that points to folder where asset is to be loaded.
        /// For example adding "Icons" location section, would mean that when key is matched to file assets, only "Icons" folder is used.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyLocationSection TryAddLocation(this IAssetKey key, String location)
        {
            if (key is IAssetKeyLocationSectionAssignable casted) return casted.Location(location);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyLocationSection"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyLocationSection FindLocationSection(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyLocationSection locationKey && !string.IsNullOrEmpty(key.Name)) return locationKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyLocationSection"/> that has a resolved Assembky.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static string FindLocation(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyLocationSection locationKey && locationKey.Name != null) return locationKey.Name;
                key = key.GetPreviousKey();
            }
            return null;
        }

    }
}
