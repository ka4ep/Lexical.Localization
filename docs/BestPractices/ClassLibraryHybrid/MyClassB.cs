using Lexical.Localization;

namespace TutorialLibrary3
{
    public class MyClassB
    {
        ILine<MyClass> localizer;

        public MyClassB(ILine<MyClass> localizer = default)
        {
            this.localizer = localizer ?? LibraryLocalization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localizer.Key("OK").Value("Operation Successful").ToString();
        }
    }
}
