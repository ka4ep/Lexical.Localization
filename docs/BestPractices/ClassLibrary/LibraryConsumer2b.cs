using Lexical.Localization;
using System;
using System.Globalization;

namespace TutorialProject
{
    public class Program2b
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Install additional localization that was not available in the TutorialLibrary
            IAssetSource source = XmlFileFormat.Instance.CreateFileAssetSource("LibraryLocalization-fi.xml");
            LocalizationRoot.Builder.AddSource(source).Build();

            TutorialLibrary.MyClass myClass = new TutorialLibrary.MyClass();

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use culture that was supplied by this application
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
            #endregion Snippet

            Console.ReadKey();
        }
    }
}
