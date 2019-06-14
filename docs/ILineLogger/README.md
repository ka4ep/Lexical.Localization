# Introduction
<details>
  <summary><b>Lexical.Localization.Common.ILogger</b> is interface for logging string and resource resolve results. (<u>Click here</u>)</summary>

```csharp
namespace Lexical.Localization.Common
{
    /// <summary>
    /// Localization logger.
    /// 
    /// See sub-interfaces
    /// <list type="bullet">
    ///     <item><see cref="Lexical.Localization.StringFormat.IStringResolverLogger"/></item>
    ///     <item><see cref="Lexical.Localization.Resource.IResourceResolverLogger"/></item>
    /// </list>
    /// </summary>
    public interface ILogger
    {
    }
}

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Logger that logs string resolving of <see cref="IStringResolver"/>.
    /// </summary>
    public interface IStringResolverLogger : Lexical.Localization.Common.ILogger, IObserver<LineString>
    {
    }
}

namespace Lexical.Localization.Resource
{
    /// <summary>
    /// Logger that logs resource resolving of <see cref="IResourceResolver"/>.
    /// </summary>
    public interface IResourceResolverLogger : Lexical.Localization.Common.ILogger, IObserver<LineResourceBytes>, IObserver<LineResourceStream>
    {
    }
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
Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
ILine root = LineRoot.Global.DiagnosticsTrace(LineStatusSeverity.Ok);
```

Resolving a string causes the logger to print the resolved result.

```csharp
Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
```

The logger output.
```none
docs Information: 0 : ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
```

# ILogger
<b>.ILogger(<i>ILoggerFactory</i>)</b> appends **Microsoft.Extensions.Logger.ILoggerFactory** to a line.
This forwards the *Type* from the line to logger.

```csharp
LoggerFactory loggerFactory = new LoggerFactory();
loggerFactory.AddConsole(LogLevel.Trace);
ILine root = LineRoot.Global.ILogger(loggerFactory);
```

<b>.ILogger(<i>ILogger</i>)</b> appends **Microsoft.Extensions.Logger.ILogger** to a line.

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

# NLog
<b>.NLog(<i>NLog.LoggerFactory</i>)</b> appends **NLog.LoggerFactory** to a line.
This forwards the *Type* from the line to logger.

```csharp
ILine root = LineRoot.Global.NLog(NLog.LogManager.LogFactory);
```

<b>.NLog(<i>NLog.ILogger</i>)</b> appends **NLog.ILogger** to a line.

```csharp
NLog.ILogger nlog = NLog.LogManager.GetLogger("MyClass");
ILine root = LineRoot.Global.NLog(nlog);
```

Resolving a string causes logger to print the resolved result.

```csharp
Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
```

The logger output.
```none
2019-06-08 14:10:46.4939|INFO|MyClass|ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
OK
```

# LineRoot
The deploying application can place a logger into **LineRoot**.

```csharp
(LineRoot.Global as LineRoot.Mutable).Logger = new LineTextLogger(Console.Out, LineStatusSeverity.Ok);
```

Any line that is derived from **LineRoot** causes logger to be applied.

```csharp
Console.WriteLine(LineRoot.Global.Type("MyClass").Key("OK").Text("OK"));
```
