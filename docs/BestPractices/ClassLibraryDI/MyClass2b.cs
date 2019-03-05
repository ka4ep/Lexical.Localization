using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyClass2b
    {
        IAssetKey localization;

        public MyClass2b(IAssetKey<MyClass2> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
