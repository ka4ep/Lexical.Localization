// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Globalization;

namespace Lexical.Localization.Unicode
{
    /// <summary>
    /// Breakdown of a number argument into parameters that are usable for plurality estimation.
    /// </summary>
    public struct PluralFeatures
    {
        /// <summary>
        /// Source number as object
        /// </summary>
        public object Number;

        /// <summary>
        /// The number in text format
        /// </summary>
        public string Text;

        /// <summary>
        /// Absolute value of the source number.
        /// </summary>
        object n;

        /// <summary>
        /// Absolute value of the source number (boxed).
        /// </summary>
        public object N
        {
            get
            {
                if (Number == null) return null;
                if (n != null) return n;
                switch (Type.GetTypeCode(Number.GetType()))
                {
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return n = Number;
                    case TypeCode.SByte:
                        sbyte _sbyte = (sbyte)Number;
                        return n = _sbyte < 0 ? (-_sbyte) : Number;
                    case TypeCode.Decimal:
                        decimal _decimal = (decimal)Number;
                        return n = _decimal < 0 ? (-_decimal) : Number;
                    case TypeCode.Int16:
                        Int16 _int16 = (Int16)Number;
                        return n = _int16 < 0 ? (-_int16) : Number;
                    case TypeCode.Int32:
                        int _int = (int)Number;
                        return n = _int < 0 ? (-_int) : Number;
                    case TypeCode.Int64:
                        Int64 _int64 = (Int64)Number;
                        return n = _int64 < 0 ? (-_int64) : Number;
                    case TypeCode.Single:
                        Single _single = (Single)Number;
                        return n = _single < 0 ? (-_single) : Number;
                    case TypeCode.Double:
                        Double _double = (Double)Number;
                        return n = _double < 0 ? (-_double) : Number;
                    default:
                        if (Status < LocalizationStatus.PluralityErrorArgumentNotNumber) Status = LocalizationStatus.PluralityErrorArgumentNotNumber;
                        return Number;
                }
            }
        }

        /// <summary>
        /// Convert <paramref name="n"/> to same type as <see cref="Number"/>.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public object Convert(object n)
            => System.Convert.ChangeType(n, Number.GetType());

        /// <summary>
        /// Status code for the operations. A value within <see cref="LocalizationStatus.PluralityMask"/>.
        /// </summary>
        public LocalizationStatus Status;

        /// <summary>
        /// Integer digits of <see cref="Text"/>. Trailing zeros are excluded.
        /// 
        /// Both values are -1, if integer digits are not detected.
        /// 
        /// For example: n=1.230, i=1
        /// </summary>
        public int i_start, i_end;

        /// <summary>
        /// Number of visible fraction digits of <see cref="Text"/> , with trailing zeroes.
        /// 
        /// For example: n=1.230, v=3
        /// </summary>
        public int v;

        /// <summary>
        /// Number of visible fraction digits of <see cref="Text"/>, without trailing zeros.
        /// 
        /// For example: n=1.230, w=23
        /// </summary>
        public int w;

        /// <summary>
        /// Visible fractional digit characters of <see cref="Text"/>, with trailing zeros.
        /// 
        /// Both values are -1, if fraction digits are not detected.
        /// 
        /// For example: n=1.230, f=230
        /// </summary>
        public int f_start, f_end;

        /// <summary>
        /// Visible fractional digit characters of <see cref="Text"/>, without trailing zeros.
        /// 
        /// Both values are -1, if fraction digits are not detected.
        /// 
        /// For example: n=1.230, t=23
        /// </summary>
        public int t_start, t_end;

        /// <summary>
        /// Extract characteristics from a printed numbers.
        /// </summary>
        /// <param name="number">(optional) number argument</param>
        /// <param name="text">text representatio of the number</param>
        /// <param name="numberFormat">format that contains decimal separators</param>
        public PluralFeatures(Object number, string text, NumberFormatInfo numberFormat)
        {
            this.Number = number;
            Text = text;
            n = null;
            i_start = -1; i_end = -1;
            f_start = -1; f_end = -1;
            t_start = -1; t_end = -1;
            w = 0;
            v = 0;
            Status = LocalizationStatus.PluralityOk;

            // Null
            if (number == null) { Status = LocalizationStatus.PluralityErrorArgumentNull; /*Argument is not required as long as text is provided, keep going*/ }
            if (text == null) { Status = LocalizationStatus.PluralityErrorArgumentTextNull; return; }
            if (numberFormat == null) { Status = LocalizationStatus.PluralityErrorArgumentNumberFormatNull; return; }

            // Scan text
            bool isFloat = number is float || number is double || number is decimal;
            ScanState state = ScanState.Zero;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '-') continue;
                if (ch == ' ') continue;
                if (isFloat && (ch == 'e'||ch=='E')) { state = ScanState.Exponent; continue; }

                if (state == ScanState.Zero)
                {
                    if (ch == '0')
                    {
                        i_start = i;
                        if (i_end < i_start) i_end = i + 1;
                        continue;
                    }
                    else if (ch >= '1' && ch <= '9' || (!isFloat && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F'))))
                    {
                        if (i_start < 0) i_start = i;
                        i_end = i + 1;
                        state = ScanState.Integer;
                        continue;
                    }
                } else if (state == ScanState.Integer)
                {
                    if (ch >= '0' && ch <= '9' || (!isFloat && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F'))))
                    {
                        if (i_start < 0) i_start = i;
                        i_end = i + 1;
                        continue;
                    }
                }

                if (state == ScanState.Zero || state == ScanState.Integer)
                { 
                    // ',' or '.' decimal separator
                    if (isFloat && (MatchString(i, numberFormat.NumberDecimalSeparator) || MatchString(i, numberFormat.PercentDecimalSeparator) || MatchString(i, numberFormat.CurrencyDecimalSeparator))) // <- this may cause problems
                    {
                        state = ScanState.Fraction;
                        f_start = i + 1; f_end = i + 1;
                        t_start = i + 1; t_end = i + 1;
                        continue;
                    }
                    continue;
                }

                if (state == ScanState.Fraction)
                {
                    if (ch >= '0' && ch <='9')
                    {
                        if (f_start < 0) f_start = i;
                        f_end = i + 1;
                        v++;
                    }

                    if (ch >= '1' && ch <= '9')
                    {
                        if (t_start < 0) t_start = i;
                        t_end = i + 1;
                    }
                    continue;
                }
            }

            // Count 'w', number of fraction digits without trailing zeroes
            if (t_start>=0 && t_end>=0)
                for (int i=t_start; i<t_end; i++)
                {
                    char ch = text[i];
                    if (ch >= '0' && ch <= '9') w++;
                }

            Status = LocalizationStatus.PluralityOk;
        }

        /// <summary>
        /// Compares whether a substring of <see cref="Text"/> matches to <paramref name="matchTo"/>, when 
        /// compare starts at end of <paramref name="endIx"/> (including <paramref name="endIx"/>).
        /// </summary>
        /// <param name="endIx"></param>
        /// <param name="matchTo"></param>
        /// <returns></returns>
        bool MatchString(int endIx, string matchTo)
        {
            if (matchTo.Length > endIx+1) return false;
            for (int i = matchTo.Length - 1; i >= 0 && endIx>=0; i--, endIx--)
                if (Text[endIx] != matchTo[i]) return false;
            return true;
        }

        enum ScanState
        {
            Zero,
            Integer,
            Fraction,
            Exponent
        }

        /// <summary>
        /// Get parameter <paramref name="parameterName"/>.
        /// </summary>
        /// <param name="parameterName">One of 'n', 'i', 'f', 'v', 't', 'w'</param>
        /// <returns></returns>
        public PluralityParameter GetParameter(char parameterName)
        {
            switch(parameterName)
            {
                // Absolute number
                case 'n':
                    bool isFloat = false;
                    switch (Type.GetTypeCode(Number.GetType()))
                    {
                        case TypeCode.Single:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                            isFloat = true;
                            break;
                    }

                    if (Text == null) return new PluralityParameter { Number = N, Text = Text, startIx = -1, endIx = -1, isFloat = isFloat };
                    return new PluralityParameter { Number = N, Text = Text, startIx = 0, endIx = Text.Length, isFloat = isFloat };

                // Integer digits
                case 'i':
                    if (Number == null) return new PluralityParameter { Number = null, Text = Text, startIx = i_start, endIx = i_end, isFloat = false };
                    switch (Type.GetTypeCode(Number.GetType()))
                    {
                        case TypeCode.Byte:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                            return new PluralityParameter { Number = N, Text = Text, startIx = i_start, endIx = i_end, isFloat = false };
                        case TypeCode.Single:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        default:
                            return new PluralityParameter { Number = null, Text = Text, startIx = i_start, endIx = i_end, isFloat = false };
                    }

                // Visible fractional digit characters of <see cref="Text"/>, with trailing zeros.
                case 'f': return new PluralityParameter { Number = null, Text = Text, startIx = f_start, endIx = f_end, isFloat = false };
                // Visible fractional digit characters of <see cref="Text"/>, without trailing zeros.
                case 't': return new PluralityParameter { Number = null, Text = Text, startIx = t_start, endIx = t_end, isFloat = false };
                // Number of visible fraction digits of <see cref="Text"/> , with trailing zeroes.
                case 'v': return new PluralityParameter { Number = v, Text = null, startIx = -1, endIx = -1 };
                // Number of visible fraction digits of <see cref="Text"/> , without trailing zeroes.
                case 'w': return new PluralityParameter { Number = w, Text = null, startIx = -1, endIx = -1 };
                // error
                default: 
                    return new PluralityParameter { Number = null, Text = null, startIx = -1, endIx = -1, isFloat = false };
            }
        }
    }

    /// <summary>
    /// Parameter values
    /// </summary>
    public struct PluralityParameter
    {
        /// <summary>
        /// (optional) Absolute number
        /// </summary>
        public object Number;

        /// <summary>
        /// The number in text format
        /// </summary>
        public string Text;

        /// <summary>
        /// Start index in <see cref="Text"/>, -1 if unavailable.
        /// </summary>
        public int startIx;

        /// <summary>
        /// End index in <see cref="Text"/>, -1 if unavailable.
        /// </summary>
        public int endIx;

        /// <summary>
        /// Is floating-point value.
        /// </summary>
        public bool isFloat;

        /// <summary>
        /// Is value available.
        /// </summary>
        public bool HasValue => (Number != null) || (Text != null && startIx >= 0 && endIx >= 0);

        /// <summary>
        /// Is value in integer range.
        /// </summary>
        /// <param name="start">Start of range (inclusive)</param>
        /// <param name="end">End of range (inclusive)</param>
        /// <returns></returns>
        public bool InRange(int start, int end)
        {
            // TODO 
            return false;
        }

        /// <summary>
        /// Is value equal to an integer value.
        /// 
        /// For example if the contained Number = "4.0" and <paramref name="value"/> is 4, then result is true.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsEqual(int value)
        {
            // TODO 
            return false;
        }

        /// <summary>
        /// Is value in <paramref name="group"/>.
        /// 
        /// For example, if the contained Number = "4.0" and <paramref name="group"/> has values [2, 4, 6], then result is true.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool InGroup(int[] group)
        {
            // TODO 
            return false;
        }

        /// <summary>
        /// Get modulo of value as integer
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public int ModuloInteger(int modulo)
        {
            // TODO 
            return -1;
        }

        /// <summary>
        /// Get module of value as floating point.
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public double ModuloFloat(int modulo)
        {
            // TODO 
            return - 1d;
        }

    }

}
