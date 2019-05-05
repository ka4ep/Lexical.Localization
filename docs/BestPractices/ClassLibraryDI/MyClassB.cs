using Lexical.Localization;

namespace TutorialLibrary2
{
    public class MyClassB
    {
        ILineKey<MyClass> localizer;

        public MyClassB(ILineKey<MyClass> localizer)
        {
            this.localizer = localizer;
        }

        public string Do()
        {
            return localizer.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
