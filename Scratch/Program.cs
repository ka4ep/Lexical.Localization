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
            asset = XmlFileFormat.Instance.CreateFileAsset("localization.xml");
            asset = JsonFileFormat.Instance.CreateFileAsset("localization.json");
            asset = IniFileFormat.Instance.CreateFileAsset("localization.ini");
            asset = ResourcesFileFormat.Instance.CreateFileAsset("localization.resources", AssetKeyNameProvider.Colon_Dot_Dot);
            asset = ResXFileFormat.Instance.CreateFileAsset("localization.resx", AssetKeyNameProvider.Colon_Dot_Dot);
            LocalizationRoot.Builder.AddAsset(asset).Build();

            // Shorter
            //LocalizationRoot.Builder.AddLocalizationFile("localization.xml").Build();

            // Create key
            IAssetKey key = LocalizationRoot.Global.TypeSection("ConsoleApp1.MyController").Key("Success");
            Key key2 = Key.Create("type", "ConsoleApp1.MyController").Append("key", "Success").Append("culture", "en");

            AssetKeyComparer comparer = AssetKeyComparer.Default;

            Console.WriteLine(comparer.GetHashCode(key.SetCulture("en")));
            Console.WriteLine(comparer.GetHashCode(key2));

            Console.WriteLine(key);
            Console.WriteLine(key.SetCulture("en"));
            Console.WriteLine(key.SetCulture("fi"));
            Console.WriteLine(key.SetCulture("sv"));

            Console.ReadKey();
        }
    }
}
