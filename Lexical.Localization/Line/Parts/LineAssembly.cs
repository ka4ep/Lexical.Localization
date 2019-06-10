// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// "Assembly" key that carries <see cref="Assembly"/>. 
    /// </summary>
    [Serializable]
    public class LineAssembly : LineKey, ILineAssembly, ILineNonCanonicalKey, ILineArgument<ILineAssembly, Assembly>
    {
        /// <summary>
        /// Assembly, null if non-standard assembly.
        /// </summary>
        protected Assembly assembly;

        /// <summary>
        /// Assembly property
        /// </summary>
        public Assembly Assembly { get => assembly; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public Assembly Argument0 => assembly;

        /// <summary>
        /// Create new assembly key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="assembly"></param>
        public LineAssembly(ILineFactory appender, ILine prevKey, Assembly assembly) : base(appender, prevKey, "Assembly", assembly?.GetName()?.Name)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineAssembly(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.assembly = info.GetValue("Assembly", typeof(Assembly)) as Assembly;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Assembly", assembly);
        }
    }

    public partial class LineAppender : ILineFactory<ILineAssembly, Assembly>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="assembly"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, Assembly assembly, out ILineAssembly line)
        {
            line = new LineAssembly(appender, previous, assembly);
            return true;
        }
    }

    /// <summary>
    /// "Assembly" key that carries <see cref="Assembly"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerAssembly : _StringLocalizerKey, ILineAssembly, ILineNonCanonicalKey, ILineArgument<ILineAssembly, Assembly>
    {
        /// <summary>
        /// Assembly, null if non-standard assembly.
        /// </summary>
        protected Assembly assembly;

        /// <summary>
        /// Assembly property
        /// </summary>
        public Assembly Assembly { get => assembly; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public Assembly Argument0 => assembly;

        /// <summary>
        /// Create new assembly key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="assembly"></param>
        public StringLocalizerAssembly(ILineFactory appender, ILine prevKey, Assembly assembly) : base(appender, prevKey, "Assembly", assembly?.GetName()?.Name)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerAssembly(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.assembly = info.GetValue("Assembly", typeof(Assembly)) as Assembly;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Assembly", assembly);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineAssembly, Assembly>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="assembly"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, Assembly assembly, out ILineAssembly StringLocalizer)
        {
            StringLocalizer = new StringLocalizerAssembly(appender, previous, assembly);
            return true;
        }
    }

}
