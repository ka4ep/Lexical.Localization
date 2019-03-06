using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Reflection;
using TutorialLibrary3;

namespace TutorialProject3
{
    public class Program2
    {
        public static void Main(string[] args)
        {
            // Create IStringLocalizerFactory
            AssetBuilder builder = new AssetBuilder.OneBuildInstance();
            IAsset asset = builder.Build();
            StringLocalizerRoot localizer = new StringLocalizerRoot(asset, new CulturePolicy());
            // Install library's [AssetSources]
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();
            #region Snippet
            /// Create class without localizer
            MyClass myClass1 = new MyClass();

            // Use the culture that was provided by with the class library (LibraryAssets)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass1.Do());

            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization3-fi.xml");
            // Add to global localizer instance for the non-DI case
            StringLocalizerRoot.Builder.AddSource(assetSource).Build();
            // Add to local localizer instance for the DI case.
            builder.AddSource(assetSource).Build();

            // Use the culture that we just supplied
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass1.Do());
            MyClass myClass2 = new MyClass(localizer.Type<MyClass>());
            Console.WriteLine(myClass2.Do());
            #endregion Snippet
        }
    }
}
