using System;

namespace docs
{
    class Program
    {
        static void Main(string[] args)
        {
            ParameterNamePolicy_Examples.Run(args);
            LocalizationAsset_Examples.Run(args);
            LocalizationStringAsset_Examples.Run(args);
            Ms_DependencyInjection_Example0.Run(args);
            Ms_DependencyInjection_Example1.Run(args);
            Ms_DependencyInjection_Example2.Run(args);
            Ms_Localization_IopExamples.Run(args);
            ICulturePolicy_Examples.Run(args);
            IAssetKeyNamePolicy_Examples.Run(args);
            Console.ReadKey();
        }
    }
}
