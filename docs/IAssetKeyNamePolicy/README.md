# IAssetKeyNamePolicy
<details>
  <summary><b>IAssetKeyNamePolicy</b> is root interface for *IAssetKey* name converter. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Signal that the class can do conversions of <see cref="IAssetKey"/> and <see cref="String"/>.
/// 
/// User of this interface should call <see cref="AssetKeyNamePolicyExtensions.BuildName(IAssetKeyNamePolicy, IAssetKey)"/>.
/// 
/// Class that imlpements to this interface should implement one or both of the following interfaces:
///  <see cref="IAssetKeyNameProvider"/>
///  <see cref="IAssetNamePattern"/>
/// </summary>
public interface IAssetKeyNamePolicy
{
}
```
</details>

<details>
  <summary><b>IAssetKeyNameProvider</b> is sub-interface that prints *IAssetKeys* as *Strings*. (<u>Click here</u>)</summary>

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

<details>
  <summary><b>IAssetKeyNameParser</b> is sub-interface that parses *Strings* into *IAssetKey*. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Parses <see cref="String"/> into <see cref="IAssetKey"/>.
/// </summary>
public interface IAssetKeyNameParser : IAssetKeyNamePolicy
{
    /// <summary>
    /// Parse string into key.
    /// </summary>
    /// <param name="str">key as string</param>
    /// <param name="rootKey">(optional) root key to span values from</param>
    /// <returns>key result or null if contained no content</returns>
    /// <exception cref="FormatException">If parse failed</exception>
    IAssetKey Parse(string str, IAssetKey rootKey = default);

    /// <summary>
    /// Parse string into key.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="key">key result or null if contained no content</param>
    /// <param name="rootKey">(optional) root key to span values from</param>
    /// <returns>true if parse was successful</returns>
    bool TryParse(string str, out IAssetKey key, IAssetKey rootKey = default);
}
```
</details>

<br />

| Class | IAssetKeyNameProvider | IAssetKeyNameParser |
|:-------|:-------|:--------|
| ParameterNamePolicy | &#9745; | &#9745; |
| AssetNamePattern | &#9745;  | &#9745; |
| AssetKeyNameProvider | &#9745; | &#9744; |

# ParameterNamePolicy
**ParameterNamePolicy** is an *IAssetNameKeyPolicy* class that prints and parses keys into strings without 
contextual information.
```none
parameterName:parameterValue:parameterName:parameterValue:...
```

Keys are converted to strings.

```csharp
IAssetKey key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
string str = ParameterNamePolicy.Instance.BuildName(key);
```

And strings parsed to keys.

```csharp
string str = @"Culture:en:Type:MyController:Key:Ok";
IAssetKey key = ParameterNamePolicy.Instance.Parse(str);
```

A specific *root* can be used from which the constructed key is appended from.

```csharp
string str = @"Culture:en:Type:MyController:Key:Ok";
IAssetKey root = new StringLocalizerRoot();
IAssetKey key = ParameterNamePolicy.Instance.Parse(str, root);
```

Policy uses the following escape rules.

| Sequence | Meaning |
|:---------|:--------|
| \\: | Colon |
| \\t | Tab |
| \\r | Carriage return |
| \\n | New line |
| \\x<i>hh</i> | Unicode 8bit |
| \\u<i>hhhh</i> | Unicode 16bit (surrogate) |
| \\U<i>hhhhhhhh</i> | Unicode 32bit |

Example of escaped key "Success\\:Plural".

```csharp
string str = @"Key:Success\:Plural";
IAssetKey key = ParameterNamePolicy.Instance.Parse(str);
```

# AssetNamePattern
<details>
  <summary><b>IAssetNamePattern</b> is interface for name patterns. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// A name pattern, akin to regular expression, that can be matched against filenames and <see cref="IAssetKey"/> instances.
/// Is a sequence of parameter and text parts.
/// 
/// Parameter parts:
///  {Culture}           - Matches to key.Culture("en")
///  {Assembly}          - Matches to key.Assembly(asm).
///  {Resource}          - Matches to key.Resource("xx").
///  {Type}              - Matches to key.Type(type)
///  {Section}           - Matches to key.Section("xx")
///  {Location}          - Matches to key.Location("xx") and a physical folder, separator is '/'.
///  {anysection}        - Matches to assembly, type and section.
///  {Key}               - Matches to key key.Key("x")
/// 
/// Before and after the part pre- and postfix separator characters can be added:
///  {/Culture.}
///  
/// Parts can be optional in curly braces {} and required in brackets [].
///  [Culture]
/// 
/// Part can be added multiple times
///  "{Location/}{Location/}{Location/}{Key}"  - Matches to, from 0 to 3 occurances of Location(), e.g. key.Location("dir").Location("dir1");
/// 
/// If parts need to be matched out of order, then occurance index can be used "_number".
///  "{Location_2/}{Location_1/}{Location_0/}{Key}"  - Matches to, from 0 to 3 occurances of Location, e.g. key.Location("dir").Location("dir1");
/// 
/// Suffix "_n" translates to five conscutive parts.
///  "[Location_n/]location.ini" translates to "[Location_0/]{Location_1/}{Location_2/}{Location_3/}{Location_4/}"
///  "[Location/]{Location_n/}location.ini" translates to "[Location_0/]{Location_1/}{Location_2/}{Location_3/}{Location_4/}{Location_5/}"
///  
/// Regular expressions can be written between &lt; and &gt; characters to specify match criteria. \ escapes \, *, +, ?, |, {, [, (,), &lt;, &gt; ^, $,., #, and white space.
///  "{Section&lt;[^:]*&gt;.}"
/// 
/// Regular expressions can be used for greedy match when matching against filenames and embedded resources.
///  "{Assembly.}{Resource&lt;.*&gt;.}{Type.}{Section.}{Key}"
/// 
/// Examples:
///   "[Assembly.]Resources.localization{-Culture}.json"
///   "[Assembly.]Resources.{Type.}localization[-Culture].json"
///   "Assets/{Type/}localization{-Culture}.ini"
///   "Assets/{Assembly/}{Type/}{Section.}localization{-Culture}.ini"
///   "{Culture.}{Type.}{Section_0.}{Section_1.}{Section_2.}[Section_n]{.Key_0}{.Key_1}{.Key_n}"
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
    /// <param name="key"></param>
    /// <returns></returns>
    IAssetNamePatternMatch Match(IAssetKey key);

    /// <summary>
    /// A regular expression pattern that captures same parts from a filename string.
    /// </summary>
    Regex Regex { get; }
}

/// <summary>
/// Part of a pattern.
/// </summary>
public interface IAssetNamePatternPart
{
    /// <summary>
    /// Text that represents this part in pattern.
    /// for "_n" part, the first part has "_n" in PatternText, and the rest have "".
    /// </summary>
    string PatternText { get; }

    /// <summary>
    /// Part identifier, unique in context of Pattern.CaptureParts.
    /// The first occurance is the "ParameterName" as is, and succeeding have underscore and index "ParameterName_#" starting with index '1'.
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Separator
    /// </summary>
    string PrefixSeparator { get; }

    /// <summary>
    /// Separator
    /// </summary>
    string PostfixSeparator { get; }

    /// <summary>
    /// Parameter identifier. Does not include occurance index, e.g. "_1".
    /// </summary>
    string ParameterName { get; }
    /// <summary>
    /// If set, then is non-matchable Text part.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Is part mandatory
    /// </summary>
    bool Required { get; }

    /// <summary>
    /// Index in <see cref="IAssetNamePattern.AllParts"/>.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Index in <see cref="IAssetNamePattern.CaptureParts"/>.
    /// </summary>
    int CaptureIndex { get; }

    /// <summary>
    /// The order of occurance to capture against.
    /// 
    /// As special case Int32.MaxValue means the last occurance "{.Section}"
    /// 
    /// For example "{.Section_0}" captures first occurance, and the part's OccuranceIndex = 0.
    ///             "{.Section}" captures the last occurance overriding possible ordered occurance if there is only one match.
    /// </summary>
    int OccuranceIndex { get; }

    /// <summary>
    /// Regex pattern for this part.
    /// </summary>
    Regex Regex { get; }

    /// <summary>
    /// Tests if text is match.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    bool IsMatch(string text);
}
    
/// <summary>
/// Match result.
/// </summary>
public interface IAssetNamePatternMatch : IReadOnlyDictionary<string, string>
{
    /// <summary>
    /// Associated patern.
    /// </summary>
    IAssetNamePattern Pattern { get; }

    /// <summary>
    /// Resolved part values.
    /// </summary>
    string[] PartValues { get; }

    /// <summary>
    /// Part values by part index in <see cref="IAssetNamePatternPart.CaptureIndex"/>.
    /// </summary>
    /// <param name="ix"></param>
    /// <returns></returns>
    string this[int ix] { get; }

    /// <summary>
    /// Get part value by part identifier.
    /// </summary>
    /// <param name="identifier">identifier, e.g. "Culture", "Type"</param>
    /// <returns>value or null</returns>
    new string this[string identifier] { get; }

    /// <summary>
    /// Where all required parts found.
    /// </summary>
    bool Success { get; }
}
```
</details>
<br />

**AssetNamePattern** is a regular-expression like pattern to print and extract parameters from keys and strings.

```csharp
// Let's create an example key
IAssetKey key = new LocalizationRoot()
        .Location("Patches")
        .Type("MyController")
        .Section("Errors")
        .Key("InvalidState")
        .Culture("en");
```

```csharp
// Create pattern
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{Culture/}{Location/}{Type/}{Section/}[Key].txt");
// "en/Patches/MyController/Errors/InvalidState.txt"
string str = myPolicy.BuildName(key);
```

Name pattern consists of parameters. They are written in format of "{prefix **ParameterName** suffix}".  
Braces "{parameter/}" make parameter optional, and brackets "[parameter/]" mandatory.

```csharp
// Create pattern
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("Patches/{Section}[-Key]{-Culture}.png");
```

Parameter can be added multiple times.

```csharp
// Create pattern
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{Location/}{Location/}{Location/}{Section}{-Key}{-Culture}.png");
// Create key
IAssetKey key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
// Converts to "Patches/20181130/icons-ok-de.png"
string str = myPolicy.BuildName(key2);
```

A shorter way to add consecutive parameters is use suffix "_n". It translates to the five following occurances.
If part is required, e.g. "[parametername_n]", then only first part is required and others optional.

```csharp
// "[Location_n/]" translates to "[Location_0/]{Location_1/}{Location_2/}{Location_3/}{Location_4/}"
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("[Location_n/]{Section}{-Key}{-Culture}.png");
// Create key
IAssetKey key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
// Converts to "Patches/20181130/icons-ok-de.png"
string str = myPolicy.BuildName(key2);
```

Parameters need to be added in non-consecutive order, then "_#" can be used to represent the occurance index.

```csharp
// Create pattern
IAssetKeyNamePolicy myPolicy = new AssetNamePattern("{Location_3}{Location_2/}{Location_1/}{Location/}{Section}{-Key}{-Culture}.png");
// Create key
IAssetKey key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
// Converts to "20181130/Patches/icons-ok-de.png"
string str = myPolicy.BuildName(key2);
```

Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}", which gives more control over matching.

```csharp
// Create pattern with regular expression detail
IAssetNamePattern myPolicy = new AssetNamePattern("{Location<[^/]+>/}{Section}{-Key}{-Culture}.png");
// Use its regular expression
Match match = myPolicy.Regex.Match("patches/icons-ok-de.png");
```

## Parameters
Reserved parameter names and respective extension methods.

| Parameter | Key Method  | Description |
|----------|:--------|:------------|
| Assembly | .Assembly(*string*) | Assembly name |
| Location | .Location(*string*) | Subdirectory in local files |
| Resource | .Resource(*string*) | Subdirectory in embedded resources |
| Type | .Type(*string*) | Class name |
| Section | .Section(*string*) | Generic section, used for grouping |
| anysection | *all above* | Matches to any section above. |
| Culture  | .Culture(*string*) | Culture |
| Key | .Key(*string*) | Key name |
| N | .N(*Type*) | Plurality key |

# AssetKeyNameProvider
**AssetKeyNameProvider** is a generic class that prints key parts into strings using various rules.

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

Policy is created by adding rules to AssetKeyNameProvider.

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

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is a subinterface where Build() can be implemented directly.
 * [IAssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetKeyNameProvider.cs) is implementation of IAssetNameProvider.
 * [AssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetNamePattern.cs) is the default implementation of IAssetNamePattern.
 * [ParameterNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/ParameterNamePolicy.cs) is context-free string format.
