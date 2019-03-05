using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Reflection;

namespace TutorialProject
{
    public class Program4
    {
        public static void Main(string[] args)
        {
            #region Snippet
            IServiceCollection services = new ServiceCollection();

            // Install default IStringLocalizerFactory
            services.AddLexicalLocalization(
                addStringLocalizerService: true,
                addCulturePolicyService: true,
                useGlobalInstance: true,
                addCache: false);

            // Install Library's [AssetSources].
            Assembly library = typeof(TutorialLibrary.MyClass2).Assembly;
            services.AddAssetLibrarySources(library);

            // Install additional localization that was not available in the TutorialLibrary.
            services.AddSingleton<IAssetSource>(XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization-fi.xml"));

            // Service MyClass2
            services.AddTransient<TutorialLibrary.MyClass2, TutorialLibrary.MyClass2>();

            // Create instance container
            using (var provider = services.BuildServiceProvider())
            {
                // Create class
                TutorialLibrary.MyClass2 myClass = provider.GetService<TutorialLibrary.MyClass2>();

                // Use culture that was provided with the class library
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
                Console.WriteLine(myClass.Do());

                // Use culture that was supplied by this application
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
                Console.WriteLine(myClass.Do());
            }
            #endregion Snippet

            Console.ReadKey();
        }
    }
}
