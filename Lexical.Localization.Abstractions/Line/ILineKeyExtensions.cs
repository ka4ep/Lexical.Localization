// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Key" parameter assignment.
    /// 
    /// Key is a leaf, or a tail part, of the identity. 
    /// Such as "Error" in key "ConsoleApp1:MyController:Error".
    /// 
    /// Consumers of this interface should use the extension method <see cref="ILineExtensions.Key(ILine, string)"/>.
    /// </summary>
    [Obsolete]
    public interface IAssetKeyAssignable : ILine
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
    public interface IAssetKeyAssigned : ILine
    {
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append "Key" non-canonical key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Key(this ILine line, string key)
            => line.Append<ILineKeyCanonicallyCompared, string, string>("Key", key);

        /// <summary>
        /// Try append "Key" non-canonical key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="key"></param>
        /// <returns>new key or null</returns>
        public static ILine TryAddKey(this ILine line, string key)
            => line.TryAppend<ILineKeyCanonicallyCompared, string, string>("Key", key);

    }

}
