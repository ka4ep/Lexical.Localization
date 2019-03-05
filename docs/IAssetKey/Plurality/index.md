# Plurality
Language strings with numeric arguments can be customized for declination of pluralized nouns.
The parameter name for pluralization of argument "{0}" is **N**, and cases are "Zero", "One", "Plural".
[!code-xml[Snippet](../../PluralityExample0b.xml)]

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
If pluralized string is not found then default string is used.
[!code-xml[Snippet](../../PluralityExample0c.xml)]

<br/>
Inlined strings are picked up by inline scanner and placed to a localization file.
Translator adds localized strings for different cultures.
The decision whether to use pluralization is left for the translator. The file is read into the application. 
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]
[!code-xml[Snippet](../../PluralityExample0a.xml)]
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

Pluralization can be added to inlining too. Add sub-key with parameter name **N** for argument "{0}" and value for each of "Zero", "One", "Plural".
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

And inlining for specific cultures too with subkey "Culture:*culture*:N:*case*".
[!code-csharp[Snippet](Examples.cs#Snippet_0c)]


If language string has two numeric arguments, then plurality keys can be added to one or both of them. The parameter name for argument "{1}" is **N1**. 
[!code-xml[Snippet](../../PluralityExample2.xml)]
[!code-csharp[Snippet](Examples.cs#Snippet_4)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
0 cat(s) and no dogs
0 cat(s) and a dog
0 cat(s) and 2 dogs
1 cat(s) and no dogs
1 cat(s) and a dog
1 cat(s) and 2 dogs
2 cat(s) and no dogs
2 cat(s) and a dog
2 cat(s) and 2 dogs
</pre>
</details>
<br/>

If translator wants to supply plurality for two numeric arguments, then all permutations of cases "Zero", "One" and "Plural" for both arguments must be covered.
[!code-xml[Snippet](../../PluralityExample2-en.xml)]
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

If there are more than two numeric arguments, pluralization can be provided for one argument, but not for any permutation of two or more arguments.
[!code-xml[Snippet](../../PluralityExample4.xml)]
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
