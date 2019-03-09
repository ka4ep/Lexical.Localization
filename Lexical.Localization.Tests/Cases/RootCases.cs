using Lexical.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests
{
    [Case(nameof(IAssetRoot), nameof(LocalizationRoot), new[]{ nameof(IAsset), nameof(ICulturePolicy) })]
    public class Root_LocalizationRoot
    {
        public object Initialize(Run init)
            => new LocalizationRoot(init.Get<IAsset>(), init.Get<ICulturePolicy>());
    }

    [Case(nameof(IAssetRoot), nameof(LocalizationRoot)+".Global", new[] { nameof(IAsset), nameof(ICulturePolicy) })]
    public class Root_LocalizationGlobal
    {
        public object Initialize(Run init)
        {
            LocalizationRoot.Global.CulturePolicy = init.Get<ICulturePolicy>();
            LocalizationRoot.Global.Asset = init.Get<IAsset>();
            return LocalizationRoot.Global;
        }
        public void Cleanup(Run cleanup)
        {
            LocalizationRoot.Global.Asset = LocalizationRoot.Builder.Build();
        }
    }

}
