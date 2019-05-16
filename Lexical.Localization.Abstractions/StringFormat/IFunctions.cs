// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Function table used by <see cref="IStringFormat"/> implementations.
    /// 
    /// 
    /// </summary>
    public interface IFunctions
    {
    }

    /// <summary>
    /// Table of functions.
    /// </summary>
    public interface IFunctionsTable : IFunctions, IDictionary<string, IFunction>
    {
    }

    /// <summary>
    /// Queryable functions table.
    /// </summary>
    public interface IFunctionsQueryable
    {
        /// <summary>
        /// Try get function.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        bool TryGetValue(string functionName, out IFunction func);
    }

    /// <summary>
    /// Enumerable capable list of string format functions.
    /// </summary>
    public interface IFunctionsEnumerable : IFunctions, IEnumerable<IFunction>
    {
    }

    /// <summary></summary>
    public static partial class FunctionsExtensions
    {
        /// <summary>
        /// Try get function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool TryGetValue(this IFunctions functions, string functionName, out IFunction func)
        {
            if (functions == null) throw new ArgumentNullException(nameof(functions));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            if (functions is IFunctionsQueryable queryable) return queryable.TryGetValue(functionName, out func);
            func = null;
            return false;
        }

        /// <summary>
        /// Try get function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <returns>function</returns>
        /// <exception cref="InvalidOperationException">If </exception>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        public static IFunction GetValue(this IFunctions functions, string functionName)
        {
            if (functions == null) throw new ArgumentNullException(nameof(functions));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func)) return func;
            throw new KeyNotFoundException(functionName);
        }

        /// <summary>
        /// Call 0-arguments function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected error</exception>
        public static bool TryEvaluate(this IFunctions functions, string functionName, ref FunctionEvaluationContext ctx, out object result)
        {
            if (functions == null || functionName == null) { result = null; return false; }
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction0 func0 && func0.TryEvaluate(ref ctx, out result)) return true;
                if (func is IFunctionN funcN && funcN.TryEvaluate(ref ctx, no_args, out result)) return true;
            }
            result = null; return false;
        }

        /// <summary>
        /// Call 1-argument function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected error</exception>
        public static bool TryEvaluate(this IFunctions functions, string functionName, ref FunctionEvaluationContext ctx, object arg0, out object result)
        {
            if (functions == null || functionName == null) { result = null; return false; }
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction1 func1) return func1.TryEvaluate(ref ctx, arg0, out result);
                if (func is IFunctionN funcN) return funcN.TryEvaluate(ref ctx, new object[] { arg0 } ?? no_args, out result);
            }
            result = null; return false;
        }

        /// <summary>
        /// Call 2-argument function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="arg1"></param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected error</exception>
        public static bool TryEvaluate(this IFunctions functions, string functionName, ref FunctionEvaluationContext ctx, object arg0, object arg1, out object result)
        {
            if (functions == null || functionName == null) { result = null; return false; }
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction2 func2 && func2.TryEvaluate(ref ctx, arg0, arg1, out result)) return true;
                if (func is IFunctionN funcN && funcN.TryEvaluate(ref ctx, new object[] { arg0, arg1 }, out result)) return true;
            }
            result = null; return false;
        }

        /// <summary>
        /// Call 3-argument function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected error</exception>
        public static bool TryEvaluate(this IFunctions functions, string functionName, ref FunctionEvaluationContext ctx, object arg0, object arg1, object arg2, out object result)
        {
            if (functions == null || functionName == null) { result = null; return false; }
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction3 func3 && func3.TryEvaluate(ref ctx, arg0, arg1, arg2, out result)) return true;
                if (func is IFunctionN funcN && funcN.TryEvaluate(ref ctx, new object[] { arg0, arg1, arg2 }, out result)) return true;
            }
            result = null; return false;
        }

        /// <summary>
        /// Call n-arguments function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="args">Function arguments</param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected error</exception>
        public static bool TryEvaluate(this IFunctions functions, string functionName, ref FunctionEvaluationContext ctx, object[] args, out object result)
        {
            if (functions == null || functionName == null) { result = null; return false; }
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (args == null || args.Length == 0)
                {
                    if (func is IFunction0 func0 && func0.TryEvaluate(ref ctx, out result)) return true;
                    if (func is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args ?? no_args, out result)) return true;
                }
                if (args.Length == 1)
                {
                    if (func is IFunction1 func1 && func1.TryEvaluate(ref ctx, args[0], out result)) return true;
                    if (func is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args, out result)) return true;
                }
                if (args.Length == 2)
                {
                    if (func is IFunction2 func2 && func2.TryEvaluate(ref ctx, args[0], args[1], out result)) return true;
                    if (func is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args, out result)) return true;
                }
                if (args.Length == 3)
                {
                    if (func is IFunction3 func3 && func3.TryEvaluate(ref ctx, args[0], args[1], args[2], out result)) return true;
                    if (func is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args, out result)) return true;
                }
                if (func is IFunctionN funcN_ && funcN_.TryEvaluate(ref ctx, args, out result)) return true;
            }
            result = null; return false;
        }

        static object[] no_args = new object[0];
    }

}
