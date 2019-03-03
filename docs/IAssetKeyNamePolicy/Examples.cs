using Lexical.Localization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class IAssetKeyNamePolicy_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_0a
                // Create localization source
                var source = new Dictionary<string, string> { { "en/MyController/Hello", "Hello World!" } };
                // Create key name policy
                IAssetKeyNamePolicy policy = new AssetKeyNameProvider().SetDefault(true, "/");
                // Create asset
                IAsset asset = new LocalizationStringAsset(source, policy);
                // Create key
                IAssetKey key = new LocalizationRoot(asset).Section("MyController").Key("Hello");
                // Retrieve string
                string str = key.SetCulture("en").ResolveFormulatedString();
                #endregion Snippet_0a

                #region Snippet_0b
                // Test if key converted correctly to expected identity "en/Section/Key"
                string id = policy.BuildName(key.SetCulture("en"));
                #endregion Snippet_0b
            }

            {
                #region Snippet_1
                // Let's create an example key
                IAssetKey key = new LocalizationRoot()
                        .Location("Patches")
                        .TypeSection("MyController")
                        .Section("Errors")
                        .Key("InvalidState")
                        .SetCulture("en");
                #endregion Snippet_1

                {
                    #region Snippet_2
                    // "en:Patches:MyController:Errors:InvalidState"
                    string str1 = AssetKeyNameProvider.Default.BuildName(key);
                    // "en.Patches.MyController.Errors.InvalidState"
                    string str2 = AssetKeyNameProvider.Dot_Dot_Dot.BuildName(key);
                    // "Patches:MyController:Errors:InvalidState"
                    string str3 = AssetKeyNameProvider.None_Colon_Colon.BuildName(key);
                    // "en:Patches.MyController.Errors.InvalidState"
                    string str4 = AssetKeyNameProvider.Colon_Dot_Dot.BuildName(key);
                    #endregion Snippet_2
                }

                {
                    #region Snippet_3
                    // Create a custom policy 
                    IAssetKeyNamePolicy myPolicy = new AssetKeyNameProvider()
                        // Enable non-canonical "Culture" parameter with "/" separator
                        .SetParameter("Culture", true, "", "/")
                        // Disable other non-canonical parts
                        .SetNonCanonicalDefault(false)
                        // Enable canonical all parts with "/" separator
                        .SetCanonicalDefault(true, "/", "")
                        // Set "Key" parameter's prefix to "/" and postfix to ".txt".
                        .SetParameter("Key", true, "/", ".txt");

                    // "en/Patches/MyController/Errors/InvalidState.txt"
                    string str = myPolicy.BuildName(key);
                    #endregion Snippet_3
                }

                {
                    #region Snippet_4
                    // Create similiar policy with AssetNamePattern
                    IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{culture/}{location/}{type/}{section/}[key].txt");
                    // "en/Patches/MyController/Errors/InvalidState.txt"
                    string str = myPolicy.BuildName(key);
                    #endregion Snippet_4
                }

                {
                    #region Snippet_4a
                    // Create name pattern
                    IAssetKeyNamePolicy myPolicy = new AssetNamePattern("Patches/{section}[-key]{-culture}.png");
                    #endregion Snippet_4a
                    // "Patches/icons-ok-de.png"
                    string str = myPolicy.BuildName(key);
                }
                {
                    #region Snippet_4b
                    // Create name pattern
                    IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{location_0/}{location_1/}{location_n/}{section}{-key}{-culture}.png");
                    // Create key
                    IAssetKey key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").SetCulture("de");
                    // Converts to "Patches/20181130/icons-ok-de.png"
                    string str = myPolicy.BuildName(key2);
                    #endregion Snippet_4b
                }
                {
                    #region Snippet_4c
                    // Create name pattern with regular expression detail
                    IAssetNamePattern myPolicy = new AssetNamePattern("{location<[^/]+>/}{section}{-key}{-culture}.png");
                    // Use its regular expression
                    Match match = myPolicy.Regex.Match("patches/icons-ok-de.png");
                    #endregion Snippet_4c
                }

            }
        }
    }

}
