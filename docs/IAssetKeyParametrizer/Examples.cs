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
    public class AssetKeyParameterizer_Examples
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
                IEnumerable<KeyValuePair<string, string>> parameters =
                    new Key("culture", "en")
                    .Append("type", "MyLibrary:Type").Append("key", "\"hello\"");
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

        }
    }
}

