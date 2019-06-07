# Line Comparer
**LineComparer.Default** is the default **<i>IEqualityComparer&lt;ILine&gt;</i>** comparer.

```csharp
IEqualityComparer<ILine> comparer = LineComparer.Default;
```

Table of comparers:
| Comparer | Description 
|:-------|:-------|
| LineComparer.Default | Makes key comparisons |
| LineComparer.IgnoreCulture | Key comparer but ignores "Culture" parameters |
| LineComparer.KeyValue | Key and String-value comparison. |
| LineComparer.Parameter | Compares parameters in order of occurance.   |

**LineComparer.Default** compares parts that implement *ILineCanonicalKey* or *ILineNonCanonicalKey*, and also *ILineParameter* if the *ParameterName* recognized as key.

```csharp
ILine key = LineAppender.NonResolving.Culture("en").Key("OK");
int hash = LineComparer.Default.GetHashCode(key);
```

Keys that are constructed from different roots are reference comparable. 

```csharp
ILine key1 = new LineRoot().Key("OK");
ILine key2 = LineAppender.NonResolving.Key("OK");
ILine key3 = LineRoot.Global.Key("OK");
ILine key4 = StringLocalizerRoot.Global.Key("OK");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
bool equals23 = LineComparer.Default.Equals(key2, key3); // Are equal
bool equals34 = LineComparer.Default.Equals(key3, key4); // Are equal
int hash1 = LineComparer.Default.GetHashCode(key1);
int hash2 = LineComparer.Default.GetHashCode(key2);
int hash3 = LineComparer.Default.GetHashCode(key3);
int hash4 = LineComparer.Default.GetHashCode(key4);
```

The location of *ILineNonCanonicalKey* parts, such as **.Culture()** does not matter to the comparer.

```csharp
ILine key1 = LineAppender.NonResolving.Culture("en").Key("OK");
ILine key2 = LineAppender.NonResolving.Key("OK").Culture("en");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
```

If a non-canonical part occurs multiple times in a key, then by rule, only the left-most if considered effective.

```csharp
ILine key1 = LineAppender.NonResolving.Culture("en").Key("OK");
ILine key2 = LineAppender.NonResolving.Culture("en").Key("OK").Culture("de");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
```

A non-canonical parameter with empty value "" is considered same as not existing.

```csharp
ILine key1 = LineAppender.NonResolving.Key("OK");
ILine key2 = LineAppender.NonResolving.Key("OK").Culture("");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
int hash1 = LineComparer.Default.GetHashCode(key1);
int hash2 = LineComparer.Default.GetHashCode(key2);
```

There is a difference though, for a non-canonical parameter such as "Culture" cannot be re-selected.

```csharp
ILine key1 = LineAppender.NonResolving.Key("OK").Culture("fi"); // <- Selects a culture
ILine key2 = LineAppender.NonResolving.Key("OK").Culture("").Culture("fi"); // <- Culture "" remains
string str1 = LineFormat.Line.Print(key1);
string str2 = LineFormat.Line.Print(key2);
```

A canonical parameter with empty value "" is considered meaningful for hash-equals comparison.

```csharp
ILine key1 = LineAppender.NonResolving.Section("").Key("OK");
ILine key2 = LineAppender.NonResolving.Key("OK");

bool equals12 = LineComparer.Default.Equals(key1, key2); // Are not equal
int hash1 = LineComparer.Default.GetHashCode(key1);
int hash2 = LineComparer.Default.GetHashCode(key2);
```
