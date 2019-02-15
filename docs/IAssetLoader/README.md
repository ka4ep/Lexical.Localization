# Introduction
**AssetLoader** is a class that loads assets on-need-basis. 
It must be populated with parts, either with [part builder helper](PartBuilder/index.md) or by [constructing from classes](PartClasses/index.md). 


```csharp
// Create asset loader
IAssetLoader assetLoader = new AssetLoader();

// Create part(s)
IAssetLoaderPart[] parts = new AssetLoaderPartBuilder()
    .Path(".")                                                  // Add root directory to search files from
    .FilePattern("Assets/localization{-culture}.ini")           // Add file name pattern
    .KeyPattern("{anysection:}[key]")                           // Add key pattern to match within files
    .Strings()                                                  // Make this part return strings
    .Build().ToArray();                                         // Build part(s)

// Add part(s)
assetLoader.AddRange(parts);

// Create key
IAssetKey key = new LocalizationRoot(assetLoader).Section("controller").Key("ok").SetCulture("de");

// Asset loader loads filename "Assets/localization-de.ini" and then searches for key "controller:ok".
string str = key.ToString();
```

<p/>
<details>
  <summary><b>IAssetProvider</b> is interface that is intended for consumers. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Loads assets based on parameters in keys.
/// </summary>
public interface IAssetProvider : IAsset
{
    /// <summary>
    /// Load assets that match the criteria of the parameters in <paramref name="key"/>.
    /// 
    /// If key doesn't have some of the required parameters, the <see cref="IAssetProvider"/> 
    /// may match against all detected filenames.
    /// 
    /// The parameters that are matched is specific to the implementation. 
    /// For example, <see cref="IAssetLoader"/> implementation matches based 
    /// on the options of it's parts (<see cref="IAssetLoaderPart.Options"/>).
    /// 
    /// <see cref="IAssetLoaderPartOptions.MatchParameters"/> is a list of parameters to 
    /// match against detected filenames.
    /// </summary>
    /// <param name="key">key as criteria, or null for no criteria</param>
    /// <returns>assets or null</returns>
    /// <exception cref="AssetException">If loading failed</exception>
    IEnumerable<IAsset> LoadAssets(IAssetKey key);

    /// <summary>
    /// Load assets that match the criteria of the parameters in <paramref name="key"/>.
    /// 
    /// If a required parameter is missing from key, the it is matched against all 
    /// detected filenames regardless of any implementation specific options.
    /// </summary>
    /// <param name="key">key as criteria, or null for no criteria</param>
    /// <returns>assets or null</returns>
    /// <exception cref="AssetException">If loading failed</exception>
    IEnumerable<IAsset> LoadAllAssets(IAssetKey key);
}
```
</details>
<details>
  <summary><b>IAssetLoader</b> is interface for the initializers. Loader parts are added here. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Loads assets based on parameters in keys, and is configurable with <see cref="IAssetLoaderPart"/>.
/// </summary>
public interface IAssetLoader : IAssetProvider
{
    /// <summary>
    /// List of loader parts.
    /// </summary>
    IAssetLoaderPart[] LoaderParts { get; }

    /// <summary>
    /// Add new loader function.
    /// </summary>
    /// <param name="part">Object that loads assets based on the parameters, such as "culture"</param>
    /// <exception cref="ArgumentException">If there was a problem parsing the filename pattern</exception>
    /// <returns>this</returns>
    IAssetLoader Add(IAssetLoaderPart part);

    /// <summary>
    /// Add loader functions.
    /// </summary>
    /// <param name="part">(optional)list of loaders</param>
    /// <exception cref="ArgumentException">If there was a problem parsing the filename pattern</exception>
    /// <returns>this</returns>
    IAssetLoader AddRange(IEnumerable<IAssetLoaderPart> part);
}
```
</details>
<details>
  <summary><b>IAssetLoaderPart</b> is the interface for classes that participate in the asset loading process. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Interface for objects that load assets from IAssetLoader depending on parameters of a <see cref="IAssetNamePattern"/>.
/// This interface is used with <see cref="LocalizationAssetLoader"/>.
/// 
/// For example, localization files are separated by culture, then file pattern could be "localization{-culture}.ini".
/// Then this loader can load different files depending on culture value.
/// </summary>
public interface IAssetLoaderPart
{
    /// <summary>
    /// Filename pattern of this loader. For example "Resources/localization{-culture}.ini".
    /// </summary>
    IAssetNamePattern Pattern { get; }

    /// <summary>
    /// Options of this loader.
    /// </summary>
    IAssetLoaderPartOptions Options { get; set; }

    /// <summary>
    /// Load an asset file. 
    /// 
    /// <paramref name="parameters"/> is a list of arguments are used for constructing filename.
    /// Parameters match the capture parts of the associated <see cref="Pattern"/> property.
    /// 
    /// If Options.MatchParameters has parameters, this method does not try to match existing files. 
    /// Instead, the caller must find suitable matches with ListLoadables.
    /// 
    /// The callee musn't take ownership of <paramref name="parameters"/>, as the caller modify the contents.
    /// </summary>
    /// <param name="parameters">Parameters that are extracted from filename using the pattern</param>
    /// <returns>loaded asset, or null if file was not found</returns>
    /// <exception cref="Exception">on problem loading asset</exception>
    IAsset Load(IReadOnlyDictionary<string, string> parameters);

    /// <summary>
    /// Get a list loadable assets of in parametrized format.
    /// Parameters correspond to capture parts of the associated <see cref="Pattern"/> property.
    /// </summary>
    /// <returns>loadables</returns>
    /// <param name="parameters"></param>
    /// <exception cref="Exception">on problem enumerating files</exception>
    IEnumerable<IReadOnlyDictionary<string, string>> ListLoadables(IReadOnlyDictionary<string, string> parameters = null);
}
```
</details>
<details>
  <summary><b>IAssetLoaderPartOptions</b> is key-value dictionary interface for options. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Options of <see cref="IAssetLoaderPart"/> instance.
/// </summary>
public interface IAssetLoaderPartOptions : IDictionary<string, object>
{
}
```
</details>

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetLoader](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoader.cs)
 * [IAssetLoaderPart](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoaderPart.cs)
 * [IAssetLoaderPartOptions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoaderPartOptions.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetLoader](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetLoader/AssetLoader.cs) is the default implementation of IAssetLoader.
