using Lexical.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace docs
{
    public class ICulturePolicy_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_0
                // Create policy
                ICulturePolicyAssignable culturePolicy = new CulturePolicy();
                #endregion Snippet_0

                #region Snippet_1
                // Create localization source
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };
                // Create asset with culture policy
                IAsset asset = new LocalizationStringAsset(source);
                // Create root and assign culturePolicy
                IAssetRoot root = new LocalizationRoot(asset, culturePolicy);
                #endregion Snippet_1
            }

            {
                #region Snippet_2a
                // Set active culture and set fallback culture
                ICulturePolicy cultureArray_ =
                    new CulturePolicy().SetCultures(
                        CultureInfo.GetCultureInfo("en-US"),
                        CultureInfo.GetCultureInfo("en"),
                        CultureInfo.GetCultureInfo("")
                    );
                #endregion Snippet_2a
            }
            {
                #region Snippet_2b
                // Create policy from array of cultures
                ICulturePolicy culturePolicy = new CulturePolicy().SetCultures("en-US", "en", "");
                #endregion Snippet_2b
            }
            {
                #region Snippet_2c
                // Create policy from culture, adds fallback cultures "en" and "".
                ICulturePolicy culturePolicy = new CulturePolicy().SetCultureWithFallbackCultures("en-US");
                #endregion Snippet_2c
            }
            

            {
                #region Snippet_3
                // Set to use CultureInfo.CurrentCulture
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentCulture();
                // Change current culture
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
                #endregion Snippet_3
            }
            {
                #region Snippet_4
                // Set to use CultureInfo.CurrentCulture
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentCulture();
                // Change current culture
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                #endregion Snippet_4
            }
            {
                #region Snippet_5
                // Assign delegate 
                ICulturePolicy culturePolicy = new CulturePolicy().SetFunc(() => CultureInfo.GetCultureInfo("fi"));
                #endregion Snippet_5
            }
            {
                #region Snippet_6
                // Assign delegate 
                ICulturePolicy source = new CulturePolicy().SetToCurrentUICulture();
                ICulturePolicy culturePolicy = new CulturePolicy().SetSourceFunc(() => source);
                #endregion Snippet_6
            }
            {
                #region Snippet_7
                // Freeze current culture
                ICulturePolicy culturePolicy = new CulturePolicy()
                    .SetToCurrentCulture()
                    .ToSnapshot()
                    .AsReadonly();
                #endregion Snippet_7
            }
        }
    }

}
