using Lexical.Localization;
using Lexical.Localization.Asset;
using System;
using System.Globalization;
using TutorialLibrary1;

namespace TutorialProject1
{
    public class Program2
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Install additional localization that was not available in the TutorialLibrary
            IAssetSource source = XmlLinesReader.Default.FileAssetSource("TutorialLibrary1-fi.xml");
            LineRoot.Builder.AddSource(source).Build();

            MyClass myClass = new MyClass();

            // Use the culture that was provided with the class library (AssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use the culture that we supplied above
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}
