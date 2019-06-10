// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Default functions that are imported automatically.
    /// <list type="bullet">
    /// <item><see cref="FormatFunction"/></item>
    /// <item><see cref="AlignmentFunction"/></item>
    /// </list>
    /// </summary>
    public class Functions : FunctionsMap
    {
        private static IFunctions instance = 
            new FunctionsMap()
            .Add(FormatFunction.Default)
            .Add(AlignmentFunction.Default);

        /// <summary>
        /// Table of default functions.
        /// </summary>
        public static IFunctions Default => instance;
    }

    /// <summary>
    /// Function that formats 
    /// </summary>
    public class FormatFunction : IFunction2
    {
        static FunctionArgumentInfo[] args = new FunctionArgumentInfo[] {
            new FunctionArgumentInfo { Name = "argument", Type = typeof(object) },
            new FunctionArgumentInfo { Name = "format", Type = typeof(string) }
        };
        private static FormatFunction instance = new FormatFunction();

        /// <summary>
        /// "Format" function
        /// </summary>
        public static FormatFunction Default => instance;

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
        /// <param name="argument"></param>
        /// <param name="format">string that contains the formatting, e.g. "X8"</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryEvaluate(ref FunctionEvaluationContext ctx, object argument, object format, out object result)
        {
            if (argument == null) { result = null; return false; }
            string formatStr = format?.ToString();

            // Try custom format provider
            if (ctx.FormatProvider != null) {
                ICustomFormatter customFormatter__ = ctx.FormatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
                if (customFormatter__ != null)
                {
                    string custom_formatter_result = customFormatter__.Format(formatStr, argument, ctx.Culture);
                    if (custom_formatter_result != null) { result = custom_formatter_result; return true; }
                }
            }

            if (argument is Enum @enum)
            {
                if (format == null || "".Equals(format) || "g".Equals(format) || "G".Equals(format) || "f".Equals(format) || "F".Equals(format) || "|".Equals(format))
                {
                    string separator = ", ";
                    if ("|".Equals(format)) separator = "|";
                    LineString enum_string = ctx.EvaluateEnum(@enum, separator);
                    if (enum_string.Value != null) { result = enum_string.Value; return true; }
                }
            }

            if (formatStr != null && argument is IFormattable formattable) { result = formattable.ToString(formatStr, ctx.Culture); return true; }
            if (ctx.Culture.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter customFormatter_) { result = customFormatter_.Format(formatStr, argument, ctx.Culture); return true; }
            result = ctx.Culture == null ? String.Format("{0:" + formatStr + "}", argument) : String.Format(ctx.Culture, "{0:" + formatStr + "}", argument);
            return true;
        }
    }

    /// <summary>
    /// Adds padding to left or to right side of a string.
    /// </summary>
    public class AlignmentFunction : IFunction2
    {
        static FunctionArgumentInfo[] args = new FunctionArgumentInfo[] {
            new FunctionArgumentInfo { Name = "argument", Type = typeof(object) },
            new FunctionArgumentInfo { Name = "alignment", Type = typeof(int) }
        };
        private static AlignmentFunction instance = new AlignmentFunction();

        /// <summary>
        /// "Alignment" function
        /// </summary>
        public static AlignmentFunction Default => instance;

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
        /// <param name="argument"></param>
        /// <param name="alignment">Int32 or Int64, negative value is padding to left of <paramref name="argument"/>, positive value is padding to right of <paramref name="argument"/></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryEvaluate(ref FunctionEvaluationContext ctx, object argument, object alignment, out object result)
        {
            String s = argument?.ToString();
            if (s == null) { result = null; return false; }
            int a;
            if (alignment is Int32 i) a = i;
            else if (alignment is Int64 ii) a = (Int32)ii;
            else { result = s; return true; }

            if (a == 0) { result = s; return true; }
            if (a < 0)
            {
                a = -a;
                if (s.Length >= a) { result = s; return true; }
                result = s + new string(' ', a - s.Length);
                return true;
            }
            {
                if (s.Length >= a) { result = s; return true; }
                result = new string(' ', a - s.Length) + s;
                return true;
            }

        }
    }
}
