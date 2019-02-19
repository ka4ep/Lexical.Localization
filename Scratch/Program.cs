using System;
using System.Collections.Generic;
using Lexical.Localization;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            List<KeyValuePair<string, string>> parrr = new List<KeyValuePair<string, string>>();
            parrr.Add(new KeyValuePair<string, string> ( "culture", "en" ));
            parrr.Add(new KeyValuePair<string, string> ( "type", "MyLibrary:Type" ));
            parrr.Add(new KeyValuePair<string, string> ( "key", "\"hello\"" ));

            string str2 = AssetKeyStringSerializer.Xml.PrintParameters(parrr);
            Console.WriteLine(str2);

            var pars = AssetKeyStringSerializer.Xml.ParseParameters(str2);

            Console.ReadKey();
        }
    }
}
