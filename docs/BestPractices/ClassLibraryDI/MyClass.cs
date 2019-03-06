using Microsoft.Extensions.Localization;

namespace TutorialLibrary2
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localization;

        public MyClass(IStringLocalizer<MyClass> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization["OK"];
        }
    }
}
