using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Lexical.Localization;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class IAssetRoot_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1a
                // Create localization source
                var source = new Dictionary<string, string> { { "en:MyController:hello", "Hello World!" } };
                // Create asset
                IAsset asset = new LocalizationStringDictionary(source);
                // Create culture policy
                ICulturePolicy culturePolicy = new CulturePolicy();
                // Create root
                IAssetRoot root = new LocalizationRoot(asset, culturePolicy);
                #endregion Snippet_1a

                #region Snippet_1b
                // Construct key
                IAssetKey key = root.TypeSection("MyController").Key("Hello");
                #endregion Snippet_1b

                #region Snippet_1c
                // Set active culture for this root
                (root.FindCulturePolicy() as ICulturePolicyAssignable).SetCultures("en", "");
                // Provide string
                string str = key.ToString();
                #endregion Snippet_1c
            }

            {
                // Create localization source
                var source = new Dictionary<string, string> { { "en:Section:Key", "Hello World!" } };
                // Create asset
                IAsset asset = new LocalizationStringDictionary(source);
                #region Snippet_5x
                // Create reference
                IAssetKey key = new LocalizationKey._Section(null, "Section").Key("Key");
                // Retreieve with reference
                string str = asset.GetString(key);
                #endregion Snippet_5x
            }

            {
                #region Snippet_2a
                // Create key from global root
                IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Hello");
                #endregion Snippet_2a

                #region Snippet_2b
                // Create localization source
                var source = new Dictionary<string, string> { { "en:MyController:hello", "Hello World!" } };
                // Create asset
                IAsset asset = new LocalizationStringDictionary(source);
                // Assets are added to global static builder. It must be (re-)built after adding.
                LocalizationRoot.Builder.AddAsset(asset).Build();
                #endregion Snippet_2b

                #region Snippet_2c
                // If ran in multi-threaded initialization, lock to LocalizationRoot.Builder.
                lock (LocalizationRoot.Builder) LocalizationRoot.Builder.AddAsset(asset).Build();
                #endregion Snippet_2c

                #region Snippet_2d
                // StringLocalizerRoot is root for IStringLocalizer interoperability
                IStringLocalizerFactory stringLocalizerFactory = StringLocalizerRoot.Global;
                #endregion Snippet_2d

                #region Snippet_2e
                // LocalizationRoot and StringLocalizerRoot are interchangeable. They share the same asset(s).
                LocalizationRoot.Builder.AddAsset(asset).Build();
                IStringLocalizer stringLocalizer = StringLocalizerRoot.Global.TypeSection<MyController>();
                #endregion Snippet_2e

                #region Snippet_2f
                // Dynamic instance is acquired with LocalizationRoot.GlobalDynamic
                dynamic key_ = LocalizationRoot.GlobalDynamic.Section("Section").Key("Key");
                #endregion Snippet_2f
            }
        }
        class MyController { }
    }

}
