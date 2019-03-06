using Lexical.Localization;

namespace TutorialLibrary2
{
    public class MyClassB
    {
        IAssetKey<MyClass> localization;

        public MyClassB(IAssetKey<MyClass> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
