using Lexical.Localization;
using Lexical.Utils.Permutation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Lexical.Localization.Tests
{
    [Case(nameof(IAsset), nameof(ResourceManagerAsset) + "/Location")]
    public class Asset_ResourceManagerAsset_Location : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create asset that reads .resx
            Assembly asm = GetType().Assembly;
            IAsset asset = ResourceManagerAsset.CreateLocation($"{asm.GetName().Name}.Resources.localization", GetType().Assembly);

            return asset;
        }
    }

    [Case(nameof(IAsset), nameof(ResourceManagerAsset) + "/Type")]
    public class Asset_ResourceManagerAsset_Type : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create asset that reads .resx
            Assembly asm = GetType().Assembly;
            IAsset asset = ResourceManagerAsset.CreateType(new ResourceManager($"{asm.GetName().Name}.Resources.{typeof(ConsoleApp1.MyController).FullName}", asm, null), typeof(ConsoleApp1.MyController));

            return asset;
        }
    }


}
