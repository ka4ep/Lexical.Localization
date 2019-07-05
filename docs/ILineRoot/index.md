# Localization Root
**ILineRoot** is an interface for root implementations. 
Root is the place where asset (the localization provider) is tied to key (localization consumer).

**LineRoot** is the default implementation. It's constructed with an asset and a culture policy.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

Further keys are constructed from root. 
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

Now that key is associated with an asset and a culture provider, it can provide strings and resources.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]

Note, that root is not mandatory, keys can be constructed with *null* as previous key.
These keys cannot be used as providers, only as references.
[!code-csharp[Snippet](Examples.cs#Snippet_5x)]

# String Localizer
**StringLocalizerRoot** is an alternative root implementation.
Every key, that is constructed from this class, implements localization interfaces IStringLocalizer and IStringLocalizerFactory.

StringLocalizerRoot is constructed with an asset and a culture policy, just as LineRoot.
[!code-csharp[Snippet](Examples_StringLocalizer.cs#Snippet_1)]
<br/>

Keys can be type-casted to **IStringLocalizer**.
[!code-csharp[Snippet](Examples_StringLocalizer.cs#Snippet_2)]
And to **IStringLocalizerFactory**.
[!code-csharp[Snippet](Examples_StringLocalizer.cs#Snippet_3)]

<br/>
Key can be assigned for a type **.Type(*Type*)** and then casted to **IStringLocalizer&lt;*Type*&gt;**.
[!code-csharp[Snippet](Examples_StringLocalizer.cs#Snippet_4a)]
Also after type casting to IStringLocalizerFactory with **.Create(*Type*)**.
[!code-csharp[Snippet](Examples_StringLocalizer.cs#Snippet_4b)]

<br/>
Culture can be locked in with **.Culture(*string*)**.
[!code-csharp[Snippet](Examples_StringLocalizer.cs#Snippet_5a)]
And also after type casting to IStringLocalizer with **.WithCulture(*CultureInfo*)**.
[!code-csharp[Snippet](Examples_StringLocalizer.cs#Snippet_5b)]

# Global static root
Localized strings can be considered as constants because they are used the same way as regular strings. 

Lexical.Localization introduces a global static root **LineRoot.Global**.
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]

Assets are added to the global root with **LineRoot.Builder**.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]

If assets are initialized in concurrent environment then please lock with **LineRoot.Builder**.
[!code-csharp[Snippet](Examples.cs#Snippet_2c)]

**StringLocalizerRoot** is the same root as *LineRoot*, but has extra feature of implementing IStringLocalizer and IStringLocalizerFactory.
The calling assembly, however, needs to import NuGet **Microsoft.Extensions.Localization.Abstractions**.
[!code-csharp[Snippet](Examples.cs#Snippet_2d)]

They share the same assets, and the root instances are interchangeable. Assets can be added to either root.
[!code-csharp[Snippet](Examples.cs#Snippet_2e)]

**LineRoot.GlobalDynamic** returns dynamic instance for the static root.
[!code-csharp[Snippet](Examples.cs#Snippet_2f)]

# Links
* [ILineRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Line/Parts/ILineRoot.cs)
* [LineRoot](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Line/Parts/LineRoot.cs)
* [IStringLocalizer](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizer.cs) 
* [IStringLocalizer&lt;T&gt;](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerOfT.cs)
* [IStringLocalizerFactory](https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/IStringLocalizerFactory.cs)

