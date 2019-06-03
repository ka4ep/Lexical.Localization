using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class Tutorial_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_1
                ILine key = LineRoot.Global
                    .Logger(Console.Out, LineStatusSeverity.Ok)
                    .Key("hello")
                    .Format("Hello, {0}.")
                    .Inline("Culture:fi", "Hei, {0}")
                    .Inline("Culture:de", "Hallo, {0}");

                Console.WriteLine(key.Value("mr. anonymous"));
                #endregion Snippet_1
            }
            {
                #region Snippet_2
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                #endregion Snippet_3
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
            {
                #region Snippet_9
                #endregion Snippet_9
            }
            {
                #region Snippet_10
                #endregion Snippet_10
            }
            {
                #region Snippet_11
                #endregion Snippet_11
            }
            {
                #region Snippet_12
                #endregion Snippet_12
            }

        }
    }

}
