# Asset Composition

**AssetComposition** is the default class. It unifies a group of assets into one asset, typically so that they can be assigned to ILineRoot.

```csharp
// Create individual assets
IAsset asset_1 = new LocalizationAsset(new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } }, ParameterParser.Instance);
IAsset asset_2 = new ResourceStringDictionary(new Dictionary<string, byte[]> { { "Culture:en:Key:Hello.Icon", new byte[] { 1, 2, 3 } } }, ParameterParser.Instance);

// Create composition asset
IAssetComposition asset_composition = new AssetComposition(asset_1, asset_2);

// Assign the composition to root
ILineRoot root = new LocalizationRoot(asset_composition, new CulturePolicy());
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
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetComposition](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetComposition.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetComposition](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetComposition.cs)


