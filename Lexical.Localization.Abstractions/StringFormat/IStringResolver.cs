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
        /// Resolve <paramref name="key"/> into <see cref="IFormatString"/>, but don't apply arguments.
        /// 
        /// If <paramref name="key"/> contains arguments, then resolves into the applicable plurality case of the format string.
        /// </summary>
        /// <param name="key"></param>
        LineString ResolveString(ILine key);

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="IFormatString"/>, and apply arguments.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        LineString ResolveFormulatedString(ILine key);
    }

}
