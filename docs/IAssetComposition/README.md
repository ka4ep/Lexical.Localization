# Composing assets

**AssetComposition** is the default class. It unifies a group of assets into one asset, typically so that they can be assigned to IAssetRoot.

```csharp
// Create individual assets
IAsset asset_1 = new LocalizationStringDictionary(new Dictionary<string, string> { { "en:hello", "Hello World!" } });
IAsset asset_2 = new AssetResourceDictionary(new Dictionary<string, byte[]> { { "en:Hello.Icon", new byte[] { 1, 2, 3 } } });
IAsset asset_3 = new AssetLoader().Add( new AssetLoaderPartEmbeddedStrings("[assembly.]localization{-culture}.ini", AssetKeyNameProvider.Default ).AddAssembly(Assembly.GetExecutingAssembly()).AddMatchParameters("assembly"));

// Create composition asset
IAssetComposition asset_composition = new AssetComposition(asset_1, asset_2, asset_3);

// Assign the composition to root
IAssetRoot root = new LocalizationRoot(asset_composition, new CulturePolicy());
```

<details>
  <summary><b>IAssetComposition</b> is the interface for classes that composes IAsset components. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Composition of <see cref="IAsset"/> components.
/// </summary>
public interface IAssetComposition : IAsset, IList<IAsset>
{
    /// <summary>
    /// Set to new content.
    /// </summary>
    /// <param name="newContent"></param>
    /// <exception cref="InvalidOperationException">If compostion is readonly</exception>
    void CopyFrom(IEnumerable<IAsset> newContent);

    /// <summary>
    /// Get component assets that implement T. 
    /// </summary>
    /// <param name="recursive">if true, visits children recursively</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>enumerable or null</returns>
    IEnumerable<T> GetComponents<T>(bool recursive) where T : IAsset;
}
```
</details>


# Links
* [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetComposition)
* [Lexical.Asset.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Asset.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Asset.Abstractions/))
 * [IAssetComposition](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetComposition.cs)
* [Lexical.Asset](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Asset) ([NuGet](https://www.nuget.org/packages/Lexical.Asset/))
 * [AssetComposition](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Asset/Asset/AssetComposition.cs)


