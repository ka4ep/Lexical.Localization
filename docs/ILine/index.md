# Line
**ILine** is interface for implementations that address broad variety of features.
Lines can carry hints, key reference and strings. They can be implemented with one class, but are compositions of smaller classes that are used to form linked lists.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]
![linked list](linkedlist.svg)
<br/>

Or constructed to span a tree structure (trie).
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]
![tree](tree.svg)
<br/>

# Parts
<b>.Asset(<i>IAsset</i>)</b> adds extra asset as line part.
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]

<b>.Logger(<i>ILocalizationLogger</i>)</b> adds logger as line part.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]

<b>.CulturePolicy(<i>ICulturePolicy</i>)</b> adds culture policy that determines the active culture in current execution context.	
[!code-csharp[Snippet](Examples.cs#Snippet_2c)]

# Hints
Hints are line parts that can be appended from localization file as well as from *ILine*.

<b>.StringFormat(<i>IStringFormat</i>)</b> and <b>.StringFormat(<i>string</i>)</b> add a string format, that determines the way the consequtive "String" parameters are parsed.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]

<b>.PluralRules(<i>IPluralRules</i>)</b> and <b>.PluralRules(<i>string</i>)</b> add plural rules that determine how plurality are used in further line parts.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]

<b>.FormatProvider(<i>IFormatProvider</i>)</b> and <b>.FormatProvider(<i>string</i>)</b> add a custom format provider that provide special format handling.
[!code-csharp[Snippet](Examples.cs#Snippet_3c)]
[!code-csharp[Snippet](Examples.cs#Snippet_3d)]

<b>.StringResolver(<i>IStringResolver</i>)</b> adds *IStringResolver* as line part.
[!code-csharp[Snippet](Examples.cs#Snippet_3e)]

<b>.StringResolver(<i>string</i>)</b> adds assembly qualified class name to *IStringResolver*.
[!code-csharp[Snippet](Examples.cs#Snippet_3f)]

<b>.ResourceResolver(<i>IResourceResolver</i>)</b> adds *IResourceResolver* as line part.
[!code-csharp[Snippet](Examples.cs#Snippet_3g)]

<b>.ResourceResolver(<i>string</i>)</b> adds assembly qualified class name to *IResourceResolver*.
[!code-csharp[Snippet](Examples.cs#Snippet_3h)]

# Canonically compared keys
Key parts determine how *ILine*s are hash-equally compared.

Canonically compared key parts are compared so that the occurance position of the key parts are relevant.

<b>.Assembly(<i>Assembly</i>)</b> and <b>.Assembly(<i>string</i>)</b> append "Assembly" key. 
[!code-csharp[Snippet](Examples.cs#Snippet_5a)]

<b>.Culture(<i>CultureInfo</i>)</b> and <b>.Culture(<i>string</i>)</b> append "Culture" key.
[!code-csharp[Snippet](Examples.cs#Snippet_5b)]

<b>.Type&lt;T&gt;(<i></i>)</b>, <b>.Type(<i>Type</i>)</b> and <b>.Type(<i>string</i>)</b> append "Type" key.
[!code-csharp[Snippet](Examples.cs#Snippet_5c)]

# Non-canonically compared keys
Non-canonically compared key parts are compared so that the position doesn't matter. The first occurance of each type is considered effective, rest are ignored.

<b>.Key(<i>string</i>)</b> appends "Key" key part.
[!code-csharp[Snippet](Examples.cs#Snippet_6a)]

<b>.Section(<i>string</i>)</b> appends "Section" key part.
[!code-csharp[Snippet](Examples.cs#Snippet_6b)]

<b>.Location(<i>string</i>)</b> appends "Location" key part.
[!code-csharp[Snippet](Examples.cs#Snippet_6c)]

<b>.BaseName(<i>string</i>)</b> appends "BaseName" key part.
[!code-csharp[Snippet](Examples.cs#Snippet_6d)]

# Strings
<b>.String(<i>IString</i>)</b> appends preparsed default string value.
[!code-csharp[Snippet](Examples.cs#Snippet_7a)]

<b>.String(<i>string</i>)</b> appends "String" hint. This needs preceding "StringFormat" part in order to determine the way to parse.
If "StringFormat" is not provided, then C# format is used as default.
[!code-csharp[Snippet](Examples.cs#Snippet_7b)]

<b>.Format(<i>string</i>)</b> appends C# String formulation value.
[!code-csharp[Snippet](Examples.cs#Snippet_7c)]

<b>.Text(<i>string</i>)</b> appends plain text "String" value.
[!code-csharp[Snippet](Examples.cs#Snippet_7d)]

<b>.ResolveString()</b> resolves the string in current executing context. The result struct is <b>LineString</b>.
[!code-csharp[Snippet](Examples.cs#Snippet_8a)]

# Inlining
Code can be [automatically scanned](http://lexical.fi/sdk/Localization/docs/InlineScanner/index.html) for inlined strings and exported to localization files.
They can be used as templates for further translation process. 
This way the templates don't need to be manually updated as the code evolves.
<br/>

<b>.Inline(<i>string</i> subkey, <i>string</i> text)</b> appends an inlined sub-line.
[!code-csharp[Snippet](Examples.cs#Snippet_7e)]

<b>.en(<i>string</i>)</b> appends inlined value for that culture "en".
[!code-csharp[Snippet](Examples.cs#Snippet_7f)]

It's recommended to put inlined lines to variables for better performance. Inline allocates a Dictionary internally.
[!code-csharp[Snippet](Examples.cs#Snippet_7h)]

# Resources
<b>.ResolveBytes()</b> resolves the line to bytes in the current executing context. The result struct is <b>LineResourceBytes</b>.
[!code-csharp[Snippet](Examples.cs#Snippet_10a)]

<b>.ResolveStream()</b> resolves the line to string in the current executing context. The result struct is <b>LineResourceStream</b>.
[!code-csharp[Snippet](Examples.cs#Snippet_10b)]

# Use in classes
If class is designed to support dependency injection without string localizers, the constructors should 
take in argument *ILine&lt;T&gt;* and possibly *ILineRoot*. See more in [Best Practices](../BestPractices/ClassLibrary/index.md).
Constructor argument **ILine&lt;T&gt;** helps the Dependency Injection to assign the localization so that it is scoped in to correct typesection.
[!code-csharp[Snippet](Examples.cs#Snippet_9a)]

If class is designed to use static instance and without dependency injection, localization reference can be acquired from **LineRoot**.
[!code-csharp[Snippet](Examples.cs#Snippet_9b)]
