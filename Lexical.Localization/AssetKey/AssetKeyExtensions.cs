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
        public static string BuildName(this IAssetKey key, IAssetKeyNamePolicy policy, IAssetKeyParametrizer parametrizer = default)
        {
            if (policy == null) policy = AssetKeyNameProvider.Default;
            if (parametrizer == null) parametrizer = AssetKeyParametrizer.Singleton;
            return policy.BuildName(key, parametrizer);
        }

        /// <summary>
        /// Build name for key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="policy">(optional)</param>
        /// <param name="parametrizer">(optional) how to extract parameters from key. If not set uses the default implementation <see cref="AssetKeyParametrizer"/></param>
        /// <returns>full name string or null</returns>
        public static string BuildName(this IAssetKey key)
            => AssetKeyNameProvider.Default.BuildName(key, AssetKeyParametrizer.Singleton);

    }
}
