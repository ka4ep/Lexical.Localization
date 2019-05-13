using Lexical.Localization;
using Lexical.Localization.Internal;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class LineFormatExamples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0
                string str = @"Culture:en:Type:MyController:Key:Ok";
                ILine key = LineFormat.Parameters.Parse(str);
                #endregion Snippet_0
            }
            {
                #region Snippet_0b
                string str = @"Culture:en:Type:MyController:Key:Ok";
                ILine root = new StringLocalizerRoot();
                ILine key = LineFormat.Parameters.Parse(str, root);
                #endregion Snippet_0b
            }

            {
                #region Snippet_1
                string str = @"Key:Success\:Plural";
                ILine key = LineFormat.Parameters.Parse(str);
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                ILine key = LineRoot.Global.Type("MyController").Key("Success").Culture("en");
                string str = LineFormat.Parameters.Print(key);
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                #endregion Snippet_3
            }
            {
                #region Snippet_4
                ILine key = LineRoot.Global.Type("MyController").Key("Success").Culture("en");
                IEnumerable<KeyValuePair<string, string>> parameters = key.GetParameters();
                string str = LineFormat.Parameters.PrintParameters(parameters);
                #endregion Snippet_4
            }

            {
                #region Snippet_5
                string str = "Culture:en:Type:MyLibrary.MyController:Key:Success";
                IEnumerable<KeyValuePair<string, string>> parameters = LineFormat.Parameters.ParseParameters(str);
                ILine key = LineRoot.Global.Parameters(parameters);
                #endregion Snippet_5
            }

            {
                #region Snippet_6
                ILine key = LineRoot.Global.Type("MyController").Key("Success").Culture("en");
                string str = LineFormat.Parameters.PrintKey(key);
                #endregion Snippet_6
            }

            {
                #region Snippet_7
                string str = "Culture:en:Type:MyLibrary.MyController:Key:Success";
                ILine key = LineFormat.Parameters.Parse(str, LineRoot.Global);
                #endregion Snippet_7
            }
        }
    }
}

