using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lexical.Localization;
using Lexical.Localization.Internal;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class HashEqualTests : TestUtils
    {
        [TestMethod]
        public void Test1()
        {
            /// Root
            // Arrange
            IAssetKey root1 = LocalizationRoot.Global;
            IAssetKey root2 = StringLocalizerRoot.Global;
            IAssetKey root3 = LocalizationRoot.CreateDefault();

            // Assert
            AreEqual(root1, root2, root3);

            /// Section-1
            // Arrange
            IAssetKey section1 = root1.Type(typeof(HashEqualTests).FullName);
            IAssetKey section2 = root2.Type(typeof(HashEqualTests));
            IAssetKey section3 = root3.Type<HashEqualTests>();
            IAssetKey section4 = new LocalizationKey._Type(new LocalizationRoot(), typeof(HashEqualTests).FullName);
            IAssetKey sectionX = root1.Section("MySection");
            // Assert
            AreEqual(section1, section2, section3);
            AreEqual(section1, section4);
            AreNotEqual(section1, sectionX);
            AreNotEqual(section2, sectionX);
            AreNotEqual(section3, sectionX);
            AreNotEqual(section4, sectionX);
            AreNotEqual(section1, root1, sectionX);
            AreNotEqual(section2, root2, sectionX);
            AreNotEqual(section3, root3, sectionX);
            AreNotEqual(section4, root1, sectionX);

            /// Section-2
            // Arrange
            IAssetKey subSection1 = section1.Section("HashEqualTests");
            IAssetKey subSection2 = section2.Section(typeof(HashEqualTests).Name);
            IAssetKey subSection3 = section3.Section(nameof(HashEqualTests));
            IAssetKey subSectionX = section1.Section("MySubSection");
            // Assert
            AreEqual(subSection1, subSection2, subSection3);
            AreNotEqual(subSection1, subSectionX);
            AreNotEqual(subSection2, subSectionX);
            AreNotEqual(subSection3, subSectionX);
            AreNotEqual(subSection1, section1, subSectionX);
            AreNotEqual(subSection2, section2, subSectionX);
            AreNotEqual(subSection3, section3, subSectionX);

            /// Key
            // Arrange
            IAssetKey key1 = subSection1.Key("Key");
            IAssetKey key2 = subSection2.Key("Key");
            IAssetKey key3 = subSection3.Key("Key");
            IAssetKey keyX = subSection1.Key("X");
            // Assert
            AreEqual(key1, key2, key3);
            AreNotEqual(key1, keyX);
            AreNotEqual(key2, keyX);
            AreNotEqual(key3, keyX);

            /// Culture
            // Arrange
            IAssetKey ckey1 = root1.Section("MySection").Section("MySubsection").Key("Key").Culture("fi");
            IAssetKey ckey2 = root1.Section("MySection").Section("MySubsection").Culture("fi").Key("Key");
            IAssetKey ckey3 = root1.Section("MySection").Culture("fi").Section("MySubsection").Key("Key");
            IAssetKey ckey4 = root1.Culture("fi").Section("MySection").Section("MySubsection").Key("Key");
            IAssetKey ckeyX = root1.Section("MySection").Section("MySubsection").Key("Key");
            // Assert
            AreEqual(ckey1, ckey2, ckey3);
            AreNotEqual(ckey1, ckeyX);
            AreNotEqual(ckey2, ckeyX);
            AreNotEqual(ckey3, ckeyX);

            /// Format args
            // Arrange
            IAssetKey fkey1 = root1.Section("MySection").Section("MySubsection").Key("Key").Format(0xBad);
            IAssetKey fkey2 = root1.Section("MySection").Section("MySubsection").Format(0xBad).Key("Key");
            IAssetKey fkey3 = root1.Section("MySection").Format(0xBad).Section("MySubsection").Key("Key");
            IAssetKey fkey4 = root1.Format(0xBad).Section("MySection").Section("MySubsection").Key("Key");
            IAssetKey fkeyX = root1.Section("MySection").Section("MySubsection").Key("Key");
            // Assert
            AreEqual(fkey1, fkey2, fkey3);
            AreNotEqual(fkey1, fkeyX);
            AreNotEqual(fkey2, fkeyX);
            AreNotEqual(fkey3, fkeyX);

        }


    }
}
