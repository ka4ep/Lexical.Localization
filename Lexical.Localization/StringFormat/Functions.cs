// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Default functions.
    /// <list type="bullet">
    /// <item><see cref="FormatFunction"/></item>
    /// <item><see cref="AlignmentFunction"/></item>
    /// </list>
    /// </summary>
    public class Functions : FunctionsTable
    {
        private static IFunctions instance = 
            new FunctionsTable()
            .Add(FormatFunction.Instance)
            .Add(AlignmentFunction.Instance);

        /// <summary>
        /// Table of default functions.
        /// </summary>
        public static IFunctions Instance => instance;
    }

    /// <summary>
    /// Function that formats 
    /// </summary>
    public class FormatFunction : IFunction2
    {
        static FunctionArgumentInfo[] args = new FunctionArgumentInfo[] { new FunctionArgumentInfo { Name = "format", Type = typeof(string) }, new FunctionArgumentInfo { Name = "argument", Type = typeof(object) } };
        private static FormatFunction instance = new FormatFunction();

        /// <summary>
        /// "Format" function
        /// </summary>
        public static FormatFunction Instance => instance;

        /// <summary>
        /// Function name
        /// </summary>
        public string Name => "Format";

        /// <summary>
        /// Function argument infos.
        /// </summary>
        public FunctionArgumentInfo[] Args => args;

        /// <summary>
        /// Formulate string.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="format">string that contains the formatting, e.g. "X8"</param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public object Evaluate(ref FunctionEvaluationContext ctx, object format, object argument)
        {
            if (argument == null) return null;
            string formatStr = format?.ToString();
            if (formatStr != null && argument is IFormattable formattable) return formattable.ToString(formatStr, ctx.Culture);
            if (ctx.Culture.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter customFormatter_)
                return customFormatter_.Format(formatStr, argument, ctx.Culture);
            return ctx.Culture == null ? String.Format("{0:" + formatStr + "}", argument) : String.Format(ctx.Culture, "{0:" + formatStr + "}", argument);
        }
    }

    /// <summary>
    /// Adds padding to left or to right side of a string.
    /// </summary>
    public class AlignmentFunction : IFunction2
    {
        static FunctionArgumentInfo[] args = new FunctionArgumentInfo[] { new FunctionArgumentInfo { Name = "str", Type = typeof(string) }, new FunctionArgumentInfo { Name = "alignment", Type = typeof(int) } };
        private static AlignmentFunction instance = new AlignmentFunction();

        /// <summary>
        /// "Alignment" function
        /// </summary>
        public static AlignmentFunction Instance => instance;

        /// <summary>
        /// Function name
        /// </summary>
        public string Name => "Alignment";

        /// <summary>
        /// Function argument infos.
        /// </summary>
        public FunctionArgumentInfo[] Args => args;

        /// <summary>
        /// Add padding.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="str"></param>
        /// <param name="alignment">Int32 or Int64, negative value is padding to left of <paramref name="str"/>, positive value is padding to right of <paramref name="str"/></param>
        /// <returns></returns>
        public object Evaluate(ref FunctionEvaluationContext ctx, object str, object alignment)
        {
            String s = str?.ToString();
            if (s == null) return null;
            int a;
            if (alignment is Int32 i) a = i;
            else if (alignment is Int64 ii) a = (Int32)ii;
            else return s;

            if (a == 0) return s;
            if (a > 0)
            {
                if (s.Length >= a) return a;
                return s + new string(' ', a - s.Length);
            }
            {
                a = -a;
                if (s.Length <= a) return a;
                return new string(' ', a - s.Length) + s;
            }

        }
    }
}
