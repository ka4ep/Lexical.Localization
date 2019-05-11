# Introduction
**StringLocalizerFactoryAsset** adapts <b><i>IStringLocalizerFactory</i></b> implementations to *IAsset*.

```csharp
// Create IStringLocalizerFactory
LoggerFactory loggerFactory = new LoggerFactory();
loggerFactory.AddConsole(LogLevel.Trace);
IOptions<LocalizationOptions> options = Options.Create(new LocalizationOptions { ResourcesPath = "" });
IStringLocalizerFactory stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);

// Adapt IStringLocalizerFactory to IAsset
IAsset asset = new StringLocalizerFactoryAsset(stringLocalizerFactory);

// Create root
ILineRoot root = new LocalizationRoot(asset, new CulturePolicy());

// There are .resx files in "Resources/ConsoleApp1.MyController" with keys "Success" and "Error"
ILine key = root
    .Assembly(Assembly.GetExecutingAssembly())
    .Resource("ConsoleApp1.MyController")
    //.Type(typeof(ConsoleApp1.MyController1))
    .Key("Success")
    .Culture("sv");

// Retrieve string from IStringLocalizerFactory
string str = key.ToString();
```

There is an extension method **.ToAsset()** that makes the same conversion.

```csharp
// Adapt IStringLocalizerFactory to IAsset
asset = stringLocalizerFactory.ToAsset();
```

**StringLocalizerAsset** adapts <b><i>IStringLocalizer</i></b> implementations to *IAsset*.

```csharp
// Adapt IStringLocalizer to IAsset
asset = new StringLocalizerAsset(stringLocalizer);
```

**StringLocalizerRoot** adapts *IAsset* implementations back to <b><i>IStringLocalizerFactory</i></b>. [Read more.](../ILine/ILineRoot/#string-localizer)

```csharp
// Create asset
var source = new Dictionary<string, string> { { "fi:ConsoleApp1.MyController:Success", "Onnistui" } };
IAsset asset = new LocalizationAsset(source, LineParameterPrinter.Default);

// Create root
ILine root = new StringLocalizerRoot(asset, new CulturePolicy());

// Type cast to IStringLocalizerFactory
IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;

// Get IStringLocalizer
IStringLocalizer stringLocalizer = stringLocalizerFactory.Create(typeof(ConsoleApp1.MyController));

// Assign culture
stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("fi"));

// Get string
string str = stringLocalizer["Success"];
```

And to <b><i>IStringLocalizer</i></b>.

```csharp
// Create asset
var source = new Dictionary<string, string> { { "fi:ConsoleApp1.MyController:Success", "Onnistui" } };
IAsset asset = new LocalizationAsset(source, LineParameterPrinter.Default);

// Create root
ILine root = new StringLocalizerRoot(asset, new CulturePolicy());

// Set key
ILine key = root.Culture("fi").Type(typeof(ConsoleApp1.MyController));

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
 * [StringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/StringLocalizerAsset.cs)
 * [StringLocalizerRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/StringLocalizerRoot.cs)
 * [StringLocalizerFactoryAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/StringLocalizerFactoryAsset.cs)
 * [ResourceManagerStringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/ResourceManagerStringLocalizerAsset.cs)
 * [ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/ResourceManagerStringLocalizerAssetSource.cs)
