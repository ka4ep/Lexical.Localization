using Lexical.Localization;
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
            // Create class without localizer
            MyClass myClass1 = new MyClass(default);
            
            // Create localizer
            IAssetBuilder builder = new AssetBuilder.OneBuildInstance();
            IAsset asset = builder.Build();
            IStringLocalizerFactory localizer = new StringLocalizerRoot(asset, new CulturePolicy());

            // Install TutorialLibrary's ILibraryAssetSources
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();

            // Create class with localizer
            IStringLocalizer<MyClass> classLocalizer = localizer.Create(typeof(MyClass)) as IStringLocalizer<MyClass>;
            MyClass myClass2 = new MyClass(classLocalizer);

            /// Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass1.Do());
            Console.WriteLine(myClass2.Do());
            #endregion Snippet
        }
    }
}
