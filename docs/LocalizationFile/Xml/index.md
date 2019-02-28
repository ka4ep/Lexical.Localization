# Xml File
Example of **.xml** localization file.
```
<?xml version="1.0" encoding="UTF-8"?>
<localization xmlns:culture="urn:lexical.fi:localization:culture" 
              xmlns:type="urn:lexical.fi:localization:type" 
              xmlns:key="urn:lexical.fi:localization:key">
  <type:ConsoleApp1.MyController>
    <key:Success>Success</key:Success>
    <key:Error>Error (Code=0x{0:X8})</key:Error>
  </type:ConsoleApp1.MyController>
  <culture:en>
    <type:ConsoleApp1.MyController>
      <key:Success>Success</key:Success>
      <key:Error>Error (Code=0x{0:X8})</key:Error>
    </type:ConsoleApp1.MyController>
  </culture:en>
  <culture:fi>
    <type:ConsoleApp1.MyController>
      <key:Success>Onnistui</key:Success>
      <key:Error>Virhe (Koodi=0x{0:X8})</key:Error>
    </type:ConsoleApp1.MyController>
  </culture:fi>
</localization>
```

Keys can be defied with attributes as well.
```
<?xml version="1.0" encoding="UTF-8"?>
<localization xmlns:culture="urn:lexical.fi:culture">
  <culture:sv type="ConsoleApp1.MyController" key="Success">Det Funkar</culture:sv>
  <culture:sv type="ConsoleApp1.MyController" key="Error">Det funkar inte (Kod=0x{0:X8})</culture:sv>
</localization>
```
