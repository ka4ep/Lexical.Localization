using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization.LocalizationFile;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class Serialization_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                // Parametrizer for AssetKey
                IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
                // Create context-dependent key
                IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
                // Convert to context-free parameters
                IEnumerable<KeyValuePair<string, string>> parameters = parametrizer.GetAllParameters(key).ToArray();
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                // Convert to context-free parameters
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("culture", "en"));
                parameters.Add(new KeyValuePair<string, string>("type", "MyLibrary:Type"));
                parameters.Add(new KeyValuePair<string, string>("key", "\"hello\""));
                // Parametrizer for AssetKey
                IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
                // Convert to context-dependent instance
                object key = LocalizationRoot.Global;
                foreach (var parameter in parameters)
                    key = parametrizer.CreatePart(key, parameter.Key, parameter.Value);
                // Type-cast
                IAssetKey key_ = (IAssetKey)key;
                #endregion Snippet_2
            }

            {
                #region Snippet_3
                // Parametrizer for AssetKey
                IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
                // Create context-dependent key
                IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
                // Convert to context-free parameters
                IEnumerable<KeyValuePair<string, string>> parameters = parametrizer.GetAllParameters(key);
                // Serialize to string
                string str = AssetKeyStringSerializer.Instance.PrintParameters(parameters);
                #endregion Snippet_3
            }

            {
                #region Snippet_4
                // Key in string format
                string str = "culture:en:type:MyLibrary.MyController:key:Success";
                // Convert to context-free parameters
                IEnumerable<KeyValuePair<string, string>> parameters = AssetKeyStringSerializer.Instance.ParseParameters(str);
                // Parametrizer for AssetKey
                IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
                // Convert to context-dependent instance
                object key = LocalizationRoot.Global;
                foreach (var parameter in parameters)
                    key = parametrizer.CreatePart(key, parameter.Key, parameter.Value);
                // Type-cast
                IAssetKey key_ = (IAssetKey)key;
                #endregion Snippet_4
            }

            {
                #region Snippet_5
                // Create context-dependent key
                IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
                // Serialize to string
                string str = AssetKeyStringSerializer.Instance.PrintKey(key);
                #endregion Snippet_5
            }

            {
                #region Snippet_6
                // Key in string format
                string str = "culture:en:type:MyLibrary.MyController:key:Success";
                // Parse string
                IAssetKey key = AssetKeyStringSerializer.Instance.ParseKey(str, LocalizationRoot.Global);
                #endregion Snippet_6
            }
        }
    }
}

