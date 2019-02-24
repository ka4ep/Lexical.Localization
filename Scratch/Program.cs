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
            XmlAsset asset = new XmlAsset("localization.xml", false);

            LocalizationRoot.Builder.AddAsset(asset).Build();
            IAssetKey key = LocalizationRoot.Global.TypeSection("ConsoleApp1.MyController").Key("Success");
            Key key2 = new Key("root", "").Append("type", "ConsoleApp1.MyController").Append("key", "Success");

            AssetKeyComparer comparer = AssetKeyComparer.Default;

            Console.WriteLine(comparer.GetHashCode(key));
            Console.WriteLine(comparer.GetHashCode(key2));

            Key key3 = key2.Append("culture", "en");
            Console.WriteLine(key3);

            Console.WriteLine(key);
            Console.WriteLine(key.SetCulture("en"));
            Console.WriteLine(key.SetCulture("fi"));
            Console.WriteLine(key.SetCulture("se"));

            Console.ReadKey();
        }
    }
}
