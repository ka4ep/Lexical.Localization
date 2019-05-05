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
    /// Consumers of this interface should use the extension method <see cref="ILinePartExtensions.Assembly(ILinePart, string)"/>.
    /// </summary>
    [Obsolete]
    public interface IAssetKeyAssemblyAssignable : ILinePart
    {
        /// <summary>
        /// Create a new key that has appended "Assembly" section.
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        ILineKeyAssembly Assembly(Assembly assembly);

        /// <summary>
        /// Create assembly section key.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        ILineKeyAssembly Assembly(string assembly);
    }

    /// <summary>
    /// Line's assigned assembly.
    /// </summary>
    public interface ILineAssembly : ILine
    {
        /// <summary>
        /// Resolved Assembly, or null if not resolved.
        /// </summary>
        Assembly Assembly { get; set; }
    }

    /// <summary>
    /// Key that (may have) has been assigned with a "Assembly" parameter.
    /// 
    /// Assembly hint is used when loading assets from embedded resources.
    /// For instance, in a name pattern "[assembly.][resource.]{type.}{section.}{Key}".
    /// </summary>
    public interface ILineKeyAssembly : ILineAssembly, ILineKeyNonCanonicallyCompared
    {
        // The inherited ParameterName property is Assembly.GetName().FullName
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Append <see cref="ILineKeyAssembly"/> section.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyAssemblyAssignable</exception>
        public static ILineKeyAssembly Assembly(this ILinePart line, Assembly assembly)
            => line.GetAppender().Append<ILineKeyAssembly, Assembly>(line, assembly);

        /// <summary>
        /// Add "Assembly" <see cref="ILineKeyNonCanonicallyCompared"/> key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyAssemblyAssignable</exception>
        public static ILineKey Assembly(this ILinePart line, string assembly)
            => line.GetAppender().Append<ILineKeyNonCanonicallyCompared, string, string>(line, "Assembly", assembly);

        /// <summary>
        /// Try to add <see cref="ILineKeyAssembly"/> section.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="assembly"></param>
        /// <returns>new key or null</returns>
        public static ILineKey TryAddAssembly(this ILinePart line, String assembly)
            => line.GetAppender()?.TryAppend<ILineKeyNonCanonicallyCompared, string, string>(line, "Assembly", assembly);

        /// <summary>
        /// Try to add <see cref="ILineKeyAssembly"/> section.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="assembly"></param>
        /// <returns>new key or null</returns>
        public static ILineKeyAssembly TryAddAssemblyKey(this ILinePart line, Assembly assembly)
            => line.GetAppender()?.TryAppend<ILineKeyAssembly, Assembly>(line, assembly);

        /// <summary>
        /// Get the effective (closest to root) non-null <see cref="ILineKeyAssembly"/> key or <see cref="ILineParameter"/> key with "Assembly".
        /// </summary>
        /// <param name="tail"></param>
        /// <returns>key or null</returns>
        public static ILinePart GetAssemblyKey(this ILinePart tail)
        {
            ILinePart result = null;
            for (ILinePart part = tail; tail != null; tail = tail.PreviousPart)
            {
                if (part is ILineKeyAssembly asmKey && asmKey.Assembly != null) result = asmKey;
                else if (part is ILineParameter parameterKey && parameterKey.ParameterName == "Assembly" && parameterKey.ParameterValue != null) result = parameterKey;
            }
            return result;
        }

        /// <summary>
        /// Search linked list and finds the selected (left-most) <see cref="ILineKeyAssembly"/> key.
        /// 
        /// If implements <see cref="ILineAssembly"/> returns the assembly. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>assembly info or null</returns>
        public static Assembly GetAssembly(this ILine line)
        {
            if (line is ILineAssembly lineAssembly && lineAssembly.Assembly != null) return lineAssembly.Assembly;

            if (line is ILinePart part)
            {
                Assembly assembly = null;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineKeyAssembly assemblyKey && assemblyKey.Assembly != null) assembly = assemblyKey.Assembly;
                if (assembly != null) return assembly;
            }

            return null;
        }

        /// <summary>
        /// Get effective (closest root) assembly value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>assembly name or null</returns>
        public static string GetAssemblyName(this ILine line)
        {
            if (line is ILineAssembly lineAssembly && lineAssembly.Assembly != null) return lineAssembly.Assembly.GetName().FullName;

            if (line is ILineParameters lineParameters)
            {
                var keys = lineParameters.Parameters;
                if (keys != null)
                    foreach (var kv in keys)
                        if (kv.Key == "Assembly" && kv.Value != null) return kv.Value;
            }

            if (line is ILinePart part)
            {
                string result = null;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                {
                    if (p is ILineKeyAssembly assemblyKey && assemblyKey.Assembly != null) result = assemblyKey.Assembly.GetName().FullName;
                    else if (p is ILineParameter parameterKey && parameterKey.ParameterName == "Assembly" && parameterKey.ParameterValue != null) result = parameterKey.ParameterValue;
                }
                if (result != null) return result;
            }

            return null;
        }

    }
}
