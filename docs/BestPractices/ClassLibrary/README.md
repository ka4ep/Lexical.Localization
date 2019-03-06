# Class Library

This article describes recommended practice for writing a localized class library that doesn't use dependency injection.

The developer of class library may want to provide its own builtin localizations. 
The recommended practice is to create a class **LibraryAssets** into the class library.
It should use **[AssetSources]** attribute to a signal that this class provides the localizations.

Internal localization files are typically added embedded resources.

```csharp
using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    [AssetSources]
    public class LibraryAssets : List<IAssetSource>
    {
        public LibraryAssets() : base()
        {
            // Asset sources are added here
            Add(XmlFileFormat.Instance.CreateEmbeddedAssetSource(
                    asm: GetType().Assembly, 
                    resourceName: "docs.LibraryLocalization1-de.xml")
            );
        }
    }
}

```
<details>
  <summary>The example localization file *LibraryLocalization1-de.xml*.  (<u>click here</u>)</summary>

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
<br/>

There should be another class called **LibraryLocalization** that is used as the *IAssetRoot* for the classes that use localization.
This root is linked to the global static root and shares its assets.

```csharp
using Lexical.Localization;

namespace TutorialLibrary1
{
    internal class LibraryLocalization : LocalizationRoot.LinkedTo
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
            Builder.AddSources(new LibraryAssets());
            // Apply changes
            Builder.Build();
        }
    }
}

```
<br/> 

All the other code in the class library can now use the library's localization root.

```csharp
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class MyClass
    {
        static IAssetKey localization = LibraryLocalization.Root.Type<MyClass>();

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}

```
<br/>

When another class library or application uses the class library the localization works out-of-the-box.
# [Snippet](#tab/snippet-2)

```csharp
MyClass myClass = new MyClass();

// Use default string
Console.WriteLine(myClass.Do());

// Use culture that was provided with the class library
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

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}

```
***

<br/>
The application can supply additional localizations by placing *IAssetSource*s to the global static **LocalizationRoot.Builder**.
# [Snippet](#tab/snippet-3)

```csharp
// Install additional localization that was not available in the TutorialLibrary
IAssetSource source = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization1-fi.xml");
LocalizationRoot.Builder.AddSource(source).Build();

MyClass myClass = new MyClass();

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
            IAssetSource source = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization1-fi.xml");
            LocalizationRoot.Builder.AddSource(source).Build();

            MyClass myClass = new MyClass();

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use culture that was supplied by this application
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}

```
***

<details>
  <summary>The example localization file *LibraryLocalization1-fi.xml*.  (<u>click here</u>)</summary>

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
