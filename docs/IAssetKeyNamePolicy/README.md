# Asset Key Name Policy
Asset key name policy is a mechanism that converts IAssetKeys into identity strings so that they can be match against lines in localization sources.

For instance, if localization source has separator character '/', 
then the loading asset must be instructed to use a policy that matches the separator. 

```csharp
// Create localization source
var source = new Dictionary<string, string> { { "en/MyController/Hello", "Hello World!" } };
// Create key name policy
IAssetKeyNamePolicy policy = new AssetKeyNameProvider().SetDefault(true, "/");
// Create asset
IAsset asset = new LocalizationStringDictionary(source, policy);
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

# Asset Key Name Provider
**AssetKeyNameProvider** is a class that appends key parts together. 
It starts with non-canonical parts, and then canoncial parts.

Let's create an example key.

```csharp
// Let's create an example key
IAssetKey key = new LocalizationRoot()
        .Location("Patches")
        .TypeSection("MyController")
        .Section("Errors")
        .Key("InvalidState")
        .SetCulture("en");
```
And now, let's try out different policies to see how they look.

```csharp
// "en:Patches:MyController:Errors:InvalidState"
string str1 = AssetKeyNameProvider.Default.BuildName(key);
// "en.Patches.MyController.Errors.InvalidState"
string str2 = AssetKeyNameProvider.Dot_Dot_Dot.BuildName(key);
// "Patches:MyController:Errors:InvalidState"
string str3 = AssetKeyNameProvider.None_Colon_Colon.BuildName(key);
// "en:Patches.MyController.Errors.InvalidState"
string str4 = AssetKeyNameProvider.Colon_Dot_Dot.BuildName(key);
```

Custom policies can be created by instantiating AssetKeyNameProvider and adding configurations.

```csharp
// Create a custom policy 
IAssetKeyNamePolicy myPolicy = new AssetKeyNameProvider()
    // Enable non-canonical "culture" parameter with "/" separator
    .SetParameter("culture", true, "", "/")
    // Disable other non-canonical parts
    .SetNonCanonicalDefault(false)
    // Enable canonical all parts with "/" separator
    .SetCanonicalDefault(true, "/", "")
    // Set "key" parameter's prefix to "/" and postfix to ".txt".
    .SetParameter("key", true, "/", ".txt");

// "en/Patches/MyController/Errors/InvalidState.txt"
string str = myPolicy.BuildName(key);
```

<details>
  <summary><b>IAssetKeyNameProvider</b> is policy interface where Build() can be implemented directly. (<u>Click here</u>)</summary>

```csharp
public interface IAssetKeyNameProvider : IAssetKeyNamePolicy
{
    /// <summary>
    /// Build path string from key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="parametrizer">(optional) how to extract parameters from key. If not set uses the default implementation <see cref="AssetKeyParametrizer"/></param>
    /// <returns>full name string</returns>
    string BuildName(object key, IAssetKeyParametrizer parametrizer = default);
}
```
</details>

# Asset Name Pattern
**AssetNamePattern** is another way of formulating *IAssetKeyNamePolicy*.

```csharp
// Create similiar policy with AssetNamePattern
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{culture/}{location/}{type/}{section/}[key].txt");
// "en/Patches/MyController/Errors/InvalidState.txt"
string str = myPolicy.BuildName(key);
```

Name pattern consists of parameters. They are written in format of "{prefix **parametername** suffix}".  
Parameter is optional when it's written inside braces "{parameter/}" and required with written inside brackets "[parameter/]".

```csharp
// Create name pattern
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("Patches/{section}[-key]{-culture}.png");
```

Parameter can be added multiple times by adding suffix "_#". Replace # with the occurance index. "_n" represents the last occurance.

```csharp
// Create name pattern
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{location_0/}{location_1/}{location_n/}{section}{-key}{-culture}.png");
// Create key
IAssetKey key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").SetCulture("de");
// Converts to "Patches/20181130/icons-ok-de.png"
string str = myPolicy.BuildName(key2);
```

Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}".
Expressions give more control when name pattern is used for matching against filenames or key-value lines.

```csharp
// Create name pattern with regular expression detail
IAssetNamePattern myPolicy = new AssetNamePattern("{location<[^/]+>/}{section}{-key}{-culture}.png");
// Use its regular expression
Match match = myPolicy.Regex.Match("patches/icons-ok-de.png");
```

Some of the parameters are well-known, and they also have a respective method that parametrizes a key.

| Parameter | Key Method  | Description |
|----------|:--------|:------------|
| assembly | .AssemblySection(*string*) | Assembly name |
| location | .Location(*string*) | Subdirectory in local files |
| resource | .Resource(*string*) | Subdirectory in embedded resources |
| type | .TypeSection(*string*) | Class name |
| section | .Section(*string*) | Generic section, used for grouping |
| anysection | *all above* | Matches to any section above. |
| culture  | .SetCulture(*string*) | Culture |
| key | .Key(*string*) | Key name |

<br/>
Custom parameters can be created. Parameter key object should implement IAssetKey, have [AssetKeyParameter] attribute and [AssetKeyConstructor] in the method that creates it.

<details>
  <summary><b>IAssetNamePattern</b> is the interface for name patterns. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// A name pattern, akin to regular expression, that can be matched against filenames and <see cref="IAssetKey"/> instances.
/// Is a sequence of parameter and text parts.
/// 
/// Parameter parts:
///  {culture}           - Matches to key.SetCulture("en")
///  {assembly}          - Matches to key.AssemblySection(asm).
///  {resource}          - Matches to key.ResourceSection("xx").
///  {type}              - Matches to key.TypeSection(type)
///  {section}           - Matches to key.Section("xx")
///  {location}          - Matches to key.LocationSection("xx") and a physical folder, separator is '/'.
///  {anysection}        - Matches to assembly, type and section.
///  {key}               - Matches to key key.Key("x")
/// 
/// Before and after the part pre- and postfix separator characters can be added:
///  {/culture.}
///  
/// Parts can be optional in curly braces {} and required in brackets [].
///  [culture]
/// 
/// Part can be added multiple times, which matches when part has identifier secion multiple times. Latter part names must be suffixed with "_number".
///  "localization{-key_0}{-key_1}.ini"  - Matches to key.Key("x").Key("x");
/// 
/// Suffix "_n" refers to the last occurance. This is also the case without an occurance number.
///  "{culture.}localization.ini"        - Matches to "fi" in: key.SetCulture("en").SetCulture("de").SetCulture("fi");
///  "{location_0/}{location_1/}{location_2/}{location_n/}location.ini 
///  
/// Regular expressions can be written between &lt; and &gt; characters to specify match criteria. \ escapes \, *, +, ?, |, {, [, (,), &lt;, &gr; ^, $,., #, and white space.
///  "{section&lt;[^:]*&gt;.}"
/// 
/// Regular expressions can be used for greedy match when matching against filenames and embedded resources.
///  "{assembly.}{resource&lt;.*&gt;.}{type.}{section.}{key}"
/// 
/// Examples:
///   "[assembly.]Resources.localization{-culture}.json"
///   "[assembly.]Resources.{type.}localization[-culture].json"
///   "Assets/{type/}localization{-culture}.ini"
///   "Assets/{assembly/}{type/}{section.}localization{-culture}.ini"
///   "{culture.}{type.}{section_0.}{section_1.}{section_2.}[section_n]{.key_0}{.key_1}{.key_n}"
/// 
/// </summary>
public interface IAssetNamePattern : IAssetKeyNamePolicy
{
    /// <summary>
    /// Pattern in string format
    /// </summary>
    string Pattern { get; }

    /// <summary>
    /// All parts of the pattern
    /// </summary>
    IAssetNamePatternPart[] AllParts { get; }

    /// <summary>
    /// All parts that capture a part of string.
    /// </summary>
    IAssetNamePatternPart[] CaptureParts { get; }
    
    /// <summary>
    /// Maps parts by identifier.
    /// </summary>
    IReadOnlyDictionary<string, IAssetNamePatternPart> PartMap { get; }

    /// <summary>
    /// List of all parameter names
    /// </summary>
    string[] ParameterNames { get; }

    /// <summary>
    /// Maps parts by parameter identifier.
    /// </summary>
    IReadOnlyDictionary<string, IAssetNamePatternPart[]> ParameterMap { get; }

    /// <summary>
    /// Match parameters from an object.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parameterReader">object that extracts parameters from object</param>
    /// <returns></returns>
    IAssetNamePatternMatch Match(object obj, IAssetKeyParametrizer parameterReader);

    /// <summary>
    /// A regular expression pattern that captures same parts from a filename string.
    /// </summary>
    Regex Regex { get; }
}
```
</details>

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is a subinterface where Build() can be implemented directly.
 * [IAssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetKeyNameProvider.cs) is implementation of IAssetNameProvider.
 * [AssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetNamePattern.cs) is the default implementation of IAssetNamePattern.
 