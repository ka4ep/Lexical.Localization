using Lexical.Localization;

namespace TutorialLibrary2
{
    public class MyClassB
    {
        ILine<MyClass> localizer;

        public MyClassB(ILine<MyClass> localizer)
        {
            this.localizer = localizer;
        }

        public string Do()
        {
            return localizer.Key("OK").Format("Operation Successful").ToString();
        }
    }
}
