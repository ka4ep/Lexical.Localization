using System;
using System.Globalization;
using TutorialLibrary1;

namespace TutorialProject1
{
    public class Program1
    {
        public static void Main(string[] args)
        {
            #region Snippet
            MyClass myClass = new MyClass();

            // Use default string
            Console.WriteLine(myClass.Do());

            // Use the culture that was provided with the class library (LibraryAssetSources)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());
            #endregion Snippet
        }
    }
}
