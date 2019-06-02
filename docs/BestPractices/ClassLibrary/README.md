# Class Library
This article describes recommended practice for writing localization for a class library that doesn't use dependency injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **AssetSources** which implements **IAssetSources** as a signal 
to indicate that this class provides localizations for this class library.

Internal localization files are typically embedded resources.

```csharp
using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace TutorialLibrary1
{
    public class AssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Default.EmbeddedAssetSource(typeof(AssetSources).Assembly, "docs.TutorialLibrary1-de.xml");

        public AssetSources() : base()
        {
            // Asset sources are added here
            Add(LocalizationSource);
        }
    }
}

```
<details>
  <summary>The example localization file *TutorialLibrary1-de.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "de" -->
  <Type:TutorialLibrary1.MyClass Culture="de">
    <Key:OK>Erfolgreich!</Key:OK>
  </Type:TutorialLibrary1.MyClass>

</Localization>

```
</details>

Class library can be configured to search for external localization from preconfigured locations.

```csharp
using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace TutorialLibrary1
{
    public class AssetSourcesB : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Default.EmbeddedAssetSource(typeof(AssetSources).Assembly, "docs.TutorialLibrary1-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LineFileSource ExternalLocalizationSource = 
            LineReaderMap.Default.FileAssetSource("Localization.xml", throwIfNotFound: false);

        public AssetSourcesB() : base()
        {
            // Add internal localization source
            Add(LocalizationSource);
            // Add optional external localization source
            Add(ExternalLocalizationSource);
        }
    }
}

```

## Localization Root
There should be another class called **Localization** that is used as the *ILineRoot* for the classes that use localization.
This root can be linked to the global static root and shares its assets. 

```csharp
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace TutorialLibrary1
{
    internal class Localization : LineRoot.LinkedTo
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

        Localization(ILineRoot linkedTo) : base(null, linkedTo, null, null, null, null, null)
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
Classes in the the class library can now use the localization root.

```csharp
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class MyClass
    {
        static ILine localizer = Localization.Root.Type<MyClass>();

        public string Do()
        {
            return localizer.Key("OK").Format("Operation Successful").ToString();
        }
    }
}

```

# Application
When another class library or application uses the class library the localization works out-of-the-box.
# [Snippet](#tab/snippet-2)

```csharp
MyClass myClass = new MyClass();

// Use default string
Console.WriteLine(myClass.Do());

// Use the culture that was provided with the class library (AssetSources)
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
Console.WriteLine(myClass.Do());
```
# [Full Code](#tab/full-2)

```csharp
using System;
using System.Globalization;
using TutorialLibrary1;

namespace TutorialProject1
{
    public class Program1
    {
        public static void Main(string[] args)
        {
            #region Snippet
            MyClass myClass = new MyClass();

            // Use default string
            Console.WriteLine(myClass.Do());

            // Use the culture that was provided with the class library (AssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}

```
***

## Supplying Localizations
Application that deploys the class library can supply additional localizations by adding *IAssetSource*s to the global static **LocalizationRoot.Builder**.
# [Snippet](#tab/snippet-3)

```csharp
// Install additional localization that was not available in the TutorialLibrary
IAssetSource source = XmlLinesReader.Default.FileAssetSource("TutorialLibrary1-fi.xml");
LineRoot.Builder.AddSource(source).Build();

MyClass myClass = new MyClass();

// Use the culture that was provided with the class library (AssetSources)
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
Console.WriteLine(myClass.Do());

// Use the culture that we supplied above
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
Console.WriteLine(myClass.Do());
```
# [Full Code](#tab/full-3)

```csharp
using Lexical.Localization;
using Lexical.Localization.Asset;
using System;
using System.Globalization;
using TutorialLibrary1;

namespace TutorialProject1
{
    public class Program2
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Install additional localization that was not available in the TutorialLibrary
            IAssetSource source = XmlLinesReader.Default.FileAssetSource("TutorialLibrary1-fi.xml");
            LineRoot.Builder.AddSource(source).Build();

            MyClass myClass = new MyClass();

            // Use the culture that was provided with the class library (AssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use the culture that we supplied above
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}

```
***

<details>
  <summary>The example localization file *TutorialLibrary1-fi.xml*.  (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Type="urn:lexical.fi:Type"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns="urn:lexical.fi">

  <!-- Example: Localization string for Culture "fi" -->
  <Type:TutorialLibrary1.MyClass Culture="fi">
    <Key:OK>Toiminto onnistui!</Key:OK>
  </Type:TutorialLibrary1.MyClass>

</Localization>

```
</details>
<br/>

