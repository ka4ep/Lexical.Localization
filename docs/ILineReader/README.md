# Localization Reader
There are following file formats that are supported with the default class libraries.

| Format | Reader Class |
|:---------|:-------|
| .ini | IniLinesWriter |
| .json | JsonLinesReader |
| .xml | LocalizationXmlReader |
| .resx | LocalizationResxReader |
| .resources | ResourcesLineReader |

**ILineFileFormat** instance can be acquired from **LineReaderMap** dictionary.

```csharp
ILineFileFormat format = LineReaderMap.Default["ini"];
```

And from singleton instances.

```csharp
ILineFileFormat format = IniLinesReader.Default;
```

# Read IAsset
File can be read right away into an *IAsset* with **.FileAsset()** extension method.

```csharp
IAsset asset = IniLinesReader.Default.FileAsset(
    filename: "localization.ini",
    throwIfNotFound: true);
```

From embedded resource with **.EmbeddedAsset()** method.

```csharp
Assembly asm = typeof(LocalizationReader_Examples).Assembly;
IAsset asset = IniLinesReader.Default.EmbeddedAsset(
    assembly: asm,
    resourceName: "docs.localization.ini",
    throwIfNotFound: true);
```

And from a file provider with **.FileProviderAsset()**. 

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IAsset asset = IniLinesReader.Default.FileProviderAsset(
    fileProvider: fileProvider,
    filepath: "localization.ini",
    throwIfNotFound: true);
```

The same extension methods are also available in the **LineReaderMap**, which selects the reader class by file extension.

```csharp
IAsset asset = LineReaderMap.Default.FileAsset(
    filename: "localization.ini",
    throwIfNotFound: true);
```

# Read IAssetSource
File can be read into an *IAssetSource* with **.FileAsset()** extension method. *IAssetSource* is a reference and a loader of asset.
It is not read right away, but when the asset is built.

```csharp
IAssetSource assetSource = 
    IniLinesReader.Default.FileAssetSource(
        filename: "localization.ini",
        throwIfNotFound: true);
IAssetBuilder assetBuilder = new AssetBuilder().AddSource(assetSource);
IAsset asset = assetBuilder.Build();
```

Reference to embedded resource source with **.EmbeddedAssetSource()**.

```csharp
Assembly asm = typeof(LocalizationReader_Examples).Assembly;
IAssetSource assetSource = 
    IniLinesReader.Default.EmbeddedAssetSource(
        assembly: asm,
        resourceName: "docs.localization.ini",
        throwIfNotFound: true);
```

And file provider with **.FileProviderAssetSource()**.

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IAssetSource assetSource = 
    IniLinesReader.Default.FileProviderAssetSource(
        fileProvider: fileProvider,
        filepath: "localization.ini",
        throwIfNotFound: true);
```

The same extension methods are also available in the **LineReaderMap**, which selects the reader class by file extension.

```csharp
IAssetSource assetSource = LineReaderMap.Default.FileAssetSource(
    filename: "localization.ini", 
    throwIfNotFound: true);
```

# Read File
Different file formats have different intrinsic formats. 
* Context free list formats are handled with **IEnumerable&lt;KeyValuePair&lt;ILine, string&gt;&gt;** class.
* Context dependent list formats are held in **IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;**.
* Structural file formats with context free keys are held in **ILineTree**.

Localization file can be read right away into key lines with **.ReadKeyLines()**.

```csharp
IEnumerable<ILine> key_lines = LineReaderMap.Default.ReadLines(
    filename: "localization.ini", 
    throwIfNotFound: true);
```
Into string lines with **.ReadStringLines()**.

```csharp
IEnumerable<KeyValuePair<string, IString>> string_lines = LineReaderMap.Default.ReadStringLines(
    filename: "localization.ini", 
    lineFormat: LineFormat.Parameters,
    throwIfNotFound: true);
```
And into a tree **.ReadLineTree()**.

```csharp
ILineTree tree = LineReaderMap.Default.ReadLineTree(
    filename: "localization.ini", 
    throwIfNotFound: true);
```

# File Reader
A file reader can be constructed with respective **.FileReaderAsKeyLines()**.
File reader reads the refered file when **.GetEnumerator()** is called, and will re-read the file again every time.

```csharp
IEnumerable<ILine> key_lines_reader = 
    LineReaderMap.Default.FileReader(
        filename: "localization.ini", 
        throwIfNotFound: true);
```
**.FileReaderAsStringLines()** creates a reader that returns string lines.

```csharp
IEnumerable<KeyValuePair<string, IString>> string_lines_reader = 
    LineReaderMap.Default.FileReaderAsStringLines(
        filename: "localization.ini",
        lineFormat: LineFormat.Parameters,
        throwIfNotFound: true);
```
And **.FileReaderAsLineTree()** a tree reader.

```csharp
IEnumerable<ILineTree> tree_reader = 
    LineReaderMap.Default.FileReaderAsLineTree(
        filename: "localization.ini", 
        throwIfNotFound: true);
```

# Embedded Reader
Embedded resource reader is created with **.EmbeddedReaderAsKeyLines()**.

```csharp
Assembly asm = typeof(LocalizationReader_Examples).Assembly;
IEnumerable<ILine> key_lines_reader = 
    LineReaderMap.Default.EmbeddedReader(
        assembly: asm, 
        resourceName: "docs.localization.ini", 
        throwIfNotFound: true);
```
**.EmbeddedReaderAsStringLines()** creates embedded reader of string lines.

```csharp
IEnumerable<KeyValuePair<string, IString>> string_lines_reader = 
    LineReaderMap.Default.EmbeddedReaderAsStringLines(
        assembly: asm, 
        resourceName: "docs.localization.ini", 
        lineFormat: LineFormat.Parameters,
        throwIfNotFound: true);
```
And **.EmbeddedReaderAsLineTree()** reader of trees

```csharp
IEnumerable<ILineTree> tree_reader = 
    LineReaderMap.Default.EmbeddedReaderAsLineTree(
        assembly: asm, 
        resourceName: "docs.localization.ini", 
        throwIfNotFound: true);
```

# IFileProvider Reader
File provider reader is created with **.FileProviderReaderAsKeyLines()**.

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IEnumerable<ILine> key_lines_reader = 
    LineReaderMap.Default.FileProviderReader(
        fileProvider: fileProvider, 
        filepath: "localization.ini", 
        throwIfNotFound: true);
```
**.FileProviderReaderAsStringLines()** creates string lines reader

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IEnumerable<KeyValuePair<string, IString>> string_lines_reader = 
    LineReaderMap.Default.FileProviderReaderAsStringLines(
        fileProvider: fileProvider, 
        filepath: "localization.ini", 
        throwIfNotFound: true);
```
And **.FileProviderReaderAsLineTree()** tree reader.

```csharp
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
IEnumerable<ILineTree> tree_reader = 
    LineReaderMap.Default.FileProviderReaderAsLineTree(
        fileProvider: fileProvider, 
        filepath: "localization.ini", 
        throwIfNotFound: true);
```

# Read Stream
Content can be read from **Stream** into key lines.

```csharp
using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
{
    IEnumerable<ILine> key_lines = IniLinesReader.Default.ReadLines(s);
}
```
Into string lines.

```csharp
using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
{
    IEnumerable<KeyValuePair<string, IString>> string_lines = IniLinesReader.Default.ReadStringLines(
        stream: s,
        lineFormat: LineFormat.Parameters);
}
```
And into a tree.

```csharp
using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
{
    ILineTree tree = IniLinesReader.Default.ReadLineTree(s);
}
```

# Read TextReader
Content can be read from **TextReader** into key lines.

```csharp
string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
using (TextReader tr = new StringReader(text))
{
    IEnumerable<ILine> key_lines = IniLinesReader.Default.ReadLines(tr);
}
```
Into string lines.

```csharp
using (TextReader tr = new StringReader(text))
{
    IEnumerable<KeyValuePair<string, IString>> string_lines = IniLinesReader.Default.ReadStringLines(
        srcText: tr,
        lineFormat: LineFormat.Parameters);
}
```
And into tree.

```csharp
using (TextReader tr = new StringReader(text))
{
    ILineTree tree = IniLinesReader.Default.ReadLineTree(tr);
}
```

# Read String
And from **String** into key lines.

```csharp
string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
IEnumerable<ILine> key_lines = 
    IniLinesReader.Default.ReadString(
        srcText: text);
```
Into string lines.

```csharp
IEnumerable<KeyValuePair<string, IString>> string_lines = 
    IniLinesReader.Default.ReadStringAsStringLines(
        srcText: text,
        lineFormat: LineFormat.Parameters);
```
And into a tree.

```csharp
ILineTree tree = 
    IniLinesReader.Default.ReadStringAsLineTree(
        srcText: text);
```

# Implementing
<details>
  <summary><b>ILineReader</b> is the root interface for localization reader classes. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Signals that file format can read localization files.
/// 
/// For reading capability, must implement one of:
/// <list type="Bullet">
/// <item><see cref="ILineStreamReader"/></item>
/// <item><see cref="ILineTreeStreamReader"/></item>
/// <item><see cref="ILineTextReader"/></item>
/// <item><see cref="ILineTreeTextReader"/></item>
/// <item><see cref="ILineStringTextReader"/></item>
/// <item><see cref="ILineStringStreamReader"/></item>
/// </list>
/// </summary>
public interface ILineReader : ILineFileFormat { }

/// <summary>
/// Reader that can read lines from a <see cref="Stream"/>.
/// </summary>
public interface ILineStreamReader : ILineReader
{
    /// <summary>
    /// Read <paramref name="stream"/> into lines.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
    /// <returns>the read lines</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<ILine> ReadLines(Stream stream, ILineFormat lineFormat = default);
}

/// <summary>
/// Reader that can read tree format from a <see cref="Stream"/>.
/// </summary>
public interface ILineTreeStreamReader : ILineReader
{
    /// <summary>
    /// Read <paramref name="stream"/> into tree structure.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
    /// <returns>lines in tree structure</returns>
    /// <exception cref="IOException"></exception>
    ILineTree ReadLineTree(Stream stream, ILineFormat lineFormat = default);
}

/// <summary>
/// Reader that can read localization lines from a <see cref="TextReader"/>.
/// </summary>
public interface ILineTextReader : ILineReader
{
    /// <summary>
    /// Read <paramref name="text"/> into tree structure.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
    /// <returns>the read lines</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<ILine> ReadLines(TextReader text, ILineFormat lineFormat = default);
}

/// <summary>
/// Reader that can read localization lines from a <see cref="TextReader"/>.
/// </summary>
public interface ILineTreeTextReader : ILineReader
{
    /// <summary>
    /// Read <paramref name="text"/> into lines.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
    /// <returns>lines in tree structure</returns>
    /// <exception cref="IOException"></exception>
    ILineTree ReadLineTree(TextReader text, ILineFormat lineFormat = default);
}

/// <summary>
/// Reader that can read string key-values from a <see cref="TextReader"/>.
/// </summary>
public interface ILineStringTextReader : ILineReader
{
    /// <summary>
    /// Read <paramref name="text"/> into lines.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
    /// <returns>the read string key-values</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<KeyValuePair<string, IString>> ReadStringLines(TextReader text, ILineFormat lineFormat = default);
}

/// <summary>
/// Reader that can read string key-values from a <see cref="Stream"/>.
/// </summary>
public interface ILineStringStreamReader : ILineReader
{
    /// <summary>
    /// Read <paramref name="stream"/> into lines.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
    /// <returns>the read string key-values</returns>
    /// <exception cref="IOException"></exception>
    IEnumerable<KeyValuePair<string, IString>> ReadStringLines(Stream stream, ILineFormat lineFormat = default);
}
```
</details>
<br/>
A class that implements **ILineReader** must to implement one of its sub-interfaces. A one that best suits the underlying format.

```csharp
class ExtFileFormatReader : ILineTextReader
{
    public string Extension => "ext";

    public IEnumerable<ILine> ReadLines(
        TextReader text, 
        ILineFormat lineFormat = null)
    {
        yield return LineAppender.Default.Section("MyClass").Key("HelloWorld").Culture("en").Format("Hello World!");
    }
}
```

Reader can be added to **LineReaderMap**.

```csharp
// Create writer
ILineReader format = new ExtFileFormatReader();

// Clone formats
LineFileFormatMap formats = LineReaderMap.Default.Clone();
// Add to clone
formats.Add(format);

// Or if in deploying application project, format can be added to the global singleton
(LineReaderMap.Default as IDictionary<string, ILineFileFormat>).Add(format);
```
