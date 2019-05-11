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
    /// Line that (may have) has been assigned with a "Assembly" parameter.
    /// 
    /// Assembly hint is used when loading assets from embedded resources.
    /// For instance, in a name pattern "[assembly.][resource.]{type.}{section.}{Key}".
    /// </summary>
    public interface ILineAssembly : ILine
    {
        /// <summary>
        /// Resolved Assembly, or null if not resolved.
        /// </summary>
        Assembly Assembly { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="ILineAssembly"/> section.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineAssembly Assembly(this ILine line, Assembly assembly)
            => line.GetAppender().Create<ILineAssembly, Assembly>(line, assembly);

        /// <summary>
        /// Add "Assembly" <see cref="ILineNonCanonicalKey"/> key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="assembly"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineKey Assembly(this ILine line, string assembly)
            => line.GetAppender().Create<ILineNonCanonicalKey, string, string>(line, "Assembly", assembly);

        /// <summary>
        /// Get the effective (closest to root) non-null <see cref="ILineAssembly"/> key or <see cref="ILineParameter"/> key with "Assembly".
        /// </summary>
        /// <param name="tail"></param>
        /// <returns>key or null</returns>
        public static ILine GetAssemblyKey(this ILine tail)
        {
            ILine result = null;
            for (ILine part = tail; tail != null; tail = tail.GetPreviousPart())
            {
                if (part is ILineAssembly asmKey && asmKey.Assembly != null) result = asmKey;
                else if (part is ILineParameter parameterKey && parameterKey.ParameterName == "Assembly" && parameterKey.ParameterValue != null) result = parameterKey;
            }
            return result;
        }

        /// <summary>
        /// Search linked list and finds the selected (left-most) <see cref="ILineAssembly"/> key.
        /// 
        /// If implements <see cref="ILineAssembly"/> returns the assembly. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>assembly info or null</returns>
        public static Assembly GetAssembly(this ILine line)
        {
            Assembly result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineAssembly key && key.Assembly != null) result = key.Assembly;
            }
            return result;
        }

        /// <summary>
        /// Get effective (closest to root) assembly value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>assembly name or null</returns>
        public static string GetAssemblyName(this ILine line)
        {
            string result = null;
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineAssembly typeKey && typeKey.Assembly != null) result = typeKey.Assembly.FullName;
                else if (part is ILineParameter parameter && parameter.ParameterName == "Assembly" && parameter.ParameterValue != null) result = parameter.ParameterValue;
                else if (part is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                        if (lineParameter.ParameterName == "Assembly" && lineParameter.ParameterValue != null) { result = lineParameter.ParameterValue; break; }
                }
            }
            return result;
        }

    }
}
