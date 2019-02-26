using System;
using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Internal;
using Lexical.Localization.LocalizationFile2;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlFileAsset asset = new XmlFileAsset("localization.xml");
            //JsonFileAsset asset = new JsonFileAsset("localization.json");
            //IniFileAsset asset = new IniFileAsset("localization.ini");
            //ResXFileAsset asset = new ResXFileAsset("localization.resx", "{culture.}[type].[key]");
            //ResourcesFileAsset asset = new ResourcesFileAsset("localization.resources", "{culture.}[type].[key]");
            asset.Load();
            LocalizationRoot.Builder.AddAsset(asset).Build();
            IAssetKey key = LocalizationRoot.Global.TypeSection("ConsoleApp1.MyController").Key("Success");
            Key key2 = Key.Create("type", "ConsoleApp1.MyController").Append("key", "Success");

            AssetKeyComparer comparer = AssetKeyComparer.Default;

            Console.WriteLine(comparer.GetHashCode(key));
            Console.WriteLine(comparer.GetHashCode(key2));

            Key key3 = key2.Append("culture", "en");
            Console.WriteLine(key3);

            Console.WriteLine(key);
            Console.WriteLine(key.SetCulture("en"));
            Console.WriteLine(key.SetCulture("fi"));
            Console.WriteLine(key.SetCulture("sv"));

            Console.ReadKey();
        }
    }
}
