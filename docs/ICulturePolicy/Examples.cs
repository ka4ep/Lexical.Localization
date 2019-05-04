using Lexical.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace docs
{
    public class ICulturePolicy_Examples
    {
        public static void Main(string[] args)
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
                IAsset asset = new LocalizationAsset(source, KeyPrinter.Default);
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
                #region Snippet_3a
                // Set to use CultureInfo.CurrentCulture
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentCulture();
                // Change current culture
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
                #endregion Snippet_3a
            }
            {
                #region Snippet_3b
                // Set to use CultureInfo.CurrentCulture
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentUICulture();
                // Change current culture
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                #endregion Snippet_3b
            }
            {
                #region Snippet_4a
                // Set to use CultureInfo.CurrentCulture
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentThreadCulture();
                // Change current thread's culture
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
                #endregion Snippet_4a
            }
            {
                #region Snippet_4b
                // Set to use CultureInfo.CurrentCulture
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentThreadUICulture();
                // Change current thread's culture
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                #endregion Snippet_4b
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
