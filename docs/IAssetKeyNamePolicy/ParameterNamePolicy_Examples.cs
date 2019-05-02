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
    public class ParameterNamePolicy_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0
                string str = @"Culture:en:Type:MyController:Key:Ok";
                ILinePart key = ParameterNamePolicy.Instance.Parse(str);
                #endregion Snippet_0
            }
            {
                #region Snippet_0b
                string str = @"Culture:en:Type:MyController:Key:Ok";
                ILinePart root = new StringLocalizerRoot();
                ILinePart key = ParameterNamePolicy.Instance.Parse(str, root);
                #endregion Snippet_0b
            }

            {
                #region Snippet_1
                string str = @"Key:Success\:Plural";
                ILinePart key = ParameterNamePolicy.Instance.Parse(str);
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                ILinePart key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
                string str = ParameterNamePolicy.Instance.BuildName(key);
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                #endregion Snippet_3
            }
            {
                #region Snippet_4
                ILinePart key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
                IEnumerable<KeyValuePair<string, string>> parameters = key.GetParameters();
                string str = ParameterNamePolicy.Instance.PrintParameters(parameters);
                #endregion Snippet_4
            }

            {
                #region Snippet_5
                string str = "Culture:en:Type:MyLibrary.MyController:Key:Success";
                IEnumerable<KeyValuePair<string, string>> parameters = ParameterNamePolicy.Instance.ParseParameters(str);
                ILinePart key = LocalizationRoot.Global.AppendParameters(parameters);
                #endregion Snippet_5
            }

            {
                #region Snippet_6
                ILinePart key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
                string str = ParameterNamePolicy.Instance.PrintKey(key);
                #endregion Snippet_6
            }

            {
                #region Snippet_7
                string str = "Culture:en:Type:MyLibrary.MyController:Key:Success";
                ILinePart key = ParameterNamePolicy.Instance.Parse(str, LocalizationRoot.Global);
                #endregion Snippet_7
            }
        }
    }
}

