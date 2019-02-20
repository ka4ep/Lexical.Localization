# Xml File
Example of **.xml** localization file.
```
<?xml version="1.0" encoding="UTF-8"?>
<localization xmlns:culture="urn:lexical.fi:culture" xmlns:type="urn:lexical.fi:type" 
               xmlns:key="urn:lexical.fi:key">
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
      <key:Success>Onnistui</Success>
      <key:Error>Virhe (Koodi=0x{0:X8})</key:Error>
    </type:ConsoleApp1.MyController>
  </culture:fi>
</localization>
```
