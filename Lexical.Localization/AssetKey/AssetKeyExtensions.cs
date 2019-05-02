// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           1.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    public static partial class AssetKeyExtensions_
    {
        /// <summary>
        /// Build name for key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="policy">(optional)</param>
        /// <param name="parametrizer">(optional) how to extract parameters from key. If not set uses the default implementation <see cref="AssetKeyParametrizer"/></param>
        /// <returns>full name string or null</returns>
        public static string BuildName(this ILinePart key, IAssetKeyNamePolicy policy)
        {
            if (policy == null) policy = AssetKeyNameProvider.Default;
            return policy.BuildName(key);
        }

        /// <summary>
        /// Build name for key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="policy">(optional)</param>
        /// <param name="parametrizer">(optional) how to extract parameters from key. If not set uses the default implementation <see cref="AssetKeyParametrizer"/></param>
        /// <returns>full name string or null</returns>
        public static string BuildName(this ILinePart key)
            => AssetKeyNameProvider.Default.BuildName(key);

    }
}
