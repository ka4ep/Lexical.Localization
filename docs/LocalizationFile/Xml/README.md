# Xml File
Example of **.xml** localization file.
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture" 
              xmlns:Type="urn:lexical.fi:Type" 
              xmlns:Key="urn:lexical.fi:Key">
  <Type:ConsoleApp1.MyController>
    <Key:Success>Success</Key:Success>
    <Key:Error>Error (Code=0x{0:X8})</Key:Error>
  </Type:ConsoleApp1.MyController>
  <Culture:en>
    <Type:ConsoleApp1.MyController>
      <Key:Success>Success</Key:Success>
      <Key:Error>Error (Code=0x{0:X8})</Key:Error>
    </Type:ConsoleApp1.MyController>
  </Culture:en>
  <Culture:fi>
    <Type:ConsoleApp1.MyController>
      <Key:Success>Onnistui</Key:Success>
      <Key:Error>Virhe (Koodi=0x{0:X8})</Key:Error>
    </Type:ConsoleApp1.MyController>
  </Culture:fi>
</Localization>
```

Keys can be defined with attributes.
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns:Culture="urn:lexical.fi:Culture">
  <Culture:sv Type="ConsoleApp1.MyController" Key="Success">Det Funkar</Culture:sv>
  <Culture:sv Type="ConsoleApp1.MyController" Key="Error">Det funkar inte (Kod=0x{0:X8})</Culture:sv>
</Localization>
```

And with "Line" node.
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Localization xmlns="urn:lexical.fi">
  <Line Culture="sv" Type="ConsoleApp1.MyController" Key="Success">Det Funkar</Line>
  <Line Culture="sv" Type="ConsoleApp1.MyController" Key="Error">Det funkar inte (Kod=0x{0:X8})</Line>
</Localization>
```


<!-- Write about parameter multiple times Section="aa" Section_1="aa" -->