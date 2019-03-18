# Class Library with Dependency Injection

This article describes recommended practice for writing a localized class library that uses inversion of control.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **LibraryAssetSources** which implements **ILibraryAssetSources** to signal that this class provides the locations of its internal localizations.

Internal localization files are typically added built-in as embedded resources.
[!code-csharp[Snippet](LibraryAssetSources.cs)]
<details>
  <summary>The example localization file *TutorialLibrary2-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary2-de.xml)]
</details>

## Using Localizer
For inversion of control, the class library can use IStringLocalizer abstractions
[!code-csharp[Snippet](MyClass.cs)]

... or alternatively Lexical.Localization.Abstractions.
[!code-csharp[Snippet](MyClassB.cs)]

## Deploying Localizer
Application that deploys the localizer must include the internal localizations with 
**<i>IAssetBuilder</i>.AddLibraryAssetSources(*Assembly*)** which searches the **ILibraryAssetSources** of the library.
# [Snippet](#tab/snippet-1)
[!code-csharp[Snippet](LibraryConsumer1.cs#Snippet)]
# [Full Code](#tab/full-1)
[!code-csharp[Snippet](LibraryConsumer1.cs)]
***
<br/>

The application can supply additional localization sources with **<i>IAssetBuilder</i>.AddSource(*IAssetSource*)**
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](LibraryConsumer2.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](LibraryConsumer2.cs)]
***
<details>
  <summary>The example localization file *TutorialLibrary2-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary2-fi.xml)]
</details>
<br/>

When class is initialized with *IServiceProvider*, additional localizations are added to *IServiceCollection* as *IAssetSource*s.
Extension method **AddLexicalLocalization(this <i>IServiceCollection</i>)** adds the default services for localization.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](LibraryConsumer3.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](LibraryConsumer3.cs)]
***

## External Localization
Class library can be configured to search for external localization from preconfigured locations.
[!code-csharp[Snippet](LibraryAssetSourcesB.cs)]
