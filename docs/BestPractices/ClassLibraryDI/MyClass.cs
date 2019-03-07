using Microsoft.Extensions.Localization;

namespace TutorialLibrary2
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localizer;

        public MyClass(IStringLocalizer<MyClass> localizer)
        {
            this.localizer = localizer;
        }

        public string Do()
        {
            return localizer["OK"];
        }
    }
}
