using Lexical.Localization;

namespace TutorialLibrary1
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
