// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using Lexical.Localization.Resource;
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    #region Interface
    /// <summary>
    /// Localization logger.
    /// 
    /// See sub-interfaces
    /// <list type="bullet">
    ///     <item><see cref="IStringResolverLogger"/></item>
    ///     <item><see cref="IResourceResolverLogger"/></item>
    /// </list>
    /// </summary>
    public interface ILocalizationLogger
    {
    }

    /// <summary>
    /// Logger that logs string resolving of <see cref="IStringResolver"/>.
    /// </summary>
    public interface IStringResolverLogger : ILocalizationLogger, IObserver<LineString>
    {
    }

    /// <summary>
    /// Logger that logs resource resolving of <see cref="IResourceResolver"/>.
    /// </summary>
    public interface IResourceResolverLogger : ILocalizationLogger, IObserver<LineResourceBytes>, IObserver<LineResourceStream>
    {
    }
    #endregion Interface

    /// <summary></summary>
    public static partial class ILocalizationLoggerExtensions
    {
    }
}
