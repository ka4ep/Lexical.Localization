// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Resolver that resolves <see cref="IAssetKey"/> into <see cref="LocalizationString"/>.
    /// </summary>
    public interface ILocalizationResolver
    {
        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="ILocalizationFormulationString"/>, but don't apply arguments.
        /// 
        /// If <paramref name="key"/> contains arguments, then resolves into the applicable plurality case of the formulation string.
        /// </summary>
        /// <param name="key"></param>
        LocalizationString ResolveString(IAssetKey key);

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="ILocalizationFormulationString"/>, and apply arguments.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        LocalizationString ResolveFormulatedString(IAssetKey key);
    }

}
