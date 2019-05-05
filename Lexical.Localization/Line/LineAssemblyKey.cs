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
    public class LineAssemblyKey : LineKey, ILineKeyAssembly
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
        /// Create new assembly key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="assembly"></param>
        public LineAssemblyKey(ILinePartAppender appender, ILinePart prevKey, Assembly assembly) : base(appender, prevKey, "Assembly", assembly?.GetName()?.FullName)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineAssemblyKey(SerializationInfo info, StreamingContext context) : base(info, context)
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

    public partial class LinePartAppender : ILinePartAppender1<ILineKeyAssembly, Assembly>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public ILineKeyAssembly Append(ILinePart previous, Assembly assembly)
            => new LineAssemblyKey(this, previous, assembly);
    }
    /*
    /// <summary>
    /// "Assembly" key that carries <see cref="Assembly"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerAssemblyKey : StringLocalizerKey, ILineKeyAssembly
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
        /// Create new assembly key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="assembly"></param>
        public StringLocalizerAssemblyKey(ILinePartAppender appender, ILinePart prevKey, Assembly assembly) : base(appender, prevKey, "Assembly", assembly?.Name)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerAssemblyKey(SerializationInfo info, StreamingContext context) : base(info, context)
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

    public partial class StringLocalizerPartAppender : ILinePartAppender1<ILineKeyAssembly, Assembly>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public ILineKeyAssembly Append(ILinePart previous, Assembly assembly)
            => new StringLocalizerAssemblyKey(this, previous, assembly);
    }
*/
}
