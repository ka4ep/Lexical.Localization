using Lexical.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Reflection;
using TutorialLibrary2;

namespace TutorialProject2
{
    public class Program1
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Create localizer
            IAssetBuilder builder = new AssetBuilder.OneBuildInstance();
            IAsset asset = builder.Build();
            StringLocalizerRoot localizer = new StringLocalizerRoot(asset, new CulturePolicy());

            // Install TutorialLibrary's IAssetSources
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();

            // Create class
            IStringLocalizer<MyClass> classLocalizer = localizer.Type<MyClass>() as IStringLocalizer<MyClass>;
            MyClass myClass = new MyClass(classLocalizer);

            // Use the culture that was provided with the class library (AssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}
