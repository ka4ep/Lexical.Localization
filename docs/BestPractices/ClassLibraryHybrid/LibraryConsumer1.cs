using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Reflection;
using TutorialLibrary3;

namespace TutorialProject3
{
    public class Program1
    {
        public static void Main(string[] args)
        {
            #region Snippet
            /// Create class without localizer
            MyClass myClass1 = new MyClass();

            /// Now with localizer
            // Create IStringLocalizerFactory
            AssetBuilder builder = new AssetBuilder.OneBuildInstance();
            IAsset asset = builder.Build();
            StringLocalizerRoot localizer = new StringLocalizerRoot(asset, new CulturePolicy());
            // Install library's [AssetSources]
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();
            // Instantiate the class
            IStringLocalizer<MyClass> classLocalizer = localizer.Type<MyClass>();
            MyClass myClass2 = new MyClass(classLocalizer);

            /// Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass1.Do());
            Console.WriteLine(myClass2.Do());
            #endregion Snippet
        }
    }
}
