# IParameterPolicy
<details>
  <summary><b>IParameterPolicy</b> is root interface for *IAssetKey* name converter. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IParameterPolicy.cs#IParameterPolicy)]
</details>

<details>
  <summary><b>IParameterPrinter</b> is sub-interface that prints *IAssetKeys* as *Strings*. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IParameterPolicy.cs#IParameterPrinter)]
</details>

<details>
  <summary><b>IParameterParser</b> is sub-interface that parses *Strings* into *IAssetKey*. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IParameterPolicy.cs#IParameterParser)]
</details>

<br />

| Class | IParameterPrinter | IParameterParser |
|:-------|:-------|:--------|
| ParameterPolicy | &#9745; | &#9745; |
| ParameterPattern | &#9745;  | &#9745; |
| KeyPrinter | &#9745; | &#9744; |

# ParameterPolicy
**ParameterPolicy** is an *IAssetNameKeyPolicy* class that prints and parses keys into strings using the following notation.
```none
parameterName:parameterValue:parameterName:parameterValue:...
```

Keys are converted to strings.
[!code-csharp[Snippet](ParameterPolicy_Examples.cs#Snippet_2)]

And strings parsed to keys.
[!code-csharp[Snippet](ParameterPolicy_Examples.cs#Snippet_0)]

A specific *root* can be used from which the constructed key is appended from.
[!code-csharp[Snippet](ParameterPolicy_Examples.cs#Snippet_0b)]

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
[!code-csharp[Snippet](ParameterPolicy_Examples.cs#Snippet_1)]

# ParameterPattern
<details>
  <summary><b>IParameterPattern</b> is interface for name patterns. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IParameterPattern.cs#Interface)]
</details>
<br />

**ParameterPattern** is a regular-expression like pattern to print and extract parameters from keys and strings.
[!code-csharp[Snippet](ParameterPattern_Examples.cs#Snippet_1)]
[!code-csharp[Snippet](ParameterPattern_Examples.cs#Snippet_2)]

Name pattern consists of parameters. They are written in format of "{prefix **ParameterName** suffix}".  
Braces "{parameter/}" make parameter optional, and brackets "[parameter/]" mandatory.
[!code-csharp[Snippet](ParameterPattern_Examples.cs#Snippet_3)]

Parameter can be added multiple times.
[!code-csharp[Snippet](ParameterPattern_Examples.cs#Snippet_4b)]

A shorter way to add consecutive parameters is use suffix "_n". It translates to the five following occurances.
If part is required, e.g. "[parametername_n]", then only first part is required and others optional.
[!code-csharp[Snippet](ParameterPattern_Examples.cs#Snippet_4c)]

Parameters need to be added in non-consecutive order, then "_#" can be used to represent the occurance index.
[!code-csharp[Snippet](ParameterPattern_Examples.cs#Snippet_4d)]

Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}", which gives more control over matching.
[!code-csharp[Snippet](ParameterPattern_Examples.cs#Snippet_5)]

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
 * [IParameterPolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IParameterPolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IParameterPrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IParameterPolicy.cs) is a subinterface where Build() can be implemented directly.
 * [IParameterPattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IParameterPattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [KeyPrinter](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/KeyPrinter.cs) is implementation of IAssetNameProvider.
 * [ParameterPattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/ParameterPattern.cs) is the default implementation of IParameterPattern.
 * [ParameterPolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/ParameterPolicy.cs) is context-free string format.
