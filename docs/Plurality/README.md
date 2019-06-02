# Plurality
Language strings with numeric arguments can be customized for declination of pluralized nouns.

To use plurality, the key must have "PluralRules" parameter configured.
There are five ways to configure the plurality rule:

1. Add class name of plural rules into the localization file (*recommended way*). The value "Lexical.Localization.CLDR35" uses [Unicode CLDR35 plural rules](http://cldr.unicode.org/index/cldr-spec/plural-rules). 
The class is derivate of CLDR35 and is licensed under [Unicode License agreement](https://www.unicode.org/license.html) as "Data Files".
<details>
  <summary>PluralRules="Lexical.Localization.CLDR35" (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Lexical.Localization.CLDR35">

  <!-- Default string from Inline scanned by Lexical.Localization.Tool -->
  <Key:Cats>{0} cat(s)</Key:Cats>

  <!-- Translator added strings for "en" -->
  <Key:Cats Culture="en">
    {cardinal:0} cat(s)
    <N:zero>no cats</N:zero>
    <N:one>a cat</N:one>
    <N:other>{0} cats</N:other>
  </Key:Cats>
  
  <!-- Translator added strings for "fi" -->
  <Key:Cats Culture="fi">
    {cardinal:0} kissa(a)
    <N:zero>ei kissoja</N:zero>
    <N:one>yksi kissa</N:one>
    <N:other>{0} kissaa</N:other>
  </Key:Cats>

</Localization>

```
</details>
<br/>

2. Add plural rules expression to localization file. See [Unicode CLDR Plural Rule Syntax](https://unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules).
<details>
  <summary>PluralRules="[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true" (<u>click here</u>)</summary> 

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1" 
              xmlns:N2="urn:lexical.fi:N2" xmlns:N3="urn:lexical.fi:N3"
              xmlns="urn:lexical.fi"
              PluralRules="[Category=cardinal,Case=zero,Optional=1]n=0[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true">
  <!-- Example: Plurality expression in the localization file ^^^ -->

  <Key:CatsDogs>
    {cardinal:0} cat(s) and {cardinal:1} dog(s)
    <N:zero>
      <N1:zero>no cats and no dogs</N1:zero>
      <N1:one>no cats but one dog</N1:one>
      <N1:other>no cats but {1} dogs</N1:other>
    </N:zero>
    <N:one>
      <N1:zero>one cat but no dogs</N1:zero>
      <N1:one>a cat and a dog</N1:one>
      <N1:other>a cat and {1} dogs</N1:other>
    </N:one>
    <N:other>
      <N1:zero>{0} cats but no dogs</N1:zero>
      <N1:one>{0} cats and a dog</N1:one>
      <N1:other>{0} cats and {1} dogs</N1:other>
    </N:other>

  </Key:CatsDogs>


</Localization>

```
</details>
<br/>

3. Add instance of **IPluralRules** to line.

```csharp
ILine root = new LineRoot().PluralRules(CLDR35.Instance);
```

4. Add *class name* of "PluralRules" to line.

```csharp
ILine root = new LineRoot().PluralRules("Lexical.Localization.CLDR35");
```

5. Add unicode plural rules expression to line. ([Unicode CLDR Plural Rule Syntax](https://unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules))

```csharp
ILine root = new LineRoot().PluralRules("[Category=cardinal,Case=zero,Optional=1]n=0[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true");
```
<br/>
<br/>

Plural *category* is placed into argument placeholder, for example "There are {cardinal:0} cats.".
The available cases depend on the culture and the *category*. 
For root culture "" the category "cardinal" has cases "zero", "one" and "other".
See [full table of cases for each culture and category](https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html).
Each case must be matched with a subkey **N:<i>case</i>**.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Lexical.Localization.CLDR35">

  <!-- Example: Plurality for one numeric argument {0} -->
  <Key:Cats>
    {cardinal:0} cat(s)
    <N:zero>no cats</N:zero>
    <N:one>a cat</N:one>
    <N:other>{0} cats</N:other>
  </Key:Cats>

</Localization>

```


```csharp
IAsset asset = XmlLinesReader.Default.FileAsset("PluralityExample0b.xml");
ILine key = new LineRoot(asset).Key("Cats");

for (int cats = 0; cats<=2; cats++)
    Console.WriteLine(key.Value(cats));
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

If all cases are not used, then then **StringResolver** will to default string.
For example, "N:one" is provided, and for values other than "1", the rules revert to default string.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Lexical.Localization.CLDR35">

  <!-- Example: One plurality case for one numeric argument {0} -->
  <Key:Cats>
    {cardinal:0} cats
    <N:one>a cat</N:one>
  </Key:Cats>

</Localization>

```

<br/>
Translator adds localized strings for different cultures.
The decision whether to use pluralization is left for the translator.

```csharp
IAsset asset = XmlLinesReader.Default.FileAsset("PluralityExample0a.xml");
ILineRoot root = new LineRoot(asset);
ILine key = root.Key("Cats").Format("{0} cat(s)");

// Print with the default string (without culture policy)
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(key.Value(cats));

// Print Culture "en"
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(key.Culture("en").Value(cats));

// Print Culture "fi"
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(key.Culture("fi").Value(cats));
```

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Lexical.Localization.CLDR35">

  <!-- Default string from Inline scanned by Lexical.Localization.Tool -->
  <Key:Cats>{0} cat(s)</Key:Cats>

  <!-- Translator added strings for "en" -->
  <Key:Cats Culture="en">
    {cardinal:0} cat(s)
    <N:zero>no cats</N:zero>
    <N:one>a cat</N:one>
    <N:other>{0} cats</N:other>
  </Key:Cats>
  
  <!-- Translator added strings for "fi" -->
  <Key:Cats Culture="fi">
    {cardinal:0} kissa(a)
    <N:zero>ei kissoja</N:zero>
    <N:one>yksi kissa</N:one>
    <N:other>{0} kissaa</N:other>
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

Inlined strings are picked up by [inline scanner](~/sdk/Localization/docs/Tool/index.html) and placed to a localization file.
Add sub-key with parameter name **N** for argument "{0}" and value for each of "zero", "one", "other".

```csharp
ILineRoot root = new LineRoot();
ILine key = root.Key("Cats")
        .PluralRules(CLDR35.Instance)
        .Format("{cardinal:0} cat(s)")  // Default string
        .Inline("N:zero", "no cats")
        .Inline("N:one", "a cat")
        .Inline("N:other", "{0} cats");
```

And inlining for specific cultures too with subkey "Culture:*culture*:N:*case*".

```csharp
ILineRoot root = new LineRoot();
ILine key = root.Key("Cats")
        .PluralRules(CLDR35.Instance)
        .Format("{0} cat(s)")   // Default string
        .Inline("Culture:en", "{cardinal:0} cat(s)")
        .Inline("Culture:en:N:zero", "no cats")
        .Inline("Culture:en:N:one", "a cat")
        .Inline("Culture:en:N:other", "{0} cats")
        .Inline("Culture:fi", "{cardinal:0} kissa(a)")
        .Inline("Culture:fi:N:zero", "ei kissoja")
        .Inline("Culture:fi:N:one", "yksi kissa")
        .Inline("Culture:fi:N:other", "{0} kissaa");

for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(key.Culture("en").Value(cats));
```



If translator wants to supply plurality for two numeric arguments, then all permutations of cases (for example "zero", "one" and "other") for both arguments must be covered.
By default the maximum number of pluralized arguments is three arguments. This value can be modified, by creating a custom instance of **StringResolver** into **ILineRoot**.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi"
              PluralRules="Lexical.Localization.CLDR35">

  <!-- Example: Plurality for two numeric arguments {0} and {1} -->
  <Key:CatsDogs Culture="en">
    {cardinal:0} cat(s) and {cardinal:1} dog(s)
    <N:zero>
      <N1:zero>no cats and no dogs</N1:zero>
      <N1:one>no cats but one dog</N1:one>
      <N1:other>no cats but {1} dogs</N1:other>
    </N:zero>
    <N:one>
      <N1:zero>one cat but no dogs</N1:zero>
      <N1:one>a cat and a dog</N1:one>
      <N1:other>a cat and {1} dogs</N1:other>
    </N:one>
    <N:other>
      <N1:zero>{0} cats but no dogs</N1:zero>
      <N1:one>{0} cats and a dog</N1:one>
      <N1:other>{0} cats and {1} dogs</N1:other>
    </N:other>
  </Key:CatsDogs>

</Localization>

```

```csharp
IAsset asset = XmlLinesReader.Default.FileAsset("PluralityExample2-en.xml");
ILineRoot root = new LineRoot(asset);
ILine key = root.Key("CatsDogs").Format("{0} cat(s) and {1} dog(s)");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        Console.WriteLine(key.Culture("en").Value(cats, dogs));
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

Pluralization is applied only to the arguments that have "{<i>category</i>:<i>arg</i>}".

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1" 
              xmlns:N2="urn:lexical.fi:N2" xmlns:N3="urn:lexical.fi:N3"
              xmlns="urn:lexical.fi"
              PluralRules="Lexical.Localization.CLDR35">

  <!-- Example: Plurality for one numeric argument {2} -->
  <Key:CatsDogsPoniesHorses>
    {0} cat(s), {1} dog(s), {cardinal:2} poni(es) and {3} horse(s)

    <N2:zero>{0} cat(s), {1} dog(s), no ponies and {3} horse(s)</N2:zero>
    <N2:one>{0} cat(s), {1} dog(s), a pony and {3} horse(s)</N2:one>
    <N2:other>{0} cat(s), {1} dog(s), {2} ponies and {3} horse(s)</N2:other>
    
  </Key:CatsDogsPoniesHorses>

</Localization>

```

```csharp
IAsset asset = XmlLinesReader.Default.FileAsset("PluralityExample4.xml");
ILine key = new LineRoot(asset).Key("CatsDogsPoniesHorses");

for (int cats = 0; cats <= 2; cats++)
    for (int dogs = 0; dogs <= 2; dogs++)
        for (int ponies = 0; ponies <= 2; ponies++)
            for (int horses = 0; horses <= 2; horses++)
                Console.WriteLine(key.Value(cats, dogs, ponies, horses));
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



