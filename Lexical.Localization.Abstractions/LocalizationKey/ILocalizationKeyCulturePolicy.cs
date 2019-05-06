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
    public interface ILocalizationKeyCulturePolicyAssignable : ILine
    {
        /// <summary>
        /// Set culture policy.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns>a key with a culture policy. (most likely the same key)</returns>
        /// <exception cref="InvalidOperationException">If object is read-only</exception>
        ILocalizationKeyCulturePolicyAssigned CulturePolicy(ICulturePolicy culturePolicy);
    }

    /// <summary>
    /// Key has been set with <see cref="ICulturePolicy"/> hint.
    /// </summary>
    public interface ILocalizationKeyCulturePolicyAssigned : ILocalizationKey
    {
        /// <summary>
        /// Selected culture policy, or null.
        /// </summary>
        ICulturePolicy CulturePolicy { get; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Try to set culture policy.
        /// </summary>
        /// <param name="key">(optional) key </param>
        /// <param name="culturePolicy"></param>
        /// <returns>key or null</returns>
        public static ILocalizationKeyCulturePolicyAssigned TrySetCulturePolicy(this ILocalizationKeyCulturePolicyAssignable key, ICulturePolicy culturePolicy)
        {
            try
            {
                return key.CulturePolicy(culturePolicy);
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
        public static ILocalizationKeyCulturePolicyAssignable FindCulturePolicyAssignableKey(this ILine key)
            => key.Find<ILocalizationKeyCulturePolicyAssignable>();

        /// <summary>
        /// Walks linked list and searches for culture policy setting.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture policy or null</returns>
        public static ICulturePolicy FindCulturePolicy(this ILine line)
        {
            for (;line!=null; line=line.GetPreviousPart())
                if (line is ILocalizationKeyCulturePolicyAssigned cultureKey && cultureKey.CulturePolicy != null) return cultureKey.CulturePolicy;
            return null;
        }
    }

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILocalizationKeyCulturePolicyAssigned"/> values of keys.
    /// </summary>
    public class LocalizationKeyCulturePolicyComparer : IEqualityComparer<ILine>
    {
        static IEqualityComparer<ICulturePolicy> comparer = new ReferenceComparer<ICulturePolicy>();
        public bool Equals(ILine x, ILine y)
            => comparer.Equals(x?.FindCulturePolicy(), y?.FindCulturePolicy());
        public int GetHashCode(ILine obj)
            => comparer.GetHashCode(obj?.FindCulturePolicy());
    }

}
