# Introduction
<details>
  <summary><b>Lexical.Localization.Common.ILogger</b> is the root interface for loggers. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Common/ILogger.cs#ILogger)]
</details>
<details>
  <summary><b>Lexical.Localization.StringFormat.IStringResolverLogger</b> is for logging string resolve results. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Common/ILogger.cs#IStringResolverLogger)]
</details>
<details>
  <summary><b>Lexical.Localization.Resource.IResourceResolverLogger</b> is for logging resource resolve results. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Common/ILogger.cs#IResourceResolverLogger)]
</details>
<br />

**LineStatusSeverity** has four severity levels.

| Level | Description |
|:-------|:-------|
| **Ok** | OK value. |
| **Warning** | Warning, but produced a value. |
| **Error** | Error occured, but produced a fallback value. |
| **Failed** | Failed, no value. |

If **LineStatusSeverity.OK** is used, then every result is logged.
If **LineStatusSeverity.Failed** then, only when no result could be produced.

# TextWriter
Logger can output to **TextWriter** such as **Console.Out**.
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]

Resolving a string causes logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

The logger output.
```none
ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
```

# Diagnostics Trace
Logger can output to System.Diagnostics.Trace.
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]

Resolving a string causes the logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]

The logger output.
```none
docs Information: 0 : ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
```

# ILogger
<b>.ILogger(<i>ILoggerFactory</i>)</b> appends **Microsoft.Extensions.Logger.ILoggerFactory** to a line.
This forwards the *Type* from the line to logger.
[!code-csharp[Snippet](Examples.cs#Snippet_4a)]

<b>.ILogger(<i>ILogger</i>)</b> appends **Microsoft.Extensions.Logger.ILogger** to a line.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]

Resolving a string causes logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]

The logger output.
```none
info: MyClass[0]
ResolveOkFromKey | CultureOkMatchedNoCulture | PluralityOkNotUsed | StringFormatOkString Type: MyClass: Key: OK = "OK"
```

# NLog
<b>.NLog(<i>NLog.LoggerFactory</i>)</b> appends **NLog.LoggerFactory** to a line.
This forwards the *Type* from the line to logger.
[!code-csharp[Snippet](Examples.cs#Snippet_6a)]

<b>.NLog(<i>NLog.ILogger</i>)</b> appends **NLog.ILogger** to a line.
[!code-csharp[Snippet](Examples.cs#Snippet_5a)]

Resolving a string causes logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_6b)]

The logger output.
```none
2019-06-08 14:10:46.4939|INFO|MyClass|ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
OK
```

# LineRoot
The deploying application can place a logger into **LineRoot**.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

Any line that is derived from **LineRoot** causes logger to be applied.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]
