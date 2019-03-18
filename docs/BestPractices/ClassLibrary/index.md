# Class Library
This article describes recommended practice for writing a localized class library that doesn't use dependency injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **LibraryAssetSources** which implements **ILibraryAssetSources** to signal that this class provides the locations of its internal localizations.

Internal localization files are typically added embedded resources.
[!code-csharp[Snippet](LibraryAssetSources.cs)]
<details>
  <summary>The example localization file *TutorialLibrary1-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary1-de.xml)]
</details>

## Using Localizer
There should be another class called **LibraryLocalization** that is used as the *IAssetRoot* for the classes that use localization.
This root is linked to the global static root and shares its assets.
[!code-csharp[Snippet](LibraryLocalization.cs)]
<br/> 

All the other code in the class library can now use the library's localization root.
[!code-csharp[Snippet](MyClass.cs)]

## Using the class library
When another class library or application uses the class library the localization works out-of-the-box.
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](LibraryConsumer1.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](LibraryConsumer1.cs)]
***

## Supplying Extra Localizations
Application that deploys the class library can supply additional localizations by adding *IAssetSource*s to the global static **LocalizationRoot.Builder**.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](LibraryConsumer2.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](LibraryConsumer2.cs)]
***

<details>
  <summary>The example localization file *TutorialLibrary1-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary1-fi.xml)]
</details>
<br/>

## External Localization
Class library can be configured to search for external localization from preconfigured locations.
[!code-csharp[Snippet](LibraryAssetSourcesB.cs)]
