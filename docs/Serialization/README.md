# Context free format
The context free format of a key is an array of keys and values **IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;**.

**AssetKeyParametrizer** converts implementations of *IAssetKey* to *IEnumerable*

```csharp
// Parametrizer for AssetKey
IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
// Create context-dependent key
IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
// Convert to context-free parameters
IEnumerable<KeyValuePair<string, string>> parameters = parametrizer.GetAllParameters(key).ToArray();
```

And back to IAssetKey.

```csharp
// Convert to context-free parameters
ParameterKey parameters = new ParameterKey("culture", "en").Append("type", "MyLibrary:Type").Append("key", "\"hello\"");
// Parametrizer for AssetKey
IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
// Convert to context-dependent instance
object key = LocalizationRoot.Global;
foreach (var parameter in parameters.ToArray())
    key = parametrizer.CreatePart(key, parameter.Name, parameter.Value);
// Type-cast
IAssetKey key_ = (IAssetKey)key;
```

# String Serialization
The string representation is uses colon (:) as separator
```none
parameterName:parameterValue:parameterName:parameterValue:...
```

For example:
```none
culture:en:type:MyController:key:Success
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

**AssetKeyStringSerializer** prints IAssetKey as string.

```csharp
// Create context-dependent key
IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
// Serialize to string
string str = AssetKeyStringSerializer.Instance.PrintKey(key);
```

And parses it back to IAssetKey.

```csharp
// Key in string format
string str = "culture:en:type:MyLibrary.MyController:key:Success";
// Parse string
IAssetKey key = AssetKeyStringSerializer.Instance.ParseKey(str, LocalizationRoot.Global);
```

# Parameters
IAssetKey supports the following parameters.

| Parameter | Canonical | Section | Description |
|:---------|:-------|:--------|:---------|
| type | canonical | yes | Type section for grouping by classes and interfaces. |
| location | canonical | yes | A directory to search assets from. |
| assembly | canonical | yes | An assembly to search embedded resource from. |
| resource | canonical | yes | Embedded resource path to search from. |
| section | canonical | yes | Generic section for grouping assets. |
| key | canonical | no | Key |
| culture | non-canonical | no | Language and region |
