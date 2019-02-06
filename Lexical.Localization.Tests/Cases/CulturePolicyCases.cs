using System.Globalization;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests
{
    [Case(nameof(ICulturePolicy), nameof(CulturePolicy))]
    public class CultureResolverCase
    {
        public object Initialize(Run run) 
            => new CulturePolicy();
    }
    
    [Case(nameof(ICulturePolicy), "en")]
    public class CulturePolicyCase_en
    {
        public object Initialize(Run run) 
            => new CulturePolicy().SetCultures("en");
    }

    [Case(nameof(ICulturePolicy), "fi")]
    public class CulturePolicyCase_fi
    {
        public object Initialize(Run run) 
            => new CulturePolicy().SetCultures("en");
    }

    [Case(nameof(ICulturePolicy), nameof(CulturePolicy) + ".SnapshotFromCurrentCulture")]
    public class CulturePolicyCase_CurrentCulture
    {
        public object Initialize(Run run) 
            => new CulturePolicy().SetToCurrentCulture().ToSnapshot();
    }

    [Case(nameof(ICulturePolicy), nameof(CulturePolicy) + ".SnapshotFromCurrentUICulture")]
    public class CulturePolicyCase_CurrentUICulture
    {
        public object Initialize(Run run) 
            => new CulturePolicy().SetToCurrentUICulture().ToSnapshot();
    }

    [Case(nameof(ICulturePolicy), nameof(CulturePolicy)+ ".SetSource()")]
    public class CulturePolicyCase_CulturePolicyFunc
    {
        public object Initialize(Run run)
        {
            CulturePolicy i = new CulturePolicy();
            return new CulturePolicy().SetSource(i);
        }
    }

    [Case(nameof(ICulturePolicy), nameof(CulturePolicy)+".SetFunc()")]
    public class CulturePolicyCase_CulturePolicyFunc2
    {
        public object Initialize(Run run)
        {
            var i = CultureInfo.GetCultureInfo("fi");
            return new CulturePolicy().SetFunc(() => i);
        }
    }

}
