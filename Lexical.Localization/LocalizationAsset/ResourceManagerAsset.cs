// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts <see cref="System.Resources.ResourceManager"/> (.resx) to <see cref="IAsset"/>.
    /// </summary>
    public class ResourceManagerAsset : 
        ILocalizationStringProvider,
        IAssetResourceProvider
    {
        public readonly ResourceManager ResourceManager;
        public readonly IParameterPolicy namePolicy;
        public readonly ILocalizationStringFormatParser ValueParser;

        /// <summary>
        /// Name policy where only "Section" and "Key" parameters are written out when creating key identifier to match against .resx.
        /// 
        /// Separator character is '.'.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public  static readonly IParameterPolicy namepolicy_for_type_resourcemanager = new KeyPrinter().Rule("Section", true, ".").Rule("Key", true, ".").DefaultRule(false);

        /// <summary>
        /// Name policy where "Type", "Section" and "Key" parameters are written out when creating key identifier to match against .resx.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public static readonly IParameterPolicy namepolicy_for_location_resourcemanager = new KeyPrinter().Rule("Type", true, ".").Rule("Section", true, ".").Rule("Key", true, ".").DefaultRule(false);

        /// <summary>
        /// Name policy where "Resource", "Type", "Section" and "Key" parameters are written out when creating key identifier to match against .resx.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public static readonly IParameterPolicy namepolicy_for_root_resourcemanager = new KeyPrinter().Rule("Resource", true, ".").Rule("Type", true, ".").Rule("Section", true, ".").Rule("Key", true, ".").DefaultRule(false);

        /// <summary>
        /// Create resource manager that is assigned to a specific type.
        /// 
        /// In these .resx language string files, the key of each line uses name policy "{section.}{Key}".
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ResourceManagerAsset CreateType(ResourceManager resourceManager, Type type)
            => new ResourceManagerAsset(resourceManager, namepolicy_for_type_resourcemanager);

        /// <summary>
        /// Create resource manager that is assigned to a specific type.
        /// 
        /// In these .resx language string files, the key of each line uses name policy "{section.}{Key}".
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ResourceManagerAsset CreateType(Type type)
            => new ResourceManagerAsset(new ResourceManager(type), namepolicy_for_type_resourcemanager);

        /// <summary>
        /// Create resource manager that is assigned to a specific embedded resource location.
        /// 
        /// In these .resx language string files, the key of each line uses name policy "{section.}{Key}".
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="assembly"></param>
        /// <param name="baseName">embedded resources location within assembly, corresponds to "Resource" parameter in <see cref="ILine"/>.</param>
        /// <returns></returns>
        public static ResourceManagerAsset CreateLocation(ResourceManager resourceManager, string baseName, Assembly assembly)
            => new ResourceManagerAsset(resourceManager, namepolicy_for_location_resourcemanager);

        /// <summary>
        /// Create resource manager that is assigned to a specific embedded resource location.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="baseName">embedded resources location within assembly, corresponds to "Resource" parameter in <see cref="ILine"/>.</param>
        /// <returns></returns>
        public static ResourceManagerAsset CreateLocation(string baseName, Assembly assembly)
            => new ResourceManagerAsset(new ResourceManager(baseName, assembly), namepolicy_for_location_resourcemanager);

        /// <summary>
        /// Create resource manager asset that is assigned to a root of an assembly.
        /// 
        /// In these .resx language string files, the key of each line uses name policy "{resource.}{type.}{section_0.}{section_1.}{section_2.}{section_3.}{.key_0}{.key_1}{.key_2}{.key_n}".
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ResourceManagerAsset CreateRoot(Assembly assembly)
            => new ResourceManagerAsset(new ResourceManager("", assembly), namepolicy_for_root_resourcemanager);

        /// <summary>
        /// Construct an <see cref="IAsset"/> that reads strings and resources from <see cref="ResourceManager"/>.
        /// 
        /// </summary>
        /// <param name="resourceManager">source of language strings and resource files</param>
        /// <param name="namePolicy">policy that converts <see cref="ILine"/> into keys that correlate with keys in <paramref name="resourceManager"/>.</param>
        /// <param name="parser"></param>
        public ResourceManagerAsset(ResourceManager resourceManager, IParameterPolicy namePolicy, ILocalizationStringFormatParser parser = default)
        {
            this.ResourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            this.namePolicy = namePolicy ?? throw new ArgumentNullException(nameof(namePolicy));
            this.ValueParser = parser ?? LexicalStringFormat.Instance;
        }

        public IFormulationString GetString(ILine key)
        {
            string id = namePolicy.Print(key);
            CultureInfo culture = key.GetCultureInfo();
            try
            {
                string value = culture == null ? ResourceManager.GetString(id) : ResourceManager.GetString(id, culture);
                return ValueParser.Parse(value);
            }
            catch (Exception e)
            {
                throw new LocalizationException(id, e);
                //return null;
            }
        }

        public byte[] GetResource(ILine key)
        {
            string id = namePolicy.Print(key);
            CultureInfo culture = key.GetCultureInfo();
            try
            {
                object obj = (culture == null ? ResourceManager.GetObject(id) : ResourceManager.GetObject(id, culture)) as byte[];
                if (obj == null) return null;
                return obj is byte[] data ? data : throw new LocalizationException($"Key={id}, Expected byte[], got {obj.GetType().FullName}");
            } catch(Exception e)
            {
                throw new LocalizationException(id, e);
            }
        }

        public Stream OpenStream(ILine key)
        {
            string id = namePolicy.Print(key);
            CultureInfo culture = key.GetCultureInfo();
            try
            {
                object obj = (culture == null ? ResourceManager.GetObject(id) : ResourceManager.GetObject(id, culture)) as byte[];
                if (obj == null) return null;
                return obj is byte[] data ? new MemoryStream(data) : throw new LocalizationException($"Key={id}, Expected byte[], got {obj.GetType().FullName}");
            }
            catch (Exception e)
            {
                throw new LocalizationException(id, e);
            }
        }

        public override string ToString()
            => $"{GetType().Name}()";
    }
}
