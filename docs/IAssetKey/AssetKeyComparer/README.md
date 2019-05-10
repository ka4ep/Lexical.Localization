# Asset Key Comparer

**LineComparer.Default** is the default **<i>IEqualityComparer&lt;ILine&gt;</i>** comparer.

```csharp
IEqualityComparer<ILine> comparer = LineComparer.Default;
```

It compares parts that implement *ILineParameterAssigned* and either *ILineCanonicallyCompared* or *ILineNonCanonicallyCompared*.

```csharp
ILine key = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
int hash = LineComparer.Default.GetHashCode(key);
```

Keys that are constructed from different roots are reference comparable. 

```csharp
ILine key1 = new LocalizationRoot().Type("MyClass").Key("OK");
ILine key2 = Key.Create("Type", "MyClass").Append("Key", "OK");
ILine key3 = LocalizationRoot.Global.Type("MyClass").Key("OK");
ILine key4 = StringLocalizerRoot.Global.Type("MyClass").Key("OK");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
bool equals23 = LineComparer.Default.Equals(key2, key3); // Are equal
bool equals34 = LineComparer.Default.Equals(key3, key4); // Are equal
int hash1 = LineComparer.Default.GetHashCode(key1);
int hash2 = LineComparer.Default.GetHashCode(key2);
int hash3 = LineComparer.Default.GetHashCode(key3);
int hash4 = LineComparer.Default.GetHashCode(key4);
```

The location of *ILineNonCanonicallyCompared* parts, such as **.Culture()** does not matter to the comparer.

```csharp
ILine key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
ILine key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("en");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
```

If a non-canonical part occurs multiple times in a key, then by rule, only the left-most if considered effective.

```csharp
ILine key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
ILine key2 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK").Culture("de");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
```

A non-canonical parameter with empty value "" is considered same as not existing.

```csharp
ILine key1 = LocalizationRoot.Global.Type("MyClass").Key("OK");
ILine key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
int hash1 = LineComparer.Default.GetHashCode(key1);
int hash2 = LineComparer.Default.GetHashCode(key2);
```

There is a difference though, for a non-canonical parameter such as "Culture" cannot be re-selected.

```csharp
ILine key1 = LocalizationRoot.Global.Type("MyClass").Key("OK");
ILine key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("");
string str1 = key1.Culture("fi").ToString();  // <- Selects a culture
string str2 = key2.Culture("fi").ToString();  // <- Doesn't change the effective culture
```

A canonical parameter with empty value "" is considered meaningful for hash-equals comparison.

```csharp
ILine key1 = LocalizationRoot.Global.Section("").Key("OK");
ILine key2 = LocalizationRoot.Global.Key("OK");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are not equal
int hash1 = LineComparer.Default.GetHashCode(key1);
int hash2 = LineComparer.Default.GetHashCode(key2);
```

