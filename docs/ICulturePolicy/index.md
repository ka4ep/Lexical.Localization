# Culture Policy
<details>
  <summary><b>ICulturePolicy</b> is an interface for controlling the active culture and the fallback culture(s). (<u>click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/CulturePolicy/ICulturePolicy.cs#ICulturePolicy)]
</details>

<details>
  <summary><b>ICulturePolicyAssignable</b> is interface for classes where policy can be modified. (<u>click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/CulturePolicy/ICulturePolicy.cs#ICulturePolicyAssignable)]
</details>
<br/>

The default implementation is **CulturePolicy**. 
[!code-csharp[Snippet](Examples.cs#Snippet_0)]

ICulturePolicy is assigned to localization root from where if affects the constructed keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

Direct way to use *CulturePolicy* is to assign prefered culture and fallback culture on the provider instance.
Typical fallback culture is the root culture "".
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]

They can also be assigned as strings.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]

**.SetCultureWithFallbackCultures()** assigns culture and its default fallback cultures.
[!code-csharp[Snippet](Examples.cs#Snippet_2c)]

Culture policy can be configured to use **CultureInfo.CurrentCulture**. 
Active culture can be controlled from that field.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]
And **CultureInfo.CurrentUICulture**.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]

And also current thread's culture.
[!code-csharp[Snippet](Examples.cs#Snippet_4a)]
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]

**.SetFunc()** assigns a delegate that uses the returned *CultureInfo*.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]

**.SetSourceFunc()** assigns a delegate that uses the returned *ICulturePolicy*.
[!code-csharp[Snippet](Examples.cs#Snippet_6)]

**.ToSnapshot()** takes an array snapshot of the source, fixing its value.
**.AsReadonly()** creates a new policy where the enumerable cannot be changed. 
Together these two make an immutable policy.
[!code-csharp[Snippet](Examples.cs#Snippet_7)]

# Resolve Examples
**LineRoot.Global** has a default CulturePolicy that uses the current thread's culture.
[!code-csharp[Snippet](Examples.cs#Snippet_8a)]

This is similiar to C#'s **String.Format**.
[!code-csharp[Snippet](Examples.cs#Snippet_8c)]

When explicit culture is appended to the ILine, then it overrides the culture in the ICulturePolicy.
[!code-csharp[Snippet](Examples.cs#Snippet_8b)]

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [ICulturePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/CulturePolicy/ICulturePolicy.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [CulturePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/CulturePolicy/CulturePolicy.cs)
 