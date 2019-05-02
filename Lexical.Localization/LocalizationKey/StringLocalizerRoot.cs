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

namespace Lexical.Localization
{
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// StringLocalizerRoot implements and is assignable to:
    ///     <see cref="IStringLocalizer"/>
    ///     <see cref="IStringLocalizerFactory"/>
    ///     <see cref="ILocalizationKey"/>
    ///     <see cref="IAssetRoot"/>
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebugPrint()}")]
    public partial class StringLocalizerRoot :
        StringLocalizerKey,
        IAssetRoot, ILocalizationKey, ILocalizationKeyCulturePolicyAssigned, IAssetKeyAssetAssigned, ILocalizationKeyResolverAssigned,
        IStringLocalizer, IStringLocalizerFactory
    {
        /// <summary>
        /// (Optional) The assigned culture policy.
        /// </summary>
        protected ICulturePolicy culturePolicy;

        /// <summary>
        /// (optional) The assigned asset.
        /// </summary>
        protected IAsset asset;

        /// <summary>
        /// (optional) The assigned resolver.
        /// </summary>
        protected ILocalizationResolver resolver;

        /// <summary>
        /// (optional) The assigned format provider.
        /// </summary>
        protected IFormatProvider formatProvider;

        /// <summary>
        /// (optional) The assigned logger.
        /// </summary>
        protected IObservable<LocalizationString> logger;

        /// <summary>
        /// Culture policy. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public virtual ICulturePolicy CulturePolicy { get => culturePolicy; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Asset. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public virtual IAsset Asset { get => asset; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Resolver. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public new virtual ILocalizationResolver Resolver { get => resolver; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Format Provider. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public new virtual IFormatProvider FormatProvider { get => formatProvider; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Logger. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public virtual IObservable<LocalizationString> Logger { get => logger; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Create new root with default settings
        /// </summary>
        /// <returns></returns>
        public static StringLocalizerRoot CreateDefault() => new StringLocalizerRoot(new AssetComposition(), new CulturePolicy(), LocalizationResolver.Instance, null, null);

        /// <summary>
        /// Construct new root.
        /// </summary>
        public StringLocalizerRoot() : this(StringLocalizerAppender.Instance, null, null, null, LocalizationResolver.Instance, null, null) { }

        /// <summary>
        /// Construct new root
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="culturePolicy"></param>
        /// <param name="resolver"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logger"></param>
        public StringLocalizerRoot(IAsset asset, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = default, IFormatProvider formatProvider = null, IObservable<LocalizationString> logger = null) : 
            this(StringLocalizerAppender.Instance, null, asset, culturePolicy, resolver ?? LocalizationResolver.Instance, formatProvider, logger)
        {
        }

        /// <summary>
        /// Construct root, for subclasses.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="asset"></param>
        /// <param name="culturePolicy"></param>
        /// <param name="resolver"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logger"></param>
        protected StringLocalizerRoot(ILinePartAppender appender, ILinePart prevKey, IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver, IFormatProvider formatProvider, IObservable<LocalizationString> logger) : base(appender, prevKey, "")
        {
            this.culturePolicy = culturePolicy;
            this.asset = asset;
            this.resolver = resolver;
            this.formatProvider = formatProvider;
            this.logger = logger;
        }

        /// <summary>
        /// Root that is linked to another root.
        /// </summary>
        public class LinkedTo : StringLocalizerRoot
        {
            /// <summary>
            /// Construct root, for subclasses.
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="prevKey"></param>
            /// <param name="asset"></param>
            /// <param name="culturePolicy"></param>
            /// <param name="resolver"></param>
            /// <param name="formatProvider"></param>
            /// <param name="logger"></param>
            public LinkedTo(ILinePartAppender appender, ILinePart prevKey, IAsset asset = null, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = null, IFormatProvider formatProvider = null, IObservable<LocalizationString> logger = null) :
                base(appender, prevKey, asset, culturePolicy, resolver, formatProvider, logger)
            { }
        }

        /// <summary>
        /// Localization root where culture policy and localization asset can be changed.
        /// 
        /// Although, they must be changed with the setters of the properties. Calling assinable interface creates a new key.
        /// </summary>
        [Serializable]
        public class Mutable : StringLocalizerRoot
        {
            /// <summary>
            /// CulturePolicy
            /// </summary>
            public override ICulturePolicy CulturePolicy { get => culturePolicy; set => culturePolicy = value; }

            /// <summary>
            /// Asset
            /// </summary>
            public override IAsset Asset { get => asset; set => asset = value; }

            /// <summary>
            /// Resolver
            /// </summary>
            public override ILocalizationResolver Resolver { get => resolver; set => resolver = value; }

            /// <summary>
            /// FormatProvider
            /// </summary>
            public override IFormatProvider FormatProvider { get => formatProvider; set => formatProvider = value; }

            /// <summary>
            /// Logger
            /// </summary>
            public override IObservable<LocalizationString> Logger { get => logger; set => logger = value; }

            /// <summary>
            /// Appender
            /// </summary>
            public override ILinePartAppender Appender { get => appender; set => appender = value; }

            /// <summary>
            /// Construct mutable root.
            /// </summary>
            public Mutable() : base(StringLocalizerAppender.Instance, null, null, null, LocalizationResolver.Instance, null, null) { }

            /// <summary>
            /// Construct new root
            /// </summary>
            /// <param name="asset"></param>
            /// <param name="culturePolicy"></param>
            /// <param name="resolver"></param>
            /// <param name="formatProvider"></param>
            /// <param name="logger"></param>
            public Mutable(ILinePartAppender appender = default, IAsset asset = null, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = default, IFormatProvider formatProvider = null, IObservable<LocalizationString> logger = null) :
                this(appender ?? StringLocalizerAppender.Instance, null, asset, culturePolicy, resolver ?? LocalizationResolver.Instance, formatProvider, logger)
            {
            }

            /// <summary>
            /// Construct root, for subclasses.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="asset"></param>
            /// <param name="culturePolicy"></param>
            /// <param name="resolver"></param>
            /// <param name="formatProvider"></param>
            /// <param name="logger"></param>
            public Mutable(ILinePartAppender appender, ILinePart prevKey, IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver, IFormatProvider formatProvider, IObservable<LocalizationString> logger) : 
                base(appender, prevKey, asset, culturePolicy, resolver, formatProvider, logger)
            {
            }

            /// <summary>
            /// Deserialize mutable root.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public Mutable(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Serialize root
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //info.AddValue(nameof(CulturePolicy), culturePolicy?.Cultures?.ToArray());
        }

        /// <summary>
        /// Deserialize root
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerRoot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.Context is IDictionary<string, object> ctx)
            {
                Object assetObject = null;
                ctx.TryGetValue(nameof(IAsset), out assetObject);
                this.asset = assetObject as IAsset;

                Object culturePolicyObject = null;
                ctx.TryGetValue(nameof(ICulturePolicy), out culturePolicyObject);
                this.culturePolicy = culturePolicyObject as ICulturePolicy;
            }
        }
     
    }
}
