# Localization Reader
There are following file formats that are supported with the default class libraries.

| Format | Reader Class |
|:---------|:-------|
| .ini | LocalizationIniReader |
| .json | LocalizationJsonReader |
| .xml | LocalizationXmlReader |
| .resx | LocalizationResxReader |
| .resources | LocalizationResourcesReader |

**ILocalizationFileFormat** instance can be acquired from **LocalizationReaderMap** dictionary.

```csharp
ILocalizationFileFormat format = LocalizationReaderMap.Instance["ini"];
```

And from singleton instances.

```csharp
ILocalizationFileFormat format = LocalizationIniReader.Instance;
```

# Read IAsset
File can be read right away into an *IAsset* with **.FileAsset()** extension method.

```csharp
IAsset asset = LocalizationIniReader.Instance.FileAsset(
    filename: "localization.ini",
    throwIfNotFound: true);
```

From embedded resource with **.EmbeddedAsset()** method.

```csharp
Assembly asm = typeof(LocalizationReader_Examples).Assembly;
IAsset asset = LocalizationIniReader.Instance.EmbeddedAsset(
    assembly: asm,
    resourceName: "docs.localization.ini",
    throwIfNotFound: true);
```

And from a file provider with **.FileProviderAsset()**. 

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IAsset asset = LocalizationIniReader.Instance.FileProviderAsset(
    fileProvider: fileProvider,
    filepath: "localization.ini",
    throwIfNotFound: true);
```

The same extension methods are also available in the **LocalizationReaderMap**, which selects the reader class by file extension.

```csharp
IAsset asset = LocalizationReaderMap.Instance.FileAsset(
    filename: "localization.ini",
    throwIfNotFound: true);
```

# Read IAssetSource
File can be read into an *IAssetSource* with **.FileAsset()** extension method. *IAssetSource* is a reference and a loader of asset.
It is not read right away, but when the asset is built.

```csharp
IAssetSource assetSource = 
    LocalizationIniReader.Instance.FileAssetSource(
        filename: "localization.ini",
        throwIfNotFound: true);
IAssetBuilder assetBuilder = new AssetBuilder().AddSource(assetSource);
IAsset asset = assetBuilder.Build();
```

Reference to embedded resource source with **.EmbeddedAssetSource()**.

```csharp
Assembly asm = typeof(LocalizationReader_Examples).Assembly;
IAssetSource assetSource = 
    LocalizationIniReader.Instance.EmbeddedAssetSource(
        assembly: asm,
        resourceName: "docs.localization.ini",
        throwIfNotFound: true);
```

And file provider with **.FileProviderAssetSource()**.

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IAssetSource assetSource = 
    LocalizationIniReader.Instance.FileProviderAssetSource(
        fileProvider: fileProvider,
        filepath: "localization.ini",
        throwIfNotFound: true);
```

The same extension methods are also available in the **LocalizationReaderMap**, which selects the reader class by file extension.

```csharp
IAssetSource assetSource = LocalizationReaderMap.Instance.FileAssetSource(
    filename: "localization.ini", 
    throwIfNotFound: true);
```

# Read File
Different file formats have different intrinsic formats. 
* Context free list formats are handled with **IEnumerable&lt;KeyValuePair&lt;IAssetKey, string&gt;&gt;** class.
* Context dependent list formats are held in **IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;**.
* Context free tree files are held in **IKeyTree**.

Localization file can be read right away into key lines with **.ReadKeyLines()**.

```csharp
IEnumerable<KeyValuePair<IAssetKey, string>> key_lines = LocalizationReaderMap.Instance.ReadKeyLines(
    filename: "localization.ini", 
    throwIfNotFound: true);
```
Into three string lines with **.ReadStringLines()**.

```csharp
IEnumerable<KeyValuePair<string, string>> string_lines = LocalizationReaderMap.Instance.ReadStringLines(
    filename: "localization.ini", 
    namePolicy: ParameterNamePolicy.Instance,
    throwIfNotFound: true);
```
And into a tree **.ReadKeyTree()**.

```csharp
IKeyTree tree = LocalizationReaderMap.Instance.ReadKeyTree(
    filename: "localization.ini", 
    throwIfNotFound: true);
```

# File Reader
A file reader can be constructed with respective **.FileReaderAsKeyLines()**.
File reader reads the refered file when **.GetEnumerator()** is called, and will re-read the file again every time.

```csharp
IEnumerable<KeyValuePair<IAssetKey, string>> key_lines_reader = 
    LocalizationReaderMap.Instance.FileReaderAsKeyLines(
        filename: "localization.ini", 
        throwIfNotFound: true);
```
**.FileReaderAsStringLines()** creates a reader that returns string lines.

```csharp
IEnumerable<KeyValuePair<string, string>> string_lines_reader = 
    LocalizationReaderMap.Instance.FileReaderAsStringLines(
        filename: "localization.ini",
        namePolicy: ParameterNamePolicy.Instance,
        throwIfNotFound: true);
```
And **.FileReaderAsKeyTree()** a tree reader.

```csharp
IEnumerable<IKeyTree> tree_reader = 
    LocalizationReaderMap.Instance.FileReaderAsKeyTree(
        filename: "localization.ini", 
        throwIfNotFound: true);
```

# Embedded Reader
Embedded resource reader is created with **.EmbeddedReaderAsKeyLines()**.

```csharp
Assembly asm = typeof(LocalizationReader_Examples).Assembly;
IEnumerable<KeyValuePair<IAssetKey, string>> key_lines_reader = 
    LocalizationReaderMap.Instance.EmbeddedReaderAsKeyLines(
        assembly: asm, 
        resourceName: "docs.localization.ini", 
        throwIfNotFound: true);
```
**.EmbeddedReaderAsStringLines()** creates embedded reader of string lines.

```csharp
IEnumerable<KeyValuePair<string, string>> string_lines_reader = 
    LocalizationReaderMap.Instance.EmbeddedReaderAsStringLines(
        assembly: asm, 
        resourceName: "docs.localization.ini", 
        namePolicy: ParameterNamePolicy.Instance,
        throwIfNotFound: true);
```
And **.EmbeddedReaderAsKeyTree()** reader of trees

```csharp
IEnumerable<IKeyTree> tree_reader = 
    LocalizationReaderMap.Instance.EmbeddedReaderAsKeyTree(
        assembly: asm, 
        resourceName: "docs.localization.ini", 
        throwIfNotFound: true);
```

# IFileProvider Reader
File provider reader is created with **.FileProviderReaderAsKeyLines()**.

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IEnumerable<KeyValuePair<IAssetKey, string>> key_lines_reader = 
    LocalizationReaderMap.Instance.FileProviderReaderAsKeyLines(
        fileProvider: fileProvider, 
        filepath: "localization.ini", 
        throwIfNotFound: true);
```
**.FileProviderReaderAsStringLines()** creates string lines reader

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IEnumerable<KeyValuePair<string, string>> string_lines_reader = 
    LocalizationReaderMap.Instance.FileProviderReaderAsStringLines(
        fileProvider: fileProvider, 
        filepath: "localization.ini", 
        throwIfNotFound: true);
```
And **.FileProviderReaderAsKeyTree()** tree reader.

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IEnumerable<IKeyTree> tree_reader = 
    LocalizationReaderMap.Instance.FileProviderReaderAsKeyTree(
        fileProvider: fileProvider, 
        filepath: "localization.ini", 
        throwIfNotFound: true);
```

# Read Stream
Content can be read from **Stream** into key lines.

```csharp
using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
{
    IEnumerable<KeyValuePair<IAssetKey, string>> key_lines = LocalizationIniReader.Instance.ReadKeyLines(s);
}
```
Into string lines.

```csharp
using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
{
    IEnumerable<KeyValuePair<string, string>> string_lines = LocalizationIniReader.Instance.ReadStringLines(
        stream: s,
        namePolicy: ParameterNamePolicy.Instance);
}
```
And into a tree.

```csharp
using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
{
    IKeyTree tree = LocalizationIniReader.Instance.ReadKeyTree(s);
}
```

# Read TextReader
Content can be read from **TextReader** into key lines.

```csharp
string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
using (TextReader tr = new StringReader(text))
{
    IEnumerable<KeyValuePair<IAssetKey, string>> key_lines = LocalizationIniReader.Instance.ReadKeyLines(tr);
}
```
Into string lines.

```csharp
using (TextReader tr = new StringReader(text))
{
    IEnumerable<KeyValuePair<string, string>> string_lines = LocalizationIniReader.Instance.ReadStringLines(
        srcText: tr,
        namePolicy: ParameterNamePolicy.Instance);
}
```
And into tree.

```csharp
using (TextReader tr = new StringReader(text))
{
    IKeyTree tree = LocalizationIniReader.Instance.ReadKeyTree(tr);
}
```

# Read String
And from **String** into key lines.

```csharp
string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
IEnumerable<KeyValuePair<IAssetKey, string>> key_lines = 
    LocalizationIniReader.Instance.ReadStringAsKeyLines(
        srcText: text);
```
Into string lines.

```csharp
IEnumerable<KeyValuePair<string, string>> string_lines = 
    LocalizationIniReader.Instance.ReadStringAsStringLines(
        srcText: text,
        namePolicy: ParameterNamePolicy.Instance);
```
And into a tree.

```csharp
IKeyTree tree = 
    LocalizationIniReader.Instance.ReadStringAsKeyTree(
        srcText: text);
```

# Implementing
<details>
  <summary><b>ILocalizationReader</b> is the root interface for localization reader classes. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Signals that file format can be read localization files.
/// 
/// For reading capability, must implement one of:
/// <list type="Bullet">
/// <item><see cref="ILocalizationKeyLinesStreamReader"/></item>
/// <item><see cref="ILocalizationKeyTreeStreamReader"/></item>
/// <item><see cref="ILocalizationKeyLinesTextReader"/></item>
/// <item><see cref="ILocalizationKeyTreeTextReader"/></item>
/// <item><see cref="ILocalizationStringLinesTextReader"/></item>
/// <item><see cref="ILocalizationStringLinesStreamReader"/></item>
/// </list>
/// </summary>
public interface ILocalizationReader : ILocalizationFileFormat { }

/// <summary>
/// Reader that can read localization lines from a <see cref="Stream"/>.
/// </summary>
public interface ILocalizationKeyLinesStreamReader : ILocalizationReader
{
    /// <summary>
    /// Read <paramref name="stream"/> into lines.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="namePolicy">(optional) name policy. </param>
    /// <returns>the read lines</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(Stream stream, IAssetKeyNamePolicy namePolicy = default);
}

/// <summary>
/// Reader that can read localization into tree format format a <see cref="Stream"/>.
/// </summary>
public interface ILocalizationKeyTreeStreamReader : ILocalizationReader
{
    /// <summary>
    /// Read <paramref name="stream"/> into tree structuer.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="namePolicy">(optional) name policy.</param>
    /// <returns>lines in tree structure</returns>
    /// <exception cref="IOException"></exception>
    IKeyTree ReadKeyTree(Stream stream, IAssetKeyNamePolicy namePolicy = default);
}

/// <summary>
/// Reader that can read localization lines from a <see cref="TextReader"/>.
/// </summary>
public interface ILocalizationKeyLinesTextReader : ILocalizationReader
{
    /// <summary>
    /// Read <paramref name="text"/> into lines.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="namePolicy">(optional) name policy.</param>
    /// <returns>the read lines</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(TextReader text, IAssetKeyNamePolicy namePolicy = default);
}

/// <summary>
/// Reader that can read localization lines from a <see cref="TextReader"/>.
/// </summary>
public interface ILocalizationKeyTreeTextReader : ILocalizationReader
{
    /// <summary>
    /// Read <paramref name="text"/> into lines.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="namePolicy">(optional) name policy.</param>
    /// <returns>lines in tree structure</returns>
    /// <exception cref="IOException"></exception>
    IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default);
}

/// <summary>
/// Reader that can read string key-values from a <see cref="TextReader"/>.
/// </summary>
public interface ILocalizationStringLinesTextReader : ILocalizationReader
{
    /// <summary>
    /// Read <paramref name="text"/> into lines.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="namePolicy">(optional) name policy.</param>
    /// <returns>the read string key-values</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<KeyValuePair<string, string>> ReadStringLines(TextReader text, IAssetKeyNamePolicy namePolicy = default);
}

/// <summary>
/// Reader that can read string key-values from a <see cref="Stream"/>.
/// </summary>
public interface ILocalizationStringLinesStreamReader : ILocalizationReader
{
    /// <summary>
    /// Read <paramref name="stream"/> into lines.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="namePolicy">(optional) name policy.</param>
    /// <returns>the read string key-values</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<KeyValuePair<string, string>> ReadStringLines(Stream stream, IAssetKeyNamePolicy namePolicy = default);
}
```
</details>
<br/>
A class that implements **ILocalizationReader** must to implement one of its sub-interfaces. A one that best suits the underlying format.

```csharp
class ExtFileFormatReader : ILocalizationKeyLinesTextReader
{
    public string Extension => "ext";

    public IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(
        TextReader text, 
        IAssetKeyNamePolicy namePolicy = null)
    {
        IAssetKey key = Key.Create("Section", "MyClass").Append("Key", "HelloWorld").Append("Culture", "en");
        yield return new KeyValuePair<IAssetKey, string>(key, "Hello World!");
    }
}
```

Reader can be added to **LocalizationReaderMap**.

```csharp
// Create writer
ILocalizationReader format = new ExtFileFormatReader();

// Clone formats
LocalizationFileFormatMap formats = LocalizationReaderMap.Instance.Clone();
// Add to clone
formats.Add(format);

// Or if in deploying application project, format can be added to the global singleton
(LocalizationReaderMap.Instance as IDictionary<string, ILocalizationFileFormat>).Add(format);
```
