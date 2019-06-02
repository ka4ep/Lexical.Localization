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
    { new LineRoot().Type("MyController").Key("hello"),               "Hello World!" },
    { new LineRoot().Type("MyController").Key("hello").Culture("en"), "Hello World!" },
    { new LineRoot().Type("MyController").Key("hello").Culture("de"), "Hallo Welt!"  }
};
// Create asset with string source
IAsset asset = new StringAsset().Add(source).Load();
```
<br/>

Keys can be enumerated with **GetAllKeys()**. 

```csharp
// Extract all keys
foreach (var _key in asset.GetStringLines(null))
    Console.WriteLine(_key);
```
<br/>

The query can be filtered with a criteria key. It returns only keys that have equal parameters as the criteria key.

```csharp
var source = new List<ILine> {
    LineAppender.Default.Type("MyController").Key("hello").Format("Hello World!"),
    LineAppender.Default.Type("MyController").Key("hello").Culture("en").Format("Hello World!"),
    LineAppender.Default.Type("MyController").Key("hello").Culture("de").Format("Hallo Welt!")
};
// Keys can be filtered
ILine filterKey = LineAppender.Default.Culture("de");
IAsset asset = new StringAsset().Add(source, "{Culture:}[Type:][Key]").Load();
foreach (var _key in asset.GetLines(filterKey))
    Console.WriteLine(_key.Print(LineFormat.ParametersInclString));
```
