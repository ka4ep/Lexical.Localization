// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Signals that this link class is to be compared non-canonically. 
    /// 
    /// Non-canonical key links are compared so that their order of appearance doesn't matter.
    /// </summary>
    public interface IAssetKeyNonCanonicallyCompared : IAssetKey
    {
    }

    /// <summary>
    /// Signals that this link class is to be compared non-canonically.
    /// 
    /// The order of canonical links must match compared in the order of occurance.
    /// </summary>
    public interface IAssetKeyCanonicallyCompared : IAssetKey
    {
    }

    public static partial class AssetKeyExtensions
    {
    }
}
