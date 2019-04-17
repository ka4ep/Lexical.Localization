// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Interface for numbers that have decimal string representation or truncated approximation. 
    /// Base 2, 8 and 16 numbers are used aswell.
    /// 
    /// This interface is for extracting features for the purpose of language string declinations, not mathematical operations.
    /// For example numbers "1.1" and "1.10" are not equal in linquistic sense.
    /// 
    /// Following features are extracted:
    /// <list type="bullet">
    ///   <item>sign</item>
    ///   <item>integer</item>
    ///   <item>fractions</item>
    ///   <item>exponent</item>
    /// </list>
    /// 
    /// 
    /// ToString() also returns the same characters.
    /// The IEnumerable&lt;char&gt; enumerator returns characters in canonicalized form with the following rules.
    /// <list type="bullet">
    ///   <item>Starts with '-' if is negative</item>
    ///   <item>Decimal separator is '.'</item>
    ///   <item>There is no whitespaces</item>
    ///   <item>Exponent is unwrapped so that is adjusts the location of decimal separator.</item>
    ///   <item>Hexdecimals are in capital 'A' to 'F'</item>
    /// </list>
    /// </summary>
    public interface IPluralNumber : IEnumerable<char> 
    {
        /// <summary>
        /// True if has non-empty value.
        /// 
        /// If value is NaN, returns false.
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
        /// Base of digits, 2, 8, 10 or 16.
        /// </summary>
        int Base { get; }

        /// <summary>
        /// Try to read into long.
        /// 
        /// If value doesn't fit or has floating-point fractions, then 
        /// If value is floating-point but fractions are zero "1.00", then returns true provided it fits long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out long value);

        /// <summary>
        /// Try to read into decimal.
        /// 
        /// If value doesn't fit decimal, then returns false.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out decimal value);

        /// <summary>
        /// Try to read into double.
        /// 
        /// If value doesn't fit double, then returns false.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out double value);

        /// <summary>
        /// Try to read into big integer.
        /// 
        /// If value doesn't fit or has floating-point fractions, then 
        /// If value is floating-point but fractions are zero "1.00", then returns true provided it fits long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(out System.Numerics.BigInteger value);

        /// <summary>
        /// Try calculate modulo. 
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns>modulo or null if failed to calculate modulo</returns>
        IPluralNumber Modulo(int modulo);

        /// <summary>
        /// Absolute (positive) value of the source number.
        /// </summary>
        IPluralNumber N { get; }

        /// <summary>
        /// Integer digits. 
        /// </summary>
        IPluralNumber I { get; }

        /// <summary>
        /// Exponent digits. 
        /// </summary>
        IPluralNumber E { get; }

        /// <summary>
        /// Visible fractional digits, with trailing zeros.
        /// </summary>
        IPluralNumber F { get; }

        /// <summary>
        /// Visible fractional digits, without trailing zeros.
        /// </summary>
        IPluralNumber T { get; }

        /// <summary>
        /// Number of integer digits.
        /// </summary>
        int I_Digits { get; }

        /// <summary>
        /// Number of exponent digits
        /// </summary>
        int E_Digits { get; }

        /// <summary>
        /// Number of visible fraction digits, with trailing zeroes.
        /// 
        /// Same as 'v' in Unicode CLDR plural.xml.
        /// </summary>
        int F_Digits { get; }

        /// <summary>
        /// Number of visible fraction digits, without trailing zeros.
        /// 
        /// Same as 'w' in Unicode CLDR plural.xml.
        /// </summary>
        int T_Digits { get; }

    }

    /// <summary>
    /// Extension methods for <see cref="IPluralNumber"/>.
    /// </summary>
    public static class PluralityNumberExtensions
    {
        /// <summary>
        /// Tests if <paramref name="number"/> exists in <paramref name="group"/>.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool InGroup(this IPluralNumber number, params IPluralNumber[] group)
        {
            foreach (var value in group)
                if (number.Equals(value)) return true;
            return false;
        }
    }

    /// <summary>
    /// This comparer compares <see cref="IPluralNumber"/>s by value.
    /// 
    /// For example "1.1e1" is equal to "11".
    /// </summary>
    public class PluralNumberComparer : IEqualityComparer<IPluralNumber>
    {
        private static IEqualityComparer<IPluralNumber> instance = new PluralNumberComparer();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IEqualityComparer<IPluralNumber> Instance => instance;

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IPluralNumber x, IPluralNumber y)
            => x.ToString() == y.ToString();

        /// <summary>
        /// Calculate hashcode
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int GetHashCode(IPluralNumber x)
        {
            int result = FNVHashBasis;
            foreach (char ch in x)
            {
                result ^= (int) ch;
                result *= FNVHashPrime;
            }
            return result;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);
        const int FNVHashPrime = 0x1000193;

    }

}


