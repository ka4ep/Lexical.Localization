using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyController1
    {
        // Use this reference 
        static ILine localization = LineRoot.Global.Type(typeof(MyController1));

        public string Do()
        {
            return localization.Key("OK").Value("Operation Successful").ToString();
        }
    }
}