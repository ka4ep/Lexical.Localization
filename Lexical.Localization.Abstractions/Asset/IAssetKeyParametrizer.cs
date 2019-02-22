// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Extracts parameters from an object.
    /// </summary>
    public interface IAssetKeyParametrizer
    {
        /// <summary>
        /// Break key into parts.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>enumerable from root to tail, or null if key type was not handled</returns>
        IEnumerable<object> Break(object key);

        /// <summary>
        /// Visit parts of a key from root to tail. Same as break, but with stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="visitor"></param>
        /// <param name="data"></param>
        void VisitParts<T>(object key, ParameterPartVisitor<T> visitor, ref T data);

        /// <summary>
        /// Get previous part
        /// </summary>
        /// <param name="part"></param>
        /// <returns>previous part or null</returns>
        object GetPreviousPart(object part);

        /// <summary>
        /// List all the possible parameters of the part. Returned in case sensitive culture invariant alphabetical ascending order.
        /// </summary>
        /// <returns>parameters or null if part type was not handled</returns>
        string[] GetPartParameters(object part);

        /// <summary>
        /// Read key part as a property.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameter"></param>
        /// <returns>parameter value or null or null of part type was not handled</returns>
        string GetPartValue(object part, string parameter);

        /// <summary>
        /// Try to append a part to an object.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns>new part or null if part type was not handled or failed</returns>
        object TryCreatePart(object key, string parameterName, string parameterValue);

        /// <summary>
        /// Tests if parameter is canonical
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        bool IsCanonical(object part, string parameterName);

        /// <summary>
        /// Tests if parameter is non-canonical
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        bool IsNonCanonical(object part, string parameterName);
    }

    /// <summary>
    /// Function that visits each part of an object, in canonical order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="part"></param>
    /// <param name="data"></param>
    public delegate void ParameterPartVisitor<T>(object part, ref T data);

    public static partial class AssetKeyParametrizerExtensions
    {
        /// <summary>
        /// Read first part that has a value for a specific parameter.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="obj"></param>
        /// <param name="parameter"></param>
        /// <returns>value or null</returns>
        public static string ReadParameter(this IAssetKeyParametrizer reader, object obj, string parameter)
        {
            IEnumerable<object> parts = reader.Break(obj);
            if (parts != null)
                foreach (object part in parts)
                {
                    string value = reader.GetPartValue(part, parameter);
                    if (value != null) return value;
                }
            return null;
        }

        /// <summary>
        /// Read all parameters as key-value pairs.
        /// </summary>
        /// <param name="parameterizer"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetAllParameters(this IAssetKeyParametrizer parameterizer, object obj)
        {
            IEnumerable<object> parts = parameterizer.Break(obj);
            if (parts!=null)
            foreach (object part in parts)
            {
                foreach(string parameterName in parameterizer.GetPartParameters(part))
                {
                    string value = parameterizer.GetPartValue(part, parameterName);
                    if (value != null) yield return new KeyValuePair<string, string>(parameterName, value);
                }
            }
        }

        /// <summary>
        /// Append a new part to object.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        /// <exception cref="AssetException">If failed to create part</exception>
        public static object CreatePart(this IAssetKeyParametrizer reader, object key, string parameterName, string parameterValue)
            => reader.TryCreatePart(key, parameterName, parameterValue) ??
                throw new AssetException($"Could not append (ParameterName=\"{parameterName}\") to instance of {key.GetType().CanonicalName()}. Perhaps missing [{nameof(AssetKeyConstructorAttribute)}(\"{parameterName}\")] attribute?");

        /// <summary>
        /// Tests if part is canonical.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public static bool IsCanonical(this IAssetKeyParametrizer reader, object part)
        {
            string[] parameters = reader.GetPartParameters(part);
            if (parameters == null) return false;
            foreach (var parameter in parameters)
                if (reader.IsCanonical(part, parameter)) return true;
            return false;
        }

    }
}
