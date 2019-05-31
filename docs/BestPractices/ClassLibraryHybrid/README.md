# Class Library with Dependency Injection optional

This article describes recommended practice for writing a class library that, for purposes of localization, is compatible with and without Dependency Injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **AssetSources** which implements **IAssetSources** as a signal 
to indicate that this class provides localizations for this class library.

Internal localization files are typically embedded resources.

```csharp
using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary3
{
    public class AssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Default.EmbeddedAssetSource(typeof(AssetSources).Assembly, "docs.TutorialLibrary3-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LineFileProviderSource ExternalLocalizationSource;

        public AssetSources() : base()
        {
            // Add internal localization source
            Add(LocalizationSource);
        }

        public AssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external localization source
            if (fileProvider != null)
            {
                ExternalLocalizationSource = 
                    XmlLinesReader.Default.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary3.xml", throwIfNotFound: false);
                Add(ExternalLocalizationSource);
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

## Localization Root
There should be another class called **Localization** that is used as the *ILineRoot* for the classes that use localization.
This root can be linked to the global static root and shares its assets.

```csharp
using Lexical.Localization;

namespace TutorialLibrary3
{
    internal class Localization : StringLocalizerRoot.LinkedTo, ILineRoot
    {
        private static readonly Localization instance = new Localization(LineRoot.Global);

        /// <summary>
        /// Singleton instance to localization root for this class library.
        /// </summary>
        public static Localization Root => instance;

        /// <summary>
        /// Add asset sources here. Then call <see cref="IAssetBuilder.Build"/> to make effective.
        /// </summary>
        public new static IAssetBuilder Builder => LineRoot.Builder;

        Localization(ILineRoot linkedTo) : base(null, linkedTo)
        {
            // Add library's internal assets here
            Builder.AddSources(new AssetSources());
            // Apply changes
            Builder.Build();
        }
    }
}

```

## Classes
For inversion of control, the class library can use IStringLocalizer abstractions. The non-dependency injection instance is acquired from *Localization* if *localizer* is null.

```csharp
using Lexical.Localization;
using Microsoft.Extensions.Localization;

namespace TutorialLibrary3
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localizer;

        public MyClass(IStringLocalizer<MyClass> localizer = default)
        {
            this.localizer = localizer ?? (Localization.Root.Type<MyClass>().AsStringLocalizer<MyClass>());
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
        ILine<MyClass> localizer;

        public MyClassB(ILine<MyClass> localizer = default)
        {
            this.localizer = localizer ?? Localization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localizer.Key("OK").Value("Operation Successful").ToString();
        }
    }
}

```

# Application
Application that deploys the localizer must include the internal localizations with 
**<i>IAssetBuilder</i>.AddAssetSources(*Assembly*)** which searches the **IAssetSources** from the library.
# [Snippet](#tab/snippet-1)

```csharp
// Create class without localizer
MyClass myClass1 = new MyClass(default);

// Create localizer
IAssetBuilder builder = new AssetBuilder.OneBuildInstance();
IAsset asset = builder.Build();
IStringLocalizerFactory localizer = new StringLocalizerRoot(asset, new CulturePolicy());

// Install TutorialLibrary's IAssetSources
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

            // Install TutorialLibrary's IAssetSources
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

// Use the culture that was provided by with the class library (AssetSources)
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
Console.WriteLine(myClass1.Do());

// Install additional localization that was not available in the TutorialLibrary.
IAssetSource assetSource = XmlLinesReader.Default.FileAssetSource("TutorialLibrary3-fi.xml");
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
            // Install TutorialLibrary's IAssetSources
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();
            #region Snippet

            // Create class without localizer
            MyClass myClass1 = new MyClass(default);

            // Use the culture that was provided by with the class library (AssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass1.Do());

            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = XmlLinesReader.Default.FileAssetSource("TutorialLibrary3-fi.xml");
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

// Install TutorialLibrary's IAssetSources.
Assembly library = typeof(MyClass).Assembly;
services.AddLibraryAssetSources(library);

// Install additional localization that was not available in the TutorialLibrary.
services.AddSingleton<IAssetSource>(XmlLinesReader.Default.FileAssetSource("TutorialLibrary3-fi.xml"));

// Service MyClass
services.AddTransient<MyClass, MyClass>();

// Create instance container
using (var provider = services.BuildServiceProvider())
{
    // Create class
    MyClass myClass = provider.GetService<MyClass>();

    // Use the culture that was provided by with the class library (AssetSources)
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

            // Install TutorialLibrary's IAssetSources.
            Assembly library = typeof(MyClass).Assembly;
            services.AddLibraryAssetSources(library);

            // Install additional localization that was not available in the TutorialLibrary.
            services.AddSingleton<IAssetSource>(XmlLinesReader.Default.FileAssetSource("TutorialLibrary3-fi.xml"));

            // Service MyClass
            services.AddTransient<MyClass, MyClass>();

            // Create instance container
            using (var provider = services.BuildServiceProvider())
            {
                // Create class
                MyClass myClass = provider.GetService<MyClass>();

                // Use the culture that was provided by with the class library (AssetSources)
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
