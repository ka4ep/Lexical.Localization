using Lexical.Localization;
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using TutorialLibrary;

namespace TutorialProject
{
    public class ExampleB
    {
        public static void Main(string[] args)
        {
            // Create asset
            Dictionary<string, string> strs = new Dictionary<string, string>();
            strs["Culture:fi:Type:TutorialLibrary.MyController2:Key:OK"] = "Toiminto onnistui";
            IAsset asset = new StringAsset()
                    .Add(strs, LineFormat.Parameters)
                    .Load();

            // Create asset root
            ILineRoot root = new LineRoot(asset, new CulturePolicy());

            // Call Controller
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            MyController2 controller2 = new MyController2(root);
            Console.WriteLine(controller2.Do());
        }
    }
}