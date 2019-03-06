using Microsoft.Extensions.Localization;

namespace TutorialLibrary3
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localization;

        public MyClass(IStringLocalizer<MyClass> localization = default)
        {
            this.localization = localization ?? LibraryLocalization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localization["OK"];
        }
    }
}
