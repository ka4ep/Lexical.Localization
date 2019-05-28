# Class Library
This article describes recommended practice for writing localization for a class library that doesn't use dependency injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **AssetSources** which implements **IAssetSources** as a signal 
to indicate that this class provides localizations for this class library.

Internal localization files are typically embedded resources.
[!code-csharp[Snippet](AssetSources.cs)]
<details>
  <summary>The example localization file *TutorialLibrary1-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary1-de.xml)]
</details>

Class library can be configured to search for external localization from preconfigured locations.
[!code-csharp[Snippet](AssetSourcesB.cs)]

## Localization Root
There should be another class called **Localization** that is used as the *ILineRoot* for the classes that use localization.
This root can be linked to the global static root and shares its assets. 
[!code-csharp[Snippet](Localization.cs)]

## Classes
Classes in the the class library can now use the localization root.
[!code-csharp[Snippet](MyClass.cs)]

# Application
When another class library or application uses the class library the localization works out-of-the-box.
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](Consumer1.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](Consumer1.cs)]
***

## Supplying Localizations
Application that deploys the class library can supply additional localizations by adding *IAssetSource*s to the global static **LocalizationRoot.Builder**.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](Consumer2.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](Consumer2.cs)]
***

<details>
  <summary>The example localization file *TutorialLibrary1-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary1-fi.xml)]
</details>
<br/>

