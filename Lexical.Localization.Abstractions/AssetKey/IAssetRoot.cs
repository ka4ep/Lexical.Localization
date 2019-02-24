// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// This interface signals that this key is the root key.
    /// 
    /// Name of localization root is "".
    /// </summary>
    public interface IAssetRoot : IAssetKey
    {
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Get the root key that implements <see cref="IAssetRoot"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>root key or null</returns>
        public static IAssetRoot GetRoot(this IAssetKey key)
            => key.Find<IAssetRoot>();
    }
}
