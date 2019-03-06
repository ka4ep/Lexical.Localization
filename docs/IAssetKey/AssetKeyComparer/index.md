# Asset Key Comparer

**AssetKeyComparer.Default** is the default **<i>IEqualityComparer&lt;IAssetKey&gt;</i>** comparer.
[!code-csharp[Snippet](Examples.cs#Snippet_0)]

It compares parts that implement *IAssetKeyParameterAssigned* and either *IAssetKeyCanonicallyCompared* or *IAssetKeyNonCanonicallyCompared*.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

Keys that are constructed from different roots are reference comparable. 
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

The location of *IAssetKeyNonCanonicallyCompared* parts, such as **.Culture()** does not matter to the comparer.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

If a non-canonical part occurs multiple times in a key, then by a rule, only the left-most if considered effective.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

