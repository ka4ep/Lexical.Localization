# ILineFormatPolicy
<details>
  <summary><b>ILineFormatPolicy</b> is root interface for *ILine* name converter. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/ILineFormatPolicy.cs#ILineFormatPolicy)]
</details>

<details>
  <summary><b>ILinePrinter</b> is sub-interface that prints *ILines* as *Strings*. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/ILineFormatPolicy.cs#ILinePrinter)]
</details>

<details>
  <summary><b>ILineParser</b> is sub-interface that parses *Strings* into *ILine*. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/ILineFormatPolicy.cs#ILineParser)]
</details>

<br />

| Class | ILinePrinter | ILineParser |
|:-------|:-------|:--------|
| ParameterParser.| &#9745; | &#9745; |
| LinePattern | &#9745;  | &#9745; |
| KeyPrinter | &#9745; | &#9744; |

# ParameterParser.
**ParameterParser.* is an *IAssetNameKeyPolicy* class that prints and parses keys into strings using the following notation.
```none
parameterName:parameterValue:parameterName:parameterValue:...
```

Keys are converted to strings.
[!code-csharp[Snippet](ParameterParser.Examples.cs#Snippet_2)]

And strings parsed to keys.
[!code-csharp[Snippet](ParameterParser.Examples.cs#Snippet_0)]

A specific *root* can be used from which the constructed key is appended from.
[!code-csharp[Snippet](ParameterParser.Examples.cs#Snippet_0b)]

Policy uses the following escape rules.

| Sequence | Meaning |
|:---------|:--------|
| \\: | Colon |
| \\t | Tab |
| \\r | Carriage return |
| \\n | New line |
| \\x<i>hh</i> | Unicode 8bit |
| \\u<i>hhhh</i> | Unicode 16bit (surrogate) |
| \\U<i>hhhhhhhh</i> | Unicode 32bit |

Example of escaped key "Success\\:Plural".
[!code-csharp[Snippet](ParameterParser.Examples.cs#Snippet_1)]

# LinePattern
<details>
  <summary><b>ILinePattern</b> is interface for name patterns. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/ILinePattern.cs#Interface)]
</details>
<br />

**LinePattern** is a regular-expression like pattern to print and extract parameters from keys and strings.
[!code-csharp[Snippet](LinePattern_Examples.cs#Snippet_1)]
[!code-csharp[Snippet](LinePattern_Examples.cs#Snippet_2)]

Name pattern consists of parameters. They are written in format of "{prefix **ParameterName** suffix}".  
Braces "{parameter/}" make parameter optional, and brackets "[parameter/]" mandatory.
[!code-csharp[Snippet](LinePattern_Examples.cs#Snippet_3)]

Parameter can be added multiple times.
[!code-csharp[Snippet](LinePattern_Examples.cs#Snippet_4b)]

A shorter way to add consecutive parameters is use suffix "_n". It translates to the five following occurances.
If part is required, e.g. "[parametername_n]", then only first part is required and others optional.
[!code-csharp[Snippet](LinePattern_Examples.cs#Snippet_4c)]

Parameters need to be added in non-consecutive order, then "_#" can be used to represent the occurance index.
[!code-csharp[Snippet](LinePattern_Examples.cs#Snippet_4d)]

Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}", which gives more control over matching.
[!code-csharp[Snippet](LinePattern_Examples.cs#Snippet_5)]

## Parameters
Reserved parameter names and respective extension methods.

| Parameter | Key Method  | Description |
|----------|:--------|:------------|
| Assembly | .Assembly(*string*) | Assembly name |
| Location | .Location(*string*) | Subdirectory in local files |
| Resource | .Resource(*string*) | Subdirectory in embedded resources |
| Type | .Type(*string*) | Class name |
| Section | .Section(*string*) | Generic section, used for grouping |
| anysection | *all above* | Matches to any section above. |
| Culture  | .Culture(*string*) | Culture |
| Key | .Key(*string*) | Key name |
| N | .N(*Type*) | Plurality key |

# KeyPrinter
**KeyPrinter** is a generic class that prints key parts into strings using various rules.

Let's create an example key.
[!code-csharp[Snippet](KeyPrinter_Examples.cs#Snippet_1)]
And now, let's try out different policies to see how they look.
[!code-csharp[Snippet](KeyPrinter_Examples.cs#Snippet_2)]

Policy is created by adding rules to KeyPrinter.
[!code-csharp[Snippet](KeyPrinter_Examples.cs#Snippet_3)]

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [ILineFormatPolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/ILineFormatPolicy.cs) is the root interface for classes that formulate ILine into identity string.
 * [ILinePrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/ILineFormatPolicy.cs) is a subinterface where Build() can be implemented directly.
 * [ILinePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/ILinePattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [KeyPrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/KeyPrinter.cs) is implementation of IAssetNameProvider.
 * [LinePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/LinePattern.cs) is the default implementation of ILinePattern.
 * [ParameterParser.(https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/ParameterParser.cs) is context-free string format.
