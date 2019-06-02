# Key
**Key** is context free format of a localization key. Essentially it is an array of keys and values. 
*Key* is used as an internal class for serializing localization files and for comparing context-dependent keys.

```csharp
ILine key = LineAppender.NonResolving.Culture("en").Type("MyController").Key("Success");
```

Key can be converted to context dependent key ILine with a parametrizer.

```csharp

```

# Links
 * [Key](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Internal/Key.cs) is context-free key format.
