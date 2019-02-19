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
using (FileStream fs = new FileStream("localization.ini", FileMode.Open))
{
    // Get .ext file format
    ILocalizationFileStreamReader fileFormat = LocalizationFileFormatMap.Singleton.TryGet("ini") as ILocalizationFileStreamReader;
    // Create reader
    ILocalizationFileReadable textReader = fileFormat.OpenStream(fs, AssetKeyNameProvider.Default);
    // Convert to asset
    IAsset asset = textReader.ToAssetAndClose();
}
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
class ExtFileFormat : ILocalizationFileFormat, ILocalizationFileStreamReader
{
    public string Extension 
        => "ext";
    public ILocalizationFileReadable OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = null)
        => new ExtReader(stream, namePolicy);
}

class ExtReader : ILocalizationFileReadable
{
    public IAssetKeyNamePolicy NamePolicy => throw new System.NotImplementedException();
    public ExtReader(Stream stream, IAssetKeyNamePolicy namePolicy)
    {
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<TextElement> Read()
    {
        throw new System.NotImplementedException();
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
