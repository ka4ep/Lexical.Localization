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
    /// Signal that the class can do conversions of <see cref="ILinePart"/> and <see cref="String"/>.
    /// 
    /// User of this interface should use extensions methods 
    /// <list type="bullet">
    /// <item><see cref="AssetKeyNamePolicyExtensions.BuildName(IAssetKeyNamePolicy, ILinePart)"/></item>
    /// <item><see cref="AssetKeyNamePolicyExtensions.Parse(IAssetKeyNamePolicy, string, ILinePart)"/></item>
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
    /// Converts <see cref="ILinePart"/> to <see cref="String"/>.
    /// </summary>
    public interface IAssetKeyNameProvider : IAssetKeyNamePolicy
    {
        /// <summary>
        /// Build path string from key.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>full name string</returns>
        string BuildName(ILinePart str);
    }
    #endregion IAssetKeyNameProvider

    #region IAssetKeyNameParser
    /// <summary>
    /// Parses <see cref="String"/> into <see cref="ILinePart"/>.
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
        ILinePart Parse(string str, ILinePart rootKey = default);

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>true if parse was successful</returns>
        bool TryParse(string str, out ILinePart key, ILinePart rootKey = default);
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
        public static string BuildName(this IAssetKeyNamePolicy policy, ILinePart key)
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
        public static ILinePart Parse(this IAssetKeyNamePolicy policy, string str, ILinePart rootKey = default)
            => policy is IAssetKeyNameParser parser ? parser.Parse(str, rootKey) : throw new ArgumentException($"Cannot parse strings to {nameof(ILinePart)} with {policy.GetType().FullName}. {policy} doesn't implement {nameof(IAssetKeyNameParser)}.");

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>true if parse was successful (even through resulted key might be null)</returns>
        public static bool TryParse(this IAssetKeyNamePolicy policy, string str, out ILinePart key, ILinePart rootKey = default)
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
        public static ILinePart TryParse(this IAssetKeyNamePolicy policy, string str, ILinePart rootKey = default)
        {
            ILinePart key;
            if (policy is IAssetKeyNameParser parser && parser.TryParse(str, out key, rootKey)) return key;
            return null;
        }

    }
}
