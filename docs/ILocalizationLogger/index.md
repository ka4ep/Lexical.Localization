# Introduction
<details>
  <summary><b>ILocalizationLogger</b> is interface for logging string and resource resolve results. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Common/ILocalizationLogger.cs#Interface)]
</details>
<br />

**LineStatusSeverity** four severity levels.

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
**Microsoft.Extensions.Logger.ILogger** can be appended with <b>.ILogger(<i>ILogger</i>)<b>.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]

Resolving a string causes the logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]

The logger output.
```none
info: MyClass[0]
ResolveOkFromKey | CultureOkMatchedNoCulture | PluralityOkNotUsed | StringFormatOkString Type: MyClass: Key: OK = "OK"
```

**Microsoft.Extensions.Logger.ILoggerFactory** can be appended with <b>.ILogger(<i>ILoggerFactory</i>)<b>.
This applies the *Type* from the key, making the type specific log rules effective to localization events.
[!code-csharp[Snippet](Examples.cs#Snippet_4a)]

Resolving a string causes logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]

The logger output.
```none
info: MyClass[0]
ResolveOkFromKey | CultureOkMatchedNoCulture | PluralityOkNotUsed | StringFormatOkString Type: MyClass: Key: OK = "OK"
```

# NLog
**NLog.ILogger** can be appended with <b>.NLog(<i>NLog.ILogger</i>)<b>.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]

Resolving a string causes logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]

The logger output.
```none
2019-06-08 14:10:46.4939|INFO|MyClass|ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
OK
```

**NLog.LoggerFactory** can be appended with <b>.NLog(<i>NLog.LoggerFactory</i>)<b>.
This applies the *Type* from the key, making the type specific log rules effective to localization events.
[!code-csharp[Snippet](Examples.cs#Snippet_4a)]

Resolving a string causes logger to print the resolved result.
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]

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
