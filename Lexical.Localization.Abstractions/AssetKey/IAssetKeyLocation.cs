// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Location" parameter assignment.
    /// 
    /// Location is hint that adds physical directory to search assets from to a given key.
    /// <see cref="IAsset"/> implementation may use this hint to search assets from. 
    /// This hint can be provided multiple times, for example: key = key.Location("Assets").Location("Icons");
    /// If <see cref="IAsset"/> implementation uses name pattern, the hint in key reflects to "Location_0", "Location_1", etc parts in name pattern. Part "Location" reflects to the last hint.
    /// For example, location hints would reflect to respective parts in the following name pattern: "{Location_0/}{Location_1/}{Type/}{Section.}{Key}.ini".
    /// 
    /// Consumers of this interface should call the extension method <see cref="ILineExtensions.Location(ILine, string)"/>.
    /// </summary>
    public interface IAssetKeyLocationAssignable : ILine
    {
        /// <summary>
        /// Create a new key with "Location" added.
        /// 
        /// Location location is a hint that describes the location embedded locations of localization files.
        /// </summary>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        IAssetKeyLocationAssigned Location(string location);
    }

    /// <summary>
    /// Key that (may have) has been assigned with a "Location" parameter.
    /// 
    /// Location means physical directory to search assets from.
    /// 
    /// For example, key.Location("Assets") would be put in place of "Location" in following filen name pattern "{Location/}{Type/}{Section.}{Key}.ini".
    /// </summary>
    public interface IAssetKeyLocationAssigned : IAssetKeySection
    {
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyLocationAssigned"/> section.
        /// 
        /// Location section is a hint that points to folder where asset is to be loaded.
        /// For example adding "Icons" location section, would mean that when key is matched to file assets, only "Icons" folder is used.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement ITypeAssignableLocalizationKey</exception>
        public static IAssetKeyLocationAssigned Location(this ILine key, string location)
        {
            if (key is IAssetKeyLocationAssignable casted) return casted.Location(location);
            throw new LineException(key, $"doesn't implement {nameof(IAssetKeyLocationAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyLocationAssigned"/> section.
        /// 
        /// Location section is a hint that points to folder where asset is to be loaded.
        /// For example adding "Icons" location section, would mean that when key is matched to file assets, only "Icons" folder is used.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyLocationAssigned TryAddLocation(this ILine key, string location)
        {
            if (key is IAssetKeyLocationAssignable casted) return casted.Location(location);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyLocationAssigned"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyLocationAssigned FindLocationKey(this ILine key)
        {
            while (key != null)
            {
                if (key is IAssetKeyLocationAssigned locationKey && !string.IsNullOrEmpty(key.GetParameterValue())) return locationKey;
                key = key.GetPreviousPart();
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyLocationAssigned"/> that has a resolved Assembky.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static string FindLocation(this ILine key)
        {
            while (key != null)
            {
                if (key is IAssetKeyLocationAssigned locationKey && locationKey.GetParameterValue() != null) return locationKey.GetParameterValue();
                key = key.GetPreviousPart();
            }
            return null;
        }

    }
}
