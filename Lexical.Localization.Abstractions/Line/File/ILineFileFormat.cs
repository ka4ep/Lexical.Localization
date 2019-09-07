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
    /// <item><see cref="ILineStreamReader"/></item>
    /// <item><see cref="ILineTextReader"/></item>
    /// 
    /// <item><see cref="ILineTreeTextReader"/></item>
    /// <item><see cref="ILineTreeStreamReader"/></item>
    /// 
    /// <item><see cref="IUnformedLineTextReader"/></item>
    /// <item><see cref="IUnformedLineStreamReader"/></item>
    /// </list>
    /// 
    /// For writing capability, must implement one of:
    /// <list type="Bullet">
    /// <item><see cref="ILineStreamWriter"/></item>
    /// <item><see cref="ILineTextWriter"/></item>
    /// 
    /// <item><see cref="ILineTreeTextWriter"/></item>
    /// <item><see cref="ILineTreeStreamWriter"/></item>
    /// 
    /// <item><see cref="IUnformedLineTextWriter"/></item>
    /// <item><see cref="IUnformedLineStreamWriter"/></item>
    /// </list>
    /// </summary>
    public interface ILineFileFormat
    {
        /// <summary>
        /// Extension of the file format without separator. e.g. "xml".
        /// </summary>
        string Extension { get; }
    }

}
