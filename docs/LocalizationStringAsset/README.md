# Localization String Dictionary
**LocalizationAsset** is simple language string container. It uses Dictionary&lt;string, string&gt; as a source.

When a language string is requested the requested IAssetKey is converted to string to be matched with the dictionary. 
[IAssetKeyNamePolicy](../IAssetKeyNamePolicy/index.html) is used for making the conversion.

```csharp
// Create localization source
var source = new Dictionary<string, string> {
    { "MyController:hello", "Hello World!" },
    { "en:MyController:hello", "Hello World!" },
    { "de:MyController:hello", "Hallo Welt!" }
};
// Create asset
IAsset asset = new LocalizationStringAsset(source, AssetKeyNameProvider.Default);
```

If the provided key name policy is *AssetKeyNameProvider.Default*, then identity string is constructed by concatenating parameters from requesting key in the order of appearance. Non-canonical parameter "Culture" is, however, appended first.

```csharp
// Create root 
IAssetRoot root = new LocalizationRoot(asset);
// Create key. Name policy converts to .
IAssetKey key = root.Section("MyController").Key("hello").SetCulture("de");
// Test what identity is produced, "de:MyController:hello"
string id = AssetKeyNameProvider.Default.BuildName(key);
// Query language string
string localizedString = key.ToString();
```

If LocalizationStringAsset was constructed with a [IAssetNamePattern](../IAssetKeyNamePolicy/index.html#asset-name-pattern) (or string), then parameters from IAssetKey substituted to the parameter names in the pattern.

```csharp
// Create asset with name pattern
IAsset asset = new LocalizationStringAsset(source, "{culture:}{type:}{Key}");
```

Also keys can be enumerated from the asset.

```csharp
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
