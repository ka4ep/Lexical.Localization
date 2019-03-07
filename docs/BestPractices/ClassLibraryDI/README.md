# Class Library with Dependency Injection

This article describes recommended practice for writing a localized class library that uses inversion of control.

The developer of class library may want to provide its own builtin localizations. 
The recommended practice is to create a class **LibraryAssets** into the class library.
It should use **[AssetSources]** attribute to a signal that this class provides the localizations.

Internal localization files are typically added built-in as embedded resources.

```csharp
using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary2
{
    [AssetSources]
    public class LibraryAssets : List<IAssetSource>
    {
        public LibraryAssets() : base()
        {
            // Asset sources are added here
            Add(XmlFileFormat.Instance.CreateEmbeddedAssetSource(
                    asm: GetType().Assembly, 
                    resourceName: "docs.LibraryLocalization2-de.xml")
            );
        }
    }
}

```
<details>
  <summary>The example localization file *LibraryLocalization2-de.xml*.  (<u>click here</u>)</summary>

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
<br/>

For inversion of control, the class library can use IStringLocalizer abstractions

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
        IAssetKey<MyClass> localizer;

        public MyClassB(IAssetKey<MyClass> localizer)
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
<br/>

Application that deploys with its localizer can include its depending libraries internal localizations with 
**<i>IAssetBuilder</i>.AddLibraryAssetSources(*Assembly*)** which searches for **[AssetSources]** and adds them as *IAssetSource*s.
# [Snippet](#tab/snippet-1)

```csharp
// Create localizer
IAssetBuilder builder = new AssetBuilder.OneBuildInstance();
IAsset asset = builder.Build();
StringLocalizerRoot localizer = new StringLocalizerRoot(asset, new CulturePolicy());

// Install TutorialLibrary's [AssetSources]
Assembly library = typeof(MyClass).Assembly;
builder.AddLibraryAssetSources(library).Build();

// Create class
IStringLocalizer<MyClass> classLocalizer = localizer.Type<MyClass>();
MyClass myClass = new MyClass(classLocalizer);

// Use the culture that was provided with the class library (LibraryAssets)
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
Console.WriteLine(myClass.Do());
```
# [Full Code](#tab/full-1)

```csharp
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
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

            // Install TutorialLibrary's [AssetSources]
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();

            // Create class
            IStringLocalizer<MyClass> classLocalizer = localizer.Type<MyClass>();
            MyClass myClass = new MyClass(classLocalizer);

            // Use the culture that was provided with the class library (LibraryAssets)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());
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
// Install additional localization that was not available in the TutorialLibrary.
IAssetSource assetSource = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization2-fi.xml");
builder.AddSource(assetSource).Build();
```
# [Full Code](#tab/full-2)

```csharp
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
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

            // Install TutorialLibrary's [AssetSources]
            Assembly library = typeof(MyClass).Assembly;
            builder.AddLibraryAssetSources(library).Build();

            #region Snippet
            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization2-fi.xml");
            builder.AddSource(assetSource).Build();
            #endregion Snippet

            // Create class
            IAssetKey<MyClass> classLocalizer = localizer.Type<MyClass>();
            MyClassB myClass = new MyClassB(classLocalizer);

            // Use the culture that was provided with the class library (LibraryAssets)
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
  <summary>The example localization file *LibraryLocalization2-fi.xml*.  (<u>click here</u>)</summary>

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

// Install TutorialLibrary's [AssetSources].
Assembly library = typeof(MyClass).Assembly;
services.AddAssetLibrarySources(library);

// Install additional localization that was not available in the TutorialLibrary.
services.AddSingleton<IAssetSource>(XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization2-fi.xml"));

// Service MyClass2
services.AddTransient<MyClass, MyClass>();

// Create instance container
using (var provider = services.BuildServiceProvider())
{
    // Create class
    MyClass myClass = provider.GetService<MyClass>();

    // Use the culture that was provided by with the class library (LibraryAssets)
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
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Reflection;
using TutorialLibrary2;

namespace TutorialProject2
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

            // Install TutorialLibrary's [AssetSources].
            Assembly library = typeof(MyClass).Assembly;
            services.AddAssetLibrarySources(library);

            // Install additional localization that was not available in the TutorialLibrary.
            services.AddSingleton<IAssetSource>(XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization2-fi.xml"));

            // Service MyClass2
            services.AddTransient<MyClass, MyClass>();

            // Create instance container
            using (var provider = services.BuildServiceProvider())
            {
                // Create class
                MyClass myClass = provider.GetService<MyClass>();

                // Use the culture that was provided by with the class library (LibraryAssets)
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

