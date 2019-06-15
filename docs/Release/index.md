# Releases
## 0.28.1
Jun 15 2019
* LineBase implements IFormattable. Can be used with String.Format($"{line}").
* StringResolver forwards applicable culture to IFormattable argument.
* StringResolver forwards applicable culture to ILine argument.
* Better culture resolving LineStatus messages.
* Improvements to string resolver algorithm regarding sub-queries.

## 0.28.0
Jun 10 2019
* Fixed issues with resolvers, LineFormat
* LineStatus.ResourceErrorDB*
* IString LineStatuses other than .StringFormat passed forward by StringResolver.
* Ms.Extensions.ILoggerFactory
* NLog
* Enumeration
* String interpolation
* Lexical.Localization.ILocalizationLogger -> Lexical.Localization.Common.ILogger
* Lexical.Localization.ILineArguments -> Lexical.Localization.ILineArgument
* Lexical.Localization.ILineParameterQualifier -> Lexical.Localization.ILineArgumentQualifier

## 0.27.0
Jun 7 2019
* Renamed IAssetKey to ILine
* Complete overhaul of everything

## 0.26.0
Apr 4 2019
* StringAsset 
 * LocalizationStringAsset is unified to StringAsset.
 * error handler
 * Loads only added or modified files.
* LocalizationFileSource to inherit FileSource for binary files. Same for embedded and file provider sources.

## 0.25.0
Apr 2 2019
* Localization___Source classes.
* Inlining is now lighter when only default string is inlined. Only one heap allocation.
* API-Changes
 * Changed interface of ILineInlines to ILineInlines. It inherits IDictionary<ILine, string>
 * Root classes no longer implement ILineParameterAssigned as "Root" parameter.

## 0.24.0 
Apr 1 2019
* LineComparer
 * Cached hashcodes
 * Unwrapped recursion
 * Immutability
* IniLinesWriter, JsonLinesReader, LocalizationXmlReader
 * Fixed escaping with control characters, white-space, braces and backslashes
* API-changes
 * LineComparer renamed .AddNonCanonicalComparer() to generic .AddComparer()
 * LineFormat, changed constructor signature.
 * LineReaderMap is now read-only, but it can be cloned.

# Upcoming Releases

## 0.29.0
* Rewriting IAssetBuilder
 * Addresses how class libraries can request for resources that can be supplied by the deploying application from file sources that may not be inheritly supported by the class library. 
 * For example, application can add resources from IFileProvider, and class library doesn't need to have a dependency to FileProvider.
 * IAssetLoader back. File patterns can be given to resources, for example "Resources/[Assembly]{.culture}.dll".

## 0.30.0
 Jun 2019
 * Support for .po files
  * Translates .po file plural rules

## 0.31.0
Jun 2019
* Better support for external functions
 * Better placeholder expressions
* StringFormat implementation that uses Humanizr functions, "There are {#0.Pluralize()} cats.".



