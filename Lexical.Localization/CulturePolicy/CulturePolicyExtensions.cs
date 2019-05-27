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
        /// <param name="culturePolicy"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
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
        public static ICulturePolicyAssignable SetCultures(this ICulturePolicyAssignable culturePolicy, params string[] cultureNames)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyArray(cultureNames.Select(_ => CultureInfo.GetCultureInfo(_)).ToArray())) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Assign cultures as an array of culture names.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="cultureInfos"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetCultures(this ICulturePolicy culturePolicy, params CultureInfo[] cultureInfos)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyArray(cultureInfos)) :
               throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Takes an array snapshot of the underlying source ienumerable and replaces that.
        /// </summary>
        /// <param name="culturePolicy">policy</param>
        /// <returns>this</returns>
        public static ICulturePolicyAssignable ToSnapshot(this ICulturePolicy culturePolicy)
            => culturePolicy.SetCultures(culturePolicy.Cultures.ToArray());

        /// <summary>
        /// Set to acquire culture policies from another culture policy <paramref name="source"/>.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <param name="source"></param>
        /// <returns>this</returns>
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
        public static ICulturePolicyAssignable SetFunc(this ICulturePolicy culturePolicy, Func<CultureInfo[]> func)
            => culturePolicy is ICulturePolicyAssignable assignable ?
               assignable.SetSource(new CulturePolicyArrayFunc(func)) :
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
        public static ICulturePolicyAssignable SetToCurrentCulture(this ICulturePolicy culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentCulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to return CultureInfo.CurrentUICulture, and then fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetToCurrentUICulture(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentUICulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to return <code>Thread.CurrentThread.CurrentCulture</code>, and its respective fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetToCurrentThreadCulture(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentThreadCulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));

        /// <summary>
        /// Set to return <code>Thread.CurrentThread.CurrentUICulture</code>, and its respective fallback cultures.
        /// </summary>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ICulturePolicyAssignable SetToCurrentThreadUICulture(this ICulturePolicyAssignable culturePolicy)
            => culturePolicy is ICulturePolicyAssignable assignable ? assignable.SetSource(new CulturePolicyFuncWithFallbacks(FuncCurrentThreadUICulture)) : throw new ArgumentException($"Is not {nameof(ICulturePolicyAssignable)}", nameof(culturePolicy));
    }

    /// <summary>
    /// Culture policy that reads from another source
    /// </summary>
    public class CulturePolicySource : ICulturePolicy
    {
        static CultureInfo[] empty = new CultureInfo[0];

        /// <summary>
        /// Cultures
        /// </summary>
        public CultureInfo[] Cultures => source?.Cultures ?? empty;

        ICulturePolicy source;

        /// <summary>
        /// Create policy
        /// </summary>
        /// <param name="source"></param>
        public CulturePolicySource(ICulturePolicy source)
        {
            this.source = source;
        }
    }


    /// <summary>
    /// Culture policy that reads active policy from function.
    /// </summary>
    public class CulturePolicyFunc : ICulturePolicy
    {
        static CultureInfo[] empty = new CultureInfo[0];

        /// <summary>
        /// Cultures
        /// </summary>
        public CultureInfo[] Cultures => sourceFunc == null ? empty : (sourceFunc()?.Cultures) ?? empty;

        /// <summary>
        /// Function
        /// </summary>
        Func<ICulturePolicy> sourceFunc;

        /// <summary>
        /// create
        /// </summary>
        /// <param name="sourceFunc"></param>
        public CulturePolicyFunc(Func<ICulturePolicy> sourceFunc)
        {
            this.sourceFunc = sourceFunc;
        }
    }

    /// <summary>
    /// Culture policy that reads active policy from array function.
    /// </summary>
    public class CulturePolicyArrayFunc : ICulturePolicy
    {
        static CultureInfo[] empty = new CultureInfo[0];

        /// <summary>
        /// Cultures
        /// </summary>
        public CultureInfo[] Cultures => funcs == null ? empty : funcs() ?? empty;

        Func<CultureInfo[]> funcs;

        /// <summary>
        /// Create policy
        /// </summary>
        /// <param name="funcs"></param>
        public CulturePolicyArrayFunc(Func<CultureInfo[]> funcs)
        {
            this.funcs = funcs;
        }
    }

    /// <summary>
    /// Culture policy that reads active policy from array function.
    /// </summary>
    public class CulturePolicyArray : ICulturePolicy
    {
        /// <summary>
        /// empty
        /// </summary>
        static CultureInfo[] empty = new CultureInfo[0];

        /// <summary>
        /// Cultures
        /// </summary>
        public CultureInfo[] Cultures { get; internal set; }

        /// <summary>
        /// Create policy
        /// </summary>
        /// <param name="cultures"></param>
        public CulturePolicyArray(CultureInfo[] cultures)
        {
            this.Cultures = cultures;
        }
    }

    /// <summary>
    /// Culture policy that returns static array of culture info and its fallback cultures.
    /// </summary>
    public class CulturePolicyWithFallbacks : ICulturePolicy
    {
        /// <summary>
        /// Cultures
        /// </summary>
        public CultureInfo[] Cultures { get; internal set; }

        /// <summary>
        /// Create with cultures info
        /// </summary>
        /// <param name="cultureInfo"></param>
        public CulturePolicyWithFallbacks(CultureInfo cultureInfo)
        {
            Cultures = MakeArray(cultureInfo);
        }

        static CultureInfo[] MakeArray(CultureInfo culture)
        {
            StructList8<CultureInfo> result = new StructList8<CultureInfo>();
            for (CultureInfo ci = culture; ci != null; ci = ci.Parent)
                if (result.Contains(ci)) break; else result.Add(ci);
            return result.ToArray();
        }
    }

    /// <summary>
    /// Culture policy that reads culture from a function, and then returns the culture plus its fallback cultures.
    /// </summary>
    public class CulturePolicyFuncWithFallbacks : ICulturePolicy
    {
        Func<CultureInfo> func;
        CultureInfo[] array;

        /// <summary>
        /// Create with function
        /// </summary>
        /// <param name="func"></param>
        public CulturePolicyFuncWithFallbacks(Func<CultureInfo> func)
        {
            this.func = func;
        }

        static CultureInfo[] empty = new CultureInfo[0];

        /// <summary>
        /// Cultures
        /// </summary>
        public CultureInfo[] Cultures
        {
            get
            {
                if (func == null) return empty;
                CultureInfo _ci = func();
                if (_ci == null) return empty;
                var _arr = array;
                if (_arr != null && _arr[0] == _ci) return _arr ?? empty;
                return array = _arr = MakeArray(_ci);
            }
        }

        static CultureInfo[] MakeArray(CultureInfo culture)
        {
            StructList8<CultureInfo> result = new StructList8<CultureInfo>();
            for (CultureInfo ci = culture; ci != null; ci = ci.Parent)
                if (result.Contains(ci)) break; else result.Add(ci);
            return result.ToArray();
        }
    }

}
