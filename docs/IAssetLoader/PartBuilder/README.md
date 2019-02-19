# Introduction
Part builder is a tool that helps constructing parts. 
It provides a uniform way to configurate and instantiate part classes.

```csharp
// Create asset loader
IAssetLoader assetLoader = new AssetLoader();

// Create part(s)
IAssetLoaderPart[] parts = new AssetLoaderPartBuilder()
    .Path(".")                                                  // Add directory to search files from
    .FilePattern("Assets/localization{-culture}.ini")           // Add file name pattern
    .KeyPolicy(default)                                         // Add key policy default to file extension
    .Strings()                                                  // Signal to read binary resources
    .Build().ToArray();                                         // Build IAssetLoaderParts

// Add part(s)
assetLoader.AddRange(parts);
```

An alternative way is to construct part builder as "one-liner" to IAssetLoader.
**.NewPart()** starts a new part builder and **.End()** closes it.

```csharp
// Create asset loader and add parts
IAssetLoader assetLoader =
    new AssetLoader()
    .NewPart()                                                  // Start part builder
        .Path(".")                                              // Add location to search files from
        .FilePattern("Assets/localization{-culture}.ini")       // File name pattern
        .KeyPattern(default)                                    // Key pattern to match to lines within files
        .Strings()                                              // Signal to read strings
    .End();                                                     // End part builder
```

# Source Type
Part builder configuration starts by choosing sources to load assets from. There are three ways.

**1.** Assets can be loaded from local files. The root directories are added with **.Path(*string*)**. 
Path can be added multiple times, this creates a new part for each.

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Path("Assets1")                                            // Add directory to search files from
    .Path("Assets2")                                            // Add another directory to search files from.
```

**2.** Embedded resources can be loaded with **.Assembly(*Assembly*)**.

```csharp
// Get Assembly reference
Assembly asm = Assembly.GetExecutingAssembly();
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Assembly(asm)                                              // Add assembly to embedded files from.
```

**3.** Files can be loaded with abstraction of [IFileProvider](https://github.com/aspnet/Extensions/blob/master/src/FileProviders/Abstractions/src/IFileProvider.cs).
File provider source is added with **.FileProvider(*IFileProvider*)**. 
Note that, this extension method is available by importing namespace Lexical.Localization.Ms.Extensions and **Lexical.Localization.Ms.Extensions**.
The caller's assembly must also import nuget library [Microsoft.Extensions.FileProviders.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Abstractions/).

```csharp
// File provider
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .FileProvider(fileProvider)                                 // Add file provider to search files from.
```

# File Pattern
Asset loader reads files based on parameters of the requesting key. 
File pattern is a way to describe filename(s) by adding "{parameters}" into the file name string.

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Path(".")                                                  // Add directory to search files from
    .FilePattern("Patches/{section}{-key}{-culture}.png")       // Add file name pattern
    .FilePattern("Assets/{section}{-key}{-culture}.png")        // Add another file name pattern
```

These parameters are later on provided by the caller in the request keys.

```csharp
// Create key. Both folders are searched for file "icons-ok-de.png".
IAssetKey key = new LocalizationRoot().Section("icons").Key("ok").SetCulture("de");
```

## Parameters
Parameters are written in format of "{prefix **parametername** suffix}". 
Some of the parameters are well-known, and they also have a respective method that parametrizes a key.

Table of well-known parameter names

| Parameter | Key Method  | Description |
|----------|:--------|:------------|
| assembly | .AssemblySection(*string*) | Assembly name |
| location | .Location(*string*) | Subdirectory in local files |
| resource | .Resource(*string*) | Subdirectory in embedded resources |
| type | .TypeSection(*string*) | Class name |
| section | .Section(*string*) | Generic section, used for grouping |
| anysection | *all above* | Matches to any section above. |
| culture  | .SetCulture(*string*) | Culture |
| key | .Key(*string*) | Key name |

## Optional Parameter
Parameter is optional when it is written inside braces "{parameter/}".

```csharp
.FilePattern("{location/}{section}{-key}{-culture}.png")    // Add file name pattern
```

With optional parameter, whether parameter is provided or not, it affects the filename that is attempted to be loaded. 
**.Location(*string*)** adds "location" parameter into the key, which affects the requested filename.

```csharp
// "location" is not used, matches to filename "icons-ok-de.png"
IAssetKey key_1 = new LocalizationRoot().Section("icons").Key("ok").SetCulture("de");
// This key matches to filename "Assets/icons-ok-de.png"
IAssetKey key_2 = new LocalizationRoot().Location("Assets").Section("icons").Key("ok").SetCulture("de");
// This key matches to filename "Patches/icons-ok-de.png"
IAssetKey key_3 = new LocalizationRoot().Location("Patches").Section("icons").Key("ok").SetCulture("de");
```

## Required Parameter
Parameter written inside brackets makes it a required parameter "[parameter/]". 

```csharp
.FilePattern("[location/]{section}{-key}{-culture}.png")    // Add file name pattern
```

Key is added with "location" parameter with the call **.Location(*string*)**.

```csharp
// Location is required but not provided. Will not work.
IAssetKey key_1 = new LocalizationRoot().Section("icons").Key("ok").SetCulture("de");
// Location is required and provided. This matches to file name "Patches/icons-ok-de.png"
IAssetKey key_2 = new LocalizationRoot().Location("Patches").Section("icons").Key("ok").SetCulture("de");
```

## Recurring Parameters
Parameter can be added multiple times by adding suffix "_#", replace # with the occurance index. "_n" represents the last occurance.

```csharp
.FilePattern("{location_0/}{location_1/}{location_n/}{section}{-key}{-culture}.png")  // Add file name pattern
```
To this asset **.Location(*string*)** can be inserted 0-3 times.

```csharp
// Location can be entered multiple times. 
// This matches to file name "Patches/20181130/icons-ok-de.png"
IAssetKey key = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").SetCulture("de");
```

## Match Parameters
To make a part to search for existing files, add **.MatchParameter(*string*)**. 
This will make the part to try to match missing parameters against detected file names.

```csharp
// Create builder
IAssetLoaderPart[] parts = new AssetLoaderPartBuilder()
    .Path(".")                                                  // Add directory to search files from
    .FilePattern("{location/}{section}{-key}{-culture}.png")    // Add file name pattern
    .MatchParameter("location")                                 // Match "location" against existing file names
    .Resource()
    .Build().ToArray();

// Create asset
IAsset asset = new AssetLoader(parts);

// Location is not provided, parameter is matched automatically. 
// Searches for "*/icons-ok-de.png".
IAssetKey key_1 = new LocalizationRoot(asset).Section("icons").Key("ok").SetCulture("de");

// Location is provided, files are not searched.
// This matches to file name "Patches/icons-ok-de.png"
IAssetKey key_2 = new LocalizationRoot(asset).Location("Patches").Section("icons").Key("ok").SetCulture("de");
```

## Regular Expressions
Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}".
This will give more control on how the pattern is matched when using with **.MatchParameter(*string*)**.

```csharp
// Searches for "*/icons-ok-de.png", but only one level deep because of "[^/]+" regular expression.
.FilePattern("{location<[^/]+>/}{section}{-key}{-culture}.png")
```

# Embedded Resource
Source type for embedded resources is set with **.Assembly(*Assembly*)**, and then 
resources selected with **.EmbeddedResource(*string*)** .

```csharp
.Assembly(asm)                                                  // Add assembly to embedded files from.
.EmbeddedPattern("namespace.Assets.icon{-key}{-culture}.png")   // Add embedded resource pattern
```
Key doesn't need "assembly" parameter if the assembly's namespace was hardcoded into the embedded pattern.

```csharp
// AssemblySection is not needed. Searches for "namespace.Assets.icon-ok-de.png"
IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
```

<br/>
Mandatory parameter is written inside brackets.

```csharp
.Assemblies(assemblies)                                         // Add assembly to embedded files from.
.EmbeddedPattern("[assembly.]Assets.icon{-key}{-culture}.png")  // Add embedded resource pattern
```
Required parameter "[assembly]" must be provided with the key by calling **.AssemblySection(*string*)**.

```csharp
// Key needs AssemblySection part to match. Searches for "namespace.Assets.icon-ok-de.png"
IAssetKey key = new LocalizationRoot().AssemblySection("namespace").Key("ok").SetCulture("de");
```

<br/>
Parameter "resource" is used for describing a resource path within embedded resources. **.ResourceSection()** adds that path to a key.

If parameter is configured to match with **.MatchParameters()**, then providing parameters is optional. 

```csharp
.EmbeddedPattern("[assembly.]{resource.}icon{-key}{-culture}.png")  // Add embedded resource pattern
.MatchParameters("assembly", "resource")                            // Match parameters against existing file names
```
Parameters "assembly" and "resource" are not provided and are matched to existing resource names.

```csharp
// Matching is optional. Searches for "*.*.icon-ok-de.png"
IAssetKey key_1 = new LocalizationRoot().Key("ok").SetCulture("de");
// Matching to "namespace.Resources.icon-ok-de.png"
IAssetKey key_2 = new LocalizationRoot().AssemblySection("namespace").ResourceSection("Resources.").Key("ok").SetCulture("de");
```

# Binary Resources
To configure the part to retrieve binary resources
add **.Resource()** to signal the builder to make it build part(s) that implement **IAssetResourceProvider**.
Resource parts provide binary files, such as icons, graphics, audio.

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Path(".")                                                  // Add directory to search files from
    .FilePattern("Assets/{section}{-key}{-culture}.png")        // Add file name pattern
    .FilePattern("Patches/{section}{-key}{-culture}.png")       // Add another file name pattern
    .Resource();                                                // Signal to read binary resources
```

# Language Strings
To read language strings from text based files, a key name policy must be added.

For example, let's assume there is **localization.ini** file with the following language strings.
```none
[en]
controller:hello = Hello World!

[de]
controller:hello = Hallo Welt!
```

Key policy is a description of how to convert IAssetKey into a localization file key.
There are three ways to add a key policy.

**1.** Assign policy with **.KeyPolicy(default)**.
Default policy finds one that is default for the given file extension type. 
For example, the default policy for .ini is to append culture and then then other parts canonically with ":" as separator.

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Path(".")                                                  // Add directory to search files from
    .FilePattern("Assets/localization.ini")                     // Add file name pattern
    .KeyPolicy(default)                                         // Add key policy default to file extension
    .Strings();                                                 // Signal to reads strings 

// Create asset
IAsset asset = new AssetLoader(builder.Build());

// Create key
IAssetKey key = new LocalizationRoot(asset).Section("controller").Key("hello").SetCulture("de");

// Issue request. Asset loader loads "Assets\localization.ini", and then searches for key "de:controller:hello".
string str = key.ToString();
```

**2.** Assign key policy with **.KeyPolicy(*IAssetKeyNameProvider*)** or **.KeyPolicy(*IAssetKeyNamePolicy*)**.
See [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetKeyNameProvider.cs) for a few default implementations.

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Path(".")                                                   // Add directory to search files from
    .FilePattern("Assets/localization.ini")                      // Add file name pattern
    .KeyPolicy(AssetKeyNameProvider.Colon_Colon_Colon)           // Add key policy with ":" separator
    .Strings();                                                  // Signal to reads strings 
```

**3.** Assign key as name pattern **.KeyPattern(*string*)**.

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Path(".")                                                   // Add directory to search files from
    .FilePattern("Assets/localization.ini")                      // Add file name pattern
    .KeyPattern("{culture:}{anysection_0:}{anysection_1:}[key]") // Add key pattern
    .Strings();                                                  // Signal to build part(s) that reads strings 
```


Finally the part builder is concluded with **.Strings()** to make it implement **ILocalizationStringProvider**.

## File Formats
File formats .ini, .json, .xml, .resx, and .resources are supported by default.
Other formats can implemented by two ways:

<br/>
**1.** Implement ILocalizationFileReader and add its constructor to Lexical.Localization.LocalizationTextReaderBuilder.BinaryReaderConstructors.

<br/>
**2.** Implement delegate Lexical.Localization.AssetFileConstructor that constructs an IAsset, and then provide that delegate to part builder with **.AssetFileConstructor(*delegate*)**

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Path(".")                                                  // Add directory to search files from
    .FilePattern("Assets/localization{-culture}.ext")           // Add file name pattern
    .AssetFileConstructor( (s, p) => new LocalizationStringDictionary(new Dictionary<string, string>()) )
    .KeyPolicy(default)                                         // Add key policy default to file extension
    .Strings();                                                 // Signal to read strings
```

# ResourceManager
Although .resx and .resources fileformats can be read as string files, an alternative way is to construct asset so
that it uses ResourceManagerAsset which forwards calls to System.Resources.ResourceManager instance.
ResourceManagerAsset can read both strings and resources. 

Construct with **.ResourceManager()**. 

```csharp
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .Assembly(asm)                                                  // Add assembly
    .EmbeddedPattern("namespace.Resources.MyController.resources")  // Add embedded resource pattern
    .KeyPolicy(default)                                             // Add key policy default to file extension
    .ResourceManager();                                             // Signal to reads strings 
```

Note that, MSBuild spreads embedded .resx files into satellite assemblies under culture specific folders.
![ResXes](resx.png)

These assemblies can be read in with IFileProvider, when used the combination of [RootFileProvider](~/sdk/FileProvider/docs/Root/index.html), [PackageFileProvider](~/sdk/FileProvider/docs/Package/index.html) 
and [DllFileProvider](~/sdk/FileProvider/docs/Dll/index.html).
To do so construct asset builder with **.FileProvider()**. 

```csharp
// Create Root File Provider
IFileProvider root = new RootFileProvider();
// Create Package File Provider
IFileProvider fileProvider = new PackageFileProvider(root)
    .ConfigureOptions(o => o.AddPackageLoader(Dll.Singleton))
    .AddDisposable(root);
// Create builder
AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder()
    .FileProvider(fileProvider)                                 // Add file provider that opens .dll files
    .FilePattern("{filename}.dll/{assembly}.Resources.{type.}{culture.}resources")  // Add file name pattern
    .MatchParameters("filename", "assembly", "culture", "type") // Match parameters against existing file names
    .KeyPattern("{assembly:}{type:}{key}")                       // Add key policy
    .Strings();                                                 // Signal to read strings
```


# Links
* [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetLoader/PartBuilder/Examples.cs)
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetLoaderPart](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoaderPart.cs)
 * [IAssetLoaderPartOptions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoaderPartOptions.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetLoaderPartBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetLoader/AssetLoaderPartBuilder.cs)
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [AssetLoaderPartBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartBuilder.cs)
