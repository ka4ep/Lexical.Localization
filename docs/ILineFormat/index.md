# ILineFormat
<details>
  <summary><b>ILineFormat</b> is root interface for *ILine* and *String* converters. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/Format/ILineFormat.cs#ILineFormat)]
</details>

<details>
  <summary><b>ILineFormatPrinter</b> prints *ILine*s as *Strings*. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/Format/ILineFormat.cs#ILinePrinter)]
</details>

<details>
  <summary><b>ILineFormatParser</b> parses *Strings* into *ILine*. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/Format/ILineFormat.cs#ILineParser)]
</details>

<br />

| Class | ILineFormatPrinter | ILineFormatParser |
|:-------|:-------|:--------|
| LineFormat | &#9745; | &#9745; |
| LinePattern | &#9745;  | &#9745; |
| LineParameterPrinter | &#9745; | &#9744; |

# LineFormat
**LineFormat** is an *ILineFormat* class that prints and parses keys into strings using the following notation.
```none
parameterName:parameterValue:parameterName:parameterValue:...
```

Keys are converted to strings.
[!code-csharp[Snippet](LineFormat_Examples.cs#Snippet_2)]

And strings parsed to keys.
[!code-csharp[Snippet](LineFormat_Examples.cs#Snippet_0)]

A specific *root* can be used from which the constructed key is appended from.
[!code-csharp[Snippet](LineFormat_Examples.cs#Snippet_0b)]

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
[!code-csharp[Snippet](LineFormat_Examples.cs#Snippet_1)]

| Instance | Description |
|:-------|:-------|
| LineFormat.Key | Prints and parses effective key of *ILine*. |
| LineFormat.Line | Prints and parses whole *ILine*. |
| LineFormat.Parameters | Prints and parses the parameters of *ILine*, excluding "String" parameter |
| LineFormat.ParametersInclString | Prints and parses every parameter of *ILine* |

# LinePattern
<details>
  <summary><b>ILinePattern</b> is interface for name patterns. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Line/Format/ILinePattern.cs#doc)]
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

# LineParameterPrinter
**LineParameterPrinter** is a generic class that prints key parts into strings by applying configured rules.

Let's create an example key.
[!code-csharp[Snippet](LineParameterPrinter_Examples.cs#Snippet_1)]
And now, let's try out different policies to see how they look.
[!code-csharp[Snippet](LineParameterPrinter_Examples.cs#Snippet_2)]

Policy is created by adding rules to LineParameterPrinter.
[!code-csharp[Snippet](LineParameterPrinter_Examples.cs#Snippet_3)]

# Links
* [ILineFormat](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/Format/ILineFormat.cs) is the root interface for classes that formulate ILine into identity string.
* [ILineFormatPrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/Format/ILineFormat.cs) is a subinterface where Build() can be implemented directly.
* [ILinePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/Format/ILinePattern.cs) is a subinterface that formulates parametrization with a template string.
* [LineParameterPrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/Format/LineParameterPrinter.cs) is implementation of IAssetNameProvider.
* [LinePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/Format/LinePattern.cs) is the default implementation of ILinePattern.
* [LineFormat](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/Format/LineFormat.cs) is context-free string format.
