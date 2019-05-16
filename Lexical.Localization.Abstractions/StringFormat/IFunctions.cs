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

    /// <summary>
    /// Function used by <see cref="IStringFormat"/>.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Name of the function.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Get arguments
        /// </summary>
        FunctionArgumentInfo[] Args { get; }
    }

    /// <summary>
    /// Function with zero arguments.
    /// </summary>
    public interface IFunction0 : IFunction
    {
        /// <summary>
        /// Call 0-argument function.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        object Evaluate(ref StringFormatEvaluationContext ctx);
    }

    /// <summary>
    /// Function with one argument.
    /// </summary>
    public interface IFunction1 : IFunction
    {
        /// <summary>
        /// Call 1-argument function.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="arg0"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        object Evaluate(ref StringFormatEvaluationContext ctx, object arg0);
    }

    /// <summary>
    /// Function with two arguments.
    /// </summary>
    public interface IFunction2 : IFunction
    {
        /// <summary>
        /// Call 2-argument function.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        object Evaluate(ref StringFormatEvaluationContext ctx, object arg0, object arg1);
    }

    /// <summary>
    /// Function with three arguments.
    /// </summary>
    public interface IFunction3 : IFunction
    {
        /// <summary>
        /// Call 3-argument function.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        object Evaluate(ref StringFormatEvaluationContext ctx, object arg0, object arg1, object arg2);
    }

    /// <summary>
    /// Function with N-arguments.
    /// </summary>
    public interface IFunctionN : IFunction
    {
        /// <summary>
        /// Call n-argument function.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="args"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        object Evaluate(ref StringFormatEvaluationContext ctx, object[] args);
    }

    /// <summary>
    /// Argument info
    /// </summary>
    public struct FunctionArgumentInfo
    {
        /// <summary>
        /// Argument name
        /// </summary>
        public string Name;

        /// <summary>
        /// Argument type
        /// </summary>
        public Type Type;
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
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunctions functions, string functionName, ref StringFormatEvaluationContext ctx)
        {
            if (functions == null) throw new ArgumentNullException(nameof(functions));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction0 func0) return func0.Evaluate(ref ctx);
                if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, no_args);
                throw new InvalidOperationException($"{func} is not callable");
            }
            throw new KeyNotFoundException(functionName);
        }

        /// <summary>
        /// Call 1-argument function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunctions functions, string functionName, ref StringFormatEvaluationContext ctx, object arg0)
        {
            if (functions == null) throw new ArgumentNullException(nameof(functions));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction1 func1) return func1.Evaluate(ref ctx, arg0);
                if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, new object[] { arg0 } ?? no_args);
                throw new InvalidOperationException($"{func} is not callable");
            }
            throw new KeyNotFoundException(functionName);
        }

        /// <summary>
        /// Call 2-argument function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="arg1"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunctions functions, string functionName, ref StringFormatEvaluationContext ctx, object arg0, object arg1)
        {
            if (functions == null) throw new ArgumentNullException(nameof(functions));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction2 func2) return func2.Evaluate(ref ctx, arg0, arg1);
                if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, new object[] { arg0, arg1 });
                throw new InvalidOperationException($"{func} is not callable");
            }
            throw new KeyNotFoundException(functionName);
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
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunctions functions, string functionName, ref StringFormatEvaluationContext ctx, object arg0, object arg1, object arg2)
        {
            if (functions == null) throw new ArgumentNullException(nameof(functions));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (func is IFunction3 func3) return func3.Evaluate(ref ctx, arg0, arg1, arg2);
                if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, new object[] { arg0, arg1, arg2 });
                throw new InvalidOperationException($"{func} is not callable");
            }
            throw new KeyNotFoundException(functionName);
        }

        /// <summary>
        /// Call n-arguments function.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="functionName"></param>
        /// <param name="ctx"></param>
        /// <param name="args">Function arguments</param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunctions functions, string functionName, ref StringFormatEvaluationContext ctx, object[] args)
        {
            if (functions == null) throw new ArgumentNullException(nameof(functions));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            IFunction func;
            if (functions is IFunctionsQueryable queryable && queryable.TryGetValue(functionName, out func))
            {
                if (args == null || args.Length == 0)
                {
                    if (func is IFunction0 func0) return func0.Evaluate(ref ctx);
                    if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, args ?? no_args);
                }
                if (args.Length == 1)
                {
                    if (func is IFunction1 func1) return func1.Evaluate(ref ctx, args[0]);
                    if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, args);
                }
                if (args.Length == 2)
                {
                    if (func is IFunction2 func2) return func2.Evaluate(ref ctx, args[0], args[1]);
                    if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, args);
                }
                if (args.Length == 3)
                {
                    if (func is IFunction3 func3) return func3.Evaluate(ref ctx, args[0], args[1], args[2]);
                    if (func is IFunctionN funcN) return funcN.Evaluate(ref ctx, args);
                }
                if (func is IFunctionN funcN_) return funcN_.Evaluate(ref ctx, args);
                throw new InvalidOperationException($"{func} is not callable");
            }
            throw new KeyNotFoundException(functionName);
        }

        static object[] no_args = new object[0];

    }

}
