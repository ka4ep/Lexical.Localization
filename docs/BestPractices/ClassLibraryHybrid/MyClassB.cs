using Lexical.Localization;

namespace TutorialLibrary3
{
    public class MyClassB
    {
        ILine<MyClass> localizer;

        public MyClassB(ILine<MyClass> localizer = default)
        {
            this.localizer = localizer ?? Localization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localizer.Key("OK").String("Operation Successful").ToString();
        }
    }
}
