using System;

namespace docs
{
    class Program
    {
        static void Main(string[] args)
        {
            Serialization_Examples.Run(args);
            LocalizationDictionary_Examples.Run(args);
            AssetLoaderPartBuilder_Examples.Run(args);
            Ms_DependencyInjection_Example0.Run(args);
            Ms_DependencyInjection_Example1.Run(args);
            Ms_DependencyInjection_Example2.Run(args);
            Ms_Localization_IopExamples.Run(args);
            ILocalizationFileReader_Examples.Run(args);
            ICulturePolicy_Examples.Run(args);
            IAssetKeyNamePolicy_Examples.Run(args);
            IAssetLoader_Example_0.Run(args);
            IAssetLoader_Example_2.Run(args);
            IAssetLoader_Example_4.Run(args);
            Console.ReadKey();
        }
    }
}
