using Lexical.Localization;
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
            // Create localizer
            IAssetBuilder builder = new AssetBuilder.OneBuildInstance();
            IAsset asset = builder.Build();
            IStringLocalizerFactory localizer = new StringLocalizerRoot(asset, new CulturePolicy());
            // Install TutorialLibrary's ILibraryAssetSources
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();
            #region Snippet

            // Create class without localizer
            MyClass myClass1 = new MyClass(default);

            // Use the culture that was provided by with the class library (LibraryAssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass1.Do());

            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = XmlLocalizationReader.Instance.FileAssetSource("TutorialLibrary3-fi.xml");
            // Add to global localizer instance for the non-DI case
            StringLocalizerRoot.Builder.AddSource(assetSource).Build();
            // Add to local localizer instance for the DI case.
            builder.AddSource(assetSource).Build();

            // Use the culture that we just supplied
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Try the class without localizer
            Console.WriteLine(myClass1.Do());
            // Try the class with localizer
            IStringLocalizer<MyClass> classLocalizer = localizer.Create(typeof(MyClass)) as IStringLocalizer<MyClass>;
            MyClass myClass2 = new MyClass(classLocalizer);
            Console.WriteLine(myClass2.Do());
            #endregion Snippet
        }
    }
}
