// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.Localization;

namespace Lexical.Localization
{
    public static partial class MsLocalizationExtensions
    {
        /// <summary>
        /// Cast to string localizer.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static IStringLocalizer AsStringLocalizer(this ILine line)
            => line is IStringLocalizer sl ? sl : new StringLocalizerPart(line.FindAppender(), line);

        /// <summary>
        /// Cast to string localizer.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static IStringLocalizer<T> AsStringLocalizer<T>(this ILine line)
            => line is IStringLocalizer<T> sl ? sl : new StringLocalizerPart<T>(line.FindAppender(), line);

        /// <summary>
        /// Cast to string localizer factory.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static IStringLocalizerFactory AsStringLocalizerFactory(this ILine line)
            => line is IStringLocalizerFactory sl ? sl : new StringLocalizerPart(line.FindAppender(), line);

    }
}
