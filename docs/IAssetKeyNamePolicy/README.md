# Asset Key Name Policy
Asset key name policy is a pattern rule mechanism that converts IAssetKeys into identity strings so that they can matched against lines in identity based localization sources.

For instance, if localization source has separator character '/', 
then the loading asset must be instructed to use a policy that matches the separator. 

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "en/MyController/Hello", "Hello World!" } };
// Create key name policy
IAssetKeyNamePolicy policy = new AssetKeyNameProvider().SetDefault(true, "/");
// Create asset
IAsset asset = new LocalizationStringAsset(source, policy);
// Create key
IAssetKey key = new LocalizationRoot(asset).Section("MyController").Key("Hello");
// Retrieve string
string str = key.SetCulture("en").ResolveFormulatedString();
```

Extension method **.Build(*IAssetKey*)** can be used to test the conversion from key to identity. It forwards the method call to correct sub-interface.

```csharp
// Test if key converted correctly to expected identity "en/Section/Key"
string id = policy.BuildName(key.SetCulture("en"));
```

<details>
  <summary><b>IAssetKeyNamePolicy</b> is the root interface for classes that formulate **IAssetKey**s into identity strings. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Signal that the class can convert <see cref="IAssetKey"/> into strings.
/// 
/// Consumer of this interface should call <see cref="AssetKeyExtensions.BuildName(IAssetKeyNamePolicy, IAssetKey)"/>.
/// 
/// Producer to this interface should implement one of the more specific interfaces:
///  <see cref="IAssetKeyNameProvider"/>
///  <see cref="IAssetNamePattern"/>
/// </summary>
public interface IAssetKeyNamePolicy
{
}
```
</details>

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is a subinterface where Build() can be implemented directly.
 * [IAssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs) is a subinterface that formulates parametrization with a template string.
 * [Key.NamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/Key.NamePolicy.cs) is context-free key format.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetKeyNameProvider.cs) is implementation of IAssetNameProvider.
 * [AssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetNamePattern.cs) is the default implementation of IAssetNamePattern.
 