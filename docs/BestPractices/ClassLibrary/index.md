# Standalone Class Library

This article describes recommended practice for writing a localized class library that doesn't use dependency injection.

The developer of class library may want to provide builtin localizations. 
The recommended practice is to create a class **LibraryAssets** into the class library.
It should use **[AssetSources]** attribute to a signal that this class provides the library's internal localizations.

Internal localization files are typically added built-in as embedded resources.
[!code-csharp[Snippet](LibraryAssets.cs)]
<details>
  <summary>The example localization file *LibraryLocalization-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization-de.xml)]
</details>
<br/>

There should be another class called **LibraryLocalization** that is used as the *IAssetRoot* for the classes that use localization.
This root is linked to the global static root and shares its assets.
[!code-csharp[Snippet](LibraryLocalization.cs)]
<br/> 

Now all the other code in the class library can now use the library's localization root.
[!code-csharp[Snippet](MyClass.cs)]
<br/>

When another class library or application uses the class library the localization works out-of-the-box.
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](LibraryConsumer2.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](LibraryConsumer2.cs)]
***

<br/>
If the deployer of the application wants to add additional localizations for the imported class libraries, these localizations
can be added to the global static **LocalizationRoot.Builder**.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](LibraryConsumer2b.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](LibraryConsumer2b.cs)]
***

<details>
  <summary>The example localization file *LibraryLocalization-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization-fi.xml)]
</details>
<br/>
