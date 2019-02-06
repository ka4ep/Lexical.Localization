using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lexical.Localization.Ms.Extensions;
using ConsoleApp1;
using Lexical.Localization;
using Lexical.Localization.Internal;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Test1()
        {
            /// Root
            // Arrange
            IAssetKey root1 = LocalizationRoot.Global;
            //ILocalizationKey root2 = StringLocalizerGlobal.Instance;
            IAssetKey root2 = new StringLocalizerRoot.Mutable();
            IAssetKey root3 = LocalizationRoot.CreateDefault();

            // Assert
            SerializeEqual(root1, root2, root3);

            /// Section-1
            // Arrange
            IAssetKey section1 = root1.Section(typeof(MyController).CanonicalName());
            // BinaryFormatter cannot serialize System.RuntimeType
            //ILocalizationKey section2 = root2.Section(typeof(MyController));
            //ILocalizationKey section3 = root3.Section<MyController>();
            IAssetKey section2 = section1;
            IAssetKey section3 = section1;
            IAssetKey section4 = new LocalizationKey._Section(new LocalizationRoot(), typeof(MyController).CanonicalName());
            IAssetKey sectionX = root1.Section("MySection");
            // Assert
            SerializeEqual(section1, section2, section3);
            SerializeEqual(section1, section4);
            SerializeNotEqual(section1, sectionX);
            SerializeNotEqual(section2, sectionX);
            SerializeNotEqual(section3, sectionX);
            SerializeNotEqual(section4, sectionX);
            SerializeNotEqual(section1, root1, sectionX);
            SerializeNotEqual(section2, root2, sectionX);
            SerializeNotEqual(section3, root3, sectionX);
            SerializeNotEqual(section4, root1, sectionX);

            /// Section-2
            // Arrange
            IAssetKey subSection1 = section1.Section(typeof(MyController).CanonicalName());
            // BinaryFormatter cannot serialize System.RuntimeType
            //ILocalizationKey subSection2 = section2.Section(typeof(MyController));
            //ILocalizationKey subSection3 = section3.Section<MyController>();
            IAssetKey subSection2 = subSection1;
            IAssetKey subSection3 = subSection1;
            IAssetKey subSectionX = section1.Section("MySubSection");
            // Assert
            SerializeEqual(subSection1, subSection2, subSection3);
            SerializeNotEqual(subSection1, subSectionX);
            SerializeNotEqual(subSection2, subSectionX);
            SerializeNotEqual(subSection3, subSectionX);
            SerializeNotEqual(subSection1, section1, subSectionX);
            SerializeNotEqual(subSection2, section2, subSectionX);
            SerializeNotEqual(subSection3, section3, subSectionX);

            /// Key
            // Arrange
            IAssetKey key1 = subSection1.Key("Key");
            IAssetKey key2 = subSection2.Key("Key");
            IAssetKey key3 = subSection3.Key("Key");
            IAssetKey keyX = subSection1.Key("X");
            // Assert
            SerializeEqual(key1, key2, key3);
            SerializeNotEqual(key1, keyX);
            SerializeNotEqual(key2, keyX);
            SerializeNotEqual(key3, keyX);

            /// Culture
            // Does not work "CultureInfo is not marked as serializable"
            /*
            // Arrange
            ILocalizationKey ckey1 = root1.Section("MySection").Section("MySubsection")["Key"].SetCulture("fi");
            ILocalizationKey ckey2 = root1.Section("MySection").Section("MySubsection").SetCulture("fi")["Key"];
            ILocalizationKey ckey3 = root1.Section("MySection").SetCulture("fi").Section("MySubsection")["Key"];
            ILocalizationKey ckey4 = root1.SetCulture("fi").Section("MySection").Section("MySubsection")["Key"];
            ILocalizationKey ckeyX = root1.Section("MySection").Section("MySubsection")["Key"];
            // Assert
            SerializeEqual(ckey1, ckey2, ckey3);
            SerializeNotEqual(ckey1, ckeyX);
            SerializeNotEqual(ckey2, ckeyX);
            SerializeNotEqual(ckey3, ckeyX);
            */

            /// Format args
            // Arrange
            IAssetKey fkey1 = root1.Section("MySection").Section("MySubsection").Key("Key").Format(0xBad);
            IAssetKey fkey2 = root1.Section("MySection").Section("MySubsection").Format(0xBad).Key("Key");
            IAssetKey fkey3 = root1.Section("MySection").Format(0xBad).Section("MySubsection").Key("Key");
            IAssetKey fkey4 = root1.Format(0xBad).Section("MySection").Section("MySubsection").Key("Key");
            IAssetKey fkeyX = root1.Section("MySection").Section("MySubsection").Key("Key");
            // Assert
            SerializeEqual(fkey1, fkey2, fkey3);
            SerializeNotEqual(fkey1, fkeyX);
            SerializeNotEqual(fkey2, fkeyX);
            SerializeNotEqual(fkey3, fkeyX);

        }

        public static void SerializeEqual(IAssetKey _x, IAssetKey _y)
        {
            byte[] xData = LocalizationKeySerialization.Instance.Serialize(_x), yData = LocalizationKeySerialization.Instance.Serialize(_y);
            IAssetKey x = LocalizationKeySerialization.Instance.Deserialize(xData), y = LocalizationKeySerialization.Instance.Deserialize(yData);
            Assert.AreEqual(_x, _y, "x.Equals(y) == false");
            Assert.AreEqual(_y, _x, "y.Equals(x) == false");
            Assert.IsTrue(_x.GetHashCode() == _y.GetHashCode(), "x.GetHashCode() != y.GetHashCode()");
        }

        public static void SerializeEqual(IAssetKey _x, IAssetKey _y, IAssetKey _z)
        {
            byte[] xData = LocalizationKeySerialization.Instance.Serialize(_x);
            byte[] yData = LocalizationKeySerialization.Instance.Serialize(_y);
            byte[] zData = LocalizationKeySerialization.Instance.Serialize(_z);
            IAssetKey x = LocalizationKeySerialization.Instance.Deserialize(xData), y = LocalizationKeySerialization.Instance.Deserialize(yData), z = LocalizationKeySerialization.Instance.Deserialize(zData);
            Assert.AreEqual(x, y, "x.Equals(y) == false");
            Assert.AreEqual(x, z, "x.Equals(z) == false");
            Assert.AreEqual(y, x, "y.Equals(x) == false");
            Assert.AreEqual(y, z, "y.Equals(z) == false");
            Assert.AreEqual(z, x, "z.Equals(x) == false");
            Assert.AreEqual(z, y, "z.Equals(y) == false");
            int xhash = x.GetHashCode(), yhash = y.GetHashCode(), zhash = z.GetHashCode();
            Assert.IsTrue(xhash == yhash, "x.GetHashCode() != y.GetHashCode()");
            Assert.IsTrue(xhash == zhash, "x.GetHashCode() != z.GetHashCode()");
        }

        public static void SerializeNotEqual(IAssetKey _x, IAssetKey _y)
        {
            byte[] xData = LocalizationKeySerialization.Instance.Serialize(_x), yData = LocalizationKeySerialization.Instance.Serialize(_y);
            IAssetKey x = LocalizationKeySerialization.Instance.Deserialize(xData), y = LocalizationKeySerialization.Instance.Deserialize(yData);
            Assert.AreNotEqual(x, y, "x.Equals(y) == true");
            Assert.AreNotEqual(y, x, "y.Equals(x) == true");
            Assert.IsFalse(x.GetHashCode() == y.GetHashCode(), "x.GetHashCode() == y.GetHashCode()");
        }

        public static void SerializeNotEqual(IAssetKey _x, IAssetKey _y, IAssetKey _z)
        {
            byte[] xData = LocalizationKeySerialization.Instance.Serialize(_x), yData = LocalizationKeySerialization.Instance.Serialize(_y), zData = LocalizationKeySerialization.Instance.Serialize(_z);
            IAssetKey x = LocalizationKeySerialization.Instance.Deserialize(xData), y = LocalizationKeySerialization.Instance.Deserialize(yData), z = LocalizationKeySerialization.Instance.Deserialize(zData);
            Assert.AreNotEqual(x, y, "x.Equals(y) == true");
            Assert.AreNotEqual(x, z, "x.Equals(z) == true");
            Assert.AreNotEqual(y, x, "y.Equals(x) == true");
            Assert.AreNotEqual(y, z, "y.Equals(z) == true");
            Assert.AreNotEqual(z, x, "z.Equals(x) == true");
            Assert.AreNotEqual(z, y, "z.Equals(y) == true");
            int xhash = x.GetHashCode(), yhash = y.GetHashCode(), zhash = z.GetHashCode();
            Assert.IsFalse(xhash == yhash, "x.GetHashCode() == y.GetHashCode()");
            Assert.IsFalse(xhash == zhash, "x.GetHashCode() == z.GetHashCode()");
        }

    }
}
