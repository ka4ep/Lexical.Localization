// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Collections;
using Lexical.Localization.Plurality;

namespace Lexical.Localization
{    
    /// <summary>
    /// Key for language strings and binary asset localizations.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebugPrint()}")]
    public class LocalizationKey : LinePart,
#region Interfaces
        ILocalizationKey, IAssetKeyAssignable, ILineInlinesAssigned, ILocalizationKeyFormattable, ILocalizationKeyCultureAssignable, ILocalizationKeyResolverAssignable, ILocalizationKeyFormatProviderAssignable, ILine, IAssetKeyTypeAssignable, IAssetKeyAssemblyAssignable, IAssetKeyResourceAssignable, IAssetKeyLocationAssignable, IAssetKeySectionAssignable, ILineParameterAssignable, IPluralRulesAssignableKey, ISerializable, IDynamicMetaObjectProvider, ILineDefaultHashCode
    #endregion Interfaces
    {
        #region Code

        /// <summary>
        /// Local name of this key.
        /// </summary>
        protected string value;

        /// <summary>
        /// (optional) Link to previous key.
        /// </summary>
        protected ILine prevPart;

        /// <summary>
        /// Name of this key.
        /// </summary>
        public virtual String ParameterValue => value;

        /// <summary>
        /// Previous key
        /// </summary>
        public virtual ILine PreviousPart => prevPart;

        /// <summary>
        /// Debug print that prints the reference id of this key.
        /// </summary>
        /// <returns></returns>
        public string DebugPrint() 
            => ParameterPolicy.Instance.Print(this); // KeyPrinter.Default.Print(this);

        /// <summary>
        /// Create new localization key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="name"></param>
        public LocalizationKey(ILineFactory appender, string name) : base(appender, null)
        {
            this.value = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Create new localization key that has (optionally) a link to <paramref name="prevKey"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey">(optional)</param>
        /// <param name="name"></param>
        public LocalizationKey(ILineFactory appender, ILine prevKey, string name) : base(appender, prevKey)
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
        public class _Key : LocalizationKey, IAssetKeyAssigned, ILineParameter, ILineCanonicalKey
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
            public _Key(ILineFactory appender, ILine prevKey, string name) : base(appender, prevKey, name) { }

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
        ILineParameter ILineParameterAssignable.AppendParameter(string parameterName, string parameterValue)
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
        public class _Parameter : LocalizationKey, IAssetKeyAssigned, ILineParameter
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
            public _Parameter(ILineFactory appender, ILine prevKey, string parameterName, string parameterValue) : base(appender, prevKey, parameterValue)
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
        ILineKeyCulture ILocalizationKeyCultureAssignable.Culture(CultureInfo culture) => new _Culture(Appender, this, null, culture);

        /// <summary>
        /// Append a culture key.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        ILineKeyCulture ILocalizationKeyCultureAssignable.Culture(string cultureName) => new _Culture(Appender, this, cultureName, null);

        /// <summary>
        /// Culture key.
        /// </summary>
        [Serializable]
        public class _Culture : LocalizationKey, ILineKeyCulture, ILineNonCanonicalKey, ILineParameter
        {
            /// <summary>
            /// ParameterName
            /// </summary>
            public String ParameterName => "Culture";

            /// <summary>
            /// CultureInfo, null if non-standard culture.
            /// </summary>
            protected CultureInfo culture;

            CultureInfo ILineCulture.Culture { get => culture; set => throw new InvalidOperationException(); }

            /// <summary>
            /// Create new culture key.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="cultureName"></param>
            /// <param name="culture"></param>
            public _Culture(ILineFactory appender, ILine prevKey, string cultureName, CultureInfo culture) : base(appender, prevKey, cultureName ?? culture.Name)
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
        public class _Inlines : LocalizationKey, ILineInlines
        {
            /// <summary>
            /// The value is here.
            /// </summary>
            protected IFormulationString _default;

            /// <summary>
            /// Dictionary of inlines other than default.
            /// </summary>
            protected Dictionary<ILine, IFormulationString> inlines;
            
            /// <summary>
            /// Create inlines.
            /// </summary>
            /// <param name="prevKey"></param>
            public _Inlines(ILineFactory appender, ILine prevKey) : base(appender, prevKey, "") { }

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
                        ILine key = ParameterPolicy.Instance.Parse(stringLine.Key);
                        if (LineComparer.Default.Equals(key, this)) _default = LexicalStringFormat.Instance.Parse(stringLine.Value);
                        else
                        {
                            if (inlines == null) inlines = new Dictionary<ILine, IFormulationString>(LineComparer.Default);
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
                if (_default != null) lines.Add(new KeyValuePair<string, string>(ParameterPolicy.Instance.Print(this), _default.Text));
                if (inlines != null)
                {
                    foreach(var line in inlines)
                        lines.Add(new KeyValuePair<string, string>(ParameterPolicy.Instance.Print(line.Key), line.Value.Text));
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
            public IFormulationString this[ILine key]
            {
                get {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (_default != null && LineComparer.Default.Equals(key, this)) return _default;
                    if (inlines != null) return inlines[key];
                    throw new KeyNotFoundException(ParameterPolicy.Instance.Print(key));
                }
                set {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (LineComparer.Default.Equals(key, this)) { _default = value; return; }
                    if (value == null)
                    {
                        if (inlines != null) inlines.Remove(key);
                    } else
                    {
                        if (inlines == null) inlines = new Dictionary<ILine, IFormulationString>(LineComparer.Default);
                        inlines[key] = value;
                    }
                }
            }

            /// <summary>
            /// Gets an System.Collections.Generic.ICollection`1 containing the keys of the System.Collections.Generic.IDictionary`2.
            /// </summary>
            public ICollection<ILine> Keys
            {
                get
                {                    
                    List<ILine> list = new List<ILine>(Count);
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
            public void Add(ILine key, IFormulationString value)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (LineComparer.Default.Equals(key, this))
                {
                    if (_default != null) throw new ArgumentException("Key already exists");
                    _default = value;
                    return;
                }
                if (inlines == null) inlines = new Dictionary<ILine, IFormulationString>(LineComparer.Default);
                inlines.Add(key, value);
            }

            /// <summary>
            /// Adds an item to the System.Collections.Generic.ICollection`1.
            /// </summary>
            /// <param name="item"></param>
            /// <exception cref="ArgumentNullException">key is null.</exception>
            /// <exception cref="ArgumentException">An element with the same key already exists in the System.Collections.Generic.IDictionary`2.</exception>
            public void Add(KeyValuePair<ILine, IFormulationString> item)
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
            public bool Contains(KeyValuePair<ILine, IFormulationString> line)
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
            public bool ContainsKey(ILine key)
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
            public void CopyTo(KeyValuePair<ILine, IFormulationString>[] array, int arrayIndex)
            {
                if (_default != null) array[arrayIndex++] = new KeyValuePair<ILine, IFormulationString>(this, _default);
                if (inlines != null) ((ICollection<KeyValuePair<ILine, IFormulationString>>)inlines).CopyTo(array, arrayIndex); 
            }

            /// <summary>
            /// Removes the element with the specified key from the System.Collections.Generic.IDictionary`2.
            /// </summary>
            /// <param name="key"></param>
            /// <returns>true if the element is successfully removed; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"></exception>
            public bool Remove(ILine key)
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
            public bool Remove(KeyValuePair<ILine, IFormulationString> line)
            {
                if (_default != null && LineComparer.Default.Equals(this, line.Key))
                {
                    if (FormulationStringComparer.Instance.Equals(_default, line.Value)) { _default = null; return true; } else { return false; }
                }
                if (inlines != null) return ((ICollection<KeyValuePair<ILine, IFormulationString>>)inlines).Remove(line);
                return false;
            }

            /// <summary>
            /// Gets the value associated with the specified key.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">key is null.</exception>
            public bool TryGetValue(ILine key, out IFormulationString value)
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
            public IEnumerator<KeyValuePair<ILine, IFormulationString>> GetEnumerator()
            {
                if (_default != null) yield return new KeyValuePair<ILine, IFormulationString>(this, _default);
                if (inlines != null) foreach (var line in inlines) yield return line;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                if (_default != null) yield return new KeyValuePair<ILine, IFormulationString>(this, _default);
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
        public class _FormatArgs : LocalizationKey, ILineFormatArgsPart
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
            public _FormatArgs(ILineFactory appender, ILine prevKey, Object[] args) : base(appender, prevKey, "") { this.args = args; }

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
        public class _PluralRules : LocalizationKey, IPluralRulesAssignedKey
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
            public _PluralRules(ILineFactory appender, ILine prevKey, IPluralRules rules) : base(appender, prevKey, null)
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
        public class _Section : LocalizationKey, IAssetKeySectionAssigned, ILineParameter, ILineCanonicalKey
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
            public _Section(ILineFactory appender, ILine prevKey, string name) : base(appender, prevKey, name) { }

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
        ILineKeyType IAssetKeyTypeAssignable.Type(string typename) => new _Type(Appender, this, typename);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        ILineKeyType IAssetKeyTypeAssignable.Type(Type t) => typeConstructor.Create(t, this);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ILineKey<T> IAssetKeyTypeAssignable.Type<T>() => new _Type<T>(Appender, this);

        static RuntimeConstructor<ILine, _Type> typeConstructor = new RuntimeConstructor<ILine, _Type>(typeof(_Type<>));

        /// <summary>
        /// Type key part.
        /// </summary>
        [Serializable]
        public class _Type : LocalizationKey, ILineKeyType, ILineParameter, ILineNonCanonicalKey
        {
            /// <summary>
            /// Refered Type, or null if type was not available at construction time.
            /// </summary>
            protected Type type;

            /// <summary>
            /// Refered Type, or null if type was not available at construction time.
            /// </summary>
            Type ILineType.Type { get => type; set => throw new InvalidOperationException(); }

            /// <summary>
            /// Create Type part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="type"></param>
            public _Type(ILineFactory appender, ILine prevKey, Type type) : base(appender, prevKey, type.FullName) { this.type = type; }

            /// <summary>
            /// Create Type part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="name"></param>
            public _Type(ILineFactory appender, ILine prevKey, String name) : base(appender, prevKey, name) { this.value = name; }

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
        public class _Type<T> : _Type, ILineKey<T>/*TypeSectionInterfaces*/
        {
            /// <summary>
            /// Create Type key part.
            /// </summary>
            /// <param name="prevKey"></param>
            public _Type(ILineFactory appender, ILine prevKey) : base(appender, prevKey, typeof(T)) {}

            /// <summary>
            /// Create Type key part.
            /// </summary>
            /// <param name="root"></param>
            public _Type(IAssetRoot root) : base((root as ILineAppendable)?.Appender, root, typeof(T)) { }

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
        ILineKeyAssembly IAssetKeyAssemblyAssignable.Assembly(Assembly assembly) => new _Assembly(Appender, this, assembly);

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        ILineKeyAssembly IAssetKeyAssemblyAssignable.Assembly(String assemblyName) => new _Assembly(Appender, this, assemblyName);

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
        public class _Assembly : LocalizationKey, ILineKeyAssembly, ILineNonCanonicalKey, ILineParameter, ILineCanonicalKey
        {
            /// <summary>
            /// Referred Assembly, or null if was not available.
            /// </summary>
            protected Assembly assembly;

            /// <summary>
            /// Referred Assembly, or null if was not available.
            /// </summary>
            Assembly ILineAssembly.Assembly { get => assembly; set => throw new InvalidOperationException(); }

            /// <summary>
            /// ParameterName
            /// </summary>
            public String ParameterName => "Assembly";

            /// <summary>
            /// Create Assembly key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="asmName"></param>
            public _Assembly(ILineFactory appender, ILine prevKey, string asmName) : base(appender, prevKey, asmName) { }

            /// <summary>
            /// Create Assembly key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="assembly"></param>
            public _Assembly(ILineFactory appender, ILine prevKey, Assembly assembly) : base(appender, prevKey, assembly.GetName().Name) { this.assembly = assembly; }

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
        public class _Resource : LocalizationKey, IAssetKeyResourceAssigned, ILineParameter, ILineCanonicalKey
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
            public _Resource(ILineFactory appender, ILine prevKey, string asmName) : base(appender, prevKey, asmName) { }

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
        public class _Location : LocalizationKey, IAssetKeyLocationAssigned, ILineParameter, ILineCanonicalKey
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
            public _Location(ILineFactory appender, ILine prevKey, string asmName) : base(appender, prevKey, asmName) { }

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
        public LocalizationKey(SerializationInfo info, StreamingContext context) : base(info.GetValue(nameof(Appender), typeof(ILineFactory)) as ILineFactory, info.GetValue(nameof(PreviousPart), typeof(ILine)) as ILine)
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
        public class _Resolver : LocalizationKey, ILocalizationKeyResolverAssigned
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
            public _Resolver(ILineFactory appender, ILine prevKey, ILocalizationResolver resolver) : base(appender, prevKey, "") { this.resolver = resolver; }

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
        public class _FormatProvider : LocalizationKey, ILocalizationKeyFormatProviderAssigned
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
            public _FormatProvider(ILineFactory appender, ILine prevKey, IFormatProvider formatProvider) : base(appender, prevKey, "") { this.formatProvider = formatProvider; }

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
            return dynamicMetaObject = new LocalizationKeyDynamicMetaObject(Library.Default, expression, BindingRestrictions.GetTypeRestriction(expression, typeof(ILine)), this);
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
        static IEqualityComparer<ILine> comparer =
            new LineComparer()
                .AddCanonicalComparer(ParameterComparer.Instance)
                .AddComparer(NonCanonicalComparer.Instance)
                .AddComparer(new LocalizationKeyFormatArgsComparer())
                .SetReadonly();

        /// <summary>
        /// Comparer that compares key reference and <see cref="ILineFormatArgsPart"/>.
        /// </summary>
        public static IEqualityComparer<ILine> FormatArgsComparer => comparer;

        /// <summary>
        /// Equals comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) 
            => comparer.Equals(this, obj as ILine);

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
            if (this is ILineCanonicalKey == false && this is ILineNonCanonicalKey == false && this.prevPart is ILineDefaultHashCode prevDefaultHashcode)
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
        #endregion Code

        /// <summary>
        /// A library of interfaces and extension methods that DynamicMetaObject implementation seaches from when 
        /// invoked with dynamic calls.
        /// </summary>
        public static class Library
        {
            private static DynamicObjectLibrary instance = CreateDefault();

            /// <summary>
            /// Library of methods, fields and properties for dynamic object.
            /// </summary>
            public static DynamicObjectLibrary Default => instance;

            /// <summary>
            /// Create library of methods, fields and properties for dynamic object implementation.
            /// </summary>
            /// <returns></returns>
            public static DynamicObjectLibrary CreateDefault()
                => new DynamicObjectLibrary()
                    .AddExtensionMethods(typeof(ILineExtensions))
                    .AddExtensionMethods(typeof(AssetKeyExtensions))
                    .AddInterface(typeof(ILine))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeyAssigned))
                    .AddInterface(typeof(ILine))
                    .AddInterface(typeof(IAssetKeyAssetAssigned))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeySectionAssigned))
                    .AddInterface(typeof(IAssetKeySectionAssignable))
                    .AddInterface(typeof(IAssetKeyLocationAssigned))
                    .AddInterface(typeof(IAssetKeyLocationAssignable))
                    .AddInterface(typeof(ILineKeyType))
                    .AddInterface(typeof(IAssetKeyTypeAssignable))
                    .AddInterface(typeof(ILineKeyAssembly))
                    .AddInterface(typeof(IAssetKeyAssemblyAssignable))
                    .AddInterface(typeof(IAssetKeyResourceAssigned))
                    .AddInterface(typeof(IAssetKeyResourceAssignable))
                    .AddExtensionMethods(typeof(ILineExtensions))
                    .AddInterface(typeof(ILocalizationKeyCultureAssignable))
                    .AddInterface(typeof(ILineKeyCulture))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssigned))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssignable))
                    .AddInterface(typeof(ILineFormatArgsPart))
                    .AddInterface(typeof(ILocalizationKeyFormattable))
                    .AddInterface(typeof(ILineInlinesAssigned))
                    .AddInterface(typeof(ILineInlines));
        }

    }
}
