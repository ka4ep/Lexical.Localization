// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries format provider.
    /// </summary>
    [Serializable]
    public class LinePluralRules : LineParameterBase, ILinePluralRules, ILineHint, ILineArgument<ILinePluralRules, IPluralRules>
    {
        /// <summary>
        /// Plural rules
        /// </summary>
        protected IPluralRules pluralRules;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IPluralRules PluralRules { get => pluralRules; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IPluralRules Argument0 => pluralRules;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="pluralRules"></param>
        public LinePluralRules(ILineFactory appender, ILine prevKey, IPluralRules pluralRules) : base(appender, prevKey, "PluralRules", pluralRules?.GetType()?.FullName)
        {
            this.pluralRules = pluralRules;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LinePluralRules(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.pluralRules = info.GetValue("PluralRules", typeof(IPluralRules)) as IPluralRules;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PluralRules", pluralRules);
        }
    }

    public partial class LineAppender : ILineFactory<ILinePluralRules, IPluralRules>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="formatProvider"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IPluralRules formatProvider, out ILinePluralRules line)
        {
            line = new LinePluralRules(appender, previous, formatProvider);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries format provider.
    /// </summary>
    [Serializable]
    public class StringLocalizerPluralRules : StringLocalizerParameterBase, ILinePluralRules, ILineHint, ILineArgument<ILinePluralRules, IPluralRules>
    {
        /// <summary>
        /// Plural rules
        /// </summary>
        protected IPluralRules pluralRules;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IPluralRules PluralRules { get => pluralRules; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IPluralRules Argument0 => pluralRules;

        /// <summary>
        /// Create new StringLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="pluralRules"></param>
        public StringLocalizerPluralRules(ILineFactory appender, ILine prevKey, IPluralRules pluralRules) : base(appender, prevKey, "PluralRules", pluralRules?.GetType()?.FullName)
        {
            this.pluralRules = pluralRules;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerPluralRules(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.pluralRules = info.GetValue("PluralRules", typeof(IPluralRules)) as IPluralRules;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PluralRules", pluralRules);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILinePluralRules, IPluralRules>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="formatProvider"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IPluralRules formatProvider, out ILinePluralRules StringLocalizer)
        {
            StringLocalizer = new StringLocalizerPluralRules(appender, previous, formatProvider);
            return true;
        }
    }

}
