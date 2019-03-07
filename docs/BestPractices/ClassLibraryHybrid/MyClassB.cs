using Lexical.Localization;

namespace TutorialLibrary3
{
    public class MyClassB
    {
        IAssetKey<MyClass> localizer;

        public MyClassB(IAssetKey<MyClass> localizer = default)
        {
            this.localizer = localizer ?? LibraryLocalization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localizer.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
