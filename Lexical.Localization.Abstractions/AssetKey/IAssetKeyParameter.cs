// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of assigning a new parameter.
    /// </summary>
    public interface IAssetKeyParameterAssignable : IAssetKey
    {
        /// <summary>
        /// Create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="parameterName">parameter name describes the key type to be created</param>
        /// <param name="parameterValue">parameter value translates to <see cref="IAssetKey.Name"/>.</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="AssetKeyException">If append failed</exception>
        IAssetKeyParametrized AppendParameter(string parameterName, string parameterValue);
    }

    /// <summary>
    /// Key node that has parametrizable name.
    /// </summary>
    public interface IAssetKeyParametrized : IAssetKeySection
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        String ParameterName { get; }
    }

    /// <summary>
    /// Function that visits one parametrized key part.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <param name="data"></param>
    public delegate void KeyParameterVisitor<T>(string parameterName, string parameterValue, ref T data);

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Get parameter name of the key link.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>name or null</returns>
        public static string GetParameterName(this IAssetKey key)
            => key is IAssetKeyParametrized parametrized ? parametrized.ParameterName : null;

        /// <summary>
        /// Get the number of parameters.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>number of parameters</returns>
        public static int GetParameterCount(this IAssetKey key)
        {
            int count = 0;
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                if (k.GetParameterName() != null) count++;
            return count;
        }

        /// <summary>
        /// Get all parameters as parameterName,parameterValue pairs.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string>[] GetParameters(this IAssetKey key)
        {
            int count = 0;
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                if (k.GetParameterName() != null) count++;

            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[count];
            int ix = count;
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
            {
                string parameterName = k.GetParameterName();
                if (parameterName != null)
                    result[--ix] = new KeyValuePair<string, string>(parameterName, k.Name);
            }

            return result;
        }

        /// <summary>
        /// Visit parametrized keys from root towards key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="visitor"></param>
        public static void VisitParameters<T>(this IAssetKey key, KeyParameterVisitor<T> visitor, ref T data)
        {
            // Push to stack
            IAssetKey prevKey = key.GetPreviousKey();
            if (prevKey != null) VisitParameters(prevKey, visitor, ref data);

            // Pop from stack in reverse order
            if (key is IAssetKeyParametrized parameter && parameter.ParameterName!=null) visitor(parameter.ParameterName, parameter.Name, ref data);
        }

        /// <summary>
        /// Create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameterName">parameter name describes the key type to be created</param>
        /// <param name="parameterValue">parameter value translates to <see cref="IAssetKey.Name"/>.</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement IAssetKeyParameterAssignable, or append failed</exception>
        public static IAssetKey AppendParameter(this IAssetKey key, string parameterName, string parameterValue)
        {
            if (key is IAssetKeyParameterAssignable assignable)
            {
                return assignable.AppendParameter(parameterName, parameterValue);
            }
            else
            {
                throw new AssetKeyException(key, $"Cannot append to {key.GetType().FullName}, doesn't implement {nameof(IAssetKeyParameterAssignable)}.");
            }
        }

        /// <summary>
        /// Try to create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameterName">parameter name describes the key type to be created</param>
        /// <param name="parameterValue">parameter value translates to <see cref="IAssetKey.Name"/>.</param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static IAssetKey TryAppendParameter(this IAssetKey key, string parameterName, string parameterValue)
        {
            if (key is IAssetKeyParameterAssignable assignable)
            {
                try
                {
                    return assignable.AppendParameter(parameterName, parameterValue);
                } catch (AssetKeyException)
                {
                }
            }
            return null;
        }

        /// <summary>
        /// Create a new key by appending an enumeration of parameters.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters">enumeration of parameters to append</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement IAssetKeyParameterAssignable, or append failed</exception>
        public static IAssetKey AppendParameters(this IAssetKey key, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            foreach (var parameter in parameters)
                key = key.AppendParameter(parameter.Key, parameter.Value);
            return key;
        }

        /// <summary>
        /// Try to create a new key by appending an enumeration of parameters.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static IAssetKey TryAppendParameters(this IAssetKey key, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            foreach (var parameter in parameters)
                key = key.TryAppendParameters(parameters);
            return key;
        }

        /// <summary>
        /// Concatenate <paramref name="anotherKey"/> to this <paramref name="key"/> and return the concatenated key.
        /// If <paramref name="anotherKey"/> contains non-parametrizable nodes such as <see cref="ILocalizationKeyInlined"/> or <see cref="ILocalizationKeyFormatArgs"/>
        /// then these keys are not appended to the result.
        /// </summary>
        /// <param name="key">Key that must implement <see cref="IAssetKeyParameterAssignable"/>.</param>
        /// <param name="anotherKey"></param>
        /// <returns>concatenated key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement IAssetKeyParameterAssignable</exception>
        public static IAssetKey Concat(this IAssetKey key, IAssetKey anotherKey)
        {
            if (key is IAssetKeyParameterAssignable assignable)
            {
                IAssetKey result = assignable;
                anotherKey.VisitFromRoot(_concatVisitor, ref result);
                return result;
            }
            else
            {
                throw new AssetKeyException(key, $"Cannot append to {key.GetType().FullName}, doesn't implement {nameof(IAssetKeyParameterAssignable)}.");
            }
        }

        /// <summary>
        /// Try concatenate <paramref name="anotherKey"/> to this <paramref name="key"/> and return the concatenated key.
        /// If <paramref name="anotherKey"/> contains non-parametrizable nodes such as <see cref="ILocalizationKeyInlined"/> or <see cref="ILocalizationKeyFormatArgs"/>
        /// then these keys are not appended to the result.
        /// </summary>
        /// <param name="key">Key that must implement <see cref="IAssetKeyParameterAssignable"/>.</param>
        /// <param name="anotherKey"></param>
        /// <returns>concatenated key or null</returns>
        public static IAssetKey TryConcat(this IAssetKey key, IAssetKey anotherKey)
        {
            if (key is IAssetKeyParameterAssignable assignable)
            {
                IAssetKey result = assignable;
                anotherKey.VisitFromRoot(_tryConcatVisitor, ref result);
                return result;
            }
            return null;
        }

        static AssetKeyVisitor<IAssetKey> _concatVisitor = concatVisitor, _tryConcatVisitor = tryConcatVisitor;
        private static void concatVisitor(IAssetKey key, ref IAssetKey result)
        {
            if (key is IAssetKeyParametrized keyParametrized)
            {
                if (result is IAssetKeyParameterAssignable assignable)
                    result = assignable.AppendParameter(keyParametrized.ParameterName, key.Name);
                else
                    throw new AssetKeyException(key, $"Cannot append to {key.GetType().FullName}, doesn't implement {nameof(IAssetKeyParameterAssignable)}.");
            }
        }
        private static void tryConcatVisitor(IAssetKey key, ref IAssetKey result)
        {
            if (key is IAssetKeyParametrized keyParametrized)
            {
                result = result?.TryAppendParameter(keyParametrized.ParameterName, key.Name);
            }
        }

    }


}
