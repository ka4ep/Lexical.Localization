using Lexical.Localization;

namespace TutorialLibrary3
{
    public class MyClassB
    {
        ILineKey<MyClass> localizer;

        public MyClassB(ILineKey<MyClass> localizer = default)
        {
            this.localizer = localizer ?? LibraryLocalization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localizer.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
