# Localization Root
**IAssetRoot** is an interface for root implementations. 
Root is the place where asset (the localization provider) is tied to key (localization consumer).

**LocalizationRoot** is the default implementation. It's constructed with an asset and a culture policy.

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "en:MyController:hello", "Hello World!" } };
// Create asset
IAsset asset = new LocalizationStringDictionary(source);
// Create culture policy
ICulturePolicy culturePolicy = new CulturePolicy();
// Create root
IAssetRoot root = new LocalizationRoot(asset, culturePolicy);
```

Further keys are constructed from root. 

```csharp
// Construct key
IAssetKey key = root.TypeSection("MyController").Key("Hello");
```

Now that key is associated with an asset and a culture provider, it can provide strings and resources.

```csharp
// Set active culture for this root
(root.FindCulturePolicy() as ICulturePolicyAssignable).SetCultures("en", "");
// Provide string
string str = key.ToString();
```

Note, that root is not mandatory, keys can be constructed with *null* as previous key.
These keys cannot be used as providers, only as references.

```csharp
// Create reference
IAssetKey key = new LocalizationKey._Section(null, "Section").Key("Key");
// Retreieve with reference
string str = asset.GetString(key);
```

# String Localizer
**StringLocalizerRoot** is an alternative root implementation.
Every key, that is constructed from this class, implements localization interfaces IStringLocalizer and IStringLocalizerFactory.

The caller must import NuGet **Microsoft.Extensions.Localization.Abstractions** and namespace **Lexical.Localization.Ms.Extensions**.
StringLocalizerRoot is constructed with an asset and a culture policy, just as LocalizationRoot.

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "en:MyController:hello", "Hello World!" } };
// Create asset
IAsset asset = new LocalizationStringDictionary(source);
// Create culture policy
ICulturePolicy culturePolicy = new CulturePolicy();
// Create root
IAssetRoot root = new StringLocalizerRoot(asset, culturePolicy);
```
<br/>

Keys can be type-casted to **IStringLocalizer**.

```csharp
// Assign as IStringLocalizer, use "MyController" as root.
IStringLocalizer stringLocalizer = root.Section("MyController") as IStringLocalizer;
```
And to **IStringLocalizerFactory**.

```csharp
// Assign as IStringLocalizerFactory
IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
// Adapt to IStringLocalizer
IStringLocalizer<MyController> stringLocalizer2 = 
    stringLocalizerFactory.Create(typeof(MyController)) 
    as IStringLocalizer<MyController>;
```

<br/>
Key can be assigned for a type **.TypeSection(*Type*)** and then casted to **IStringLocalizer&lt;*Type*&gt;**.

```csharp
// Assign to IStringLocalizer for the class MyController
IStringLocalizer<MyController> stringLocalizer = 
    root.TypeSection(typeof(MyController)) 
    as IStringLocalizer<MyController>;
```
Also after type casting to IStringLocalizerFactory with **.Create(*Type*)**.

```csharp
// Assign as IStringLocalizerFactory
IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
// Create IStringLocalizer for the class MyController
IStringLocalizer<MyController> stringLocalizer = 
    stringLocalizerFactory.Create(typeof(MyController)) 
    as IStringLocalizer<MyController>;
```

<br/>
Culture can be locked in with **.SetCulture(*string*)**.

```csharp
// Create IStringLocalizer and assign culture
IStringLocalizer stringLocalizer = 
    root.SetCulture("en").TypeSection<MyController>() 
    as IStringLocalizer<MyController>;
```
And also after type casting to IStringLocalizer with **.WithCulture(*CultureInfo*)**.

```csharp
// Assign as IStringLocalizerFactory
IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
// Create IStringLocalizer and assign culture
IStringLocalizer stringLocalizer = stringLocalizerFactory.Create(typeof(MyController))
    .WithCulture(CultureInfo.GetCultureInfo("en"));
```

# Global static root
In programming, the usage of global instances is typically refrained due to various downsides.
However, these reasons don't apply for localization use. 
Localized strings can be considered as constants because they are used the same way as regular strings. 
Therefore global static instance for localization root is worth consideration.

Lexical.Localization introduces a global static root **LocalizationRoot.Global**.

```csharp
// Create key from global root
IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Hello");
```

Assets are added to the global root with **LocalizationRoot.Builder**.

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "en:MyController:hello", "Hello World!" } };
// Create asset
IAsset asset = new LocalizationStringDictionary(source);
// Assets are added to global static builder. It must be (re-)built after adding.
LocalizationRoot.Builder.AddAsset(asset).Build();
```

If assets are initialized in concurrent environment then please lock with **LocalizationRoot.Builder**.

```csharp
// If ran in multi-threaded initialization, lock to LocalizationRoot.Builder.
lock (LocalizationRoot.Builder) LocalizationRoot.Builder.AddAsset(asset).Build();
```

**StringLocalizerRoot** is the same root as *LocalizationRoot*, but has extra feature of implementing IStringLocalizer and IStringLocalizerFactory.
The calling assembly, however, needs to import NuGet **Microsoft.Extensions.Localization.Abstractions**
and namespace **Lexical.Localization.Ms.Extensions**.

```csharp
// StringLocalizerRoot is root for IStringLocalizer interoperability
IStringLocalizerFactory stringLocalizerFactory = StringLocalizerRoot.Global;
```

They share the same assets, and the root instances are interchangeable. Assets can be added to either root.

```csharp
// LocalizationRoot and StringLocalizerRoot are interchangeable. They share the same asset(s).
LocalizationRoot.Builder.AddAsset(asset).Build();
IStringLocalizer stringLocalizer = StringLocalizerRoot.Global.TypeSection<MyController>();
```

**LocalizationRoot.GlobalDynamic** returns dynamic instance for the static root.

```csharp
// Dynamic instance is acquired with LocalizationRoot.GlobalDynamic
dynamic key_ = LocalizationRoot.GlobalDynamic.Section("Section").Key("Key");
```

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/AssetKey/IAssetRoot.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [LocalizationRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationKey/LocalizationRoot.cs) ([Global](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationKey/LocalizationRoot_Global.cs))
 * [StringLocalizerRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Localization/StringLocalizerRoot.cs) ([Global](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Localization/StringLocalizerRoot_Global.cs))
* [Microsoft.Extensions.Localization.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/Localization/Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization.Abstractions/))
 * [IStringLocalizer](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizer.cs) 
 * [IStringLocalizer&lt;T&gt;](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerOfT.cs)
 * [IStringLocalizerFactory](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerFactory.cs)
* [Microsoft.Extensions.Localization](https://github.com/aspnet/Localization/tree/master/src/Microsoft.Extensions.Localization) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization/))

