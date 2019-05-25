using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyController2
    {
        ILine localization;

        public MyController2(ILineRoot root)
        {
            this.localization = root.Type(GetType());
        }

        public MyController2(ILine<MyController2> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization.Key("OK").Value("Operation Successful").ToString();
        }
    }
}