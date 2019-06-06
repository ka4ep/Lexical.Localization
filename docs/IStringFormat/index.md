# Introduction
<details>
  <summary><b>IStringFormat</b> is interface for printers and parsers of string formats. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/StringFormat/IStringFormat.cs#Interface)]
</details>
<br />

Implementations
| Class | Description | Extension Method |
|:-------|:-------|:-------|
| CSharpFormat | Uses C# String.Format() notation. | <i>ILine</i>.Format(string) |
| TextFormat | Uses plain text notation. | <i>ILine</i>.String(string) |

#CSharpFormat
**CSharpFormat.Default** represents a string format that is similar to **String.Format**.

<b>CSharpFormat.Default.Parse(<i>string</i>)</b> parses placeholders from a string.
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]

Extension method **.Format(<i>string</i>)** appends CSharpFormat string to ILine as "String" parameter.
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

**CSharpFormat** uses following format.
  "Text {<i>[pluralCategory:]argumentIndex:[,alignment][:format]</i>} text".

It uses the following rules:
1. Placeholders are inside braces and contain numbered arguments:
   "Hello {0} and {1}"
2. Braces are escaped.
   "Control characters are \\{\\}."
3. "format"-specifier is after <b>:</b> colon.
   "Hex value {0:X4}."
4. <i>PluralCategory</i> is before <b>:</b> colon.

# TextFormat
**TextFormat.Default** is a string format that contains plain text without placeholders. It doesn't use escaping.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

Extension method **.Text(<i>string</i>)** appends TextFormat string to ILine as "String" parameter.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

# Escaping
Two **IStringFormat**s can be used for escaping and unescaping.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

# Resolving string
String is parsed into *IString* that contains placeholders and texts.
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]

<b>.String(<i>string</i>)</b> extension method appends *IString* to *ILine*.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]

An argument is applied to placeholder {0} and resolved string is returned in a **LineString** record.
[!code-csharp[Snippet](Examples.cs#Snippet_2c)]

If it resolved to an OK result, then the *.Value* can be used.
[!code-csharp[Snippet](Examples.cs#Snippet_2d)]

