# Plurality
Language strings with numeric arguments can be customized for declination of pluralized nouns.

Plural *category* is placed into argument placeholder, for example "There are {cardinal:0} cats.".
The available cases depend on the culture and the *category*. 
The root culture "" has category "cardinal" and "ordinal", and the former has three possible cases "zero", "one" and "other".
See [table of case for each culture and category](#plural-rules-table).
Each case must be matched with a subkey **N:<i>case</i>**.
[!code-xml[Snippet](../PluralityExample0b.xml)]

[!code-csharp[Snippet](Examples.cs#Snippet_1a)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats
a cat
2 cats
</pre>
</details>

<br/>

If all cases are not used, then then **StringResolver** will revert to default string.
For example, "N:one" is provided, and for values other than "1", the rules revert to default string.
[!code-xml[Snippet](../PluralityExample0c.xml)]

<br/>
Translator adds localized strings for different cultures.
The decision whether to use pluralization is left for the translator.
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]
# [xml](#tab/xml)
[!code-xml[Snippet](../PluralityExample0a.xml)]
# [json](#tab/json)
[!code-json[Snippet](../PluralityExample0a.json)]
# [ini](#tab/ini)
[!code-ini[Snippet](../PluralityExample0a.ini)]
***
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
0 cat(s)
1 cat(s)
2 cat(s)
no cats
a cat
2 cats
ei kissoja
yksi kissa
2 kissaa
</pre>
</details>
<br/>

Inlined strings are picked up by [inline scanner](~/sdk/Localization/docs/Tool/index.html) and placed to a localization file.
Add sub-key with parameter name **N** for argument "{0}" and value for each of "zero", "one", "other".
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

And inlining for specific cultures too with subkey "Culture:*culture*:N:*case*".
[!code-csharp[Snippet](Examples.cs#Snippet_0c)]



If translator wants to supply plurality for two numeric arguments, then all permutations of required cases (for example "zero", "one" and "other") for both arguments must be covered.
By default the maximum number of pluralized arguments is three arguments. This value can be modified, by creating a custom instance of **StringResolver** into **ILineRoot**.
[!code-xml[Snippet](../PluralityExample2.xml)]
[!code-csharp[Snippet](Examples.cs#Snippet_5)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats and no dogs
no cats but one dog
no cats but 2 dogs
one cat but no dogs
a cat and a dog
a cat and 2 dogs
2 cats but no dogs
2 cats and a dog
2 cats and 2 dogs
</pre>
</details>
<br/>

Pluralization is applied only to the arguments that have "{<i>category</i>:<i>arg</i>}".
[!code-xml[Snippet](../PluralityExample4.xml)]
[!code-csharp[Snippet](Examples.cs#Snippet_6)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
0 cat(s), 0 dog(s), no ponies and 0 horse(s)
0 cat(s), 0 dog(s), no ponies and 1 horse(s)
0 cat(s), 0 dog(s), no ponies and 2 horse(s)
0 cat(s), 0 dog(s), a pony and 0 horse(s)
0 cat(s), 0 dog(s), a pony and 1 horse(s)
0 cat(s), 0 dog(s), a pony and 2 horse(s)
0 cat(s), 0 dog(s), 2 ponies and 0 horse(s)
0 cat(s), 0 dog(s), 2 ponies and 1 horse(s)
0 cat(s), 0 dog(s), 2 ponies and 2 horse(s)
0 cat(s), 1 dog(s), no ponies and 0 horse(s)
0 cat(s), 1 dog(s), no ponies and 1 horse(s)
0 cat(s), 1 dog(s), no ponies and 2 horse(s)
0 cat(s), 1 dog(s), a pony and 0 horse(s)
0 cat(s), 1 dog(s), a pony and 1 horse(s)
0 cat(s), 1 dog(s), a pony and 2 horse(s)
0 cat(s), 1 dog(s), 2 ponies and 0 horse(s)
0 cat(s), 1 dog(s), 2 ponies and 1 horse(s)
0 cat(s), 1 dog(s), 2 ponies and 2 horse(s)
0 cat(s), 2 dog(s), no ponies and 0 horse(s)
0 cat(s), 2 dog(s), no ponies and 1 horse(s)
0 cat(s), 2 dog(s), no ponies and 2 horse(s)
0 cat(s), 2 dog(s), a pony and 0 horse(s)
0 cat(s), 2 dog(s), a pony and 1 horse(s)
0 cat(s), 2 dog(s), a pony and 2 horse(s)
0 cat(s), 2 dog(s), 2 ponies and 0 horse(s)
0 cat(s), 2 dog(s), 2 ponies and 1 horse(s)
0 cat(s), 2 dog(s), 2 ponies and 2 horse(s)
1 cat(s), 0 dog(s), no ponies and 0 horse(s)
1 cat(s), 0 dog(s), no ponies and 1 horse(s)
1 cat(s), 0 dog(s), no ponies and 2 horse(s)
1 cat(s), 0 dog(s), a pony and 0 horse(s)
1 cat(s), 0 dog(s), a pony and 1 horse(s)
1 cat(s), 0 dog(s), a pony and 2 horse(s)
1 cat(s), 0 dog(s), 2 ponies and 0 horse(s)
1 cat(s), 0 dog(s), 2 ponies and 1 horse(s)
1 cat(s), 0 dog(s), 2 ponies and 2 horse(s)
1 cat(s), 1 dog(s), no ponies and 0 horse(s)
1 cat(s), 1 dog(s), no ponies and 1 horse(s)
1 cat(s), 1 dog(s), no ponies and 2 horse(s)
1 cat(s), 1 dog(s), a pony and 0 horse(s)
1 cat(s), 1 dog(s), a pony and 1 horse(s)
1 cat(s), 1 dog(s), a pony and 2 horse(s)
1 cat(s), 1 dog(s), 2 ponies and 0 horse(s)
1 cat(s), 1 dog(s), 2 ponies and 1 horse(s)
1 cat(s), 1 dog(s), 2 ponies and 2 horse(s)
1 cat(s), 2 dog(s), no ponies and 0 horse(s)
1 cat(s), 2 dog(s), no ponies and 1 horse(s)
1 cat(s), 2 dog(s), no ponies and 2 horse(s)
1 cat(s), 2 dog(s), a pony and 0 horse(s)
1 cat(s), 2 dog(s), a pony and 1 horse(s)
1 cat(s), 2 dog(s), a pony and 2 horse(s)
1 cat(s), 2 dog(s), 2 ponies and 0 horse(s)
1 cat(s), 2 dog(s), 2 ponies and 1 horse(s)
1 cat(s), 2 dog(s), 2 ponies and 2 horse(s)
2 cat(s), 0 dog(s), no ponies and 0 horse(s)
2 cat(s), 0 dog(s), no ponies and 1 horse(s)
2 cat(s), 0 dog(s), no ponies and 2 horse(s)
2 cat(s), 0 dog(s), a pony and 0 horse(s)
2 cat(s), 0 dog(s), a pony and 1 horse(s)
2 cat(s), 0 dog(s), a pony and 2 horse(s)
2 cat(s), 0 dog(s), 2 ponies and 0 horse(s)
2 cat(s), 0 dog(s), 2 ponies and 1 horse(s)
2 cat(s), 0 dog(s), 2 ponies and 2 horse(s)
2 cat(s), 1 dog(s), no ponies and 0 horse(s)
2 cat(s), 1 dog(s), no ponies and 1 horse(s)
2 cat(s), 1 dog(s), no ponies and 2 horse(s)
2 cat(s), 1 dog(s), a pony and 0 horse(s)
2 cat(s), 1 dog(s), a pony and 1 horse(s)
2 cat(s), 1 dog(s), a pony and 2 horse(s)
2 cat(s), 1 dog(s), 2 ponies and 0 horse(s)
2 cat(s), 1 dog(s), 2 ponies and 1 horse(s)
2 cat(s), 1 dog(s), 2 ponies and 2 horse(s)
2 cat(s), 2 dog(s), no ponies and 0 horse(s)
2 cat(s), 2 dog(s), no ponies and 1 horse(s)
2 cat(s), 2 dog(s), no ponies and 2 horse(s)
2 cat(s), 2 dog(s), a pony and 0 horse(s)
2 cat(s), 2 dog(s), a pony and 1 horse(s)
2 cat(s), 2 dog(s), a pony and 2 horse(s)
2 cat(s), 2 dog(s), 2 ponies and 0 horse(s)
2 cat(s), 2 dog(s), 2 ponies and 1 horse(s)
2 cat(s), 2 dog(s), 2 ponies and 2 horse(s)
</pre>
</details>

# Plural Rules Table
[!include[Plural Rules Table](PluralRulesTable.html)]

This table is derived from rules of [Unicode CLDR](https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html) "Data files".

# PluralRules Parameter
To use plurality, the key must have "PluralRules" parameter either in the ILine or in the localization file.
There are five ways to configure the plurality rule:

1. Add class name of plural rules into the localization file (*recommended way*). The value "Lexical.Localization.CLDR35" uses [Unicode CLDR35 plural rules](http://cldr.unicode.org/index/cldr-spec/plural-rules). 
The class is derivate of CLDR35 and is licensed under [Unicode License agreement](https://www.unicode.org/license.html) as "Data Files".
<details>
  <summary>PluralRules="Lexical.Localization.CLDR35" (<u>click here</u>)</summary>
[!code-xml[Snippet](../PluralityExample0a.xml)]
</details>
<br/>

2. Add plural rules expression to localization file. See [Unicode CLDR Plural Rule Syntax](https://unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules).
<details>
  <summary>PluralRules="[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true" (<u>click here</u>)</summary> 
[!code-xml[Snippet](../PluralityExample5.xml)]
</details>
<br/>

3. Add instance of **IPluralRules** to line.
[!code-csharp[Snippet](Examples.cs#Snippet_A1)]

4. Add *class name* of **IPluralRules** to line.
[!code-csharp[Snippet](Examples.cs#Snippet_A2)]

5. Add unicode plural rules expression to line. ([Unicode CLDR Plural Rule Syntax](https://unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules))
[!code-csharp[Snippet](Examples.cs#Snippet_A3)]
<br/>
<br/>
