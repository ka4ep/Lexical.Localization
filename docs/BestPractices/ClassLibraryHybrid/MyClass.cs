using Microsoft.Extensions.Localization;

namespace TutorialLibrary3
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localizer;

        public MyClass(IStringLocalizer<MyClass> localizer = default)
        {
            this.localizer = localizer ?? LibraryLocalization.Root.Type<MyClass>();
        }

        public string Do()
        {
            return localizer["OK"];
        }
    }
}
