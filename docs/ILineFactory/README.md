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
<br />

Implementations

| Implementation | Description |
|:-------|:-------|
| LineAppender.Default | Resolves parameter values into instances. |
| LineAppender.NonResolving | Doesn't resolve parameter values into instances, but keeps references as strings. |
| StringLocalizerAppender.Default | Resolves parameter values into instances. Constructed parts implement *IStringLocalizer* and *IStringLocalizerFactory* |
| StringLocalizerAppender.NonResolving | Doesn't resolve parameter values into instances, but keeps references as strings. |
| LineFactoryComposition | Collection of component *ILineFactory* parts. |


```csharp

```

