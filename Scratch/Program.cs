using System;
using System.Collections.Generic;
using Lexical.Localization;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };

            IAsset asset = new LocalizationAsset().AddStringSource(source, "{culture:}{type:}[key]").Load();

            LocalizationRoot.Builder.AddAsset(asset).Build();
            IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("hello");

            Console.WriteLine(key);
            Console.WriteLine(key.SetCulture("en"));
            Console.WriteLine(key.SetCulture("de"));

            Console.ReadKey();
        }
    }
}
