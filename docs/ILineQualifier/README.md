# ILineQualifier
<details>
  <summary><b>ILineQualifier</b> is an for measuring qualities of <i>ILine</i>s. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Measures qualities of <see cref="ILine"/>s and <see cref="ILineParameter"/>.
/// </summary>
public interface ILineQualifier
{
}

/// <summary>
/// Tests whether a <see cref="ILine"/> matches a qualification criteria.
/// </summary>
public interface ILineQualifierEvaluatable : ILineQualifier
{
    /// <summary>
    /// Qualify <paramref name="line"/> against the criteria.
    /// </summary>
    /// <param name="line"></param>
    /// <returns>true if line is qualified, false if disqualified</returns>
    bool Qualify(ILine line);
}

/// <summary>
/// Tests whether a <see cref="ILine"/> matches a criteria.
/// </summary>
public interface ILineQualifierLinesEvaluatable : ILineQualifier
{
    /// <summary>
    /// Qualifies lines. 
    /// </summary>
    /// <param name="lines"></param>
    /// <returns>all lines that were qualified</returns>
    IEnumerable<ILine> Qualify(IEnumerable<ILine> lines);
}

/// <summary>
/// Measures qualifications of <see cref="ILineArgument"/>.
/// </summary>
public interface ILineArgumentQualifier : ILineQualifier
{
    /// <summary>
    /// Policy whether occuranceIndex needs to be supplied when providing <see cref="ILineParameter"/>, <see cref="ILineHint"/>, <see cref="ILineCanonicalKey"/>, <see cref="ILineNonCanonicalKey"/> arguments.
    /// 
    /// If true, <see cref="QualifyArgument(ILineArgument, int)"/> caller must supply occurance index.
    /// If false, caller can use -1 for unspecified.
    /// 
    /// Occurance describes the position of parameter of same parameter name.
    /// For example, "Section:A:Section:B:Section:C" has parameter "Section" with three 
    /// occurance indices: 0, 1, 2.
    /// </summary>
    bool NeedsOccuranceIndex { get; }

    /// <summary>
    /// Qualify <paramref name="argument"/>.
    /// </summary>
    /// <param name="argument">argument</param>
    /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc. Use -1 if occurance is unspecified</param>
    /// <returns>true if parameter is qualified, false if disqualified</returns>
    bool QualifyArgument(ILineArgument argument, int occuranceIndex = -1);
}

/// <summary>
/// Qualifier that can enumerate component qualifiers.
/// </summary>
public interface ILineQualifierEnumerable : ILineQualifier, IEnumerable<ILineQualifier>
{
}

/// <summary>
/// Composition of line qualifiers.
/// </summary>
public interface ILineQualifierComposition : ILineQualifier
{
    /// <summary>
    /// Is collection in read-only state.
    /// </summary>
    bool ReadOnly { get; set; }

    /// <summary>
    /// Add rule that is validated against complete <see cref="ILine"/>.
    /// </summary>
    /// <param name="qualifierRule"></param>
    void Add(ILineQualifier qualifierRule);
}
```
</details>

<br />

```csharp

```

| Class | Description |  |
|:-------|:-------|:--------|
|  |  |   |
|  |  |   |
|  |  |   |

