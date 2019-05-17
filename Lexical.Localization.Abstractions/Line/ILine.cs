// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

namespace Lexical.Localization
{
    /// <summary>
    /// Line that can contain resolve hints, be comparable as key, and have a string value.
    /// </summary>
    public interface ILine
    {
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Find <see cref="IAsset"/> and get format string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>format string</returns>
        /// <exception cref="LineException">If resolving failed or resolver was not found</exception>
        public static IFormatString GetString(this ILine key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new LineException(key, "String resolver was not found.");
            IFormatString str = asset.GetString(key);
            if (str == null) throw new LineException(key, "String was not found.");
            return str;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get format string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>format string, or null if format string was not found, or if resolver was not found</returns>
        public static IFormatString TryGetString(this ILine key)
            => key.FindAsset()?.GetString(key);

        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        /// <exception cref="LineException">If resolving failed or resolver was not found</exception>
        public static byte[] GetResource(this ILine key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new LineException(key, "String resolver was not found.");
            byte[] data = asset.GetResource(key);
            if (data == null) throw new LineException(key, $"String {key} was not found.");
            return data;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        public static byte[] TryGetResource(this ILine key)
            => key.FindAsset()?.GetResource(key);
    }


}
