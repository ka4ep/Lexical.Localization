// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    #region IParameterPolicy
    /// <summary>
    /// Signals that the class can do conversions of <see cref="ILine"/> and <see cref="String"/>.
    /// 
    /// User of this interface should use extensions methods 
    /// <list type="bullet">
    /// <item><see cref="IParameterPolicyExtensions.Print(IParameterPolicy, ILine)"/></item>
    /// <item><see cref="IParameterPolicyExtensions.Parse(IParameterPolicy, string, ILine)"/></item>
    /// </list>
    /// 
    /// Class that implements to this interface should implement one or both of the following interfaces:
    ///  <see cref="IParameterPrinter"/>
    ///  <see cref="IParameterPattern"/>
    /// </summary>
    public interface IParameterPolicy
    {
    }
    #endregion IParameterPolicy

    #region IParameterPrinter
    /// <summary>
    /// Converts <see cref="ILine"/> to <see cref="String"/>.
    /// </summary>
    public interface IParameterPrinter : IParameterPolicy
    {
        /// <summary>
        /// Build path string from key.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>full name string</returns>
        string Print(ILine str);
    }
    #endregion IParameterPrinter

    #region IParameterParser
    /// <summary>
    /// Parses <see cref="String"/> into <see cref="ILine"/>.
    /// </summary>
    public interface IParameterParser : IParameterPolicy
    {
        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str">key as string</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="FormatException">If parse failed</exception>
        ILine Parse(string str, ILine rootKey = default);

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>true if parse was successful</returns>
        bool TryParse(string str, out ILine key, ILine rootKey = default);
    }
    #endregion IParameterParser

    /// <summary>
    /// Extension functions for <see cref="IParameterPolicy"/>.
    /// </summary>
    public static partial class IParameterPolicyExtensions
    {
        /// <summary>
        /// Build name for key. 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="key"></param>
        /// <returns>full name string or null</returns>
        public static string Print(this IParameterPolicy policy, ILine key)
        {
            if (policy is IParameterPrinter provider)
            {
                string name = provider.Print(key);
                if (name != null) return name;
            }
            return null;
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="str">key as string</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="FormatException">If parse failed</exception>
        /// <exception cref="ArgumentException">If <paramref name="policy"/> doesn't implement <see cref="IParameterParser"/>.</exception>
        public static ILine Parse(this IParameterPolicy policy, string str, ILine rootKey = default)
            => policy is IParameterParser parser ? parser.Parse(str, rootKey) : throw new ArgumentException($"Cannot parse strings to {nameof(ILine)} with {policy.GetType().FullName}. {policy} doesn't implement {nameof(IParameterParser)}.");

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>true if parse was successful (even through resulted key might be null)</returns>
        public static bool TryParse(this IParameterPolicy policy, string str, out ILine key, ILine rootKey = default)
        {
            if (policy is IParameterParser parser) return parser.TryParse(str, out key, rootKey);
            key = null;
            return false;
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="str"></param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>Key or null</returns>
        public static ILine TryParse(this IParameterPolicy policy, string str, ILine rootKey = default)
        {
            ILine key;
            if (policy is IParameterParser parser && parser.TryParse(str, out key, rootKey)) return key;
            return null;
        }

    }
}
