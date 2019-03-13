using System;
using System.Collections.Generic;
using System.Linq;
using Lexical.Localization;
using Lexical.Localization.Utils;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            IAsset asset;
            asset = LocalizationReaderMap.Instance.FileAsset("localization.resources", AssetKeyNameProvider.Colon_Dot_Dot);
            asset = LocalizationReaderMap.Instance.FileAsset("localization.resx", AssetKeyNameProvider.Colon_Dot_Dot);
            asset = LocalizationReaderMap.Instance.FileAsset("localization.xml");
            asset = LocalizationReaderMap.Instance.FileAsset("localization.ini");
            asset = LocalizationReaderMap.Instance.FileAsset("localization.json");
            LocalizationRoot.Builder.AddAsset(asset).Build();

            IKeyTree tree = LocalizationReaderMap.Instance.ReadKeyTree("localization.json");
            Console.WriteLine(tree.ToString());
            Console.WriteLine(tree.Children.First().ToString());
            Console.WriteLine(tree.Children.Take(1).First().ToString());

            // Shorter
            //LocalizationRoot.Builder.AddLocalizationFile("localization.xml").Build();

            // Create key
            IAssetKey key = LocalizationRoot.Global.Type("ConsoleApp1.MyController").Key("Success");
            Key key2 = Key.Create("Type", "ConsoleApp1.MyController").Append("Key", "Success").Append("Culture", "en");

            AssetKeyComparer comparer = AssetKeyComparer.Default;

            Console.WriteLine(comparer.GetHashCode(key.Culture("en")));
            Console.WriteLine(comparer.GetHashCode(key2));

            Console.WriteLine(key);
            Console.WriteLine(key.Culture("en"));
            Console.WriteLine(key.Culture("fi"));
            Console.WriteLine(key.Culture("sv"));

            Console.ReadKey();
        }
    }
}
