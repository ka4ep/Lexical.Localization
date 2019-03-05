using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyClass
    {
        static IAssetKey localization = LibraryLocalization.Root.Type<MyClass>();

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
