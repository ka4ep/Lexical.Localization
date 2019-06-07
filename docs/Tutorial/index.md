# Tutorial
**ILine** is a key to localization asset. It can also be embedded with a default string.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

Values are provided with <b>.Value(<i>object[]</i>)</b>.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

Providing **.Format()** and **.Value()** is equivalent to **String.Format()**.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]

Default strings can be *inlined* to multiple cultures.
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

Localization can be logged with various kinds of loggers.
[!code-csharp[Snippet](Examples.cs#Snippet_7)]

Different string formats, such as C#'s *String.Format*, are supported. 
[!code-csharp[Snippet](Examples.cs#Snippet_8)]

**ICulturePolicy** determines which culture to apply.
[!code-csharp[Snippet](Examples.cs#Snippet_9)]

**Links**
* [Website](http://lexical.fi/Localization/index.html)
* [Github](https://github.com/tagcode/Lexical.Localization)
* [Nuget](https://www.nuget.org/packages/Lexical.Localization/)
