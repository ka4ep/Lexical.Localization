// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Key" parameter assignment.
    /// 
    /// Key is a leaf, or a tail part, of the identity. 
    /// Such as "Error" in key "ConsoleApp1:MyController:Error".
    /// 
    /// Consumers of this interface should use the extension method <see cref="LinePartExtensions.Key(ILinePart, string)"/>.
    /// </summary>
    public interface IAssetKeyAssignable : ILinePart
    {
        /// <summary>
        /// Concatenate new name to this key and create a new subkey.
        /// 
        /// For instance if this is "ConsoleApp1:MyController" and subkey is "Error", then 
        /// the key of the resulted instance is "ConsoleApp1:MyController:Error".
        /// </summary>
        /// <param name="name">Name for new sub key.</param>
        /// <returns>new key</returns>
        IAssetKeyAssigned Key(string keyName);
    }

    /// <summary>
    /// Key (may have) has a "Key" parameter assignment.
    /// 
    /// Key is a leaf, or a tail part, of the identity. 
    /// Such as "Error" in key "ConsoleApp1:MyController:Error".
    /// </summary>
    public interface IAssetKeyAssigned : ILinePart
    {
    }

    public static partial class LinePartExtensions
    {
        /// <summary>
        /// Add regular subkey.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        /// <exception cref="AssetException"></exception>
        public static IAssetKeyAssigned Key(this ILinePart key, string keyName)
            => key is IAssetKeyAssignable assignable ? assignable.Key(keyName) : throw new AssetException($"Key is not {nameof(IAssetKeyAssignable)}");

        /// <summary>
        /// Try to add key section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyAssigned TryAddKey(this ILinePart key, string name)
        {
            if (key is IAssetKeyAssignable casted) return casted.Key(name);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyAssigned"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>key or null</returns>
        public static ILinePart FindKey(this ILinePart key)
        {
            while (key != null)
            {
                if (key is IAssetKeyAssigned && !string.IsNullOrEmpty(key.GetParameterValue())) return key;
                key = key.PreviousPart;
            }
            return null;
        }
    }

}
