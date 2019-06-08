# Format and Values
**ILine** is a localization line. It can be embedded with a default string.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

Values are provided with <b>.Value(<i>object[]</i>)</b>.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

Providing **.Format()** and **.Value()** is equivalent to **String.Format()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]

<b>.Format(<i>$interpolated_string</i>)</b> also creates **.Format()** and **.Value()** parts.
[!code-csharp[Snippet](Examples.cs#Snippet_1c1)]

# Culture
The format culture can be enforced with <b>.Culture(<i>CultureInfo</i>)</b>, without changing the thread-local culture variable.
[!code-csharp[Snippet](Examples.cs#Snippet_1c2)]

# Inlining
Default strings can be *inlined* for multiple cultures.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

And *inlined* with different plurality cases.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

And with permutations of different cultures and plurality cases.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

# Files
Localization assets can be read from files and placed into the global **LineRoot.Global**.
[!code-csharp[Snippet](Examples.cs#Snippet_5a)]
<details>
  <summary>PluralityExample0a.xml (<u>click here</u>)</summary>
[!code-xml[Snippet](../PluralityExample0a.xml)]
</details>
<br/>

Or, assets can be loaded and placed into a new ILineRoot.
[!code-csharp[Snippet](Examples.cs#Snippet_5b)]

**IAsset** is an abstraction to localization lines and localized resources. 
Implementing classes can be provided within code.
[!code-csharp[Snippet](Examples.cs#Snippet_6)]

# Logging
Loggers can be appended to *ILine* for debugging purposes.
[!code-csharp[Snippet](Examples.cs#Snippet_7)]

# StringFormat
Different string formats, such as C#'s *String.Format*, are supported. **IStringFormat** is an abstraction to string formats.
[!code-csharp[Snippet](Examples.cs#Snippet_8)]

# Culture Policy
**ICulturePolicy** determines which culture to apply.
[!code-csharp[Snippet](Examples.cs#Snippet_9)]

# Resolving string
<b><i>ILine</i>.ResolveString()</b> returns more information about the string resolve process than *ILine.ToString()*.
[!code-csharp[Snippet](Examples.cs#Snippet_1d)]

# String Localizer
**StringLocalizerRoot.Global** is same root as **LineRoot.Global** with the difference, that parts derived from it implement *IStringLocalizer* and *IStringLocalizerFactory*.
[!code-csharp[Snippet](Examples.cs#Snippet_10)]

New **StringLocalizerRoot** can also be constructed.
[!code-csharp[Snippet](Examples.cs#Snippet_11)]

*IStringLocalizer* reference can be adapted from regular **LineRoot** as well, but causes an additional heap object to be instantiated.
[!code-csharp[Snippet](Examples.cs#Snippet_12)]

# Example Class
Keys can be placed in static references if the singleton **LineRoot.Global** is used.
[!code-csharp[Snippet](ExampleClass.cs#Snippet)]

# Links
* [Website](http://lexical.fi/Localization/index.html)
* [Github](https://github.com/tagcode/Lexical.Localization)
* [Nuget](https://www.nuget.org/packages/Lexical.Localization/)
