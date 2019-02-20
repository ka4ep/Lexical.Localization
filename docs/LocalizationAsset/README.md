# Localization Asset
**LocalizationAsset** is simple language string container. Asset is populated from different IEnumeration sources, which become effective content is built with **Load()** method.

**.AddStringSource(*IEnumerable&lt;KeyValuePair&lt;string, string &gt;, string&gt;*)** adds language strings with strings as keys.

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

**.AddKeySource(*IEnumerable&lt;KeyValuePair&lt;Key, string &gt;, string&gt;*)** adds language strings with context-free keys.

```csharp
// Create localization source
var source = new Dictionary<Key, string> {
    { new Key("type", "MyController").Append("key", "hello"),                         "Hello World!" },
    { new Key("type", "MyController").Append("key", "hello").Append("culture", "en"), "Hello World!" },
    { new Key("type", "MyController").Append("key", "hello").Append("culture", "de"), "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new LocalizationAsset().AddKeySource(source).Load();
```

**.AddAssetKeySource(*IEnumerable&lt;KeyValuePair&lt;IAssetKey, string &gt;, string&gt;*)** adds language strings with IAssetKeys as keys. These keys are converted to Key internally when **.Load()** is called.

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
