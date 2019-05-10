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
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]

And from singleton instances.
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

# Read IAsset
File can be read right away into an *IAsset* with **.FileAsset()** extension method.
[!code-csharp[Snippet](Examples.cs#Snippet_10a)]

From embedded resource with **.EmbeddedAsset()** method.
[!code-csharp[Snippet](Examples.cs#Snippet_10b)]

And from a file provider with **.FileProviderAsset()**. 
[!code-csharp[Snippet](Examples.cs#Snippet_10c)]

The same extension methods are also available in the **LocalizationReaderMap**, which selects the reader class by file extension.
[!code-csharp[Snippet](Examples.cs#Snippet_10d)]

# Read IAssetSource
File can be read into an *IAssetSource* with **.FileAsset()** extension method. *IAssetSource* is a reference and a loader of asset.
It is not read right away, but when the asset is built.
[!code-csharp[Snippet](Examples.cs#Snippet_11a)]

Reference to embedded resource source with **.EmbeddedAssetSource()**.
[!code-csharp[Snippet](Examples.cs#Snippet_11b)]

And file provider with **.FileProviderAssetSource()**.
[!code-csharp[Snippet](Examples.cs#Snippet_11c)]

The same extension methods are also available in the **LocalizationReaderMap**, which selects the reader class by file extension.
[!code-csharp[Snippet](Examples.cs#Snippet_11d)]

# Read File
Different file formats have different intrinsic formats. 
* Context free list formats are handled with **IEnumerable&lt;KeyValuePair&lt;IAssetKey, string&gt;&gt;** class.
* Context dependent list formats are held in **IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;**.
* Structural file formats with context free keys are held in **ILineTree**.

Localization file can be read right away into key lines with **.ReadKeyLines()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]
Into string lines with **.ReadStringLines()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]
And into a tree **.ReadLineTree()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]

# File Reader
A file reader can be constructed with respective **.FileReaderAsKeyLines()**.
File reader reads the refered file when **.GetEnumerator()** is called, and will re-read the file again every time.
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]
**.FileReaderAsStringLines()** creates a reader that returns string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]
And **.FileReaderAsLineTree()** a tree reader.
[!code-csharp[Snippet](Examples.cs#Snippet_2c)]

# Embedded Reader
Embedded resource reader is created with **.EmbeddedReaderAsKeyLines()**.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]
**.EmbeddedReaderAsStringLines()** creates embedded reader of string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]
And **.EmbeddedReaderAsLineTree()** reader of trees
[!code-csharp[Snippet](Examples.cs#Snippet_3c)]

# IFileProvider Reader
File provider reader is created with **.FileProviderReaderAsKeyLines()**.
[!code-csharp[Snippet](Examples.cs#Snippet_4a)]
**.FileProviderReaderAsStringLines()** creates string lines reader
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]
And **.FileProviderReaderAsLineTree()** tree reader.
[!code-csharp[Snippet](Examples.cs#Snippet_4c)]

# Read Stream
Content can be read from **Stream** into key lines.
[!code-csharp[Snippet](Examples.cs#Snippet_5a)]
Into string lines.
[!code-csharp[Snippet](Examples.cs#Snippet_5b)]
And into a tree.
[!code-csharp[Snippet](Examples.cs#Snippet_5c)]

# Read TextReader
Content can be read from **TextReader** into key lines.
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
  <summary><b>ILocalizationReader</b> is the root interface for localization reader classes. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/LocalizationFile/ILocalizationReader.cs#Interface)]
</details>
<br/>
A class that implements **ILocalizationReader** must to implement one of its sub-interfaces. A one that best suits the underlying format.
[!code-csharp[Snippet](Examples.cs#Snippet_30)]

Reader can be added to **LocalizationReaderMap**.
[!code-csharp[Snippet](Examples.cs#Snippet_30a)]
