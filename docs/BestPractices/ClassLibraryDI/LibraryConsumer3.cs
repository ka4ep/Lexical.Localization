using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Reflection;
using TutorialLibrary2;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace TutorialProject2
{
    public class Program3
    {
        public static void Main(string[] args)
        {
            #region Snippet
            IServiceCollection services = new ServiceCollection();

            // Install file provider service
            services.AddSingleton<IFileProvider>(s=>new PhysicalFileProvider(Directory.GetCurrentDirectory()));

            // Install default IStringLocalizerFactory
            services.AddLexicalLocalization(
                addStringLocalizerService: true,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: false);

            // Install TutorialLibrary's ILibraryAssetSources.
            Assembly library = typeof(MyClass).Assembly;
            services.AddLibraryAssetSources(library);

            // Install additional localization that was not available in the TutorialLibrary.
            services.AddSingleton<IAssetSource>(XmlLocalizationReader.Instance.FileAssetSource("TutorialLibrary2-fi.xml"));

            // Service MyClass
            services.AddTransient<MyClass, MyClass>();

            // Create instance container
            using (var provider = services.BuildServiceProvider())
            {
                // Create class
                MyClass myClass = provider.GetService<MyClass>();

                // Use the culture that was provided by with the class library (LibraryAssetSources)
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
                Console.WriteLine(myClass.Do());

                // Use the culture that we supplied above
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
                Console.WriteLine(myClass.Do());
            }
            #endregion Snippet
        }
    }
}
