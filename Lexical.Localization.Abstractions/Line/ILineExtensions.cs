// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------

namespace Lexical.Localization
{
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append "Key" non-canonical key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Key(this ILine line, string key)
            => line.Append<ILineCanonicalKey, string, string>("Key", key);

        /// <summary>
        /// Append "Section" key.
        /// 
        /// Section is a key that points to a folder is used when loading assets from files, embedded resources, and withint language string dictionaries.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey Section(this ILine key, string location)
            => key.Append<ILineCanonicalKey, string, string>("Section", location);

        /// <summary>
        /// Append "Location" key.
        /// 
        /// Location is a key that points to folder where asset is to be loaded.
        /// For example adding "Icons" location section, would mean that when key is matched to file assets, only "Icons" folder is used.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey Location(this ILine key, string location)
            => key.Append<ILineCanonicalKey, string, string>("Location", location);

        /// <summary>
        /// Append "BaseName" key.
        /// 
        /// BaseName means a part of a path to assembly's embedded resource.
        /// For instance, resource hint matches in name pattern "[Assembly.][BaseName.]{Type.}{Section.}{Key}".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey BaseName(this ILine key, string resource)
            => key.Append<ILineCanonicalKey, string, string>("BaseName", resource);

        /// <summary>
        /// Find <see cref="IAsset"/> and get formulation string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formulation string</returns>
        /// <exception cref="LineException">If resolving failed or resolver was not found</exception>
        public static IFormulationString GetString(this ILine key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new LineException(key, "String resolver was not found.");
            IFormulationString str = asset.GetString(key);
            if (str == null) throw new LineException(key, "String was not found.");
            return str;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get formulation string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formulation string, or null if formulation string was not found, or if resolver was not found</returns>
        public static IFormulationString TryGetString(this ILine key)
            => key.FindAsset()?.GetString(key);

        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        /// <exception cref="LineException">If resolving failed or resolver was not found</exception>
        public static byte[] GetResource(this ILine key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new LineException(key, "String resolver was not found.");
            byte[] data = asset.GetResource(key);
            if (data == null) throw new LineException(key, $"String {key} was not found.");
            return data;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILine)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        public static byte[] TryGetResource(this ILine key)
            => key.FindAsset()?.GetResource(key);
    }

}
