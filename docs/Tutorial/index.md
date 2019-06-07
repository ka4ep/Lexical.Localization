# Tutorial
**ILine** is a localization line. It can be embedded with a default string.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

Values are provided with <b>.Value(<i>object[]</i>)</b>.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

Providing **.Format()** and **.Value()** is equivalent to **String.Format()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]

Default strings can be *inlined* for multiple cultures.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

And *inlined* with different plurality cases.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

And with permutations of different cultures and plurality cases.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

Localization assets can be read from files.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]
<details>
  <summary>PluralityExample0a.xml (<u>click here</u>)</summary>
[!code-xml[Snippet](../PluralityExample0a.xml)]
</details>
<br/>

**IAsset** is an abstraction to localization lines and localized resources. 
Implementing classes can be provided within code.
[!code-csharp[Snippet](Examples.cs#Snippet_6)]

Loggers can be appended to *ILine* for debugging purposes.
[!code-csharp[Snippet](Examples.cs#Snippet_7)]

Different string formats, such as C#'s *String.Format*, are supported. **IStringFormat** is an abstraction to string formats.
[!code-csharp[Snippet](Examples.cs#Snippet_8)]

**ICulturePolicy** determines which culture to apply.
[!code-csharp[Snippet](Examples.cs#Snippet_9)]

*ILine.ToString()* is a shortcut to <b><i>ILine</i>.ResolveString()</b>, which returns with additional information about the resolve process. 
[!code-csharp[Snippet](Examples.cs#Snippet_1d)]

**Links**
* [Website](http://lexical.fi/Localization/index.html)
* [Github](https://github.com/tagcode/Lexical.Localization)
* [Nuget](https://www.nuget.org/packages/Lexical.Localization/)
