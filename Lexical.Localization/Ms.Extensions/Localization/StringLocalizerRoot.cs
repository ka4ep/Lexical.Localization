// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lexical.Localization;
using Microsoft.Extensions.Localization;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// StringLocalizerRoot implements and is assignable to:
    ///     <see cref="IStringLocalizer"/>
    ///     <see cref="IStringLocalizerFactory"/>
    ///     <see cref="ILocalizationKey"/>
    ///     <see cref="ILocalizationRoot"/>
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebugPrint()}")]
    public partial class StringLocalizerRoot :
        StringLocalizerKey,
        IAssetRoot, ILocalizationKey, ILocalizationKeyCulturePolicy, IAssetKeyAssetAssigned,
        IStringLocalizer, IStringLocalizerFactory,
        IAssetKeyNonCanonicallyCompared
    {
        protected ICulturePolicy culturePolicy;
        protected IAsset localizationAsset;

        public virtual ICulturePolicy CulturePolicy { get => culturePolicy; set { throw new InvalidOperationException(); } }
        public virtual IAsset Asset { get => localizationAsset; set { throw new InvalidOperationException(); } }

        public static StringLocalizerRoot CreateDefault() => new StringLocalizerRoot(new AssetComposition(), new CulturePolicy());

        public StringLocalizerRoot() : this("", null, null) { }
        public StringLocalizerRoot(IAsset asset) : this("", asset, null) { }
        public StringLocalizerRoot(IAsset asset, ICulturePolicy culturePolicy) : this("", asset, culturePolicy) { }
        public StringLocalizerRoot(string name, IAsset asset, ICulturePolicy culturePolicy) : base(null, name)
        {
            this.culturePolicy = culturePolicy;
            this.localizationAsset = asset;
        }

        /// <summary>
        /// Localization root where culture policy and localization asset can be changed.
        /// </summary>
        [Serializable]
        public class Mutable : StringLocalizerRoot, IAssetKeyAssetAssignable, ILocalizationKeyCulturePolicyAssignable
        {
            public override ICulturePolicy CulturePolicy { get => culturePolicy; set { SetCulturePolicy(value); } }
            public override IAsset Asset { get => localizationAsset; set { SetAsset(value); } }
            public Mutable() : base(null, null) { }
            public Mutable(IAsset languageStrings) : base(languageStrings, null) { }
            public Mutable(IAsset languageStrings, ICulturePolicy culturePolicy) : base(languageStrings, culturePolicy) { }
            public Mutable(SerializationInfo info, StreamingContext context) : base(info, context) { }

            public virtual ILocalizationKeyCulturePolicy SetCulturePolicy(ICulturePolicy culturePolicy)
            {
                this.culturePolicy = culturePolicy;
                return this;
            }

            public IAssetKeyAssetAssigned SetAsset(IAsset languageStrings)
            {
                this.localizationAsset = languageStrings;
                return this;
            }
        }

        /// <summary>
        /// Localization root where culture policy and localization asset are taken used another root.
        /// </summary>
        [Serializable]
        public class LinkedTo : StringLocalizerRoot, 
            IAssetKeyAssignable, IAssetKeyAssetAssigned,
            ILocalizationKeyCulturePolicyAssignable, ILocalizationKeyCulturePolicy
        {
            public readonly IAssetRoot link;
            IAssetKeyAssetAssignable linkLocalizationAssetAssignable;
            IAssetKeyAssetAssigned linkLocalizationAsset;
            ILocalizationKeyCulturePolicyAssignable linkCulturePolicyAssignable;
            ILocalizationKeyCulturePolicy linkCulturePolicy;

            public override ICulturePolicy CulturePolicy { get => linkCulturePolicy != null ? linkCulturePolicy.CulturePolicy : link.FindCulturePolicy(); set { SetCulturePolicy(value); } }
            public override IAsset Asset { get => linkLocalizationAsset != null ? linkLocalizationAsset.Asset : link.FindAsset(); set { SetAsset(value); } }

            public LinkedTo(IAssetRoot link) : base(null, null)
            {
                this.link = link ?? throw new ArgumentNullException(nameof(link));
                this.linkLocalizationAsset = link as IAssetKeyAssetAssigned;
                this.linkLocalizationAssetAssignable = link as IAssetKeyAssetAssignable;
                this.linkCulturePolicy = link as ILocalizationKeyCulturePolicy;
                this.linkCulturePolicyAssignable = link as ILocalizationKeyCulturePolicyAssignable;
            }

            public virtual ILocalizationKeyCulturePolicy SetCulturePolicy(ICulturePolicy culturePolicy)
            {
                ILocalizationKeyCulturePolicyAssignable a = linkCulturePolicyAssignable != null ? linkCulturePolicyAssignable : link.Get<ILocalizationKeyCulturePolicyAssignable>();
                a.SetCulturePolicy(culturePolicy);
                return this;
            }

            public IAssetKeyAssetAssigned SetAsset(IAsset asset)
            {
                IAssetKeyAssetAssignable a = linkLocalizationAssetAssignable != null ? linkLocalizationAssetAssignable : link.Get<IAssetKeyAssetAssignable>();
                a.SetAsset(asset);
                return this;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //info.AddValue(nameof(CulturePolicy), culturePolicy?.Cultures?.ToArray());
        }

        public StringLocalizerRoot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.Context is IDictionary<string, object> ctx)
            {
                Object langStringsObject = null;
                ctx.TryGetValue(nameof(IAsset), out langStringsObject);
                this.localizationAsset = langStringsObject as IAsset;

                Object culturePolicyObject = null;
                ctx.TryGetValue(nameof(ICulturePolicy), out culturePolicyObject);
                this.culturePolicy = culturePolicyObject as ICulturePolicy;
            }
        }
     
    }
}
