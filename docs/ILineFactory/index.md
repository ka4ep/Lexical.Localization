# Introduction
<details>
  <summary><b>ILineFactory</b> is interface for printers and parsers of string formats. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/ILineFactory.cs#Interface)]
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

[!code-csharp[Snippet](Examples.cs#Snippet_0)]

