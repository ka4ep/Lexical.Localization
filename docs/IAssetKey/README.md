# Asset Keys
Asset key serves a couple of functions. 
Mainly, it is a reference and a provider of localization resources and language strings.
It also mediates parameters and hints from the localization consumer to the producer.
<p/>

Asset key is constructed by appending parts from root key towards tail. 
The constructed result is, however, a linked list from tail to root,
as each link has a directed reference only towards its parent.

```csharp
IAssetKey key = new LocalizationRoot().Section("Section").Section("Section").Key("Key");
```
![linked list](linkedlist.svg)
<p/>
Nodes can also be constructed to span a tree structure.

```csharp
IAssetRoot root = new LocalizationRoot();
IAssetKey section1 = root.Section("Section1");
IAssetKey section2 = section1.Section("Section2");
IAssetKey section1_1 = section1.Section("Section1.1");
IAssetKey key1_1_1 = section1_1.Key("Key1");
IAssetKey key1_1_2 = section1_1.Key("Key2");
// ...
```
![tree](tree.svg)
<p/>

## Reference and Provider
Key can be used as a *reference*. 

```csharp
// Create localization reference
IAssetKey key = new LocalizationRoot().Section("Section").Section("Section").Key("Key");

// Retrieve string with a reference
string str = asset.GetString(key.Culture("en"));

// Retrieve binary resource with a reference
byte[] data = asset.GetResource(key.Culture("en"));
```

And, as a *provider*.
Provider key needs to have a root, which must be associated with an asset and a culture policy.
Extension methods can be used for retrieving strings and binary resources.

```csharp
// Language string source
Dictionary<string, string> src = new Dictionary<string, string> { { "en:Section:Key", "Hello World!" } };
// Create Asset
IAsset asset = new LocalizationStringAsset(src, AssetKeyNameProvider.Colon_Colon_Colon);
// Create culture policy
ICulturePolicy culturePolicy = new CulturePolicy();
// Create root
IAssetRoot root = new LocalizationRoot(asset, culturePolicy);
// Set Current Culture
CultureInfo.CurrentCulture = new CultureInfo("en");
// Create key specific provider
IAssetKey key = root.Section("Section").Key("Key");
// Retieve string from provider
string str = key.ToString();
// Retrieve binary resoruce from provider
byte[] data = key.GetResource();
```

## Canonical and non-canonical key parts
*Canonical* key parts are parts whose position in the linked list matters for the consumer of the localization content.
Canonical parts typically add to the identity of the key. 

```csharp
// Add canonical parts
IAssetKey key = new LocalizationRoot().Section("Section1").Section("Section2").Key("Key");

// "Section1:Section2:Key"
string id = AssetKeyNameProvider.Colon_Colon_Colon.BuildName(key);
```

For parts that are *non-canonical* the position doesn't matter.
Non-canonical parts are typically hints, such as **.SetCulture(*string*)**.

```csharp
// Add canonical parts, and non-canonical culture
IAssetKey key1 = new LocalizationRoot().Section("Section").Key("Key").Culture("en");
IAssetKey key2 = new LocalizationRoot().Culture("en").Section("Section").Key("Key");

// "en:Section1:Section2:Key"
string id1 = AssetKeyNameProvider.Colon_Colon_Colon.BuildName(key1);
// "en:Section1:Section2:Key"
string id2 = AssetKeyNameProvider.Colon_Colon_Colon.BuildName(key2);
```

## Key Parts
| Parameter | Canonical | Interface | Extension Method | Description |
|:---------|:-------|:--------|:---------|:---------|
| Root | canonical | IAssetRoot |  | Contains asset and culture policy. Keys are constructed from here. |
| Type | non-canonical | IAssetKeyTypeAssignable | .Type(*Type*) | Type section for grouping by classes and interfaces. |
| Location | canonical | IAsseyKeyLocationAssignable | .Location(*string*) | Hint to asset for a directory to search from. |
| Assembly | non-canonical | IAssetKeyAssemblyAssignable | .Assembly(*Assembly*) | Hint to asset for an assembly to search from. |
| Resource | canonical | IAssetKeyResourceAssignable | .Resource(*string*) | Hint to asset for an embedded resource path to search from. |
| Section | canonical | IAssetKeySectionAssignable | .Section(*string*) | Generic section for grouping assets. |
| Key | canonical | IAssetKeyAssignable | .Key(*string*) | Leaf key |
| Culture | non-canonical | ILocalizationKeyCultureAssignable | .Culture(*CultureInfo*) | Parameter to override current culture. |
| N | non-canonical | ILocalizationKeyPluralityAssignable | .N(*Type*) | Key that specifies plurality |
|  | non-canonical | ILocalizationKeyFormatArgs | .Format(*Object[]*) | Format arguments parameter. |
|  | non-canonical | ILocalizationKeyInlined | .Inline(*string*, *string*) | Hint for default culture specific string values. |

## Type Section
Type section is a key that narrows down the scope of localization to a specific class, interface or structure.

```csharp
// Assign key to localization of type "MyController"
IAssetKey key = new LocalizationRoot().Type(typeof(MyController));
// Search "MyController:Success"
string str = key.Key("Success").ToString();
```

There is another way with the generic method **.Type&lt;T&gt;()**. 

```csharp
// Assign key to localization of type "MyController"
IAssetKey<MyController> key = new LocalizationRoot().Type<MyController>();
```

## Culture Key
Key can be forced to ignore culture policy, possibly due to lack of policy assignment, and to use an explicit culture info.

```csharp
// Create root that matches only to english strings
IAssetKey root_en = new LocalizationRoot().Culture("en");
// Create key
IAssetKey key = root_en.Section("Section").Key("Key");
```

## Formatting Args
Formattable language strings are written in format of "**{#}**", where # is replaced with argument index ([Format Syntax](https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting#format-item-syntax)).

Let's have an example file *localization.ini*.
```None
[Culture:en]
Type:ConsoleApp1.MyController:Key:Success      = Success
Type:ConsoleApp1.MyController:Key:Error        = Error (Code=0x{0:X8})
```

The key assigned with a format argument by **.Format(*Object[]* args)** call.

```csharp
// Create key "Error"
IAssetKey key = root.Type("ConsoleApp1.MyController").Key("Error");
// Formulate key
IAssetKey key_formulated = key.Format(0xFeedF00d);
```

The parametrized key can be resolved to formatted string with **.ToString()** or **.ResolveFormulatedString()**.

```csharp
// Resolve to formulated string to "Error (Code=0xFEEDF00D)"
string str = key_formulated.ResolveFormulatedString();
```

But, if needed an unformulated string can be resolved with **.ResolveString()**.

```csharp
// Resolve to localized string "Error (Code=0x{0:X8})", but does not formulate.
string str = key_formulated.ResolveString();
```

## Inlining
Default language strings can be written right into the code.
Code can be [automatically scanned](http://lexical.fi/sdk/Localization/docs/InlineScanner/index.html) for inlined strings and exported to localization files.
They can be used as templates for further translation process. 
This way the templates don't need to be manually updated as the code evolves.

```csharp
// Create root
IAssetRoot root = new LocalizationRoot();
// Create key and add default strings
IAssetKey key = root.Section("Section").Key("Success")                    
    .Inline("Culture:en", "Success")                           // Add inlining to the root culture ""
    .Inline("Culture:fi", "Onnistui")                          // Add inlining to culture "fi"
    .Inline("Culture:sv", "Det funkar");                       // Add inlining to culture "sv"

// Resolve string from inlined key "Success"
string str = key.Culture("en").ToString();
```

There are extension methods for every language in namespace **Lexical.Localization.Inlines**. 

```csharp
using Lexical.Localization.Inlines;
```
Which can be used for slightly shortened presentation.

```csharp
// Create key and add default strings
IAssetKey key = root.Section("Section").Key("Success")
    .en("Success")                                     // Add inlining to culture "en"
    .fi("Onnistui")                                    // Add inlining to culture "fi"
    .sv("Det funkar");                                 // Add inlining to culture "sv"
```

Root culture is the one with empty string "" as its name. 
It is the fallback culture to look into in case localization is not provided for the active culture.
The recommended practice to add strings for the fallback culture, and in international english.

```csharp
IAssetKey key = root.Section("Section").Key("Success")
    .Inline("Success")                                 // Add inlining to the root culture ""
    .en("Success")                                     // Add inlining to "en", optional as root culture is same
    .fi("Onnistui")                                    // Add inlining to "fi"
    .sv("Det funkar");                                 // Add inlining to "sv"
```

## Dynamic use
Asset keys from LocalizationRoot and StringLocalizationRoot are dynamically usable.

```csharp
// Dynamic assignment
dynamic root = new LocalizationRoot();
// Provides to string on typecast
string str = root.Section("Section").Key("Hello");
```

## Use in classes
If class is designed to support dependency injection without string localizers, the constructors should 
take in argument *IAssetKey&lt;T&gt;* and possibly *IAssetRoot*. See more in [Best Practices](../BestPractices/).
Constructor argument **IAssetKey&lt;T&gt;** helps the Dependency Injection to assign the localization so that it is scoped in to correct typesection.

```csharp
class MyController
{
    IAssetKey localization;

    public MyController(IAssetKey<MyController> localization)
    {
        this.localization = localization;
    }

    public MyController(IAssetRoot localizationRoot)
    {
        this.localization = localizationRoot.Type<MyController>();
    }

    public void Do()
    {
        string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
    }
}
```

If class is designed to use static instance and without dependency injection, localization reference can be acquired from **LocalizationRoot**.

```csharp
class MyControllerB
{
    static IAssetKey localization = LocalizationRoot.Global.Type<MyControllerB>();

    public void Do()
    {
        string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
    }
}
```
