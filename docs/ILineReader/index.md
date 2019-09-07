# Localization Reader
There are following file formats are supported by the Lexical.Localization class library.

| Format | Reader Class |
|:---------|:-------|
| .ini | IniLinesWriter |
| .json | JsonLinesReader |
| .xml | LocalizationXmlReader |
| .resx | LocalizationResxReader |
| .resources | ResourcesLineReader |

**ILineFileFormat** instance can be acquired from **LineReaderMap** dictionary.
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]

And from singleton instances.
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

# Read IAsset
File can be read right away into an *IAsset* with **.FileAsset()** extension method.
[!code-csharp[Snippet](Examples.cs#Snippet_10a)]
[!code-csharp[Snippet](Examples.cs#Snippet_10a2)]

From embedded resource with **.EmbeddedAsset()** method.
[!code-csharp[Snippet](Examples.cs#Snippet_10b)]
[!code-csharp[Snippet](Examples.cs#Snippet_10b2)]

And from a file provider with **.FileProviderAsset()**. 
[!code-csharp[Snippet](Examples.cs#Snippet_10c)]
[!code-csharp[Snippet](Examples.cs#Snippet_10c2)]

# Read IAssetSource
File can be read into an *IAssetSource* with **.FileAssetSource()** extension method. *IAssetSource* is a reference and a reader of asset.
It is not read right away, but when the asset is built.
[!code-csharp[Snippet](Examples.cs#Snippet_11a)]
[!code-csharp[Snippet](Examples.cs#Snippet_11a2)]

Reference to embedded resource source with **.EmbeddedAssetSource()**.
[!code-csharp[Snippet](Examples.cs#Snippet_11b)]
[!code-csharp[Snippet](Examples.cs#Snippet_11b2)]

And file provider with **.FileProviderAssetSource()**.
[!code-csharp[Snippet](Examples.cs#Snippet_11c)]
[!code-csharp[Snippet](Examples.cs#Snippet_11c2)]

# Read File
Different file formats have different intrinsic formats. 
* String list formats are **IEnumerable&lt;ILine&gt;**.
* Context dependent string list formats are **IEnumerable&lt;KeyValuePair&lt;string, IString&gt;&gt;**.
* Structural tree formats are **ILineTree**.

Localization file can be read right away into key lines with **.ReadLines()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]
Into string lines with **.ReadUnformedLines()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]
And into a tree **.ReadLineTree()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]

# File Reader
A file reader can be constructed with respective **.FileReader()**.
File reader reads the refered file when **.GetEnumerator()** is called, and will re-read the file again every time.
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]
**.FileReaderAsUnformedLines()** creates a reader that returns string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]
And **.FileReaderAsLineTree()** a tree reader.
[!code-csharp[Snippet](Examples.cs#Snippet_2c)]

# Embedded Reader
Embedded resource reader is created with **.EmbeddedReader()**.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]
**.EmbeddedReaderAsUnformedLines()** creates embedded reader of string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]
And **.EmbeddedReaderAsLineTree()** reader of trees
[!code-csharp[Snippet](Examples.cs#Snippet_3c)]

# IFileProvider Reader
File provider reader is created with **.FileProviderReader()**.
[!code-csharp[Snippet](Examples.cs#Snippet_4a)]
**.FileProviderReaderAsUnformedLines()** creates string lines reader
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]
And **.FileProviderReaderAsLineTree()** tree reader.
[!code-csharp[Snippet](Examples.cs#Snippet_4c)]

# Read Stream
Text files can be read from **Stream** into key lines.
[!code-csharp[Snippet](Examples.cs#Snippet_5a)]
Into string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_5b)]
And into a tree.
[!code-csharp[Snippet](Examples.cs#Snippet_5c)]

# Read TextReader
Text files can be read from **TextReader** into key lines.
[!code-csharp[Snippet](Examples.cs#Snippet_6a)]
Into string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_6b)]
And into tree.
[!code-csharp[Snippet](Examples.cs#Snippet_6c)]

# Read String
And from **String** into key lines.
[!code-csharp[Snippet](Examples.cs#Snippet_7a)]
Into string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_7b)]
And into a tree.
[!code-csharp[Snippet](Examples.cs#Snippet_7c)]

# Implementing
<details>
  <summary><b>ILineReader</b> is the root interface for localization reader classes. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/File/ILineReader.cs#doc)]
</details>
<br/>
A class that implements **ILineReader** must to implement one of its sub-interfaces. A one that best suits the underlying format.
[!code-csharp[Snippet](Examples.cs#Snippet_30)]

Reader can be added to **LineReaderMap**.
[!code-csharp[Snippet](Examples.cs#Snippet_30a)]
