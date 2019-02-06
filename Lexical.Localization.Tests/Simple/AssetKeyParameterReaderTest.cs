using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Lexical.Localization;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class AssetKeyAttributeReader_
    {
        [TestMethod]
        public void Test1()
        {
            var reader = AssetKeyParametrizer.Singleton;
            ILocalizationKey key = LocalizationRoot.Global.TypeSection<AssetKeyAttributeReader_>().Section("Icons").Key("MyKey");

            string id = AssetKeyNameProvider.Default.BuildName(key);

            foreach (var part in reader.Break(key))
            {
                string str = $"part={part}, parameters={string.Join(",", reader.GetPartParameters(part))}, values={string.Join(",", reader.Break(key))}";
                Console.WriteLine(str);
            }
            ILocalizationKey key2 = reader.CreatePart(key, "section", "newSection") as ILocalizationKey;


        }
    }
}
