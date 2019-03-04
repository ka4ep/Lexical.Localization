using Lexical.Localization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class AssetNamePattern_Examples
    {
        public static void Main(string[] args)
        {
            
            {
                #region Snippet_1
                // Let's create an example key
                IAssetKey key = new LocalizationRoot()
                        .Location("Patches")
                        .Type("MyController")
                        .Section("Errors")
                        .Key("InvalidState")
                        .Culture("en");
                #endregion Snippet_1

                {
                    #region Snippet_2
                    // Create similiar policy with AssetNamePattern
                    IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{culture/}{location/}{type/}{section/}[Key].txt");
                    // "en/Patches/MyController/Errors/InvalidState.txt"
                    string str = myPolicy.BuildName(key);
                    #endregion Snippet_2
                }

                {
                    #region Snippet_3
                    // Create name pattern
                    IAssetKeyNamePolicy myPolicy = new AssetNamePattern("Patches/{Section}[-key]{-culture}.png");
                    #endregion Snippet_3
                    // "Patches/icons-ok-de.png"
                    string str = myPolicy.BuildName(key);
                }
                {
                    #region Snippet_4
                    // Create name pattern
                    IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{location_0/}{location_1/}{location_n/}{Section}{-key}{-culture}.png");
                    // Create key
                    IAssetKey key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
                    // Converts to "Patches/20181130/icons-ok-de.png"
                    string str = myPolicy.BuildName(key2);
                    #endregion Snippet_4
                }
                {
                    #region Snippet_5
                    // Create name pattern with regular expression detail
                    IAssetNamePattern myPolicy = new AssetNamePattern("{location<[^/]+>/}{Section}{-key}{-culture}.png");
                    // Use its regular expression
                    Match match = myPolicy.Regex.Match("patches/icons-ok-de.png");
                    #endregion Snippet_5
                }

            }
        }
    }

}
