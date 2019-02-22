# Asset Key Parametrizer
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

# Asset Key Parametrizer
The context free format of a key is an array of keys and values **IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;**.

**AssetKeyParametrizer** converts implementations of *IAssetKey* to *IEnumerable*

```csharp
// Parametrizer for AssetKey
IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
// Create context-dependent key
IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
// Convert to context-free parameters
IEnumerable<KeyValuePair<string, string>> parameters = parametrizer.GetAllParameters(key).ToArray();
```

And back to IAssetKey.

```csharp
// Convert to context-free parameters
IEnumerable<KeyValuePair<string, string>> parameters =
    new Key("culture", "en")
    .Append("type", "MyLibrary:Type").Append("key", "\"hello\"");
// Parametrizer for AssetKey
IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
// Convert to context-dependent instance
object key = LocalizationRoot.Global;
foreach (var parameter in parameters)
    key = parametrizer.CreatePart(key, parameter.Key, parameter.Value);
// Type-cast
IAssetKey key_ = (IAssetKey)key;
```
 