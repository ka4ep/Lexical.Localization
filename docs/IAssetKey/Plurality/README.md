# Plurality
Language strings with numeric arguments can be customized for declination of pluralized nouns.
The parameter name for pluralization of argument "{0}" is **N**, and cases are "Zero", "One", "Plural".

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for one numeric argument {0} -->
  <Key:Cats>
    {0} cat(s)
    <N:Zero>no cats</N:Zero>
    <N:One>a cat</N:One>
    <N:Plural>{0} cats</N:Plural>
  </Key:Cats>

</Localization>

```


```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample0b.xml");
IAssetKey key = new LocalizationRoot(asset).Key("Cats");

for (int cats = 0; cats<=2; cats++)
    Console.WriteLine(key.Format(cats));
```
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats
a cat
2 cats
</pre>
</details>

<br/>
If pluralized string is not found then default string is used.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi">

  <!-- Example: One plurality case for one numeric argument {0} -->
  <Key:Cats>
    {0} cats
    <N:One>a cat</N:One>
  </Key:Cats>

</Localization>

```

<br/>
Inlined strings are picked up by [inline scanner](~/sdk/Localization/docs/Tool/index.html) and placed to a localization file.
Translator adds localized strings for different cultures.
The decision whether to use pluralization is left for the translator. The file is read into the application. 

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample0a.xml");
IAssetRoot root = new LocalizationRoot(asset);
IAssetKey key = root.Key("Cats").Inline("{0} cat(s)");

// Print with the default string (without culture policy)
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(key.Format(cats));

// Print Culture "en"
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(key.Culture("en").Format(cats));

// Print Culture "fi"
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(key.Culture("fi").Format(cats));
```

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi">

  <!-- Default string from Inline scanned by Lexical.Localization.Tool -->
  <Key:Cats>{0} cat(s)</Key:Cats>

  <!-- Translator added strings for "en" -->
  <Key:Cats Culture="en">
    {0} cat(s)
    <N:Zero>no cats</N:Zero>
    <N:One>a cat</N:One>
    <N:Plural>{0} cats</N:Plural>
  </Key:Cats>
  
  <!-- Translator added strings for "fi" -->
  <Key:Cats Culture="fi">
    <N:Zero>ei kissoja</N:Zero>
    <N:One>yksi kissa</N:One>
    <N:Plural>{0} kissaa</N:Plural>
  </Key:Cats>

</Localization>

```
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
0 cat(s)
1 cat(s)
2 cat(s)
no cats
a cat
2 cats
ei kissoja
yksi kissa
2 kissaa
</pre>
</details>
<br/>

Pluralization can be added to inlining too. Add sub-key with parameter name **N** for argument "{0}" and value for each of "Zero", "One", "Plural".

```csharp
IAssetRoot root = new LocalizationRoot();
IAssetKey key = root.Key("Cats")
        .Inline("{0} cat(s)")  // Default string
        .Inline("N:Zero", "no cats")
        .Inline("N:One", "a cat")
        .Inline("N:Plural", "{0} cats");
```

And inlining for specific cultures too with subkey "Culture:*culture*:N:*case*".

```csharp
IAssetRoot root = new LocalizationRoot();
IAssetKey key = root.Key("Cats")
        .Inline("{0} cat(s)")   // Default string
        .Inline("Culture:en:N:Zero", "no cats")
        .Inline("Culture:en:N:One", "a cat")
        .Inline("Culture:en:N:Plural", "{0} cats")
        .Inline("Culture:fi:N:Zero", "ei kissoja")
        .Inline("Culture:fi:N:One", "yksi kissa")
        .Inline("Culture:fi:N:Plural", "{0} kissaa");
```


If language string has two numeric arguments, then plurality keys can be added to one or both of them. The parameter name for argument "{1}" is **N1**. 

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for numeric argument {1} only -->
  <Key:CatsDogs>
    {0} cat(s) and {1} dog(s)
    <N1:Zero>{0} cat(s) and no dogs</N1:Zero>
    <N1:One>{0} cat(s) and a dog</N1:One>
    <N1:Plural>{0} cat(s) and {1} dogs</N1:Plural>
  </Key:CatsDogs>

</Localization>

```

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample2.xml");
IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        Console.WriteLine(key.Format(cats, dogs));
```
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

If translator wants to supply plurality for two numeric arguments, then all permutations of cases "Zero", "One" and "Plural" for both arguments must be covered.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for two numeric arguments {0} and {1} -->
  <Key:CatsDogs Culture="en">
    {0} cat(s) and {1} dog(s)
    <N:Zero>
      <N1:Zero>no cats and no dogs</N1:Zero>
      <N1:One>no cats but one dog</N1:One>
      <N1:Plural>no cats but {1} dogs</N1:Plural>
    </N:Zero>
    <N:One>
      <N1:Zero>one cat but no dogs</N1:Zero>
      <N1:One>a cat and a dog</N1:One>
      <N1:Plural>a cat and {1} dogs</N1:Plural>
    </N:One>
    <N:Plural>
      <N1:Zero>{0} cats but no dogs</N1:Zero>
      <N1:One>{0} cats and a dog</N1:One>
      <N1:Plural>{0} cats and {1} dogs</N1:Plural>
    </N:Plural>
  </Key:CatsDogs>

</Localization>

```

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample2-en.xml");
IAssetRoot root = new LocalizationRoot(asset);
IAssetKey key = root.Key("CatsDogs").Inline("{0} cat(s) and {1} dog(s)");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        Console.WriteLine(key.Culture("en").Format(cats, dogs));
```
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

If there are more than two numeric arguments, pluralization can be provided for one argument, but not for any permutation of two or more arguments.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1" 
              xmlns:N2="urn:lexical.fi:N2" xmlns:N3="urn:lexical.fi:N3"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for one numeric argument {2} -->
  <Key:CatsDogsPoniesHorses>
    {0} cat(s), {1} dog(s), {2} poni(es) and {3} horse(s)
    
    <N2:Zero>{0} cat(s), {1} dog(s), no ponies and {3} horse(s)</N2:Zero>
    <N2:One>{0} cat(s), {1} dog(s), a pony and {3} horse(s)</N2:One>
    <N2:Plural>{0} cat(s), {1} dog(s), {2} ponies and {3} horse(s)</N2:Plural>
    
  </Key:CatsDogsPoniesHorses>

</Localization>

```

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample4.xml");
IAssetKey key = new LocalizationRoot(asset).Key("CatsDogsPoniesHorses");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        for (int ponies = 0; ponies <= 2; ponies++)
            for (int horses = 0; horses <= 2; horses++)
                Console.WriteLine(key.Format(cats, dogs, ponies, horses));
```
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
