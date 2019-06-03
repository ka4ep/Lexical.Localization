// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resolver;
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has <see cref="IFunctions"/> for usage of <see cref="IStringFormat"/>s.
    /// </summary>
    public interface ILineFunctions : ILine
    {
        /// <summary>
        /// (Optional) The assigned format provider.
        /// </summary>
        IFunctions Functions { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append string format functions.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="functions"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineFunctions Functions(this ILine key, IFunctions functions)
            => key.Append<ILineFunctions, IFunctions>(functions);

        /// <summary>
        /// Append string format functions.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="functions"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineHint Functions(this ILine key, string functions)
            => key.Append<ILineHint, string, string>("Functions", functions);

        /// <summary>
        /// Create string format functions.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="functions"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineFunctions Functions(this ILineFactory lineFactory, IFunctions functions)
            => lineFactory.Create<ILineFunctions, IFunctions>(null, functions);

        /// <summary>
        /// Create string format functions.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="functions"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineHint Functions(this ILineFactory lineFactory, string functions)
            => lineFactory.Create<ILineHint, string, string>(null, "Functions", functions);

        /// <summary>
        /// Search linked list and finds the effective (left-most) <see cref="ILineFunctions"/> key.
        /// 
        /// Returns parameter "Functions" value as <see cref="IFunctions"/>, if <paramref name="resolver"/> is provided.
        /// 
        /// If implements <see cref="ILineFunctions"/> returns the type. 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resolver">(optional) type resolver that resolves "IFunctions" parameter into type. Returns null, if could not resolve, exception if resolve fails</param>
        /// <returns>type info or null</returns>
        /// <exception cref="Exception">from <paramref name="resolver"/></exception>
        public static IFunctions FindFunctions(this ILine line, IResolver resolver = null)
        {
            IFunctions type = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineFunctions part && part.Functions != null) { type = part.Functions; continue; }
                if (resolver != null && l is ILineParameterEnumerable lineParameters)
                {
                    IFunctions tt = null;
                    foreach (ILineParameter parameter in lineParameters)
                    {
                        if (parameter.ParameterName == "Functions" && parameter.ParameterValue != null)
                        {
                            tt = resolver.Resolve<IFunctions>(parameter.ParameterValue);
                            if (tt != null) break;
                        }
                    }
                    if (tt != null) { type = tt; continue; }
                }
                if (resolver != null && l is ILineParameter lineParameter && lineParameter.ParameterName == "Functions" && lineParameter.ParameterValue != null)
                {
                    IFunctions t = resolver.Resolve<IFunctions>(lineParameter.ParameterValue);
                    if (t != null) type = t;
                }
            }
            return type;
        }

        /// <summary>
        /// Get effective (closest to root) type name.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>type name or null</returns>
        public static string FindFunctionsName(this ILine line)
        {
            string result = null;
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                        if (lineParameter.ParameterName == "Functions" && lineParameter.ParameterValue != null) { result = lineParameter.ParameterValue; break; }
                }
                else if (part is ILineParameter parameter && parameter.ParameterName == "Functions" && parameter.ParameterValue != null) result = parameter.ParameterValue;
                else if (part is ILineFunctions key && key.Functions != null) result = key.Functions.GetType().FullName;
            }
            return result;
        }


    }
}
