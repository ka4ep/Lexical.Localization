// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Logger that logs string resolving of <see cref="IStringResolver"/>.
    /// </summary>
    public interface IStringResolverLogger : ILocalizationLogger, IObserver<LineString>
    {
    }

}

