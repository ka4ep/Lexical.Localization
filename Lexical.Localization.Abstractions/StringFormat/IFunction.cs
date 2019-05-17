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
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected error</exception>
        bool TryEvaluate(ref FunctionEvaluationContext ctx, object result);
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
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        bool TryEvaluate(ref FunctionEvaluationContext ctx, object arg0, out object result);
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
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        bool TryEvaluate(ref FunctionEvaluationContext ctx, object arg0, object arg1, out object result);
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
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        bool TryEvaluate(ref FunctionEvaluationContext ctx, object arg0, object arg1, object arg2, out object result);
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
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        bool TryEvaluate(ref FunctionEvaluationContext ctx, object[] args, out object result);
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
        //public ILine Line;
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
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static bool TryEvaluate(this IFunction function, ref FunctionEvaluationContext ctx, out object result)
        {
            if (function is IFunction0 func0 && func0.TryEvaluate(ref ctx, out result)) return true;
            if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, no_args, out result)) return true;
            result = null; return false;
        }

        /// <summary>
        /// Call 1-argument function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static bool TryEvaluate(this IFunction function, ref FunctionEvaluationContext ctx, object arg0, out object result)
        {
            if (function is IFunction1 func1 && func1.TryEvaluate(ref ctx, arg0, out result)) return true;
            if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, new object[] { arg0 } ?? no_args, out result)) return true;
            result = null; return false;
        }

        /// <summary>
        /// Call 2-argument function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="arg1"></param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="InvalidOperationException">If function is not found</exception>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static bool TryEvaluate(this IFunction function, ref FunctionEvaluationContext ctx, object arg0, object arg1, out object result)
        {
            if (function is IFunction2 func2 && func2.TryEvaluate(ref ctx, arg0, arg1, out result)) return true;
            if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, new object[] { arg0, arg1 }, out result)) return true;
            result = null; return false;
        }

        /// <summary>
        /// Call 3-argument function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="arg0">Function argument</param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected situation</exception>
        public static bool TryEvaluate(this IFunction function, ref FunctionEvaluationContext ctx, object arg0, object arg1, object arg2, out object result)
        {
            if (function is IFunction3 func3 && func3.TryEvaluate(ref ctx, arg0, arg1, arg2, out result)) return true;
            if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, new object[] { arg0, arg1, arg2 }, out result)) return true;
            result = null; return false;
        }

        /// <summary>
        /// Call n-arguments function.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="ctx"></param>
        /// <param name="args">Function arguments</param>
        /// <param name="result">Object, String, Long, Double or null</param>
        /// <returns>true if evaluated, false if not</returns>
        /// <exception cref="Exception">On unexpected error</exception>
        public static bool TryEvaluate(this IFunction function, ref FunctionEvaluationContext ctx, object[] args, out object result)
        {
            if (args == null || args.Length == 0)
            {
                if (function is IFunction0 func0 && func0.TryEvaluate(ref ctx, out result)) return true;
                if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args ?? no_args, out result)) return true;
            }
            if (args.Length == 1)
            {
                if (function is IFunction1 func1 && func1.TryEvaluate(ref ctx, args[0], out result)) return true;
                if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args, out result)) return true;
            }
            if (args.Length == 2)
            {
                if (function is IFunction2 func2 && func2.TryEvaluate(ref ctx, args[0], args[1], out result)) return true;
                if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args, out result)) return true;
            }
            if (args.Length == 3)
            {
                if (function is IFunction3 func3 && func3.TryEvaluate(ref ctx, args[0], args[1], args[2], out result)) return true;
                if (function is IFunctionN funcN && funcN.TryEvaluate(ref ctx, args, out result)) return true;
            }
            if (function is IFunctionN funcN_ && funcN_.TryEvaluate(ref ctx, args, out result)) return true;
            result = null; return false;
        }


    }

}
