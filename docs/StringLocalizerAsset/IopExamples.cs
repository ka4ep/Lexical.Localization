using Lexical.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace docs
{
    public class Ms_Localization_IopExamples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0a
                // Create IStringLocalizerFactory
                LoggerFactory loggerFactory = new LoggerFactory();
                loggerFactory.AddConsole(LogLevel.Trace);
                IOptions<LocalizationOptions> options = Options.Create(new LocalizationOptions { ResourcesPath = "" });
                IStringLocalizerFactory stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);

                // Adapt IStringLocalizerFactory to IAsset
                IAsset asset = new StringLocalizerFactoryAsset(stringLocalizerFactory);

                // Create root
                IAssetRoot root = new LocalizationRoot(asset, new CulturePolicy());

                // There are .resx files in "Resources/ConsoleApp1.MyController" with keys "Success" and "Error"
                IAssetKey key = root
                    .Assembly(Assembly.GetExecutingAssembly())
                    .Resource("ConsoleApp1.MyController")
                    //.Type(typeof(ConsoleApp1.MyController1))
                    .Key("Success")
                    .Culture("sv");

                // Retrieve string from IStringLocalizerFactory
                string str = key.ToString();
                #endregion Snippet_0a

                #region Snippet_0b
                // Adapt IStringLocalizerFactory to IAsset
                asset = stringLocalizerFactory.ToAsset();
                #endregion Snippet_0b

                IStringLocalizer stringLocalizer = null;
                #region Snippet_0c
                // Adapt IStringLocalizer to IAsset
                asset = new StringLocalizerAsset(stringLocalizer);
                #endregion Snippet_0c

            }

            {
                #region Snippet_4a
                // Create asset
                var source = new Dictionary<string, string> { { "fi:ConsoleApp1.MyController:Success", "Onnistui" } };
                IAsset asset = new LocalizationAsset(source, AssetKeyNameProvider.Default);

                // Create root
                IAssetKey root = new StringLocalizerRoot(asset, new CulturePolicy());

                // Type cast to IStringLocalizerFactory
                IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;

                // Get IStringLocalizer
                IStringLocalizer stringLocalizer = stringLocalizerFactory.Create(typeof(ConsoleApp1.MyController));

                // Assign culture
                stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("fi"));

                // Get string
                string str = stringLocalizer["Success"];
                #endregion Snippet_4a
            }

            {
                #region Snippet_4b
                // Create asset
                var source = new Dictionary<string, string> { { "fi:ConsoleApp1.MyController:Success", "Onnistui" } };
                IAsset asset = new LocalizationAsset(source, AssetKeyNameProvider.Default);

                // Create root
                IAssetKey root = new StringLocalizerRoot(asset, new CulturePolicy());

                // Set key
                IAssetKey key = root.Culture("fi").Type(typeof(ConsoleApp1.MyController));

                // Type cast key to IStringLocalizer
                IStringLocalizer stringLocalizer = key as IStringLocalizer;

                // Get string
                string str = stringLocalizer["Success"];
                #endregion Snippet_4b
            }


            {
                #region Snippet_9
                // Create asset
                var source = new Dictionary<string, string> { { "fi:ConsoleApp1.MyController:Success", "Onnistui" } };
                IAsset asset = new LocalizationAsset(source, AssetKeyNameProvider.Default);

                // Create root
                IAssetKey root = new StringLocalizerRoot(asset, new CulturePolicy());
                #endregion Snippet_9
            }

            {
                #region Snippet_3
                #endregion Snippet_3
            }

            {
                #region Snippet_4
                #endregion Snippet_4
            }
        }
    }

}

namespace ConsoleApp1 { public class MyController { } }
