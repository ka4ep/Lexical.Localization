// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that the class gives localizations specific parameters, hints or capabilities.
    /// 
    /// Parameters:
    ///     <see cref="ILineCultureKey"/>
    ///     <see cref="ILineInlines"/>
    ///     <see cref="ILineFormatArgsPart"/>
    ///     
    /// Hints:
    ///     <see cref="ILocalizationKeyCulturePolicyAssigned"/>
    ///     
    /// Capabilities:
    ///     <see cref="ILocalizationKeyCultureAssignable"/>
    ///     <see cref="ILocalizationKeyCulturePolicyAssignable"/>
    ///     <see cref="ILocalizationKeyFormattable"/>
    ///     <see cref="ILineInlinesAssigned"/>
    ///     
    /// The ILocalizationKey ToString() must try to resolve the key.
    /// If resolve fails ToString returns the built name of the key.
    /// </summary>
    public interface ILocalizationKey : ILinePart
    {
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Find <see cref="IAsset"/> and get formulation string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILinePart)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formulation string</returns>
        /// <exception cref="AssetKeyException">If resolving failed or resolver was not found</exception>
        public static IFormulationString GetString(this ILinePart key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new AssetKeyException(key, "String resolver was not found.");
            IFormulationString str = asset.GetString(key);
            if (str == null) throw new AssetKeyException(key, "String was not found.");
            return str;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get formulation string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILinePart)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formulation string, or null if formulation string was not found, or if resolver was not found</returns>
        public static IFormulationString TryGetString(this ILinePart key)
            => key.FindAsset()?.GetString(key);



    }
}
