// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolver that resolves <see cref="ILine"/> into <see cref="LineString"/>.
    /// </summary>
    public interface IStringResolver
    {
        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="IString"/>, but without applying format arguments.
        /// 
        /// If the <see cref="IString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>format string</returns>
        IString ResolveFormatString(ILine key);

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="LineString"/> with format arguments applied.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        LineString ResolveString(ILine key);
    }

}
