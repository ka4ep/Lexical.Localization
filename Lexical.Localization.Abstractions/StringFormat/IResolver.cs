// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

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
    }
}
