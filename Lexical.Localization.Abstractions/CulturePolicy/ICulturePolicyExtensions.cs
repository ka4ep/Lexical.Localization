//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.12.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods for <see cref="ICulturePolicy"/>.
    /// </summary>
    public static partial class ICulturePolicyExtensions
    {
        /// <summary>
        /// Return an <see cref="ICulturePolicy"/> that doesn't implement <see cref="ICulturePolicyAssignable"/>.
        /// Returns the same enumerable as in <paramref name="culturePolicy"/>.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicy AsReadonly(this ICulturePolicy culturePolicy)
            => culturePolicy is CulturePolicyImmutable immutable ? immutable : new CulturePolicyImmutable(culturePolicy.Cultures);

        /// <summary>
        /// Assign culture and default fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetCultureWithFallbackCultures(this ICulturePolicy culturePolicy, string cultureName)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyWithFallbacks(CultureInfo.GetCultureInfo(cultureName))) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Assign cultures as an array of culture names.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="cultureNames"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetCultures(this ICulturePolicy culturePolicy, params string[] cultureNames)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyArray(cultureNames.Select(_ => CultureInfo.GetCultureInfo(_)).ToArray())) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Assign cultures as an array of culture names.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="cultureInfos"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetCultures(this ICulturePolicy culturePolicy, params CultureInfo[] cultureInfos)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyArray(cultureInfos)) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Takes an array snapshot of the underlying source ienumerable and replaces that.
        /// </summary>
        /// <param name="culturePolicy">policy</param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable ToSnapshot(this ICulturePolicy culturePolicy)
            => culturePolicy.SetCultures(culturePolicy.Cultures.ToArray());

        /// <summary>
        /// Set to acquire culture policies from another culture policy <paramref name="source"/>.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="source"></param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetSource(this ICulturePolicy culturePolicy, ICulturePolicy source)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(source) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to acquire culture policies from a delegate that returns another culture policy <paramref name="sourceFunc"/>.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="sourceFunc"></param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetSourceFunc(this ICulturePolicy culturePolicy, Func<ICulturePolicy> sourceFunc)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyFunc(sourceFunc)) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to acquire cultures from an array of delegates.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="func"></param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetFunc(this ICulturePolicy culturePolicy, Func<CultureInfo[]> func)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyArrayFunc(func)) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to acquire cultures from an array of delegates.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="func"></param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetFunc(this ICulturePolicy culturePolicy, Func<CultureInfo> func)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyFuncWithFallbacks(func)) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Function that returns <code>CultureInfo.CurrentCulture</code>.
        /// </summary>
        public static Func<CultureInfo> FuncCurrentCulture = () => CultureInfo.CurrentCulture;

        /// <summary>
        /// Function that returns <code>CultureInfo.CurrentUICulture</code>.
        /// </summary>
        public static Func<CultureInfo> FuncCurrentUICulture = () => CultureInfo.CurrentUICulture;

        /// <summary>
        /// Function that returns <code>Thread.CurrentThread.CurrentCulture</code>.
        /// </summary>
        public static Func<CultureInfo> FuncCurrentThreadCulture = () => Thread.CurrentThread.CurrentCulture;

        /// <summary>
        /// Function that returns <code>Thread.CurrentThread.CurrentUICulture</code>.
        /// </summary>
        public static Func<CultureInfo> FuncCurrentThreadUICulture = () => Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// Set to return CultureInfo.CurrentCulture, and then fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetToCurrentCulture(this ICulturePolicy culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentCulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to return CultureInfo.CurrentUICulture, and then fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetToCurrentUICulture(this ICulturePolicy culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentUICulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to return <code>Thread.CurrentThread.CurrentCulture</code>, and its respective fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetToCurrentThreadCulture(this ICulturePolicy culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentThreadCulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to return <code>Thread.CurrentThread.CurrentUICulture</code>, and its respective fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="culturePolicy"/> doesnt implement <see cref="ICulturePolicyAssignable"/></exception>
        public static ICulturePolicyAssignable SetToCurrentThreadUICulture(this ICulturePolicy culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentThreadUICulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));
    }

}
