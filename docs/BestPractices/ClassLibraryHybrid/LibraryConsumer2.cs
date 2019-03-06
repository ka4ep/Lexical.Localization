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
            // Install localization libraries that are available in the TutorialLibrary.
            // Search for classes with [AssetSources] attribute.
            Assembly library = typeof(MyClass).Assembly;
            StringLocalizerRoot.Builder.AddLibraryAssetSources(library);

            #region Snippet
            // Create class without localizer
            MyClass myClass1 = new MyClass();

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass1.Do());

            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization3-fi.xml");
            StringLocalizerRoot.Builder.AddSource(assetSource).Build();

            // Use culture that we just supplied
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass1.Do());
            #endregion Snippet

            // Create class with localizer
            IStringLocalizerFactory factory = StringLocalizerRoot.Global;
            IStringLocalizer<MyClass> localizer = StringLocalizerRoot.Global.Type<MyClass>();
            MyClass myClass2 = new MyClass( localizer );

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass2.Do());

            // Use culture that was supplied by this application
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass2.Do());
        }
    }
}
