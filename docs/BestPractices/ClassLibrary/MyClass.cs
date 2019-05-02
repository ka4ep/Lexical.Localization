using Lexical.Localization;

namespace TutorialLibrary1
{
    public class MyClass
    {
        static ILinePart localizer = LibraryLocalization.Root.Type<MyClass>();

        public string Do()
        {
            return localizer.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
