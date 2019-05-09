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
    public partial class LineRoot : LineBase, ILineRoot, ILineCulturePolicy, ILineAsset, ILineLocalizationResolver, ILineFormatProvider, ILineLogger
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
        protected IObserver<LocalizationString> logger;

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
        public virtual ILocalizationResolver Resolver { get => resolver; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Format Provider. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public virtual IFormatProvider FormatProvider { get => formatProvider; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Logger. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public virtual IObserver<LocalizationString> Logger { get => logger; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Create new root with default settings
        /// </summary>
        /// <returns></returns>
        public static LineRoot CreateDefault() => new LineRoot(new AssetComposition(), new CulturePolicy(), LocalizationResolver.Instance, null, null);

        /// <summary>
        /// Construct new root.
        /// </summary>
        public LineRoot() : this(LineAppender.Default, null, null, null, LocalizationResolver.Instance, null, null) { }

        /// <summary>
        /// Construct new root
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="culturePolicy"></param>
        /// <param name="resolver"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logger"></param>
        public LineRoot(IAsset asset, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = default, IFormatProvider formatProvider = null, IObserver<LocalizationString> logger = null) : 
            this(LineAppender.Default, null, asset, culturePolicy, resolver ?? LocalizationResolver.Instance, formatProvider, logger)
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
        protected LineRoot(ILineFactory appender, ILine prevKey, IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver, IFormatProvider formatProvider, IObserver<LocalizationString> logger) : base(appender, prevKey)
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
        public class LinkedTo : LineRoot
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
            public LinkedTo(ILineFactory appender, ILine prevKey, IAsset asset = null, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = null, IFormatProvider formatProvider = null, IObserver<LocalizationString> logger = null) :
                base(appender, prevKey, asset, culturePolicy, resolver, formatProvider, logger)
            { }
        }

        /// <summary>
        /// Localization root where culture policy and localization asset can be changed.
        /// 
        /// Although, they must be changed with the setters of the properties. Calling assinable interface creates a new key.
        /// </summary>
        [Serializable]
        public class Mutable : LineRoot
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
            public override IObserver<LocalizationString> Logger { get => logger; set => logger = value; }

            /// <summary>
            /// Appender
            /// </summary>
            public override ILineFactory Appender { get => appender; set => appender = value; }

            /// <summary>
            /// Construct mutable root.
            /// </summary>
            public Mutable() : base(StringLocalizerAppender.Default, null, null, null, LocalizationResolver.Instance, null, null) { }

            /// <summary>
            /// Construct new root
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="asset"></param>
            /// <param name="culturePolicy"></param>
            /// <param name="resolver"></param>
            /// <param name="formatProvider"></param>
            /// <param name="logger"></param>
            public Mutable(ILineFactory appender = default, IAsset asset = null, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = default, IFormatProvider formatProvider = null, IObserver<LocalizationString> logger = null) :
                this(appender ?? LineAppender.Default, null, asset, culturePolicy, resolver ?? LocalizationResolver.Instance, formatProvider, logger)
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
            public Mutable(ILineFactory appender, ILine prevKey, IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver, IFormatProvider formatProvider, IObserver<LocalizationString> logger) : 
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
        }

        /// <summary>
        /// Deserialize root
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineRoot(SerializationInfo info, StreamingContext context) : base(info, context)
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





    /// <summary>
    /// Localization root where culture policy and language strings cannot be modified after construction.
    /// </summary>
    [Serializable]
    public partial class StringLocalizerRoot : StringLocalizerBase, ILineRoot, ILineCulturePolicy, ILineAsset, ILineLocalizationResolver, ILineFormatProvider, ILineLogger
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
        protected IObserver<LocalizationString> logger;

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
        public virtual ILocalizationResolver Resolver { get => resolver; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Format Provider. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public virtual IFormatProvider FormatProvider { get => formatProvider; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Logger. Writable if <see cref="Mutable"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If writing to not <see cref="Mutable"/>.</exception>
        public virtual IObserver<LocalizationString> Logger { get => logger; set { throw new InvalidOperationException(); } }

        /// <summary>
        /// Create new root with default settings
        /// </summary>
        /// <returns></returns>
        public static StringLocalizerRoot CreateDefault() => new StringLocalizerRoot(new AssetComposition(), new CulturePolicy(), LocalizationResolver.Instance, null, null);

        /// <summary>
        /// Construct new root.
        /// </summary>
        public StringLocalizerRoot() : this(StringLocalizerAppender.Default, null, null, null, LocalizationResolver.Instance, null, null) { }

        /// <summary>
        /// Construct new root
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="culturePolicy"></param>
        /// <param name="resolver"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logger"></param>
        public StringLocalizerRoot(IAsset asset, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = default, IFormatProvider formatProvider = null, IObserver<LocalizationString> logger = null) :
            this(StringLocalizerAppender.Default, null, asset, culturePolicy, resolver ?? LocalizationResolver.Instance, formatProvider, logger)
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
        protected StringLocalizerRoot(ILineFactory appender, ILine prevKey, IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver, IFormatProvider formatProvider, IObserver<LocalizationString> logger) : base(appender, prevKey)
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
            public LinkedTo(ILineFactory appender, ILine prevKey, IAsset asset = null, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = null, IFormatProvider formatProvider = null, IObserver<LocalizationString> logger = null) :
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
            public override IObserver<LocalizationString> Logger { get => logger; set => logger = value; }

            /// <summary>
            /// Appender
            /// </summary>
            public override ILineFactory Appender { get => appender; set => appender = value; }

            /// <summary>
            /// Construct mutable root.
            /// </summary>
            public Mutable() : base(StringLocalizerAppender.Default, null, null, null, LocalizationResolver.Instance, null, null) { }

            /// <summary>
            /// Construct new root
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="asset"></param>
            /// <param name="culturePolicy"></param>
            /// <param name="resolver"></param>
            /// <param name="formatProvider"></param>
            /// <param name="logger"></param>
            public Mutable(ILineFactory appender = default, IAsset asset = null, ICulturePolicy culturePolicy = null, ILocalizationResolver resolver = default, IFormatProvider formatProvider = null, IObserver<LocalizationString> logger = null) :
                this(appender ?? StringLocalizerAppender.Default, null, asset, culturePolicy, resolver ?? LocalizationResolver.Instance, formatProvider, logger)
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
            public Mutable(ILineFactory appender, ILine prevKey, IAsset asset, ICulturePolicy culturePolicy, ILocalizationResolver resolver, IFormatProvider formatProvider, IObserver<LocalizationString> logger) :
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
