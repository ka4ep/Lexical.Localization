using Microsoft.Extensions.Localization;

namespace TutorialLibrary
{
    public class MyClass2
    {
        IStringLocalizer localization;

        public MyClass2(IStringLocalizer<MyClass2> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization["OK"];
        }
    }
}
