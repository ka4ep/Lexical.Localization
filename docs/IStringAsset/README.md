# Localization Asset
**StringAsset** is language string container. Asset is populated from different IEnumeration sources, which become effective when **Load()** is called.
<br/>

The default way is to add a reader <i>IEnumerable&lt;ILine&gt;</i>.

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
<br/>
<br/>

Some source have keys as *strings* that cannot be parsed into a structured key. 
These assets can be added as <b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;, ILineFormat</i>)</b>.

An instance of **ILineFormatPrinter** must be provided to convert requesting keys to strings that can used to match the key in the source asset.
These values use **CSharpFormat** string format.

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
<br/>

*StringAsset* can import key-string pairs as well. 
<b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;ILine, string&gt;&gt;</i>, ILineFormat keyPolicy)</b> adds language string source with ILine based keys.

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
<br/>

Keys can be enumerated with **GetAllKeys()**. 

```csharp
// Extract all keys
foreach (var _key in asset.GetStringLines(null))
    Console.WriteLine(_key);
```

