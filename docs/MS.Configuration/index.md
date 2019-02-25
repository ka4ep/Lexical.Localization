# Ms.Configuration

Microsoft.Extensions.Configuration.**IConfiguration** is an abstraction for loading configurations
of various sources. Configurations are key-value pairs, just like language strings are, 
which makes *IConfiguration* usable for localization use.

**ConfigurationLocalizationAsset** is a class that adapts *IConfiguration* into an *IAsset*.
Note that the calling assembly must import NuGet **Microsoft.Extensions.Configuration.Abstractions** and
namespace **Lexical.Localization.Ms.Extensions.Configuration**.
# [Snippet](#tab/snippet-1)
[!code-csharp[Snippet](Example1.cs#Snippet)]
# [Full Code](#tab/full-1)
[!code-csharp[Snippet](Example1.cs)]
***

<br/>
If asset is constructed with asset builder, then **ConfigurationBuilderLocalizationAssetSource** 
can be used to adapt IConfigurationBuilder to IAssetSource. 
When asset is being built, asset source asks the IConfigurationBuilder to construct its IConfiguration, which will then be adapted to IAsset.

There is an extension method **.AddConfigurationBuilder()** that adds IConfigurationBuilder it to IAssetBuilder.
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](Example2.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](Example2.cs)]
***

<br/>
Example of localization file when read with *IConfiguration*.

# [Xml](#tab/xml)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<localization>
  <ConsoleApp1.MyController>
    <Success>Success</Success>
    <Error>Error (Code=0x{0:X8})</Error>
  </ConsoleApp1.MyController>
  <en>
    <ConsoleApp1.MyController>
      <Success>Success</Success>
      <Error>Error (Code=0x{0:X8})</Error>
    </ConsoleApp1.MyController>
  </en>
  <fi>
    <ConsoleApp1.MyController>
      <Success>Onnistui</Success>
      <Error>Virhe (Koodi=0x{0:X8})</Error>
    </ConsoleApp1.MyController>
  </fi>
</localization>
```
# [Ini](#tab/ini)

```ini
ConsoleApp1.MyController:Success      = Success
ConsoleApp1.MyController:Error        = Error (Code=0x{0:X8})

[en]
ConsoleApp1.MyController:Success      = Success
ConsoleApp1.MyController:Error        = Error (Code=0x{0:X8})

[fi]
ConsoleApp1.MyController:Success      = Onnistui
ConsoleApp1.MyController:Error        = Virhe (Koodi=0x{0:X8})
```

# [Json](#tab/json)
```json
{
  "ConsoleApp1.MyController": {
      "Success": "Success",
      "Error": "Error (Code=0x{0:X8})"
  },
  "en": {
    "ConsoleApp1.MyController": {
      "Success": "Success",
      "Error": "Error (Code=0x{0:X8})"
    }
  },
  "fi": {
    "ConsoleApp1.MyController": {
      "Success": "Onnistui",
      "Error": "Virhe (Koodi=0x{0:X8})"
    }
  }
}
```

***


# Links
* [Microsoft.Extensions.Configuration.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/Configuration/Config.Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Abstractions/))
 * [IConfiguration](https://github.com/aspnet/Extensions/blob/master/src/Configuration/Config.Abstractions/src/IConfiguration.cs)
 * [IConfigurationBuilder](https://github.com/aspnet/Extensions/blob/master/src/Configuration/Config.Abstractions/src/IConfigurationBuilder.cs)
* [Microsoft.Extensions.Configuration](https://github.com/aspnet/Extensions/tree/master/src/Configuration/Config/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/))
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [ConfigurationLocalizationAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Configuration/ConfigurationLocalizationAsset.cs)
 * [ConfigurationBuilderLocalizationAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Configuration/ConfigurationBuilderLocalizationAssetSource.cs)
