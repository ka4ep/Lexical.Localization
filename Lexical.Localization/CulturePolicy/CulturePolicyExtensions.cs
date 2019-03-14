//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.12.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods for <see cref="ICulturePolicy"/>.
    /// </summary>
    public static partial class CulturePolicyExtensions
    {
        /// <summary>
        /// Return an <see cref="ICulturePolicy"/> that doesn't implement <see cref="ICulturePolicyAssignable"/>.
        /// Returns the same enumerable as in <paramref name="culturePolicy"/>.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicy AsReadonly(this ICulturePolicy culturePolicy)
            => new CulturePolicyImmutable(culturePolicy.Cultures);

        /// <summary>
        /// Assign culture and default fallback cultures.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetCultureWithFallbackCultures(this ICulturePolicyAssignable culturePolicy, string cultureName)
            => culturePolicy.SetCultures( new CultureFallbackEnumerable(CultureInfo.GetCultureInfo(cultureName)).ToArray() );

        /// <summary>
        /// Assign cultures as an array of culture names.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="cultureNames"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetCultures(this ICulturePolicyAssignable culturePolicy, params string[] cultureNames)
            => culturePolicy.SetCultures(cultureNames.Select(_ => CultureInfo.GetCultureInfo(_)).ToArray());

        /// <summary>
        /// Assign cultures as an array of culture names.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="cultureInfos"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetCultures(this ICulturePolicyAssignable culturePolicy, params CultureInfo[] cultureInfos)
            => culturePolicy.SetCultures(cultureInfos);

        /// <summary>
        /// Takes an array snapshot of the underlying source ienumerable and replaces that.
        /// </summary>
        /// <param name="culturePolicy">policy</param>
        /// <returns>this</returns>
        public static ICulturePolicyAssignable ToSnapshot(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy.SetCultures(culturePolicy.Cultures.ToArray());

        /// <summary>
        /// Set to acquire culture policies from another culture policy <paramref name="source"/>.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="source"></param>
        /// <returns>this</returns>
        public static ICulturePolicyAssignable SetSource(this ICulturePolicyAssignable culturePolicy, ICulturePolicy source)
            => culturePolicy.SetCultures(new CulturePolicySourceEnumerable(source));

        /// <summary>
        /// Set to acquire culture policies from a delegate that returns another culture policy <paramref name="sourceFunc"/>.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="sourceFunc"></param>
        /// <returns>this</returns>
        public static ICulturePolicyAssignable SetSourceFunc(this ICulturePolicyAssignable culturePolicy, Func<ICulturePolicy> sourceFunc)
            => culturePolicy.SetCultures(new CulturePolicySourceFuncEnumerable(sourceFunc));

        /// <summary>
        /// Set to acquire cultures from an array of delegates.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="funcs"></param>
        /// <returns>this</returns>
        public static ICulturePolicyAssignable SetFunc(this ICulturePolicyAssignable culturePolicy, params Func<CultureInfo>[] funcs)
            => culturePolicy.SetCultures(new CulturePolicyFuncs(funcs));

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
        public static ICulturePolicyAssignable SetToCurrentCulture(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy.SetCultures(new CultureFallbackEnumerableFunc(FuncCurrentCulture));

        /// <summary>
        /// Set to return CultureInfo.CurrentUICulture, and then fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetToCurrentUICulture(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy.SetCultures(new CultureFallbackEnumerableFunc(FuncCurrentUICulture));

        /// <summary>
        /// Set to return <code>Thread.CurrentThread.CurrentCulture</code>, and its respective fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetToCurrentThreadCulture(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy.SetCultures(new CultureFallbackEnumerableFunc(FuncCurrentThreadCulture));

        /// <summary>
        /// Set to return <code>Thread.CurrentThread.CurrentUICulture</code>, and its respective fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetToCurrentThreadUICulture(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy.SetCultures(new CultureFallbackEnumerableFunc(FuncCurrentThreadUICulture));
    }

    class CulturePolicySourceEnumerable : IEnumerable<CultureInfo>
    {
        ICulturePolicy source;
        public CulturePolicySourceEnumerable(ICulturePolicy source)
        {
            this.source = source;
        }
        public IEnumerator<CultureInfo> GetEnumerator() => source.Cultures.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => source.Cultures.GetEnumerator();
    }

    class CulturePolicySourceFuncEnumerable : IEnumerable<CultureInfo>
    {
        Func<ICulturePolicy> sourceFunc;
        public CulturePolicySourceFuncEnumerable(Func<ICulturePolicy> sourceFunc)
        {
            this.sourceFunc = sourceFunc;
        }
        public IEnumerator<CultureInfo> GetEnumerator() => sourceFunc().Cultures.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => sourceFunc().Cultures.GetEnumerator();
    }

    class CulturePolicyFuncs : IEnumerable<CultureInfo>
    {
        Func<CultureInfo>[] funcs;

        public CulturePolicyFuncs(Func<CultureInfo>[] funcs)
        {
            this.funcs = funcs;
        }

        public IEnumerator<CultureInfo> GetEnumerator()
        {
            foreach(var func in funcs)
            {
                CultureInfo ci = func();
                if (ci != null) yield return ci;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var func in funcs)
            {
                CultureInfo ci = func();
                if (ci != null) yield return ci;
            }
        }
    }

    class CultureFallbackEnumerable : IEnumerable<CultureInfo>
    {
        CultureInfo cultureInfo;

        public CultureFallbackEnumerable(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        public IEnumerator<CultureInfo> GetEnumerator()
        {
            CultureInfo culture = cultureInfo;
            while (culture != null)
            {
                // yield this
                yield return culture;

                // Go to next culture
                var nextCulture = culture.Parent;
                if (nextCulture == culture) yield break;
                culture = nextCulture;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            CultureInfo culture = cultureInfo;
            while (culture != null)
            {
                // yield this
                yield return culture;

                // Go to next culture
                var nextCulture = culture.Parent;
                if (nextCulture == culture) yield break;
                culture = nextCulture;
            }
        }
    }

    class CultureFallbackEnumerableFunc : IEnumerable<CultureInfo>
    {
        Func<CultureInfo> func;

        public CultureFallbackEnumerableFunc(Func<CultureInfo> func)
        {
            this.func = func;
        }

        public IEnumerator<CultureInfo> GetEnumerator()
        {
            CultureInfo culture = func == null ? null : func();
            while (culture != null)
            {
                // yield this
                yield return culture;

                // Go to next culture
                var nextCulture = culture.Parent;
                if (nextCulture == culture) yield break;
                culture = nextCulture;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            CultureInfo culture = func == null ? null : func();
            while (culture != null)
            {
                // yield this
                yield return culture;

                // Go to next culture
                var nextCulture = culture.Parent;
                if (nextCulture == culture) yield break;
                culture = nextCulture;
            }
        }
    }

}
