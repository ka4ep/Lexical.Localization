// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of culture policy assignment.
    /// </summary>
    public interface ILocalizationKeyCulturePolicyAssignable : IAssetKey
    {
        /// <summary>
        /// Set culture policy.
        /// </summary>
        /// <param name=""></param>
        /// <returns>a key with a culture policy. (most likely the same key)</returns>
        /// <exception cref="InvalidOperationException">If object is read-only</exception>
        ILocalizationKeyCulturePolicy SetCulturePolicy(ICulturePolicy culturePolicy);
    }

    /// <summary>
    /// Key has been set with <see cref="ICulturePolicy"/> hint.
    /// </summary>
    public interface ILocalizationKeyCulturePolicy : ILocalizationKey
    {
        /// <summary>
        /// Selected culture policy, or null.
        /// </summary>
        ICulturePolicy CulturePolicy { get; }
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Try to set culture policy.
        /// </summary>
        /// <param name="key">(optional) key </param>
        /// <param name="culturePolicy"></param>
        /// <returns>key or null</returns>
        public static ILocalizationKeyCulturePolicy TrySetCulturePolicy(this ILocalizationKeyCulturePolicyAssignable key, ICulturePolicy culturePolicy)
        {
            try
            {
                return key.SetCulturePolicy(culturePolicy);
            } catch (InvalidOperationException)
            {
                return null;
            }
        }
        /// <summary>
        /// Find key where culture policy can be assigned to.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>key or null</returns>
        public static ILocalizationKeyCulturePolicyAssignable FindCulturePolicyAssignableKey(this IAssetKey key)
            => key.Find<ILocalizationKeyCulturePolicyAssignable>();

        /// <summary>
        /// Walks linked list and searches for culture policy setting.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>culture policy or null</returns>
        public static ICulturePolicy FindCulturePolicy(this IAssetKey key)
        {
            for (;key!=null; key=key.GetPreviousKey())
                if (key is ILocalizationKeyCulturePolicy cultureKey && cultureKey.CulturePolicy != null) return cultureKey.CulturePolicy;
            return null;
        }
    }

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILocalizationKeyCulturePolicy"/> values of keys.
    /// </summary>
    public class LocalizationKeyCulturePolicyComparer : IEqualityComparer<IAssetKey>
    {
        static IEqualityComparer<ICulturePolicy> comparer = new ReferenceComparer<ICulturePolicy>();
        public bool Equals(IAssetKey x, IAssetKey y)
            => comparer.Equals(x?.FindCulturePolicy(), y?.FindCulturePolicy());
        public int GetHashCode(IAssetKey obj)
            => comparer.GetHashCode(obj?.FindCulturePolicy());
    }

}
