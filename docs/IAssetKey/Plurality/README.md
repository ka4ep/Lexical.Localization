# Plurality
Language strings with numeric arguments can be customized for declination.
Inlining must provide a sub-key with parameter name **N** for argument "{0}" with value for each of "Zero", "One", "Plural".

```csharp
IAssetKey key = LocalizationRoot.Global.Key("Cats")
        .Inline("{0} cat(s)")
        .Inline("N:Zero", "no cats")
        .Inline("N:One", "a cat")
        .Inline("N:Plural", "{0} cats");

for (int cats = 0; cats <= 2; cats++)
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
Pluralized language strings can be read from an xml file.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi">
  
  <Key:Cats>
    {0} cat(s)
    <N:Zero>no cats</N:Zero>
    <N:One>a cat</N:One>
    <N:Plural>{0} cats</N:Plural>
  </Key:Cats>

</Localization>

```

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample0.xml");
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

If there are two numeric arguments in a formulation string, then plurality keys can be added to one of them.

```csharp
IAssetKey key = LocalizationRoot.Global.Key("CatsDogs")
        .Inline("{0} cats and {1} dog(s)")
        .Inline("N:Zero", "no cats and {1} dog(s)")
        .Inline("N:One", "a cat and {1} dog(s)")
        .Inline("N:Plural", "{0} cats and {1} dog(s)");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        Console.WriteLine(key.Format(cats, dogs));
```
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats and 0 dog(s)
no cats and 1 dog(s)
no cats and 2 dog(s)
a cat and 0 dog(s)
a cat and 1 dog(s)
a cat and 2 dog(s)
2 cats and 0 dog(s)
2 cats and 1 dog(s)
2 cats and 2 dog(s)
</pre>
</details>

<br/>
Same from an xml file.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for argument {0} only -->
  <Key:CatsDogs1>
    {0} cat(s) and {1} dog(s)
    <N:Zero>no cats and {1} dog(s)</N:Zero>
    <N:One>a cat and {1} dog(s)</N:One>
    <N:Plural>{0} cats and {1} dog(s)</N:Plural>
  </Key:CatsDogs1>

</Localization>

```

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample1.xml");
IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs1");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        Console.WriteLine(key.Format(cats, dogs));
```
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats and 0 dog(s)
no cats and 1 dog(s)
no cats and 2 dog(s)
a cat and 0 dog(s)
a cat and 1 dog(s)
a cat and 2 dog(s)
2 cats and 0 dog(s)
2 cats and 1 dog(s)
2 cats and 2 dog(s)
</pre>
</details>

<br/>
If the pluralized argument is "{1}", then the parameter name is **N1**.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for argument {1} only -->
  <Key:CatsDogs2>
    {0} cat(s) and {1} dog(s)
    <N1:Zero>{0} cat(s) and no dogs</N1:Zero>
    <N1:One>{0} cat(s) and a dog</N1:One>
    <N1:Plural>{0} cat(s) and {1} dogs</N1:Plural>
  </Key:CatsDogs2>

</Localization>

```

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample2.xml");
IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs2");

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
All permutations can also be supplied, but only if there are two numeric arguments. All cases "Zero", "One" and "Plural" must be supplied.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for permutations of arguments {0} and {1} -->
  <Key:CatsDogs3>
    {0} cat(s) and {1} dog(s)
    <N:Zero>
      <N1:Zero>no cats and no dogs</N1:Zero>
      <N1:One>no cats and a dog</N1:One>
      <N1:Plural>no cats and {1} dogs</N1:Plural>
    </N:Zero>
    <N:One>
      <N1:Zero>a cat and no dogs</N1:Zero>
      <N1:One>a cat and a dog</N1:One>
      <N1:Plural>a cat and {1} dogs</N1:Plural>
    </N:One>
    <N:Plural>
      <N1:Zero>{0} cats and no dogs</N1:Zero>
      <N1:One>{0} cats and a dog</N1:One>
      <N1:Plural>{0} cats and {1} dogs</N1:Plural>
    </N:Plural>
  </Key:CatsDogs3>

</Localization>

```

```csharp
IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample3.xml");
IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs3");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        Console.WriteLine(key.Format(cats, dogs));
```
<details>
  <summary>The result (<u>click here</u>)</summary>
<pre>
no cats and no dogs
no cats and a dog
no cats and 2 dogs
a cat and no dogs
a cat and a dog
a cat and 2 dogs
2 cats and no dogs
2 cats and a dog
2 cats and 2 dogs
</pre>
</details>

<br/>
If there are more than two numeric arguments, pluralization can be used for one argument. Again, all cases "Zero", "One" and "Plural" must be supplied.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1" xmlns:N2="urn:lexical.fi:N2" xmlns:N3="urn:lexical.fi:N3"
              xmlns="urn:lexical.fi">

  <!-- Example: Plurality for argument {3} only -->
  <Key:CatsDogsPoniesHorses>
    {0} cat(s), {1} dog(s), {2} pony(es) and {3} horse(s)
    
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
