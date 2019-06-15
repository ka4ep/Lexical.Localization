# Format and Values
**ILine** is a localization line. It can be embedded with a default string.

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

<b>.Formulate(<i>$string</i>)</b> appends interpolated strings. It's equivalent to appending **.Format()** and **.Value()** parts.

```csharp
DateTime time = DateTime.Now;
Console.WriteLine(LineRoot.Global.Key("Time").Formulate($"It is now {time:d} at {time:t}"));
```

Enumeration localization strings are searched with key <i>"Assembly:asm:Type:enumType:Key:enumCase"</i>.

```csharp
Permissions permissions = Permissions.Add | Permissions.Modify | Permissions.Remove;
Console.WriteLine( LineRoot.Global.Key("Permission").Formulate($"User permissions {permissions}") );
```

```csharp
[Flags]
public enum Permissions { Add = 1, Remove = 2, Modify = 4 }
```

# Culture
The format culture can be enforced with <b>.Culture(<i>CultureInfo</i>)</b>, without changing the thread-local culture variable.

```csharp
Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Culture("sv").Value(DateTime.Now));
Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Culture("de").Value(DateTime.Now));
```

# Inlining
Default strings can be *inlined* for multiple cultures.

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
ILine line = LineRoot.Global.PluralRules("Unicode.CLDR35")
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
ILine line = LineRoot.Global.PluralRules("Unicode.CLDR35")
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

# Files
Localization assets can be read from files and placed into the global **LineRoot.Global**.

```csharp
IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.xml");
LineRoot.Builder.AddAsset(asset).Build();
ILine line = LineRoot.Global.Key("Cats").Format("{0} cat(s)");
// Print with plurality
for (int cats = 0; cats <= 2; cats++)
    Console.WriteLine(line.Culture("fi").Value(cats));
```
<details>
  <summary>PluralityExample0a.xml (<u>click here</u>)</summary>

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

Or, assets can be loaded and placed into a new ILineRoot.

```csharp
IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.xml");
ILineRoot root = new LineRoot(asset, new CulturePolicy());
ILine line = root.Key("Cats").Format("{0} cat(s)");
```

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

# Logging
Loggers can be appended to *ILine* for debugging purposes.

```csharp
ILine root = LineRoot.Global.Logger(Console.Out, LineStatusSeverity.Ok);
ILine line = root.Type("MyClass").Key("hello").Format("Hello, {0}.");
Console.WriteLine(line.Value("Corellia Melody"));
```

# StringFormat
Different string formats, such as C#'s *String.Format*, are supported. **IStringFormat** is an abstraction to string formats.

```csharp
ILine line1a = LineRoot.Global.Key("Cats").Format("{0} cat(s)");
IString string1 = CSharpFormat.Default.Parse("{0} cat(s)");
ILine line1b = LineRoot.Global.Key("Cats").String(string1);

ILine line2a = LineRoot.Global.Key("Ok").Text("{in braces}");
IString string2 = TextFormat.Default.Parse("{in braces}");
ILine line2b = LineRoot.Global.Key("Ok").String(string2);
```

# Culture Policy
**ICulturePolicy** determines which culture to apply.

```csharp
ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentThreadUICulture();
ILine root = new LineRoot(null, culturePolicy: culturePolicy);
```

# Resolving string
<b><i>ILine</i>.ResolveString()</b> returns more information about the string resolve process than *ILine.ToString()*.

```csharp
LineString resolved_string = line.Value("Corellia Melody").ResolveString();
Console.WriteLine(resolved_string.Status);
```

# String Localizer
**StringLocalizerRoot.Global** is same root as **LineRoot.Global** with the difference, that parts derived from it implement *IStringLocalizer* and *IStringLocalizerFactory*.

```csharp
ILine line = StringLocalizerRoot.Global.Type("MyClass").Key("hello").Format("Hello, {0}.");
IStringLocalizer localizer = line.AsStringLocalizer();
IStringLocalizerFactory localizerFactory = line.AsStringLocalizerFactory();
```

New **StringLocalizerRoot** can also be constructed.

```csharp
ILineRoot root = new StringLocalizerRoot(null, new CulturePolicy());
ILine line = root.Type("MyClass").Key("hello").Format("Hello, {0}.");
IStringLocalizer localizer = line.AsStringLocalizer();
IStringLocalizerFactory localizerFactory = line.AsStringLocalizerFactory();
```

*IStringLocalizer* reference can be adapted from regular **LineRoot** as well, but causes an additional heap object to be instantiated.

```csharp
ILine line = LineRoot.Global.Type("MyClass").Key("hello").Format("Hello, {0}.");
IStringLocalizer localizer = line.AsStringLocalizer();
IStringLocalizerFactory localizerFactory = line.AsStringLocalizerFactory();
```

# Example Class
Keys can be placed in static references if the singleton **LineRoot.Global** is used.

```csharp
public class MyClass
{
    /// <summary>
    /// Localization root for this class.
    /// </summary>
    static ILine localization = LineRoot.Global.Assembly("MyLibrary").Type<MyClass>();

    /// <summary>
    /// Localization key "Ok" with a default string, and couple of inlined strings for two cultures.
    /// </summary>
    static ILine ok = localization.Key("Success")
            .Text("Success")
            .fi("Onnistui")
            .sv("Det funkar");

    /// <summary>
    /// Localization key "Error" with a default string, and couple of inlined ones for two cultures.
    /// </summary>
    static ILine error = localization.Key("Error")
            .Format("Error (Code=0x{0:X8})")
            .fi("Virhe (Koodi=0x{0:X8})")
            .sv("Sönder (Kod=0x{0:X8})");

    public void DoOk()
    {
        Console.WriteLine(ok);
    }

    public void DoError()
    {
        Console.WriteLine(error.Value(0x100));
    }
    
    public void DoExample()
    {
        string msg = "";
        Console.WriteLine(localization.Key("Msg").Formulate($"You received a message: \"{msg}\""));
    }
}
```

# Links
* [Website](http://lexical.fi/Localization/index.html)
* [Github](https://github.com/tagcode/Lexical.Localization)
* [Nuget](https://www.nuget.org/packages/Lexical.Localization/)
* [Slack](https://lexicalworkspace.slack.com/messages/CKLADPL7P/)
