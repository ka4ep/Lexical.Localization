// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Preparsed formulation string. 
    /// 
    /// For example "Welcome, {0}!" is a formulation string. 
    /// When it's in parsed format the argument "{0}" is extracted and the string can be processed more efficiently.
    /// 
    /// <see cref="IFormulationString"/> is produced by <see cref="ILocalizationStringFormatParser"/>.
    /// </summary>
    public interface IFormulationString
    {
        /// <summary>
        /// Parse result. One of:
        /// <list type="table">
        /// <item><see cref="LocalizationStatus.FormulationErrorMalformed"/> if there is a problem in the stirng</item>
        /// <item><see cref="LocalizationStatus.FormulationOk"/> if formulation was parsed ok.</item>
        /// </list>
        /// </summary>
        LocalizationStatus Status { get; }

        /// <summary>
        /// Formulation string as it appears, for example "You received {plural:0} coin(s).".
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Formulation string as sequence of text and argument parts.
        /// </summary>
        IFormulationStringPart[] Parts { get; }

        /// <summary>
        /// Arguments ordered by argument index first, then occurance index.
        /// 
        /// If there are gaps in argument index, gaps are not included.
        /// E.g. "Value={0}, Description={5}" returns argument {0} first then {5}.
        /// 
        /// If same argument occurs twice, then ordered by occurance index.
        /// E.g. "Value={0,10}, x={0:X8}" returns first {0,10} then second {0:X8}.
        /// 
        /// They are parsed from <see cref="Text"/> by an <see cref="ILocalizationStringFormatParser"/>.
        /// </summary>
        IFormulationStringArgument[] Arguments { get; }

        /// <summary>
        /// (optional) Formatters to apply to the formulation string.
        /// Some asset files may enforce their own rules.
        /// 
        /// The formatter is requested for following interfaces (Depends on <see cref="ILocalizationResolver"/> implementation.)
        /// <list type="bullet">
        /// <item><see cref="ILocalizationArgumentFormatter"/></item>
        /// <item><see cref="ICustomFormatter"/></item>
        /// <item><see cref="IPluralityRuleMap"/></item>
        /// <item><see cref="IPluralityCategory"/></item>
        /// </list>
        /// 
        /// <see cref="ILocalizationResolver"/> combines format providers from asset and key.
        /// The format provider that comes from <see cref="IFormulationString"/> has the highest priority.
        /// </summary>
        IFormatProvider FormatProvider { get; }
    }

    /// <summary>
    /// Type of string part
    /// </summary>
    public enum FormulationStringPartKind
    {
        /// <summary>
        /// Text
        /// </summary>
        Text,

        /// <summary>
        /// Argument
        /// </summary>
        Argument
    }

    /// <summary>
    /// A part in formulation string.
    /// </summary>
    public interface IFormulationStringPart
    {
        /// <summary>
        /// The 'parent' formulation string.
        /// </summary>
        IFormulationString FormulationString { get; }

        /// <summary>
        /// Part type.
        /// </summary>
        FormulationStringPartKind Kind { get; }

        /// <summary>
        /// Character index in the formulation string where argument starts.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Length of the character segment that defines argument.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The whole part as it appears in the formulation string.
        /// </summary>
        /// <returns></returns>
        string Text { get; }

        /// <summary>
        /// The index in the <see cref="IFormulationString.Parts"/>.
        /// </summary>
        int PartsIndex { get; }
    }

    /// <summary>
    /// Formulation of an argument e.g. "{[function:]0[,alignment][:format]}"
    /// </summary>
    public interface IFormulationStringArgument : IFormulationStringPart
    {
        /// <summary>
        /// Occurance index in formulation string.
        /// </summary>
        int OccuranceIndex { get; }

        /// <summary>
        /// If format is argument based, then this is the argument index, otherwise -1.
        /// Also index in <see cref="ILocalizationKeyFormatArgs.Args"/>.
        /// </summary>
        int ArgumentIndex { get; }

        /// <summary>
        /// Argument reference by index. -1 if arguments are not refered by index.
        /// The index correspondes to index in <see cref="IFormulationString.Arguments"/>.
        /// </summary>
        int ArgumentsIndex { get; }

        /// <summary>
        /// Argument reference by name, or null.
        /// </summary>
        string ArgumentName { get; }

        /// <summary>
        /// (Optional) The function name that is passed to <see cref="ILocalizationArgumentFormatter"/>.
        /// E.g. "plural", "optional", "range", "ordinal".
        /// </summary>
        string Function { get; }

        /// <summary>
        /// Alignment is an integer that defines field width. If negative then field is left-aligned, if positive then right-aligned.
        /// </summary>
        int Alignment { get; }

        /// <summary>
        /// (Optional) The format text that is passed to <see cref="ICustomFormatter"/>, <see cref="IFormattable"/> and <see cref="ILocalizationArgumentFormatter"/>.
        /// E.g. "x2".
        /// </summary>
        string Format { get; }

        /// <summary>
        /// (Optional) Default value, used if argument is not provided.
        /// </summary>
        string DefaultValue { get; }
    }

    /// <summary>
    /// Compares formulation strings
    /// </summary>
    public class FormulationStringComparer : IEqualityComparer<IFormulationString>, IComparer<IFormulationString>
    {
        private static FormulationStringComparer instance = new FormulationStringComparer(FormulationStringPartComparer.Instance, FormulationStringPartComparer.Instance);

        /// <summary>
        /// Default instance
        /// </summary>
        public static FormulationStringComparer Instance => instance;

        IEqualityComparer<IFormulationStringPart> partComparer;
        IComparer<IFormulationStringPart> partComparer2;

        /// <summary>
        /// Create part comparer
        /// </summary>
        /// <param name="partComparer"></param>
        /// <param name="partComparer2"></param>
        public FormulationStringComparer(IEqualityComparer<IFormulationStringPart> partComparer, IComparer<IFormulationStringPart> partComparer2)
        {
            this.partComparer = partComparer;
            this.partComparer2 = partComparer2;
        }

        /// <summary>
        /// Compare formulation strings for sorting order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1, 0, 1</returns>
        public int Compare(IFormulationString x, IFormulationString y)
        {
            string _x = x?.Text, _y = y?.Text;
            if (_x == null && _y == null) return 0;
            if (_x == null) return -1;
            if (_y == null) return 1;
            return _x.CompareTo(_y);
        }

        /// <summary>
        /// Compare formulation strings for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IFormulationString x, IFormulationString y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x == y) return true;
            if (x.Status != y.Status) return false;
            var x_parts = x.Parts;
            var y_parts = y.Parts;
            if (x_parts.Length != y_parts.Length) return false;
            int c = x_parts.Length;
            for (int i = 0; i < c; i++)
                if (!partComparer.Equals(x_parts[i], y_parts[i])) return false;
            return true;
        }

        /// <summary>
        /// Calculate hashcode.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int GetHashCode(IFormulationString o)
        {
            if (o == null) return 0;
            int result = FNVHashBasis;

            // Hash Status
            result ^= (int) ((ulong)(o.Status & LocalizationStatus.FormulationMask) >> Shift.Formulation);
            result *= FNVHashPrime;

            // Hash FormatProvider
            if (o.FormatProvider != null) { result ^= o.FormatProvider.GetHashCode(); result *= FNVHashPrime; }

            // Hash Parts
            foreach (var part in o.Parts)
            {
                result ^= partComparer.GetHashCode(part);
                result *= FNVHashPrime;
            }
            return result;
        }
        
        const int FNVHashBasis = unchecked((int)2166136261);
        const int FNVHashPrime = 16777619;
    }

    /// <summary>
    /// Compares formulation strings
    /// </summary>
    public class FormulationStringPartComparer : IEqualityComparer<IFormulationStringPart>, IComparer<IFormulationStringPart>
    {
        private static FormulationStringPartComparer instance = new FormulationStringPartComparer();

        /// <summary>
        /// Default instance
        /// </summary>
        public static FormulationStringPartComparer Instance => instance;

        /// <summary>
        /// Compare formulation string parts for sorting order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1, 0, 1</returns>
        public int Compare(IFormulationStringPart x, IFormulationStringPart y)
        {
            string _x = x?.Text, _y = y?.Text;
            if (_x == null && _y == null) return 0;
            if (_x == null) return -1;
            if (_y == null) return 1;
            return _x.CompareTo(_y);
        }

        /// <summary>
        /// Compare formulation strings for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IFormulationStringPart x, IFormulationStringPart y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x == y) return true;
            if (x.Kind != y.Kind) return false;

            if (x.Kind == FormulationStringPartKind.Text)
            {
                if (x.Text != y.Text) return false;
            }

            if (x.Kind == FormulationStringPartKind.Argument)
            {
                var x_arg = x as IFormulationStringArgument;
                var y_arg = y as IFormulationStringArgument;
                if (x_arg.ArgumentIndex != y_arg.ArgumentIndex) return false;
                if (x_arg.ArgumentName != y_arg.ArgumentName) return false;
                if (x_arg.DefaultValue == null && y_arg.DefaultValue == null) { }
                else if (x_arg.DefaultValue != null && y_arg.DefaultValue != null)
                {
                    if (!x_arg.Equals(y_arg)) return false;
                }
                else return false;
                if (x_arg.Alignment != y_arg.Alignment) return false;
                if (x_arg.Format != y_arg.Format) return false;
                if (x_arg.Function != y_arg.Function) return false;
            }

            return true;
        }

        /// <summary>
        /// Calculate hashcode.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int GetHashCode(IFormulationStringPart o)
        {
            if (o == null) return 0;
            int result = FNVHashBasis;

            if (o.Kind == FormulationStringPartKind.Text)
            {
                result ^= o.Text.GetHashCode();
                result *= FNVHashPrime;
            }

            if (o.Kind == FormulationStringPartKind.Argument && o is IFormulationStringArgument arg)
            {
                if (arg.ArgumentIndex>=0) { result ^= arg.ArgumentIndex; result *= FNVHashPrime; }
                if (arg.ArgumentName != null) { result ^= arg.ArgumentName.GetHashCode(); result *= FNVHashPrime; }
                if (arg.DefaultValue != null) { result ^= arg.DefaultValue.GetHashCode(); result *= FNVHashPrime; }
                if (arg.Alignment != 0) { result ^= arg.Alignment; result *= FNVHashPrime; }
                if (arg.Format != null) { result ^= arg.Format.GetHashCode(); result *= FNVHashPrime; }
                if (arg.Function != null) { result ^= arg.Function.GetHashCode(); result *= FNVHashPrime; }
            }

            return result;
        }

        const int FNVHashBasis = unchecked((int)2166136261);
        const int FNVHashPrime = 16777619;
    }

}
