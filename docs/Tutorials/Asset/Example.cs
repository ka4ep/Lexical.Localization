using Lexical.Localization;
using Lexical.Localization.Asset;
using System;
using System.Globalization;

namespace TutorialProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a loader
            IAsset asset = IniLinesReader.Default.FileAsset("HelloWorld.ini");

            // Add asset to global singleton instance
            LineRoot.Builder.AddAsset(asset);
            LineRoot.Builder.Build();

            // Take reference of the root
            ILineRoot root = LineRoot.Global;

            // Create key
            ILine key = root.Type<Program>().Key("Hello").Format("Hello World!");

            // Print with current culture
            Console.WriteLine(key);

            // Print with other cultures
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(key);

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(key);

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            Console.WriteLine(key);

            Console.ReadKey();
        }
    }
}
