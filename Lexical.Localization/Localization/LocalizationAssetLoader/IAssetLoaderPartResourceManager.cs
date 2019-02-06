// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using System;
using System.Resources;

namespace Lexical.Localization
{
    /// <summary>
    /// <see cref="IAssetLoaderPart"/> that loads <see cref="ResourceManager"/>s.
    /// </summary>
    public interface IAssetLoaderPartResourceManager : IAssetLoaderPart
    {        
        /// <summary>
        /// This option determines which <see cref="ResourceSet"/> to use when loading <see cref="ResourceManager"/>.
        /// The type <see cref="ResourceSet"/> assignable type, or null.
        /// </summary>
        Type ResourceSetType { get; set; }

        /// <summary>
        /// Key name policy that is used within .resource/.resx file.
        /// </summary>
        IAssetKeyNamePolicy KeyNamePolicy { get; }
    }

    public static partial class AssetLoaderPartResourceManagerExtensions
    {
        /// <summary>
        /// Key name in <see cref="AssetLoaderPartOptions"/> to option that determines which <see cref="ResourceSet"/> to use when loading <see cref="ResourceManager"/>.
        /// The type <see cref="ResourceSet"/> assignable type, or null.
        /// </summary>
        public const string Key_ResourceSetType = "ResourceSetType";

        /// <summary>
        /// This option determines which <see cref="ResourceSet"/> to use when loading <see cref="ResourceManager"/>.
        /// The type <see cref="ResourceSet"/> assignable type, or null.
        /// </summary>
        /// <param name="resourceSetType">resource set, or null for default</param>
        /// <returns>this</returns>
        public static IAssetLoaderPartResourceManager SetResourceSetType(this IAssetLoaderPartResourceManager part, Type resourceSetType)
        {
            part.Options.Set<Type>(Key_ResourceSetType, resourceSetType);
            return part;
        }

    }
}
