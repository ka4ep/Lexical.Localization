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
    /// Key has capability of "Assembly" parameter assignment.
    /// 
    /// Assembly is a hint that is used when assets are loaded from embedded rsources.
    /// For instance, assembly hint matches in a name pattern such as "[Assembly.][Resource.]{Type.}{Section.}{Key}".
    /// 
    /// Consumers of this interface should use the extension method <see cref="LinePartExtensions.Assembly(ILinePart, string)"/>.
    /// </summary>
    public interface IAssetKeyAssemblyAssignable : ILinePart
    {
        /// <summary>
        /// Create a new key that has appended "Assembly" section.
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        IAssetKeyAssemblyAssigned Assembly(Assembly assembly);

        /// <summary>
        /// Create assembly section key.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        IAssetKeyAssemblyAssigned Assembly(string assembly);
    }

    /// <summary>
    /// Key that (may have) has been assigned with a "Assembly" parameter.
    /// 
    /// Assembly hint is used when loading assets from embedded rsources.
    /// For instance, in a name pattern "[assembly.][resource.]{type.}{section.}{Key}".
    /// </summary>
    public interface IAssetKeyAssemblyAssigned : IAssetKeySection
    {
        /// <summary>
        /// Resolved Assembly, or null if not resolved.
        /// </summary>
        Assembly Assembly { get; }

        // The inherited Name property is either Assembly.FullName or Assembly.GetName().Name.
    }

    public static partial class LinePartExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyAssemblyAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement IAssetKeyAssemblyAssignable</exception>
        public static IAssetKeyAssemblyAssigned Assembly(this ILinePart key, Assembly assembly)
        {
            if (key is IAssetKeyAssemblyAssignable casted) return casted.Assembly(assembly);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyAssemblyAssignable)}.");
        }

        /// <summary>
        /// Add <see cref="IAssetKeyAssemblyAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement IAssetKeyAssemblyAssignable</exception>
        public static IAssetKeyAssemblyAssigned Assembly(this ILinePart key, string assembly)
        {
            if (key is IAssetKeyAssemblyAssignable casted) return casted.Assembly(assembly);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyAssemblyAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyAssemblyAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyAssemblyAssigned TryAddAssembly(this ILinePart key, String assembly)
        {
            if (key is IAssetKeyAssemblyAssignable casted) return casted.Assembly(assembly);
            return null;
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyAssemblyAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assembly"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyAssemblyAssigned TryAddAssemblyKey(this ILinePart key, Assembly assembly)
        {
            if (key is IAssetKeyAssemblyAssignable casted) return casted.Assembly(assembly);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyAssemblyAssigned"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyAssemblyAssigned FindAssemblyKey(this ILinePart key)
        {
            while (key != null)
            {
                if (key is IAssetKeyAssemblyAssigned asmKey && !string.IsNullOrEmpty(key.GetParameterValue())) return asmKey;
                key = key.PreviousPart;
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyAssemblyAssigned"/> that has a resolved Assembky.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static Assembly FindAssembly(this ILinePart key)
        {
            while (key != null)
            {
                if (key is IAssetKeyAssemblyAssigned casted && casted.Assembly != null) return casted.Assembly;
                key = key.PreviousPart;
            }
            return null;
        }


        /// <summary>
        /// Searches key for either 
        ///    <see cref="IAssetKeyTypeAssigned"/> with type
        ///    <see cref="IAssetKeyAssemblyAssigned"/> and <see cref="IAssetKeyResourceAssigned"/> with string, not type.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="basename"></param>
        /// <param name="location"></param>
        /// <returns>
        ///     0 found nothing
        ///     1 found type
        ///     2 found basename + location</returns>
        public static int FindResourceInfos(this ILinePart key, out Type type, out string basename, out string location)
        {
            IAssetKeyAssemblyAssigned asmSection;
            IAssetKeyResourceAssigned resSection;
            IAssetKeyTypeAssigned typeSection;
            int x = key.FindResourceKeys(out asmSection, out resSection, out typeSection);
            if (x == 1) { type = typeSection.Type; basename = null; location = null; }
            if (x == 2) { type = null; basename = typeSection.GetParameterValue(); location = asmSection.GetParameterValue(); }
            type = null; basename = null; location = null;
            return 0;
        }
        /// <summary>
        /// Searches key for either 
        ///    <see cref="IAssetKeyTypeAssigned"/> with type
        ///    <see cref="IAssetKeyAssemblyAssigned"/> and <see cref="IAssetKeyResourceAssigned"/> with string, not type.
        /// </summary>
        /// <returns>
        ///     0 found nothing
        ///     1 found typeSection
        ///     2 found asmSection + typeSection</returns>
        public static int FindResourceKeys(this ILinePart key, out IAssetKeyAssemblyAssigned asmSection, out IAssetKeyResourceAssigned resSection, out IAssetKeyTypeAssigned typeSection)
        {
            IAssetKeyAssemblyAssigned _asmSection = null;
            IAssetKeyResourceAssigned _resSection = null;
            IAssetKeyTypeAssigned _typeSection = null;
            for (ILinePart k = key; k != null; k = k.PreviousPart)
            {
                if (k is IAssetKeyAssemblyAssigned __asmSection) _asmSection = __asmSection;
                else if (k is IAssetKeyResourceAssigned __resSection) _resSection = __resSection;
                else if (k is IAssetKeyTypeAssigned __typeSection) _typeSection = __typeSection;
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
