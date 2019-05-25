// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolves name to <typeparamref name="T"/>.
    /// </summary>
    public interface IResolver<T>
    {
        /// <summary>
        /// Resolve <paramref name="className"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="className"></param>
        /// <returns><typeparamref name="T"/> or null</returns>
        T Resolve(string className);

        /// <summary>
        /// Try resolve into value.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="result"></param>
        /// <returns>true if was resolved with result</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        bool TryResolve(string typeName, out T result);
    }
}
