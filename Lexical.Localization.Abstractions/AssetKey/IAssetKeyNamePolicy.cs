// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    #region IAssetKeyNamePolicy
    /// <summary>
    /// Signal that the class can do conversions of <see cref="IAssetKey"/> and <see cref="String"/>.
    /// 
    /// User of this interface should use extensions methods 
    /// <list type="bullet">
    /// <item><see cref="AssetKeyNamePolicyExtensions.BuildName(IAssetKeyNamePolicy, IAssetKey)"/></item>
    /// <item><see cref="AssetKeyNamePolicyExtensions.Parse(IAssetKeyNamePolicy, string, IAssetKey)"/></item>
    /// </list>
    /// 
    /// Class that implements to this interface should implement one or both of the following interfaces:
    ///  <see cref="IAssetKeyNameProvider"/>
    ///  <see cref="IAssetNamePattern"/>
    /// </summary>
    public interface IAssetKeyNamePolicy
    {
    }
    #endregion IAssetKeyNamePolicy

    #region IAssetKeyNameProvider
    /// <summary>
    /// Converts <see cref="IAssetKey"/> to <see cref="String"/>.
    /// </summary>
    public interface IAssetKeyNameProvider : IAssetKeyNamePolicy
    {
        /// <summary>
        /// Build path string from key.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>full name string</returns>
        string BuildName(IAssetKey str);
    }
    #endregion IAssetKeyNameProvider

    #region IAssetKeyNameParser
    /// <summary>
    /// Parses <see cref="String"/> into <see cref="IAssetKey"/>.
    /// </summary>
    public interface IAssetKeyNameParser : IAssetKeyNamePolicy
    {
        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str">key as string</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="FormatException">If parse failed</exception>
        IAssetKey Parse(string str, IAssetKey rootKey = default);

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>true if parse was successful</returns>
        bool TryParse(string str, out IAssetKey key, IAssetKey rootKey = default);
    }
    #endregion IAssetKeyNameParser

    /// <summary>
    /// Extension functions for <see cref="IAssetKeyNamePolicy"/>.
    /// </summary>
    public static partial class AssetKeyNamePolicyExtensions
    {
        /// <summary>
        /// Build name for key. 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="key"></param>
        /// <returns>full name string or null</returns>
        public static string BuildName(this IAssetKeyNamePolicy policy, IAssetKey key)
        {
            if (policy is IAssetKeyNameProvider provider)
            {
                string name = provider.BuildName(key);
                if (name != null) return name;
            }
            return null;
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="str">key as string</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="FormatException">If parse failed</exception>
        /// <exception cref="ArgumentException">If <paramref name="policy"/> doesn't implement <see cref="IAssetKeyNameParser"/>.</exception>
        public static IAssetKey Parse(this IAssetKeyNamePolicy policy, string str, IAssetKey rootKey = default)
            => policy is IAssetKeyNameParser parser ? parser.Parse(str, rootKey) : throw new ArgumentException($"Cannot parse strings to {nameof(IAssetKey)} with {policy.GetType().FullName}. {policy} doesn't implement {nameof(IAssetKeyNameParser)}.");

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>true if parse was successful (even through resulted key might be null)</returns>
        public static bool TryParse(this IAssetKeyNamePolicy policy, string str, out IAssetKey key, IAssetKey rootKey = default)
        {
            if (policy is IAssetKeyNameParser parser) return parser.TryParse(str, out key, rootKey);
            key = null;
            return false;
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="str"></param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>Key or null</returns>
        public static IAssetKey TryParse(this IAssetKeyNamePolicy policy, string str, IAssetKey rootKey = default)
        {
            IAssetKey key;
            if (policy is IAssetKeyNameParser parser && parser.TryParse(str, out key, rootKey)) return key;
            return null;
        }

    }
}
