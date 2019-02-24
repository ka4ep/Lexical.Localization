// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Reflection;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "assembly" parameter assignment.
    /// 
    /// Assembly is a hint that is used when assets are loaded from embedded rsources.
    /// For instance, assembly hint matches in a name pattern such as "[assembly.][resource.]{type.}{section.}{key}".
    /// 
    /// Consumers of this interface should use the extension method <see cref="AssetKeyExtensions.AssemblySection(IAssetKey, string)"/>.
    /// </summary>
    public interface IAssetKeyAssemblySectionAssignable : IAssetKey
    {
        /// <summary>
        /// Create assembly section.
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        IAssetKeyAssemblySection AssemblySection(Assembly assembly);

        /// <summary>
        /// Create assembly section key.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        IAssetKeyAssemblySection AssemblySection(string assembly);
    }

    /// <summary>
    /// Key that (may have) has been assigned with a "assembly" parameter.
    /// 
    /// Assembly hint is used when loading assets from embedded rsources.
    /// For instance, in a name pattern "[assembly.][resource.]{type.}{section.}{key}".
    /// </summary>
    public interface IAssetKeyAssemblySection : IAssetKeySection
    {
        /// <summary>
        /// Resolved Assembly, or null if not resolved.
        /// </summary>
        Assembly Assembly { get; }

        // The inherited Name property is either Assembly.FullName or Assembly.GetName().Name.
    }

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyAssemblySection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement IAssetKeyAssemblySectionAssignable</exception>
        public static IAssetKeyAssemblySection AssemblySection(this IAssetKey key, Assembly assembly)
        {
            if (key is IAssetKeyAssemblySectionAssignable casted) return casted.AssemblySection(assembly);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyAssemblySectionAssignable)}.");
        }

        /// <summary>
        /// Add <see cref="IAssetKeyAssemblySection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement IAssetKeyAssemblySectionAssignable</exception>
        public static IAssetKeyAssemblySection AssemblySection(this IAssetKey key, string assembly)
        {
            if (key is IAssetKeyAssemblySectionAssignable casted) return casted.AssemblySection(assembly);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyAssemblySectionAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyAssemblySection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyAssemblySection TryAddAssemblySection(this IAssetKey key, String assembly)
        {
            if (key is IAssetKeyAssemblySectionAssignable casted) return casted.AssemblySection(assembly);
            return null;
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyAssemblySection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyAssemblySection TryAddAssemblySection(this IAssetKey key, Assembly assembly)
        {
            if (key is IAssetKeyAssemblySectionAssignable casted) return casted.AssemblySection(assembly);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyAssemblySection"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyAssemblySection FindAssemblySection(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyAssemblySection asmKey && !string.IsNullOrEmpty(key.Name)) return asmKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyAssemblySection"/> that has a resolved Assembky.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static Assembly FindAssembly(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyAssemblySection casted && casted.Assembly != null) return casted.Assembly;
                key = key.GetPreviousKey();
            }
            return null;
        }


        /// <summary>
        /// Searches key for either 
        ///    <see cref="IAssetKeyTypeSection"/> with type
        ///    <see cref="IAssetKeyAssemblySection"/> and <see cref="IAssetKeyResourceSection"/> with string, not type.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="basename"></param>
        /// <param name="location"></param>
        /// <returns>
        ///     0 found nothing
        ///     1 found type
        ///     2 found basename + location</returns>
        public static int FindResourceInfos(this IAssetKey key, out Type type, out string basename, out string location)
        {
            IAssetKeyAssemblySection asmSection;
            IAssetKeyResourceSection resSection;
            IAssetKeyTypeSection typeSection;
            int x = key.FindResourceSections(out asmSection, out resSection, out typeSection);
            if (x == 1) { type = typeSection.Type; basename = null; location = null; }
            if (x == 2) { type = null; basename = typeSection.Name; location = asmSection.Name; }
            type = null; basename = null; location = null;
            return 0;
        }
        /// <summary>
        /// Searches key for either 
        ///    <see cref="IAssetKeyTypeSection"/> with type
        ///    <see cref="IAssetKeyAssemblySection"/> and <see cref="IAssetKeyResourceSection"/> with string, not type.
        /// </summary>
        /// <returns>
        ///     0 found nothing
        ///     1 found typeSection
        ///     2 found asmSection + typeSection</returns>
        public static int FindResourceSections(this IAssetKey key, out IAssetKeyAssemblySection asmSection, out IAssetKeyResourceSection resSection, out IAssetKeyTypeSection typeSection)
        {
            IAssetKeyAssemblySection _asmSection = null;
            IAssetKeyResourceSection _resSection = null;
            IAssetKeyTypeSection _typeSection = null;
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
            {
                if (k is IAssetKeyAssemblySection __asmSection) _asmSection = __asmSection;
                else if (k is IAssetKeyResourceSection __resSection) _resSection = __resSection;
                else if (k is IAssetKeyTypeSection __typeSection) _typeSection = __typeSection;
            }

            if (_asmSection != null && _resSection != null)
            {
                asmSection = _asmSection;
                resSection = _resSection;
                typeSection = null;
                return 2;
            }
            if (_typeSection != null && _typeSection.Type != null)
            {
                asmSection = null;
                resSection = null;
                typeSection = _typeSection;
                return 1;
            }

            asmSection = null;
            resSection = null;
            typeSection = null;
            return 0;
        }

    }
}
