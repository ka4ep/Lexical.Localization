// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Table of default functions.
    /// </summary>
    public class Functions : FunctionsTable
    {
        private static IFunctions instance = new FunctionsTable().Add(FormatFunction.Instance).Add(AlignmentFunction.Instance);

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
        public FunctionArgumentInfo[] Args => throw new NotImplementedException();

        /// <summary>
        /// Formulate string.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="format">string that contains the formatting, e.g. "X8"</param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public object Evaluate(ref StringFormatEvaluationContext ctx, object format, object argument)
        {
            return String.Format(format?.ToString(), argument);
        }
    }

    /// <summary>
    /// Adds padding to left or to right side of a string.
    /// </summary>
    public class AlignmentFunction : IFunction1
    {
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
        public FunctionArgumentInfo[] Args => throw new NotImplementedException();

        /// <summary>
        /// Add padding.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public object Evaluate(ref StringFormatEvaluationContext ctx, object alignment)
        {
            throw new NotImplementedException();
        }
    }
}
