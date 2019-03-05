using Lexical.Localization;
using System;
using System.Globalization;

namespace TutorialProject
{
    public class Program2
    {
        public static void Main(string[] args)
        {
            #region Snippet
            TutorialLibrary.MyClass myClass = new TutorialLibrary.MyClass();

            // Use default string
            Console.WriteLine(myClass.Do());

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());
            #endregion Snippet

            Console.ReadKey();
        }
    }
}
