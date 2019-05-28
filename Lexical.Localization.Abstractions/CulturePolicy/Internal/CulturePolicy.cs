//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.12.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// <see cref="ICulturePolicy"/> implementation where culture enumerable is immutable.
    /// Note that the source of the enumerable is modifiable.
    /// </summary>
    public class CulturePolicyImmutable : ICulturePolicy
    {
        /// <summary>
        /// No cultures array.
        /// </summary>
        public static CultureInfo[] NO_CULTURES = new CultureInfo[0];

        /// <summary>
        /// Source of cultures
        /// </summary>
        protected ICulturePolicy source;

        /// <summary>
        /// Cultures. The first element is active culture, others fallback cultures.
        /// </summary>
        public CultureInfo[] Cultures => source?.Cultures ?? NO_CULTURES;

        /// <summary>
        /// Create new culture policy with initial cultures <paramref name="initialCultures"/>.
        /// </summary>
        /// <param name="initialCultures"></param>
        public CulturePolicyImmutable(IEnumerable<CultureInfo> initialCultures)
        {
            source = new CulturePolicyArray(initialCultures?.ToArray() ?? NO_CULTURES);
        }

        /// <summary>
        /// Culture policy with unmodifiable source.
        /// </summary>
        /// <param name="culturePolicy"></param>
        public CulturePolicyImmutable(ICulturePolicy culturePolicy)
        {
            source = culturePolicy;
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
