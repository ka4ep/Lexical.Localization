// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Lexical.Asset
{
    /// <summary>
    /// Asset root where asset can be assigned.
    /// </summary>
    [Serializable]
    public partial class AssetRoot : AssetKey, IAssetRoot, IAssetKeyAssetAssigned, IAssetKeyNonCanonicallyCompared
    {
        protected IAsset localizationAsset;

        public virtual IAsset Asset { get => localizationAsset; set { throw new InvalidOperationException(); } }

        public AssetRoot() : this(null) { }
        public AssetRoot(IAsset asset) : base(null, "") { }

        /// <summary>
        /// Asset root where culture policy and localization asset can be changed.
        /// </summary>
        [Serializable]
        public class Mutable : AssetRoot, IAssetKeyAssetAssignable
        {
            public override IAsset Asset { get => localizationAsset; set { SetAsset(value); } }
            public Mutable() : base() { }
            public Mutable(IAsset asset) : base(asset) { }
            public Mutable(SerializationInfo info, StreamingContext context) : base(info, context) { }

            public IAssetKeyAssetAssigned SetAsset(IAsset languageStrings)
            {
                this.localizationAsset = languageStrings;
                return this;
            }
        }

        /// <summary>
        /// Asset root where culture policy and localization asset are taken used another root.
        /// </summary>
        [Serializable]
        public class LinkedTo : AssetRoot, IAssetKeyAssetAssignable, IAssetKeyAssetAssigned
        {
            public readonly IAssetRoot link;
            IAssetKeyAssetAssignable linkAssetAssetAssignable;
            IAssetKeyAssetAssigned linkAssetAsset;

            public override IAsset Asset { get => linkAssetAsset != null ? linkAssetAsset.Asset : link.FindAsset(); set { SetAsset(value); } }

            public LinkedTo(IAssetRoot link) : base(null)
            {
                this.link = link ?? throw new ArgumentNullException(nameof(link));
                this.linkAssetAsset = link as IAssetKeyAssetAssigned;
                this.linkAssetAssetAssignable = link as IAssetKeyAssetAssignable;
            }

            public IAssetKeyAssetAssigned SetAsset(IAsset asset)
            {
                IAssetKeyAssetAssignable a = linkAssetAssetAssignable != null ? linkAssetAssetAssignable : link.Get<IAssetKeyAssetAssignable>();
                a.SetAsset(asset);
                return this;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //info.AddValue(nameof(CulturePolicy), culturePolicy?.Cultures?.ToArray());
        }

        public AssetRoot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.Context is IDictionary<string, object> ctx)
            {
                Object langStringsObject = null;
                ctx.TryGetValue(nameof(IAsset), out langStringsObject);
                this.localizationAsset = langStringsObject as IAsset;
            }
        }
    }
}
