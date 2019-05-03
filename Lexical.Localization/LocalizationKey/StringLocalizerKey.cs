// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lexical.Localization;
using Lexical.Localization.Plurality;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// StringLocalizerKey that implements and is assignable to following interfaces:
    ///     <see cref="IStringLocalizer"/>
    ///     <see cref="IStringLocalizerFactory"/>
    ///     <see cref="ILocalizationKey"/>
    /// </summary>
    [DebuggerDisplay("{DebugPrint()}")]
    [Serializable]
    public class StringLocalizerKey : LinePart, 
        ILocalizationKey, IAssetKeyAssignable, ILineInlinesAssigned, ILocalizationKeyFormattable, ILocalizationKeyCultureAssignable, ILocalizationKeyResolverAssignable, ILocalizationKeyFormatProviderAssignable, ILinePart, IAssetKeyTypeAssignable, IAssetKeyAssemblyAssignable, IAssetKeyResourceAssignable, IAssetKeyLocationAssignable, IAssetKeySectionAssignable, ILineParameterAssignable, IPluralRulesAssignableKey, ISerializable, IDynamicMetaObjectProvider, ILineDefaultHashCode,
        IStringLocalizer, IStringLocalizerFactory, IStringLocalizerKey
    {
        /// <summary>
        /// Local name of this key.
        /// </summary>
        protected string value;

        /// <summary>
        /// (optional) Link to previous key.
        /// </summary>
        protected ILinePart prevPart;

        /// <summary>
        /// Name of this key.
        /// </summary>
        public virtual String ParameterValue => value;

        /// <summary>
        /// Previous key
        /// </summary>
        public virtual ILinePart PreviousPart => prevPart;

        /// <summary>
        /// Debug print that prints the reference id of this key.
        /// </summary>
        /// <returns></returns>
        public string DebugPrint() 
            => ParameterNamePolicy.Instance.BuildName(this); // AssetKeyNameProvider.Default.BuildName(this);

        /// <summary>
        /// Create new localization key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="name"></param>
        public StringLocalizerKey(ILinePartAppender appender, string name) : base(appender, null)
        {
            this.value = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Create new localization key that has (optionally) a link to <paramref name="prevKey"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey">(optional)</param>
        /// <param name="name"></param>
        public StringLocalizerKey(ILinePartAppender appender, ILinePart prevKey, string name) : base(appender, prevKey)
        {
            this.value = name ?? throw new ArgumentNullException(nameof(name));
            this.prevPart = prevKey;
        }

        /// <summary>
        /// Append "Key".
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        public _Key Key(string subkey) => new _Key(Appender, this, subkey);

        /// <summary>
        /// Append "Key".
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        IAssetKeyAssigned IAssetKeyAssignable.Key(string subkey) => new _Key(Appender, this, subkey);

        /// <summary>
        /// Key part.
        /// </summary>
        [Serializable]
        public class _Key : StringLocalizerKey, IAssetKeyAssigned, ILineParameterPart, ILineKeyCanonicallyCompared
        {
            /// <summary>
            /// ParameterName
            /// </summary>
            public String ParameterName => "Key";

            /// <summary>
            /// Create new "Key" part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="name"></param>
            public _Key(ILinePartAppender appender, ILinePart prevKey, string name) : base(appender, prevKey, name) { }

            /// <summary>
            /// Deserialize a "Key".
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Key(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Append new parameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILineParameterPart ILineParameterAssignable.AppendParameter(string parameterName, string parameterValue)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (parameterValue == null) throw new ArgumentNullException(nameof(parameterValue));
            switch (parameterName)
            {
                case "Key": return new _Key(Appender, this, parameterValue);
                case "Culture": return new _Culture(Appender, this, parameterValue, null);
                case "Type": return new _Type(Appender, this, parameterValue);
                case "Section": return new _Section(Appender, this, parameterValue);
                case "Resource": return new _Resource(Appender, this, parameterValue);
                case "Assembly": return new _Assembly(Appender, this, parameterValue);
                case "Location": return new _Location(Appender, this, parameterValue);
                default: return new _Parameter(Appender, this, parameterName, parameterValue);
            }
        }

        /// <summary>
        /// Key for a parameterName that wasn't hard coded.
        /// </summary>
        [Serializable]
        public class _Parameter : StringLocalizerKey, IAssetKeyAssigned, ILineParameterPart, ILineKeyCanonicallyCompared
        {
            /// <summary>
            /// ParameterName
            /// </summary>
            protected string parameterName;

            /// <summary>
            /// Parameter Name
            /// </summary>
            public String ParameterName => parameterName;

            /// <summary>
            /// Create new parameter part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="parameterName"></param>
            /// <param name="parameterValue"></param>
            public _Parameter(ILinePartAppender appender, ILinePart prevKey, string parameterName, string parameterValue) : base(appender, prevKey, parameterValue)
            {
                this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            }

            /// <summary>
            /// Deserialize a parameter key.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Parameter(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.parameterName = info.GetValue(nameof(ParameterName), typeof(string)) as string;
            }

            /// <summary>
            /// Serialize a parameter key.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(ParameterName), parameterName);
            }
        }

        /// <summary>
        /// Append a "Culture" key part.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public _Culture Culture(CultureInfo culture) => new _Culture(Appender, this, null, culture);

        /// <summary>
        /// Append a culture key.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public _Culture Culture(string cultureName) => new _Culture(Appender, this, cultureName, null);

        /// <summary>
        /// Append a culture key.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.Culture(CultureInfo culture) => new _Culture(Appender, this, null, culture);

        /// <summary>
        /// Append a culture key.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.Culture(string cultureName) => new _Culture(Appender, this, cultureName, null);

        /// <summary>
        /// Culture key.
        /// </summary>
        [Serializable]
        public class _Culture : StringLocalizerKey, ILocalizationKeyCultureAssigned, ILineKeyNonCanonicallyCompared, ILineParameterPart
        {
            /// <summary>
            /// ParameterName
            /// </summary>
            public String ParameterName => "Culture";

            /// <summary>
            /// CultureInfo, null if non-standard culture.
            /// </summary>
            protected CultureInfo culture;

            CultureInfo ILocalizationKeyCultureAssigned.Culture => culture;

            /// <summary>
            /// Create new culture key.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="cultureName"></param>
            /// <param name="culture"></param>
            public _Culture(ILinePartAppender appender, ILinePart prevKey, string cultureName, CultureInfo culture) : base(appender, prevKey, cultureName ?? culture.Name)
            {
                try
                {
                    this.culture = culture ?? CultureInfo.GetCultureInfo(cultureName);
                }
                catch (Exception) { }
            }

            /// <summary>
            /// Deserialize culture key.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Culture(SerializationInfo info, StreamingContext context) : base(info, context) { this.culture = info.GetValue(nameof(Culture), typeof(CultureInfo)) as CultureInfo; }

            /// <summary>
            /// Serialize culture key.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Culture), culture);
            }
        }

        /// <summary>
        /// Append Inlines key part.
        /// </summary>
        /// <returns></returns>
        public _Inlines AddInlines() => _addinlines();

        /// <summary>
        /// Append Inlines key part.
        /// </summary>
        /// <returns></returns>
        protected virtual _Inlines _addinlines() => new _Inlines(Appender, this);

        /// <summary>
        /// Append Inlines key part.
        /// </summary>
        /// <returns></returns>
        ILineInlines ILineInlinesAssigned.AddInlines() => _addinlines();

        /// <summary>
        /// Key that contains inlines. 
        /// 
        /// The default value is contained in a field <see cref="_default"/>.
        /// Others are allocated dynamically with a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        [Serializable]
        public class _Inlines : StringLocalizerKey, ILineInlines
        {
            /// <summary>
            /// The value is here.
            /// </summary>
            protected IFormulationString _default;

            /// <summary>
            /// Dictionary of inlines other than default.
            /// </summary>
            protected Dictionary<ILinePart, IFormulationString> inlines;
            
            /// <summary>
            /// Create inlines.
            /// </summary>
            /// <param name="prevKey"></param>
            public _Inlines(ILinePartAppender appender, ILinePart prevKey) : base(appender, prevKey, "") { }

            /// <summary>
            /// Deserialize from <paramref name="info"/>.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Inlines(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                List<KeyValuePair<string, string>> stringLines = info.GetValue(nameof(inlines), typeof(List<KeyValuePair<string, string>>)) as List<KeyValuePair<string, string>>;
                if (stringLines != null)
                {
                    foreach(var stringLine in stringLines)
                    {
                        ILinePart key = ParameterNamePolicy.Instance.Parse(stringLine.Key);
                        if (LineComparer.Default.Equals(key, this)) _default = LexicalStringFormat.Instance.Parse(stringLine.Value);
                        else
                        {
                            if (inlines == null) inlines = new Dictionary<ILinePart, IFormulationString>(LineComparer.Default);
                            inlines[key] = LexicalStringFormat.Instance.Parse(stringLine.Value);
                        }
                    }
                }
            }

            /// <summary>
            /// Serialize into <paramref name="info"/>.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                List<KeyValuePair<string, string>> lines = new List<KeyValuePair<string, string>>();
                if (_default != null) lines.Add(new KeyValuePair<string, string>(ParameterNamePolicy.Instance.BuildName(this), _default.Text));
                if (inlines != null)
                {
                    foreach(var line in inlines)
                        lines.Add(new KeyValuePair<string, string>(ParameterNamePolicy.Instance.BuildName(line.Key), line.Value.Text));
                }
                info.AddValue(nameof(inlines), lines);
            }

            /// <summary>
            /// Gets or sets the element with the specified key.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="KeyNotFoundException"></exception>
            public IFormulationString this[ILinePart key]
            {
                get {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (_default != null && LineComparer.Default.Equals(key, this)) return _default;
                    if (inlines != null) return inlines[key];
                    throw new KeyNotFoundException(ParameterNamePolicy.Instance.BuildName(key));
                }
                set {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (LineComparer.Default.Equals(key, this)) { _default = value; return; }
                    if (value == null)
                    {
                        if (inlines != null) inlines.Remove(key);
                    } else
                    {
                        if (inlines == null) inlines = new Dictionary<ILinePart, IFormulationString>(LineComparer.Default);
                        inlines[key] = value;
                    }
                }
            }

            /// <summary>
            /// Gets an System.Collections.Generic.ICollection`1 containing the keys of the System.Collections.Generic.IDictionary`2.
            /// </summary>
            public ICollection<ILinePart> Keys
            {
                get
                {                    
                    List<ILinePart> list = new List<ILinePart>(Count);
                    if (_default != null) list.Add(this);
                    if (inlines != null) list.AddRange(inlines.Keys);
                    return list;
                }
            }

            /// <summary>
            /// Gets an System.Collections.Generic.ICollection`1 containing the values in the System.Collections.Generic.IDictionary`2.
            /// </summary>
            public ICollection<IFormulationString> Values
            {
                get
                {
                    List<IFormulationString> list = new List<IFormulationString>(Count);
                    if (_default != null) list.Add(_default);
                    if (inlines != null) list.AddRange(inlines.Values);
                    return list;
                }
            }

            /// <summary>
            /// Gets the number of elements contained in the System.Collections.Generic.ICollection`1.
            /// </summary>
            public int Count => (_default == null ? 0 : 1) + (inlines == null ? 0 : inlines.Count);

            /// <summary>
            /// Gets a value indicating whether the System.Collections.Generic.ICollection`1 is read-only.
            /// </summary>
            public bool IsReadOnly => false;

            /// <summary>
            /// Adds an item to the System.Collections.Generic.ICollection`1.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <exception cref="ArgumentNullException">key is null.</exception>
            /// <exception cref="ArgumentException">An element with the same key already exists in the System.Collections.Generic.IDictionary`2.</exception>
            public void Add(ILinePart key, IFormulationString value)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (LineComparer.Default.Equals(key, this))
                {
                    if (_default != null) throw new ArgumentException("Key already exists");
                    _default = value;
                    return;
                }
                if (inlines == null) inlines = new Dictionary<ILinePart, IFormulationString>(LineComparer.Default);
                inlines.Add(key, value);
            }

            /// <summary>
            /// Adds an item to the System.Collections.Generic.ICollection`1.
            /// </summary>
            /// <param name="item"></param>
            /// <exception cref="ArgumentNullException">key is null.</exception>
            /// <exception cref="ArgumentException">An element with the same key already exists in the System.Collections.Generic.IDictionary`2.</exception>
            public void Add(KeyValuePair<ILinePart, IFormulationString> item)
                => Add(item.Key, item.Value);

            /// <summary>
            /// Removes all items from the System.Collections.Generic.ICollection`1.
            /// </summary>
            public void Clear()
            {
                _default = null;
                if (inlines != null) inlines.Clear();
            }

            /// <summary>
            /// Determines whether the System.Collections.Generic.ICollection`1 contains a specific value.
            /// </summary>
            /// <param name="line"></param>
            /// <returns></returns>
            public bool Contains(KeyValuePair<ILinePart, IFormulationString> line)
            {
                if (line.Key == null) return false;
                if (_default != null && LineComparer.Default.Equals(this, line.Key)) return line.Value == _default;
                if (inlines != null) return inlines.Contains(line);
                return false;
            }

            /// <summary>
            /// Determines whether the System.Collections.Generic.IDictionary`2 contains an element with the specified key.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public bool ContainsKey(ILinePart key)
            {
                if (key == null) return false;
                if (_default != null && LineComparer.Default.Equals(this, key)) return true;
                if (inlines != null) return inlines.ContainsKey(key);
                return false;
            }

            /// <summary>
            /// Copies the elements of the System.Collections.Generic.ICollection`1 to an System.Array, starting at a particular System.Array index.
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            /// <exception cref="ArgumentException"></exception>
            public void CopyTo(KeyValuePair<ILinePart, IFormulationString>[] array, int arrayIndex)
            {
                if (_default != null) array[arrayIndex++] = new KeyValuePair<ILinePart, IFormulationString>(this, _default);
                if (inlines != null) ((ICollection<KeyValuePair<ILinePart, IFormulationString>>)inlines).CopyTo(array, arrayIndex); 
            }

            /// <summary>
            /// Removes the element with the specified key from the System.Collections.Generic.IDictionary`2.
            /// </summary>
            /// <param name="key"></param>
            /// <returns>true if the element is successfully removed; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"></exception>
            public bool Remove(ILinePart key)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (_default != null && LineComparer.Default.Equals(this, key)) { _default = null; return true; }
                if (inlines != null) return inlines.Remove(key);
                return false;
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection`1.
            /// </summary>
            /// <param name="line"></param>
            /// <returns>true if item was successfully removed</returns>
            public bool Remove(KeyValuePair<ILinePart, IFormulationString> line)
            {
                if (_default != null && LineComparer.Default.Equals(this, line.Key))
                {
                    if (FormulationStringComparer.Instance.Equals(_default, line.Value)) { _default = null; return true; } else { return false; }
                }
                if (inlines != null) return ((ICollection<KeyValuePair<ILinePart, IFormulationString>>)inlines).Remove(line);
                return false;
            }

            /// <summary>
            /// Gets the value associated with the specified key.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">key is null.</exception>
            public bool TryGetValue(ILinePart key, out IFormulationString value)
            {
                if (_default != null && LineComparer.Default.Equals(this, key)) { value = _default; return true; }
                if (inlines != null) return inlines.TryGetValue(key, out value);
                value = null;
                return false;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<KeyValuePair<ILinePart, IFormulationString>> GetEnumerator()
            {
                if (_default != null) yield return new KeyValuePair<ILinePart, IFormulationString>(this, _default);
                if (inlines != null) foreach (var line in inlines) yield return line;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                if (_default != null) yield return new KeyValuePair<ILinePart, IFormulationString>(this, _default);
                if (inlines != null) foreach (var line in inlines) yield return line;
            }
        }

        /// <summary>
        /// Append format args key part.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        ILineFormatArgsPart ILocalizationKeyFormattable.Format(params object[] args) => new _FormatArgs(Appender, this, args);

        /// <summary>
        /// Append format args key part.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual _FormatArgs Format(params object[] args) => new _FormatArgs(Appender, this, args);

        /// <summary>
        /// Format arguments key.
        /// </summary>
        [Serializable]
        public class _FormatArgs : StringLocalizerKey, ILineFormatArgsPart
        {
            /// <summary>
            /// Format arguments.
            /// </summary>
            protected object[] args;

            /// <summary>
            /// Format arguments.
            /// </summary>
            public virtual object[] Args => args;

            /// <summary>
            /// Create format arguments key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="args"></param>
            public _FormatArgs(ILinePartAppender appender, ILinePart prevKey, Object[] args) : base(appender, prevKey, "") { this.args = args; }

            /// <summary>
            /// Deserialize format args key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _FormatArgs(SerializationInfo info, StreamingContext context) : base(info, context) { this.args = info.GetValue(nameof(Args), typeof(Object[])) as object[]; }

            /// <summary>
            /// Serialize format args key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Args), Args);
            }
        }

        /// <summary>
        /// Append plural rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        public IPluralRulesAssignedKey PluralRules(IPluralRules rules) => new _PluralRules(Appender, this, rules);

        /// <summary>
        /// Append plural rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        IPluralRulesAssignedKey IPluralRulesAssignableKey.PluralRules(IPluralRules rules) => new _PluralRules(Appender, this, rules);

        /// <summary>
        /// Plural Rules key.
        /// </summary>
        [Serializable]
        public class _PluralRules : StringLocalizerKey, IPluralRulesAssignedKey
        {
            /// <summary>
            /// Assigned rules
            /// </summary>
            protected IPluralRules rules;

            /// <summary>
            /// Assigned rules
            /// </summary>
            public IPluralRules PluralRules => rules;

            /// <summary>
            /// Create new culture key.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="rules"></param>
            public _PluralRules(ILinePartAppender appender, ILinePart prevKey, IPluralRules rules) : base(appender, prevKey, null)
            {
            }

            /// <summary>
            /// Deserialize culture key.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _PluralRules(SerializationInfo info, StreamingContext context) : base(info, context) { this.rules = info.GetValue(nameof(PluralRules), typeof(IPluralRules)) as IPluralRules; }

            /// <summary>
            /// Serialize culture key.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(PluralRules), PluralRules);
            }
        }

        /// <summary>
        /// Append "Section" key part.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public _Section Section(string sectionName) => new _Section(Appender, this, sectionName);

        /// <summary>
        /// Append "Section" key part.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        IAssetKeySectionAssigned IAssetKeySectionAssignable.Section(string sectionName) => new _Section(Appender, this, sectionName);

        /// <summary>
        /// Section key part.
        /// </summary>
        [Serializable]
        public class _Section : StringLocalizerKey, IAssetKeySectionAssigned, ILineParameterPart, ILineKeyCanonicallyCompared
        {
            /// <summary>
            /// ParameterName
            /// </summary>
            public virtual String ParameterName => "Section";

            /// <summary>
            /// Create section key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="name"></param>
            public _Section(ILinePartAppender appender, ILinePart prevKey, string name) : base(appender, prevKey, name) { }

            /// <summary>
            /// Deserialize section key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Section(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        public _Type Type(string typename) => new _Type(Appender, this, typename);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public _Type Type(Type t) => typeConstructor.Create(t, this);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public _Type<T> Type<T>() => new _Type<T>(Appender, this);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(string typename) => new _Type(Appender, this, typename);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(Type t) => typeConstructor.Create(t, this);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IAssetKey<T> IAssetKeyTypeAssignable.Type<T>() => new _Type<T>(Appender, this);

        static RuntimeConstructor<ILinePart, _Type> typeConstructor = new RuntimeConstructor<ILinePart, _Type>(typeof(_Type<>));

        /// <summary>
        /// Type key part.
        /// </summary>
        [Serializable]
        public class _Type : StringLocalizerKey, IAssetKeyTypeAssigned, ILineParameterPart, ILineKeyNonCanonicallyCompared
        {
            /// <summary>
            /// Refered Type, or null if type was not available at construction time.
            /// </summary>
            protected Type type;

            /// <summary>
            /// Refered Type, or null if type was not available at construction time.
            /// </summary>
            Type IAssetKeyTypeAssigned.Type => type;

            /// <summary>
            /// Create Type part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="type"></param>
            public _Type(ILinePartAppender appender, ILinePart prevKey, Type type) : base(appender, prevKey, type.FullName) { this.type = type; }

            /// <summary>
            /// Create Type part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="name"></param>
            public _Type(ILinePartAppender appender, ILinePart prevKey, String name) : base(appender, prevKey, name) { this.value = name; }

            /// <summary>
            /// Deserialize Type part. Does not work in .NET Core.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Type(SerializationInfo info, StreamingContext context) : base(info, context) {
                this.type = info.GetValue(nameof(Type), typeof(Type)) as Type;
            }

            /// <summary>
            /// Serialize Type part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var t = type;
                // .NET Core can't serialize Type<T> if T isn't [Serializable]
                if (t == null) info.AddValue(nameof(Type), value); 
                else if (t.IsSerializable) info.AddValue(nameof(Type), t);
                base.GetObjectData(info, context);
            }

            /// <summary>
            /// ParameterName
            /// </summary>
            public String ParameterName => "Type";
        }

        /// <summary>
        /// Type key part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
        public class _Type<T> : _Type, IAssetKey<T>, IStringLocalizer<T>
        {
            /// <summary>
            /// Create Type key part.
            /// </summary>
            /// <param name="prevKey"></param>
            public _Type(ILinePartAppender appender, ILinePart prevKey) : base(appender, prevKey, typeof(T)) {}

            /// <summary>
            /// Create Type key part.
            /// </summary>
            /// <param name="root"></param>
            public _Type(IAssetRoot root) : base(root.Appender, root, typeof(T)) { }

            /// <summary>
            /// Deserialize Type key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Type(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(Assembly assembly) => new _Assembly(Appender, this, assembly);

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(String assemblyName) => new _Assembly(Appender, this, assemblyName);

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public _Assembly Assembly(Assembly assembly) => new _Assembly(Appender, this, assembly);

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public _Assembly Assembly(String assemblyName) => new _Assembly(Appender, this, assemblyName);

        /// <summary>
        /// Assembly key part.
        /// </summary>
        [Serializable]
        public class _Assembly : StringLocalizerKey, IAssetKeyAssemblyAssigned, ILineKeyNonCanonicallyCompared, ILineParameterPart, ILineKeyCanonicallyCompared
        {
            /// <summary>
            /// Referred Assembly, or null if was not available.
            /// </summary>
            protected Assembly assembly;

            /// <summary>
            /// Referred Assembly, or null if was not available.
            /// </summary>
            Assembly IAssetKeyAssemblyAssigned.Assembly => assembly;

            /// <summary>
            /// ParameterName
            /// </summary>
            public String ParameterName => "Assembly";

            /// <summary>
            /// Create Assembly key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="asmName"></param>
            public _Assembly(ILinePartAppender appender, ILinePart prevKey, string asmName) : base(appender, prevKey, asmName) { }

            /// <summary>
            /// Create Assembly key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="assembly"></param>
            public _Assembly(ILinePartAppender appender, ILinePart prevKey, Assembly assembly) : base(appender, prevKey, assembly.GetName().Name) { this.assembly = assembly; }

            /// <summary>
            /// Deserialize Assembly key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Assembly(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.assembly = info.GetValue(nameof(Assembly), typeof(Assembly)) as Assembly;
            }

            /// <summary>
            /// Serialize Assembly key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var a = assembly;
                info.AddValue(nameof(Assembly), a);
                base.GetObjectData(info, context);
            }
        }

        /// <summary>
        /// Append Resource key part.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        IAssetKeyResourceAssigned IAssetKeyResourceAssignable.Resource(String resourceName) => new _Resource(Appender, this, resourceName);

        /// <summary>
        /// Append Resource key part.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public _Resource Resource(String resourceName) => new _Resource(Appender, this, resourceName);

        /// <summary>
        /// Resource key part.
        /// </summary>
        [Serializable]
        public class _Resource : StringLocalizerKey, IAssetKeyResourceAssigned, ILineParameterPart, ILineKeyCanonicallyCompared
        {
            /// <summary>
            /// ParameterName.
            /// </summary>
            public String ParameterName => "Resource";

            /// <summary>
            /// Create Resource key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="asmName"></param>
            public _Resource(ILinePartAppender appender, ILinePart prevKey, string asmName) : base(appender, prevKey, asmName) { }

            /// <summary>
            /// Deserialize Resource key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Resource(SerializationInfo info, StreamingContext context) : base(info, context) {}
        }

        /// <summary>
        /// Append Location key part.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        IAssetKeyLocationAssigned IAssetKeyLocationAssignable.Location(String resourceName) => new _Location(Appender, this, resourceName);

        /// <summary>
        /// Append Location key part.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public _Location Location(String resourceName) => new _Location(Appender, this, resourceName);

        /// <summary>
        /// Location key part.
        /// </summary>
        [Serializable]
        public class _Location : StringLocalizerKey, IAssetKeyLocationAssigned, ILineParameterPart, ILineKeyCanonicallyCompared
        {
            /// <summary>
            /// ParameterName.
            /// </summary>
            public String ParameterName => "Location";

            /// <summary>
            /// Create Location key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="asmName"></param>
            public _Location(ILinePartAppender appender, ILinePart prevKey, string asmName) : base(appender, prevKey, asmName) { }

            /// <summary>
            /// Deserialize Location key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Location(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Deserialize from stream.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerKey(SerializationInfo info, StreamingContext context) : base(info.GetValue(nameof(Appender), typeof(ILinePartAppender)) as ILinePartAppender, info.GetValue(nameof(PreviousPart), typeof(ILinePart)) as ILinePart)
        {
            this.value = info.GetString(nameof(ParameterValue));
        }

        /// <summary>
        /// Append Resolver key part.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        ILocalizationKeyResolverAssigned ILocalizationKeyResolverAssignable.Resolver(ILocalizationResolver resolver) => new _Resolver(Appender, this, resolver);

        /// <summary>
        /// Append Resolver key part.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public _Resolver Resolver(ILocalizationResolver resolver) => new _Resolver(Appender, this, resolver);

        /// <summary>
        /// Resolver key part.
        /// </summary>
        [Serializable]
        public class _Resolver : StringLocalizerKey, ILocalizationKeyResolverAssigned
        {
            /// <summary>
            /// The assigned resolver, or null.
            /// </summary>
            protected ILocalizationResolver resolver;

            ILocalizationResolver ILocalizationKeyResolverAssigned.Resolver => resolver;

            /// <summary>
            /// Create Resolver key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="resolver"></param>
            public _Resolver(ILinePartAppender appender, ILinePart prevKey, ILocalizationResolver resolver) : base(appender, prevKey, "") { this.resolver = resolver; }

            /// <summary>
            /// Deserialize Resolver key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Resolver(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.resolver = info.GetValue(nameof(Resolver), typeof(ILocalizationResolver)) as ILocalizationResolver;
            }

            /// <summary>
            /// Serialize resolver key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Resolver), resolver);
            }
        }

        /// <summary>
        /// Append FormatProvider key part.
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        ILocalizationKeyFormatProviderAssigned ILocalizationKeyFormatProviderAssignable.FormatProvider(IFormatProvider formatProvider) => new _FormatProvider(Appender, this, formatProvider);

        /// <summary>
        /// Append FormatProvider key part.
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public _FormatProvider FormatProvider(IFormatProvider formatProvider) => new _FormatProvider(Appender, this, formatProvider);

        /// <summary>
        /// FormatProvider key part.
        /// </summary>
        [Serializable]
        public class _FormatProvider : StringLocalizerKey, ILocalizationKeyFormatProviderAssigned
        {
            /// <summary>
            /// The assigned formatProvider, or null.
            /// </summary>
            protected IFormatProvider formatProvider;

            IFormatProvider ILocalizationKeyFormatProviderAssigned.FormatProvider => formatProvider;

            /// <summary>
            /// Create FormatProvider key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="formatProvider"></param>
            public _FormatProvider(ILinePartAppender appender, ILinePart prevKey, IFormatProvider formatProvider) : base(appender, prevKey, "") { this.formatProvider = formatProvider; }

            /// <summary>
            /// Deserialize FormatProvider key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _FormatProvider(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.formatProvider = info.GetValue(nameof(FormatProvider), typeof(IFormatProvider)) as IFormatProvider;
            }

            /// <summary>
            /// Serialize formatProvider key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(FormatProvider), formatProvider);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ParameterValue), ParameterValue);
            info.AddValue(nameof(PreviousPart), prevPart);
        }

        /// <summary>
        /// Produce string using the following algorithm:
        ///   1. Search for language strings
        ///      a. Search for formultion arguments. Apply arguments. Return
        ///   2. Build and return key identity.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => this.ResolveFormulatedString().Value ?? this.DebugPrint();

        /// <summary>
        /// Cached dynamic object.
        /// </summary>
        protected DynamicMetaObject dynamicMetaObject;

        /// <summary>
        /// Get-or-create dynamic object.
        /// </summary>
        /// <param name="expression">the expression in the calling environgment</param>
        /// <returns>object</returns>
        public virtual DynamicMetaObject GetMetaObject(Expression expression)
        {
            var prev = dynamicMetaObject;
            if (prev?.Expression == expression) return prev;
            return dynamicMetaObject = new LocalizationKeyDynamicMetaObject(Library.Default, expression, BindingRestrictions.GetTypeRestriction(expression, typeof(ILinePart)), this);
        }

        /// <summary>
        /// Cached hashcode
        /// </summary>
        int hashcode = -1, defaultHashcode = -1;

        /// <summary>
        /// Determines if hashcode is calculated and cached
        /// </summary>
        bool hashcodeCalculated, defaultHashcodeCalculated;

        /// <summary>
        /// Preferred comparer
        /// </summary>
        static IEqualityComparer<ILinePart> comparer =
            new LineComparer()
                .AddCanonicalComparer(ParameterComparer.Instance)
                .AddComparer(NonCanonicalComparer.Instance)
                .AddComparer(new LocalizationKeyFormatArgsComparer())
                .SetReadonly();

        /// <summary>
        /// Comparer that compares key reference and <see cref="ILineFormatArgsPart"/>.
        /// </summary>
        public static IEqualityComparer<ILinePart> FormatArgsComparer => comparer;

        /// <summary>
        /// Equals comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) 
            => comparer.Equals(this, obj as ILinePart);

        /// <summary>
        /// Calculate hashcode with hashesin format arguments. Result is cached.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (hashcodeCalculated) return hashcode;
            hashcode = comparer.GetHashCode(this);
            hashcodeCalculated = true;
            return hashcode;
        }

        /// <summary>
        /// Calculate default reference hashcode. Result is cached.
        /// </summary>
        /// <returns></returns>
        int ILineDefaultHashCode.GetDefaultHashCode()
        {
            // Return cached default hashcode
            if (defaultHashcodeCalculated) return defaultHashcode;

            // Get previous key's default hashcode
            if (this is ILineKeyCanonicallyCompared == false && this is ILineKeyNonCanonicallyCompared == false && this.prevPart is ILineDefaultHashCode prevDefaultHashcode)
            {
                defaultHashcode = prevDefaultHashcode.GetDefaultHashCode();
            } else
            {
                defaultHashcode = LineComparer.Default.CalculateHashCode(this);
            }

            // Mark calculated
            Thread.MemoryBarrier();
            defaultHashcodeCalculated = true;
            return defaultHashcode;
        }


        /// <summary>
        /// Create Resource key part.
        /// </summary>
        /// <param name="basename"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public IStringLocalizer Create(string basename, string location) => new _Resource(Appender, new _Assembly(Appender, this, location), basename);

        /// <summary>
        /// Create Type key part.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IStringLocalizer Create(Type type) => typeConstructor.Create(type, this);

        /// <summary>
        /// Create Culture key part.
        /// </summary>
        /// <param name="newCulture"></param>
        /// <returns></returns>
        public IStringLocalizer WithCulture(CultureInfo newCulture)
        {
            // Find culture key
            ILinePart oldCultureKey = this.FindCultureKey();
            // No culture key, create new
            if (oldCultureKey == null) return newCulture == null ? this : new _Culture(Appender, this, null, newCulture);
            // Old culture matches the new, return as is
            if (oldCultureKey?.GetParameterValue() == newCulture?.Name) return this;

            // Replace culture
            ILinePart beforeCultureKey = oldCultureKey?.PreviousPart;
            if (beforeCultureKey == null) throw new InvalidOperationException("Cannot change culture when culture is the root key.");
            // Read parameters
            List<(string, string)> parameters = new List<(string, string)>();
            for (ILinePart k = this; k != oldCultureKey; k = k.PreviousPart)
                if (k is ILineParameterPart parameterKey && parameterKey.ParameterName != null)
                    parameters.Add((parameterKey.ParameterName, parameterKey.ParameterValue));
            // Assign new culture
            ILinePart result = newCulture == null ? beforeCultureKey : beforeCultureKey.Culture(newCulture);
            // Apply parameters
            for (int i = parameters.Count - 1; i >= 0; i--)
                result = result.Parameter(parameters[i].Item1, parameters[i].Item2);
            return (IStringLocalizer)result;
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalizedString this[string name] 
        { 
            get {
                ILocalizationKey key = this.Key(name);
                LocalizationString printedString = key.ResolveFormulatedString();
                if (printedString.Value == null)
                    return new LocalizedString(name, key.BuildName(), true);
                else
                    return new LocalizedString(name, printedString.Value);
                }
        }

        /// <summary>
        /// Create localized string with format arguments.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public LocalizedString this[string name, params object[] arguments] 
        { 
            get {
                ILocalizationKey key = this.Key(name).Format(arguments);
                LocalizationString printedString = key.ResolveFormulatedString();
                if (printedString.Value == null)
                    return new LocalizedString(name, key.BuildName(), true);
                else
                    return new LocalizedString(name, printedString.Value);
                }
        }

        /// <summary>
        /// Get all strings as localized strings.
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            ILocalizationStringLinesEnumerable collections = this.FindAsset() as ILocalizationStringLinesEnumerable;
            if (collections == null) return null;

            CultureInfo ci = null;
            if (includeParentCultures && ((ci = this.FindCulture()) != null))
            {
                IEnumerable<LocalizedString> result = null;
                while (true)
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> strs = collections?.GetAllStringLines(this);
                    if (strs != null)
                    {
                        IEnumerable<LocalizedString> converted = ConvertStrings(strs);
                        result = result == null ? converted : result.Concat(converted);
                    }

                    if (ci.Parent == ci || ci.Parent == null || ci.Name == ci.Parent?.Name) break;
                }
                return result;
            }
            else
            {
                IEnumerable<KeyValuePair<string, IFormulationString>> strs = collections?.GetAllStringLines(this);
                return strs == null ? null : ConvertStrings(strs);
            }
        }

        /// <summary>
        /// Convert strings to localized strings.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        IEnumerable<LocalizedString> ConvertStrings(IEnumerable<KeyValuePair<string, IFormulationString>> lines)
        {
            foreach(var kp in lines)
            {
                string value = kp.Value.Text; // <- What kind of value is expected? Is formulation expected?
                yield return new LocalizedString(kp.Key, value);
            }
        }

        /// <summary>
        /// A library of interfaces and extension methods that DynamicMetaObject implementation seaches from when 
        /// invoked with dynamic calls.
        /// </summary>
        public static class Library
        {
            private static Lazy<DynamicObjectLibrary> lazy = new Lazy<DynamicObjectLibrary>(CreateDefault);

            /// <summary>
            /// Library of methods, fields and properties for dynamic object.
            /// </summary>
            public static DynamicObjectLibrary Default => lazy.Value;

            /// <summary>
            /// Create library of methods, fields and properties for dynamic object implementation.
            /// </summary>
            /// <returns></returns>
            public static DynamicObjectLibrary CreateDefault()
                => LocalizationKey.Library.CreateDefault();
        }
    }

}
