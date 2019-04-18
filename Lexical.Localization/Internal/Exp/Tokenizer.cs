// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Exp
{
    /// <summary>
    /// Token kinds
    /// http://www.theasciicode.com.ar/ascii-printable-characters/braces-curly-brackets-opening-ascii-code-123.html
    /// </summary>
    [Flags]
    public enum TokenKind : UInt64
    {
        /// <summary>Undetermined</summary>
        Unset = 0UL,

        /// <summary>32434234234234</summary>
        IntegerLiteral = 1UL << 0,
        /// <summary>0x00ff</summary>
        HexIntegerLiteral = 1UL << 1,
        /// <summary>0.23e+232.1</summary>
        FloatLiteral = 1UL << 3,
        /// <summary>"Hello \nworld"</summary>
        StringLiteral = 1UL << 4,
        /// <summary>"""Hello world""""</summary>
        LongStringLiteral = 1UL << 5,
        /// <summary>Name\ Escaped \"stuff\"</summary>
        NameLiteral = 1UL << 6,
        /// <summary>@Namespace.Name</summary>
        AtNameLiteral = 1UL << 7,
        /// <summary>@"{http://xml}element"</summary>
        AtQuotedNameLiteral = 1UL << 8,
        /// <summary>@"""name"""</summary>
        AtLongQuotedNameLiteral = 1UL << 9,
        /// <summary>// Line Comment</summary>
        LineCommentLiteral = 1UL << 10,
        /// <summary>/* Block Comment */</summary>
        BlockCommentLiteral = 1UL << 11,
        /// <summary>//// Document Comment</summary>
        DocumentCommentLiteral = 1UL << 12,
        /// <summary>Whitespace other than line feed \t nbsp</summary>
        Whitespace = 1UL << 13,
        /// <summary>line feed characters \n \r</summary>
        Linefeed = 1UL << 14,

        /// <summary>40 (</summary>
        LParenthesis = 1UL << 20,
        /// <summary>41 )</summary>
        RParenthesis = 1UL << 21,
        /// <summary>91 [</summary>
        LBracket = 1UL << 22,
        /// <summary>93 ]</summary>
        RBracket = 1UL << 23,
        /// <summary>123 {</summary>
        LBrace = 1UL << 24,
        /// <summary>125 }</summary>
        RBrace = 1UL << 25,
        /// <summary>60 &lt;</summary>
        Lt = 1UL << 26,
        /// <summary>62 &gt;</summary>
        Gt = 1UL << 27,
        /// <summary>61 =</summary>
        Equals = 1UL << 28,
        /// <summary>44 ,</summary>
        Comma = 1UL << 29,
        /// <summary>58 :</summary>
        Colon = 1UL << 30,
        /// <summary>59 ;</summary>
        Semicolon = 1UL << 31,
        /// <summary>33 !</summary>
        Exclamation = 1UL << 32,
        /// <summary>42 *</summary>
        Asterisk = 1UL << 33,
        /// <summary>63 ?</summary>
        Questionmark = 1UL << 34,
        /// <summary>35 #</summary>
        Hash = 1UL << 35,
        /// <summary>45 -</summary>
        Minus = 1UL << 36,
        /// <summary>92 \</summary>
        Backslash = 1UL << 37,
        /// <summary>47 /</summary>
        Slash = 1UL << 38,
        /// <summary>124 |</summary>
        Pipe = 1UL << 39,
        /// <summary>43 +</summary>
        Plus = 1UL << 40,
        /// <summary>94 ^</summary>
        Caret = 1UL << 41,
        /// <summary>46 .</summary>
        Dot = 1UL << 42,
        /// <summary>38 &amp;</summary>
        Ampersand = 1UL << 43,
        /// <summary>94 ^</summary>
        Accent = 1UL << 44,
        /// <summary>126 ~</summary>
        Tilde = 1UL << 45,
        /// <summary>==</summary>
        Equals2 = 1UL << 46,
        /// <summary>!=</summary>
        InEquals2 = 1UL << 47,
        /// <summary>&lt;=</summary>
        LtOrEq = 1UL << 48,
        /// <summary>&gt;=</summary>
        GtOrEq = 1UL << 49,
        /// <summary>37 %</summary>
        Percent = 1UL << 50,
        /// <summary>&lt;&lt;</summary>
        Lt2 = 1UL << 51,
        /// <summary>&gt;Gt;</summary>
        Gt2 = 1UL << 52,
        /// <summary>pow</summary>
        Pow = 1UL << 53,
        /// <summary>@</summary>
        At = 1UL << 54,
        /// <summary>§</summary>
        Section = 1UL << 55,
        /// <summary>…</summary>
        Ellipsis = 1UL << 56,
        /// <summary>$</summary>
        Dollar = 1UL << 57,
        /// <summary>..</summary>
        Range = 1UL << 58,

        /// <summary></summary>
        Unknown = 1UL << 63,

        /// <summary></summary>
        NonEssential = Whitespace | Linefeed | LineCommentLiteral | BlockCommentLiteral | DocumentCommentLiteral
    }

    /// <summary>
    /// Represent a segment of a string.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class Token
    {
        /// <summary>
        /// Type of this token
        /// </summary>
        public TokenKind Kind;

        /// <summary>
        /// Substring
        /// </summary>
        public StringStream Text;

        /// <summary>
        /// Number of characters in the token.
        /// </summary>
        public int Count => Text.Length;

        /// <summary>
        /// X,Y coodinates
        /// </summary>
        public int BeginLine, EndLine, BeginColumn, EndColumn;

        /// <summary>
        /// StringLiteral|NameLiteral : unescaped text.
        /// IntegerLiteral|PosIntegerLiteral : Decimal
        /// FloatLiteral : Double
        /// other : substring
        /// </summary>
        /// <exception cref="Exception">If parsing failed.</exception>
        public object Value
        {
            get
            {
                if (cachedValue != null) return cachedValue;
                switch (Kind)
                {
                    case TokenKind.IntegerLiteral:
                        {
                            long value;
                            if (long.TryParse(Text, NumberStyles.Number, CultureInfo.InvariantCulture, out value)) cachedValue = value;
                            return cachedValue;
                            // Returns null if string doesn't fit into decimal.
                        }
                    case TokenKind.FloatLiteral:
                        {
                            double value;
                            if (Double.TryParse(Text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent,
                                CultureInfo.InvariantCulture, out value)) cachedValue = value;
                            return cachedValue;
                            // Returns null if string doesn't fit into double.
                        }
                    case TokenKind.AtNameLiteral: return cachedValue = TokenUtils.NameLiteralUnescape.Replace(Text.Str.Substring(1), TokenUtils.UnescapeChar);
                    case TokenKind.AtQuotedNameLiteral: return cachedValue = TokenUtils.UnescapeStringLiteral(Text.Str.Substring(Text.Index + 2, Text.Length - 3));
                    case TokenKind.AtLongQuotedNameLiteral: return cachedValue = TokenUtils.UnescapeStringLiteral(Text.Str.Substring(Text.Index + 4, Text.Length - 7));
                    case TokenKind.NameLiteral: return cachedValue = TokenUtils.UnescapeNameLiteral(Text);
                    case TokenKind.StringLiteral: return cachedValue = TokenUtils.UnescapeStringLiteral(Text.Str.Substring(Text.Index + 1, Text.Length - 2));
                    case TokenKind.LongStringLiteral: return cachedValue = TokenUtils.UnescapeStringLiteral(Text.Str.Substring(Text.Index + 3, Text.Length - 6));
                    case TokenKind.HexIntegerLiteral:
                        {
                            String str = Text.Str.Substring(Text.Index + 2, Text.Length - 2);
                            decimal value;
                            // 0x123
                            // 0x0123
                            if (Hex.TryParseToDecimal(str, out value))
                                cachedValue = value;
                            return cachedValue;
                        }
                    default: return cachedValue = Text.ToString();
                }
            }
            set
            {
                cachedValue = value;

            }
        }
        private object cachedValue;

        /// <summary>
        /// Print segment
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{Kind}: {Text.ToString()}";
    }

    /// <summary>
    /// Contains utilities for escaping and unescaping name literals.
    /// </summary>
    // 
    // The following charaters are escaped: \/:!#-+(){}[]<>&|",=*^? and also whitespace ' ', line-feed (\n) carriage return (\r) and tab (\t).
    // Escape character is \.
    // 
    public static class TokenUtils
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
        //public static Regex Integer = new Regex(@"^[+-]?\d+", opts);
        /// <summary></summary>
        public static Regex Integer = new Regex(@"^\d+", opts);
        /// <summary></summary>
        public static Regex Hex = new Regex(@"0[xX][0-9a-fA-F]+", opts); // 0x000000000000000
        //public static Regex Float = new Regex(@"^[-+]?\d+((\.\d+)([eE][-+]?\d+)?)|((\.\d+)?([eE][-+]?\d+))", opts); // "5.0" "5e0" "5e0.0"
        /// <summary></summary>
        public static Regex Float = new Regex(@"^\d+((\.\d+)([eE][-+]?\d+)?)|((\.\d+)?([eE][-+]?\d+))", opts); // "5.0" "5e0" "5e0.0"
        /// <summary></summary>
        public static Regex FieldModifiers = new Regex(@"^[\#\-]{1,2}", opts);
        /// <summary></summary>
        public static Regex Whitespace = new Regex(@"^[ \t\u00ff]+", opts);
        /// <summary></summary>
        public static Regex Linefeed = new Regex(@"^\r*\n", opts);
        /// <summary></summary>
        public static Regex Pow = new Regex(@"^pow", opts);
        /// <summary></summary>
        public static Regex Ellipsis = new Regex(@"^(…|\.\.\.)", opts);
        /// <summary></summary>
        public static Regex Range = new Regex(@"^\.\.", opts);

        /// <summary></summary>
        public static Regex NameLiteral = new Regex(@"^(\\[0-9])?([^\./\\:\!#\-\+\^(\)\{\}\[\]&\<\>\|"",\n\t\r=\*\^\?; ]|(\\[\."",ntr/\\:\!#&\-\+\^,\(\)\{\}\[\]\|=\*\^\?; ]))+", opts);
        /// <summary></summary>
        public static Regex AtNameLiteral = new Regex(@"@([^/\\:\!#-\+\^(\)\{\}\[\]&\<\>\|"",\n\t\r=\*\^\? ]|(\\["",ntr/\\:\!#&-\+\^,\(\)\{\}\[\]\|=\*\^\? ]))+", opts);
        /// <summary></summary>
        public static Regex NameLiteralEscape = new Regex(@"[""\!,\n\t\r\\:#&-\+\^,\(\)\{\}\[\]\|=\*\^\? ]", opts);
        /// <summary></summary>
        public static Regex NameLiteralUnescape = new Regex(@"\\(.)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        /// <summary></summary>
        public static Regex AtQuotedNameLiteral = new Regex(@"^@""([^\\""\n\t\r]|(\\[""ntr\\]))+""", opts);
        /// <summary></summary>
        public static Regex AtQuotedLongNameLiteral = new Regex("^@\"\"\".*\"\"\"+", opts);

        /// <summary></summary>
        public static Regex StringLiteral = new Regex(@"^""([^\\""\n\r]|\\[""ntr\\]|\\u[0-9a-fA-F]{4}|\\x[0-9a-fA-F]{2})+""", opts);
        /// <summary></summary>
        public static Regex LongStringLiteral = new Regex("^\"\"\".*\"\"\"+", opts);
        /// <summary></summary>
        public static Regex StringLiteralEscape = new Regex(@"[""\n\t\r\\]", opts);
        /// <summary></summary>
        public static Regex StringLiteralUnescape = new Regex(@"\\(u[0-9a-fA-F]{4}|x[0-9a-fA-F]{2}|.)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        /// <summary></summary>
        public static Regex LineCommentLiteral = new Regex("^//[^\r\n]*", opts);
        /// <summary></summary>
        public static Regex DocumentCommentLiteral = new Regex("^///(?!/)[^\r\n]*", opts);
        /// <summary></summary>
        public static Regex BlockCommentLiteral = new Regex("^/\\*.*\\*/", opts);

        /// <summary></summary>
        public static String EscapeNameLiteral(String input) => NameLiteralEscape.Replace(input, EscapeChar);
        /// <summary></summary>
        public static String UnescapeNameLiteral(String input) => NameLiteralUnescape.Replace(input, UnescapeChar);
        /// <summary></summary>
        public static String EscapeStringLiteral(String input) => StringLiteralEscape.Replace(input, EscapeChar);
        /// <summary></summary>
        public static String UnescapeStringLiteral(String input) => StringLiteralUnescape.Replace(input, UnescapeChar);

        /// <summary></summary>
        public static String EscapeChar(Match m) => @"\" + m.Value;
        /// <summary></summary>
        public static String UnescapeChar(Match m)
        {
            string capture = m.Groups[1].Value;
            switch (capture[0])
            {
                case 't': return "\t";
                case 'n': return "\n";
                case 'r': return "\r";
                case 'u':
                    char ch = (char)Lexical.Localization.Internal.Hex.ToUInt(capture.Substring(1));
                    return new string(ch, 1);
                case 'x':
                    char c = (char)Lexical.Localization.Internal.Hex.ToUInt(capture.Substring(1));
                    return new string(c, 1);
                default: return capture;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Double? StringToDouble(String str)
            => str == null ? null : (double?)Double.Parse(str, CultureInfo.InvariantCulture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String DoubleToString(Double? value)
            => value == null ? null : ((double)value).ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Decimal? StringToDecimal(String str)
            => str == null ? null : (decimal?)Decimal.Parse(str, CultureInfo.InvariantCulture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String DecimalToString(Decimal? value)
            => value == null ? null : ((decimal)value).ToString(CultureInfo.InvariantCulture);

    }

    /// <summary>
    /// Converts a String or StringStream into an enumerable of tokens.
    /// </summary>
    public class StringToTokenEnumerable : IEnumerable<Token>, IEnumerable
    {
        StringStream text;

        /// <summary>
        /// Reads text into an array of tokens.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Token[] ToArray(StringStream text)
        {
            List<Token> list = new List<Token>(text.Length / 2);
            list.AddRange(new StringToTokenEnumerable(text));
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public StringToTokenEnumerable(StringStream text)
        {
            this.text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public StringToTokenEnumerable(String text, int index, int length)
        {
            this.text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Token> GetEnumerator()
            => new StringToTokenEnumerator(text);

        IEnumerator IEnumerable.GetEnumerator()
            => new StringToTokenEnumerator(text);
    }

    /// <summary>
    /// 
    /// </summary>
    public class StringToTokenEnumerator : IEnumerator<Token>, IEnumerator
    {
        /// <summary>
        /// 
        /// </summary>
        public Token Current => current;
        object IEnumerator.Current => current;

        StringStream orig, stream;
        Token current;

        int line, column;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public StringToTokenEnumerator(StringStream text)
        {
            this.orig = text;
            stream = text;
            line = 1; column = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            orig.Str = null; stream.Str = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            this.current = null;
            if (stream.Length <= 0) return false;

            int startIndex = stream.Index;
            current = new Token { BeginColumn = column, BeginLine = line };

            Match match;

            if ((match = stream.Take(TokenUtils.Hex)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.HexIntegerLiteral;
            }
            else if ((match = stream.Take(TokenUtils.Float)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.FloatLiteral;
            }
            else if ((match = stream.Take(TokenUtils.Integer)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.IntegerLiteral;
            }
            else if ((match = stream.Take(TokenUtils.DocumentCommentLiteral)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.DocumentCommentLiteral;
            }
            else if ((match = stream.Take(TokenUtils.LineCommentLiteral)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.LineCommentLiteral;
            }
            else if ((match = stream.Take(TokenUtils.BlockCommentLiteral)) != null)
            {
                int endIndex = startIndex + match.Length;
                for (int i = startIndex; i < endIndex; i++)
                {
                    char ch = stream.Str[i];
                    switch (ch)
                    {
                        case '\n': line++; column = 0; break;
                        case '\r': column = 0; break;
                        case '\u00ff': break;
                        case '\t': column += 4; break;
                        default: column++; break;
                    }
                }
                current.Kind = TokenKind.BlockCommentLiteral;
            }
            else if ((match = stream.Take(TokenUtils.AtQuotedLongNameLiteral)) != null)
            {
                current.Kind = TokenKind.AtLongQuotedNameLiteral;
                int endIndex = startIndex + match.Length;
                for (int i = startIndex; i < endIndex; i++)
                {
                    char ch = stream.Str[i];
                    switch (ch)
                    {
                        case '\n': line++; column = 0; break;
                        case '\r': column = 0; break;
                        case '\u00ff': break;
                        case '\t': column += 4; break;
                        default: column++; break;
                    }
                }
            }
            else if ((match = stream.Take(TokenUtils.AtQuotedNameLiteral)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.AtQuotedNameLiteral;
            }
            else if ((match = stream.Take(TokenUtils.AtNameLiteral)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.AtNameLiteral;
            }
            else if ((match = stream.Take(TokenUtils.NameLiteral)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.NameLiteral;
            }
            else if ((match = stream.Take(TokenUtils.StringLiteral)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.StringLiteral;
            }
            else if ((match = stream.Take(TokenUtils.LongStringLiteral)) != null)
            {
                current.Kind = TokenKind.LongStringLiteral;
                int endIndex = startIndex + match.Length;
                for (int i = startIndex; i < endIndex; i++)
                {
                    char ch = stream.Str[i];
                    switch (ch)
                    {
                        case '\n': line++; column = 0; break;
                        case '\r': column = 0; break;
                        case '\u00ff': break;
                        case '\t': column += 4; break;
                        default: column++; break;
                    }
                }
            }
            else if ((match = stream.Take(TokenUtils.Whitespace)) != null)
            {
                int endIndex = startIndex + match.Length;
                for (int i = startIndex; i < endIndex; i++)
                {
                    char ch = stream.Str[i];
                    switch (ch)
                    {
                        case '\u00ff': break;
                        case '\t': column += 4; break;
                        default: column++; break;
                    }
                }
                current.Kind = TokenKind.Whitespace;
            }
            else if ((match = stream.Take(TokenUtils.Linefeed)) != null)
            {
                int endIndex = startIndex + match.Length;
                for (int i = startIndex; i < endIndex; i++)
                {
                    char ch = stream.Str[i];
                    switch (ch)
                    {
                        case '\n': line++; column = 0; break;
                        case '\r': column = 0; break;
                    }
                }
                current.Kind = TokenKind.Linefeed;
            }
            else if ((match = stream.Take(TokenUtils.Pow)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.Pow;
            }
            else if ((match = stream.Take(TokenUtils.Ellipsis)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.Ellipsis;
            }
            else if ((match = stream.Take(TokenUtils.Range)) != null)
            {
                column += match.Length;
                current.Kind = TokenKind.Range;
            }
            else
            {
                char ch = stream.TakeChar();
                switch (ch)
                {
                    case '(': current.Kind = TokenKind.LParenthesis; break;
                    case ')': current.Kind = TokenKind.RParenthesis; break;
                    case '[': current.Kind = TokenKind.LBracket; break;
                    case ']': current.Kind = TokenKind.RBracket; break;
                    case '{': current.Kind = TokenKind.LBrace; break;
                    case '}': current.Kind = TokenKind.RBrace; break;
                    case '<':
                        current.Kind = TokenKind.Lt;
                        // <<
                        if (stream.PeekChar() == '<') { stream.TakeChar(); current.Kind = TokenKind.Lt2; }
                        // <=
                        if (stream.PeekChar() == '=') { stream.TakeChar(); current.Kind = TokenKind.LtOrEq; }
                        break;
                    case '>':
                        current.Kind = TokenKind.Gt;
                        // >>
                        if (stream.PeekChar() == '>') { stream.TakeChar(); current.Kind = TokenKind.Gt2; }
                        // >=
                        if (stream.PeekChar() == '=') { stream.TakeChar(); current.Kind = TokenKind.GtOrEq; }
                        break;
                    case '=':
                        current.Kind = TokenKind.Equals;
                        // ==
                        if (stream.PeekChar() == '=') { stream.TakeChar(); current.Kind = TokenKind.Equals2; }
                        break;
                    case ',': current.Kind = TokenKind.Comma; break;
                    case ':': current.Kind = TokenKind.Colon; break;
                    case ';': current.Kind = TokenKind.Semicolon; break;
                    case '!':
                        current.Kind = TokenKind.Exclamation;
                        // !=
                        if (stream.PeekChar() == '=') { stream.TakeChar(); current.Kind = TokenKind.InEquals2; }
                        break;
                    case '*': current.Kind = TokenKind.Asterisk; break;
                    case '?': current.Kind = TokenKind.Questionmark; break;
                    case '#': current.Kind = TokenKind.Hash; break;
                    case '+': current.Kind = TokenKind.Plus; break;
                    case '-': current.Kind = TokenKind.Minus; break;
                    case '\\': current.Kind = TokenKind.Backslash; break;
                    case '/': current.Kind = TokenKind.Slash; break;
                    case '.': current.Kind = TokenKind.Dot; break;
                    case '&': current.Kind = TokenKind.Ampersand; break;
                    case '^': current.Kind = TokenKind.Accent; break;
                    case '|': current.Kind = TokenKind.Pipe; break;
                    case '%': current.Kind = TokenKind.Percent; break;
                    case '~': current.Kind = TokenKind.Tilde; break;
                    case '@': current.Kind = TokenKind.At; break;
                    case '§': current.Kind = TokenKind.Section; break;
                    case '$': current.Kind = TokenKind.Dollar; break;
                    case '…': current.Kind = TokenKind.Ellipsis; break;
                    default: current.Kind = TokenKind.Unknown; break;
                }
                column++;
            }

            current.EndColumn = column;
            current.EndLine = line;
            current.Text = new StringStream { Str = stream.Str, Index = startIndex, Length = stream.Index - startIndex };

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            line = 1; column = 1;
            stream = orig;
        }
    }

    /// <summary>
    /// Represents a segment of tokens. 
    /// </summary>
    [DebuggerDisplay("{DebugPrint()}")]
    public struct Tokens : IEnumerable<Token>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool EndOfStream => Index >= Count;

        /// <summary>
        /// 
        /// </summary>
        public String Text => Src.Count == 0 ? null : Src[0].Text.Str;

        /// <summary>
        /// 
        /// </summary>
        public Token Current => Index >= Count ? null : Src[Index];

        /// <summary>
        /// 
        /// </summary>
        public int EndIndex => StartIndex + Count;

        /// <summary>
        /// Source array of tokens.
        /// </summary>
        public readonly IList<Token> Src;

        /// <summary>
        /// First index represented by this reader.
        /// </summary>
        public readonly int StartIndex;

        /// <summary>
        /// Number of tokens to read from Src.
        /// </summary>
        public readonly int Count;

        /// <summary>
        /// Current index, relative to Src-array.
        /// </summary>
        public int Index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public Tokens(String text) : this(StringToTokenEnumerable.ToArray(text)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        public Tokens(IList<Token> tokens) : this(tokens, 0, tokens.Count) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="startIndex"></param>
        public Tokens(IList<Token> tokens, int startIndex) : this(tokens, startIndex, tokens.Count - startIndex) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public Tokens(IList<Token> tokens, int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > tokens.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (startIndex + count > tokens.Count) throw new ArgumentOutOfRangeException(nameof(count));

            this.Src = tokens ?? throw new ArgumentNullException(nameof(tokens));
            StartIndex = Index = startIndex;
            Count = count;
        }

        /// <summary>
        /// Try to take next token from stream.
        /// </summary>
        /// <returns>Token or null</returns>
        public Token Take()
        {
            Token token = Current;
            if (token != null) Index++;
            return token;
        }

        /// <summary>
        /// Take as many tokens as fits the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>List of tokens</returns>
        public Tokens TakeAll(TokenKind criteria)
        {
            int startIndex = Index;
            while (Current != null && ((Current.Kind & criteria) != 0)) Index++;
            return Between(startIndex, Index);
        }

        /// <summary>
        /// Take as many tokens as fits the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>List of tokens</returns>
        public Tokens TakeAllOnSameLine(TokenKind criteria)
        {
            if (Current == null) return Between(Index, Index);
            int startIndex = Index;
            int line = Index == 0 ? Current.EndLine : Src[Index - 1].EndLine;
            while (
                Current != null &&
                ((Current.Kind & criteria) != 0) &&
                (Index == startIndex || Current.BeginLine <= line)) Index++;
            return Between(startIndex, Index);
        }

        /// <summary>
        /// Try to take token from stream.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>Token or null</returns>
        public Token Take(TokenKind criteria)
        {
            Token token = Current;
            if (token != null && (token.Kind & criteria) != 0) { Index++; return token; }
            return null;
        }

        /// <summary>
        /// Take function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">a reference to a reader that the caller may modify. Caller doesn't need to reset position.</param>
        /// <returns></returns>
        public delegate T Taker0<T>(ref Tokens reader) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <param name="reader"></param>
        /// <param name="a0"></param>
        /// <returns></returns>
        public delegate T Taker1<T, A0>(ref Tokens reader, A0 a0) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <param name="reader"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <returns></returns>
        public delegate T Taker2<T, A0, A1>(ref Tokens reader, A0 a0, A1 a1) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <param name="reader"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public delegate T Taker3<T, A0, A1, A2>(ref Tokens reader, A0 a0, A1 a1, A2 a2) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <param name="reader"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <returns></returns>
        public delegate T Taker4<T, A0, A1, A2, A3>(ref Tokens reader, A0 a0, A1 a1, A2 a2, A3 a3) where T : class;

        /// <summary>
        /// Try to take an object. If successful, moves index position forward.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns>T or null</returns>
        public T Take<T>(Taker0<T> func) where T : class
        {
            Tokens copy = this;
            T value = func(ref copy);
            if (value != null) this = copy;
            return value;
        }

        /// <summary>
        /// Try to take an object. If successful, moves index position forward.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <param name="func"></param>
        /// <param name="a0"></param>
        /// <returns>T or null</returns>
        public T Take<T, A0>(Taker1<T, A0> func, A0 a0) where T : class
        {
            Tokens copy = this;
            T value = func(ref copy, a0);
            if (value != null) this = copy;
            return value;
        }

        /// <summary>
        /// Try to take an object. If successful, moves index position forward.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <param name="func"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <returns>T or null</returns>
        public T Take<T, A0, A1>(Taker2<T, A0, A1> func, A0 a0, A1 a1) where T : class
        {
            Tokens copy = this;
            T value = func(ref copy, a0, a1);
            if (value != null) this = copy;
            return value;
        }

        /// <summary>
        /// Try to take an object. If successful, moves index position forward.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <param name="func"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns>T or null</returns>
        public T Take<T, A0, A1, A2>(Taker3<T, A0, A1, A2> func, A0 a0, A1 a1, A2 a2) where T : class
        {
            Tokens copy = this;
            T value = func(ref copy, a0, a1, a2);
            if (value != null) this = copy;
            return value;
        }

        /// <summary>
        /// Try to take an object. If successful, moves index position forward.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <param name="func"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <returns>T or null</returns>
        public T Take<T, A0, A1, A2, A3>(Taker4<T, A0, A1, A2, A3> func, A0 a0, A1 a1, A2 a2, A3 a3) where T : class
        {
            Tokens copy = this;
            T value = func(ref copy, a0, a1, a2, a3);
            if (value != null) this = copy;
            return value;
        }

        /// <summary>
        /// Returns true if this one of criteris is found.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public bool ContainsKind(TokenKind criteria)
        {
            for (int i = StartIndex; i < EndIndex; i++)
                if ((Src[i].Kind & criteria) != 0) return true;
            return false;
        }

        /// <summary>
        /// Get new range between indices.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public Tokens Between(int startIndex, int endIndex)
            => new Tokens(Src, startIndex, endIndex - startIndex);

        /// <summary>
        /// Scans token range and returns a range between startindex and 
        /// the last token to match token kind criteria.
        /// 
        /// <code>
        /// tokens.UntilLast(~TokenKind.NonEssential)
        /// </code>
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Tokens UntilLast(TokenKind criteria)
        {
            int ix = StartIndex;
            for (int i = StartIndex; i < EndIndex; i++)
                if ((Src[i].Kind & criteria) != 0) ix = i + 1;
            return Between(StartIndex, ix);
        }

        IEnumerator<Token> IEnumerable<Token>.GetEnumerator() => new TokenEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new TokenEnumerator(this);
        TokenEnumerator GetEnumerator() => new TokenEnumerator(this);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerator<Token> Filter(TokenKind criteria)
            => new TokenEnumerator(this, criteria);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dst"></param>
        public void AppendTo(IList<Token> dst)
        {
            for (int i = StartIndex; i < EndIndex; i++)
                dst.Add(Src[i]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Count == 0) return "";
            string text = Text;
            int start_index = Src[StartIndex].Text.Index;
            int end_index = Src[Math.Max(StartIndex, EndIndex - 1)].Text.EndIndex;
            return text.Substring(start_index, end_index - start_index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DebugPrint()
        {
            if (Count == 0) return "";
            string text = Text;
            int start_index = Src[Math.Max(StartIndex, Index)].Text.Index;
            int end_index = Src[Math.Max(StartIndex, EndIndex - 1)].Text.EndIndex;
            return text.Substring(start_index, end_index - start_index);
        }

        /// <summary>
        /// Extends current stream of tokens with the tokens in the argument. 
        /// 
        /// If current stream and the argument stream are constructed from same source,
        /// then the start and end indices are updated to include the argument.
        /// 
        /// If current stream and the argument stream are constructed from different source,
        /// then a new text stream is constructed, and argument stream is appended to the current stream.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="toAppendWith"></param>
        public static void ExpandWith(ref Tokens current, Tokens toAppendWith)
        {
            if (toAppendWith.Count == 0) return;
            if (current.Src == toAppendWith.Src)
            {
                // Same source stream
                int newStartIndex = Math.Min(current.StartIndex, toAppendWith.StartIndex);
                int newEndIndex = Math.Max(current.EndIndex, toAppendWith.EndIndex);
                if (newStartIndex != current.StartIndex || newEndIndex != current.EndIndex)
                    current = new Tokens(current.Src, newStartIndex, newEndIndex - newStartIndex);
            }
            else
            if (current.Src == null && current.Count == 0)
            {
                // Current is uninitialized
                current = toAppendWith;
            }
            else
            {
                // Different source stream. Append to new string.
                int newCount = current.Count + toAppendWith.Count;
                Token[] newArray = new Token[newCount];
                for (int i = 0; i < current.Count; i++) newArray[i] = current.Src[i];
                for (int i = 0; i < toAppendWith.Count; i++) newArray[i + current.Count] = toAppendWith.Src[i];
                current = new Tokens(newArray, 0, newCount);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct TokenEnumerator : IEnumerator<Token>
    {
        Tokens tokens;
        TokenKind criteria;
        int index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        public TokenEnumerator(Tokens tokens)
        {
            this.tokens = tokens;
            index = tokens.StartIndex - 1;
            criteria = (TokenKind)UInt64.MaxValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="criteria"></param>
        public TokenEnumerator(Tokens tokens, TokenKind criteria)
        {
            this.tokens = tokens;
            index = tokens.StartIndex - 1;
            this.criteria = criteria;
        }

        /// <summary>
        /// 
        /// </summary>
        public Token Current => index >= tokens.StartIndex && index < tokens.EndIndex ? tokens.Src[index] : null;

        object IEnumerator.Current => index >= tokens.StartIndex && index < tokens.EndIndex ? tokens.Current : null;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            do
            {
                index++;
            } while (index < tokens.EndIndex && Current != null && ((Current.Kind & criteria) == 0));
            return index >= tokens.StartIndex && index < tokens.EndIndex && ((Current.Kind & criteria) != 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            index = tokens.StartIndex - 1;
        }
    }

}
