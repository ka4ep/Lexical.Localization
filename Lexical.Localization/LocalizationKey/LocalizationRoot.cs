// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Localization root where culture policy and language strings cannot be modified after construction.
    /// </summary>
    [Serializable]
    public partial class LocalizationRoot : 
        LocalizationKey, IAssetRoot, ILocalizationKeyCulturePolicyAssigned, IAssetKeyAssetAssigned, ILocalizationKeyResolverAssigned
    {
        #region Code
        protected ICulturePolicy culturePolicy;
        protected IAsset localizationAsset;
        protected ILocalizationResolver resolver;

        public virtual ICulturePolicy CulturePolicy { get => culturePolicy; set { throw new InvalidOperationException(); } }
        public virtual IAsset Asset { get => localizationAsset; set { throw new InvalidOperationException(); } }
        public new virtual ILocalizationResolver Resolver { get => resolver; set { throw new InvalidOperationException(); } }

        public static LocalizationRoot CreateDefault() => new LocalizationRoot(new AssetComposition(), new CulturePolicy());

        public LocalizationRoot() : this("", null, null, LocalizationResolver.Instance) { }
        public LocalizationRoot(IAsset asset) : this("", asset, null, LocalizationResolver.Instance) { }
        public LocalizationRoot(IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver = default) : this("", asset, culturePolicy, resolver ?? LocalizationResolver.Instance) { }

        protected LocalizationRoot(string name, IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver) : base(null, name)
        {
            this.culturePolicy = culturePolicy;
            this.localizationAsset = asset;
            this.resolver = resolver ?? LocalizationResolver.Instance;
        }

        /// <summary>
        /// Localization root where culture policy and localization asset can be changed.
        /// </summary>
        [Serializable]
        public class Mutable : LocalizationRoot, IAssetKeyAssetAssignable, ILocalizationKeyCulturePolicyAssignable
        {
            public override ICulturePolicy CulturePolicy { get => culturePolicy; set => SetCulturePolicy(culturePolicy); }
            public override IAsset Asset { get => localizationAsset; set { SetAsset(value); } }
            public override ILocalizationResolver Resolver { get => resolver; set { SetResolver(value); } }
            public Mutable() : base(null, null) { }
            public Mutable(IAsset languageStrings) : base(languageStrings, null) { }
            public Mutable(IAsset languageStrings, ICulturePolicy culturePolicy, ILocalizationResolver resolver) : base(languageStrings, culturePolicy, resolver) { }
            public Mutable(SerializationInfo info, StreamingContext context) : base(info, context) { }

            ILocalizationKeyCulturePolicyAssigned ILocalizationKeyCulturePolicyAssignable.CulturePolicy(ICulturePolicy culturePolicy)
                => SetCulturePolicy(culturePolicy);

            public virtual ILocalizationKeyCulturePolicyAssigned SetCulturePolicy(ICulturePolicy culturePolicy)
            {
                this.culturePolicy = culturePolicy;
                return this;
            }

            public IAssetKeyAssetAssigned SetAsset(IAsset languageStrings)
            {
                this.localizationAsset = languageStrings;
                return this;
            }

            public ILocalizationKeyResolverAssigned SetResolver(ILocalizationResolver resolver)
            {
                this.resolver = resolver;
                return this;
            }
        }

        /// <summary>
        /// Localization root where culture policy and localization asset are taken used another root.
        /// </summary>
        [Serializable]
        public class LinkedTo : LocalizationRoot, 
            IAssetKeyAssignable, IAssetKeyAssetAssigned,
            ILocalizationKeyCulturePolicyAssignable, ILocalizationKeyCulturePolicyAssigned
        {
            public readonly IAssetRoot link;
            IAssetKeyAssetAssignable linkLocalizationAssetAssignable;
            IAssetKeyAssetAssigned linkLocalizationAsset;
            ILocalizationKeyCulturePolicyAssignable linkCulturePolicyAssignable;
            ILocalizationKeyCulturePolicyAssigned linkCulturePolicy;

            public override ICulturePolicy CulturePolicy { get => linkCulturePolicy != null ? linkCulturePolicy.CulturePolicy : link.FindCulturePolicy(); set { SetCulturePolicy(value); } }
            public override IAsset Asset { get => linkLocalizationAsset != null ? linkLocalizationAsset.Asset : link.FindAsset(); set { SetAsset(value); } }

            public LinkedTo(IAssetRoot link) : base(null, null)
            {
                this.link = link ?? throw new ArgumentNullException(nameof(link));
                this.linkLocalizationAsset = link as IAssetKeyAssetAssigned;
                this.linkLocalizationAssetAssignable = link as IAssetKeyAssetAssignable;
                this.linkCulturePolicy = link as ILocalizationKeyCulturePolicyAssigned;
                this.linkCulturePolicyAssignable = link as ILocalizationKeyCulturePolicyAssignable;
            }

            public virtual ILocalizationKeyCulturePolicyAssigned SetCulturePolicy(ICulturePolicy culturePolicy)
            {
                ILocalizationKeyCulturePolicyAssignable a = linkCulturePolicyAssignable != null ? linkCulturePolicyAssignable : link.Get<ILocalizationKeyCulturePolicyAssignable>();
                a.CulturePolicy(culturePolicy);
                return this;
            }

            ILocalizationKeyCulturePolicyAssigned ILocalizationKeyCulturePolicyAssignable.CulturePolicy(ICulturePolicy culturePolicy)
                => SetCulturePolicy(culturePolicy);

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

        public LocalizationRoot(SerializationInfo info, StreamingContext context) : base(info, context)
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
        #endregion Code
    }
}
