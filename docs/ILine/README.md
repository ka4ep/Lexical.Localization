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

<b>.Logger(<i>ILocalizationLogger</i>)</b> adds logger as line part.

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
    new ResolveSource[] { ResolveSource.Inlines, ResolveSource.Asset, ResolveSource.Key };

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
    new ResolveSource[] { ResolveSource.Inlines, ResolveSource.Asset, ResolveSource.Key };

IResourceResolver resourceResolver = new ResourceResolver(Resolvers.Default, resolveSequence);
ILine line = LineRoot.Global.ResourceResolver(resourceResolver);
```

<b>.ResourceResolver(<i>string</i>)</b> adds assembly qualified class name to *IResourceResolver*.

```csharp
ILine line = LineRoot.Global.ResourceResolver("Lexical.Localization.Resource.ResourceResolver");
```

# Canonically compared keys
Key parts determine how *ILine*s are hash-equally compared.

Canonically compared key parts are compared so that the occurance position of the key parts are relevant.

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

# Non-canonically compared keys
Non-canonically compared key parts are compared so that the position doesn't matter. The first occurance of each type is considered effective, rest are ignored.

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

# Strings
<b>.String(<i>IString</i>)</b> appends preparsed default string value.

```csharp
IString str = CSharpFormat.Default.Parse("ErrorCode={0}");
ILine line = LineRoot.Global.Key("Error").String(str);
```

<b>.String(<i>string</i>)</b> appends "String" hint. This needs preceding "StringFormat" part in order to determine the way to parse.
If "StringFormat" is not provided, then C# format is used as default.

```csharp
ILine line = LineRoot.Global.Key("Error").StringFormat(CSharpFormat.Default).String("ErrorCode={0}");
```

<b>.Format(<i>string</i>)</b> appends C# String formulation value.

```csharp
ILine line = LineRoot.Global.Key("Error").Format("ErrorCode={0}");
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
    static ILine localization = LineRoot.Global.Type<MyControllerB>();
    static ILine Success = localization.Key("Success").Format("Success").sv("Det funkar").fi("Onnistui");

    public string Do()
    {
        return Success.ToString();
    }
}
```

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
        this.localization = localizationRoot.Type<MyController>();
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
    static ILine localization = LineRoot.Global.Type<MyControllerB>();

    public void Do()
    {
        string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
    }
}
```
