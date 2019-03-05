# Dependency Injection Class Library with 

This article describes recommended practice for writing a localized class library that uses inversion of control.

The developer of class library may want to provide builtin localizations. 
The recommended practice is to create a class **LibraryAssets** into the class library.
It should use **[AssetSources]** attribute to a signal that this class provides the library's internal localizations.

Internal localization files are typically added built-in as embedded resources.

```csharp
using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary
{
    [AssetSources]
    public class LibraryAssets : List<IAssetSource>
    {
        public LibraryAssets() : base()
        {
            // Asset sources are added here
            Add(XmlFileFormat.Instance.CreateEmbeddedAssetSource(
                    asm: GetType().Assembly, 
                    resourceName: "docs.LibraryLocalization-de.xml")
            );
        }
    }
}

```
<details>
  <summary>The example localization file *LibraryLocalization-de.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "de" -->
  <Type:TutorialLibrary.MyClass Culture="de">
    <Key:OK>Erfolgreich!</Key:OK>
  </Type:TutorialLibrary.MyClass>

  <Type:TutorialLibrary.MyClass2 Culture="de">
    <Key:OK>Erfolgreich!</Key:OK>
  </Type:TutorialLibrary.MyClass2>

</Localization>

```
</details>
<br/>

For inversion of control, the the class library must use IStringLocalizer abstractions

```csharp
using Microsoft.Extensions.Localization;

namespace TutorialLibrary
{
    public class MyClass2
    {
        IStringLocalizer localization;

        public MyClass2(IStringLocalizer<MyClass2> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization["OK"];
        }
    }
}

```

... or alternatively Lexical.Localization.Abstractions.

```csharp
using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyClass2b
    {
        IAssetKey localization;

        public MyClass2b(IAssetKey<MyClass2> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}

```
<br/>

The deploying application must include the library's internal localizations into its localization implementation.
**<i>IAssetBuilder</i>.AddLibrarySources(*Assembly*)** searches for **[AssetSources]** and adds them as *IAssetSource*s.
If implementation is not Lexical.Localization based, then the library localizations must be adapted.
# [Snippet](#tab/snippet-3)

```csharp
// Install localization libraries that are available in the TutorialLibrary.
// Search for classes with [AssetSources] attribute.
Assembly library = typeof(TutorialLibrary.MyClass2).Assembly;
StringLocalizerRoot.Builder.AddLibrarySources(library);

// Apply sources
StringLocalizerRoot.Builder.Build();

// Create class
IStringLocalizerFactory factory = StringLocalizerRoot.Global;
IStringLocalizer<TutorialLibrary.MyClass2> localizer = StringLocalizerRoot.Global.Type<TutorialLibrary.MyClass2>();
TutorialLibrary.MyClass2 myClass = new TutorialLibrary.MyClass2( localizer );

// Use culture that was provided with the class library
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
Console.WriteLine(myClass.Do());

// Use culture that was supplied by this application
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
Console.WriteLine(myClass.Do());
```
# [Full Code](#tab/full-3)

```csharp
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TutorialProject
{
    public class Program3
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Install localization libraries that are available in the TutorialLibrary.
            // Search for classes with [AssetSources] attribute.
            Assembly library = typeof(TutorialLibrary.MyClass2).Assembly;
            StringLocalizerRoot.Builder.AddLibrarySources(library);

            // Apply sources
            StringLocalizerRoot.Builder.Build();

            // Create class
            IStringLocalizerFactory factory = StringLocalizerRoot.Global;
            IStringLocalizer<TutorialLibrary.MyClass2> localizer = StringLocalizerRoot.Global.Type<TutorialLibrary.MyClass2>();
            TutorialLibrary.MyClass2 myClass = new TutorialLibrary.MyClass2( localizer );

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use culture that was supplied by this application
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
            #endregion Snippet

            Console.ReadKey();
        }
    }
}

```
***
<br/>

The application can also add additional localization sources with **<i>IAssetBuilder</i>.AddSource(*IAssetSource*)**
# [Snippet](#tab/snippet-3b)

```csharp
// Install additional localization that was not available in the TutorialLibrary.
IAssetSource assetSource = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization-fi.xml");
StringLocalizerRoot.Builder.AddSource(assetSource);
```
# [Full Code](#tab/full-3b)

```csharp
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TutorialProject
{
    public class Program3b
    {
        public static void Main(string[] args)
        {
            // Install localization libraries that are available in the TutorialLibrary.
            // Search for classes with [AssetSources] attribute.
            Assembly library = typeof(TutorialLibrary.MyClass2).Assembly;
            StringLocalizerRoot.Builder.AddLibrarySources(library);

            #region Snippet
            // Install additional localization that was not available in the TutorialLibrary.
            IAssetSource assetSource = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization-fi.xml");
            StringLocalizerRoot.Builder.AddSource(assetSource);
            #endregion Snippet

            // Apply sources
            StringLocalizerRoot.Builder.Build();

            // Create class
            IStringLocalizerFactory factory = StringLocalizerRoot.Global;
            IStringLocalizer<TutorialLibrary.MyClass2> localizer = StringLocalizerRoot.Global.Type<TutorialLibrary.MyClass2>();
            TutorialLibrary.MyClass2 myClass = new TutorialLibrary.MyClass2( localizer );

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use culture that was supplied by this application
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());

            Console.ReadKey();
        }
    }
}

```
***
<details>
  <summary>The example localization file *LibraryLocalization-fi.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "fi" -->
  <Type:TutorialLibrary.MyClass Culture="fi">
    <Key:OK>Toiminto onnistui!</Key:OK>
  </Type:TutorialLibrary.MyClass>

  <Type:TutorialLibrary.MyClass2 Culture="fi">
    <Key:OK>Toiminto onnistui!</Key:OK>
  </Type:TutorialLibrary.MyClass2>

</Localization>

```
</details>
<br/>

The same example as above but with dependency injection. **<i>IServiceCollection</i>.AddLexicalLocalization()** adds the default services.
# [Snippet](#tab/snippet-4)

```csharp
IServiceCollection services = new ServiceCollection();

// Install default IStringLocalizerFactory
services.AddLexicalLocalization(
    addStringLocalizerService: true,
    addCulturePolicyService: true,
    useGlobalInstance: true,
    addCache: false);

// Install Library's [AssetSources].
Assembly library = typeof(TutorialLibrary.MyClass2).Assembly;
services.AddLibrarySources(library);

// Install additional localization that was not available in the TutorialLibrary.
services.AddSingleton<IAssetSource>(XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization-fi.xml"));

// Service for MyClass2
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
```
# [Full Code](#tab/full-4)

```csharp
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
            services.AddLibrarySources(library);

            // Install additional localization that was not available in the TutorialLibrary.
            services.AddSingleton<IAssetSource>(XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization-fi.xml"));

            // Service for MyClass2
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

```
***

