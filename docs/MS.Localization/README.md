# Ms.Localization
Adapter classes create 2-way interoperability between
Microsoft.Extensions.Localization.Abstractions and Lexical.Localization.Abstractions.

## Ms.Localization to Lexical.Localization
**StringLocalizerFactoryAsset** adapts *IStringLocalizerFactory* implementations to *IAsset*.

```csharp
// Create IStringLocalizerFactory
LoggerFactory loggerFactory = new LoggerFactory();
loggerFactory.AddConsole(LogLevel.Trace);
IOptions<LocalizationOptions> options = Options.Create(new LocalizationOptions { ResourcesPath = "" });
IStringLocalizerFactory stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);

// Adapt IStringLocalizerFactory to IAsset
IAsset asset = new StringLocalizerFactoryAsset(stringLocalizerFactory);

// Create root
IAssetRoot root = new LocalizationRoot(asset, new CulturePolicy());

// There are .resx files in "Resources/ConsoleApp1.MyController" with keys "Success" and "Error"
IAssetKey key = root
    .AssemblySection(Assembly.GetExecutingAssembly())
    .ResourceSection("ConsoleApp1.MyController")
    //.TypeSection(typeof(ConsoleApp1.MyController1))
    .Key("Success")
    .SetCulture("sv");

// Retrieve string from IStringLocalizerFactory
string str = key.ToString();
```

There is an extension method **.ToAsset()** that makes the same conversion.

```csharp
// Adapt IStringLocalizerFactory to IAsset
asset = stringLocalizerFactory.ToAsset();
```

And **StringLocalizerAsset** adapts *IStringLocalizer* implementations to *IAsset*.

```csharp
// Adapt IStringLocalizer to IAsset
asset = new StringLocalizerAsset(stringLocalizer);
```

## Lexical.Localization to Ms.Localization
**StringLocalizerRoot** adapts *IAsset* implementations to *IStringLocalizerFactory*. [Read more.](../IAssetRoot/#string-localizer)

```csharp
// Create asset
var source = new Dictionary<string, string> { { "fi:ConsoleApp1.MyController:Success", "Onnistui" } };
IAsset asset = new LocalizationStringDictionary(source, AssetKeyNameProvider.Default);

// Create root
IAssetKey root = new StringLocalizerRoot(asset, new CulturePolicy());

// Type cast to IStringLocalizerFactory
IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;

// Get IStringLocalizer
IStringLocalizer stringLocalizer = stringLocalizerFactory.Create(typeof(ConsoleApp1.MyController));

// Assign culture
stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("fi"));

// Get string
string str = stringLocalizer["Success"];
```

And to *IStringLocalizer*.

```csharp
// Create asset
var source = new Dictionary<string, string> { { "fi:ConsoleApp1.MyController:Success", "Onnistui" } };
IAsset asset = new LocalizationStringDictionary(source, AssetKeyNameProvider.Default);

// Create root
IAssetKey root = new StringLocalizerRoot(asset, new CulturePolicy());

// Set key
IAssetKey key = root.SetCulture("fi").TypeSection(typeof(ConsoleApp1.MyController));

// Type cast key to IStringLocalizer
IStringLocalizer stringLocalizer = key as IStringLocalizer;

// Get string
string str = stringLocalizer["Success"];
```


# Links
* [Microsoft.Extensions.Localization.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/Localization/Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization.Abstractions/))
 * [IStringLocalizer](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizer.cs) 
 * [IStringLocalizer&lt;T&gt;](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerOfT.cs)
 * [IStringLocalizerFactory](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerFactory.cs)
* [Microsoft.Extensions.Localization](https://github.com/aspnet/Localization/tree/master/src/Microsoft.Extensions.Localization) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization/))
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [StringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/StringLocalizerAsset.cs)
 * [StringLocalizerRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/StringLocalizerRoot.cs)
 * [StringLocalizerFactoryAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/StringLocalizerFactoryAsset.cs)
 * [ResourceManagerStringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/ResourceManagerStringLocalizerAsset.cs)
 * [ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/ResourceManagerStringLocalizerAssetSource.cs)
