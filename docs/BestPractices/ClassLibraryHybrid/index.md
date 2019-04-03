# Class Library with Dependency Injection optional

This article describes recommended practice for writing a class library that, for purposes of localization, is compatible with and without Dependency Injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **LibraryAssetSources** which implements **ILibraryAssetSources** as a signal 
to indicate that this class provides localizations for this class library.

Internal localization files are typically embedded resources.
[!code-csharp[Snippet](LibraryAssetSources.cs)]
<details>
  <summary>The example localization file *TutorialLibrary3-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary3-de.xml)]
</details>

## Localization Root
There should be another class called **LibraryLocalization** that is used as the *IAssetRoot* for the classes that use localization.
This root can be linked to the global static root and shares its assets.
[!code-csharp[Snippet](LibraryLocalization.cs)]

## Classes
For inversion of control, the class library can use IStringLocalizer abstractions. The non-dependency injection instance is acquired from *LibraryLocalization* if *localizer* is null.
[!code-csharp[Snippet](MyClass.cs)]

... or alternatively Lexical.Localization.Abstractions.
[!code-csharp[Snippet](MyClassB.cs)]

# Application
Application that deploys the localizer must include the internal localizations with 
**<i>IAssetBuilder</i>.AddLibraryAssetSources(*Assembly*)** which searches the **ILibraryAssetSources** from the library.
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
  <summary>The example localization file *TutorialLibrary3-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary3-fi.xml)]
</details>
<br/>

When class is initialized with *IServiceProvider*, additional localizations are added to *IServiceCollection* as *IAssetSource*s.
The extension method **AddLexicalLocalization(this <i>IServiceCollection</i>)** adds the default services.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](LibraryConsumer3.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](LibraryConsumer3.cs)]
***
