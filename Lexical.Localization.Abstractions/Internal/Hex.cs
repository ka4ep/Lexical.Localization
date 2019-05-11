// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.8.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Utils for handling hexadecimals
    /// </summary>
    public static class Hex
    {
        /// <summary>
        /// Print <paramref name="value"/> as hex-string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String Print(decimal value) => value.ToString("X", CultureInfo.InvariantCulture);

        /// <summary>
        /// Print <paramref name="value"/> as hex-string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String Print(ulong value) => value.ToString("X", CultureInfo.InvariantCulture);

        /// <summary>
        /// Print <paramref name="value"/> as hex-string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String Print(uint value) => value.ToString("X", CultureInfo.InvariantCulture);

        /// <summary>
        /// Print <paramref name="value"/> as hex-string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String Print(BigInteger value) => value.ToString("X", CultureInfo.InvariantCulture);

        /// <summary>
        /// Parse a string to uint. String must not start with '0x'.
        /// UInt can accomodate only 8 meaningful hex-decimals.
        /// </summary>
        /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
        /// <param name="startIndex">character start index</param>
        /// <returns>value</returns>
        /// <exception cref="System.FormatException"></exception>
        public static uint ToUInt(String str, int startIndex = 0)
        {
            CharEnumerator enumr = str.GetEnumerator();
            for (int i = 0; i < startIndex; i++) 
                if (!enumr.MoveNext())
                throw new FormatException("End of stream before startIndex.");
            HexEnumerator stream = new HexEnumerator(enumr);

            // Calculate hex character count
            int count = 0;
            // Calculate meaningful number characters
            int meaningfulDigits = 0;
            for (int i = 0; i < 999 && stream.MoveNext(); i++)
            {
                // Got non-hex value.
                if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");
                // Start calculating after first non-zero.
                if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
                // Total character count.
                count++;
            }

            // Found non-zero value.
            if (stream.Error) if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");

            // Restart
            stream.Reset();
            for (int i = 0; i < startIndex; i++) enumr.MoveNext();

            // Too many characters
            if (meaningfulDigits >= 9) throw new System.FormatException("Hex value too large to fit into decimal.");

            uint value = 0U;
            while (count > 8 && stream.MoveNext())
            {
                // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
                if (stream.Current != 0) throw new System.FormatException("Hex value too large to fit into decimal.");
                count--;
            }
            while (count > 0 && stream.MoveNext())
            {
                uint hex = (uint)stream.Current;
                value = value << 4 | hex;
                count--;
            }

            return value;
        }

        /// <summary>
        /// Parse a string to long. String must not start with '0x'.
        /// UInt can accomodate only 8 meaningful hex-decimals.
        /// </summary>
        /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
        /// <param name="value">result value</param>
        /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
        /// <exception cref="System.FormatException"></exception>
        public static bool TryParseToUInt(String str, out uint value)
        {
            HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

            // Calculate hex character count
            int count = 0;
            // Calculate meaningful number characters
            int meaningfulDigits = 0;
            for (int i = 0; i < 999 && stream.MoveNext(); i++)
            {
                // Got non-hex value.
                if (stream.Current < 0) { value = default(uint); return false; }
                // Start calculating after first non-zero.
                if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
                // Total character count.
                count++;
            }

            // Found non-zero value.
            if (stream.Error) { value = default(uint); return false; }

            // Restart
            stream.Reset();

            // Too many characters
            if (meaningfulDigits >= 9) { value = default(uint); return false; }

            value = 0U;
            while (count > 8 && stream.MoveNext())
            {
                // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
                if (stream.Current != 0) { value = default(uint); return false; }
                count--;
            }
            while (count > 0 && stream.MoveNext())
            {
                uint hex = (uint)stream.Current;
                value = value << 4 | hex;
                count--;
            }

            return true;
        }

        /// <summary>
        /// Try to parse a string to decimal number.
        /// String must not start with '0x'.
        /// Decimal can accomodate only 24 meaningful hexnumbers. 
        /// Although, the string can precede any number of zeroes, e.g. "0000000000000000000000000000000000000000000000000000000000000123123abff".
        /// </summary>
        /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
        /// <param name="value">result value</param>
        /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
        public static bool TryParseToDecimal(String str, out decimal value)
        {
            HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

            // Calculate hex character count
            int count = 0;
            // Calculate meaningful number characters
            int meaningfulDigits = 0;
            for (int i = 0; i < 999 && stream.MoveNext(); i++)
            {
                // Got non-hex value.
                if (stream.Current < 0) { value = 0; return false; }
                // Start calculating after first non-zero.
                if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
                // Total character count.
                count++;
            }

            // Found non-zero value.
            if (stream.Error) { value = 0; return false; }

            // Restart
            stream.Reset();

            // Too many characters
            if (meaningfulDigits >= 25) { value = 0; return false; }

            int lo = 0, mid = 0, hi = 0;
            while (count > 24 && stream.MoveNext())
            {
                // decimal can fit 96bit value, any non-zero over 96bits cannot be parsed.
                if (stream.Current != 0) { value = 0; return false; }
                count--;
            }
            while (count > 16 && stream.MoveNext())
            {
                hi = (hi << 4) | stream.Current;
                count--;
            }
            while (count > 8 && stream.MoveNext())
            {
                mid = (mid << 4) | stream.Current;
                count--;
            }
            while (count > 0 && stream.MoveNext())
            {
                lo = (lo << 4) | stream.Current;
                count--;
            }

            value = new decimal(lo, mid, hi, false, 0);
            return !stream.Error;
        }


        /// <summary>
        /// Parse a string to long. String must not start with '0x'.
        /// Long can accomodate only 16 meaningful hex-decimals.
        /// </summary>
        /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
        /// <returns>value</returns>
        /// <exception cref="System.FormatException"></exception>
        public static ulong ToULong(String str)
        {
            HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

            // Calculate hex character count
            int count = 0;
            // Calculate meaningful number characters
            int meaningfulDigits = 0;
            for (int i = 0; i < 999 && stream.MoveNext(); i++)
            {
                // Got non-hex value.
                if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");
                // Start calculating after first non-zero.
                if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
                // Total character count.
                count++;
            }

            // Found non-zero value.
            if (stream.Error) if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");

            // Restart
            stream.Reset();

            // Too many characters
            if (meaningfulDigits >= 17) throw new System.FormatException("Hex value too large to fit into decimal.");

            ulong value = 0UL;
            while (count > 16 && stream.MoveNext())
            {
                // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
                if (stream.Current != 0) throw new System.FormatException("Hex value too large to fit into decimal.");
                count--;
            }
            while (count > 0 && stream.MoveNext())
            {
                ulong hex = (ulong)stream.Current;
                value = value << 4 | hex;
                count--;
            }

            return value;
        }

        /// <summary>
        /// Parse a string to long. String must not start with '0x'.
        /// Long can accomodate only 16 meaningful hex-decimals.
        /// </summary>
        /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
        /// <param name="value">result value</param>
        /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
        public static bool TryParseToULong(String str, out ulong value)
        {
            HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

            // Calculate hex character count
            int count = 0;
            // Calculate meaningful number characters
            int meaningfulDigits = 0;
            for (int i = 0; i < 999 && stream.MoveNext(); i++)
            {
                // Got non-hex value.
                if (stream.Current < 0) { value = default(ulong); return false; }
                // Start calculating after first non-zero.
                if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
                // Total character count.
                count++;
            }

            // Found non-zero value.
            if (stream.Error) { value = default(ulong); return false; }

            // Restart
            stream.Reset();

            // Too many characters
            if (meaningfulDigits >= 17) { value = default(ulong); return false; }

            value = 0UL;
            while (count > 16 && stream.MoveNext())
            {
                // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
                if (stream.Current != 0) { value = default(ulong); return false; }
                count--;
            }
            while (count > 0 && stream.MoveNext())
            {
                ulong hex = (ulong)stream.Current;
                value = value << 4 | hex;
                count--;
            }

            return true;
        }


    }

    /// <summary>
    /// Enumerable of hex characters (0..15)
    /// </summary>
    public struct HexEnumerable : IEnumerable<int>
    {
        /// <summary>
        /// Character source
        /// </summary>
        public readonly IEnumerable<char> charStream;

        /// <summary>
        /// Create enumerable
        /// </summary>
        /// <param name="charStream"></param>
        public HexEnumerable(IEnumerable<char> charStream)
        {
            this.charStream = charStream;
        }

        /// <summary>
        /// Get enumerator. If charStream is String, no heap objects are allocated.
        /// </summary>
        /// <returns>Hex enumerator</returns>
        public HexEnumerator GetEnumerator() => new HexEnumerator(charStream is String str ? str.GetEnumerator() : charStream.GetEnumerator());
        IEnumerator IEnumerable.GetEnumerator() => new HexEnumerator(charStream is String str ? str.GetEnumerator() : charStream.GetEnumerator());
        IEnumerator<int> IEnumerable<int>.GetEnumerator() => new HexEnumerator(charStream is String str ? str.GetEnumerator() : charStream.GetEnumerator());
    }

    /// <summary>
    /// Reads hex characters (0..15) until end of stream.
    /// 
    /// Returns -1 for any character that is not hexadecimal (0..9, a..f, A..F).
    /// 
    /// Error flag is raised to true if encountered a non-hexadecimal character.
    /// </summary>
    public struct HexEnumerator : IEnumerator<int>
    {
        /// <summary>
        /// Current character (0..15)
        /// </summary>
        public int Current => current;

        /// <summary>
        /// Is parse error detected
        /// </summary>
        public bool Error => error;

        bool error;
        IEnumerator<char> charStream;
        CharEnumerator charStream2;
        object IEnumerator.Current => current;

        int current;
        bool eos;

        /// <summary>
        /// Create enumerator
        /// </summary>
        /// <param name="charStream"></param>
        public HexEnumerator(IEnumerator<char> charStream)
        {
            this.charStream = charStream ?? throw new ArgumentNullException(nameof(charStream));
            this.charStream2 = null;
            current = 0;
            error = false;
            eos = false;
        }

        /// <summary>
        /// Create enumerator
        /// </summary>
        /// <param name="charStream"></param>
        public HexEnumerator(CharEnumerator charStream)
        {
            this.charStream = null;
            this.charStream2 = charStream;
            current = 0;
            error = false;
            eos = false;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (charStream != null) charStream.Dispose();
            else charStream2.Dispose();
        }

        /// <summary>
        /// Move next
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (eos) return false;
            int value;
            if (charStream != null)
            {
                if (charStream != null && !charStream.MoveNext()) { current = -1; eos = true; return false; }
                value = (int)charStream.Current;
            }
            else
            {
                if (charStream2 != null && !charStream2.MoveNext()) { current = -1; eos = true; return false; }
                value = (int)charStream2.Current;
            }
            current = (value >= 48 && value <= 57) ? value - 48 : value >= 65 && value <= 70 ? value - 55 : value >= 97 && value <= 102 ? value - 87 : -1;
            error |= current < 0;
            return true;
        }

        /// <summary>
        /// Reset enumerator.
        /// </summary>
        public void Reset()
        {
            if (charStream != null) charStream.Reset(); else charStream2.Reset();
            current = 0;
            error = false;
            eos = false;
        }
    }

}
