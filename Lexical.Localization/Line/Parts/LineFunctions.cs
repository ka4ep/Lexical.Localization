// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that contains functions for string formats to use.
    /// </summary>
    [Serializable]
    public class LineFunctions : LineParameterBase, ILineFunctions, ILineHint, ILineArguments<ILineFunctions, IFunctions>
    {
        /// <summary>
        /// Functions for string formats to use
        /// </summary>
        protected IFunctions functions;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IFunctions Functions { get => functions; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IFunctions Argument0 => functions;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="functions"></param>
        public LineFunctions(ILineFactory appender, ILine prevKey, IFunctions functions) : base(appender, prevKey, "Functions", functions?.GetType()?.FullName)
        {
            this.functions = functions;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineFunctions(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.functions = info.GetValue("Functions", typeof(IFunctions)) as IFunctions;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Functions", functions);
        }
    }

    public partial class LineAppender : ILineFactory<ILineFunctions, IFunctions>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="functions"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IFunctions functions, out ILineFunctions line)
        {
            line = new LineFunctions(appender, previous, functions);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries format provider.
    /// </summary>
    [Serializable]
    public class StringLocalizerFunctions : StringLocalizerParameterBase, ILineFunctions, ILineHint, ILineArguments<ILineFunctions, IFunctions>
    {
        /// <summary>
        /// Format provider
        /// </summary>
        protected IFunctions functions;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IFunctions Functions { get => functions; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IFunctions Argument0 => functions;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="functions"></param>
        public StringLocalizerFunctions(ILineFactory appender, ILine prevKey, IFunctions functions) : base(appender, prevKey, "Functions", functions?.GetType()?.FullName)
        {
            this.functions = functions;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerFunctions(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.functions = info.GetValue("Functions", typeof(IFunctions)) as IFunctions;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Functions", functions);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineFunctions, IFunctions>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="functions"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IFunctions functions, out ILineFunctions StringLocalizer)
        {
            StringLocalizer = new StringLocalizerFunctions(appender, previous, functions);
            return true;
        }
    }

}
