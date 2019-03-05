# Dependency Injection Class Library with 

This article describes recommended practice for writing a localized class library that uses inversion of control.

The developer of class library may want to provide builtin localizations. 
The recommended practice is to create a class **LibraryAssets** into the class library.
It should use **[AssetSources]** attribute to a signal that this class provides the library's internal localizations.

Internal localization files are typically added built-in as embedded resources.
[!code-csharp[Snippet](../ClassLibrary/LibraryAssets.cs)]
<details>
  <summary>The example localization file *LibraryLocalization-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization-de.xml)]
</details>
<br/>

For inversion of control, the the class library must use IStringLocalizer abstractions
[!code-csharp[Snippet](MyClass2.cs)]

... or alternatively Lexical.Localization.Abstractions.
[!code-csharp[Snippet](MyClass2b.cs)]
<br/>

The deploying application must include the library's internal localizations into its localization implementation.
**<i>IAssetBuilder</i>.AddLibraryAssetSources(*Assembly*)** searches for **[AssetSources]** and adds them as *IAssetSource*s.
If implementation is not Lexical.Localization based, then the library localizations must be adapted.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](LibraryConsumer3.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](LibraryConsumer3.cs)]
***
<br/>

The application can also add additional localization sources with **<i>IAssetBuilder</i>.AddSource(*IAssetSource*)**
# [Snippet](#tab/snippet-3b)
[!code-csharp[Snippet](LibraryConsumer3b.cs#Snippet)]
# [Full Code](#tab/full-3b)
[!code-csharp[Snippet](LibraryConsumer3b.cs)]
***
<details>
  <summary>The example localization file *LibraryLocalization-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization-fi.xml)]
</details>
<br/>

The same example as above but with dependency injection. **<i>IServiceCollection</i>.AddLexicalLocalization()** adds the default services.
# [Snippet](#tab/snippet-4)
[!code-csharp[Snippet](LibraryConsumer4.cs#Snippet)]
# [Full Code](#tab/full-4)
[!code-csharp[Snippet](LibraryConsumer4.cs)]
***

