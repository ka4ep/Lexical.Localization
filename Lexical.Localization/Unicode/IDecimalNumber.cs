using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization.Plural
{
    /// <summary>
    /// Interface for decimal numbers.
    /// </summary>
    public interface IDecimalNumber
    {
        /// <summary>
        /// True if has non-empty value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// True if has decimal fractions. For example "0.10" is true, but "0.00" is false
        /// </summary>
        bool IsFloat { get; }

        /// <summary>
        /// Sign of the value, -1, 0 or 1.
        /// </summary>
        int Sign { get; }

        /// <summary>
        /// Try to read into long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out long value);

        /// <summary>
        /// Try to read into decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out decimal value);

        /// <summary>
        /// Try to read into double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out double value);

        /// <summary>
        /// Try to read into big integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out System.Numerics.BigInteger value);

        /// <summary>
        /// Calculate modulo
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns>modulo or null if failed to calculate modulo</returns>
        IDecimalNumber Modulo(int modulo);

        /// <summary>
        /// Absolute (positive) value of the source number.
        /// </summary>
        IDecimalNumber N { get; }

        /// <summary>
        /// Integer digits. 
        /// </summary>
        IDecimalNumber I { get; }

        /// <summary>
        /// Visible fractional digit characters, with trailing zeros.
        /// </summary>
        IDecimalNumber F { get; }

        /// <summary>
        /// Visible fractional digit characters, without trailing zeros.
        /// </summary>
        IDecimalNumber T { get; }

        /// <summary>
        /// Number of visible fraction digits, with trailing zeroes.
        /// </summary>
        IDecimalNumber V { get; }

        /// <summary>
        /// Number of visible fraction digits, without trailing zeros.
        /// </summary>
        IDecimalNumber W { get; }

        /// <summary>
        /// Integer part decimal (0-9) digits starting from most significant (left)
        /// </summary>
        IEnumerable<byte> I_Digits { get; }

        /// <summary>
        /// Fraction part decimal (0-9) digits starting from most significant (left).
        /// </summary>
        IEnumerable<byte> F_Digits { get; }

        /// <summary>
        /// Exponent digits (0-9) starting from most significant (left).
        /// </summary>
        IEnumerable<byte> E_Digits { get; }
    }

    /// <summary>
    /// Extension methods for <see cref="IDecimalNumber"/>.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Tests if <paramref name="number"/> exists in <paramref name="group"/>.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool InGroup(this IDecimalNumber number, params IDecimalNumber[] group)
        {
            foreach (var value in group)
                if (number.Equals(value)) return true;
            return false;
        }
    }

    /// <summary>
    /// Decimal number
    /// </summary>
    public abstract class DecimalNumber : IDecimalNumber
    {
        /// <summary>
        /// Zero constant
        /// </summary>
        public static readonly IDecimalNumber _0 = new Long(0L);

        /// <summary>
        /// Empty value constant
        /// </summary>
        public static readonly IDecimalNumber Empty = new Text("", 0, 0, CultureInfo.InvariantCulture);

        /// <summary>
        /// Test if has value (is not empty)
        /// </summary>
        public abstract bool HasValue { get; }

        /// <summary>
        /// Test if has fractions. e.g. "0.01" = true, but "0.00" = false.
        /// </summary>
        public abstract bool IsFloat { get; }

        /// <summary>
        /// Sign, -1=negative, 1=positive, 0=zero.
        /// </summary>
        public abstract int Sign { get; }

        /// <summary>
        /// Absolute positive value.
        /// </summary>
        public abstract IDecimalNumber N { get; }

        /// <summary>
        /// Integer value digits. (positive, no fractions)
        /// </summary>
        public abstract IDecimalNumber I { get; }

        /// <summary>
        /// Fraction digits with trailing zeroes.
        /// </summary>
        public abstract IDecimalNumber F { get; }

        /// <summary>
        /// Fraction digits without trailing zeroes.
        /// </summary>
        public abstract IDecimalNumber T { get; }

        /// <summary>
        /// Number of visible fraction digits, with trailing zeroes.
        /// </summary>
        public abstract IDecimalNumber V { get; }

        /// <summary>
        /// Number of visible fraction digits, without trailing zeroes.
        /// </summary>
        public abstract IDecimalNumber W { get; }

        public IEnumerable<byte> I_Digits => throw new NotImplementedException();
        public IEnumerable<byte> F_Digits => throw new NotImplementedException();
        public IEnumerable<byte> E_Digits => throw new NotImplementedException();

        /// <summary>
        /// Calculate modulo.
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public abstract IDecimalNumber Modulo(int modulo);

        /// <summary>
        /// Get value as long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGet(out long value);

        /// <summary>
        /// Get value as decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGet(out decimal value);

        /// <summary>
        /// Get value as double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGet(out double value);

        /// <summary>
        /// Get value as BigInteger.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGet(out System.Numerics.BigInteger value);

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IDecimalNumber other)
            {
                // Compare floating points
                if (IsFloat || other.IsFloat)
                {
                    // Compare double
                    double _da, _db;
                    if (TryGet(out _da) && other.TryGet(out _db)) return _da == _db;
                    // Compare decimal
                    decimal da, db;
                    if (TryGet(out da) && other.TryGet(out db)) return da == db;

                    // Compare integers and fractions
                    if (Sign != other.Sign) return false;
                    return I.Equals(other.I) && F.Equals(other.F);
                }

                {
                    // Compare long
                    long la, lb;
                    if (TryGet(out la) && other.TryGet(out lb)) return la == lb;
                    // Compare decimal
                    decimal da, db;
                    if (TryGet(out da) && other.TryGet(out db)) return da == db;
                    // Compare big integers
                    System.Numerics.BigInteger ba, bb;
                    if (TryGet(out ba) && other.TryGet(out bb)) return ba == bb;
                }
            }

            return false;
        }

        /// <summary>
        /// Long value
        /// </summary>
        public class Long : DecimalNumber
        {
            /// <summary>
            /// Long value.
            /// </summary>
            public readonly long Value;

            /// <summary>
            /// Create long value
            /// </summary>
            /// <param name="value"></param>
            public Long(long value)
            {
                Value = value;
            }

            /// <inheritdoc />
            public override bool HasValue => true;
            /// <inheritdoc />
            public override bool IsFloat => false;
            /// <inheritdoc />
            public override IDecimalNumber N => Value<0l ? new Long(-Value) : this;
            /// <inheritdoc />
            public override IDecimalNumber I => this;
            /// <inheritdoc />
            public override IDecimalNumber F => Empty;
            /// <inheritdoc />
            public override IDecimalNumber T => Empty;
            /// <inheritdoc />
            public override IDecimalNumber V => _0;
            /// <inheritdoc />
            public override IDecimalNumber W => _0;
            /// <inheritdoc />
            public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
            /// <inheritdoc />
            public override IDecimalNumber Modulo(int modulo)
                => new Long(Value % modulo);
            /// <inheritdoc />
            public override bool TryGet(out long value)
            {
                value = Value;
                return true;
            }
            /// <inheritdoc />
            public override bool TryGet(out decimal value)
            {
                value = Value;
                return true;
            }
            /// <inheritdoc />
            public override bool TryGet(out double value)
            {
                double _doubleValue = unchecked(Value);
                long _backTo = unchecked((long)_doubleValue);
                if (_backTo != Value) { value = default; return false; }
                value = _doubleValue;
                return true;
            }
            /// <inheritdoc />
            public override bool TryGet(out System.Numerics.BigInteger value)
            {
                value = Value;
                return true;
            }
            /// <inheritdoc />
            public override int Sign => Value < 0L ? -1 : Value == 0L ? 0 : 1;
        }

        /// <summary>
        /// Double number
        /// </summary>
        public class Double : DecimalNumber
        {
            /// <summary>
            /// Double value.
            /// </summary>
            public readonly double Value;
            /// <inheritdoc />
            IDecimalNumber integer;
            /// <inheritdoc />
            IDecimalNumber fractions;
            /// <inheritdoc />
            double truncate;
            /// <summary>
            /// Create double number
            /// </summary>
            /// <param name="v"></param>
            public Double(double v)
            {
                this.Value = v;
                this.truncate = Math.Truncate(Value);
            }

            /// <inheritdoc />
            public IDecimalNumber Fractions
            {
                get
                {
                    if (fractions != null) return fractions;

                    double fractionDouble = Math.Abs(Value) - truncate;
                    string text = fractionDouble.ToString(CultureInfo.InvariantCulture);
                    return fractions = new Text(text, 0, text.Length, CultureInfo.InvariantCulture);
                }
            }
            /// <inheritdoc />
            public override bool HasValue => true;
            /// <inheritdoc />
            public override bool IsFloat => Value != truncate;
            /// <inheritdoc />
            public override int Sign => Value < 0d ? -1 : Value == 0d ? 0 : 1;
            /// <inheritdoc />
            public override IDecimalNumber N => Value < 0d ? new Double(-Value) : this;
            /// <inheritdoc />
            public override IDecimalNumber I => integer ?? (integer = new Double(Math.Abs(Math.Truncate(Value))));
            /// <inheritdoc />
            public override IDecimalNumber F => Fractions.F;
            /// <inheritdoc />
            public override IDecimalNumber T => Fractions.T;
            /// <inheritdoc />
            public override IDecimalNumber V => Fractions.V;
            /// <inheritdoc />
            public override IDecimalNumber W => Fractions.W;
            /// <inheritdoc />
            public override IDecimalNumber Modulo(int modulo)
                => new Double(Value % modulo);
            /// <inheritdoc />
            public override bool TryGet(out long value)
            {
                long _value = unchecked((long)Value);
                double _backTo = unchecked((double)_value);
                if (_backTo != Value) { value = default; return false; }
                value = _value;
                return true;
            }
            /// <inheritdoc />
            public override bool TryGet(out decimal value)
            {
                decimal _value = unchecked((decimal)Value);
                double _backTo = unchecked((double)_value);
                if (_backTo != Value) { value = default; return false; }
                value = _value;
                return true;
            }
            /// <inheritdoc />
            public override bool TryGet(out double value)
            {
                value = Value;
                return true;
            }
            /// <inheritdoc />
            public override bool TryGet(out System.Numerics.BigInteger value)
            {
                if (IsFloat) { value = default; return false; }
                value = (System.Numerics.BigInteger)Value;
                return true;
            }
        }

        /// <summary>
        /// Big integer number
        /// </summary>
        public class BigInteger : DecimalNumber
        {
            /// <summary>
            /// Big integer value
            /// </summary>
            public readonly System.Numerics.BigInteger Value;

            /// <summary>
            /// Create big integer number
            /// </summary>
            /// <param name="bigInteger"></param>
            public BigInteger(System.Numerics.BigInteger bigInteger)
            {
                this.Value = bigInteger;
            }

            /// <inheritdoc />
            public override bool HasValue => true;
            /// <inheritdoc />
            public override bool IsFloat => false;
            /// <inheritdoc />
            public override int Sign => Value.Sign;
            /// <inheritdoc />
            public override IDecimalNumber N => Value.Sign < 0 ? new BigInteger(-Value) : this;
            /// <inheritdoc />
            public override IDecimalNumber I => Value.Sign < 0 ? new BigInteger(-Value) : this;
            /// <inheritdoc />
            public override IDecimalNumber F => Empty;
            /// <inheritdoc />
            public override IDecimalNumber T => Empty;
            /// <inheritdoc />
            public override IDecimalNumber V => _0;
            /// <inheritdoc />
            public override IDecimalNumber W => _0;
            /// <inheritdoc />
            public override IDecimalNumber Modulo(int modulo)
                => new BigInteger(Value % modulo);
            /// <inheritdoc />
            public override bool TryGet(out long value)
            {
                if (Value >= long.MinValue && Value <= long.MaxValue) { value = (long)Value; return true; }
                value = default;
                return false;
            }
            /// <inheritdoc />
            public override bool TryGet(out decimal value)
            {
                if (Value >= MinDecimal && Value <= MaxDecimal) { value = (decimal)Value; return true; }
                value = default;
                return false;
            }

            static readonly System.Numerics.BigInteger MinDecimal = new System.Numerics.BigInteger(- 79228162514264337593543950335M);
            static readonly System.Numerics.BigInteger MaxDecimal = new System.Numerics.BigInteger(79228162514264337593543950335M);

            /// <inheritdoc />
            public override bool TryGet(out double value)
            {
                double _value = unchecked((double)Value);
                System.Numerics.BigInteger _backTo = unchecked((System.Numerics.BigInteger)_value);
                if (_backTo != Value) { value = default; return false; }
                value = _value;
                return true;
            }

            /// <inheritdoc />
            public override bool TryGet(out System.Numerics.BigInteger value)
            {
                value = Value;
                return true;
            }

            /// <inheritdoc />
            public override string ToString()
                => Value.ToString();
        }

        /// <summary>
        /// Text based number
        /// </summary>
        public class Text : DecimalNumber
        {
            /// <summary>
            /// The number in text format
            /// </summary>
            public readonly string String;

            /// <summary>
            /// Does text start with negative symbol.
            /// </summary>
            public readonly bool negative;

            /// <summary>
            /// Does text contain zero value.
            /// For example "0", "000", "00.000" or "-00.00e00".
            /// </summary>
            public readonly bool zero;

            /// <summary>
            /// Value start and end.
            /// 
            /// Both values are -1, if integer digits are not detected.
            /// 
            /// For example: Value="-001.230", v_start = 0, v_end = 8.
            /// </summary>
            public readonly int s_start, s_end;

            /// <summary>
            /// Digits of <see cref="String"/> for positive value. Negative signa and preceding zeros are excluded.
            /// 
            /// Both values are -1, if integer digits are not detected.
            /// 
            /// For example: Value="-001.230", n_start = 3, n_end = 8.
            /// </summary>
            public readonly int n_start, n_end;

            /// <summary>
            /// Integer digits of <see cref="String"/>. Preceding zeros are excluded.
            /// 
            /// Both values are -1, if integer digits are not detected.
            /// 
            /// For example: Value="10.230", i_start=0, i_end=2
            /// </summary>
            public readonly int i_start, i_end;

            /// <summary>
            /// Exponent digits of <see cref="String"/>. Preceding zeros are excluded.
            /// 
            /// Both values are -1, if integer digits are not detected.
            /// 
            /// For example: "-10.00e010", i_start=7, i_end=10
            /// </summary>
            public readonly int e_start, e_end;

            /// <summary>
            /// Number of visible fraction digits of <see cref="String"/> , with trailing zeroes.
            /// 
            /// For example: n=1.230, v=3
            /// </summary>
            public readonly int v;

            /// <summary>
            /// Number of visible fraction digits of <see cref="String"/>, without trailing zeros.
            /// 
            /// For example: n=1.230, w=23
            /// </summary>
            public readonly int w;

            /// <summary>
            /// Visible fractional digit characters of <see cref="String"/>, with trailing zeros.
            /// 
            /// Both values are -1, if fraction digits are not detected.
            /// 
            /// For example: n=1.230, f=230
            /// </summary>
            public readonly int f_start, f_end;

            /// <summary>
            /// Visible fractional digit characters of <see cref="String"/>, without trailing zeros.
            /// 
            /// Both values are -1, if fraction digits are not detected.
            /// 
            /// For example: n=1.230, t=23
            /// </summary>
            public readonly int t_start, t_end;

            /// <summary>
            /// Culture info
            /// </summary>
            public readonly CultureInfo cultureInfo;

            /// <inheritdoc />
            public override bool HasValue => String != null && String.Length > 0 && ((i_start>=0&&i_end>=0&&i_start<i_end)||(f_start>=0&&f_end>=0&&f_start<f_end));
            /// <inheritdoc />
            public override bool IsFloat => w > 0;
            /// <inheritdoc />
            public override int Sign => zero ? 0 : negative ? -1 : 1;

            /// <inheritdoc />
            public override IDecimalNumber N => negative ? new Text(String, true, zero, cultureInfo, n_start, n_end, n_start, n_end, i_start, i_end, v, w, f_start, f_end, t_start, t_end, e_start, e_end) : this;
            /// <inheritdoc />
            public override IDecimalNumber I => zero ? _0 : String == null || i_start < 0 || i_end < 0 || i_start == i_end ? Empty : new Text(String, false, false, cultureInfo, i_start, i_end, i_start, i_end, i_start, i_end, 0, 0, -1, -1, -1, -1, e_start, e_end);
            /// <inheritdoc />
            public override IDecimalNumber F => !IsFloat || v == 0 ? _0 : new Text(String, false, false, cultureInfo, f_start, f_end, f_start, f_end, f_start, f_end, 0, 0, -1, -1, -1, -1, -1, -1);
            /// <inheritdoc />
            public override IDecimalNumber T => !IsFloat || w == 0 ? _0 : new Text(String, false, false, cultureInfo, t_start, t_end, t_start, t_end, t_start, t_end, 0, 0, -1, -1, -1, -1, -1, -1);
            /// <inheritdoc />
            public override IDecimalNumber V => new Long(v);
            /// <inheritdoc />
            public override IDecimalNumber W => new Long(w);
            /// <summary>
            /// Exponent digits
            /// </summary>
            public IDecimalNumber E => String == null || e_start<0 || e_end< 0 || e_start >= e_end ? Empty : new Text(String, false, false, cultureInfo, e_start, e_end, e_start, e_end, e_start, e_end, 0, 0, -1, -1, -1, -1, -1, -1);

            /// <summary>
            /// Create with specific parameters.
            /// </summary>
            /// <param name="string"></param>
            /// <param name="negative"></param>
            /// <param name="zero"></param>
            /// <param name="cultureInfo"></param>
            /// <param name="s_start"></param>
            /// <param name="s_end"></param>
            /// <param name="n_start"></param>
            /// <param name="n_end"></param>
            /// <param name="i_start"></param>
            /// <param name="i_end"></param>
            /// <param name="v"></param>
            /// <param name="w"></param>
            /// <param name="f_start"></param>
            /// <param name="f_end"></param>
            /// <param name="t_start"></param>
            /// <param name="t_end"></param>
            /// <param name="e_start"></param>
            /// <param name="e_end"></param>
            public Text(string @string, bool negative, bool zero, CultureInfo cultureInfo, int s_start, int s_end, int n_start, int n_end, int i_start, int i_end, int v, int w, int f_start, int f_end, int t_start, int t_end, int e_start, int e_end)
            {
                String = @string;
                this.negative = negative;
                this.zero = zero;
                this.s_start = s_start;
                this.s_end = s_end;
                this.n_start = n_start;
                this.n_end = n_end;
                this.i_start = i_start;
                this.i_end = i_end;
                this.cultureInfo = cultureInfo;
                this.v = v;
                this.w = w;
                this.f_start = f_start;
                this.f_end = f_end;
                this.t_start = t_start;
                this.t_end = t_end;
                this.e_start = e_start;
                this.e_end = e_end;
            }

            /// <summary>
            /// Create text and scan for parameters.
            /// </summary>
            /// <param name="string"></param>
            /// <param name="cultureInfo"></param>
            /// <param name="isHex"></param>
            public Text(string @string, CultureInfo cultureInfo, bool isHex = false) : this(@string, @string == null ? -1 : 0, @string == null ? -1 : @string.Length, cultureInfo, isHex) { }

            /// <summary>
            /// Create text and scan <paramref name="string"/> for parameters.
            /// </summary>
            /// <param name="string"></param>
            /// <param name="s_start"></param>
            /// <param name="s_end"></param>
            /// <param name="cultureInfo"></param>
            /// <param name="isHex"></param>
            public Text(string @string, int s_start, int s_end, CultureInfo cultureInfo, bool isHex = false)
            {
                String = @string;
                n_start = -1; n_end = -1;
                i_start = -1; i_end = -1;
                f_start = -1; f_end = -1;
                t_start = -1; t_end = -1;
                e_start = -1; e_end = -1;
                this.s_start = s_start;
                this.s_end = s_end;
                this.cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
                w = 0;
                v = 0;

                // Null
                if (@string == null) return;

                NumberFormatInfo numberFormat = this.cultureInfo.NumberFormat;

                zero = true;
                ScanState state = ScanState.Zero;
                for (int i = s_start; i < s_end; i++)
                {
                    char ch = @string[i];

                    if (ch == ' ') continue;

                    if (ch == '-' && state == ScanState.Zero)
                    {
                        negative = true;
                        continue;
                    }

                    if (!isHex && (ch == 'e' || ch == 'E'))
                    {
                        state = ScanState.Exponent;
                        continue;
                    }

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
                            zero = false;
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
                            zero = false;
                            if (i_start < 0) i_start = i;
                            i_end = i + 1;
                            continue;
                        }
                    }

                    if (state == ScanState.Zero || state == ScanState.Integer)
                    {
                        // ',' or '.' decimal separator
                        if ((MatchString(i, numberFormat.NumberDecimalSeparator) || MatchString(i, numberFormat.PercentDecimalSeparator) || MatchString(i, numberFormat.CurrencyDecimalSeparator))) // <- 3 separators this may cause problems, if they differ
                        {
                            state = ScanState.Fraction;
                            f_start = i + 1; f_end = i + 1;
                            t_start = i + 1; t_end = i + 1;
                            continue;
                        }
                        continue;
                    }

                    if (state == ScanState.Exponent)
                    {
                        if ((ch >= '0' && ch <= '9')||ch=='-'||ch=='+')
                        {
                            if (e_start<0) e_start = i;
                            e_end = i + 1;
                            continue;
                        }
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
                            zero = false;
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
                        char ch = @string[i];
                        if (ch >= '0' && ch <= '9') w++;
                    }

                // Make n positive number region
                if (i_start >= 0 && i_start > n_start) n_start = i_start;
                if (f_start >= 0 && f_start > n_start) n_start = f_start;
                if (e_start >= 0 && e_start > n_start) n_start = e_start;
                if (t_start >= 0 && t_start > n_start) n_start = t_start;
                if (i_end >= 0 && i_end > n_end) n_end = i_end;
                if (f_end >= 0 && f_end > n_end) n_end = f_end;
                if (e_end >= 0 && e_end > n_end) n_end = e_end;
                if (t_end >= 0 && t_end > n_end) n_end = t_end;
            }

            /// <inheritdoc />
            public override string ToString()
                => !HasValue ? null : String.Substring(s_start, s_end-s_start);
                
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
                    if (String[endIx] != matchTo[i]) return false;
                return true;
            }

            /// <summary>
            /// Try to calculate modulo.
            /// </summary>
            /// <param name="modulo"></param>
            /// <returns></returns>
            public override IDecimalNumber Modulo(int modulo)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) return _0;
                // Negative divider
                if (modulo < 0) modulo = -modulo;
                // As is
                if (modulo == 1) return this;
                // Division by zero
                if (modulo == 0) throw new DivideByZeroException();
                // Get last digits
                if (String != null && i_start >= 0 && i_end >= 0 && e_start < 0 && e_end < 0)
                {
                    // Get last digit
                    (int, int) digits = (-1, -1);
                    if (modulo == 10 && TryGetLastIntegerDigits(1, out digits)) { }
                    else if (modulo == 100 && TryGetLastIntegerDigits(2, out digits)) { }
                    else if (modulo == 1000 && TryGetLastIntegerDigits(3, out digits)) { }
                    else if (modulo == 10000 && TryGetLastIntegerDigits(4, out digits)) { }
                    else if (modulo == 100000 && TryGetLastIntegerDigits(5, out digits)) { }
                    else if (modulo == 1000000 && TryGetLastIntegerDigits(6, out digits)) { }
                    else if (modulo == 10000000 && TryGetLastIntegerDigits(7, out digits)) { }
                    else if (modulo == 100000000 && TryGetLastIntegerDigits(8, out digits)) { }
                    else if (modulo == 1000000000 && TryGetLastIntegerDigits(9, out digits)) { }

                    (int, int) integer_digits = digits;

                    // Add fraction 
                    if (digits.Item1>=0 && digits.Item2>=0 && v > 0)
                    {
                        if (f_end > digits.Item2) digits.Item2 = f_end; else goto failed;
                    }

                    // Is zero?
                    bool isZero = true;
                    for (int i = digits.Item1; i < digits.Item2; i++)
                    {
                        char ch = String[i];
                        if (ch >= '1' && ch <= '9') { isZero = false; break; }
                    }
                    if (isZero) return _0;

                    // Complement
                    if (Sign < 0)
                    {
                        string text = "-"+String.Substring(digits.Item1, digits.Item2 - digits.Item1);
                        return new Text(text, 0, text.Length, cultureInfo, false);
                    }

                    return new Text(String, false, false, cultureInfo, digits.Item1, digits.Item2, digits.Item1, digits.Item2, integer_digits.Item1, integer_digits.Item2, 0, 0, -1, -1, -1, -1, -1, -1);
                }
                failed:

                if (IsFloat)
                {
                    double _d;
                    if (TryGet(out _d)) return new Double(_d % modulo);

                    // ?
                } else
                {
                    long _l;
                    if (TryGet(out _l)) return new Long(_l % modulo);
                    System.Numerics.BigInteger _b;
                    if (TryGet(out _b)) return new BigInteger(_b % modulo);

                    // ?    
                }

                return null;
            }

            /// <summary>
            /// Get last integer part digits
            /// </summary>
            /// <param name="count"></param>
            /// <param name="digits_range"></param>
            /// <returns></returns>
            bool TryGetLastIntegerDigits(int count, out (int, int) digits_range)
            {
                if (String == null || i_start < 0 || i_start < 0 || i_start > i_end) { digits_range = (-1, -1); return false; }

                // Return all integer digits
                if (count >= i_end - i_start) { digits_range = (i_start, i_end); return true; }
                // Return count digits
                int i = i_end;
                while (count>0 && i>i_start)
                {
                    i--;
                    char ch = String[i];
                    if (ch == '0' && ch <= '9') count--;
                }
                digits_range = (i, i_end);
                return true;
            }

            /// <inheritdoc />
            public override bool TryGet(out long value)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) { value = 0; return false; }
                // Float 
                if (IsFloat) { value = 0; return false; }

                // Parse integer
                long result = 0L;
                if (i_start >= 0 && i_end >= 0 && i_start < i_end)
                {
                    for (int i = i_start; i < i_end; i++)
                    {
                        char ch = String[i];
                        if (ch >= '0' && ch <= '9')
                        {
                            if (result >= LongTenthOfMax) { value = 0L; return false; }
                            long x = (long)(ch - '0');
                            result = result * 10L + x;
                        }
                    }
                }

                // Parse exponent
                if (e_start >= 0 && e_end >= 0 && e_start < e_end)
                {
                    long exp = 0;
                    for (int i = e_start; i < e_end; i++)
                    {
                        char ch = String[i];
                        if (ch >= '0' && ch <= '9')
                        {
                            if (exp >= LongTenthOfMax) { value = 0L; return false; }
                            long x = (long)(ch - '0');
                            exp = exp * 10L + x;
                        }
                    }

                    for (int i=0; i<exp; i++)
                    {
                        if (result >= LongTenthOfMax) { value = 0L; return false; }
                        result *= 10;
                    }
                }

                value = result;
                if (negative) value = -value;
                return true;
            }

            const int IntTenthOfMax = int.MaxValue / 10;
            const long LongTenthOfMax = long.MaxValue / 10L;
            static readonly decimal DecimalTenthOfMax = Math.Truncate(decimal.MaxValue / 10m);

            /// <inheritdoc />
            public override bool TryGet(out decimal value)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) { value = 0; return false; }

                if (s_start == 0 && s_end == String.Length) return decimal.TryParse(String, out value);
                return decimal.TryParse(String.Substring(s_start, s_end-s_start), out value);

                /*

                // Parse integer
                decimal result = 0m;
                if (i_start >= 0 && i_end >= 0 && i_start < i_end)
                {
                    for (int i = i_start; i < i_end; i++)
                    {
                        char ch = String[i];
                        if (ch >= '0' && ch <= '9')
                        {
                            if (result >= DecimalTenthOfMax) { value = 0m; return false; }
                            decimal x = (decimal)(ch - '0');
                            result = result * 10m + x;
                        }
                    }
                }

                // Parse fragment
                if (f_start >= 0 && f_end >= 0 && f_start < f_end)
                {
                    decimal fragment = 0m;
                    for (int i = f_end-1; i >= f_start; i--)
                    {
                        char ch = String[i];
                        if (ch >= '0' && ch <= '9')
                        {
                            decimal x = ((decimal)(ch - '0'))/10m;
                            fragment = fragment / 10L + x;
                        }
                    }
                    result += fragment;
                }

                // Parse exponent
                int exp = 0;
                if (e_start >= 0 && e_end >= 0 && e_start < e_end)
                {
                    for (int i = e_start; i < e_end; i++)
                    {
                        char ch = String[i];
                        if (ch >= '0' && ch <= '9')
                        {
                            if (exp >= IntTenthOfMax) { value = 0m; return false; }
                            int x = (int)(ch - '0');
                            exp = exp * 10 + x;
                        }
                    }
                    decimal factor = 1m;
                    for (int i = 0; i < exp; i++)
                        factor *= 10m;
                    result *= factor;
                }

                if (negative) result = -result;
                value = result;
                return true;*/
            }

            /// <inheritdoc />
            public override bool TryGet(out double value)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) { value = 0; return false; }

                if (s_start == 0 && s_end == String.Length) return double.TryParse(String, out value);
                return double.TryParse(String.Substring(s_start, s_end - s_start), out value);
            }

            /// <inheritdoc />
            public override bool TryGet(out System.Numerics.BigInteger value)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) { value = 0; return false; }

                if (s_start == 0 && s_end == String.Length) return System.Numerics.BigInteger.TryParse(String, out value);
                return System.Numerics.BigInteger.TryParse(String.Substring(s_start, s_end - s_start), out value);
            }

            enum ScanState
            {
                Zero,
                Integer,
                Fraction,
                Exponent
            }

        }

    }

}


