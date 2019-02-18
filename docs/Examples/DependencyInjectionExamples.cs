using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace docs
{
    public class DependencyInjectionExamples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            // Initialize service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add lexical localizations
            //     ILocalizationKey<>, ILocalizationRoot
            //     IStringLocalizer<>, IStringLozalizerFactory
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: true,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);

            // Add embedded .ini file(s) using IConfiguration
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            serviceCollection.AddSingleton<IConfiguration>(configurationBuilder.Build());
            IFileProvider embedded = new EmbeddedFileProvider(typeof(DependencyInjectionExamples).Assembly);
            configurationBuilder.SetFileProvider(embedded);
            configurationBuilder.AddIniFile("localization.ini", optional: true, reloadOnChange: false);
            serviceCollection.AddSingleton<IAssetSource, ConfigurationBuilderLocalizationAssetSource>();

            // Add ResourceManagers
            //serviceCollection.AddSingleton<ResourceManagerStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
            //serviceCollection.AddSingleton( s=>s.GetService<ResourceManagerStringLocalizerFactory>().ToSource() );

            // Add cache
            serviceCollection.AddSingleton<IAssetSource>(new AssetCacheSource(_ => _.AddResourceCache().AddStringsCache().AddCulturesCache()));

            serviceCollection.AddTransient<MyController4>();
            serviceCollection.AddTransient<MyController5>();

            using (ServiceProvider service = serviceCollection.BuildServiceProvider())
            {
                IAssetKey root = service.GetService<IAssetRoot>();
                IStringLocalizerFactory factory = service.GetService<IStringLocalizerFactory>();

                Console.WriteLine(root.Section("ConsoleApp1").Section("MyController").Key("Success"));

                var controller4 = service.GetService<MyController4>();
                var controller5 = service.GetService<MyController5>();

                IAssetKey key = root.AssemblySection(typeof(MyController5).Assembly).TypeSection(typeof(MyController5));
                IStringLocalizer localizer = factory.Create(typeof(MyController5).Name, typeof(MyController5).Assembly.FullName);
                
            }
        }
    }

    public class MyController4
    {
        IStringLocalizer<MyController4> localizer;

        public MyController4(IStringLocalizer<MyController4> localizer)
        {
            this.localizer = localizer;
            Console.WriteLine(this.localizer?["Success"]);
        }
    }

    public class MyController5
    {
        IAssetKey<MyController5> localizer;

        public MyController5(IAssetKey<MyController5> localizer)
        {
            this.localizer = localizer;
            Console.WriteLine(this.localizer?.Key("Success"));
        }
    }

}

