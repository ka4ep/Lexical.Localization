using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyController2
    {
        IAssetKey localization;

        public MyController2(IAssetRoot root)
        {
            this.localization = root.Type(GetType());
        }

        public MyController2(IAssetKey<MyController2> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}