# Introduction
<details>
  <summary><b>IStringFormat</b> is interface for printers and parsers of string formats. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// String format of string value. 
/// 
/// For example C# format that uses numbered arguments "{0[,parameters]}" that are written inside braces and have
/// parameters after number.
/// 
/// It has following sub-interfaces:
/// <list type="bullet">
/// <item><see cref="IStringFormatParser"/></item>
/// </list>
/// </summary>
public interface IStringFormat
{
    /// <summary>
    /// Name of the format name, e.g. "csharp", "c", or "lexical"
    /// </summary>
    string Name { get; }
}

/// <summary>
/// Parses arguments from format strings. Handles escaping.
/// 
/// For example "You received {caridnal:0} coin(s)." is a format string
/// that parsed into argument and non-argument sections.
/// </summary>
public interface IStringFormatParser : IStringFormat
{
    /// <summary>
    /// Parse format string into an <see cref="IString"/>.
    /// 
    /// If parse fails this method should return an instance where state is <see cref="LineStatus.StringFormatErrorMalformed"/>.
    /// If parse succeeds, the returned instance has state <see cref="LineStatus.StringFormatOkString"/> or some other format state.
    /// If <paramref name="str"/> is null then stat is <see cref="LineStatus.StringFormatFailedNull"/>.
    /// </summary>
    /// <param name="str"></param>
    /// <returns>format string</returns>
    IString Parse(string str);
}

/// <summary>
/// Prints <see cref="IString"/> into the format.
/// </summary>
public interface IStringFormatPrinter : IStringFormat
{
    /// <summary>
    /// Print <paramref name="str"/> into string that represents the notation of this <see cref="IStringFormat"/>.
    /// 
    /// If print fails status is:
    /// <list type="bullet">
    ///     <item><see cref="LineStatus.StringFormatErrorPrintNoCapabilityPluralCategory"/></item>
    ///     <item><see cref="LineStatus.StringFormatErrorPrintNoCapabilityPlaceholder"/></item>
    ///     <item><see cref="LineStatus.StringFormatErrorPrintUnsupportedExpression"/></item>
    ///     <item><see cref="LineStatus.StringFormatFailed"/></item>
    /// </list>
    /// 
    /// If formulated ok, status is <see cref="LineStatus.StringFormatOkString"/>.
    /// </summary>
    /// <param name="str"></param>
    /// <returns>format string</returns>
    LineString Print(IString str);
}
```
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

```csharp
IStringFormat stringFormat = CSharpFormat.Default;
IString str = stringFormat.Parse("Hello, {0}.");
```

Extension method **.Format(<i>string</i>)** appends CSharpFormat string to ILine as "String" parameter.

```csharp
ILine line = LineRoot.Global.Key("").Format("Hello, {0}.");
```

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
**TextFormat.Default** is a string format that contains plain text without placeholders. It doesn't need or use escaping.

```csharp
IStringFormat stringFormat = TextFormat.Default;
IString str = stringFormat.Parse("{in braces}");
```

Extension method **.Text(<i>string</i>)** appends TextFormat string to ILine as "String" parameter.

```csharp
ILine line = LineRoot.Global.Key("").Text("{in braces}");
```

# Escaping
Two **IStringFormat**s can be used for escaping and unescaping.

```csharp
// Convert unescaped string "{not placeholder}" to IString
IString ss = TextFormat.Default.Parse("{not placeholder}");
// Escape with CSharpFormat to "\{not placeholder\}"
string str = CSharpFormat.Default.Print(ss);
// Convert escaped string back to IString
IString cs = CSharpFormat.Default.Parse(str);
// Unescape IString back to "{not placeholder}"
string tt = TextFormat.Default.Print(cs);
```

# Resolving string
String is parsed into *IString* that contains placeholders and texts.

```csharp
IStringFormat stringFormat = CSharpFormat.Default;
IString str = stringFormat.Parse("Hello, {0}.");
```

<b>.String(<i>string</i>)</b> extension method appends *IString* to *ILine*.

```csharp
ILine line = LineRoot.Global.Key("Hello").String(str);
```

An argument is applied to placeholder {0} and resolved string is returned in a **LineString** record.

```csharp
LineString lineString = line.Value("Corellia Melody").ResolveString();
```

If it resolved to an OK or non-Failed result, then the *.Value* can be used.

```csharp
if (!lineString.Failed) Console.WriteLine(lineString.Value);
```

