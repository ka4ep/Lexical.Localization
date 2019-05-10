using Lexical.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace docs
{
    public class IAssetRoot_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_1a
                // Create localization source
                var source = new Dictionary<string, string> { { "Culture:en:Type:MyController:Key:hello", "Hello World!" } };
                // Create asset
                IAsset asset = new LocalizationAsset(source, ParameterParser.Instance);
                // Create culture policy
                ICulturePolicy culturePolicy = new CulturePolicy();
                // Create root
                ILineRoot root = new LineRoot(asset, culturePolicy);
                #endregion Snippet_1a

                #region Snippet_1b
                // Construct key
                ILine key = root.Type("MyController").Key("Hello");
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
                var source = new Dictionary<string, string> { { "Culture:en:Section:Section:Key:Key", "Hello World!" } };
                // Create asset
                IAsset asset = new LocalizationAsset(source, ParameterParser.Instance);
                #region Snippet_5x
                // Create reference
                ILine key = new LocalizationKey._Section(null, null, "Section").Key("Key");
                // Retreieve with reference
                IFormulationString str = asset.GetString(key);
                #endregion Snippet_5x
            }

            {
                #region Snippet_2a
                // Create key from global root
                ILine key = LineRoot.Global.Type("MyController").Key("Hello");
                #endregion Snippet_2a

                #region Snippet_2b
                // Create localization source
                var source = new Dictionary<string, string> { { "Culture:en:Type:MyController:Key:hello", "Hello World!" } };
                // Create asset
                IAsset asset = new LocalizationAsset(source, ParameterParser.Instance);
                // Assets are added to global static builder. It must be (re-)built after adding.
                LineRoot.Builder.AddAsset(asset).Build();
                #endregion Snippet_2b

                #region Snippet_2c
                // If ran in multi-threaded initialization, lock to LocalizationRoot.Builder.
                lock (LineRoot.Builder) LineRoot.Builder.AddAsset(asset).Build();
                #endregion Snippet_2c

                #region Snippet_2d
                // StringLocalizerRoot is root for IStringLocalizer interoperability
                IStringLocalizerFactory stringLocalizerFactory = StringLocalizerRoot.Global;
                #endregion Snippet_2d

                #region Snippet_2e
                // LocalizationRoot and StringLocalizerRoot are interchangeable. They share the same asset(s).
                LineRoot.Builder.AddAsset(asset).Build();
                IStringLocalizer stringLocalizer = StringLocalizerRoot.Global.Type<MyController>();
                #endregion Snippet_2e

                #region Snippet_2f
                // Dynamic instance is acquired with LocalizationRoot.GlobalDynamic
                dynamic key_ = LineRoot.GlobalDynamic.Section("Section").Key("Key");
                #endregion Snippet_2f
            }
        }
        class MyController { }
    }

}
