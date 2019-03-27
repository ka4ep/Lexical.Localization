# Class Library with Dependency Injection optional

This article describes recommended practice for writing a class library that, for purposes of localization, is compatible with and without Dependency Injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **LibraryAssetSources** which implements **ILibraryAssetSources** to signal that this class provides the locations of its internal localizations.

Internal localization files are typically added built-in as embedded resources.

```csharp
using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary3
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        public LibraryAssetSources() : base()
        {
            // Create source that reads an embedded resource.
            IAssetSource internalLocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary3-de.xml");
            // Asset sources are added here
            Add(internalLocalizationSource);
        }

        public LibraryAssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external localization source
            if (fileProvider != null)
            {
                IAssetSource externalLocalizationSource = LocalizationXmlReader.Instance.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary3.xml", throwIfNotFound: false);
                Add(externalLocalizationSource);
            }
        }
    }
}

```
<details>
  <summary>The example localization file *TutorialLibrary3-de.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "de" -->
  <Type:TutorialLibrary3.MyClass Culture="de">
    <Key:OK>Erfolgreich!</Key:OK>
  </Type:TutorialLibrary3.MyClass>

</Localization>

```
</details>

## Using Localizer
There should be another class called **LibraryLocalization** that is used as the *IAssetRoot* for the classes that use localization.
This root is linked to the global static root and shares its assets.

```csharp
using Lexical.Localization;

namespace TutorialLibrary3
{
    internal class LibraryLocalization : StringLocalizerRoot.LinkedTo
    {
        private static readonly LibraryLocalization instance = new LibraryLocalization(LocalizationRoot.Global);

        /// <summary>
        /// Singleton instance to localization root for this class library.
        /// </summary>
        public static LibraryLocalization Root => instance;

        /// <summary>
        /// Add asset sources here. Then call <see cref="IAssetBuilder.Build"/> to make effective.
        /// </summary>
        public new static IAssetBuilder Builder => LocalizationRoot.Builder;

        LibraryLocalization(IAssetRoot linkedTo) : base(linkedTo)
        {
            // Add library's internal assets here
            Builder.AddSources(new LibraryAssetSources());
            // Apply changes
            Builder.Build();
        }
    }
}

```

For inversion of control, the class library can use IStringLocalizer abstractions. The non-dependency injection instance is acquired from *LibraryLocalization* if *localizer* is null.

```csharp
using Microsoft.Extensions.Localization;

namespace TutorialLibrary3
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localizer;

        public MyClass(IStringLocalizer<MyClass> localizer = default)
        {
            this.localizer = localizer ?? LibraryLocalization.Root.Type<MyClass>();
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

namespace TutorialLibrary3
{
    public class MyClassB
    {
        IAssetKey<MyClass> localizer;

        public MyClassB(IAssetKey<MyClass> localizer = default)
        {
            this.localizer = localizer ?? LibraryLocalization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localizer.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}

```

## Deploying Localizer
Application that deploys the localizer must include the internal localizations with 
**<i>IAssetBuilder</i>.AddLibraryAssetSources(*Assembly*)** which searches the **ILibraryAssetSources** of the library.
# [Snippet](#tab/snippet-1)

```csharp
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
```
# [Full Code](#tab/full-1)

```csharp
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

```
***
<br/>

The application can supply additional localization sources with **<i>IAssetBuilder</i>.AddSource(*IAssetSource*)**
# [Snippet](#tab/snippet-2)

```csharp
// Create class without localizer
MyClass myClass1 = new MyClass(default);

// Use the culture that was provided by with the class library (LibraryAssetSources)
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
Console.WriteLine(myClass1.Do());

// Install additional localization that was not available in the TutorialLibrary.
IAssetSource assetSource = LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary3-fi.xml");
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
```
# [Full Code](#tab/full-2)

```csharp
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
            IAssetSource assetSource = LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary3-fi.xml");
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

```
***
<details>
  <summary>The example localization file *TutorialLibrary3-fi.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "fi" -->
  <Type:TutorialLibrary3.MyClass Culture="fi">
    <Key:OK>Toiminto onnistui!</Key:OK>
  </Type:TutorialLibrary3.MyClass>

</Localization>

```
</details>
<br/>

When class is initialized with *IServiceProvider*, additional localizations are added to *IServiceCollection* as *IAssetSource*s.
The extension method **AddLexicalLocalization(this <i>IServiceCollection</i>)** adds the default services.
# [Snippet](#tab/snippet-3)

```csharp
IServiceCollection services = new ServiceCollection();

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
services.AddSingleton<IAssetSource>(LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary3-fi.xml"));

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

    // Use the culture that was supplied above
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
using TutorialLibrary3;

namespace TutorialProject3
{
    public class Program3
    {
        public static void Main(string[] args)
        {
            #region Snippet
            IServiceCollection services = new ServiceCollection();

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
            services.AddSingleton<IAssetSource>(LocalizationXmlReader.Instance.FileAssetSource("TutorialLibrary3-fi.xml"));

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

                // Use the culture that was supplied above
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
                Console.WriteLine(myClass.Do());
            }
            #endregion Snippet
        }
    }
}

```
***
