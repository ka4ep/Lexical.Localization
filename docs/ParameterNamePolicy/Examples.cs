using Lexical.Localization;
using Lexical.Localization.Internal;
using Lexical.Localization.Ms.Extensions;
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
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                #endregion Snippet_2
            }
            {
                #region Snippet_3a
                // Create context-dependent key
                IAssetKey key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
                // Serialize to string
                string str = ParameterNamePolicy.Instance.PrintKey(key);
                #endregion Snippet_3a
            }
            {
                #region Snippet_3b
                // Create context-dependent key
                IAssetKey key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
                // Convert to context-free parameters
                IEnumerable<KeyValuePair<string, string>> parameters = key.GetParameters();
                // Serialize to string
                string str = ParameterNamePolicy.Instance.PrintParameters(parameters);
                #endregion Snippet_3b
            }

            {
                #region Snippet_4
                // Key in string format
                string str = "culture:en:type:MyLibrary.MyController:key:Success";
                // Convert to context-free parameters
                IEnumerable<KeyValuePair<string, string>> parameters = ParameterNamePolicy.Instance.ParseParameters(str);
                // Type-cast
                IAssetKey key = LocalizationRoot.Global.AppendParameters(parameters);
                #endregion Snippet_4
            }

            {
                #region Snippet_5
                // Create context-dependent key
                IAssetKey key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
                // Serialize to string
                string str = ParameterNamePolicy.Instance.PrintKey(key);
                #endregion Snippet_5
            }

            {
                #region Snippet_6
                // Key in string format
                string str = "culture:en:type:MyLibrary.MyController:key:Success";
                // Parse string
                IAssetKey key = ParameterNamePolicy.Instance.Parse(str, LocalizationRoot.Global);
                #endregion Snippet_6
            }
        }
    }
}

