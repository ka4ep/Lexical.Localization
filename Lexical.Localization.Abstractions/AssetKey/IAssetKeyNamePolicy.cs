// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    #region IAssetKeyNamePolicy
    /// <summary>
    /// Signal that the class can convert <see cref="IAssetKey"/> into strings.
    /// 
    /// Consumer of this interface should call <see cref="AssetKeyExtensions.BuildName(IAssetKeyNamePolicy, IAssetKey)"/>.
    /// 
    /// Producer to this interface should implement one of the more specific interfaces:
    ///  <see cref="IAssetKeyNameProvider"/>
    ///  <see cref="IAssetNamePattern"/>
    /// </summary>
    public interface IAssetKeyNamePolicy
    {
    }
    #endregion IAssetKeyNamePolicy

    #region IAssetKeyNameProvider
    public interface IAssetKeyNameProvider : IAssetKeyNamePolicy
    {
        /// <summary>
        /// Build path string from key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>full name string</returns>
        string BuildName(IAssetKey key);
    }
    #endregion IAssetKeyNameProvider

    /// <summary>
    /// Converts localization key to a strings that identifies a language string.
    /// 
    /// Policy depends on how keys are formulated in the localization files.
    /// </summary>
    public interface IAssetKeyNameDescription : IAssetKeyNamePolicy
    {
        /// <summary>
        /// Get all parameter policies
        /// </summary>
        IEnumerable<IAssetKeyParameterDescription> Parameters { get; }

        /// <summary>
        /// Get parameter policy by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAssetKeyParameterDescription this[string name] { get; }
    }

    public interface IAssetKeyParameterDescription
    {
        /// <summary>
        /// Identifier in <see cref="IAssetKeyParametrizer"/>, non-unique identifier.
        /// </summary>
        string ParameterName { get; }

        /// <summary>
        /// Separator
        /// </summary>
        string PrefixSeparator { get; }

        /// <summary>
        /// Separator
        /// </summary>
        string PostfixSeparator { get; }

        /// <summary>
        /// Is this parameter to be included in name .
        /// </summary>
        bool IsIncluded { get; }
    }

    public static partial class AssetKeyNamePolicyExtensions
    {
        /// <summary>
        /// Build name for key. 
        /// 
        /// This method completely ignores the selected culture in the key and uses the <paramref name="culture"/>.
        /// If <paramref name="culture"/> is null, then doesn't print any culture at all.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="policy"></param>
        /// <returns>full name string</returns>
        public static string BuildName(this IAssetKeyNamePolicy policy, IAssetKey key)
        {
            if (policy is IAssetKeyNameProvider provider)
            {
                string name = provider.BuildName(key);
                if (name != null) return name;
            }
            if (policy is IAssetNamePattern pattern)
            {
                IAssetNamePatternMatch match = pattern.Match(key);
                string name = AssetNamePatternExtensions.BuildName(pattern, match.PartValues);
                if (name != null) return name;
            }
            return null;
        }

    }
}
