// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resource;
using Lexical.Localization.StringFormat;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Adapts <see cref="System.Resources.ResourceManager"/> (.resx) to <see cref="IAsset"/>.
    /// </summary>
    public class ResourceManagerAsset :
        IStringAsset,
        IResourceAsset
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly ResourceManager ResourceManager;

        /// <summary>
        /// 
        /// </summary>
        public readonly ILineFormat namePolicy;

        /// <summary>
        /// 
        /// </summary>
        public readonly IStringFormatParser ValueParser;

        /// <summary>
        /// Name policy where only "Section" and "Key" parameters are written out when creating key identifier to match against .resx.
        /// 
        /// Separator character is '.'.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public static readonly ILineFormat namepolicy_for_type_resourcemanager = new LineParameterPrinter().Rule("Section", true, ".").Rule("Key", true, ".").DefaultRule(false);

        /// <summary>
        /// Name policy where "Type", "Section" and "Key" parameters are written out when creating key identifier to match against .resx.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public static readonly ILineFormat namepolicy_for_location_resourcemanager = new LineParameterPrinter().Rule("Type", true, ".").Rule("Section", true, ".").Rule("Key", true, ".").DefaultRule(false);

        /// <summary>
        /// Name policy where "Resource", "Type", "Section" and "Key" parameters are written out when creating key identifier to match against .resx.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public static readonly ILineFormat namepolicy_for_root_resourcemanager = new LineParameterPrinter().Rule("Resource", true, ".").Rule("Type", true, ".").Rule("Section", true, ".").Rule("Key", true, ".").DefaultRule(false);

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
        /// <param name="lineFormat">policy that converts <see cref="ILine"/> into keys that correlate with keys in <paramref name="resourceManager"/>.</param>
        /// <param name="parser"></param>
        public ResourceManagerAsset(ResourceManager resourceManager, ILineFormat lineFormat, IStringFormatParser parser = default)
        {
            this.ResourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            this.namePolicy = lineFormat ?? throw new ArgumentNullException(nameof(lineFormat));
            this.ValueParser = parser ?? CSharpFormat.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ILine GetString(ILine key)
        {
            string id = namePolicy.Print(key);
            CultureInfo culture = null;
            key.TryGetCultureInfo(out culture);
            try
            {
                string value = culture == null ? ResourceManager.GetString(id) : ResourceManager.GetString(id, culture);
                if (value == null) return null;
                IString str = ValueParser.Parse(value);
                return key.String(str);
            }
            catch (Exception e)
            {
                throw new LocalizationException(id, e);
                //return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineResourceBytes GetResourceBytes(ILine key)
        {
            string id = namePolicy.Print(key);
            CultureInfo culture = null;
            key.TryGetCultureInfo(out culture);
            try
            {
                object obj = (culture == null ? ResourceManager.GetObject(id) : ResourceManager.GetObject(id, culture)) as byte[];
                if (obj == null) return new LineResourceBytes(key, LineStatus.ResolveFailedNoValue);
                return obj is byte[] data ? 
                    new LineResourceBytes(key, data, LineStatus.ResolveOkFromAsset) :
                    new LineResourceBytes(key, LineStatus.ResolveFailedNoValue | LineStatus.ResourceFailedConversionError);
            }
            catch (Exception e)
            {
                throw new LocalizationException(id, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineResourceStream GetResourceStream(ILine key)
        {
            string id = namePolicy.Print(key);
            CultureInfo culture = null;
            key.TryGetCultureInfo(out culture);
            try
            {
                object obj = (culture == null ? ResourceManager.GetObject(id) : ResourceManager.GetObject(id, culture)) as byte[];
                if (obj == null) return new LineResourceStream(key, LineStatus.ResolveFailedNoValue);
                return obj is byte[] data ?
                    new LineResourceStream(key, new MemoryStream(data), LineStatus.ResolveOkFromAsset) :
                    new LineResourceStream(key, LineStatus.ResolveFailedNoValue | LineStatus.ResourceFailedConversionError);
            }
            catch (Exception e)
            {
                throw new LocalizationException(id, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}()";
    }
}
