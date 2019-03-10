using System;
using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Utils;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            IAsset asset;
            asset = JsonLocalizationReader.Instance.FileAsset("localization.json");
            asset = ResourcesLocalizationReader.Instance.FileAsset("localization.resources", AssetKeyNameProvider.Colon_Dot_Dot);
            asset = ResXLocalizationReader.Instance.FileAsset("localization.resx", AssetKeyNameProvider.Colon_Dot_Dot);
            asset = XmlLocalizationReader.Instance.FileAsset("localization.xml");
            asset = IniLocalizationReader.Instance.FileAsset("localization.ini");
            LocalizationRoot.Builder.AddAsset(asset).Build();

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
