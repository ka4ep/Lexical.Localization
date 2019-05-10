# Class Library with Dependency Injection
This article describes recommended practice for writing a localized class library that uses Depedency Injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **LibraryAssetSources** which implements **ILibraryAssetSources** as a signal 
to indicate that this class provides localizations for this class library.

Internal localization files are typically embedded resources.

```csharp
using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary2
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary2-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LocalizationFileProviderSource ExternalLocalizationSource;

        public LibraryAssetSources() : base()
        {
            // Add internal localization source
            Add(LocalizationSource);
        }

        public LibraryAssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external localization source
            if (fileProvider != null)
            {
                ExternalLocalizationSource = 
                    LocalizationXmlReader.Instance.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary2.xml", throwIfNotFound: false);
                Add(ExternalLocalizationSource);
            }
        }
    }
}

```
<details>
  <summary>The example localization file *TutorialLibrary2-de.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "de" -->
  <Type:TutorialLibrary2.MyClass Culture="de">
    <Key:OK>Erfolgreich!</Key:OK>
  </Type:TutorialLibrary2.MyClass>

</Localization>

```
</details>

## Classes
Classes in the class library can use IStringLocalizer abstractions

```csharp
using Microsoft.Extensions.Localization;

namespace TutorialLibrary2
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localizer;

        public MyClass(IStringLocalizer<MyClass> localizer)
        {
            this.localizer = localizer;
        }

        public string Do()
        {
            return localizer["OK"];
        }
    }
}

```

... or alternatively Lexical.Localization.Abstractions.

```csharp
using Lexical.Localization;

namespace TutorialLibrary2
{
    public class MyClassB
    {
        ILine<MyClass> localizer;

        public MyClassB(ILine<MyClass> localizer)
        {
            this.localizer = localizer;
        }

        public string Do()
        {
            return localizer.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}

```

# Application
Application that deploys the localizer must include the internal localizations with 
**<i>IAssetBuilder</i>.AddLibraryAssetSources(*Assembly*)** which searches the **ILibraryAssetSources** from the library.
# [Snippet](#tab/snippet-1)

```csharp
// Create localizer
IAssetBuilder builder = new AssetBuilder.OneBuildInstance();
IAsset asset = builder.Build();
StringLocalizerRoot localizer = new StringLocalizerRoot(asset, new CulturePolicy());

// Install TutorialLibrary's ILibraryAssetSources
Assembly library = typeof(MyClass).Assembly;
builder.AddLibraryAssetSources(library).Build();

// Create class
IStringLocalizer<MyClass> classLocalizer = localizer.Type<MyClass>();
MyClass myClass = new MyClass(classLocalizer);

// Use the culture that was provided with the class library (LibraryAssetSources)
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
Console.WriteLine(myClass.Do());
```
# [Full Code](#tab/full-1)

```csharp
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

            // Install TutorialLibrary's ILibraryAssetSources
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();

            // Create class
            IStringLocalizer<MyClass> classLocalizer = localizer.Type<MyClass>();
            MyClass myClass = new MyClass(classLocalizer);

            // Use the culture that was provided with the class library (LibraryAssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}

```
***
<br/>

## Supplying Localizations
The application can supply additional localization sources with **<i>IAssetBuilder</i>.AddSource(*IAssetSource*)**
# [Snippet](#tab/snippet-2)

```csharp
// Install additional localization that was not available in the TutorialLibrary.
IAssetSource assetSource = LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary2-fi.xml");
builder.AddSource(assetSource).Build();
```
# [Full Code](#tab/full-2)

```csharp
using Lexical.Localization;
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

            // Install TutorialLibrary's ILibraryAssetSources
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();

            #region Snippet
            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary2-fi.xml");
            builder.AddSource(assetSource).Build();
            #endregion Snippet

            // Create class
            ILine<MyClass> classLocalizer = localizer.Type<MyClass>();
            MyClassB myClass = new MyClassB(classLocalizer);

            // Use the culture that was provided with the class library (LibraryAssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use the culture that was supplied above
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
        }
    }
}

```
***
<details>
  <summary>The example localization file *TutorialLibrary2-fi.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "fi" -->
  <Type:TutorialLibrary2.MyClass Culture="fi">
    <Key:OK>Toiminto onnistui!</Key:OK>
  </Type:TutorialLibrary2.MyClass>

</Localization>

```
</details>
<br/>

When class is initialized with *IServiceProvider*, additional localizations are added to *IServiceCollection* as *IAssetSource*s.
Extension method **AddLexicalLocalization(this <i>IServiceCollection</i>)** adds the default services for localization.
# [Snippet](#tab/snippet-3)

```csharp
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
services.AddSingleton<IAssetSource>(LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary2-fi.xml"));

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
```
# [Full Code](#tab/full-3)

```csharp
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
            services.AddSingleton<IAssetSource>(LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary2-fi.xml"));

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

```
***
