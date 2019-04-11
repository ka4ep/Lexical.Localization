// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Globalization;
using System.Numerics;

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
                        int _int32 = (int)Number;
                        return n = _int32 < 0 ? (-_int32) : Number;
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
            if (text == null) { Status = LocalizationStatus.PluralityErrorArgumentTextNull; return; }
            if (numberFormat == null) { Status = LocalizationStatus.PluralityErrorArgumentNumberFormatNull; return; }

            // Scan text
            bool isHex, isFloat, isInteger;
            switch (Number == null ? TypeCode.DBNull : Type.GetTypeCode(Number.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    isInteger = true;
                    isFloat = false;
                    isHex = false;
                    break;
                case TypeCode.Single:
                case TypeCode.Decimal:
                case TypeCode.Double:
                    isInteger = false;
                    isFloat = true;
                    isHex = false;
                    break;
                default:
                    isInteger = true;
                    isFloat = true;
                    isHex = false;
                    break;
            }

            Status = LocalizationStatus.PluralityOk;

            // No need to scan integer
            if (isInteger && !isFloat && Text != null)
            {
                i_start = 0; i_end = Text.Length;
                return;
            }

            ScanState state = ScanState.Zero;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '-') continue;
                if (ch == ' ') continue;
                if (isFloat && (ch == 'e' || ch == 'E')) { state = ScanState.Exponent; continue; }

                if (state == ScanState.Zero)
                {
                    if (ch == '0')
                    {
                        i_start = i;
                        if (i_end < i_start) i_end = i + 1;
                        continue;
                    }
                    else if (ch >= '1' && ch <= '9' || (isHex && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F'))))
                    {
                        if (i_start < 0) i_start = i;
                        i_end = i + 1;
                        state = ScanState.Integer;
                        continue;
                    }
                }
                else if (state == ScanState.Integer)
                {
                    if (ch >= '0' && ch <= '9' || (isHex && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F'))))
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
                    if (ch >= '0' && ch <= '9')
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
            if (t_start >= 0 && t_end >= 0)
                for (int i = t_start; i < t_end; i++)
                {
                    char ch = text[i];
                    if (ch >= '0' && ch <= '9') w++;
                }

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
            if (matchTo.Length > endIx + 1) return false;
            for (int i = matchTo.Length - 1; i >= 0 && endIx >= 0; i--, endIx--)
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
        public PluralityParameter this[char parameterName]
        {
            get
            {
                switch (parameterName)
                {
                    // Absolute number
                    case 'n':
                        bool isFloat = false;
                        switch (Number == null ? TypeCode.DBNull : Type.GetTypeCode(Number.GetType()))
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
                        switch (Number == null ? TypeCode.DBNull : Type.GetTypeCode(Number.GetType()))
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
        public bool InRange(ulong start, ulong end)
        {
            ulong value;
            return TryParseUInt64(out value) && value >= start && value <= end;
        }

        /// <summary>
        /// Try to parse Text to unsigned long. 
        /// 
        /// If value is decimal with fractions, e.g. "0.5" then returns false.
        /// However, if fractions are "0" then returns true.
        /// 
        /// If "Text" value is too large to fit ulong, then returns false.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryParseUInt64(out ulong value)
        {
            switch (Number == null ? TypeCode.DBNull : Type.GetTypeCode(Number.GetType()))
            {
                case TypeCode.Byte: value = (ulong)(byte)Number; return true;
                case TypeCode.UInt16: value = (ulong)(UInt16)Number; return true;
                case TypeCode.UInt32: value = (ulong)(UInt32)Number; return true;
                case TypeCode.UInt64: value = (ulong)(UInt64)Number; return true;
                case TypeCode.SByte: value = (ulong)(SByte)Number; return true;
                case TypeCode.Int16: value = (ulong)(Int16)Number; return true;
                case TypeCode.Int32: value = (ulong)(Int32)Number; return true;
                case TypeCode.Int64: value = (ulong)(Int64)Number; return true;
                case TypeCode.Decimal:
                    decimal _decimal = (Decimal)Number;
                    // Has fraction values -> false
                    if (Math.Truncate(_decimal) != _decimal) { value = 0; return false; }
                    if (_decimal < 0 || _decimal >= ulong.MaxValue) { value = 0; return false; }
                    value = (ulong)_decimal;
                    return true;
                case TypeCode.Single:
                    Single _single = (Single)Number;
                    // Has fraction values -> false
                    if (Math.Truncate(_single) != _single) { value = 0; return false; }
                    if (_single < 0 || _single >= ulong.MaxValue) { value = 0; return false; }
                    value = (ulong)_single;
                    return true;
                case TypeCode.Double:
                    Double _double = (Double)Number;
                    // Has fraction values -> false
                    if (Math.Truncate(_double) != _double) { value = 0; return false; }
                    if (_double < 0 || _double >= ulong.MaxValue) { value = 0; return false; }
                    value = (ulong)_double;
                    return true;
            }

            // Value is ""
            if (Text != null && startIx >= 0 && endIx == startIx) { value = 0; return true; }

            if (Text == null || startIx < 0 || endIx < 0 || endIx <= startIx) { value = 0; return false; }

            // Skip preceding '0's
            int _startix = startIx;
            while (_startix < endIx && Text[_startix] == '0') _startix++;

            // Parse
            ulong result = 0UL;
            for (int i = endIx - 1; i >= startIx; i--)
            {
                char ch = Text[i];
                if (ch >= '0' && ch <= '9')
                {
                    if (result >= ULongTenthOfMax) { value = 0UL; return false; }
                    ulong x = (ulong)(ch - '0');
                    result = result * 10UL + x;
                }
            }
            value = result;
            return true;
        }

        /// <summary>
        /// Try to parse Text to decimal integer. 
        /// 
        /// If value is decimal with fractions, e.g. "0.5" then returns false.
        /// However, if fractions are "0" then returns true.
        /// 
        /// If "Text" value is too large to fit ulong, then returns false.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryParseDecimal(out decimal value)
        {
            switch (Number == null ? TypeCode.DBNull : Type.GetTypeCode(Number.GetType()))
            {
                case TypeCode.Byte: value = (decimal)(byte)Number; return true;
                case TypeCode.UInt16: value = (decimal)(UInt16)Number; return true;
                case TypeCode.UInt32: value = (decimal)(UInt32)Number; return true;
                case TypeCode.UInt64: value = (decimal)(UInt64)Number; return true;
                case TypeCode.SByte: value = (decimal)(SByte)Number; return true;
                case TypeCode.Int16: value = (decimal)(Int16)Number; return true;
                case TypeCode.Int32: value = (decimal)(Int32)Number; return true;
                case TypeCode.Int64: value = (decimal)(Int64)Number; return true;
                case TypeCode.Decimal:
                    decimal _decimal = (Decimal)Number;
                    // Has fraction values -> false
                    if (Math.Truncate(_decimal) != _decimal) { value = 0m; return false; }
                    if (_decimal < 0m || _decimal >= decimal.MaxValue) { value = 0m; return false; }
                    value = _decimal;
                    return true;
                case TypeCode.Single:
                    decimal _single = (decimal)(Single)Number;
                    // Has fraction values -> false
                    if (Math.Truncate(_single) != _single) { value = 0m; return false; }
                    if (_single < 0m || _single >= decimal.MaxValue) { value = 0m; return false; }
                    value = _single;
                    return true;
                case TypeCode.Double:
                    decimal _double = (decimal)(Double)Number;
                    // Has fraction values -> false
                    if (Math.Truncate(_double) != _double) { value = 0m; return false; }
                    if (_double < 0m || _double >= decimal.MaxValue) { value = 0m; return false; }
                    value = _double;
                    return true;
            }

            // Value is ""
            if (Text != null && startIx >= 0 && endIx == startIx) { value = 0m; return true; }

            if (Text == null || startIx < 0 || endIx < 0 || endIx <= startIx) { value = 0m; return false; }

            // Skip preceding '0's
            int _startix = startIx;
            while (_startix < endIx && Text[_startix] == '0') _startix++;

            // Parse
            decimal result = 0m;
            for (int i = endIx - 1; i >= startIx; i--)
            {
                char ch = Text[i];
                if (ch >= '0' && ch <= '9')
                {
                    if (result >= DecimalTenthOfMax) { value = 0m; return false; }
                    decimal x = (decimal)(ch - '0');
                    result = result * 10m + x;
                }
            }
            value = result;
            return true;
        }

        const ulong ULongTenthOfMax = ulong.MaxValue / 10UL;
        static readonly decimal DecimalTenthOfMax = Math.Truncate(decimal.MaxValue / 10m);

        /// <summary>
        /// Is value equal to an integer value.
        /// 
        /// For example if the contained Number = "4.0" and <paramref name="value"/> is 4, then result is true.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsEqual(ulong value)
        {
            ulong _value;
            return TryParseUInt64(out _value) && value == _value;
        }

        /// <summary>
        /// Is value in <paramref name="group"/>.
        /// 
        /// For example, if the contained Number = "4.0" and <paramref name="group"/> has values [2, 4, 6], then result is true.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool InGroup(ulong[] group)
        {
            ulong _value;
            if (TryParseUInt64(out _value))
                foreach (ulong value in group)
                    if (value == _value) return true;
            return false;
        }

        /// <summary>
        /// Try get modulo of value as integer
        /// </summary>
        /// <param name="modulo">the divider</param>
        /// <param name="reminder">the reminder</param>
        /// <returns></returns>
        public bool ModuloInteger(ulong modulo, out ulong reminder)
        {
            ulong _value;
            if (TryParseUInt64(out _value)) { reminder = _value % modulo; return true; }

            if (Text != null)
            {
                // Get last digit
                if (modulo == 10UL && TryGetLastDigits(1, out _value)) { reminder = _value; return true; }
                if (modulo == 100UL && TryGetLastDigits(2, out _value)) { reminder = _value; return true; }
                if (modulo == 1000UL && TryGetLastDigits(3, out _value)) { reminder = _value; return true; }
                if (modulo == 10000UL && TryGetLastDigits(4, out _value)) { reminder = _value; return true; }
                if (modulo == 100000UL && TryGetLastDigits(5, out _value)) { reminder = _value; return true; }
                if (modulo == 1000000UL && TryGetLastDigits(6, out _value)) { reminder = _value; return true; }
                if (modulo == 10000000UL && TryGetLastDigits(7, out _value)) { reminder = _value; return true; }
                if (modulo == 100000000UL && TryGetLastDigits(8, out _value)) { reminder = _value; return true; }
                if (modulo == 1000000000UL && TryGetLastDigits(9, out _value)) { reminder = _value; return true; }
                if (modulo == 10000000000UL && TryGetLastDigits(10, out _value)) { reminder = _value; return true; }
                if (modulo == 100000000000UL && TryGetLastDigits(11, out _value)) { reminder = _value; return true; }
                if (modulo == 1000000000000UL && TryGetLastDigits(12, out _value)) { reminder = _value; return true; }
                if (modulo == 10000000000000UL && TryGetLastDigits(13, out _value)) { reminder = _value; return true; }
                if (modulo == 100000000000000UL && TryGetLastDigits(14, out _value)) { reminder = _value; return true; }
                if (modulo == 1000000000000000UL && TryGetLastDigits(15, out _value)) { reminder = _value; return true; }
                if (modulo == 10000000000000000UL && TryGetLastDigits(16, out _value)) { reminder = _value; return true; }
                if (modulo == 100000000000000000UL && TryGetLastDigits(17, out _value)) { reminder = _value; return true; }
                if (modulo == 1000000000000000000UL && TryGetLastDigits(18, out _value)) { reminder = _value; return true; }
                if (modulo == 10000000000000000000UL && TryGetLastDigits(19, out _value)) { reminder = _value; return true; }
            }

            decimal _decimal;
            if (TryParseDecimal(out _decimal))
            {
                try
                {
                    reminder = checked((ulong)(_decimal % (decimal)modulo));
                    return true;
                }
                catch (Exception)
                {
                }
            }

            if (Text != null && startIx >= 0 && endIx >= 0 && startIx < endIx)
            {
                try
                {
                    BigInteger big = BigInteger.Parse(Text.Substring(startIx, endIx - startIx));
                    BigInteger _modulo = new BigInteger(modulo);
                    BigInteger _reminder = big % modulo;
                    reminder = (ulong)_reminder;
                    return true;
                }
                catch (Exception)
                {
                }
            }

            reminder = 0UL;
            return false;
        }

        /// <summary>
        /// Try get last digits
        /// </summary>
        /// <param name="count"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetLastDigits(int count, out ulong value)
        {

            if (Text == null || startIx < 0 || endIx < 0 || endIx <= startIx) { value = 0; return false; }

            ulong result = 0UL;
            for (int i = endIx - 1; count > 0 && i >= startIx; i--)
            {
                char ch = Text[i];
                if (ch >= '0' && ch <= '9')
                {
                    if (result >= ULongTenthOfMax) { value = 0UL; return false; }
                    ulong x = (ulong)(ch - '0');
                    result = result * 10UL + x;
                    count--;
                }
            }
            value = result;
            return true;
        }

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Text != null) return Text;
            if (Number != null) return Number.ToString();
            return "";
        }

    }

}
