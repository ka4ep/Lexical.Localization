# Localization Asset
**StringAsset** is simple language string container. Asset is populated from different IEnumeration sources, which become effective when **Load()** is called.

<b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;, IAssetNamePolicy keyPolicy</i>)</b> adds language string source with String based keys.

```csharp
// Create localization source
var source = new Dictionary<string, string> {
    { "MyController:hello", "Hello World!"    },
    { "en:MyController:hello", "Hello World!" },
    { "de:MyController:hello", "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new StringAsset().Add(source, "{Culture:}[Type:][Key]").Load();
```
<br/>

<b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;ILine, string&gt;&gt;</i>, IAssetNamePolicy keyPolicy)</b> adds language string source with ILine based keys.

```csharp
// Create localization source
var source = new Dictionary<ILine, string> {
    { new LocalizationRoot().Type("MyController").Key("hello"),               "Hello World!" },
    { new LocalizationRoot().Type("MyController").Key("hello").Culture("en"), "Hello World!" },
    { new LocalizationRoot().Type("MyController").Key("hello").Culture("de"), "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new StringAsset().Add(source).Load();
```
<br/>

Keys can be enumerated with **GetAllKeys()**. 

```csharp
// Extract all keys
foreach (ILine _key in asset.GetKeyLines(null).Select(line=>line.Key))
    Console.WriteLine(_key);
```
<br/>

The query can be filtered with a criteria key. It returns only keys that have equal parameters as the criteria key.

```csharp
// Keys can be filtered
ILine filterKey = LocalizationRoot.Global.Culture("de");
foreach (ILine _key in asset.GetKeyLines(filterKey).Select(line => line.Key))
    Console.WriteLine(_key);
```
