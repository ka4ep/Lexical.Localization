# Context free format
The context free format of a key is an array of keys and values **IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;**.

**AssetKeyParametrizer** converts implementations of *IAssetKey* to *IEnumerable*
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

And back to IAssetKey.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

# String Serialization
The string representation is uses colon (:) as separator
```none
parameterName:parameterValue:parameterName:parameterValue:...
```

For example:
```none
culture:en:Type:MyController:Key:Success
```

Escaping uses the following rules.

| Sequence | Meaning |
|:---------|:--------|
| \\: | Colon |
| \\t | Tab |
| \\r | Carriage return |
| \\n | New line |
| \\xnnnn | Unicode 16bit surrogate |
| \\unnnn | Unicode variable length surrogate |

For example to escape key "Success:Plural" would be
```none
key:Success\:Plural
```

**AssetKeyStringSerializer** Serializes key to string.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

And string to key.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

# Parameters
Well known parameters are

| Parameter | Canonical | Section | Description |
|:---------|:-------|:--------|:---------|
| type | canonical | yes | Type section for grouping by classes and interfaces. |
| location | canonical | yes | A directory to search assets from. |
| assembly | canonical | yes | An assembly to search embedded resource from. |
| resource | canonical | yes | Embedded resource path to search from. |
| section | canonical | yes | Generic section for grouping assets. |
| key | canonical | no | Key |
| culture | non-canonical | no | Language and region |
