# Line
**ILine** is a set of interfaces for broad variety of features.
Lines can carry hints, key references and strings and binary resources. 
They can be implemented with one class, but are are typically used as compositions of smaller classes that form linked lists.

```csharp
ILine key = LineRoot.Global.PluralRules("Unicode.CLDR35").Key("Key").Format("Hello, {0}");
```
![linked list](linkedlist.svg)
<br/>

Or constructed to span a tree structure (trie).

```csharp
ILineRoot root = new LineRoot();
ILine hint1 = root.PluralRules("Unicode.CLDR35");
ILine section1 = hint1.Section("Section2");
ILine section1_1 = hint1.Section("Section1.1");
ILine key1_1_1 = section1_1.Key("Key1");
ILine key1_1_2 = section1_1.Key("Key2");
ILine value1_1_1 = key1_1_1.Format("Hello, {0}");
// ...
```
![tree](tree.svg)
<br/>

# Parts
<b>.Asset(<i>IAsset</i>)</b> adds extra asset as line part.

```csharp
IAsset resourceAsset = new ResourceDictionary( new Dictionary<ILine, byte[]>() );
ILine line = LineRoot.Global.Asset(resourceAsset);
```

<b>.Logger(<i>ILogger</i>)</b> adds logger as line part.

```csharp
ILine line = LineRoot.Global.Logger(Console.Out, LineStatusSeverity.Ok);
```

<b>.CulturePolicy(<i>ICulturePolicy</i>)</b> adds a culture policy that determines which culture is active in a given execution context.	

```csharp
ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentCulture();
ILine line = LineRoot.Global.CulturePolicy(culturePolicy);
```

# Hints
Hints are line parts that can be appended from localization file as well as from *ILine*.

**Hints.ini**
 

```ini
[PluralRules:Unicode.CLDR35]
Key:Error:FormatProvider:docs.CustomFormat,docs = Error, (Date = {0:DATE})
Key:Ok:StringFormat:Lexical.Localization.StringFormat.TextFormat,Lexical.Localization = OK {C# format is not in use here}.

```

<b>.StringFormat(<i>IStringFormat</i>)</b> and <b>.StringFormat(<i>string</i>)</b> add a string format, that determines the way the consequtive "String" parameters are parsed.

```csharp
ILine line1 = LineRoot.Global.StringFormat(TextFormat.Default).String("Text");
ILine line2 = LineRoot.Global.StringFormat("Lexical.Localization.StringFormat.TextFormat,Lexical.Localization")
    .String("Text");
```

<b>.PluralRules(<i>IPluralRules</i>)</b> and <b>.PluralRules(<i>string</i>)</b> add plural rules that determine how plurality are used in further line parts.

```csharp
IPluralRules pluralRules = PluralRulesResolver.Default.Resolve("Unicode.CLDR35");
ILine line1 = LineRoot.Global.PluralRules(pluralRules);
ILine line2 = LineRoot.Global.PluralRules("Unicode.CLDR35");
ILine line3 = LineRoot.Global.PluralRules("[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true");
```

<b>.FormatProvider(<i>IFormatProvider</i>)</b> and <b>.FormatProvider(<i>string</i>)</b> add a custom format provider that provides special format handling.

```csharp
IFormatProvider customFormat = new CustomFormat();
ILine line1 = LineRoot.Global.FormatProvider(customFormat).Format("{0:DATE}").Value(DateTime.Now);
ILine line2 = LineRoot.Global.FormatProvider("docs.CustomFormat,docs").Format("{0:DATE}").Value(DateTime.Now);
```

```csharp
public class CustomFormat : IFormatProvider, ICustomFormatter
{
    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        if (format == "DATE" && arg is DateTime time)
        {
            return time.Date.ToString();
        }
        return null;
    }

    public object GetFormat(Type formatType)
        => formatType == typeof(ICustomFormatter) ? this : default;
}
```

<b>.StringResolver(<i>IStringResolver</i>)</b> adds *IStringResolver* as line part.

```csharp
ResolveSource[] resolveSequence = 
    new ResolveSource[] { ResolveSource.Inline, ResolveSource.Asset, ResolveSource.Line };

IStringResolver stringResolver = new StringResolver(Resolvers.Default, resolveSequence);
ILine line = LineRoot.Global.StringResolver(stringResolver);
```

<b>.StringResolver(<i>string</i>)</b> adds assembly qualified class name to *IStringResolver*.

```csharp
ILine line = LineRoot.Global.StringResolver("Lexical.Localization.StringFormat.StringResolver");
```

<b>.ResourceResolver(<i>IResourceResolver</i>)</b> adds *IResourceResolver* as line part.

```csharp
ResolveSource[] resolveSequence = 
    new ResolveSource[] { ResolveSource.Inline, ResolveSource.Asset, ResolveSource.Line };

IResourceResolver resourceResolver = new ResourceResolver(Resolvers.Default, resolveSequence);
ILine line = LineRoot.Global.ResourceResolver(resourceResolver);
```

<b>.ResourceResolver(<i>string</i>)</b> adds assembly qualified class name to *IResourceResolver*.

```csharp
ILine line = LineRoot.Global.ResourceResolver("Lexical.Localization.Resource.ResourceResolver");
```

# Canonically compared keys
Key parts give *ILine*s hash-equal comparison information. Key parts are used to match the line to a corresponding line of another culture in
localization files.

Canonically compared key parts are compared so that the position of occurance is relevant.

<b>.Key(<i>string</i>)</b> appends "Key" key part.

```csharp
ILine line = LineRoot.Global.Key("Ok");
```

<b>.Section(<i>string</i>)</b> appends "Section" key part.

```csharp
ILine line = LineRoot.Global.Section("Resources").Key("Ok");
```

<b>.Location(<i>string</i>)</b> appends "Location" key part.

```csharp
ILine line = LineRoot.Global.Location(@"c:\dir");
```

<b>.BaseName(<i>string</i>)</b> appends "BaseName" key part.

```csharp
ILine line = LineRoot.Global.BaseName("docs.Resources");
```


# Non-canonically compared keys
Non-canonically compared key parts are compared so that the position doesn't matter. The first occurance of each type is considered effective, rest are ignored.

<b>.Assembly(<i>Assembly</i>)</b> and <b>.Assembly(<i>string</i>)</b> append "Assembly" key. 

```csharp
Assembly asm = typeof(ILine_Examples).Assembly;
ILine line1 = LineRoot.Global.Assembly(asm);
ILine line2 = LineRoot.Global.Assembly("docs");
```

<b>.Culture(<i>CultureInfo</i>)</b> and <b>.Culture(<i>string</i>)</b> append "Culture" key.

```csharp
CultureInfo culture = CultureInfo.GetCultureInfo("en");
ILine line1 = LineRoot.Global.Culture(culture);
ILine line2 = LineRoot.Global.Culture("en");
```

<b>.Type&lt;T&gt;(<i></i>)</b>, <b>.Type(<i>Type</i>)</b> and <b>.Type(<i>string</i>)</b> append "Type" key.

```csharp
ILine line1 = LineRoot.Global.Type<ILine_Examples>();
ILine line2 = LineRoot.Global.Type(typeof(ILine_Examples));
```

# Strings
<b>.String(<i>IString</i>)</b> appends preparsed default string value.

```csharp
IString str = CSharpFormat.Default.Parse("ErrorCode = 0x{0:X8}");
ILine line = LineRoot.Global.Key("Error").String(str);
```

<b>.String(<i>string</i>)</b> appends "String" hint. This needs preceding "StringFormat" part in order to determine the way to parse.
If "StringFormat" is not provided, then C# format is used as default.

```csharp
ILine line = LineRoot.Global.Key("Error").StringFormat(CSharpFormat.Default).String("ErrorCode = 0x{0:X8}");
```

<b>.Format(<i>string</i>)</b> appends C# String formulation value.

```csharp
ILine line = LineRoot.Global.Key("Error").Format("ErrorCode = 0x{0:X8}");
```

<b>.Format(<i>$interpolated_string</i>)</b> appends formulation and value using string interpolation.

```csharp
int code = 0x100;
ILine line = LineRoot.Global.Key("Error").Format($"ErrorCode = 0x{code:X8}");
```

<b>.Text(<i>string</i>)</b> appends plain text "String" value.

```csharp
ILine line = LineRoot.Global.Key("Hello").Text("Hello World");
```

<b>.ResolveString()</b> resolves the string in current executing context. The result struct is <b>LineString</b>.

```csharp
ILine line = LineRoot.Global.Key("Error").Format("ErrorCode={0}").Value(0x100);
LineString result = line.ResolveString();
```

# Inlining
Code can be [automatically scanned](http://lexical.fi/sdk/Localization/docs/InlineScanner/index.html) for inlined strings and exported to localization files.
They can be used as templates for further translation process. 
This way the templates don't need to be manually updated as the code evolves.
<br/>

<b>.Inline(<i>string</i> subkey, <i>string</i> text)</b> appends an inlined sub-line.

```csharp
ILine line = LineRoot.Global.Section("Section").Key("Success")
    .Format("Success")                                  // Add inlining to the root culture ""
    .Inline("Culture:en", "Success")                   // Add inlining to culture "en"
    .Inline("Culture:fi", "Onnistui")                  // Add inlining to culture "fi"
    .Inline("Culture:sv", "Det funkar");               // Add inlining to culture "sv"
```

<b>.en(<i>string</i>)</b> appends inlined value for that culture "en".

```csharp
ILine line = LineRoot.Global.Section("Section").Key("Success")
    .Format("Success")                                  // Add inlining to the root culture ""
    .en("Success")                                      // Add inlining to culture "en"
    .fi("Onnistui")                                     // Add inlining to culture "fi"
    .sv("Det funkar");                                  // Add inlining to culture "sv"
```

It's recommended to put inlined lines to variables for better performance. Inline allocates a Dictionary internally.

```csharp
class MyController__
{
    static ILine localization = LineRoot.Global.Assembly("docs").Type<MyControllerB>();
    static ILine Success = localization.Key("Success").Format("Success").sv("Det funkar").fi("Onnistui");

    public string Do()
    {
        return Success.ToString();
    }
}
```

# Enumerations
Enumerables can be localized just as any other type. 

```csharp
[Flags]
enum CarFeature
{
    // Fuel Type
    Electric = 0x0001,
    Petrol = 0x0002,
    NaturalGas = 0x0003,

    // Door count
    TwoDoors = 0x0010,
    FourDoors = 0x0020,
    FiveDoors = 0x0030,

    // Color
    Red = 0x0100,
    Black = 0x0200,
    White = 0x0300,
}
```

<b>.Assembly&lt;T&gt;()</b> and <b>.Type&lt;T&gt;()</b> appends "Assembly" and "Type" keys to refer to an enumeration type.

```csharp
ILine carFeature = LineRoot.Global.Assembly("docs").Type<CarFeature>().Format("{0}");
```

<b>.InlineEnum&lt;T&gt;()</b> inlines every case of enum for culture "". It applies *[Description]* attribute when available.

```csharp
ILine carFeature = LineRoot.Global.Assembly("docs").Type<CarFeature>().InlineEnum<CarFeature>().Format("{0}");
```

Enum localization strings can be supplied from files. (See <a href="#CarFeatures.ini">CarFeatures.ini</a>)

```csharp
IAssetSource assetSource = LineReaderMap.Default.FileAssetSource(@"ILine\CarFeature.ini");
LineRoot.Builder.AddSource(assetSource).Build();
```


A single enum case can be matched with <b>.Key(<i>case</i>)</b>. 

```csharp
Console.WriteLine(carFeature.Key(CarFeature.Petrol));
Console.WriteLine(carFeature.Key(CarFeature.Petrol).Culture("fi"));
Console.WriteLine(carFeature.Key(CarFeature.Petrol).Culture("sv"));
```

The result of the example above.
```none
Petrol
Bensiini
Bensin
```

If enum value contains multiple cases, it must be resolved with inside a formulated string.
Localization strings for the refered enum value are matched against keys <i>"Assembly:asm:Type:enumtype:Key:case"</i> from the *IAsset*.
Inlined strings only apply with *ILine* instance that contains the inlinings.

```csharp
CarFeature features = CarFeature.Petrol | CarFeature.FiveDoors | CarFeature.Black;
Console.WriteLine(carFeature.Value(features));
```

The result of the example above.
```none
Petrol, FiveDoors, Black
Bensiini, Viisiovinen, Musta
Bensin, Femdörras, Svart
```

<b>.InlineEnum(<i>enumCase, culture, text</i>)</b> inlines culture specific texts to the *ILine* reference.

```csharp
ILine carFeature = LineRoot.Global.Assembly("docs").Type<CarFeature>()
    .InlineEnum<CarFeature>()
    .InlineEnum(CarFeature.Electric, "fi", "Sähkö")
    .InlineEnum(CarFeature.Petrol, "fi", "Bensiini")
    .InlineEnum(CarFeature.NaturalGas, "fi", "Maakaasu")
    .InlineEnum(CarFeature.TwoDoors, "fi", "Kaksiovinen")
    .InlineEnum(CarFeature.FourDoors, "fi", "Neliovinen")
    .InlineEnum(CarFeature.FiveDoors, "fi", "Viisiovinen")
    .InlineEnum(CarFeature.Red, "fi", "Punainen")
    .InlineEnum(CarFeature.Black, "fi", "Musta")
    .InlineEnum(CarFeature.White, "fi", "Valkoinen")
    .Format("{0}");
```


If placeholder format is "{enum:|}" then the printed string uses "|" as separator. "{enum: |}" prints with " | ".

```csharp
Console.WriteLine(carFeature.Formulate($"{CarFeature.Petrol | CarFeature.Black:|}").Culture("fi"));
Console.WriteLine(carFeature.Formulate($"{CarFeature.Petrol | CarFeature.Black: |}").Culture("fi"));
```
```none
Bensiini|Musta
Bensiini | Musta
```

Inlines placed in an *ILine* instance are applicable in another *ILine* instances,
if the .Value() is supplied as *ILine* with inlinings and not as *Enum*.

```csharp
ILine carFeature = LineRoot.Global.Assembly("docs").Type<CarFeature>().InlineEnum<CarFeature>()
    .InlineEnum(CarFeature.Electric, "de", "Elektroauto")
    .InlineEnum(CarFeature.Petrol, "de", "Benzinwagen")
    .InlineEnum(CarFeature.NaturalGas, "de", "Erdgasauto")
    .InlineEnum(CarFeature.TwoDoors, "de", "Zweitürig")
    .InlineEnum(CarFeature.FourDoors, "de", "Viertürig")
    .InlineEnum(CarFeature.FiveDoors, "de", "Fünftürige")
    .InlineEnum(CarFeature.Red, "de", "Rot")
    .InlineEnum(CarFeature.Black, "de", "Schwartz")
    .InlineEnum(CarFeature.White, "de", "Weiß")
    .Format("{0}");

ILine message = LineRoot.Global.Assembly("docs").Type("MyClass").Key("Msg")
    .Format("Your car has following features: {0}")
    .de("Ihr Auto hat folgende Eigenschaften: {0}");

CarFeature features = CarFeature.Petrol | CarFeature.Red | CarFeature.TwoDoors;

// Inlined enum strings don't work as Enum (unless tool is used)
Console.WriteLine(message.Value(features).Culture("de"));
// But works when ILine reference is used.
Console.WriteLine(message.Value(carFeature.Value(features)).Culture("de"));
```

```none
Ihr Auto hat folgende Eigenschaften: Petrol, TwoDoors, Red
Ihr Auto hat folgende Eigenschaften: Benzinwagen, Zweitürig, Rot
```

However, if <i>Lexical.Localization.Tool</i> is used in the build process, then inlined 
strings are available as enums too. Tool picks up the inlined strings and places them into
localization file.

```none
Ihr Auto hat folgende Eigenschaften: Benzinwagen, Zweitürig, Rot
Ihr Auto hat folgende Eigenschaften: Benzinwagen, Zweitürig, Rot
```


<a id="CarFeatures.ini" />
Files that supply enumeration localization should use key in format of <i>"Assembly:asm:Type:enumtype:Key:case"</i>.
<details>
<summary><b>CarFeatures.ini</b> example file (<u>Click here</u>)</summary>

```ini
[Assembly:docs:Type:docs.CarFeature:Culture:sv]
// Fuel Type
Key:Electric = El
Key:Petrol = Bensin
Key:NaturalGas = Naturgas

// Door count
Key:TwoDoors = Tvådörras
Key:FourDoors = Fyrdörras
Key:FiveDoors = Femdörras

// Color
Key:Red = Röd
Key:Black = Svart
Key:White = Vit

[Culture:sv]
Assembly:docs:Type:MyClass:Key:Msg = Din bil har: {0}

[Assembly:docs:Type:docs.CarFeature:Culture:fi]
// Fuel Type
Key:Electric = Sähkö
Key:Petrol = Bensiini
Key:NaturalGas = Maakaasu

// Door count
Key:TwoDoors = Kaksiovinen
Key:FourDoors = Neliovinen
Key:FiveDoors = Viisiovinen

// Color
Key:Red = Punainen
Key:Black = Musta
Key:White = Valkoinen

[Culture:fi]
Assembly:docs:Type:MyClass:Key:Msg = Autosi ominaisuudet: {0}

```
</details> 

# Resources
<b>.ResolveBytes()</b> resolves the line to bytes in the current executing context. The result struct is <b>LineResourceBytes</b>.

```csharp
ILine line = LineRoot.Global.Key("Error").Resource(new byte[] { 1, 2, 3 });
LineResourceBytes result = line.ResolveBytes();
```

<b>.ResolveStream()</b> resolves the line to string in the current executing context. The result struct is <b>LineResourceStream</b>.

```csharp
ILine line = LineRoot.Global.Key("Error").Resource(new byte[] { 1, 2, 3 });
using (LineResourceStream result = line.ResolveStream())
{

}
```

# Use in classes
If class is designed to support dependency injection without string localizers, the constructors should 
take in argument *ILine&lt;T&gt;* and possibly *ILineRoot*. See more in [Best Practices](../BestPractices/ClassLibrary/index.md).
Constructor argument **ILine&lt;T&gt;** helps the Dependency Injection to assign the localization so that it is scoped in to correct typesection.

```csharp
class MyController
{
    ILine localization;

    public MyController(ILine<MyController> localization)
    {
        this.localization = localization;
    }

    public MyController(ILineRoot localizationRoot)
    {
        this.localization = localizationRoot.Assembly("docs").Type<MyController>();
    }

    public void Do()
    {
        string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
    }
}
```

If class is designed to use static instance and without dependency injection, localization reference can be acquired from **LineRoot**.

```csharp
class MyControllerB
{
    static ILine localization = LineRoot.Global.Assembly("docs").Type<MyControllerB>();

    public void Do()
    {
        string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
    }
}
```
