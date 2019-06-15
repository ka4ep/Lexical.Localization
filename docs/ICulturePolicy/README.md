# Culture Policy
<details>
  <summary><b>ICulturePolicy</b> is an interface for controlling the active culture and the fallback culture(s). (<u>click here</u>)</summary>

```csharp
/// <summary>
/// Interface for policy that returns active culture policy, and fallback cultures.
/// </summary>
public interface ICulturePolicy
{
    /// <summary>
    /// Array property returns the prefered culture as first element.
    /// Other cultures are considered fallback cultures.
    /// 
    /// For example: "en-UK", "en", "".
    /// </summary>
    CultureInfo[] Cultures { get; }
}
```
</details>

<details>
  <summary><b>ICulturePolicyAssignable</b> is interface for classes where policy can be modified. (<u>click here</u>)</summary>

```csharp
/// <summary>
/// Interface for culture policy where culture is assignable.
/// </summary>
public interface ICulturePolicyAssignable : ICulturePolicy
{
    /// <summary>
    /// Set source of cultures. The first element is active culture, others fallback cultures.
    /// </summary>
    /// <param name="cultureSource"></param>
    /// <returns></returns>
    ICulturePolicyAssignable SetSource(ICulturePolicy cultureSource);
}
```
</details>
<br/>

The default implementation is **CulturePolicy**. 

```csharp
// Create policy
ICulturePolicyAssignable culturePolicy = new CulturePolicy();
```

ICulturePolicy is assigned to localization root from where if affects the constructed keys.

```csharp
// Create localization source
var source = new Dictionary<string, string> {
    { "MyController:hello", "Hello World!" },
    { "en:MyController:hello", "Hello World!" },
    { "de:MyController:hello", "Hallo Welt!" }
};
// Create asset with culture policy
IAsset asset = new StringAsset(source, LineParameterPrinter.Default);
// Create root and assign culturePolicy
ILineRoot root = new LineRoot(asset, culturePolicy);
```

Direct way to use *CulturePolicy* is to assign prefered culture and fallback culture on the provider instance.
Typical fallback culture is the root culture "".

```csharp
// Set active culture and set fallback culture
ICulturePolicy cultureArray_ =
    new CulturePolicy().SetCultures(
        CultureInfo.GetCultureInfo("en-US"),
        CultureInfo.GetCultureInfo("en"),
        CultureInfo.GetCultureInfo("")
    );
```

They can also be assigned as strings.

```csharp
// Create policy from array of cultures
ICulturePolicy culturePolicy = new CulturePolicy().SetCultures("en-US", "en", "");
```

**.SetCultureWithFallbackCultures()** assigns culture and its default fallback cultures.

```csharp
// Create policy from culture, adds fallback cultures "en" and "".
ICulturePolicy culturePolicy = new CulturePolicy().SetCultureWithFallbackCultures("en-US");
```

Culture policy can be configured to use **CultureInfo.CurrentCulture**. 
Active culture can be controlled from that field.

```csharp
// Set to use CultureInfo.CurrentCulture
ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentCulture();
// Change current culture
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
```
And **CultureInfo.CurrentUICulture**.

```csharp
// Set to use CultureInfo.CurrentCulture
ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentUICulture();
// Change current culture
CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
```

And also current thread's culture.

```csharp
// Set to use CultureInfo.CurrentCulture
ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentThreadCulture();
// Change current thread's culture
Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
```

```csharp
// Set to use CultureInfo.CurrentCulture
ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentThreadUICulture();
// Change current thread's culture
Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
```

**.SetFunc()** assigns a delegate that uses the returned *CultureInfo*.

```csharp
// Assign delegate 
ICulturePolicy culturePolicy = new CulturePolicy().SetFunc(() => CultureInfo.GetCultureInfo("fi"));
```

**.SetSourceFunc()** assigns a delegate that uses the returned *ICulturePolicy*.

```csharp
// Assign delegate 
ICulturePolicy source = new CulturePolicy().SetToCurrentUICulture();
ICulturePolicy culturePolicy = new CulturePolicy().SetSourceFunc(() => source);
```

**.ToSnapshot()** takes an array snapshot of the source, fixing its value.
**.AsReadonly()** creates a new policy where the enumerable cannot be changed. 
Together these two make an immutable policy.

```csharp
// Freeze current culture
ICulturePolicy culturePolicy = new CulturePolicy()
    .SetToCurrentCulture()
    .ToSnapshot()
    .AsReadonly();
```

# Resolve Examples
**LineRoot.Global** has a default CulturePolicy that uses the current thread's culture.

```csharp
// Default CulturePolicy
Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Value(DateTime.Now));
```

This is similiar to C#'s **String.Format**.

```csharp
// C#'s String.Format uses Thread.CurrentCulture
Console.WriteLine(String.Format("It is now {0:d} at {0:t}", DateTime.Now));
```

When explicit culture is appended to the ILine, then it overrides the culture in the ICulturePolicy.

```csharp
// Format uses explicit CultureInfo "fi"
Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Value(DateTime.Now).Culture("fi"));
// Format uses explicit CultureInfo "sv"
Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Value(DateTime.Now).Culture("sv"));
// Format uses explicit CultureInfo "en"
Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Value(DateTime.Now).Culture("en"));
```

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [ICulturePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/CulturePolicy/ICulturePolicy.cs)
 * [ICulturePolicyAssignable](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/CulturePolicy/ICulturePolicy.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [CulturePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/CulturePolicy/CulturePolicy.cs)
 * [CulturePolicyImmutable](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/CulturePolicy/CulturePolicyImmutable.cs)
 * [CulturePolicyExtensions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/CulturePolicy/CulturePolicyExtensions.cs)
