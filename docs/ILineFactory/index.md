# Introduction
<details>
  <summary><b>ILineFactory</b> is interface for printers and parsers of string formats. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/ILineFactory.cs#Interface)]
</details>
<details>
  <summary><b>ILineAppendable</b> is interface ILine parts that can be appended. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/Parts/ILineAppendable.cs#Interface)]
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
[!code-csharp[Snippet](Examples.cs#Snippet_0)]
<br/>

**LineAppender.Default** appends parts to ILine and resolves values into respective instances.
For example, calling <b>.PluralRules(<i>string</i>)</b> causes the rules to be constructed and cached into the **IResolver** 
that is associated with the *ILineFactory*. This is most suitable for constructing lines that are used
by assets, as the referenced instances are already available.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]
<br/>

*ILine*s are associated with an *ILineFactory*. This factory can be replaced in middle of line with <b>.SetAppender(<i>ILineFactory</i>)</b>.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]
<br/>

New custom appenders can be added to line with <b>.AddAppender(<i>ILineFactory</i>)</b>. Note that this makes a whole new copy of the previous *ILineFactory*.
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]
