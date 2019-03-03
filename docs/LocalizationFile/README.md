# Localization File
Four fileformats can be used out of the box, provided that the files follow a suitable structure.
Localization files can be organized in many ways, internally and between each other. 
[Key name policy](../IAssetKeyNamePolicy/) is used for describing file names and lines.

Files can be loaded with a couple of ways.
1. Using a reader to construct [an asset](#loading-asset).
2. With asset loader and [part builder](../IAssetLoader/PartBuilder/index.md).
3. With asset loader and [part class](../IAssetLoader/PartClasses/index.md#file-strings).

Example of **.resx** localization file.
![resx](img1.png)

# Loading Asset
Files can then be loaded with a constructor.

```csharp
IAsset asset = IniFileFormat.Instance.CreateFileAsset("localization.ini");
```

# Implementing
<details>
  <summary><b>ILocalizationFileFormat</b> is interface for classes that tokenize text file formats, and any hierarchical formats. (<u>Click here</u>)</summary>

```csharp

```
</details>

<p/>
And then adding to constructor delegate to **LocalizationFileFormatMap.Singleton**.

```csharp
// Add reader of custom .ext format to the global collection of readers.
LocalizationFileFormatMap.Singleton["ext"] = new ExtFileFormat();
```

Non-hierarchical formats can be implemented by implementing IAsset that reads the format.	

<details>
  <summary>Example implementation ExtFileFormat. (<u>Click here</u>)</summary>

```csharp
class ExtFileFormat : ILocalizationKeyLinesStreamReader
{
    public string Extension => "ext";
    public IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(Stream stream, IAssetKeyNamePolicy namePolicy = default)
    {
        IAssetKey key = Key.Create("Section", "MyClass").Append("Key", "HelloWorld").Append("Culture", "en");
        yield return new KeyValuePair<IAssetKey, string>(key, "Hello World!");
    }
}
```
</details>

# Links
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [ILocalizationFileReadable](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationFile/ILocalizationFileReadable.cs)
 * [ILocalizationFileWritable](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationFile/ILocalizationFileWritable.cs)
 * [IniFileFormat](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationFile/IniFileFormat.cs)
 * [JsonFileFormat](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationFile/JsonFileFormat.cs)
 * [ResourcesFileFormat](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationFile/ResourcesFileFormat.cs)
 * [ResxFileFormat](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationFile/ResxFileFormat.cs)
 * [XmlFileFormat](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationFile/XmlFileFormat.cs)
