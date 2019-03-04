using Microsoft.Extensions.Localization;

namespace TutorialLibrary
{
    public class MyController3
    {
        IStringLocalizer localization;

        public MyController3(IStringLocalizer<MyController3> localization)
        {
            this.localization = localization;
        }

        public MyController3(IStringLocalizerFactory localizationFactory)
        {
            this.localization = localizationFactory.Create(GetType());
        }

        public string Do()
        {
            return localization["OK"].ToString();
        }
    }

}
