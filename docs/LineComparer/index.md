# Line Comparer
**LineComparer.Default** is the default **<i>IEqualityComparer&lt;ILine&gt;</i>** comparer.
[!code-csharp[Snippet](Examples.cs#Snippet_0)]

Table of comparers:

| Comparer | Description 
|:-------|:-------|
| LineComparer.Default | Makes key comparisons |
| LineComparer.IgnoreCulture | Key comparer but ignores "Culture" parameters |
| LineComparer.KeyValue | Key and String-value comparison. |
| LineComparer.Parameter | Compares parameters in order of occurance.   |

**LineComparer.Default** compares parts that implement *ILineCanonicalKey* or *ILineNonCanonicalKey*, and also *ILineParameter* if the *ParameterName* recognized as key.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

Keys that are constructed from different roots are reference comparable. 
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

The location of *ILineNonCanonicalKey* parts, such as **.Culture()** does not matter to the comparer.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

If a non-canonical part occurs multiple times in a key, then by rule, only the left-most if considered effective.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

A non-canonical parameter with empty value "" is considered same as not existing.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]

There is a difference though, for a non-canonical parameter such as "Culture" cannot be re-selected.
[!code-csharp[Snippet](Examples.cs#Snippet_5b)]

A canonical parameter with empty value "" is considered meaningful for hash-equals comparison.
[!code-csharp[Snippet](Examples.cs#Snippet_6)]
