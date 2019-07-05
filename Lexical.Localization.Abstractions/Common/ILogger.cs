// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.Common
{
    #region ILogger
    /// <summary>
    /// Localization logger.
    /// 
    /// See sub-interfaces
    /// <list type="bullet">
    ///     <item><see cref="Lexical.Localization.StringFormat.IStringResolverLogger"/></item>
    ///     <item><see cref="Lexical.Localization.Resource.IResourceResolverLogger"/></item>
    /// </list>
    /// </summary>
    public interface ILogger
    {
    }
    #endregion ILogger
}

namespace Lexical.Localization.StringFormat
{
    #region IStringResolverLogger
    /// <summary>
    /// Logger that logs string resolving of <see cref="IStringResolver"/>.
    /// </summary>
    public interface IStringResolverLogger : Lexical.Localization.Common.ILogger, IObserver<LineString>
    {
    }
    #endregion IStringResolverLogger
}

namespace Lexical.Localization.Resource
{
    #region IResourceResolverLogger
    /// <summary>
    /// Logger that logs resource resolving of <see cref="IResourceResolver"/>.
    /// </summary>
    public interface IResourceResolverLogger : Lexical.Localization.Common.ILogger, IObserver<LineResourceBytes>, IObserver<LineResourceStream>
    {
    }
    #endregion IResourceResolverLogger
}
