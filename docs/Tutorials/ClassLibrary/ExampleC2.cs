using Lexical.Localization;
using Lexical.Localization.Asset;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using TutorialLibrary;

namespace TutorialProject
{
    public class ExampleC
    {
        public static void Main(string[] args)
        {
            // Create asset
            Dictionary<string, string> strs = new Dictionary<string, string>();
            strs["Culture:fi:Type:TutorialLibrary.MyController3:Key:OK"] = "Toiminto onnistui";
            IAsset asset = new StringAsset()
                    .Add(strs, LineFormat.Parameters)
                    .Load();

            // Create asset root
            IStringLocalizerFactory root = new StringLocalizerRoot(asset, new CulturePolicy());

            // Call Controller
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            MyController3 controller3 = new MyController3(root);
            Console.WriteLine(controller3.Do());
        }
    }
}