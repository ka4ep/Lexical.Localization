// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries <see cref="ILineInlines"/>. 
    /// </summary>
    [Serializable]
    public class LineInlines : LineBase, ILineInlines, ILineArguments<IDictionary<ILine, IFormulationString>>
    {
        /// <summary>
        /// Inlines.
        /// </summary>
        protected IDictionary<ILine, IFormulationString> inlines;

        /// <summary>
        /// ILineInlines property
        /// </summary>
        public IDictionary<ILine, IFormulationString> Inlines { get => inlines; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IDictionary<ILine, IFormulationString> Argument0 => inlines;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="inlines"></param>
        public LineInlines(ILineFactory appender, ILine prevKey, IDictionary<ILine, IFormulationString> inlines) : base(appender, prevKey)
        {
            this.inlines = inlines ?? new Dictionary<ILine, IFormulationString>( LineComparer.Default );
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineInlines(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.inlines = info.GetValue("Inlines", typeof(IDictionary<ILine, IFormulationString>)) as IDictionary<ILine, IFormulationString>;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Inlines", inlines);
        }

        ICollection<ILine> IDictionary<ILine, IFormulationString>.Keys => inlines.Keys;
        ICollection<IFormulationString> IDictionary<ILine, IFormulationString>.Values => inlines.Values;
        int ICollection<KeyValuePair<ILine, IFormulationString>>.Count => inlines.Count;
        bool ICollection<KeyValuePair<ILine, IFormulationString>>.IsReadOnly => inlines.IsReadOnly;
        IFormulationString IDictionary<ILine, IFormulationString>.this[ILine key] { get => inlines[key]; set => inlines[key] = value; }
        void IDictionary<ILine, IFormulationString>.Add(ILine key, IFormulationString value) => inlines.Add(key, value);
        bool IDictionary<ILine, IFormulationString>.ContainsKey(ILine key) => inlines.ContainsKey(key);
        bool IDictionary<ILine, IFormulationString>.Remove(ILine key) => inlines.Remove(key);
        bool IDictionary<ILine, IFormulationString>.TryGetValue(ILine key, out IFormulationString value) => inlines.TryGetValue(key, out value);
        void ICollection<KeyValuePair<ILine, IFormulationString>>.Add(KeyValuePair<ILine, IFormulationString> item) => inlines.Add(item);
        void ICollection<KeyValuePair<ILine, IFormulationString>>.Clear() => inlines.Clear();
        bool ICollection<KeyValuePair<ILine, IFormulationString>>.Contains(KeyValuePair<ILine, IFormulationString> item) => inlines.Contains(item);
        void ICollection<KeyValuePair<ILine, IFormulationString>>.CopyTo(KeyValuePair<ILine, IFormulationString>[] array, int arrayIndex) => inlines.CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<ILine, IFormulationString>>.Remove(KeyValuePair<ILine, IFormulationString> item) => inlines.Remove(item);
        IEnumerator<KeyValuePair<ILine, IFormulationString>> IEnumerable<KeyValuePair<ILine, IFormulationString>>.GetEnumerator() => inlines.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => inlines.GetEnumerator();
    }

    public partial class LineAppender : ILineFactory<ILineInlines, IDictionary<ILine, IFormulationString>>, ILineFactory<ILineInlines>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="inlines"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineInlines, IDictionary<ILine, IFormulationString>>.TryCreate(ILineFactory appender, ILine previous, IDictionary<ILine, IFormulationString> inlines, out ILineInlines line)
        {
            line = new LineInlines(appender, previous, inlines);
            return true;
        }

        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineInlines>.TryCreate(ILineFactory appender, ILine previous, out ILineInlines line)
        {
            line = new LineInlines(appender, previous, new Dictionary<ILine, IFormulationString>(LineComparer.Default));
            return true;
        }
    }


    /// <summary>
    /// StringLocalizer part that carries <see cref="ILineInlines"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerInlines : StringLocalizerBase, ILineInlines, ILineArguments<IDictionary<ILine, IFormulationString>>
    {
        /// <summary>
        /// Inlines.
        /// </summary>
        protected IDictionary<ILine, IFormulationString> inlines;

        /// <summary>
        /// ILineInlines property
        /// </summary>
        public IDictionary<ILine, IFormulationString> Inlines { get => inlines; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IDictionary<ILine, IFormulationString> Argument0 => inlines;

        /// <summary>
        /// Create new StringLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="inlines"></param>
        public StringLocalizerInlines(ILineFactory appender, ILine prevKey, IDictionary<ILine, IFormulationString> inlines) : base(appender, prevKey)
        {
            this.inlines = inlines ?? new Dictionary<ILine, IFormulationString>(LineComparer.Default);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerInlines(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.inlines = info.GetValue("Inlines", typeof(IDictionary<ILine, IFormulationString>)) as IDictionary<ILine, IFormulationString>;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Inlines", inlines);
        }

        ICollection<ILine> IDictionary<ILine, IFormulationString>.Keys => inlines.Keys;
        ICollection<IFormulationString> IDictionary<ILine, IFormulationString>.Values => inlines.Values;
        int ICollection<KeyValuePair<ILine, IFormulationString>>.Count => inlines.Count;
        bool ICollection<KeyValuePair<ILine, IFormulationString>>.IsReadOnly => inlines.IsReadOnly;
        IFormulationString IDictionary<ILine, IFormulationString>.this[ILine key] { get => inlines[key]; set => inlines[key] = value; }
        void IDictionary<ILine, IFormulationString>.Add(ILine key, IFormulationString value) => inlines.Add(key, value);
        bool IDictionary<ILine, IFormulationString>.ContainsKey(ILine key) => inlines.ContainsKey(key);
        bool IDictionary<ILine, IFormulationString>.Remove(ILine key) => inlines.Remove(key);
        bool IDictionary<ILine, IFormulationString>.TryGetValue(ILine key, out IFormulationString value) => inlines.TryGetValue(key, out value);
        void ICollection<KeyValuePair<ILine, IFormulationString>>.Add(KeyValuePair<ILine, IFormulationString> item) => inlines.Add(item);
        void ICollection<KeyValuePair<ILine, IFormulationString>>.Clear() => inlines.Clear();
        bool ICollection<KeyValuePair<ILine, IFormulationString>>.Contains(KeyValuePair<ILine, IFormulationString> item) => inlines.Contains(item);
        void ICollection<KeyValuePair<ILine, IFormulationString>>.CopyTo(KeyValuePair<ILine, IFormulationString>[] array, int arrayIndex) => inlines.CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<ILine, IFormulationString>>.Remove(KeyValuePair<ILine, IFormulationString> item) => inlines.Remove(item);
        IEnumerator<KeyValuePair<ILine, IFormulationString>> IEnumerable<KeyValuePair<ILine, IFormulationString>>.GetEnumerator() => inlines.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => inlines.GetEnumerator();
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineInlines, IDictionary<ILine, IFormulationString>>, ILineFactory<ILineInlines>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="inlines"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        bool ILineFactory<ILineInlines, IDictionary<ILine, IFormulationString>>.TryCreate(ILineFactory appender, ILine previous, IDictionary<ILine, IFormulationString> inlines, out ILineInlines StringLocalizer)
        {
            StringLocalizer = new StringLocalizerInlines(appender, previous, inlines);
            return true;
        }

        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineInlines>.TryCreate(ILineFactory appender, ILine previous, out ILineInlines line)
        {
            line = new LineInlines(appender, previous, new Dictionary<ILine, IFormulationString>(LineComparer.Default));
            return true;
        }
    }

}
