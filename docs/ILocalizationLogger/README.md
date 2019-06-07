# Introduction
<details>
  <summary><b>ILocalizationLogger</b> is interface for logging string and resource resolve results. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Localization logger.
/// 
/// See sub-interfaces
/// <list type="bullet">
///     <item><see cref="IStringResolverLogger"/></item>
///     <item><see cref="IResourceResolverLogger"/></item>
/// </list>
/// </summary>
public interface ILocalizationLogger
{
}

/// <summary>
/// Logger that logs string resolving of <see cref="IStringResolver"/>.
/// </summary>
public interface IStringResolverLogger : ILocalizationLogger, IObserver<LineString>
{
}

/// <summary>
/// Logger that logs resource resolving of <see cref="IResourceResolver"/>.
/// </summary>
public interface IResourceResolverLogger : ILocalizationLogger, IObserver<LineResourceBytes>, IObserver<LineResourceStream>
{
}
```
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

```csharp
ILine root = LineRoot.Global.Logger(Console.Out, LineStatusSeverity.Ok);
```

Resolving a string causes logger to print the resolved result.

```csharp
Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
```

The logger output.
```none
ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
```

# Diagnostics Trace
Logger can output to System.Diagnostics.Trace.

```csharp
Trace.Listeners.Add(new ConsoleTraceListener());
ILine root = LineRoot.Global.DiagnosticsTrace(LineStatusSeverity.Ok);
```

Resolving a string causes logger to print the resolved result.

```csharp
Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
```

The logger output.
```none
docs Information: 0 : ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
```

# ILogger
**Microsoft.Extensions.Logger.ILogger** can be appended with **.ILogger**.

```csharp
LoggerFactory loggerFactory = new LoggerFactory();
loggerFactory.AddConsole(LogLevel.Trace);
ILogger logger = loggerFactory.CreateLogger("MyClass");
ILine root = LineRoot.Global.ILogger(logger);
```

Resolving a string causes logger to print the resolved result.

```csharp
Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
```

The logger output.
```none
info: MyClass[0]
ResolveOkFromKey | CultureOkMatchedNoCulture | PluralityOkNotUsed | StringFormatOkString Type: MyClass: Key: OK = "OK"
```

#LineRoot
The deploying application can place a logger into **LineRoot**.

```csharp
(LineRoot.Global as LineRoot.Mutable).Logger = new LineTextLogger(Console.Out, LineStatusSeverity.Ok);
```

Any line that is derived from **LineRoot** causes logger to be applied.

```csharp
Console.WriteLine(LineRoot.Global.Type("MyClass").Key("OK").Text("OK"));
```
