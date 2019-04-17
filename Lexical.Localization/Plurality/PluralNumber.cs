// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Decimal number
    /// </summary>
    public abstract class DecimalNumber : IPluralNumber
    {
        /// <summary>
        /// Zero constant
        /// </summary>
        public static readonly IPluralNumber _0 = new Long(0L, new Text("0", numberStyles: NumberStyles.Integer));

        /// <summary>
        /// Empty value constant
        /// </summary>
        public static readonly IPluralNumber Empty = new Text("", 0, 0, CultureInfo.InvariantCulture, numberStyles: NumberStyles.Integer);

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
        /// Get text version.
        /// </summary>
        public abstract Text AsText { get; }

        /// <summary>
        /// Absolute positive value.
        /// </summary>
        public abstract IPluralNumber N { get; }

        /// <summary>
        /// Integer value digits. (positive, no fractions)
        /// </summary>
        public abstract IPluralNumber I { get; }

        /// <summary>
        /// Fraction digits with trailing zeroes.
        /// </summary>
        public abstract IPluralNumber F { get; }

        /// <summary>
        /// Exponent digits.
        /// </summary>
        public abstract IPluralNumber E { get; }

        /// <summary>
        /// Fraction digits without trailing zeroes.
        /// </summary>
        public abstract IPluralNumber T { get; }

        /// <summary>
        /// Number of visible integer digits, with trailing zeroes.
        /// </summary>
        public abstract int I_Digits { get; }

        /// <summary>
        /// Number of visible exponent digits, without trailing zeroes.
        /// </summary>
        public abstract int E_Digits { get; }

        /// <summary>
        /// Number of visible fraction digits, with trailing zeroes.
        /// </summary>
        public abstract int F_Digits { get; }

        /// <summary>
        /// Number of visible fraction digits, without trailing zeroes.
        /// </summary>
        public abstract int T_Digits { get; }

        /// <summary>
        /// Number base, 2, 8, 10 or 16.
        /// </summary>
        public abstract int Base { get; }

        /// <summary>
        /// Calculate modulo.
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public abstract IPluralNumber Modulo(int modulo);

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

        /// <inheritdoc />
        public virtual IEnumerator<char> GetEnumerator()
            => AsText.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator(); // <- kind of risky, but current compiler points to the other method that returns IEnumerator<char> result.

        /// <inheritdoc />
        public override string ToString()
            => AsText.ToString();


        bool hashcodeCalculated;
        int hashcode;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!hashcodeCalculated)
            {
                hashcode = PluralNumberComparer.Instance.GetHashCode(this);
                hashcodeCalculated = true;
            }
            return hashcode;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is IPluralNumber number) return PluralNumberComparer.Instance.Equals(this, number);
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
            /// Value as text
            /// </summary>
            Text text;

            /// <summary>
            /// Text representation of the value.
            /// </summary>
            public override Text AsText => text ?? (text = new Text(Value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, NumberStyles.Integer));

            /// <summary>
            /// Create long value
            /// </summary>
            /// <param name="value"></param>
            /// <param name="text">(optional) value as text</param>
            public Long(long value, Text text = default)
            {
                this.Value = value;
                this.text = text;
            }

            /// <inheritdoc />
            public override bool HasValue => true;
            /// <inheritdoc />
            public override bool IsFloat => false;
            /// <inheritdoc />
            public override IPluralNumber N => Value < 0L ? new Long(-Value, AsText.N as Text) : this;
            /// <inheritdoc />
            public override IPluralNumber I => AsText.I;
            /// <inheritdoc />
            public override IPluralNumber F => Empty;
            /// <inheritdoc />
            public override IPluralNumber T => Empty;
            /// <inheritdoc />
            public override IPluralNumber E => AsText.E;
            /// <inheritdoc />
            public override int I_Digits => AsText.I_Digits;
            /// <inheritdoc />
            public override int E_Digits => AsText.E_Digits;
            /// <inheritdoc />
            public override int F_Digits => 0;
            /// <inheritdoc />
            public override int T_Digits => 0;
            /// <inheritdoc />
            public override string ToString() => AsText.ToString();
            /// <inheritdoc />
            public override IPluralNumber Modulo(int modulo)
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
            public override IEnumerator<char> GetEnumerator()
                => AsText.GetEnumerator();

            /// <inheritdoc />
            public override int Sign => Value < 0L ? -1 : Value == 0L ? 0 : 1;
            /// <inheritdoc />
            public override int Base => text == null ? 10 : text.Base;
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
            double truncate;
            /// <summary>
            /// Value as text
            /// </summary>
            Text text;

            /// <summary>
            /// Text representation of the value.
            /// </summary>
            public override Text AsText => text ?? (text = new Text(Value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, NumberStyles.Integer));

            /// <summary>
            /// Create double number
            /// </summary>
            /// <param name="value"></param>
            /// <param name="text">(optional) value as text</param>
            public Double(double value, Text text = default)
            {
                this.Value = value;
                this.text = text;
                this.truncate = Math.Truncate(Value);
            }

            /// <inheritdoc />
            public override bool HasValue => true;
            /// <inheritdoc />
            public override bool IsFloat => Value != truncate;
            /// <inheritdoc />
            public override int Sign => Value < 0d ? -1 : Value == 0d ? 0 : 1;
            /// <inheritdoc />
            public override IPluralNumber N => Value < 0d ? new Double(-Value) : this;
            /// <inheritdoc />
            public override IPluralNumber I => AsText.I;
            /// <inheritdoc />
            public override IPluralNumber E => AsText.E;
            /// <inheritdoc />
            public override IPluralNumber F => AsText.F;
            /// <inheritdoc />
            public override IPluralNumber T => AsText.T;
            /// <inheritdoc />
            public override int F_Digits => AsText.F_Digits;
            /// <inheritdoc />
            public override int T_Digits => AsText.T_Digits;
            /// <inheritdoc />
            public override int Base => throw new NotImplementedException();
            /// <inheritdoc />
            public override int I_Digits => AsText.I_Digits;
            /// <inheritdoc />
            public override int E_Digits => AsText.E_Digits;

            /// <inheritdoc />
            public override IPluralNumber Modulo(int modulo)
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
            /// <inheritdoc />
            public override IEnumerator<char> GetEnumerator()
                => AsText.GetEnumerator();
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
            /// Value as text
            /// </summary>
            Text text;

            /// <summary>
            /// Absolute version.
            /// </summary>
            IPluralNumber absolute;

            /// <summary>
            /// Text representation of the value.
            /// </summary>
            public override Text AsText => text ?? (text = new Text(Value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, NumberStyles.Integer));

            /// <summary>
            /// Create big integer number
            /// </summary>
            /// <param name="bigInteger"></param>
            public BigInteger(System.Numerics.BigInteger bigInteger)
            {
                this.Value = bigInteger;
            }

            /// <summary>
            /// Absolute value
            /// </summary>
            public IPluralNumber Absolute => absolute ?? (absolute = Value.Sign < 0 ? new BigInteger(-Value) : this);
            /// <inheritdoc />
            public override bool HasValue => true;
            /// <inheritdoc />
            public override bool IsFloat => false;
            /// <inheritdoc />
            public override int Sign => Value.Sign;
            /// <inheritdoc />
            public override IPluralNumber N => Absolute;
            /// <inheritdoc />
            public override IPluralNumber I => Absolute;
            /// <inheritdoc />
            public override IPluralNumber F => Empty;
            /// <inheritdoc />
            public override IPluralNumber T => Empty;
            /// <inheritdoc />
            public override IPluralNumber E => _0;
            /// <inheritdoc />
            public override int I_Digits => throw new NotImplementedException();
            /// <inheritdoc />
            public override int F_Digits => 0;
            /// <inheritdoc />
            public override int T_Digits => 0;
            /// <inheritdoc />
            public override int E_Digits => 0;
            /// <inheritdoc />
            public override int Base => 10;

            /// <inheritdoc />
            public override IPluralNumber Modulo(int modulo)
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

            static readonly System.Numerics.BigInteger MinDecimal = new System.Numerics.BigInteger(-79228162514264337593543950335M);
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
            /// Number base
            /// </summary>
            public readonly int NumberBase;

            /// <summary>
            /// Sign: -1, 0, 1
            /// </summary>
            protected int sign;

            /// <summary>
            /// Number base: 2, 8, 10 or 16.
            /// </summary>
            public override int Base => NumberBase;

            /// <summary>
            /// Cached number features.
            /// </summary>
            IPluralNumber n, i, f, t, e;

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
            /// Number of digits. (Not same as i_end-i_start, which is the length of the character range including whitespace.)
            /// </summary>
            public readonly int i_digits;

            /// <summary>
            /// Exponent digits of <see cref="String"/>. Preceding zeros are excluded.
            /// 
            /// Both values are -1, if integer digits are not detected.
            /// 
            /// For example: "-10.00e010", i_start=7, i_end=10
            /// </summary>
            public readonly int e_start, e_end;

            /// <summary>
            /// Number of e digits. (Not same as i_end-i_start, which is the length of the character range including whitespace.)
            /// </summary>
            public readonly int e_digits;

            /// <summary>
            /// Visible fractional digit characters of <see cref="String"/>, with trailing zeros.
            /// 
            /// Both values are -1, if fraction digits are not detected.
            /// 
            /// For example: n=1.230, f=230
            /// </summary>
            public readonly int f_start, f_end;

            /// <summary>
            /// Number of visible fraction digits of <see cref="String"/> , with trailing zeroes.
            /// 
            /// Same as 'v' in unicode CLDR plural.xml.
            /// 
            /// For example: n=1.230, v=3
            /// </summary>
            public readonly int f_digits;

            /// <summary>
            /// Visible fractional digit characters of <see cref="String"/>, without trailing zeros.
            /// 
            /// Both values are -1, if fraction digits are not detected.
            /// 
            /// For example: n=1.230, t=23
            /// </summary>
            public readonly int t_start, t_end;

            /// <summary>
            /// Number of visible fraction digits of <see cref="String"/>, without trailing zeros.
            /// 
            /// Same as 'w' in unicod CLDR plural.xml.
            /// 
            /// For example: n=1.230, w=23
            /// </summary>
            public readonly int t_digits;

            /// <summary>
            /// Culture info
            /// </summary>
            public readonly CultureInfo cultureInfo;

            /// <summary>
            /// Number styles
            /// </summary>
            public readonly NumberStyles numberStyle;

            /// <inheritdoc />
            public override bool HasValue => String != null && String.Length > 0 && (i_digits > 0 || f_digits > 0 || t_digits > 0);
            /// <inheritdoc />
            public override bool IsFloat => t_digits > 0;
            /// <inheritdoc />
            public override int Sign => sign;
            /// <inheritdoc />
            public override IPluralNumber N => n ?? (n = sign >= 0 ? this : /*positive version*/new Text(String, 1, cultureInfo, NumberStyles.Integer, n_start, n_end, n_start, n_end, i_start, i_end, i_digits, f_start, f_end, f_digits, t_start, t_end, t_digits, e_start, e_end, e_digits, NumberBase));
            /// <inheritdoc />
            public override IPluralNumber I => i ?? (i = new Text(String, Math.Abs(sign)/*0/1*/, cultureInfo, NumberStyles.Integer, i_start, i_end, i_start, i_end, i_start, i_end, i_digits, -1, -1, 0, -1, -1, 0, -1, -1, 0, NumberBase));
            /// <inheritdoc />
            public override IPluralNumber F => f ?? (f = f_digits == 0 ? Empty : new Text(String, f_start, f_end, cultureInfo, NumberStyles.Integer, NumberBase));
            /// <inheritdoc />
            public override IPluralNumber T => t ?? (t = t_digits == 0 ? Empty : new Text(String, t_start, t_end, cultureInfo, NumberStyles.Integer, NumberBase));
            /// <inheritdoc />
            public override IPluralNumber E => e ?? (e = e_digits == 0 ? Empty : new Text(String, e_start, e_end, cultureInfo, NumberStyles.Integer, NumberBase));
            /// <inheritdoc />
            public override int F_Digits => f_digits;
            /// <inheritdoc />
            public override int T_Digits => t_digits;
            /// <inheritdoc />
            public override int E_Digits => e_digits;
            /// <inheritdoc />
            public override int I_Digits => i_digits;
            /// <summary>
            /// Get as text.
            /// </summary>
            public override Text AsText => this;

            /// <summary>
            /// Create with specific parameters.
            /// </summary>
            /// <param name="string"></param>
            /// <param name="sign"></param>
            /// <param name="cultureInfo"></param>
            /// <param name="numberStyles"></param>
            /// <param name="s_start"></param>
            /// <param name="s_end"></param>
            /// <param name="n_start"></param>
            /// <param name="n_end"></param>
            /// <param name="i_start"></param>
            /// <param name="i_end"></param>
            /// <param name="i_digits"></param>
            /// <param name="f_digits"></param>
            /// <param name="t_digits"></param>
            /// <param name="f_start"></param>
            /// <param name="f_end"></param>
            /// <param name="t_start"></param>
            /// <param name="t_end"></param>
            /// <param name="e_start"></param>
            /// <param name="e_end"></param>
            /// <param name="e_digits"></param>
            /// <param name="numberBase"></param>
            public Text(string @string, int sign, CultureInfo cultureInfo, NumberStyles numberStyles, int s_start, int s_end, int n_start, int n_end, int i_start, int i_end, int i_digits, int f_start, int f_end, int f_digits, int t_start, int t_end, int t_digits, int e_start, int e_end, int e_digits, int numberBase)
            {
                this.String = @string;
                this.cultureInfo = cultureInfo;
                this.numberStyle = numberStyles;
                this.sign = sign;
                this.s_start = s_start;
                this.s_end = s_end;
                this.n_start = n_start;
                this.n_end = n_end;
                this.i_start = i_start;
                this.i_end = i_end;
                this.i_digits = i_digits;
                this.f_start = f_start;
                this.f_end = f_end;
                this.f_digits = f_digits;
                this.t_start = t_start;
                this.t_end = t_end;
                this.t_digits = t_digits;
                this.e_start = e_start;
                this.e_end = e_end;
                this.e_digits = e_digits;
                this.NumberBase = numberBase >=0 ? numberBase : ((numberStyles & NumberStyles.AllowHexSpecifier) != 0) ? 16 : 10;
            }

            /// <summary>
            /// Create text and scan for parameters.
            /// </summary>
            /// <param name="string"></param>
            /// <param name="cultureInfo"></param>
            /// <param name="numberStyles"></param>
            /// <param name="numberBase"></param>
            public Text(string @string, CultureInfo cultureInfo = default, NumberStyles numberStyles = NumberStyles.Float, int numberBase = -1) : this(@string, @string == null ? -1 : 0, @string == null ? -1 : @string.Length, cultureInfo, numberStyles, numberBase) { }

            /// <summary>
            /// Create text and scan <paramref name="string"/> for parameters.
            /// </summary>
            /// <param name="string"></param>
            /// <param name="s_start"></param>
            /// <param name="s_end"></param>
            /// <param name="cultureInfo"></param>
            /// <param name="numberStyles"></param>
            /// <param name="numberBase"></param>
            public Text(string @string, int s_start, int s_end, CultureInfo cultureInfo = default, NumberStyles numberStyles = NumberStyles.Float, int numberBase = -1)
            {
                this.String = @string;
                this.numberStyle = numberStyles;
                this.NumberBase = numberBase >= 0 ? numberBase : ((numberStyles & NumberStyles.AllowHexSpecifier) != 0) ? 16 : 10;
                n_start = -1; n_end = -1;
                i_start = -1; i_end = -1;
                f_start = -1; f_end = -1;
                t_start = -1; t_end = -1;
                e_start = -1; e_end = -1;
                this.s_start = s_start;
                this.s_end = s_end;
                this.cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
                t_digits = 0;
                f_digits = 0;
                e_digits = 0;
                i_digits = 0;

                // Null
                if (@string == null) return;

                NumberFormatInfo numberFormat = this.cultureInfo.NumberFormat;

                bool zero = true;
                bool negative = false;
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

                    if (numberBase <= 10 && ((this.numberStyle & NumberStyles.AllowExponent) != 0) && (ch == 'e' || ch == 'E' || ch == '⏨'))
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
                        else if (IsNonZeroDigit(ch))
                        {
                            zero = false;
                            if (i_start < 0) i_start = i;
                            i_end = i + 1;
                            i_digits++;
                            state = ScanState.Integer;
                            continue;
                        }
                    }
                    else if (state == ScanState.Integer)
                    {
                        if (IsDigit(ch))
                        {
                            zero = false;
                            if (i_start < 0) i_start = i;
                            i_end = i + 1;
                            i_digits++;
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
                        if (IsDigit(ch) || ch == '-' || ch == '+')
                        {
                            if (e_start < 0) e_start = i;
                            e_end = i + 1;
                            e_digits++;
                            continue;
                        }
                    }

                    if (state == ScanState.Fraction)
                    {
                        if (IsDigit(ch))
                        {
                            if (f_start < 0) f_start = i;
                            f_end = i + 1;
                            f_digits++;
                        }

                        if (IsNonZeroDigit(ch))
                        {
                            zero = false;
                            if (t_start < 0) t_start = i;
                            t_end = i + 1;
                        }
                        continue;
                    }
                }

                // Count t_digits 'w', number of fraction digits without trailing zeroes
                if (t_start >= 0 && t_end >= 0)
                    for (int i = t_start; i < t_end; i++)
                    {
                        char ch = @string[i];
                        if (IsDigit(ch)) t_digits++;
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

                // Set sign
                this.sign = zero ? 0 : negative ? -1 : 1;
            }

            /// <summary>
            /// Test if <paramref name="ch"/> is a digit of the <see cref="NumberBase"/>.
            /// </summary>
            /// <param name="ch"></param>
            /// <returns></returns>
            bool IsDigit(char ch)
                => ch >= '0' && ch <= '9' || ((NumberBase == 16) && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F')));
            /// <summary>
            /// Test if <paramref name="ch"/> is a digit of the <see cref="NumberBase"/>.
            /// </summary>
            /// <param name="ch"></param>
            /// <returns></returns>
            bool IsNonZeroDigit(char ch)
                => ch >= '1' && ch <= '9' || ((NumberBase == 16) && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F')));

            /// <summary>
            /// Get digit value basd on <see cref="NumberBase"/>.
            /// </summary>
            /// <param name="ch"></param>
            /// <returns>value, or -1 if was not digit</returns>
            int DigitValue(char ch)
            {
                if (ch >= '0' && ch <= '9') return (int)(ch - '0');
                if (NumberBase == 16 && (ch >= 'a' && ch <= 'f')) return (int)(ch - 'a') + 10;
                if (NumberBase == 16 && (ch >= 'A' && ch <= 'F')) return (int)(ch - 'a') + 10;
                return -1;
            }

            /// <inheritdoc />
            public override IEnumerator<char> GetEnumerator()
            {
                // Exponent value
                long e = 0L;
                // If exponent digit read fails, use other methods
                TryGetExponent(out e);

                IEnumerator<char> digits = GetDigits(i_digits, (int)e);
                if (Sign < 0) yield return '-';
                while (digits.MoveNext()) yield return digits.Current;
            }

            string canonicalizedString;

            /// <inheritdoc />
            public override string ToString()
            {
                if (canonicalizedString == null)
                {
                    StringBuilder sb = new StringBuilder(i_digits+f_digits+2);
                    // Exponent value
                    long e = 0L;
                    // If exponent digit read fails, use other methods
                    TryGetExponent(out e);

                    IEnumerator<char> digits = GetDigits(i_digits, (int)e);
                    if (Sign < 0) sb.Append('-');
                    while (digits.MoveNext()) sb.Append(digits.Current);
                    canonicalizedString = sb.ToString();
                }
                return canonicalizedString;
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
                    if (String[endIx] != matchTo[i]) return false;
                return true;
            }

            /// <summary>
            /// Try to calculate modulo.
            /// </summary>
            /// <param name="modulo"></param>
            /// <returns></returns>
            public override IPluralNumber Modulo(int modulo)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) return _0;
                // Zero
                if (sign == 0) return _0;
                // Negative divider
                if (modulo < 0) modulo = -modulo;
                // As is
                if (modulo == 1) return this;
                // Division by zero
                if (modulo == 0) throw new DivideByZeroException();
                // Modulo by BaseNumber - so get substring digits
                if (String != null && i_start >= 0 && i_end >= 0)
                {
                    int numberOfDigitsToGet = -1;
                    if (NumberBase == 10)
                    {
                        switch(modulo)
                        {
                            case 10: numberOfDigitsToGet = 1; break;
                            case 100: numberOfDigitsToGet = 2; break;
                            case 1000: numberOfDigitsToGet = 3; break;
                            case 10000: numberOfDigitsToGet = 4; break;
                            case 100000: numberOfDigitsToGet = 5; break;
                            case 1000000: numberOfDigitsToGet = 6; break;
                            case 10000000: numberOfDigitsToGet = 7; break;
                            case 100000000: numberOfDigitsToGet = 8; break;
                            case 1000000000: numberOfDigitsToGet = 9; break;
                        }
                    } else if (NumberBase == 16)
                    {
                        switch (modulo)
                        {
                            case 0x10: numberOfDigitsToGet = 1; break;
                            case 0x100: numberOfDigitsToGet = 2; break;
                            case 0x1000: numberOfDigitsToGet = 3; break;
                            case 0x10000: numberOfDigitsToGet = 4; break;
                            case 0x100000: numberOfDigitsToGet = 5; break;
                            case 0x1000000: numberOfDigitsToGet = 6; break;
                            case 0x10000000: numberOfDigitsToGet = 7; break;
                        }
                    }
                    if (numberOfDigitsToGet <= 0) goto noDigitExtraction;

                    // Exponent value
                    long e = 0L;
                    // If exponent digit read fails, use other methods
                    if (e_digits > 0 && !TryGetExponent(out e)) goto noDigitExtraction;

                    // Exponent == 0
                    if (e == 0L)
                    {
                        // Get integer digits
                        (int, int) digits;
                        if (!TryGetLastIntegerDigitIndices(0, numberOfDigitsToGet, out digits)) goto noDigitExtraction;

                        // Add fraction digits
                        if (digits.Item1 >= 0 && digits.Item2 >= 0 && f_digits > 0)
                        {
                            if (digits.Item2 < f_end)
                                digits.Item2 = f_end;
                            else
                                goto noDigitExtraction;
                        }

                        // Is it zero by value?
                        bool isZero = true;
                        for (int i = digits.Item1; i < digits.Item2; i++)
                        {
                            char ch = String[i];
                            if (IsNonZeroDigit(ch)) { isZero = false; break; }
                        }
                        if (isZero) return _0;

                        // Move range to include minus '-'
                        if (Sign < 0)
                        {
                            digits = (digits.Item1 - 1, digits.Item2);
                            return new Text(String, digits.Item1, digits.Item2, cultureInfo, numberStyle, NumberBase);
                        }

                        // Return as is
                        if (digits.Item1 == s_start && digits.Item2 == s_end) return this;

                        // Make complement by creating new string
                        if (Sign < 0)
                        {
                            string text = "-" + String.Substring(digits.Item1, digits.Item2 - digits.Item1);
                            return new Text(text, 0, text.Length, cultureInfo, numberStyle, NumberBase);
                        }

                        return new Text(String, digits.Item1, digits.Item2, cultureInfo, numberStyle, NumberBase);
                    }
                    // There are exponents
                    else
                    {
                        if (Math.Abs(e) > String.Length) goto noDigitExtraction;

                        StringBuilder sb = new StringBuilder();
                        if (Sign < 0) sb.Append("-");

                        int numbersAppended = 0;
                        IEnumerator<char> etor = GetDigits(numberOfDigitsToGet - ((int)e), (int)e);
                        while (etor.MoveNext())
                        {
                            if (etor.Current == '.') continue;
                            if (numberOfDigitsToGet-- == 0)
                            {
                                if (numbersAppended == 0) sb.Append('0');
                                sb.Append(cultureInfo.NumberFormat.NumberDecimalSeparator);
                            }
                            sb.Append(etor.Current);
                            numbersAppended++;
                        }
                        while (numberOfDigitsToGet-->0) sb.Append('0');
                        string text = sb.ToString();
                        return new Text(text, 0, text.Length, cultureInfo, numberStyle, NumberBase);
                    }
                }
                noDigitExtraction:

                if (IsFloat)
                {
                    double _d;
                    if (TryGet(out _d)) return new Double(_d % modulo);

                    // ?
                }
                else
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
            /// Returns integer and fraction digits. 
            /// </summary>
            /// <param name="index">starting location, 0=between integer and fraction separator, 1+ integer digits, 1- fraction digits</param>
            /// <param name="exp">Adjusts decimal separator</param>
            /// <returns></returns>
            IEnumerator<char> GetDigits(int index, int exp)
            {
                if (String == null) yield break;

                // Add digits from integers part
                int decimalSeparatorToGo = index + exp;
                {                    
                    int i = i_end - index;
                    while (index>0)
                    {
                        if (decimalSeparatorToGo-- == 0) yield return '.'; // canonical separator '.'
                        if (i >= i_end) break;
                        if (i < i_start)
                        {
                            yield return '0';
                            index--;
                        }
                        else
                        {
                            char ch = String[i];
                            if (IsDigit(ch))
                            {
                                if (((numberStyle & NumberStyles.AllowHexSpecifier)!=0) && ch>='a' && ch<='f') ch = (char) (ch - 'a' + 'A');
                                yield return ch;
                                index--;
                            }
                        }
                        i++;
                    }
                }


                // Add digits from fractions part
                if (f_digits>0)
                {
                    int i = f_start - index;
                    while (i < f_end)
                    {
                        if (decimalSeparatorToGo-- == 0) yield return '.'; // canonical separator '.'
                        char ch = String[i++];
                        if (IsDigit(ch))
                        {
                            if (((numberStyle & NumberStyles.AllowHexSpecifier) != 0) && ch >= 'a' && ch <= 'f') ch = (char)(ch - 'a' + 'A');
                            yield return ch;
                            index--;
                        }
                    }
                }

            }

            /// <summary>
            /// Get last integer part digits
            /// </summary>
            /// <param name="index">Starting index from right. e.g. 0=starts from the first digit from the right (least significant). 1=the next from the right.</param>
            /// <param name="count"></param>
            /// <param name="digits_range"></param>
            /// <returns></returns>
            bool TryGetLastIntegerDigitIndices(int index, int count, out (int, int) digits_range)
            {
                if (String == null || i_start < 0 || i_start < 0 || i_start > i_end) { digits_range = (-1, -1); return false; }

                // Return all integer digits
                if (index == 0 && count >= i_digits) { digits_range = (i_start, i_end); return true; }
                // Return count digits
                int i = i_end - index, __end = i_end - index;
                while (count > 0 && i > i_start)
                {
                    i--;
                    char ch = String[i];
                    if (IsDigit(ch)) count--;
                }
                digits_range = (i, __end);
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
                long max = long.MaxValue / NumberBase;
                long _base = NumberBase;
                if (i_start >= 0 && i_end >= 0 && i_start < i_end)
                {
                    for (int i = i_start; i < i_end; i++)
                    {
                        char ch = String[i];
                        int digit = DigitValue(ch);
                        if (digit < 0) continue;
                        if (result >= max) { value = 0L; return false; }
                        result = result * _base + (long)digit;
                    }
                }

                // Parse exponent
                if (e_start >= 0 && e_end >= 0 && e_start < e_end)
                {
                    long exp = 0L;
                    if (TryGetExponent(out exp) && exp != 0L)
                    {
                        // Positive exponent
                        if (exp > 0L)
                        {
                            for (int i = 0; i < exp; i++)
                            {
                                if (result >= max) { value = 0L; return false; }
                                result *= _base;
                            }
                        } else 
                        // Negative exponent
                        if (exp < 0L)
                        {
                            exp *= -1;
                            for (int i = 0; i < exp; i++)
                            {
                                // Check if there is non-0 reminder
                                long reminder = result % _base;
                                // Cannot fit fractions into result, -> fail
                                if (reminder != 0L) { value = 0L; return false; }
                                // Divide by base
                                result /= _base;
                            }
                        }
                    }
                }

                value = result;
                if (sign < 0) value = -value;
                return true;
            }

            /// <summary>
            /// Get exponent value as long, if exponent exists and can fit long.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool TryGetExponent(out long value)
            {
                // Parse exponent
                if (e_start < 0 || e_end < 0 || e_end < e_start) { value = 0L; return false; }

                long exp = 0;
                long sign = 1;
                long max = long.MaxValue / NumberBase;
                for (int i = e_start; i < e_end; i++)
                {
                    char ch = String[i];
                    if (ch == '-') { sign *= -1; continue; }
                    int digit = DigitValue(ch);
                    if (digit < 0) continue;
                    if (exp >= max) { value = 0L; return false; }
                    exp = exp * NumberBase + (long)digit;
                }

                value = exp*sign;
                return true;
            }

            /// <inheritdoc />
            public override bool TryGet(out decimal value)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) { value = 0; return false; }

                if (s_start == 0 && s_end == String.Length)
                {
                    return decimal.TryParse(String, out value);
                }

                return decimal.TryParse(String.Substring(s_start, s_end - s_start), out value);

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

            NumberStyles NumberStyle =>
                NumberBase == 16 ? NumberStyles.HexNumber : IsFloat ? NumberStyles.Float : NumberStyles.Integer;

            /// <inheritdoc />
            public override bool TryGet(out double value)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) { value = 0; return false; }

                if (s_start == 0 && s_end == String.Length) return double.TryParse(String, NumberStyle, cultureInfo, out value);
                return double.TryParse(String.Substring(s_start, s_end - s_start), NumberStyle, cultureInfo, out value);
            }

            /// <inheritdoc />
            public override bool TryGet(out System.Numerics.BigInteger value)
            {
                // No string
                if (String == null || s_start < 0 || s_end < 0 || s_end <= s_start) { value = 0; return false; }

                if (s_start == 0 && s_end == String.Length) return System.Numerics.BigInteger.TryParse(String, NumberStyle, cultureInfo, out value);
                return System.Numerics.BigInteger.TryParse(String.Substring(s_start, s_end - s_start), NumberStyle, cultureInfo, out value);
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
