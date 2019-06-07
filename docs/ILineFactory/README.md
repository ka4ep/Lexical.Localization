# Introduction
<details>
  <summary><b>ILineFactory</b> is interface for printers and parsers of string formats. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Line factory can append new parts to <see cref="ILine"/>s. 
/// Appended parts are typically immutable, and form a linked list or a trie.
/// </summary>
public interface ILineFactory
{
}

/// <summary>
/// Policy for <see cref="ILineFactoryCollection"/>.
/// </summary>
public enum LineFactoryAddPolicy
{
    /// <summary>
    /// If appender with same key exists, throw <see cref="InvalidOperationException"/>.
    /// </summary>
    ThrowIfExists,

    /// <summary>
    /// If appender with same key exists, overwrite the previous appender.
    /// </summary>
    OverwriteIfExists,

    /// <summary>
    /// If appender with same key exists, ignore the new appender. Don't throw.
    /// </summary>
    IgnoreIfExists
}

/// <summary>
/// Collection of line factories
/// </summary>
public interface ILineFactoryCollection
{
    /// <summary>
    /// Add line factory to collection.
    /// </summary>
    /// <param name="lineFactory"></param>
    /// <param name="policy"></param>
    ILineFactoryCollection Add(ILineFactory lineFactory, LineFactoryAddPolicy policy = LineFactoryAddPolicy.ThrowIfExists);
}

/// <summary>
/// Enumerable of line factory's component factories.
/// </summary>
public interface ILineFactoryEnumerable : IEnumerable<ILineFactory>
{
}

/// <summary>
/// Zero argument line factory.
/// </summary>
/// <typeparam name="Intf">the interface type of the line part that can be appended </typeparam>
public interface ILineFactory<Intf> : ILineFactory where Intf : ILine
{
    /// <summary>
    /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="previous"></param>
    /// <param name="line">create output</param>
    /// <returns>true if part was created</returns>
    /// <exception cref="LineException">If append failed due to unexpected reasons</exception>
    bool TryCreate(ILineFactory factory, ILine previous, out Intf line);
}

/// <summary>
/// One argument line part factory.
/// </summary>
/// <typeparam name="Intf">the part type</typeparam>
/// <typeparam name="A0"></typeparam>
public interface ILineFactory<Intf, A0> : ILineFactory where Intf : ILine
{
    /// <summary>
    /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="previous"></param>
    /// <param name="a0"></param>
    /// <param name="line">create output</param>
    /// <returns>true if part was created</returns>
    /// <exception cref="LineException">If append failed due to unexpected reason</exception>
    bool TryCreate(ILineFactory factory, ILine previous, A0 a0, out Intf line);
}

/// <summary>
/// Two argument line part factory.
/// </summary>
/// <typeparam name="Intf">the part type</typeparam>
/// <typeparam name="A0"></typeparam>
/// <typeparam name="A1"></typeparam>
public interface ILineFactory<Intf, A0, A1> : ILineFactory where Intf : ILine
{
    /// <summary>
    /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="previous"></param>
    /// <param name="a0"></param>
    /// <param name="a1"></param>
    /// <param name="line">create output</param>
    /// <returns>true if part was created</returns>
    /// <exception cref="LineException">If append failed due to unexpected reason</exception>
    bool TryCreate(ILineFactory factory, ILine previous, A0 a0, A1 a1, out Intf line);
}

/// <summary>
/// Three argument line part factory.
/// </summary>
/// <typeparam name="Intf">the part type</typeparam>
/// <typeparam name="A0"></typeparam>
/// <typeparam name="A1"></typeparam>
/// <typeparam name="A2"></typeparam>
public interface ILineFactory<Intf, A0, A1, A2> : ILineFactory where Intf : ILine
{
    /// <summary>
    /// Append new <see cref="ILine"/> to <paramref name="previous"/>.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="previous"></param>
    /// <param name="a0"></param>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <param name="line">create output</param>
    /// <returns>true if part was created</returns>
    /// <exception cref="LineException">If append failed due to unexpected reason</exception>
    bool TryCreate(ILineFactory factory, ILine previous, A0 a0, A1 a1, A2 a2, out Intf line);
}

/// <summary>
/// Adapts to different <see cref="ILineFactory"/> types.
/// </summary>
public interface ILineFactoryCastable : ILineFactory
{
    /// <summary>
    /// Get factory for part type <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <returns>factory or null</returns>
    ILineFactory<Intf> Cast<Intf>() where Intf : ILine;

    /// <summary>
    /// Get factory for part type <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0">argument 0 type</typeparam>
    /// <returns>factory or null</returns>
    ILineFactory<Intf, A0> Cast<Intf, A0>() where Intf : ILine;

    /// <summary>
    /// Get factory for part type <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0">argument 0 type</typeparam>
    /// <typeparam name="A1">argument 1 type</typeparam>
    /// <returns>factory or null</returns>
    ILineFactory<Intf, A0, A1> Cast<Intf, A0, A1>() where Intf : ILine;

    /// <summary>
    /// Get factory for part type <typeparamref name="Intf"/>.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0">argument 0 type</typeparam>
    /// <typeparam name="A1">argument 1 type</typeparam>
    /// <typeparam name="A2">argument 2 type</typeparam>
    /// <returns>factory or null</returns>
    ILineFactory<Intf, A0, A1, A2> Cast<Intf, A0, A1, A2>() where Intf : ILine;
}

/// <summary>
/// Appender can append new <see cref="ILine"/>s.
/// </summary>
public interface ILineFactoryByArgument : ILineFactory
{
    /// <summary>
    /// Create line (part) with <paramref name="arguments"/>.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="previous"></param>
    /// <param name="arguments">Line construction arguments</param>
    /// <param name="line">create output</param>
    /// <returns>true if part was created</returns>
    /// <exception cref="LineException">If append failed due to unexpected reason</exception>
    bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line);
}

/// <summary>
/// Line factory that has an assigned resolver.
/// </summary>
public interface ILineFactoryResolver : ILineFactory
{
    /// <summary>
    /// (optional) Type and parameter resolver
    /// </summary>
    IResolver Resolver { get; set; }
}

/// <summary>
/// Line factory that has parameter infos assigned
/// </summary>
public interface ILineFactoryParameterInfos : ILineFactory
{
    /// <summary>
    /// (optional) Associated parameter infos.
    /// </summary>
    IParameterInfos ParameterInfos { get; set; }
}
```
</details>
<details>
  <summary><b>ILineAppendable</b> is interface ILine parts that can be appended. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// A line where new <see cref="ILine"/> can be appended.
/// </summary>
public interface ILineAppendable : ILine
{
    /// <summary>
    /// (Optional) part constructor. If null, the caller should follow to <see cref="ILinePart.PreviousPart"/> for appender.
    /// </summary>
    ILineFactory Appender { get; set; }
}
```
</details>
<br />

| Implementation | Description |
|:-------|:-------|
| LineAppender.Default | Resolves parameter values into instances. |
| LineAppender.NonResolving | Doesn't resolve parameter values into instances. |
| StringLocalizerAppender.Default | Resolves parameter values into instances. |
| StringLocalizerAppender.NonResolving | Doesn't resolve parameter values into instances. |
| LineFactoryComposition | Collection of component *ILineFactory* parts. |

<br/>
**ILineFactory** is used for appending and constructing *ILine* parts.
The two main factories are **LineAppender.NonResolving** and **LineAppender.Default**. 
They produce lines that are equally usable, but with differences in terms of performance at 
usage-time and at construction-time.
<br/>

**LineAppender.NonResolving** appends parts to ILine as string based parameters. 
It doesn't resolve parameters into respective instances at instantion time.
The user of the ILine, must evaluate the parameters into respective instances.
This is most suitable for constructing hash-equals compatible keys. 

```csharp
ILine key = LineAppender.NonResolving.Type("ILineFactory_Examples").Key("Hello");
IString str = asset.GetLine(key).GetString();
```
<br/>

**LineAppender.Default** appends parts to ILine and resolves values into respective instances.
For example, calling <b>.PluralRules(<i>string</i>)</b> causes the rules to be constructed and cached into the **IResolver** 
that is associated with the *ILineFactory*. This is most suitable for constructing lines that are used
by assets, as the referenced instances are already available.

```csharp
ILine localization = LineAppender.Default
    .PluralRules("[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true")
    .Assembly("docs")
    .Culture("en")
    .Type<ILineFactory_Examples>();

List<ILine> lines = new List<ILine>
{
    localization.Key("OK").Text("Successful"),
    localization.Key("Error").Format("Failed (ErrorCode={0})")
};

IAsset asset = new StringAsset().Add(lines);
```
<br/>

*ILine*s are associated with an *ILineFactory*. This factory can be replaced in middle of line with <b>.SetAppender(<i>ILineFactory</i>)</b>.

```csharp
IStringLocalizer localizer = 
    LineRoot.Global.Type("Hello").SetAppender(StringLocalizerAppender.Default).Key("Ok") 
    as IStringLocalizer;
```
<br/>

New custom appenders can be added to line with <b>.AddAppender(<i>ILineFactory</i>)</b>. Note that this makes a whole new copy of the previous *ILineFactory*.

```csharp
ILine line = LineRoot.Global.AddAppender(new MyAppender()).Key("Ok");
```

```csharp
class MyAppender : ILineFactory<ILineCanonicalKey, string, string>
{
    public bool TryCreate(
        ILineFactory factory, 
        ILine previous, 
        string parameterName, 
        string parameterValue, 
        out ILineCanonicalKey line)
    {
        line = new LineCanonicalKey(factory, previous, parameterName, parameterValue);
        return true;
    }
}
```
