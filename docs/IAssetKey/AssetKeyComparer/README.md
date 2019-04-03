# Asset Key Comparer

**AssetKeyComparer.Default** is the default **<i>IEqualityComparer&lt;IAssetKey&gt;</i>** comparer.

```csharp
IEqualityComparer<IAssetKey> comparer = AssetKeyComparer.Default;
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

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
bool equals23 = AssetKeyComparer.Default.Equals(key2, key3); // Are equal
bool equals34 = AssetKeyComparer.Default.Equals(key3, key4); // Are equal
int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
int hash3 = AssetKeyComparer.Default.GetHashCode(key3);
int hash4 = AssetKeyComparer.Default.GetHashCode(key4);
```

The location of *IAssetKeyNonCanonicallyCompared* parts, such as **.Culture()** does not matter to the comparer.

```csharp
IAssetKey key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
IAssetKey key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("en");

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
```

If a non-canonical part occurs multiple times in a key, then by rule, only the left-most if considered effective.

```csharp
IAssetKey key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
IAssetKey key2 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK").Culture("de");

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
```

A non-canonical parameter with empty value "" is considered same as not existing.

```csharp
IAssetKey key1 = LocalizationRoot.Global.Type("MyClass").Key("OK");
IAssetKey key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("");

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
```

There is a difference though, for a non-canonical parameter such as "Culture" cannot be re-selected.

```csharp
IAssetKey key1 = LocalizationRoot.Global.Type("MyClass").Key("OK");
IAssetKey key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("");
string str1 = key1.Culture("fi").ToString();  // <- Selects a culture
string str2 = key2.Culture("fi").ToString();  // <- Doesn't change the effective culture
```

A canonical parameter with empty value "" is considered meaningful for hash-equals comparison.

```csharp
IAssetKey key1 = LocalizationRoot.Global.Section("").Key("OK");
IAssetKey key2 = LocalizationRoot.Global.Key("OK");

bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are not equal
int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
```

