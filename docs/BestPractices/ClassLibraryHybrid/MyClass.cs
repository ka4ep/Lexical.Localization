using Lexical.Localization;
using Microsoft.Extensions.Localization;

namespace TutorialLibrary3
{
    public class MyClass
    {
        IStringLocalizer<MyClass> localizer;

        public MyClass(IStringLocalizer<MyClass> localizer = default)
        {
            this.localizer = localizer ?? (Localization.Root.Type<MyClass>().AsStringLocalizer<MyClass>());
        }

        public string Do()
        {
            return localizer["OK"];
        }
    }
}
