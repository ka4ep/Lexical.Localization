# Ms.DependencyInjection
Microsoft has introduced an abstraction to dependency injection in the NuGet package **Microsoft.Extensions.DependencyInjection.Abstractions**.

The extension method **<i>IServiceCollection</i>.AddLexicalLocalization(*addStringLocalizerService*, *addCulturePolicyService*, *useGlobalInstance*, *addCache*)** 
that adds various localization service implementations. The following services are added:
* IAssetRoot. If *useGlobalInstance* is true, then the root is **[global static](../IAssetRoot#global-static-root)**.
* IAssetKey&lt;T&gt;
* IAssetBuilder
* ICulturePolicy, if *addCulturePolicyService* is set to true.
* IStringLocalizerFactory and *IStringLocalizer&lt;T&gt;*, if *addStringLocalizerService* is set to true.

[!code-csharp[Snippet](Example0.cs#Snippet_1)]

Assets are contributed to the service provider by adding *IAssetSource*s.
[!code-csharp[Snippet](Example0.cs#Snippet_2)]

Asset, root and keys can be acquired from the service provider.
[!code-csharp[Snippet](Example0.cs#Snippet_3)]

<details><summary>Example full code (<u>click here</u>).</summary>[!code-csharp[Snippet](Example1.cs)]</details>

# String localizer
When the argument *addStringLocalizerService* is set to true, then the extension method adds implementations to  
services *IStringLocalizer&lt;T&gt;* and *IStringLocalizerFactory*.
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](Example2.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](Example2.cs)]
***

# Links
* [Microsoft.Extensions.DependencyInjection.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/DependencyInjection/DI.Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/))
 * [IServiceCollection](https://github.com/aspnet/Extensions/blob/master/src/DependencyInjection/DI.Abstractions/src/IServiceCollection.cs)
* [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/Extensions/tree/master/src/DependencyInjection/DI/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/))
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [MsLocalizationExtensions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/DependencyInjection/MsLocalizationExtensions.cs)
 * [MsConfigurationExtensions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/DependencyInjection/MsConfigurationExtensions.cs)
