# Tutorial

**Links**
* [Website](http://lexical.fi/Localization/index.html)
* [Github](https://github.com/tagcode/Lexical.Localization)
* [Nuget](https://www.nuget.org/packages/Lexical.Localization/)

Short example.

```csharp
ILine key = LineRoot.Global
    .Logger(Console.Out, LineStatusSeverity.Ok)
    .Key("hello")
    .Format("Hello, {0}.")
    .Inline("Culture:fi", "Hei, {0}")
    .Inline("Culture:de", "Hallo, {0}");

Console.WriteLine(key.Value("mr. anonymous"));
```

