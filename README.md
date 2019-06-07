# Tutorial
**ILine** is a key to localization asset. It can also be embedded with a default string.

```csharp
ILine line = LineRoot.Global.Type("MyClass").Key("hello").Format("Hello, {0}.");
```

Values are provided with <b>.Value(<i>object[]</i>)</b>.

```csharp
Console.WriteLine(line.Value("Corellia Melody"));
```

Providing **.Format()** and **.Value()** is equivalent to **String.Format()**.

```csharp
Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Value(DateTime.Now));
Console.WriteLine(String.Format("It is now {0:d} at {0:t}", DateTime.Now));
```

Default strings can be *inlined* to multiple cultures.

```csharp
ILine line = LineRoot.Global
    .Key("hello")
    .Format("Hello, {0}.")
    .Inline("Culture:fi", "Hei, {0}")
    .Inline("Culture:de", "Hallo, {0}");
Console.WriteLine(line.Value("Corellia Melody"));
```

And *inlined* with different plurality cases.

```csharp
ILine line = LineRoot.Global.PluralRules(CLDR35.Instance)
        .Type("MyClass").Key("Cats")
        .Format("{cardinal:0} cat(s)")
        .Inline("N:zero", "no cats")
        .Inline("N:one", "a cat")
        .Inline("N:other", "{0} cats");
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(line.Value(cats));
```

And with permutations of different cultures and plurality cases.

```csharp
ILine line = LineRoot.Global.PluralRules(CLDR35.Instance)
        .Type("MyClass").Key("Cats")
        .Format("{0} cat(s)")
        .Inline("Culture:en", "{cardinal:0} cat(s)")
        .Inline("Culture:en:N:zero", "no cats")
        .Inline("Culture:en:N:one", "a cat")
        .Inline("Culture:en:N:other", "{0} cats")
        .Inline("Culture:fi", "{cardinal:0} kissa(a)")
        .Inline("Culture:fi:N:zero", "ei kissoja")
        .Inline("Culture:fi:N:one", "yksi kissa")
        .Inline("Culture:fi:N:other", "{0} kissaa");

// Print with plurality and to culture "en"
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(line.Culture("en").Value(cats));
```

Localization assets can be read from files.

```csharp
IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.xml");
ILineRoot root = new LineRoot(asset, new CulturePolicy());
ILine line = root.Key("Cats").Format("{0} cat(s)");
// Print with plurality
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(line.Value(cats));
```
<details>
  <summary>PluralityExample0a.xml (<u>click here</u>)</summary>

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture"
              xmlns:Key="urn:lexical.fi:Key"
              xmlns:N="urn:lexical.fi:N"
              xmlns="urn:lexical.fi"
              PluralRules="Lexical.Localization.CLDR35">

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

**IAsset** is an abstraction to localization lines and localized resources. 
Implementing classes can be provided within code.

```csharp
List<ILine> lines = new List<ILine> {
    LineAppender.Default.Type("MyClass").Key("Hello").Format("Hello, {0}"),
    LineAppender.Default.Type("MyClass").Key("Hello").Culture("fi").Format("Hei, {0}"),
    LineAppender.Default.Type("MyClass").Key("Hello").Culture("de").Format("Hallo, {0}")
};
IAsset asset = new StringAsset().Add(lines).Load();
ILineRoot root = new LineRoot(asset, new CulturePolicy());
ILine line = root.Type("MyClass").Key("Hello");
Console.WriteLine(line.Value("Corellia Melody"));
```

Loggers can be appended to *ILine* for debugging purposes.

```csharp
ILine root = LineRoot.Global.Logger(Console.Out, LineStatusSeverity.Ok);
ILine line = root.Type("MyClass").Key("hello").Format("Hello, {0}.");
Console.WriteLine(line.Value("Corellia Melody"));
```

Different string formats, such as C#'s *String.Format*, are supported. **IStringFormat** is an abstraction to string formats.

```csharp
ILine line1a = LineRoot.Global.Key("Cats").Format("{0} cat(s)");
IString string1 = CSharpFormat.Default.Parse("{0} cat(s)");
ILine line1b = LineRoot.Global.Key("Cats").String(string1);

ILine line2a = LineRoot.Global.Key("Ok").Text("{in braces}");
IString string2 = TextFormat.Default.Parse("{in braces}");
ILine line2b = LineRoot.Global.Key("Ok").String(string2);
```

**ICulturePolicy** determines which culture to apply.

```csharp
ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentThreadUICulture();
ILine root = new LineRoot(null, culturePolicy: culturePolicy);
```

*ILine.ToString()* is a shortcut to <b><i>ILine</i>.ResolveString()</b>, which returns with additional information about the resolve process. 

```csharp
LineString resolved_string = line.Value("Corellia Melody").ResolveString();
Console.WriteLine(resolved_string.Status);
```

**Links**
* [Website](http://lexical.fi/Localization/index.html)
* [Github](https://github.com/tagcode/Lexical.Localization)
* [Nuget](https://www.nuget.org/packages/Lexical.Localization/)
