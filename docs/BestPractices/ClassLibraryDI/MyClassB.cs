using Lexical.Localization;

namespace TutorialLibrary2
{
    public class MyClassB
    {
        IAssetKey<MyClass> localizer;

        public MyClassB(IAssetKey<MyClass> localizer)
        {
            this.localizer = localizer;
        }

        public string Do()
        {
            return localizer.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
