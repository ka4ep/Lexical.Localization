# Key
**Key** is context free format of a localization key. Essentially it is an array of keys and values. 
*Key* is used as an internal class for serializing localization files and for comparing context-dependent keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

Key can be converted to context dependent key ILine with a parametrizer.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]

# Links
 * [Key](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Internal/Key.cs) is context-free key format.
