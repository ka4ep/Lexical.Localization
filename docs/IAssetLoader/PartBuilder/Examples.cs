using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace docs
{
    public class AssetLoaderPartBuilder_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path("Assets1")                                            // Add directory to search files from
                    .Path("Assets2")                                            // Add another directory to search files from.
                #endregion Snippet_1
                ;
            }

            {
                #region Snippet_2
                // Get Assembly reference
                Assembly asm = Assembly.GetExecutingAssembly();
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Assembly(asm)                                              // Add assembly to embedded files from.
                #endregion Snippet_2
                ;
            }

            {
                #region Snippet_3
                // File provider
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .FileProvider(fileProvider)                                 // Add file provider to search files from.
                #endregion Snippet_3
                ;
            }

            {
                #region Snippet_4a1
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                    .FilePattern("Patches/{section}{-key}{-culture}.png")       // Add file name pattern
                    .FilePattern("Assets/{section}{-key}{-culture}.png")        // Add another file name pattern
                #endregion Snippet_4a1
                    .Resource();

                // ... after asset is built ... //

                #region Snippet_4a2
                // Create key. Both folders are searched for file "icons-ok-de.png".
                IAssetKey key = new LocalizationRoot().Section("icons").Key("ok").SetCulture("de");
                #endregion Snippet_4a2
            }
            {
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                #region Snippet_4b1
                    .FilePattern("{location/}{section}{-key}{-culture}.png")    // Add file name pattern
                #endregion Snippet_4b1
                ;

                // ...

                #region Snippet_4b2
                // "location" is not used, matches to filename "icons-ok-de.png"
                IAssetKey key_1 = new LocalizationRoot().Section("icons").Key("ok").SetCulture("de");
                // This key matches to filename "Assets/icons-ok-de.png"
                IAssetKey key_2 = new LocalizationRoot().Location("Assets").Section("icons").Key("ok").SetCulture("de");
                // This key matches to filename "Patches/icons-ok-de.png"
                IAssetKey key_3 = new LocalizationRoot().Location("Patches").Section("icons").Key("ok").SetCulture("de");
                #endregion Snippet_4b2
            }
            {
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                #region Snippet_4c1
                    .FilePattern("[location/]{section}{-key}{-culture}.png")    // Add file name pattern
                #endregion Snippet_4c1
                    ;

                // ...

                #region Snippet_4c2
                // Location is required but not provided. Will not work.
                IAssetKey key_1 = new LocalizationRoot().Section("icons").Key("ok").SetCulture("de");
                // Location is required and provided. This matches to file name "Patches/icons-ok-de.png"
                IAssetKey key_2 = new LocalizationRoot().Location("Patches").Section("icons").Key("ok").SetCulture("de");
                #endregion Snippet_4c2
            }
            {
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                #region Snippet_4d1
                    .FilePattern("{location_0/}{location_1/}{location_n/}{section}{-key}{-culture}.png")  // Add file name pattern
                #endregion Snippet_4d1
                ;

                // ...

                #region Snippet_4d2
                // Location can be entered multiple times. 
                // This matches to file name "Patches/20181130/icons-ok-de.png"
                IAssetKey key = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").SetCulture("de");
                #endregion Snippet_4d2
            }

            {
                #region Snippet_4e
                // Create builder
                IAssetLoaderPart[] parts = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                    .FilePattern("{location/}{section}{-key}{-culture}.png")    // Add file name pattern
                    .MatchParameter("location")                                 // Match "location" against existing file names
                    .Resource()
                    .Build().ToArray();

                // Create asset
                IAsset asset = new AssetLoader(parts);

                // Location is not provided, parameter is matched automatically. 
                // Searches for "*/icons-ok-de.png".
                IAssetKey key_1 = new LocalizationRoot(asset).Section("icons").Key("ok").SetCulture("de");

                // Location is provided, files are not searched.
                // This matches to file name "Patches/icons-ok-de.png"
                IAssetKey key_2 = new LocalizationRoot(asset).Location("Patches").Section("icons").Key("ok").SetCulture("de");
                #endregion Snippet_4e
            }

            {
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                #region Snippet_4f
                    // Searches for "*/icons-ok-de.png", but only one level deep because of "[^/]+" regular expression.
                    .FilePattern("{location<[^/]+>/}{section}{-key}{-culture}.png")
                #endregion Snippet_4f
                    .MatchParameter("location");                                // Match "location" against existing file names

                // ...

                // Searches for "*/icons-ok-de.png", but only one level deep because of "[^/]+" regular expression.
                IAssetKey key = new LocalizationRoot().Section("icons").Key("ok").SetCulture("de");
            }

            {
                // Get Assembly reference
                Assembly asm = Assembly.GetExecutingAssembly();
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                #region Snippet_5a1
                    .Assembly(asm)                                                  // Add assembly to embedded files from.
                    .EmbeddedPattern("namespace.Assets.icon{-key}{-culture}.png")   // Add embedded resource pattern
                #endregion Snippet_5a1
                ;

                #region Snippet_5a2
                // AssemblySection is not needed. Searches for "namespace.Assets.icon-ok-de.png"
                IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
                #endregion Snippet_5a2
            }
            {
                // Get Assembly reference
                Assembly[] assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                #region Snippet_5b1
                    .Assemblies(assemblies)                                         // Add assembly to embedded files from.
                    .EmbeddedPattern("[assembly.]Assets.icon{-key}{-culture}.png")  // Add embedded resource pattern
                #endregion Snippet_5b1
                    ;

                #region Snippet_5b2
                // Key needs AssemblySection part to match. Searches for "namespace.Assets.icon-ok-de.png"
                IAssetKey key = new LocalizationRoot().AssemblySection("namespace").Key("ok").SetCulture("de");
                #endregion Snippet_5b2
            }
            {
                // Get Assembly reference
                Assembly asm = Assembly.GetExecutingAssembly();
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Assembly(asm)                                                      // Add assembly to embedded files from.
                #region Snippet_5c1
                    .EmbeddedPattern("[assembly.]{resource.}icon{-key}{-culture}.png")  // Add embedded resource pattern
                    .MatchParameters("assembly", "resource")                            // Match parameters against existing file names
                #endregion Snippet_5c1
                ;

                #region Snippet_5c2
                // Matching is optional. Searches for "*.*.icon-ok-de.png"
                IAssetKey key_1 = new LocalizationRoot().Key("ok").SetCulture("de");
                // Matching to "namespace.Resources.icon-ok-de.png"
                IAssetKey key_2 = new LocalizationRoot().AssemblySection("namespace").ResourceSection("Resources.").Key("ok").SetCulture("de");
                #endregion Snippet_5c2
            }


            {
                #region Snippet_6
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                    .FilePattern("Assets/localization.ini")                     // Add file name pattern
                    .KeyPolicy(default)                                         // Add key policy default to file extension
                    .Strings();                                                 // Signal to reads strings 

                // Create asset
                IAsset asset = new AssetLoader(builder.Build());

                // Create key
                IAssetKey key = new LocalizationRoot(asset).Section("controller").Key("hello").SetCulture("de");

                // Issue request. Asset loader loads "Assets\localization.ini", and then searches for key "de:controller:hello".
                string str = key.ToString();
                #endregion Snippet_6
            }

            {
                #region Snippet_7
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                   // Add directory to search files from
                    .FilePattern("Assets/localization.ini")                      // Add file name pattern
                    .KeyPolicy(AssetKeyNameProvider.Colon_Colon_Colon)           // Add key policy with ":" separator
                    .Strings();                                                  // Signal to reads strings 
                #endregion Snippet_7
            }

            {
                #region Snippet_8
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                   // Add directory to search files from
                    .FilePattern("Assets/localization.ini")                      // Add file name pattern
                    .KeyPattern("{culture:}{anysection_0:}{anysection_1:}[key]") // Add key pattern
                    .Strings();                                                  // Signal to build part(s) that reads strings 
                #endregion Snippet_8
            }

            {
                // Get Assembly reference
                Assembly asm = Assembly.GetExecutingAssembly();
                #region Snippet_9
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Assembly(asm)                                                  // Add assembly
                    .EmbeddedPattern("namespace.Resources.MyController.resources")  // Add embedded resource pattern
                    .KeyPolicy(default)                                             // Add key policy default to file extension
                    .ResourceManager();                                             // Signal to reads strings 
                #endregion Snippet_9
            }

            {
                #region Snippet_10
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                    .FilePattern("Assets/{section}{-key}{-culture}.png")        // Add file name pattern
                    .FilePattern("Patches/{section}{-key}{-culture}.png")       // Add another file name pattern
                    .Resource();                                                // Signal to read binary resources
                #endregion Snippet_10
            }

            {
                #region Snippet_11
                // Create asset loader
                IAssetLoader assetLoader = new AssetLoader();

                // Create part(s)
                IAssetLoaderPart[] parts = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                    .FilePattern("Assets/localization{-culture}.ini")           // Add file name pattern
                    .KeyPolicy(default)                                         // Add key policy default to file extension
                    .Strings()                                                  // Signal to read binary resources
                    .Build().ToArray();                                         // Build IAssetLoaderParts

                // Add part(s)
                assetLoader.AddRange(parts);
                #endregion Snippet_11
            }

            {
                #region Snippet_12
                // Create asset loader and add parts
                IAssetLoader assetLoader =
                    new AssetLoader()
                    .NewPart()                                                  // Start part builder
                        .Path(".")                                              // Add location to search files from
                        .FilePattern("Assets/localization{-culture}.ini")       // File name pattern
                        .KeyPattern(default)                                    // Key pattern to match to lines within files
                        .Strings()                                              // Signal to read strings
                    .End();                                                     // End part builder
                #endregion Snippet_12
            }

            { 
                #region Snippet_13
                // Create builder
                AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
                    .Path(".")                                                  // Add directory to search files from
                    .FilePattern("Assets/localization{-culture}.ext")           // Add file name pattern
                    .AssetFileConstructor( (s, p) => new LocalizationStringDictionary(new Dictionary<string, string>()) )
                    .KeyPolicy(default)                                         // Add key policy default to file extension
                    .Strings();                                                 // Signal to read strings
                #endregion Snippet_13
            }


        }
    }

}
