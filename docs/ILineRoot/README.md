# Localization Root
**ILineRoot** is an interface for root implementations. 
Root is the place where asset (the localization provider) is tied to key (localization consumer).

**LocalizationRoot** is the default implementation. It's constructed with an asset and a culture policy.

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "Culture:en:Type:MyController:Key:hello", "Hello World!" } };
// Create asset
IAsset asset = new StringAsset(source, LineFormat.Parameters);
// Create culture policy
ICulturePolicy culturePolicy = new CulturePolicy();
// Create root
ILineRoot root = new LineRoot(asset, culturePolicy);
```

Further keys are constructed from root. 

```csharp
// Construct key
ILine key = root.Type("MyController").Key("Hello");
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
ILine key = LineAppender.NonResolving.Section("Section").Key("Key");
// Retreieve with reference
IString str = asset.GetString(key).GetString();
```

# String Localizer
**StringLocalizerRoot** is an alternative root implementation.
Every key, that is constructed from this class, implements localization interfaces IStringLocalizer and IStringLocalizerFactory.

StringLocalizerRoot is constructed with an asset and a culture policy, just as LocalizationRoot.

```csharp
// Create localization source
var source = new List<ILine> { LineFormat.Parameters.Parse("Culture:en:Type:MyController:Key:hello").Format("Hello World!") };
// Create asset
IAsset asset = new StringAsset(source);
// Create culture policy
ICulturePolicy culturePolicy = new CulturePolicy();
// Create root
ILineRoot root = new StringLocalizerRoot(asset, culturePolicy);
```
<br/>

Keys can be type-casted to **IStringLocalizer**.

```csharp
// Assign as IStringLocalizer, use "MyController" as root.
IStringLocalizer stringLocalizer = root.Section("MyController").AsStringLocalizer();
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
Key can be assigned for a type **.Type(*Type*)** and then casted to **IStringLocalizer&lt;*Type*&gt;**.

```csharp
// Assign to IStringLocalizer for the class MyController
IStringLocalizer<MyController> stringLocalizer = 
    root.Type(typeof(MyController)).AsStringLocalizer<MyController>();
```
Also after type casting to IStringLocalizerFactory with **.Create(*Type*)**.

```csharp
// Assign as IStringLocalizerFactory
IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
// Create IStringLocalizer for the class MyController
IStringLocalizer<MyController> stringLocalizer = 
    stringLocalizerFactory.Create(typeof(MyController)) as IStringLocalizer<MyController>;
```

<br/>
Culture can be locked in with **.Culture(*string*)**.

```csharp
// Create IStringLocalizer and assign culture
IStringLocalizer stringLocalizer =
    root.Culture("en").Type<MyController>().AsStringLocalizer();
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
Localized strings can be considered as constants because they are used the same way as regular strings. 

Lexical.Localization introduces a global static root **LocalizationRoot.Global**.

```csharp
// Create key from global root
ILine key = LineRoot.Global.Type("MyController").Key("Hello");
```

Assets are added to the global root with **LocalizationRoot.Builder**.

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "Culture:en:Type:MyController:Key:hello", "Hello World!" } };
// Create asset
IAsset asset = new StringAsset(source, LineFormat.Parameters);
// Assets are added to global static builder. It must be (re-)built after adding.
LineRoot.Builder.AddAsset(asset).Build();
```

If assets are initialized in concurrent environment then please lock with **LocalizationRoot.Builder**.

```csharp
// If ran in multi-threaded initialization, lock to LocalizationRoot.Builder.
lock (LineRoot.Builder) LineRoot.Builder.AddAsset(asset).Build();
```

**StringLocalizerRoot** is the same root as *LocalizationRoot*, but has extra feature of implementing IStringLocalizer and IStringLocalizerFactory.
The calling assembly, however, needs to import NuGet **Microsoft.Extensions.Localization.Abstractions**.

```csharp
// StringLocalizerRoot is root for IStringLocalizer interoperability
IStringLocalizerFactory stringLocalizerFactory = StringLocalizerRoot.Global;
```

They share the same assets, and the root instances are interchangeable. Assets can be added to either root.

```csharp
// LocalizationRoot and StringLocalizerRoot are interchangeable. They share the same asset(s).
LineRoot.Builder.AddAsset(asset).Build();
IStringLocalizer<MyController> stringLocalizer = StringLocalizerRoot.Global.Type<MyController>().AsStringLocalizer<MyController>();
```

**LocalizationRoot.GlobalDynamic** returns dynamic instance for the static root.

```csharp
// Dynamic instance is acquired with LocalizationRoot.GlobalDynamic
dynamic key_ = LineRoot.GlobalDynamic.Section("Section").Key("Key");
```

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [ILineRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/ILineRoot.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [LocalizationRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationKey/LocalizationRoot.cs) ([Global](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationKey/LocalizationRoot_Global.cs))
 * [StringLocalizerRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringLocalizerRoot.cs) ([Global](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/StringLocalizerRoot_Global.cs))
* [Microsoft.Extensions.Localization.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/Localization/Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization.Abstractions/))
 * [IStringLocalizer](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizer.cs) 
 * [IStringLocalizer&lt;T&gt;](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerOfT.cs)
 * [IStringLocalizerFactory](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerFactory.cs)
* [Microsoft.Extensions.Localization](https://github.com/aspnet/Localization/tree/master/src/Microsoft.Extensions.Localization) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Localization/))

