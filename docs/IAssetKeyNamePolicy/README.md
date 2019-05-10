# ILineFormatPolicy
<details>
  <summary><b>ILineFormatPolicy</b> is root interface for *ILine* name converter. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Signal that the class can do conversions of <see cref="ILine"/> and <see cref="String"/>.
/// 
/// User of this interface should use extensions methods 
/// <list type="bullet">
/// <item><see cref="LineNamePolicyExtensions.BuildName(ILineFormatPolicy, ILine)"/></item>
/// <item><see cref="LineNamePolicyExtensions.Parse(ILineFormatPolicy, string, ILine)"/></item>
/// </list>
/// 
/// Class that implements to this interface should implement one or both of the following interfaces:
///  <see cref="ILinePrinter"/>
///  <see cref="ILinePattern"/>
/// </summary>
public interface ILineFormatPolicy
{
}
```
</details>

<details>
  <summary><b>ILinePrinter</b> is sub-interface that prints *ILines* as *Strings*. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Converts <see cref="ILine"/> to <see cref="String"/>.
/// </summary>
public interface ILinePrinter : ILineFormatPolicy
{
    /// <summary>
    /// Build path string from key.
    /// </summary>
    /// <param name="str"></param>
    /// <returns>full name string</returns>
    string BuildName(ILine str);
}
```
</details>

<details>
  <summary><b>ILineParser</b> is sub-interface that parses *Strings* into *ILine*. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Parses <see cref="String"/> into <see cref="ILine"/>.
/// </summary>
public interface ILineParser : ILineFormatPolicy
{
    /// <summary>
    /// Parse string into key.
    /// </summary>
    /// <param name="str">key as string</param>
    /// <param name="rootKey">(optional) root key to span values from</param>
    /// <returns>key result or null if contained no content</returns>
    /// <exception cref="FormatException">If parse failed</exception>
    ILine Parse(string str, ILine rootKey = default);

    /// <summary>
    /// Parse string into key.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="key">key result or null if contained no content</param>
    /// <param name="rootKey">(optional) root key to span values from</param>
    /// <returns>true if parse was successful</returns>
    bool TryParse(string str, out ILine key, ILine rootKey = default);
}
```
</details>

<br />

| Class | ILinePrinter | ILineParser |
|:-------|:-------|:--------|
| ParameterParser.| &#9745; | &#9745; |
| LinePattern | &#9745;  | &#9745; |
| KeyPrinter | &#9745; | &#9744; |

# ParameterParser.
**ParameterParser.* is an *IAssetNameKeyPolicy* class that prints and parses keys into strings using the following notation.
```none
parameterName:parameterValue:parameterName:parameterValue:...
```

Keys are converted to strings.

```csharp
ILine key = LocalizationRoot.Global.Type("MyController").Key("Success").Culture("en");
string str = ParameterParser.Instance.BuildName(key);
```

And strings parsed to keys.

```csharp
string str = @"Culture:en:Type:MyController:Key:Ok";
ILine key = ParameterParser.Instance.Parse(str);
```

A specific *root* can be used from which the constructed key is appended from.

```csharp
string str = @"Culture:en:Type:MyController:Key:Ok";
ILine root = new StringLocalizerRoot();
ILine key = ParameterParser.Instance.Parse(str, root);
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
ILine key = ParameterParser.Instance.Parse(str);
```

# LinePattern
<details>
  <summary><b>ILinePattern</b> is interface for name patterns. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// A name pattern, akin to regular expression, that can be matched against filenames and <see cref="ILine"/> instances.
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
public interface ILinePattern : ILineFormatPolicy
{
    /// <summary>
    /// Pattern in string format
    /// </summary>
    string Pattern { get; }

    /// <summary>
    /// All parts of the pattern
    /// </summary>
    ILinePatternPart[] AllParts { get; }

    /// <summary>
    /// All parts that capture a part of string.
    /// </summary>
    ILinePatternPart[] CaptureParts { get; }
    
    /// <summary>
    /// Maps parts by identifier.
    /// </summary>
    IReadOnlyDictionary<string, ILinePatternPart> PartMap { get; }

    /// <summary>
    /// List of all parameter names
    /// </summary>
    string[] ParameterNames { get; }

    /// <summary>
    /// Maps parts by parameter identifier.
    /// </summary>
    IReadOnlyDictionary<string, ILinePatternPart[]> ParameterMap { get; }

    /// <summary>
    /// Match parameters from an object.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ILinePatternMatch Match(ILine key);

    /// <summary>
    /// A regular expression pattern that captures same parts from a filename string.
    /// </summary>
    Regex Regex { get; }
}

/// <summary>
/// Part of a pattern.
/// </summary>
public interface ILinePatternPart
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
    /// Index in <see cref="ILinePattern.AllParts"/>.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Index in <see cref="ILinePattern.CaptureParts"/>.
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
public interface ILinePatternMatch : IReadOnlyDictionary<string, string>
{
    /// <summary>
    /// Associated patern.
    /// </summary>
    ILinePattern Pattern { get; }

    /// <summary>
    /// Resolved part values.
    /// </summary>
    string[] PartValues { get; }

    /// <summary>
    /// Part values by part index in <see cref="ILinePatternPart.CaptureIndex"/>.
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

**LinePattern** is a regular-expression like pattern to print and extract parameters from keys and strings.

```csharp
// Let's create an example key
ILine key = new LocalizationRoot()
        .Location("Patches")
        .Type("MyController")
        .Section("Errors")
        .Key("InvalidState")
        .Culture("en");
```

```csharp
// Create pattern
ILineFormatPolicy myPolicy = new LinePattern("{Culture/}{Location/}{Type/}{Section/}[Key].txt");
// "en/Patches/MyController/Errors/InvalidState.txt"
string str = myPolicy.BuildName(key);
```

Name pattern consists of parameters. They are written in format of "{prefix **ParameterName** suffix}".  
Braces "{parameter/}" make parameter optional, and brackets "[parameter/]" mandatory.

```csharp
// Create pattern
ILineFormatPolicy myPolicy = new LinePattern("Patches/{Section}[-Key]{-Culture}.png");
```

Parameter can be added multiple times.

```csharp
// Create pattern
ILineFormatPolicy myPolicy = new LinePattern("{Location/}{Location/}{Location/}{Section}{-Key}{-Culture}.png");
// Create key
ILine key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
// Converts to "Patches/20181130/icons-ok-de.png"
string str = myPolicy.BuildName(key2);
```

A shorter way to add consecutive parameters is use suffix "_n". It translates to the five following occurances.
If part is required, e.g. "[parametername_n]", then only first part is required and others optional.

```csharp
// "[Location_n/]" translates to "[Location_0/]{Location_1/}{Location_2/}{Location_3/}{Location_4/}"
ILineFormatPolicy myPolicy = new LinePattern("[Location_n/]{Section}{-Key}{-Culture}.png");
// Create key
ILine key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
// Converts to "Patches/20181130/icons-ok-de.png"
string str = myPolicy.BuildName(key2);
```

Parameters need to be added in non-consecutive order, then "_#" can be used to represent the occurance index.

```csharp
// Create pattern
ILineFormatPolicy myPolicy = new LinePattern("{Location_3}{Location_2/}{Location_1/}{Location/}{Section}{-Key}{-Culture}.png");
// Create key
ILine key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
// Converts to "20181130/Patches/icons-ok-de.png"
string str = myPolicy.BuildName(key2);
```

Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}", which gives more control over matching.

```csharp
// Create pattern with regular expression detail
ILinePattern myPolicy = new LinePattern("{Location<[^/]+>/}{Section}{-Key}{-Culture}.png");
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

# KeyPrinter
**KeyPrinter** is a generic class that prints key parts into strings using various rules.

Let's create an example key.

```csharp
// Let's create an example key
ILine key = new LocalizationRoot()
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
string str1 = KeyPrinter.Default.BuildName(key);
// "en.Patches.Controllers.MyController.Errors.InvalidState"
string str2 = KeyPrinter.Dot_Dot_Dot.BuildName(key);
// "Patches:Controllers:MyController:Errors:InvalidState"
string str3 = KeyPrinter.None_Colon_Colon.BuildName(key);
// "en:Patches.Controllers.MyController.Errors.InvalidState"
string str4 = KeyPrinter.Colon_Dot_Dot.BuildName(key);
// "en:Patches:Controllers:MyController:Errors.InvalidState"
string str5 = KeyPrinter.Colon_Colon_Dot.BuildName(key);
```

Policy is created by adding rules to KeyPrinter.

```csharp
// Create a custom policy 
ILineFormatPolicy myPolicy = new KeyPrinter()
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
 * [ILineFormatPolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/ILineFormatPolicy.cs) is the root interface for classes that formulate ILine into identity string.
 * [ILinePrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/ILineFormatPolicy.cs) is a subinterface where Build() can be implemented directly.
 * [ILinePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/ILinePattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [KeyPrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/KeyPrinter.cs) is implementation of IAssetNameProvider.
 * [LinePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/LinePattern.cs) is the default implementation of ILinePattern.
 * [ParameterParser.(https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/ParameterParser.cs) is context-free string format.
