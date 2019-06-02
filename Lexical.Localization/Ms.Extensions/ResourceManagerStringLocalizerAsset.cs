// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.IO;

namespace Lexical.Localization
{
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// 
    /// </summary>
    public static class ResourceManagerStringLocalizerAsset
    {
        /// <summary>
        /// Creates <see cref="ResourceManagerStringLocalizer"/> and then adapts it into <see cref="IAsset"/>.
        /// 
        /// Search path = Assembly.RootNameSpace + [.resourcesPath] + filename + [.culture] + ".resx"
        /// </summary>
        /// <param name="asmRef">"Location" assembly name, short name or full name.</param>
        /// <param name="resourcePath">(optional) embedded resource folder, e.g. "Resources"</param>
        /// <param name="filename">"basename" name of the resx file, e.g. "localization", searches for "localization.xx.resx"</param>
        /// <param name="loggerFactory"></param>
        /// <exception cref="ArgumentNullException">assemblyRef is null</exception>
        /// <exception cref="FileNotFoundException">assemblyRef is not found.</exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded.</exception>
        /// <exception cref="BadImageFormatException">assemblyRef is not a valid assembly</exception>
        public static IAsset Create(string asmRef, string resourcePath, string filename, ILoggerFactory loggerFactory)
        {
            IOptions<LocalizationOptions> options = Options.Create(new LocalizationOptions { ResourcesPath = resourcePath });
            ResourceManagerStringLocalizerFactory factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);
            IStringLocalizer stringLocalizer = factory.Create(filename, asmRef);
            return new StringLocalizerAsset.Location(stringLocalizer, filename, asmRef, null);
        }

        /// <summary>
        /// Creates <see cref="ResourceManagerStringLocalizer"/> and then adapts it into <see cref="IAsset"/>.
        /// 
        /// Search path = Assembly.RootNameSpace + [.resourcesPath] + filename + [.culture] + ".resx"
        /// </summary>
        /// <param name="resourcePath">(optional) embedded resource folder, e.g. "Resources"</param>
        /// <param name="type">type searches for "typename.xx.resx"</param>
        /// <param name="loggerFactory"></param>
        /// <exception cref="ArgumentNullException">assemblyRef is null</exception>
        /// <exception cref="FileNotFoundException">assemblyRef is not found.</exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded.</exception>
        /// <exception cref="BadImageFormatException">assemblyRef is not a valid assembly</exception>
        public static IAsset Create(string resourcePath, Type type, ILoggerFactory loggerFactory)
        {
            IOptions<LocalizationOptions> options = Options.Create(new LocalizationOptions { ResourcesPath = resourcePath });
            ResourceManagerStringLocalizerFactory factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);
            IStringLocalizer stringLocalizer = factory.Create(type);
            return new StringLocalizerAsset.Type(stringLocalizer, type, null);
        }

        /// <summary>
        /// Creates <see cref="ResourceManagerStringLocalizerFactory"/> and then adapts it into <see cref="IAsset"/>.
        /// 
        /// This asset serves keys that have Assembly and Resource hints, or Type{T}() hint.
        /// <code>
        ///  var resx_key = root.Assembly("MyAssembly").Resource("Resources").Type("localization");
        ///  var key = resx_key["Success"];
        /// </code>
        /// 
        /// This asset is needed if <see cref="ILineRoot"/> is converted to <see cref="IStringLocalizerFactory" />.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IAsset CreateFactory(string resourcePath, ILoggerFactory loggerFactory)
        {
            IOptions<LocalizationOptions> options = Options.Create(new LocalizationOptions { ResourcesPath = resourcePath });
            ResourceManagerStringLocalizerFactory factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);
            return new StringLocalizerFactoryAsset(factory);
        }
    }
}
