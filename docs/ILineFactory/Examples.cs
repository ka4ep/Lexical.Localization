using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class ILineFactory_Examples
    {
        public static void Main(string[] args)
        {
            {
                List<ILine> lines = new List<ILine>
                {
                    LineAppender.Default.Type("ILineFactory_Examples").Key("Hello").Text("Hello World"),
                };
                IAsset asset = new StringAsset().Add(lines);
                #region Snippet_0
                ILine key = LineAppender.NonResolving.Type("ILineFactory_Examples").Key("Hello");
                IString str = asset.GetLine(key).GetString();
                #endregion Snippet_0
            }
            {
                #region Snippet_1
                ILine localization = LineAppender.Default
                    .PluralRules("[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true")
                    .Assembly("docs")
                    .Culture("en")
                    .Type<ILineFactory_Examples>();

                List<ILine> lines = new List<ILine>
                {
                    localization.Key("OK").Text("Successful"),
                    localization.Key("Error").Format("Failed (ErrorCode={0})")
                };

                IAsset asset = new StringAsset().Add(lines);
                #endregion Snippet_1
            }
            {
                #region Snippet_2
                IStringLocalizer localizer = 
                    LineRoot.Global.Type("Hello").SetAppender(StringLocalizerAppender.Default).Key("Ok") 
                    as IStringLocalizer;
                #endregion Snippet_2
            }
            {
                #region Snippet_3a
                ILine line = LineRoot.Global.AddAppender(new MyAppender()).Key("Ok");
                #endregion Snippet_3a
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

    #region Snippet_3b
    class MyAppender : ILineFactory<ILineCanonicalKey, string, string>
    {
        public bool TryCreate(
            ILineFactory factory, 
            ILine previous, 
            string parameterName, 
            string parameterValue, 
            out ILineCanonicalKey line)
        {
            line = new LineCanonicalKey(factory, previous, parameterName, parameterValue);
            return true;
        }
    }
    #endregion Snippet_3b

}
