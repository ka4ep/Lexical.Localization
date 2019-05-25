﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Localization string value.
    /// </summary>
    public interface ILineValue : ILine
    {
        /// <summary>
        /// Localization string value.
        /// </summary>
        IFormatString Value { get; set; }
    }
    
    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="ILineValue"/> part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineValue Value(this ILine part, IFormatString value)
            => part.Append<ILineValue, IFormatString>(value);

        /// <summary>
        /// Append "Value" parameter.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILine Value(this ILine part, string value)
            => part.Append<ILineHint, string, string>("Value", value);

        /// <summary>
        /// Get the <see cref="IFormatString"/> of a <see cref="ILineValue"/>.
        /// 
        /// If parameter "Value" exists and <paramref name="resolver"/> is provided then value is resolved using
        /// the default string format or the format that can be found.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resolver"></param>
        /// <returns>value</returns>
        public static IFormatString GetValue(this ILine line, IResolver<IStringFormat> resolver = null)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineValue valuePart && valuePart.Value != null) return valuePart.Value;
                if (resolver != null && part is ILineParameterEnumerable lineParameters)
                {
                    foreach(ILineParameter parameter in lineParameters)
                    {
                        if (parameter.ParameterName == "Value" && parameter.ParameterValue != null)
                        {
                            IStringFormat stringFormat = line.FindStringFormat(resolver);
                            return stringFormat.Parse(parameter.ParameterValue);
                        }
                    }
                }
                if (resolver != null && part is ILineParameter lineParameter && lineParameter.ParameterName == "Value" && lineParameter.ParameterValue != null)
                {
                    IStringFormat stringFormat = line.FindStringFormat(resolver);
                    return stringFormat.Parse(lineParameter.ParameterValue);
                }
            }
            return new StatusFormatString(null, LineStatus.FormatFailedNull);
        }

    }

}

