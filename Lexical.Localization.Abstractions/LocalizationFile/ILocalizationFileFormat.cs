//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Represents a localization file format.
    /// 
    /// For reading capability, must implement one of:
    /// <list type="Bullet">
    /// <item><see cref="ILocalizationKeyLinesStreamReader"/></item>
    /// <item><see cref="ILocalizationKeyTreeStreamReader"/></item>
    /// <item><see cref="ILocalizationKeyLinesTextReader"/></item>
    /// <item><see cref="ILocalizationKeyTreeTextReader"/></item>
    /// <item><see cref="ILocalizationStringLinesTextReader"/></item>
    /// <item><see cref="ILocalizationStringLinesStreamReader"/></item>
    /// </list>
    /// 
    /// For writing capability, must implement one of:
    /// <list type="Bullet">
    /// <item><see cref="ILocalizationStringLinesTextWriter"/></item>
    /// <item><see cref="ILocalizationStringLinesStreamWriter"/></item>
    /// <item><see cref="ILocalizationKeyLinesTextWriter"/></item>
    /// <item><see cref="ILocalizationKeyLinesStreamWriter"/></item>
    /// <item><see cref="ILocalizationKeyTreeStreamWriter"/></item>
    /// <item><see cref="ILocalizationKeyTreeTextWriter"/></item>
    /// </list>
    /// </summary>
    public interface ILocalizationFileFormat
    {
        /// <summary>
        /// Extension of the file format without separator. e.g. "xml".
        /// </summary>
        string Extension { get; }
    }

}
