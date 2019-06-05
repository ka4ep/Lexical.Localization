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

    /// <summary>
    /// 
    /// </summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Make clone. <paramref name="line"/> must have an appender.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Clone(this ILine line)
            => line.GetAppender().Clone(line);

        /// <summary>
        /// Try to create clone
        /// </summary>
        /// <param name="line"></param>
        /// <param name="clone"></param>
        /// <returns></returns>
        public static bool TryClone(this ILine line, out ILine clone)
        {
            ILineFactory appender;
            if (line.TryGetAppender(out appender) && appender.TryClone(line, out clone)) return true;
            clone = default;
            return false;
        }
    }

}
