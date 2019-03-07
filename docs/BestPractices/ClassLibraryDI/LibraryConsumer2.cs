using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using System;
using System.Globalization;
using System.Reflection;
using TutorialLibrary2;

namespace TutorialProject2
{
    public class Program2
    {
        public static void Main(string[] args)
        {
            // Create localizer
            IAssetBuilder builder = new AssetBuilder.OneBuildInstance();
            IAsset asset = builder.Build();
            StringLocalizerRoot localizer = new StringLocalizerRoot(asset, new CulturePolicy());

            // Install TutorialLibrary's [AssetSources]
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();

            #region Snippet
            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization2-fi.xml");
            builder.AddSource(assetSource).Build();
            #endregion Snippet

            // Create class
            IAssetKey<MyClass> classLocalizer = localizer.Type<MyClass>();
            MyClassB myClass = new MyClassB(classLocalizer);

            // Use the culture that was provided with the class library (LibraryAssets)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use the culture that was supplied above
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
        }
    }
}
