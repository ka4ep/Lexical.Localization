using Lexical.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace docs
{
    public class ILineRoot_StringLocalizer_Examples
    {
        public static void Main(string[] args)
        {
            #region Snippet_1
            // Create localization source
            var source = new List<ILine> { LineFormat.Parameters.Parse("Culture:en:Type:MyController:Key:hello").Value("Hello World!") };
            // Create asset
            IAsset asset = new LocalizationAsset(source);
            // Create culture policy
            ICulturePolicy culturePolicy = new CulturePolicy();
            // Create root
            ILineRoot root = new StringLocalizerRoot(asset, culturePolicy);
            #endregion Snippet_1

            {
                #region Snippet_2
                // Assign as IStringLocalizer, use "MyController" as root.
                IStringLocalizer stringLocalizer = root.Section("MyController").AsStringLocalizer();
                #endregion Snippet_2
            }

            {
                #region Snippet_3
                // Assign as IStringLocalizerFactory
                IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
                // Adapt to IStringLocalizer
                IStringLocalizer<MyController> stringLocalizer2 = 
                    stringLocalizerFactory.Create(typeof(MyController)) 
                    as IStringLocalizer<MyController>;
                #endregion Snippet_3
            }

            {
                #region Snippet_4a
                // Assign to IStringLocalizer for the class MyController
                IStringLocalizer<MyController> stringLocalizer = 
                    root.Type(typeof(MyController)).AsStringLocalizer<MyController>();
                #endregion Snippet_4a
            }
            {
                #region Snippet_4b
                // Assign as IStringLocalizerFactory
                IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
                // Create IStringLocalizer for the class MyController
                IStringLocalizer<MyController> stringLocalizer = 
                    stringLocalizerFactory.Create(typeof(MyController)) as IStringLocalizer<MyController>;
                #endregion Snippet_4b
            }

            {
                #region Snippet_5a            
                // Create IStringLocalizer and assign culture
                IStringLocalizer stringLocalizer =
                    root.Culture("en").Type<MyController>().AsStringLocalizer();
                #endregion Snippet_5a
            }
            {
                #region Snippet_5b            
                // Assign as IStringLocalizerFactory
                IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
                // Create IStringLocalizer and assign culture
                IStringLocalizer stringLocalizer = stringLocalizerFactory.Create(typeof(MyController))
                    .WithCulture(CultureInfo.GetCultureInfo("en"));
                #endregion Snippet_5b
                stringLocalizer = (IStringLocalizer)root.Culture("en").Type<MyController>();
                // Change language
                stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("fi"));
                // Keep language
                stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("fi"));
                // Change language
                stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("sv"));
                // Remove language
                stringLocalizer = stringLocalizer.WithCulture(null);
            }

        }

        class MyController
        {

        }
    }

}
