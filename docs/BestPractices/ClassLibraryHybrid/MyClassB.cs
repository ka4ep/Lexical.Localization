using Lexical.Localization;

namespace TutorialLibrary3
{
    public class MyClassB
    {
        IAssetKey<MyClass> localization;

        public MyClassB(IAssetKey<MyClass> localization = default)
        {
            this.localization = localization ?? LibraryLocalization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
