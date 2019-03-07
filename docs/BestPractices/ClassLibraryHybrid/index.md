﻿# Class Library with compability to Dependency Injection

This article describes recommended practice for writing a class library that, for purposes of localization, is compatible with and without Dependency Injection.

The developer of class library may want to provide its own builtin localizations. 
The recommended practice is to create a class **LibraryAssets** into the class library.
It should use **[AssetSources]** attribute to a signal that this class provides the localizations.

Internal localization files are typically added built-in as embedded resources.
[!code-csharp[Snippet](LibraryAssets.cs)]
<details>
  <summary>The example localization file *LibraryLocalization3-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization3-de.xml)]
</details>
<br/>

There should be another class called **LibraryLocalization** that is used as the *IAssetRoot* for the classes that use localization.
This root is linked to the global static root and shares its assets.
[!code-csharp[Snippet](LibraryLocalization.cs)]
<br/> 

For inversion of control, the class library can use IStringLocalizer abstractions. The non-dependency injection instance is acquired from *LibraryLocalization* if *localizer* is null.
[!code-csharp[Snippet](MyClass.cs)]

... or alternatively Lexical.Localization.Abstractions.
[!code-csharp[Snippet](MyClassB.cs)]
<br/>

Application that deploys with its localizer can include its depending libraries internal localizations with 
**<i>IAssetBuilder</i>.AddLibraryAssetSources(*Assembly*)** which searches for **[AssetSources]** and adds them as *IAssetSource*s.
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
  <summary>The example localization file *LibraryLocalization3-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization3-fi.xml)]
</details>
<br/>

When class is initialized with *IServiceProvider*, additional localizations are added to *IServiceCollection* as *IAssetSource*s.
The extension method **AddLexicalLocalization(this <i>IServiceCollection</i>)** adds the default services.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](LibraryConsumer3.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](LibraryConsumer3.cs)]
***
