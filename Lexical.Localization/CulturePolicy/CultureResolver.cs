// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Resolver;
using Lexical.Localization.StringFormat;
using System;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// Resolves culture name to <see cref="CultureInfo"/>.
    /// </summary>
    public class CultureResolver : IResolver<CultureInfo>, IParameterResolver
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        static readonly CultureResolver instance = new CultureResolver();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static CultureResolver Default => instance;

        /// <summary>
        /// Parameter names supported by this resolver.
        /// </summary>
        static string[] parameterNames = new string[] { "Culture" };

        /// <summary>
        /// Parameter Name
        /// </summary>
        public string[] ParameterNames => parameterNames;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public CultureInfo Resolve(string cultureName)
        {
            try
            {
                return CultureInfo.GetCultureInfo(cultureName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cultureName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryResolve(string cultureName, out CultureInfo result)
        {
            try
            {
                result = CultureInfo.GetCultureInfo(cultureName);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Resolve "Culture" parameter into arguments.
        /// </summary>
        /// <param name="previous">(optional)</param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="resolvedLineArgument"></param>
        /// <returns></returns>
        public bool TryResolveParameter(ILine previous, string parameterName, string parameterValue, out ILineArgument resolvedLineArgument)
        {
            if (parameterValue != null && parameterValue != "" && parameterName == "Culture")
            {
                CultureInfo cultureInfo;
                if (TryResolve(parameterValue, out cultureInfo))
                {
                    resolvedLineArgument = new LineArgument<ILineCulture, CultureInfo>(cultureInfo);
                    return true;
                }
            }

            resolvedLineArgument = default;
            return false;
        }
    }
}
