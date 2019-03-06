# Asset Key Comparer

**AssetKeyComparer.Default** is the default **<i>IEqualityComparer&lt;IAssetKey&gt;</i>** comparer.

```csharp
AssetKeyComparer comparer = AssetKeyComparer.Default;
```

It compares parts that implement *IAssetKeyParameterAssigned* and either *IAssetKeyCanonicallyCompared* or *IAssetKeyNonCanonicallyCompared*.

```csharp
IAssetKey key = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
int hash = AssetKeyComparer.Default.GetHashCode(key);
```

Keys that are constructed from different roots are reference comparable. 

```csharp
IAssetKey key1 = new LocalizationRoot().Type("MyClass").Key("OK");
IAssetKey key2 = Key.Create("Type", "MyClass").Append("Key", "OK");
IAssetKey key3 = LocalizationRoot.Global.Type("MyClass").Key("OK");
IAssetKey key4 = StringLocalizerRoot.Global.Type("MyClass").Key("OK");

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2);
bool equals23 = AssetKeyComparer.Default.Equals(key2, key3);
bool equals34 = AssetKeyComparer.Default.Equals(key3, key4);
int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
int hash3 = AssetKeyComparer.Default.GetHashCode(key3);
int hash4 = AssetKeyComparer.Default.GetHashCode(key4);
```

The location of *IAssetKeyNonCanonicallyCompared* parts, such as **.Culture()** does not matter to the comparer.

```csharp
IAssetKey key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
IAssetKey key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("en");

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2);
```

If a non-canonical part occurs multiple times in a key, then by a rule, only the left-most if considered effective.

```csharp
IAssetKey key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
IAssetKey key2 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK").Culture("de");

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2);
```

