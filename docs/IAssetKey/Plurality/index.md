# Plurality
Language strings with numeric arguments can be customized for declination.
Inlining must provide a sub-key with parameter name **N** for argument "{0}" with value for each of "Zero", "One", "Plural".
[!code-csharp[Snippet](Examples.cs#Snippet_0)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats
a cat
2 cats
</pre>
</details>

<br/>
Pluralized language strings can be read from an xml file.
[!code-xml[Snippet](../../PluralityExample0.xml)]
[!code-csharp[Snippet](Examples.cs#Snippet_1)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats
a cat
2 cats
</pre>
</details>
<br/>

If there are two numeric arguments in a formulation string, then plurality keys can be added to one of them.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats and 0 dog(s)
no cats and 1 dog(s)
no cats and 2 dog(s)
a cat and 0 dog(s)
a cat and 1 dog(s)
a cat and 2 dog(s)
2 cats and 0 dog(s)
2 cats and 1 dog(s)
2 cats and 2 dog(s)
</pre>
</details>

<br/>
Same from an xml file.
[!code-xml[Snippet](../../PluralityExample1.xml)]
[!code-csharp[Snippet](Examples.cs#Snippet_3)]
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats and 0 dog(s)
no cats and 1 dog(s)
no cats and 2 dog(s)
a cat and 0 dog(s)
a cat and 1 dog(s)
a cat and 2 dog(s)
2 cats and 0 dog(s)
2 cats and 1 dog(s)
2 cats and 2 dog(s)
</pre>
</details>

<br/>
If the argument is "{1}" is to be declinated for pluralization, then the parameter name is **N1**.
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
For two numeric arguments all permutations can be supplied. All cases of "Zero", "One" and "Plural" must be provided.
[!code-xml[Snippet](../../PluralityExample3.xml)]
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
If there are more than two numeric arguments, pluralization can be used for one argument. Again, all cases "Zero", "One" and "Plural" must be supplied.
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
