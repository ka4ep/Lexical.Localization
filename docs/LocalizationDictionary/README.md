# Localization String Dictionary
**LocalizationDictionary** is the simplest asset. It uses a dictionary as a source.

```csharp
// Create localization source
var source = new Dictionary<string, string> {
    { "MyController:hello", "Hello World!" },
    { "en:MyController:hello", "Hello World!" },
    { "de:MyController:hello", "Hallo Welt!" }
};
// Create asset
IAsset asset = new LocalizationDictionary(source);
```

If LocalizationDictionary was constructed with a [name pattern](../IAssetKeyNamePolicy/index.html#asset-name-pattern) (IAssetNamePattern or string) then keys can be enumerated from it.

```csharp
// Create asset with name pattern
IAsset asset = new LocalizationDictionary(source, "{culture:}{type:}{key}");
// Extract all keys
foreach (IAssetKey key in asset.GetAllKeys())
    Console.WriteLine(key);
```

**GetAllKeys()** can be filtered with a criteria key.

```csharp
// Keys can be filtered
foreach (IAssetKey key in asset.GetAllKeys(LocalizationRoot.Global.SetCulture("de")))
    Console.WriteLine(key);
```
