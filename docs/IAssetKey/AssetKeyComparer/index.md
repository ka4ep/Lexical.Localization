# Asset Key Comparer

**AssetKeyComparer.Default** is the default **<i>IEqualityComparer&lt;IAssetKey&gt;</i>** comparer.
[!code-csharp[Snippet](Examples.cs#Snippet_0)]

It compares parts that implement *IAssetKeyParameterAssigned* and either *IAssetKeyCanonicallyCompared* or *IAssetKeyNonCanonicallyCompared*.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

Keys that are constructed from different roots are reference comparable. 
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

The location of *IAssetKeyNonCanonicallyCompared* parts, such as **.Culture()** does not matter to the comparer.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

If a non-canonical part occurs multiple times in a key, then by rule, only the left-most if considered effective.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

A non-canonical parameter with empty value "" is considered same as not existing.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]

There is a difference though, for a non-canonical parameter such as "Culture" cannot be re-selected.
[!code-csharp[Snippet](Examples.cs#Snippet_5b)]

A canonical parameter with empty value "" is considered meaningful for hash-equals comparison.
[!code-csharp[Snippet](Examples.cs#Snippet_6)]

