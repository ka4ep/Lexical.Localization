# Localization Asset
**StringAsset** is language string container. Asset is populated from different IEnumeration sources, which become effective when **Load()** is called.
<br/>

The default way is to add a reader <i>IEnumerable&lt;ILine&gt;</i>.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]
<br/>
<br/>

Some source have keys as *strings* that cannot be parsed into a structured key. 
These assets can be added as <b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;, ILineFormat</i>)</b>.

An instance of **ILineFormatPrinter** must be provided to convert requesting keys to strings that can used to match the key in the source asset.
These values use **CSharpFormat** string format.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]
<br/>
<br/>

*StringAsset* can import key-string pairs as well. 
<b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;ILine, string&gt;&gt;</i>, ILineFormat keyPolicy)</b> adds language string source with ILine based keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]
<br/>
<br/>

Keys can be enumerated with **GetAllKeys()**. 
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]

