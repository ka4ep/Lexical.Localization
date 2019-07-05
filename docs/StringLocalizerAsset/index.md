# Introduction
**StringLocalizerFactoryAsset** adapts <b><i>IStringLocalizerFactory</i></b> implementations to *IAsset*.
[!code-csharp[Snippet](IopExamples.cs#Snippet_0a)]

There is an extension method **.ToAsset()** that makes the same conversion.
[!code-csharp[Snippet](IopExamples.cs#Snippet_0b)]

**StringLocalizerAsset** adapts <b><i>IStringLocalizer</i></b> implementations to *IAsset*.
[!code-csharp[Snippet](IopExamples.cs#Snippet_0c)]

**StringLocalizerRoot** adapts *IAsset* implementations back to <b><i>IStringLocalizerFactory</i></b>. [Read more.](../ILine/ILineRoot/#string-localizer)
[!code-csharp[Snippet](IopExamples.cs#Snippet_4a)]

And to <b><i>IStringLocalizer</i></b>.
[!code-csharp[Snippet](IopExamples.cs#Snippet_4b)]


# Links
* [Microsoft.Extensions.Localization.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/Localization/Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization.Abstractions/))
 * [IStringLocalizer](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizer.cs) 
 * [IStringLocalizer&lt;T&gt;](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerOfT.cs)
 * [IStringLocalizerFactory](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerFactory.cs)
* [Microsoft.Extensions.Localization](https://github.com/aspnet/Localization/tree/master/src/Microsoft.Extensions.Localization) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization/))
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [StringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringLocalizerAsset.cs)
 * [StringLocalizerRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringLocalizerRoot.cs)
 * [StringLocalizerFactoryAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringLocalizerFactoryAsset.cs)
 * [ResourceManagerStringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/ResourceManagerStringLocalizerAsset.cs)
 * [ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/ResourceManagerStringLocalizerAssetSource.cs)
