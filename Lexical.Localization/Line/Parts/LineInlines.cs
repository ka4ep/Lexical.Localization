// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
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
    public class LineInlines : LineBase, ILineInlines, ILineArguments<IDictionary<ILine, ILine>>
    {
        /// <summary>
        /// Inlines.
        /// </summary>
        protected IDictionary<ILine, ILine> inlines;

        /// <summary>
        /// ILineInlines property
        /// </summary>
        public IDictionary<ILine, ILine> Inlines { get => inlines; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IDictionary<ILine, ILine> Argument0 => inlines;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="inlines"></param>
        public LineInlines(ILineFactory appender, ILine prevKey, IDictionary<ILine, ILine> inlines) : base(appender, prevKey)
        {
            this.inlines = inlines ?? new Dictionary<ILine, ILine>( LineComparer.Default );
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineInlines(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.inlines = info.GetValue("Inlines", typeof(IDictionary<ILine, ILine>)) as IDictionary<ILine, ILine>;
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

        ICollection<ILine> IDictionary<ILine, ILine>.Keys => inlines.Keys;
        ICollection<ILine> IDictionary<ILine, ILine>.Values => inlines.Values;
        int ICollection<KeyValuePair<ILine, ILine>>.Count => inlines.Count;
        bool ICollection<KeyValuePair<ILine, ILine>>.IsReadOnly => inlines.IsReadOnly;
        ILine IDictionary<ILine, ILine>.this[ILine key] { get => inlines[key]; set => inlines[key] = value; }
        void IDictionary<ILine, ILine>.Add(ILine key, ILine value) => inlines.Add(key, value);
        bool IDictionary<ILine, ILine>.ContainsKey(ILine key) => inlines.ContainsKey(key);
        bool IDictionary<ILine, ILine>.Remove(ILine key) => inlines.Remove(key);
        bool IDictionary<ILine, ILine>.TryGetValue(ILine key, out ILine value) => inlines.TryGetValue(key, out value);
        void ICollection<KeyValuePair<ILine, ILine>>.Add(KeyValuePair<ILine, ILine> item) => inlines.Add(item);
        void ICollection<KeyValuePair<ILine, ILine>>.Clear() => inlines.Clear();
        bool ICollection<KeyValuePair<ILine, ILine>>.Contains(KeyValuePair<ILine, ILine> item) => inlines.Contains(item);
        void ICollection<KeyValuePair<ILine, ILine>>.CopyTo(KeyValuePair<ILine, ILine>[] array, int arrayIndex) => inlines.CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<ILine, ILine>>.Remove(KeyValuePair<ILine, ILine> item) => inlines.Remove(item);
        IEnumerator<KeyValuePair<ILine, ILine>> IEnumerable<KeyValuePair<ILine, ILine>>.GetEnumerator() => inlines.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => inlines.GetEnumerator();
    }

    public partial class LineAppender : ILineFactory<ILineInlines, IDictionary<ILine, ILine>>, ILineFactory<ILineInlines>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="inlines"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IDictionary<ILine, ILine> inlines, out ILineInlines line)
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
        public virtual bool TryCreate(ILineFactory appender, ILine previous, out ILineInlines line)
        {
            line = new LineInlines(appender, previous, new Dictionary<ILine, ILine>(LineComparer.Default));
            return true;
        }
    }


    /// <summary>
    /// StringLocalizer part that carries <see cref="ILineInlines"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerInlines : StringLocalizerBase, ILineInlines, ILineArguments<IDictionary<ILine, ILine>>
    {
        /// <summary>
        /// Inlines.
        /// </summary>
        protected IDictionary<ILine, ILine> inlines;

        /// <summary>
        /// ILineInlines property
        /// </summary>
        public IDictionary<ILine, ILine> Inlines { get => inlines; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IDictionary<ILine, ILine> Argument0 => inlines;

        /// <summary>
        /// Create new StringLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="inlines"></param>
        public StringLocalizerInlines(ILineFactory appender, ILine prevKey, IDictionary<ILine, ILine> inlines) : base(appender, prevKey)
        {
            this.inlines = inlines ?? new Dictionary<ILine, ILine>(LineComparer.Default);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerInlines(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.inlines = info.GetValue("Inlines", typeof(IDictionary<ILine, ILine>)) as IDictionary<ILine, ILine>;
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

        ICollection<ILine> IDictionary<ILine, ILine>.Keys => inlines.Keys;
        ICollection<ILine> IDictionary<ILine, ILine>.Values => inlines.Values;
        int ICollection<KeyValuePair<ILine, ILine>>.Count => inlines.Count;
        bool ICollection<KeyValuePair<ILine, ILine>>.IsReadOnly => inlines.IsReadOnly;
        ILine IDictionary<ILine, ILine>.this[ILine key] { get => inlines[key]; set => inlines[key] = value; }
        void IDictionary<ILine, ILine>.Add(ILine key, ILine value) => inlines.Add(key, value);
        bool IDictionary<ILine, ILine>.ContainsKey(ILine key) => inlines.ContainsKey(key);
        bool IDictionary<ILine, ILine>.Remove(ILine key) => inlines.Remove(key);
        bool IDictionary<ILine, ILine>.TryGetValue(ILine key, out ILine value) => inlines.TryGetValue(key, out value);
        void ICollection<KeyValuePair<ILine, ILine>>.Add(KeyValuePair<ILine, ILine> item) => inlines.Add(item);
        void ICollection<KeyValuePair<ILine, ILine>>.Clear() => inlines.Clear();
        bool ICollection<KeyValuePair<ILine, ILine>>.Contains(KeyValuePair<ILine, ILine> item) => inlines.Contains(item);
        void ICollection<KeyValuePair<ILine, ILine>>.CopyTo(KeyValuePair<ILine, ILine>[] array, int arrayIndex) => inlines.CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<ILine, ILine>>.Remove(KeyValuePair<ILine, ILine> item) => inlines.Remove(item);
        IEnumerator<KeyValuePair<ILine, ILine>> IEnumerable<KeyValuePair<ILine, ILine>>.GetEnumerator() => inlines.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => inlines.GetEnumerator();
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineInlines, IDictionary<ILine, ILine>>, ILineFactory<ILineInlines>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="inlines"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IDictionary<ILine, ILine> inlines, out ILineInlines StringLocalizer)
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
        public virtual bool TryCreate(ILineFactory appender, ILine previous, out ILineInlines line)
        {
            line = new LineInlines(appender, previous, new Dictionary<ILine, ILine>(LineComparer.Default));
            return true;
        }
    }

    /// <summary></summary>
    public static partial class LineExtensions
    {
        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKey">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="valueText">(optional) value to add, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, ILine subKey, string valueText)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = line.Concat(subKey);
            if (valueText == null)
            {
                inlines.Remove(subline);
            }
            else
            {
                IStringFormat stringFormat = subline.FindStringFormat(StringFormatResolver.Default) ?? CSharpFormat.Default;
                IFormatString format = stringFormat.Parse(valueText);
                ILine value = subline.Value(format);
                inlines[subline] = value;
            }
            return line;
        }

        /// <summary>
        /// Inline <paramref name="valueText"/> to <paramref name="culture"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="culture">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="valueText">value to add</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine InlineCulture(this ILine line, string culture, string valueText)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = line.Culture(culture);
            IStringFormat stringFormat = subline.FindStringFormat(StringFormatResolver.Default) ?? CSharpFormat.Default;
            IFormatString format = stringFormat.Parse(valueText);
            ILine value = subline.Value(format);
            inlines[subline] = value;
            return line;
        }

        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKeyText">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="valueText">(optional) value to add, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, string subKeyText, string valueText)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = LineFormat.Parameters.Parse(subKeyText, line?.GetPreviousPart());
            if (valueText == null)
            {
                inlines.Remove(subline);
            }
            else
            {
                IStringFormat stringFormat = subline.FindStringFormat(StringFormatResolver.Default) ?? CSharpFormat.Default;
                IFormatString format = stringFormat.Parse(valueText);
                ILine value = subline.Value(format);
                inlines[subline] = value;
            }
            return line;
        }

        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKeyText">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="value">(optional) value to append, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, string subKeyText, IFormatString value)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = LineFormat.Parameters.Parse(subKeyText, line);
            if (value == null)
            {
                inlines.Remove(subline);
            }
            else
            {
                inlines[subline] = subline.Value(value);
            }
            return line;
        }
    }
}
