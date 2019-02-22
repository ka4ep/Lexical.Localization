# Localization Asset
**LocalizationAsset** is simple language string container. Asset is populated from different IEnumeration sources, which become effective when **Load()** is called.

**.AddKeySource(*IEnumerable&lt;KeyValuePair&lt;Key, string&gt;, string&gt;*)** add a source of language strings.

```csharp
// Create localization source
var source = new Dictionary<Key, string> {
    { Key.NamePolicy.Instance.ParseKey("type:MyController:key:hello"),            "Hello World!" },
    { Key.NamePolicy.Instance.ParseKey("culture:en:type:MyController:key:hello"), "Hello World!" },
    { Key.NamePolicy.Instance.ParseKey("culture:de:type:MyController:key:hello"), "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new LocalizationAsset().AddKeySource(source).Load();
```

Language strings can now be queried from the asset.

```csharp
IAssetKey key = new LocalizationRoot(asset).TypeSection("MyController").Key("hello");
Console.WriteLine(key);
Console.WriteLine(key.SetCulture("en"));
Console.WriteLine(key.SetCulture("de"));
```

<details>
  <summary><b>.AddStringSource()</b> adds language string source with String based keys. (<u>Click here</u>)</summary>
These keys are converted to Key internally when <b>.Load()</b> is called.

```csharp
// Create localization source
var source = new Dictionary<string, string> {
    { "MyController:hello", "Hello World!"    },
    { "en:MyController:hello", "Hello World!" },
    { "de:MyController:hello", "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new LocalizationAsset().AddStringSource(source, "{culture:}{type:}{key}").Load();
```
</details>

<details>
  <summary><b>.AddAssetKeySource()</b> adds language string source with IAssetKey based keys. (<u>Click here</u>)</summary>
These keys are converted to Key internally when <b>.Load()</b> is called.

```csharp
// Create localization source
var source = new Dictionary<IAssetKey, string> {
    { new LocalizationRoot().TypeSection("MyController").Key("hello"),                  "Hello World!" },
    { new LocalizationRoot().TypeSection("MyController").Key("hello").SetCulture("en"), "Hello World!" },
    { new LocalizationRoot().TypeSection("MyController").Key("hello").SetCulture("de"), "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new LocalizationAsset().AddAssetKeySource(source).Load();
```
</details>
<br/>

Keys can be enumerated with **GetAllKeys()**. 

```csharp
// Extract all keys
foreach (Key _key in asset.GetAllKeys())
    Console.WriteLine(_key);
```

The query can be filtered with a criteria key. It returns only keys that have equal parameters as the criteria key.

```csharp
// Keys can be filtered
foreach (Key _key in asset.GetAllKeys(LocalizationRoot.Global.SetCulture("de")))
    Console.WriteLine(_key);
```
