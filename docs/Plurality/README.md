# Plurality
Language strings with numeric arguments can be customized for declination of pluralized nouns.

Plural *category* is placed into argument placeholder, for example "There are {cardinal:0} cats.".
The available cases depend on the culture and the *category*. 
The root culture "" has category "cardinal" and "ordinal", and the former has three possible cases "zero", "one" and "other".
See [table of case for each culture and category](#plural-rules-table).
Each case must be matched with a subkey **N:<i>case</i>**.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Unicode.CLDR35">

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
IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0b.xml");
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

If all cases are not used, then then **StringResolver** will revert to default string.
For example, "N:one" is provided, and for values other than "1", the rules revert to default string.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Unicode.CLDR35">

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
IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.xml");
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
# [xml](#tab/xml)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Unicode.CLDR35">

  <!-- Fallback string, for "" culture -->
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
# [json](#tab/json)

```json
{
  "PluralRules:Unicode.CLDR35": {
    /* Fallback string, for "" culture */
    "Key:Cats": "{0} cat(s)",

    /* Translator added strings for en */
    "Culture:en": {
      "Key:Cats": "{cardinal:0} cat(s)",
      "Key:Cats:N:zero": "no cats",
      "Key:Cats:N:one": "a cat",
      "Key:Cats:N:other": "{0} cats"
    },

    /* Translator added strings for fi */
    "Culture:fi": {
      "Key:Cats": "{cardinal:0} kissa(a)",
      "Key:Cats:N:zero": "ei kissoja",
      "Key:Cats:N:one": "yksi kissa",
      "Key:Cats:N:other": "{0} kissaa"
    }
  }
}

```
# [ini](#tab/ini)

```ini
; Fallback string, for "" culture
[PluralRules:Unicode.CLDR35]
Key:Cats = {0} cat(s)

; Translator added strings for en
[Culture:en:PluralRules:Unicode.CLDR35]
Key:Cats = {cardinal:0} cat(s)
Key:Cats:N:zero = no cats
Key:Cats:N:one = a cat
Key:Cats:N:other = {0} cats

; Translator added strings for fi
[Culture:fi:PluralRules:Unicode.CLDR35]
Key:Cats = {cardinal:0} kissa(a)
Key:Cats:N:zero = ei kissoja
Key:Cats:N:one = yksi kissa
Key:Cats:N:other = {0} kissaa

```
***
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
        .PluralRules("Unicode.CLDR35")
        .Format("{cardinal:0} cat(s)")  // Default string
        .Inline("N:zero", "no cats")
        .Inline("N:one", "a cat")
        .Inline("N:other", "{0} cats");
```

And inlining for specific cultures too with subkey "Culture:*culture*:N:*case*".

```csharp
ILineRoot root = new LineRoot();
ILine key = root.Key("Cats")
        .PluralRules("Unicode.CLDR35")
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



If translator wants to supply plurality for two numeric arguments, then all permutations of required cases (for example "zero", "one" and "other") for both arguments must be covered.
By default the maximum number of pluralized arguments is three arguments. This value can be modified, by creating a custom instance of **StringResolver** into **ILineRoot**.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N" xmlns:N1="urn:lexical.fi:N1"
              xmlns="urn:lexical.fi"
              PluralRules="Unicode.CLDR35">

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
IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample2.xml");
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
              PluralRules="Unicode.CLDR35">

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
IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample4.xml");
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

# Plural Rules Table

<details>
    <summary>PluralRules=&quot;Unicode.CLDR35&quot; (<u>click here</u>)</summary>
    <table>
        <tbody>
            <tr><th style="text-align:left">Culture</th><th style="text-align:left">Category</th><th style="text-align:left">Case</th><th style="text-align:left">Required</th><th style="text-align:left">Rule</th><th style="text-align:left">Samples</th></tr>

            <tr><td style="text-align:left">&quot;&quot;</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">af</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ak</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">am</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ar</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0</td><td style="text-align:left">@integer 0<br />@decimal 0.0, 0.00, 0.000, 0.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 100=3..10</td><td style="text-align:left">@integer 3~10, 103~110, 1003, …<br />@decimal 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 103.0, 1003.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 100=11..99</td><td style="text-align:left">@integer 11~26, 111, 1011, …<br />@decimal 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 111.0, 1011.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 100~102, 200~202, 300~302, 400~402, 500~502, 600, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ars</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0</td><td style="text-align:left">@integer 0<br />@decimal 0.0, 0.00, 0.000, 0.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 100=3..10</td><td style="text-align:left">@integer 3~10, 103~110, 1003, …<br />@decimal 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 103.0, 1003.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 100=11..99</td><td style="text-align:left">@integer 11~26, 111, 1011, …<br />@decimal 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 111.0, 1011.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 100~102, 200~202, 300~302, 400~402, 500~502, 600, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">as</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1,5,7,8,9,10</td><td style="text-align:left">@integer 1, 5, 7~10</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,3</td><td style="text-align:left">@integer 2, 3</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=4</td><td style="text-align:left">@integer 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=6</td><td style="text-align:left">@integer 6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 11~25, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">asa</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ast</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">az</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i % 10=1,2,5,7,8 or i % 100=20,50,70,80</td><td style="text-align:left">@integer 1, 2, 5, 7, 8, 11, 12, 15, 17, 18, 20~22, 25, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i % 10=3,4 or i % 1000=100,200,300,400,500,600,700,800,900</td><td style="text-align:left">@integer 3, 4, 13, 14, 23, 24, 33, 34, 43, 44, 53, 54, 63, 64, 73, 74, 100, 1003, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or i % 10=6 or i % 100=40,60,90</td><td style="text-align:left">@integer 0, 6, 16, 26, 36, 40, 46, 56, 106, 1006, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 9, 10, 19, 29, 30, 39, 49, 59, 69, 79, 109, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">be</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=1 and n % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 1.0, 21.0, 31.0, 41.0, 51.0, 61.0, 71.0, 81.0, 101.0, 1001.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=2..4 and n % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …<br />@decimal 2.0, 3.0, 4.0, 22.0, 23.0, 24.0, 32.0, 33.0, 102.0, 1002.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=0 or n % 10=5..9 or n % 100=11..14</td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@decimal 0.1~0.9, 1.1~1.7, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=2,3 and n % 100!=12,13</td><td style="text-align:left">@integer 2, 3, 22, 23, 32, 33, 42, 43, 52, 53, 62, 63, 72, 73, 82, 83, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 1, 4~17, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">bem</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">bez</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">bg</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">bh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">bm</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">bn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1,5,7,8,9,10</td><td style="text-align:left">@integer 1, 5, 7~10</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,3</td><td style="text-align:left">@integer 2, 3</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=4</td><td style="text-align:left">@integer 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=6</td><td style="text-align:left">@integer 6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 11~25, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">bo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">br</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=1 and n % 100!=11,71,91</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 81, 101, 1001, …<br />@decimal 1.0, 21.0, 31.0, 41.0, 51.0, 61.0, 81.0, 101.0, 1001.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=2 and n % 100!=12,72,92</td><td style="text-align:left">@integer 2, 22, 32, 42, 52, 62, 82, 102, 1002, …<br />@decimal 2.0, 22.0, 32.0, 42.0, 52.0, 62.0, 82.0, 102.0, 1002.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=3..4,9 and n % 100!=10..19,70..79,90..99</td><td style="text-align:left">@integer 3, 4, 9, 23, 24, 29, 33, 34, 39, 43, 44, 49, 103, 1003, …<br />@decimal 3.0, 4.0, 9.0, 23.0, 24.0, 29.0, 33.0, 34.0, 103.0, 1003.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n!=0 and n % 1000000=0</td><td style="text-align:left">@integer 1000000, …<br />@decimal 1000000.0, 1000000.00, 1000000.000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~8, 10~20, 100, 1000, 10000, 100000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, …</td></tr>
            <tr><td style="text-align:left">brx</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">bs</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2..4 and i % 100!=12..14 or f % 10=2..4 and f % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …<br />@decimal 0.2~0.4, 1.2~1.4, 2.2~2.4, 3.2~3.4, 4.2~4.4, 5.2, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 0.5~1.0, 1.5~2.0, 2.5~2.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ca</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1,3</td><td style="text-align:left">@integer 1, 3</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=4</td><td style="text-align:left">@integer 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ce</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ceb</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i=1,2,3 or v=0 and i % 10!=4,6,9 or v!=0 and f % 10!=4,6,9</td><td style="text-align:left">@integer 0~3, 5, 7, 8, 10~13, 15, 17, 18, 20, 21, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.3, 0.5, 0.7, 0.8, 1.0~1.3, 1.5, 1.7, 1.8, 2.0, 2.1, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 4, 6, 9, 14, 16, 19, 24, 26, 104, 1004, …<br />@decimal 0.4, 0.6, 0.9, 1.4, 1.6, 1.9, 2.4, 2.6, 10.4, 100.4, 1000.4, …</td></tr>
            <tr><td style="text-align:left">cgg</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">chr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ckb</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">cs</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=2..4 and v=0</td><td style="text-align:left">@integer 2~4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v!=0</td><td style="text-align:left">@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">cy</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0</td><td style="text-align:left">@integer 0<br />@decimal 0.0, 0.00, 0.000, 0.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=3</td><td style="text-align:left">@integer 3<br />@decimal 3.0, 3.00, 3.000, 3.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=6</td><td style="text-align:left">@integer 6<br />@decimal 6.0, 6.00, 6.000, 6.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 4, 5, 7~20, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0,7,8,9</td><td style="text-align:left">@integer 0, 7~9</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=3,4</td><td style="text-align:left">@integer 3, 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=5,6</td><td style="text-align:left">@integer 5, 6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 10~25, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">da</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1 or t!=0 and i=0,1</td><td style="text-align:left">@integer 1<br />@decimal 0.1~1.6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 2.0~3.4, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">de</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">dsb</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=1 or f % 100=1</td><td style="text-align:left">@integer 1, 101, 201, 301, 401, 501, 601, 701, 1001, …<br />@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=2 or f % 100=2</td><td style="text-align:left">@integer 2, 102, 202, 302, 402, 502, 602, 702, 1002, …<br />@decimal 0.2, 1.2, 2.2, 3.2, 4.2, 5.2, 6.2, 7.2, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=3..4 or f % 100=3..4</td><td style="text-align:left">@integer 3, 4, 103, 104, 203, 204, 303, 304, 403, 404, 503, 504, 603, 604, 703, 704, 1003, …<br />@decimal 0.3, 0.4, 1.3, 1.4, 2.3, 2.4, 3.3, 3.4, 4.3, 4.4, 5.3, 5.4, 6.3, 6.4, 7.3, 7.4, 10.3, 100.3, 1000.3, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 0.5~1.0, 1.5~2.0, 2.5~2.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">dv</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">dz</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ee</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">el</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">en</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=1 and n % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=2 and n % 100!=12</td><td style="text-align:left">@integer 2, 22, 32, 42, 52, 62, 72, 82, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=3 and n % 100!=13</td><td style="text-align:left">@integer 3, 23, 33, 43, 53, 63, 73, 83, 103, 1003, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 4~18, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">eo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">es</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">et</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">eu</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">fa</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ff</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0,1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.5</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 2.0~3.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">fi</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">fil</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i=1,2,3 or v=0 and i % 10!=4,6,9 or v!=0 and f % 10!=4,6,9</td><td style="text-align:left">@integer 0~3, 5, 7, 8, 10~13, 15, 17, 18, 20, 21, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.3, 0.5, 0.7, 0.8, 1.0~1.3, 1.5, 1.7, 1.8, 2.0, 2.1, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 4, 6, 9, 14, 16, 19, 24, 26, 104, 1004, …<br />@decimal 0.4, 0.6, 0.9, 1.4, 1.6, 1.9, 2.4, 2.6, 10.4, 100.4, 1000.4, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">fo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">fr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0,1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.5</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 2.0~3.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">fur</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">fy</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ga</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=3..6</td><td style="text-align:left">@integer 3~6<br />@decimal 3.0, 4.0, 5.0, 6.0, 3.00, 4.00, 5.00, 6.00, 3.000, 4.000, 5.000, 6.000, 3.0000, 4.0000, 5.0000, 6.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=7..10</td><td style="text-align:left">@integer 7~10<br />@decimal 7.0, 8.0, 9.0, 10.0, 7.00, 8.00, 9.00, 10.00, 7.000, 8.000, 9.000, 10.000, 7.0000, 8.0000, 9.0000, 10.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 11~25, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">gd</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1,11</td><td style="text-align:left">@integer 1, 11<br />@decimal 1.0, 11.0, 1.00, 11.00, 1.000, 11.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,12</td><td style="text-align:left">@integer 2, 12<br />@decimal 2.0, 12.0, 2.00, 12.00, 2.000, 12.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=3..10,13..19</td><td style="text-align:left">@integer 3~10, 13~19<br />@decimal 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 3.00</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 20~34, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1,11</td><td style="text-align:left">@integer 1, 11</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,12</td><td style="text-align:left">@integer 2, 12</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=3,13</td><td style="text-align:left">@integer 3, 13</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 4~10, 14~21, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">gl</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">gsw</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">gu</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,3</td><td style="text-align:left">@integer 2, 3</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=4</td><td style="text-align:left">@integer 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=6</td><td style="text-align:left">@integer 6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5, 7~20, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">guw</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">gv</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1</td><td style="text-align:left">@integer 1, 11, 21, 31, 41, 51, 61, 71, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2</td><td style="text-align:left">@integer 2, 12, 22, 32, 42, 52, 62, 72, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=0,20,40,60,80</td><td style="text-align:left">@integer 0, 20, 40, 60, 80, 100, 120, 140, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v!=0</td><td style="text-align:left">@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 3~10, 13~19, 23, 103, 1003, …</td></tr>
            <tr><td style="text-align:left">ha</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">haw</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">he</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=2 and v=0</td><td style="text-align:left">@integer 2</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and n!=0..10 and n % 10=0</td><td style="text-align:left">@integer 20, 30, 40, 50, 60, 70, 80, 90, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 101, 1001, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">hi</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,3</td><td style="text-align:left">@integer 2, 3</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=4</td><td style="text-align:left">@integer 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=6</td><td style="text-align:left">@integer 6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5, 7~20, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">hr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2..4 and i % 100!=12..14 or f % 10=2..4 and f % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …<br />@decimal 0.2~0.4, 1.2~1.4, 2.2~2.4, 3.2~3.4, 4.2~4.4, 5.2, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 0.5~1.0, 1.5~2.0, 2.5~2.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">hsb</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=1 or f % 100=1</td><td style="text-align:left">@integer 1, 101, 201, 301, 401, 501, 601, 701, 1001, …<br />@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=2 or f % 100=2</td><td style="text-align:left">@integer 2, 102, 202, 302, 402, 502, 602, 702, 1002, …<br />@decimal 0.2, 1.2, 2.2, 3.2, 4.2, 5.2, 6.2, 7.2, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=3..4 or f % 100=3..4</td><td style="text-align:left">@integer 3, 4, 103, 104, 203, 204, 303, 304, 403, 404, 503, 504, 603, 604, 703, 704, 1003, …<br />@decimal 0.3, 0.4, 1.3, 1.4, 2.3, 2.4, 3.3, 3.4, 4.3, 4.4, 5.3, 5.4, 6.3, 6.4, 7.3, 7.4, 10.3, 100.3, 1000.3, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 0.5~1.0, 1.5~2.0, 2.5~2.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">hu</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1,5</td><td style="text-align:left">@integer 1, 5</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~4, 6~17, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">hy</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0,1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.5</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 2.0~3.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ia</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">id</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ig</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ii</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">in</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">io</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">is</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">t=0 and i % 10=1 and i % 100!=11 or t!=0</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1~1.6, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">it</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=11,8,80,800</td><td style="text-align:left">@integer 8, 11, 80, 800</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~7, 9, 10, 12~17, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">iu</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">iw</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=2 and v=0</td><td style="text-align:left">@integer 2</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and n!=0..10 and n % 10=0</td><td style="text-align:left">@integer 20, 30, 40, 50, 60, 70, 80, 90, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 101, 1001, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ja</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">jbo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">jgo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ji</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">jmc</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">jv</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">jw</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ka</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or i % 100=2..20,40,60,80</td><td style="text-align:left">@integer 0, 2~16, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 21~36, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">kab</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0,1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.5</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 2.0~3.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">kaj</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">kcg</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">kde</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">kea</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">kk</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=6 or n % 10=9 or n % 10=0 and n!=0</td><td style="text-align:left">@integer 6, 9, 10, 16, 19, 20, 26, 29, 30, 36, 39, 40, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~5, 7, 8, 11~15, 17, 18, 21, 101, 1001, …</td></tr>
            <tr><td style="text-align:left">kkj</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">kl</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">km</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">kn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ko</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ks</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ksb</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ksh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0</td><td style="text-align:left">@integer 0<br />@decimal 0.0, 0.00, 0.000, 0.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ku</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">kw</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0</td><td style="text-align:left">@integer 0<br />@decimal 0.0, 0.00, 0.000, 0.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 100=2,22,42,62,82 or n % 1000=0 and n % 100000=1000..20000,40000,60000,80000 or n!=0 and n % 1000000=100000</td><td style="text-align:left">@integer 2, 22, 42, 62, 82, 102, 122, 142, 1002, …<br />@decimal 2.0, 22.0, 42.0, 62.0, 82.0, 102.0, 122.0, 142.0, 1002.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 100=3,23,43,63,83</td><td style="text-align:left">@integer 3, 23, 43, 63, 83, 103, 123, 143, 1003, …<br />@decimal 3.0, 23.0, 43.0, 63.0, 83.0, 103.0, 123.0, 143.0, 1003.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n!=1 and n % 100=1,21,41,61,81</td><td style="text-align:left">@integer 21, 41, 61, 81, 101, 121, 141, 161, 1001, …<br />@decimal 21.0, 41.0, 61.0, 81.0, 101.0, 121.0, 141.0, 161.0, 1001.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 4~19, 100, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1..4 or n % 100=1..4,21..24,41..44,61..64,81..84</td><td style="text-align:left">@integer 1~4, 21~24, 41~44, 61~64, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=5 or n % 100=5</td><td style="text-align:left">@integer 5, 105, 205, 305, 405, 505, 605, 705, 1005, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 6~20, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ky</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">lag</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0</td><td style="text-align:left">@integer 0<br />@decimal 0.0, 0.00, 0.000, 0.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0,1 and n!=0</td><td style="text-align:left">@integer 1<br />@decimal 0.1~1.6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 2.0~3.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">lb</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">lg</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">lkt</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ln</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">lo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">lt</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=1 and n % 100!=11..19</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 1.0, 21.0, 31.0, 41.0, 51.0, 61.0, 71.0, 81.0, 101.0, 1001.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=2..9 and n % 100!=11..19</td><td style="text-align:left">@integer 2~9, 22~29, 102, 1002, …<br />@decimal 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 22.0, 102.0, 1002.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">f!=0</td><td style="text-align:left">@decimal 0.1~0.9, 1.1~1.7, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 10~20, 30, 40, 50, 60, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">lv</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=0 or n % 100=11..19 or v=2 and f % 100=11..19</td><td style="text-align:left">@integer 0, 10~20, 30, 40, 50, 60, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=1 and n % 100!=11 or v=2 and f % 10=1 and f % 100!=11 or v!=2 and f % 10=1</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1, 1.0, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~9, 22~29, 102, 1002, …<br />@decimal 0.2~0.9, 1.2~1.9, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">mas</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">mg</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">mgo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">mk</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 0.2~1.0, 1.2~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i % 10=1 and i % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i % 10=2 and i % 100!=12</td><td style="text-align:left">@integer 2, 22, 32, 42, 52, 62, 72, 82, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i % 10=7,8 and i % 100!=17,18</td><td style="text-align:left">@integer 7, 8, 27, 28, 37, 38, 47, 48, 57, 58, 67, 68, 77, 78, 87, 88, 107, 1007, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~6, 9~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ml</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">mn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">mo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v!=0 or n=0 or n % 100=2..19</td><td style="text-align:left">@integer 0, 2~16, 102, 1002, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 20~35, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">mr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,3</td><td style="text-align:left">@integer 2, 3</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=4</td><td style="text-align:left">@integer 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ms</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">mt</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0 or n % 100=2..10</td><td style="text-align:left">@integer 0, 2~10, 102~107, 1002, …<br />@decimal 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 10.0, 102.0, 1002.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 100=11..19</td><td style="text-align:left">@integer 11~19, 111~117, 1011, …<br />@decimal 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 111.0, 1011.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 20~35, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">my</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">nah</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">naq</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">nb</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">nd</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ne</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1..4</td><td style="text-align:left">@integer 1~4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">nl</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">nn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">nnh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">no</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">nqo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">nr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">nso</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ny</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">nyn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">om</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">or</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1,5,7..9</td><td style="text-align:left">@integer 1, 5, 7~9</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2,3</td><td style="text-align:left">@integer 2, 3</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=4</td><td style="text-align:left">@integer 4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=6</td><td style="text-align:left">@integer 6</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 10~24, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">os</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">pa</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">pap</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">pl</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2..4 and i % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i!=1 and i % 10=0..1 or v=0 and i % 10=5..9 or v=0 and i % 100=12..14</td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">prg</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=0 or n % 100=11..19 or v=2 and f % 100=11..19</td><td style="text-align:left">@integer 0, 10~20, 30, 40, 50, 60, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=1 and n % 100!=11 or v=2 and f % 10=1 and f % 100!=11 or v!=2 and f % 10=1</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1, 1.0, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~9, 22~29, 102, 1002, …<br />@decimal 0.2~0.9, 1.2~1.9, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ps</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">pt</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.5</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 2.0~3.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">pt-PT</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">rm</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ro</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v!=0 or n=0 or n % 100=2..19</td><td style="text-align:left">@integer 0, 2~16, 102, 1002, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 20~35, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">rof</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ru</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1 and i % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2..4 and i % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=0 or v=0 and i % 10=5..9 or v=0 and i % 100=11..14</td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">rwk</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sah</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">saq</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sc</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=11,8,80,800</td><td style="text-align:left">@integer 8, 11, 80, 800</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~7, 9, 10, 12~17, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">scn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=11,8,80,800</td><td style="text-align:left">@integer 8, 11, 80, 800</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~7, 9, 10, 12~17, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">sd</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">sdh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">se</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">seh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ses</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sg</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2..4 and i % 100!=12..14 or f % 10=2..4 and f % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …<br />@decimal 0.2~0.4, 1.2~1.4, 2.2~2.4, 3.2~3.4, 4.2~4.4, 5.2, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 0.5~1.0, 1.5~2.0, 2.5~2.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">shi</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2..10</td><td style="text-align:left">@integer 2~10<br />@decimal 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 11~26, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~1.9, 2.1~2.7, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">si</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0,1 or i=0 and f=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 0.1, 1.0, 0.00, 0.01, 1.00, 0.000, 0.001, 1.000, 0.0000, 0.0001, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.2~0.9, 1.1~1.8, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">sk</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=2..4 and v=0</td><td style="text-align:left">@integer 2~4</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v!=0</td><td style="text-align:left">@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">sl</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=1</td><td style="text-align:left">@integer 1, 101, 201, 301, 401, 501, 601, 701, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=2</td><td style="text-align:left">@integer 2, 102, 202, 302, 402, 502, 602, 702, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 100=3..4 or v!=0</td><td style="text-align:left">@integer 3, 4, 103, 104, 203, 204, 303, 304, 403, 404, 503, 504, 603, 604, 703, 704, 1003, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">sma</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">smi</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">smj</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">smn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sms</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">two</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=2</td><td style="text-align:left">@integer 2<br />@decimal 2.0, 2.00, 2.000, 2.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">so</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sq</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=4 and n % 100!=14</td><td style="text-align:left">@integer 4, 24, 34, 44, 54, 64, 74, 84, 104, 1004, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2, 3, 5~17, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">sr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …<br />@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2..4 and i % 100!=12..14 or f % 10=2..4 and f % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …<br />@decimal 0.2~0.4, 1.2~1.4, 2.2~2.4, 3.2~3.4, 4.2~4.4, 5.2, 10.2, 100.2, 1000.2, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0, 0.5~1.0, 1.5~2.0, 2.5~2.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ss</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ssy</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">st</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">sv</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=1,2 and n % 100!=11,12</td><td style="text-align:left">@integer 1, 2, 21, 22, 31, 32, 41, 42, 51, 52, 61, 62, 71, 72, 81, 82, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">sw</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">syr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ta</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">te</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">teo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">th</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ti</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">tig</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">tk</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=6,9 or n=10</td><td style="text-align:left">@integer 6, 9, 10, 16, 19, 26, 29, 36, 39, 106, 1006, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~5, 7, 8, 11~15, 17, 18, 20, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">tl</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i=1,2,3 or v=0 and i % 10!=4,6,9 or v!=0 and f % 10!=4,6,9</td><td style="text-align:left">@integer 0~3, 5, 7, 8, 10~13, 15, 17, 18, 20, 21, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.3, 0.5, 0.7, 0.8, 1.0~1.3, 1.5, 1.7, 1.8, 2.0, 2.1, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 4, 6, 9, 14, 16, 19, 24, 26, 104, 1004, …<br />@decimal 0.4, 0.6, 0.9, 1.4, 1.6, 1.9, 2.4, 2.6, 10.4, 100.4, 1000.4, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">tn</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">to</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">tr</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ts</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">tzm</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1 or n=11..99</td><td style="text-align:left">@integer 0, 1, 11~24<br />@decimal 0.0, 1.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~10, 100~106, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ug</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">uk</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=1 and i % 100!=11</td><td style="text-align:left">@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=2..4 and i % 100!=12..14</td><td style="text-align:left">@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">many</td><td style="text-align:left">&#9745;</td><td style="text-align:left">v=0 and i % 10=0 or v=0 and i % 10=5..9 or v=0 and i % 100=11..14</td><td style="text-align:left">@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">few</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n % 10=3 and n % 100!=13</td><td style="text-align:left">@integer 3, 23, 33, 43, 53, 63, 73, 83, 103, 1003, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~2, 4~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">ur</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">uz</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">wa</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=0..1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">wae</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">ve</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">vi</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">vo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">wo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">vun</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">xh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">xog</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">n=1</td><td style="text-align:left">@integer 1<br />@decimal 1.0, 1.00, 1.000, 1.0000</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">yi</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=1 and v=0</td><td style="text-align:left">@integer 1</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">yo</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left">yue</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">zh</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>
            <tr><td style="text-align:left">zu</td><td style="text-align:left">cardinal</td><td style="text-align:left">zero</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=0</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">one</td><td style="text-align:left">&#9745;</td><td style="text-align:left">i=0 or n=1</td><td style="text-align:left">@integer 0, 1<br />@decimal 0.0~1.0, 0.00~0.04</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 2~17, 100, 1000, 10000, 100000, 1000000, …<br />@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left">ordinal</td><td style="text-align:left">one</td><td style="text-align:left">&#9744;</td><td style="text-align:left">n=1</td><td style="text-align:left"></td></tr>
            <tr><td style="text-align:left"></td><td style="text-align:left"></td><td style="text-align:left">other</td><td style="text-align:left">&#9745;</td><td style="text-align:left"></td><td style="text-align:left">@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</td></tr>

        </tbody>
    </table>
</details>


This table is derived from rules of [Unicode CLDR](https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html) "Data files".

# PluralRules Parameter
To use plurality, the key must have "PluralRules" parameter either in the ILine or in the localization file.
There are five ways to configure the plurality rule:

1. Add class name of plural rules into the localization file (*recommended way*). The value "Unicode.CLDR35" uses [Unicode CLDR35 plural rules](http://cldr.unicode.org/index/cldr-spec/plural-rules). 
The class is derivate of CLDR35 and is licensed under [Unicode License agreement](https://www.unicode.org/license.html) as "Data Files".
<details>
  <summary>PluralRules="Unicode.CLDR35" (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Unicode.CLDR35">

  <!-- Fallback string, for "" culture -->
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
IPluralRules rules = PluralRulesResolver.Default.Resolve("Unicode.CLDR35");
ILine root = LineRoot.Global.PluralRules(rules);
```

4. Add *class name* of **IPluralRules** to line.

```csharp
ILine root = LineRoot.Global.PluralRules("Unicode.CLDR35");
```

5. Add unicode plural rules expression to line. ([Unicode CLDR Plural Rule Syntax](https://unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules))

```csharp
ILine root = LineRoot.Global.PluralRules("[Category=cardinal,Case=zero,Optional=1]n=0[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true");
```
<br/>
<br/>
