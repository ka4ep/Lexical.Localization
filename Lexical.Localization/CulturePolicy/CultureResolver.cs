// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// Resolves culture name to <see cref="CultureInfo"/>.
    /// </summary>
    public class CultureResolver : IParameterResolver<CultureInfo>
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
        /// Parameter Name
        /// </summary>
        public string ParameterName => "Culture";

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
    }
}
