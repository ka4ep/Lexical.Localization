using Lexical.Asset;
using System;
using System.Collections.Generic;
using System.Text;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests
{
    [Case("Cache", "No")]
    public class Cache_No
    {
        public object Initialize(Run init)
            => init.Get<IAsset>();
    }
    
    [Case("Cache", nameof(AssetCachePartStrings)+",CloneKeys=True")]
    public class Cache_LocalizationAssetCache_CloneKeys
    {
        public object Initialize(Run init)
            => init.Set<IAsset>(new AssetCache(init.Get<IAsset>()).AddResourceCache().AddStringsCache().AddCulturesCache().ConfigureOptions(o=>o.SetCloneKeys(true)));
    }
    
    [Case("Cache", nameof(AssetCachePartStrings)+",CloneKeys=False")]
    public class Cache_LocalizationAssetCache
    {
        public object Initialize(Run init)
            => init.Set<IAsset>(new AssetCache(init.Get<IAsset>()).AddResourceCache().AddStringsCache().AddCulturesCache().ConfigureOptions(o => o.SetCloneKeys(false)));
    }

}
