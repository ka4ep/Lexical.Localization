# Asset Key Name Provider
**AssetKeyNameProvider** is a class that appends key parts together. 
It starts with non-canonical parts, and then canoncial parts.

Let's create an example key.

```csharp
// Let's create an example key
IAssetKey key = new LocalizationRoot()
        .Location("Patches")
        .Section("Controllers")
        .Type("MyController")
        .Section("Errors")
        .Key("InvalidState")
        .Culture("en");
```
And now, let's try out different policies to see how they look.

```csharp
// "en:Patches:Controllers:MyController:Errors:InvalidState"
string str1 = AssetKeyNameProvider.Default.BuildName(key);
// "en.Patches.Controllers.MyController.Errors.InvalidState"
string str2 = AssetKeyNameProvider.Dot_Dot_Dot.BuildName(key);
// "Patches:Controllers:MyController:Errors:InvalidState"
string str3 = AssetKeyNameProvider.None_Colon_Colon.BuildName(key);
// "en:Patches.Controllers.MyController.Errors.InvalidState"
string str4 = AssetKeyNameProvider.Colon_Dot_Dot.BuildName(key);
// "en:Patches:Controllers:MyController:Errors.InvalidState"
string str5 = AssetKeyNameProvider.Colon_Colon_Dot.BuildName(key);
```

Custom policies can be created by instantiating AssetKeyNameProvider and adding configurations.

```csharp
// Create a custom policy 
IAssetKeyNamePolicy myPolicy = new AssetKeyNameProvider()
    // Enable non-canonical "Culture" parameter with "/" separator
    .Rule("Culture", true, postfixSeparator: "/", order: ParameterInfos.Default["Culture"].Order)
    // Disable other non-canonical parts
    .NonCanonicalRule(false)
    // Enable canonical all parts with "/" separator
    .CanonicalRule(true, prefixSeparator: "/")
    // Set "Key" parameter's prefix to "/"
    .Rule("Key", true, prefixSeparator: "/", order: ParameterInfos.Default["Key"].Order);

// "en/Patches/MyController/Errors/InvalidState"
string str = myPolicy.BuildName(key);
```

<details>
  <summary><b>IAssetKeyNameProvider</b> is policy interface where Build() can be implemented directly. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Converts <see cref="IAssetKey"/> to <see cref="String"/>.
/// </summary>
public interface IAssetKeyNameProvider : IAssetKeyNamePolicy
{
    /// <summary>
    /// Build path string from key.
    /// </summary>
    /// <param name="str"></param>
    /// <returns>full name string</returns>
    string BuildName(IAssetKey str);
}
```
</details>

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is a subinterface where Build() can be implemented directly.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetKeyNameProvider.cs) is implementation of IAssetNameProvider.
 