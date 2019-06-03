// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Exp
{
    /// <summary>
    /// String segment, character span.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct StringStream
    {
        /// <summary>
        /// String
        /// </summary>
        public String Str;

        /// <summary>
        /// start index
        /// </summary>
        public int Index;

        /// <summary>
        /// Length of segment
        /// </summary>
        public int Length;

        /// <summary>
        /// End index
        /// </summary>
        public int EndIndex => Index + Length;

        /// <summary>
        /// Convert from string
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator StringStream(String str)
            => new StringStream(str);

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator String(StringStream str)
            => str.ToString();

        /// <summary>
        /// Create string segment
        /// </summary>
        /// <param name="str"></param>
        public StringStream(String str)
        {
            this.Str = str ?? throw new ArgumentNullException(nameof(str));
            this.Index = 0;
            this.Length = str.Length;
        }

        /// <summary>
        /// Create string segment
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        public StringStream(String str, int index)
        {
            if (index < 0 || index > str.Length) throw new ArgumentException(nameof(index));
            this.Str = str ?? throw new ArgumentNullException(nameof(str));
            this.Index = index;
            this.Length = str.Length - index;
        }

        /// <summary>
        /// Create string segment
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public StringStream(String str, int index, int length)
        {
            if (index < 0 || index > str.Length) throw new ArgumentException(nameof(index));
            if (length < 0 || length + index > str.Length) throw new ArgumentException(nameof(length));
            this.Str = str ?? throw new ArgumentNullException(nameof(str));
            this.Index = index;
            this.Length = length;
        }

        /// <summary>
        /// Peek next character
        /// </summary>
        /// <returns>char or '\0' </returns>
        public char PeekChar()
            => Index >= Str.Length ? (char)0 : Str[Index];

        /// <summary>
        /// Take next character
        /// </summary>
        /// <returns>char or '\0'</returns>
        public char TakeChar()
        {
            if (Index >= Str.Length) return (char)0;
            char ch = Str[Index];
            Index++; Length--;
            return ch;
        }

        /// <summary>
        /// Try to take a character. If succeeds, moves index one index.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>true, character was taken, index moved</returns>
        public bool Take(char ch)
        {
            if (Index >= Str.Length) return false;
            if (Str[Index] != ch) return false;
            Index++; Length--;
            return true;
        }

        /// <summary>
        /// Try to take a substring. If succeeds, moves index by length of string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>true, string was taken, index moved</returns>
        public bool Take(String s)
        {
            if (Index + s.Length > Str.Length) return false;
            for (int i = 0; i < s.Length; i++)
                if (Str[Index + i] != s[i]) return false;
            Index += s.Length; Length -= s.Length;
            return true;
        }

        /// <summary>
        /// Try to take a pattern. If succeeds, moves index.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns>Matcher, pattern was taken, if null, did not match</returns>
        public Match Take(Regex pattern)
        {
            Match match = pattern.Match(Str, Index, Length);
            if (match == null) return null;
            if (!match.Success) return null;
            if (match.Index != Index) return null;
            Index += match.Length; Length -= match.Length;
            return match;
        }

        /// <summary>
        /// Move cursor
        /// </summary>
        /// <param name="count"></param>
        public void Move(int count)
        {
            if (count + Index > EndIndex) throw new ArgumentException("Cannot move over EndIndex");
            if (count < -Index) throw new ArgumentException("Cannot move below index 0");
            Index += count; Length -= count;
        }

        /// <summary>
        /// Try to take an object with a reader function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>object with endposition given with IPosition</returns>
        public T Take<T>(Func<StringStream, T> reader) where T : IPosition
        {
            T token = reader(this);
            if (token == null) return default;
            Move(token.EndIndex - Index);
            return token;
        }

        /// <summary>
        /// Try to take an object with a reader function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <param name="reader"></param>
        /// <param name="arg0"></param>
        /// <returns>object with endposition given with IPosition</returns>
        public T Take<T, A0>(Func<StringStream, A0, T> reader, A0 arg0) where T : IPosition
        {
            T token = reader(this, arg0);
            if (token == null) return default;
            Move(token.EndIndex - Index);
            return token;
        }

        /// <summary>
        /// Try to take an object with a reader function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <param name="reader"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns>object with endposition given with IPosition</returns>
        public T Take<T, A0, A1>(Func<StringStream, A0, A1, T> reader, A0 arg0, A1 arg1) where T : IPosition
        {
            T token = reader(this, arg0, arg1);
            if (token == null) return default;
            Move(token.EndIndex - Index);
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        public interface IPosition
        {
            /// <summary>
            /// End index
            /// </summary>
            int EndIndex { get; }
        }

        /// <summary>
        /// Create a substring from relative index.
        /// </summary>
        /// <param name="index">index relative to index of current index</param>
        /// <returns>new fragment</returns>
        public StringStream SubstringR(int index)
            => new StringStream(Str, Index + index, Length - index - Index);

        /// <summary>
        /// Create a substring from index relative to the underlying string.
        /// </summary>
        /// <param name="index">index relative to index of current index</param>
        /// <returns>new fragment</returns>
        public StringStream SubstringA(int index)
            => new StringStream(Str, index, EndIndex - index);

        /// <summary>
        /// Return the segment that is represented by this stream.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Str.Substring(Index, Length);

        /// <summary>
        /// Calculate hash of segment.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => (Str == null ? 0 : Str.GetHashCode()) ^ Index ^ ~Length;

        /// <summary>
        /// Equals comparison of segment
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is StringStream ss ? ss.Str == Str && ss.Index == Index && ss.Length == Length : false;
    }
}
