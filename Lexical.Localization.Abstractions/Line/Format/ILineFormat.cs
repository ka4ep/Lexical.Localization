// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    #region ILineFormat
    /// <summary>
    /// Line format makes conversions between <see cref="ILine"/> and <see cref="String"/>.
    /// 
    /// Class that implements to this interface should implement one or both of the following interfaces:
    /// <list type="bullet">
    ///     <item><see cref="ILinePrinter"/></item>
    ///     <item><see cref="ILinePattern"/></item>
    /// </list>
    /// 
    /// The decision on what types are instantiated is a configuration decision of the implementing class.
    /// </summary>
    public interface ILineFormat
    {
    }
    #endregion ILineFormat

    #region ILinePrinter
    /// <summary>
    /// Converts <see cref="ILine"/> to string.
    /// </summary>
    public interface ILinePrinter : ILineFormat
    {
        /// <summary>
        /// Print <paramref name="line"/> as <see cref="String"/>.
        /// 
        /// The decision on what types are instantiated is a configuration decision of the implementing class.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>full name string</returns>
        string Print(ILine line);
    }
    #endregion ILinePrinter

    #region ILineParser
    /// <summary>
    /// Parses string into <see cref="ILine"/>.
    /// </summary>
    public interface ILineParser : ILineFormat
    {
        /// <summary>
        /// Parse string into <see cref="ILine"/>.
        /// 
        /// The decision on what types are instantiated is a configuration decision of the implementing class.
        /// </summary>
        /// <param name="str">key as string</param>
        /// <returns>Arguments that can be used for constructing or appending to a line</returns>
        /// <exception cref="LineException">If parse failed</exception>
        IEnumerable<ILineArguments> ParseArgs(string str);

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args">Arguments that can be used for constructing or appending to a line</param>
        /// <returns>true if parse was successful</returns>
        bool TryParseArgs(string str, out IEnumerable<ILineArguments> args);
    }

    /// <summary>
    /// Alternative parser interface where parts are appended right into previous line.
    /// </summary>
    public interface ILineAppendParser : ILineFormat
    {
        /// <summary>
        /// Parse string into <see cref="ILine"/>.
        /// 
        /// The decision on what types are instantiated is a configuration decision of the implementing class.
        /// </summary>
        /// <param name="str">key as string</param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="FormatException">If parse failed</exception>
        ILine Parse(string str, ILine prevPart = default, ILineFactory appender = default);

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>true if parse was successful</returns>
        bool TryParse(string str, out ILine key, ILine prevPart = default, ILineFactory appender = default);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ILineFormatParameterInfos : ILineFormat
    {
        /// <summary>
        /// Get line format's parameter infos.
        /// </summary>
        IParameterInfos ParameterInfos { get; set; }
    }
    #endregion ILineParser

    /// <summary>
    /// Extension functions for <see cref="ILineFormat"/>.
    /// </summary>
    public static partial class ILineFormatExtensions
    {
        /// <summary>
        /// Build name for key. 
        /// </summary>
        /// <param name="lineFormat"></param>
        /// <param name="key"></param>
        /// <returns>full name string or null</returns>
        public static string Print(this ILineFormat lineFormat, ILine key)
        {
            if (lineFormat is ILinePrinter provider)
            {
                string name = provider.Print(key);
                if (name != null) return name;
            }
            return null;
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="lineFormat"></param>
        /// <param name="str">key as string</param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="LineException">If parse failed</exception>
        /// <exception cref="LineException">If <paramref name="lineFormat"/> doesn't implement <see cref="ILineParser"/>.</exception>
        /// <exception cref="LineException">Error if appender is not available</exception>
        public static ILine Parse(this ILineFormat lineFormat, string str, ILine prevPart = default, ILineFactory appender = default)
        {
            if (lineFormat is ILineAppendParser appendParser)
            {
                return appendParser.Parse(str, prevPart, appender);
            }
            if (lineFormat is ILineParser parser)
            {
                if (appender == null) appender = prevPart.GetAppender();
                foreach (ILineArguments arg in parser.ParseArgs(str))
                    prevPart = appender.Create(prevPart, arg);
                return prevPart;
            }
            else throw new LineException(prevPart, $"Cannot parse strings to {nameof(ILine)} with {lineFormat.GetType().FullName}. {lineFormat} doesn't implement {nameof(ILineParser)}.");
        }


        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="lineFormat"></param>
        /// <param name="str"></param>
        /// <param name="result">key result or null if contained no content</param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>true if parse was successful (even through resulted key might be null)</returns>
        public static bool TryParse(this ILineFormat lineFormat, string str, out ILine result, ILine prevPart = default, ILineFactory appender = default)
        {
            if (lineFormat is ILineAppendParser appendParser)
            {
                return appendParser.TryParse(str, out result, prevPart, appender);
            }

            // Try get appender
            if (appender == null && !prevPart.TryGetAppender(out appender)) { result = null; return false; }
            // Try parse
            IEnumerable<ILineArguments> args;
            if (lineFormat is ILineParser parser && parser.TryParseArgs(str, out args))
            {
                // Append arguments
                foreach (ILineArguments arg in parser.ParseArgs(str))
                    if (!appender.TryCreate(prevPart, arg, out prevPart)) { result = null; return false; }
                result = prevPart;
                return true;
            }
            else { result = null; return false; }
        }

        /// <summary>
        /// Get parameter infos.
        /// </summary>
        /// <param name="lineFormat"></param>
        /// <returns>infos or null</returns>
        public static IParameterInfos GetParameterInfos(this ILineFormat lineFormat)
            => lineFormat is ILineFormatParameterInfos i ? i.ParameterInfos : null;

        /// <summary>
        /// Try get parameter infos.
        /// </summary>
        /// <param name="lineFormat"></param>
        /// <param name="infos"></param>
        /// <returns>true if returned infos</returns>
        public static bool TryGetParameterInfos(this ILineFormat lineFormat, out IParameterInfos infos)
        {
            if (lineFormat is ILineFormatParameterInfos i && i.ParameterInfos != null)
            {
                infos = i.ParameterInfos;
                return true;
            }
            infos = null;
            return false;
        }

        /// <summary>
        /// Get parameter info.
        /// </summary>
        /// <param name="lineFormat"></param>
        /// <param name="parameterName"></param>
        /// <returns>info or null</returns>
        public static IParameterInfo GetParameterInfo(this ILineFormat lineFormat, string parameterName)
        {
            IParameterInfo info;
            if (lineFormat is ILineFormatParameterInfos i && i.ParameterInfos != null && i.ParameterInfos.TryGetValue(parameterName, out info)) return info;
            return null;
        }

        /// <summary>
        /// Try get parameter info.
        /// </summary>
        /// <param name="lineFormat"></param>
        /// <param name="parameterName"></param>
        /// <param name="info"></param>
        /// <returns>true if returned info</returns>
        public static bool TryGetParameterInfo(this ILineFormat lineFormat, string parameterName, out IParameterInfo info)
        {
            if (lineFormat is ILineFormatParameterInfos i && i.ParameterInfos != null) return i.ParameterInfos.TryGetValue(parameterName, out info);
            info = null;
            return false;
        }
    }
}
