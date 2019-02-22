using System;
using System.Collections.Generic;
using Lexical.Localization;
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

            // TODO: Root shouldn't be hash-equal compared (??) //

            IAssetKeyParametrizer compositeParametrizer = Key.Parametrizer.Default.Concat(AssetKeyParametrizer.Singleton);
            AssetKeyComparer comparer1 = new AssetKeyComparer().AddCanonicalParametrizerComparer(AssetKeyParametrizer.Singleton).AddNonCanonicalParametrizerComparer(AssetKeyParametrizer.Singleton);
            AssetKeyComparer comparer2 = new AssetKeyComparer().AddCanonicalParametrizerComparer(Key.Parametrizer.Default).AddNonCanonicalParametrizerComparer(Key.Parametrizer.Default);

            Console.WriteLine(comparer1.GetHashCode(key));
            Console.WriteLine(comparer2.GetHashCode(key2));

            Console.WriteLine(key);
            Console.WriteLine(key.SetCulture("en"));
            Console.WriteLine(key.SetCulture("fi"));
            Console.WriteLine(key.SetCulture("se"));

            Console.ReadKey();
        }
    }
}
