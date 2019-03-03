# Localization Asset
**LocalizationAsset** is simple language string container. Asset is populated from different IEnumeration sources, which become effective when **Load()** is called.

**.AddKeySource(*IEnumerable&lt;KeyValuePair&lt;Key, string&gt;, string&gt;*)** add a source of language strings.

```csharp
// Create localization source
var source = new Dictionary<Key, string> {
    { (Key)ParameterNamePolicy.Instance.Parse("type:MyController:key:hello", Key.Root),            "Hello World!" },
    { (Key)ParameterNamePolicy.Instance.Parse("culture:en:type:MyController:key:hello", Key.Root), "Hello World!" },
    { (Key)ParameterNamePolicy.Instance.Parse("culture:de:type:MyController:key:hello", Key.Root), "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new LoadableLocalizationAsset().AddKeyLinesSource(source).Load();
```

Language strings can now be queried from the asset.

```csharp
IAssetKey key = new LocalizationRoot(asset).Type("MyController").Key("hello");
Console.WriteLine(key);
Console.WriteLine(key.Culture("en"));
Console.WriteLine(key.Culture("de"));
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
IAsset asset = new LoadableLocalizationAsset().AddKeyStringSource(source, "{culture:}{type:}{Key}").Load();
```
</details>

<details>
  <summary><b>.AddAssetKeySource()</b> adds language string source with IAssetKey based keys. (<u>Click here</u>)</summary>
These keys are converted to Key internally when <b>.Load()</b> is called.

```csharp
// Create localization source
var source = new Dictionary<IAssetKey, string> {
    { new LocalizationRoot().Type("MyController").Key("hello"),                  "Hello World!" },
    { new LocalizationRoot().Type("MyController").Key("hello").Culture("en"), "Hello World!" },
    { new LocalizationRoot().Type("MyController").Key("hello").Culture("de"), "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new LoadableLocalizationAsset().AddKeyLinesSource(source).Load();
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
foreach (Key _key in asset.GetAllKeys(LocalizationRoot.Global.Culture("de")))
    Console.WriteLine(_key);
```
