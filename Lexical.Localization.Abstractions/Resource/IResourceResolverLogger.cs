// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Resource;
using System;

namespace Lexical.Localization.Resource
{
    /// <summary>
    /// Logger that logs resource resolving of <see cref="IResourceResolver"/>.
    /// </summary>
    public interface IResourceResolverLogger : ILocalizationLogger, IObserver<LineResourceBytes>, IObserver<LineResourceStream>
    {
    }
}
