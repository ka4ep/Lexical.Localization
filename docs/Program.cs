using System;

namespace docs
{
    class Program
    {
        static void Main(string[] args)
        {
            Ms_DependencyInjection_Example0.Main(args);
            Ms_DependencyInjection_Example1.Main(args);
            Ms_DependencyInjection_Example2.Main(args);

            TutorialProject1.Program1.Main(args);
            TutorialProject1.Program2.Main(args);
            TutorialProject2.Program1.Main(args);
            TutorialProject2.Program2.Main(args);
            TutorialProject2.Program3.Main(args);
            TutorialProject3.Program1.Main(args);
            TutorialProject3.Program2.Main(args);
            TutorialProject3.Program3.Main(args);


            IAssetRoot_StringLocalizer_Examples.Main(args);
            KeyPrinter_Examples.Main(args);
            ParameterParserExamples.Main(args);
            ILinePolicy_Examples.Main(args);
            LinePattern_Examples.Main(args);

            LocalizationReader_Examples.Main(args);
            AssetKeyComparer_Examples.Main(args);
            LocalizationAsset_Examples.Main(args);
            LocalizationAsset_Examples.Main(args);
            Plurality_Examples.Main(args);
            TutorialProject.ExampleB.Main(args);
            TutorialProject.ExampleC.Main(args);
            ParameterParserExamples.Main(args);
            ICulturePolicy_Examples.Main(args);
            ILinePolicy_Examples.Main(args);
            Ms_Localization_IopExamples.Main(args);
            Console.ReadKey();
        }
    }
}
