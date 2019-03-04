using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyController1
    {
        // Use this reference 
        static IAssetKey localization = LocalizationRoot.Global.Type(typeof(MyController1));

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}