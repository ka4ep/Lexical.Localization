using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class IStringFormat_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0a
                IStringFormat stringFormat = CSharpFormat.Default;
                IString str = stringFormat.Parse("Hello, {0}.");
                #endregion Snippet_0a
            }
            {
                #region Snippet_0b
                ILine line = LineRoot.Global.Key("").Format("Hello, {0}.");
                #endregion Snippet_0b
            }
            {
                #region Snippet_1a
                IStringFormat stringFormat = TextFormat.Default;
                IString str = stringFormat.Parse("{in braces}");
                #endregion Snippet_1a
            }
            {
                #region Snippet_1b
                ILine line = LineRoot.Global.Key("").Text("{in braces}");
                #endregion Snippet_1b
            }

            {
                #region Snippet_2a
                IStringFormat stringFormat = CSharpFormat.Default;
                IString str = stringFormat.Parse("Hello, {0}.");
                #endregion Snippet_2a
                #region Snippet_2b
                ILine line = LineRoot.Global.Key("Hello").String(str);
                #endregion Snippet_2b
                #region Snippet_2c
                LineString lineString = line.Value("Corellia Melody").ResolveString();
                #endregion Snippet_2c
                #region Snippet_2d
                if (!lineString.Failed) Console.WriteLine(lineString.Value);
                #endregion Snippet_2d
            }
            {
                #region Snippet_3
                // Convert unescaped string "{not placeholder}" to IString
                IString ss = TextFormat.Default.Parse("{not placeholder}");
                // Escape with CSharpFormat to "\{not placeholder\}"
                string str = CSharpFormat.Default.Print(ss);
                // Convert escaped string back to IString
                IString cs = CSharpFormat.Default.Parse(str);
                // Unescape IString back to "{not placeholder}"
                string tt = TextFormat.Default.Print(cs);
                #endregion Snippet_3
                Console.WriteLine(tt);
            }
            {
                #region Snippet_4
                #endregion Snippet_4
            }
            {
                #region Snippet_5
                #endregion Snippet_5
            }
            {
                #region Snippet_6
                #endregion Snippet_6
            }
            {
                #region Snippet_7
                #endregion Snippet_7
            }
            {
                #region Snippet_8
                #endregion Snippet_8
            }
        }
    }

}
