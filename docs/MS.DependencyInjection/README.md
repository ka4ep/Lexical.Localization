# Ms.DependencyInjection
Microsoft has introduced an abstraction to dependency injection in the NuGet package **Microsoft.Extensions.DependencyInjection.Abstractions**.

The extension method **<i>IServiceCollection</i>.AddLexicalLocalization(*addStringLocalizerService*, *addCulturePolicyService*, *useGlobalInstance*, *addCache*)** 
that adds various localization service implementations. The following services are added:
* IAssetRoot. If *useGlobalInstance* is true, then the root is **[global static](../IAssetRoot#global-static-root)**.
* IAssetKey&lt;T&gt;
* IAssetBuilder
* ICulturePolicy, if *addCulturePolicyService* is set to true.
* IStringLocalizerFactory and *IStringLocalizer&lt;T&gt;*, if *addStringLocalizerService* is set to true.


```csharp
// Create service collection
IServiceCollection serviceCollection = new ServiceCollection();

// Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
serviceCollection.AddLexicalLocalization(
    addStringLocalizerService: false,
    addCulturePolicyService: true,
    useGlobalInstance: false,
    addCache: true);
```

Assets are contributed to the service provider by adding *IAssetSource*s.

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "Culture:en:Type:ConsoleApp1.MyController:Key:Hello", "Hello World!" } };
// Create asset source
IAssetSource assetSource = new LocalizationAsset(source, ParameterPolicy.Instance).ToSource();
// Add asset source
serviceCollection.AddSingleton<IAssetSource>(assetSource);
```

Asset, root and keys can be acquired from the service provider.

```csharp
// Build service provider
using (var serviceProvider = serviceCollection.BuildServiceProvider())
{
    // Service can provide the asset
    IAsset asset = serviceProvider.GetService<IAsset>();

    // Service can provide root
    IAssetRoot root = serviceProvider.GetService<IAssetRoot>();

    // Service can provide type key
    IAssetKey typeKey = serviceProvider.GetService<IAssetKey<ConsoleApp1.MyController>>();

    // Get "Hello World!"
    string str = typeKey.Key("Hello").Culture("en").ToString();
}
```

<details><summary>Example full code (<u>click here</u>).</summary>
```csharp
using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace docs
{
    public class Ms_DependencyInjection_Example1
    {
        public static void Main(string[] args)
        {
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);

            // Create localization source
            var source = new Dictionary<string, string> { { "Culture:en:Type:ConsoleApp1.MyController:Key:Hello", "Hello World!" } };
            // Create asset source
            IAssetSource assetSource = new LocalizationAsset(source, ParameterPolicy.Instance).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);

            // Build service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Service can provide the asset
                IAsset asset = serviceProvider.GetService<IAsset>();

                // Service can provide root
                IAssetRoot root = serviceProvider.GetService<IAssetRoot>();

                // Service can provide type key
                IAssetKey typeKey = serviceProvider.GetService<IAssetKey<ConsoleApp1.MyController>>();

                // Get "Hello World!"
                string str = typeKey.Key("Hello").Culture("en").ToString();
            }
        }
    }

}

```</details>

# String localizer
When the argument *addStringLocalizerService* is set to true, then the extension method adds implementations to  
services *IStringLocalizer&lt;T&gt;* and *IStringLocalizerFactory*.
# [Snippet](#tab/snippet-2)

```csharp
// Create service collection
IServiceCollection serviceCollection = new ServiceCollection();

// Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
//                            IStringLocalizer<T>, IStringLocalizerFactory
serviceCollection.AddLexicalLocalization(
    addStringLocalizerService: true,     // <- string localizer
    addCulturePolicyService: true,
    useGlobalInstance: false,
    addCache: true);

// Create localization source
var source = new Dictionary<string, string> {
    { "Culture:en:Type:ConsoleApp1.MyController:Key:Hello", "Hello World!" }
};
// Create asset source
IAssetSource assetSource = new LocalizationAsset(source, ParameterPolicy.Instance).ToSource();
// Add asset source
serviceCollection.AddSingleton<IAssetSource>(assetSource);

// Build service provider
using (var serviceProvider = serviceCollection.BuildServiceProvider())
{
    // Get string localizer for class "ConsoleApp1.MyController".
    IStringLocalizer stringLocalizer 
        = serviceProvider.GetService<IStringLocalizer<ConsoleApp1.MyController>>();

    // Narrow scope down to "en" culture
    IStringLocalizer stringLocalizerScoped = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("en"));

    // Get "Hello World!"
    string str = stringLocalizerScoped.GetString("Hello");
}
```
# [Full Code](#tab/full-2)

```csharp
using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace docs
{
    public class Ms_DependencyInjection_Example2
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
            //                            IStringLocalizer<T>, IStringLocalizerFactory
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: true,     // <- string localizer
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);

            // Create localization source
            var source = new Dictionary<string, string> {
                { "Culture:en:Type:ConsoleApp1.MyController:Key:Hello", "Hello World!" }
            };
            // Create asset source
            IAssetSource assetSource = new LocalizationAsset(source, ParameterPolicy.Instance).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);

            // Build service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Get string localizer for class "ConsoleApp1.MyController".
                IStringLocalizer stringLocalizer 
                    = serviceProvider.GetService<IStringLocalizer<ConsoleApp1.MyController>>();

                // Narrow scope down to "en" culture
                IStringLocalizer stringLocalizerScoped = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("en"));

                // Get "Hello World!"
                string str = stringLocalizerScoped.GetString("Hello");
            }
            #endregion Snippet
        }
    }

}

```
***

# Links
* [Microsoft.Extensions.DependencyInjection.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/DependencyInjection/DI.Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/))
 * [IServiceCollection](https://github.com/aspnet/Extensions/blob/master/src/DependencyInjection/DI.Abstractions/src/IServiceCollection.cs)
* [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/Extensions/tree/master/src/DependencyInjection/DI/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/))
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [DependencyInjection](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/DependencyInjection.cs)
