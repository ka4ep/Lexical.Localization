using Lexical.Localization;
using Lexical.Localization.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class IAssetKeyNameProvider_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0a
                // Create localization source
                var source = new Dictionary<string, string> { { "en/MyController/Hello", "Hello World!" } };
                // Create key name policy
                IAssetKeyNamePolicy policy = 
                    new AssetKeyNameProvider()
                        .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: "/") // Sorts parameters
                        .Ignore("Root") // Ignore "Root"
                        .DefaultRule(true, prefixSeparator: "/"); // Default separator
                // Create asset
                IAsset asset = new LocalizationStringAsset(source, policy);
                // Create key
                IAssetKey key = new LocalizationRoot(asset).Section("MyController").Key("Hello");
                // Retrieve string
                string str = key.Culture("en").ResolveFormulatedString();
                #endregion Snippet_0a

                #region Snippet_0b
                // Test if key converted correctly to expected identity "en/Section/Key"
                string id = policy.BuildName(key.Culture("en"));
                #endregion Snippet_0b
            }

            {
                #region Snippet_1
                // Let's create an example key
                IAssetKey key = new LocalizationRoot()
                        .Location("Patches")
                        .Section("Controllers")
                        .Type("MyController")
                        .Section("Errors")
                        .Key("InvalidState")
                        .Culture("en");
                #endregion Snippet_1

                {
                    #region Snippet_2
                    // "en:Patches:Controllers:MyController:Errors:InvalidState"
                    string str1 = AssetKeyNameProvider.Default.BuildName(key);
                    // "en.Patches.Controllers.MyController.Errors.InvalidState"
                    string str2 = AssetKeyNameProvider.Dot_Dot_Dot.BuildName(key);
                    // "Patches:Controllers:MyController:Errors:InvalidState"
                    string str3 = AssetKeyNameProvider.None_Colon_Colon.BuildName(key);
                    // "en:Patches.Controllers.MyController.Errors.InvalidState"
                    string str4 = AssetKeyNameProvider.Colon_Dot_Dot.BuildName(key);
                    // "en:Patches:Controllers:MyController:Errors.InvalidState"
                    string str5 = AssetKeyNameProvider.Colon_Colon_Dot.BuildName(key);
                    #endregion Snippet_2
                }

                {
                    #region Snippet_3
                    // Create a custom policy 
                    IAssetKeyNamePolicy myPolicy = new AssetKeyNameProvider()
                        // Enable non-canonical "Culture" parameter with "/" separator
                        .Rule("Culture", true, postfixSeparator: "/", order: ParameterInfos.Default["Culture"].Order)
                        // Disable other non-canonical parts
                        .NonCanonicalRule(false)
                        // Enable canonical all parts with "/" separator
                        .CanonicalRule(true, prefixSeparator: "/")
                        // Set "Key" parameter's prefix to "/"
                        .Rule("Key", true, prefixSeparator: "/", order: ParameterInfos.Default["Key"].Order);

                    // "en/Patches/MyController/Errors/InvalidState"
                    string str = myPolicy.BuildName(key);
                    #endregion Snippet_3
                }

            }
        }
    }

}
