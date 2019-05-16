// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization.StringFormat
{
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
        object Evaluate(ref FunctionEvaluationContext ctx);
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
        object Evaluate(ref FunctionEvaluationContext ctx, object arg0);
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
        object Evaluate(ref FunctionEvaluationContext ctx, object arg0, object arg1);
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
        object Evaluate(ref FunctionEvaluationContext ctx, object arg0, object arg1, object arg2);
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
        object Evaluate(ref FunctionEvaluationContext ctx, object[] args);
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

    /// <summary>
    /// String evaluation context
    /// </summary>
    public partial struct FunctionEvaluationContext
    {
        /// <summary>
        /// (optional) Custom format provider, excluding culture.
        /// </summary>
        public IFormatProvider FormatProvider;

        /// <summary>
        /// (optional) Active culture.
        /// </summary>
        public CultureInfo Culture;

        /// <summary>
        /// (optional) Functions.
        /// </summary>
        public IFunctions Functions;

        /// <summary>
        /// (optional) The <see cref="ILine"/> that is being evaluated.
        /// </summary>
        public ILine Line;
    }

    /// <summary></summary>
    public static partial class FunctionExtensions
    {
        static object[] no_args = new object[0];

        /// <summary>
        /// Call 0-arguments function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunction function, ref FunctionEvaluationContext ctx)
        {
            if (function is IFunction0 func0) return func0.Evaluate(ref ctx);
            if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, no_args);
            throw new InvalidOperationException($"{function} is not callable");
        }

        /// <summary>
        /// Call 1-argument function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="InvalidOperationException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunction function, ref FunctionEvaluationContext ctx, object arg0)
        {
            if (function is IFunction1 func1) return func1.Evaluate(ref ctx, arg0);
            if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, new object[] { arg0 } ?? no_args);
            throw new InvalidOperationException($"{function} is not callable");
        }

        /// <summary>
        /// Call 2-argument function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="arg1"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="InvalidOperationException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunction function, ref FunctionEvaluationContext ctx, object arg0, object arg1)
        {
            if (function is IFunction2 func2) return func2.Evaluate(ref ctx, arg0, arg1);
            if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, new object[] { arg0, arg1 });
            throw new InvalidOperationException($"{function} is not callable");
        }

        /// <summary>
        /// Call 3-argument function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="InvalidOperationException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunction function, ref FunctionEvaluationContext ctx, object arg0, object arg1, object arg2)
        {
            if (function is IFunction3 func3) return func3.Evaluate(ref ctx, arg0, arg1, arg2);
            if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, new object[] { arg0, arg1, arg2 });
            throw new InvalidOperationException($"{function} is not callable");
        }

        /// <summary>
        /// Call n-arguments function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="args">Function arguments</param>
        /// <returns>Object, String, Long, Double or null</returns>
        /// <exception cref="KeyNotFoundException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static object Evaluate(this IFunction function, ref FunctionEvaluationContext ctx, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                if (function is IFunction0 func0) return func0.Evaluate(ref ctx);
                if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, args ?? no_args);
            }
            if (args.Length == 1)
            {
                if (function is IFunction1 func1) return func1.Evaluate(ref ctx, args[0]);
                if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, args);
            }
            if (args.Length == 2)
            {
                if (function is IFunction2 func2) return func2.Evaluate(ref ctx, args[0], args[1]);
                if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, args);
            }
            if (args.Length == 3)
            {
                if (function is IFunction3 func3) return func3.Evaluate(ref ctx, args[0], args[1], args[2]);
                if (function is IFunctionN funcN) return funcN.Evaluate(ref ctx, args);
            }
            if (function is IFunctionN funcN_) return funcN_.Evaluate(ref ctx, args);
            throw new InvalidOperationException($"{function} is not callable");
        }


    }

}
