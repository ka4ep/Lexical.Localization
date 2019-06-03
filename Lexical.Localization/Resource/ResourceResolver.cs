// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Resource
{
    /// <summary>
    /// Resolves keys to resources by applying contextual information such as culture.
    /// </summary>
    public class ResourceResolver : IResourceResolver
    {
        private static ResourceResolver instance = new ResourceResolver();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ResourceResolver Default => instance;

        /// <summary>
        /// Resolvers
        /// </summary>
        public readonly IResolver Resolvers;

        /// <summary>
        /// Resolve localization resource using the active culture. Uses the following algorithm:
        ///   1. If key has a selected culture, try that
        ///      a) from Asset
        ///   2. If key has <see cref="ICulturePolicy"/>, iterate the cultures.
        ///      a) Try asset
        ///   3. Try to read value for key from asset as is
        ///   4. Return null
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public LineResourceStream ResolveStream(ILine line)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolve localization resource using the active culture. Uses the following algorithm:
        ///   1. If key has a selected culture, try that
        ///      a) from Asset
        ///   2. If key has <see cref="ICulturePolicy"/>, iterate the cultures.
        ///      a) Try asset
        ///   3. Try to read value for key from asset as is
        ///   4. Return null
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public LineResourceBytes ResolveBytes(ILine line)
        {
            /*
                         // Arrange
            IAsset asset = key.FindAsset();
            byte[] result = null;

            // 1. Try selected culture
            CultureInfo selectedCulture = key.GetCultureInfo();
            if (selectedCulture != null)
            {
                // 1a. Try from asset
                result = asset.GetResource(key);
                if (result != null) return result;
            }

            // 2. Try culture policy
            IEnumerable<CultureInfo> cultures = key.FindCulturePolicy()?.Cultures;
            if (cultures != null)
            {
                foreach (var culture in cultures)
                {
                    // This was already tried above
                    if (culture == selectedCulture) continue;

                    // 2a. Try from asset
                    if (asset != null)
                    {
                        ILine cultured = key.Culture(culture);
                        if (cultured != null)
                        {
                            result = asset.GetResource(cultured);
                            if (result != null) return result;
                        }
                    }
                }
            }

            // 3. Try key as is
            if (asset != null)
            {
                result = asset.GetResource(key);
                if (result != null) return result;
            }

            // Not found
            return null;
             */
            throw new NotImplementedException();
        }
    }
}
