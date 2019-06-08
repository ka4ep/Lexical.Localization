// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using Lexical.Localization.Resolver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization.Resolver
{
    /// <summary>
    /// Resolve source
    /// </summary>
    public enum ResolveSource
    {
        /// <summary>Resolve from <see cref="ILineAsset"/></summary>
        Asset,
        /// <summary>Resolve from <see cref="ILineInlines"/></summary>
        Inline,
        /// <summary>Resolve the value from the <see cref="ILine"/> itself</summary>
        Line
    };

    /// <summary>
    /// Resolver sequence.
    /// </summary>
    public class ResolverSequence
    {
        /// <summary>
        /// Default resolver sequence.
        /// </summary>
        public static readonly ResolveSource[] Default = new ResolveSource[] { ResolveSource.Asset, ResolveSource.Inline, ResolveSource.Line };
    }
}
