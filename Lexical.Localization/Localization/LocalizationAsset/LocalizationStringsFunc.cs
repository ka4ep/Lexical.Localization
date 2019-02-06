﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts Delegate to <see cref="ILocalizationStringProvider"/>.
    /// </summary>
    public class LocalizationStringsFunc : ILocalizationStringProvider
    {
        /// <summary>
        /// Function that resolve key to a language string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved string or null</returns>
        public readonly Func<IAssetKey, string> ResolverFunc;

        public LocalizationStringsFunc(Func<IAssetKey, string> resolverFunc)
        {
            this.ResolverFunc = resolverFunc;
        }

        public string GetString(IAssetKey key) => ResolverFunc(key);
        public override string ToString() => $"{GetType().Name}()";
    }

    public static partial class LocalizationAssetExtensions_
    {
        public static IAssetSource ToSource(this Func<IAssetKey, string> stringFunc)
            => new AssetSource(new LocalizationStringsFunc(stringFunc));
        public static ILocalizationStringProvider ToAsset(this Func<IAssetKey, string> stringFunc)
            => new LocalizationStringsFunc(stringFunc);

        /// <summary>
        /// Adapts <see cref="Delegate"/> to <see cref="ILocalizationStringProvider"/> and adds to composition.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resolverFunc"></param>
        /// <returns>composition</returns>
        public static IAssetComposition Add(this IAssetComposition composition, Func<IAssetKey, string> resolverFunc)
        {
            composition.Add(new LocalizationStringsFunc(resolverFunc));
            return composition;
        }
    }
}
