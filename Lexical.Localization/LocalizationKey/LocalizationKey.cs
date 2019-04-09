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

namespace Lexical.Localization
{    
    /// <summary>
    /// Key for language strings and binary asset localizations.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebugPrint()}")]
    public class LocalizationKey :
#region Interfaces
        ILocalizationKey, IAssetKeyAssignable, ILocalizationKeyInlineAssignable, ILocalizationKeyFormattable, ILocalizationKeyCultureAssignable, ILocalizationKeyResolverAssignable, ILocalizationKeyFormatProviderAssignable, IAssetKeyLinked, IAssetKeyTypeAssignable, IAssetKeyAssemblyAssignable, IAssetKeyResourceAssignable, IAssetKeyLocationAssignable, IAssetKeySectionAssignable, IAssetKeyParameterAssignable, ILocalizationKeyPluralityAssignable, ISerializable, IDynamicMetaObjectProvider, IAssetKeyDefaultHashCode
    #endregion Interfaces
    {
        #region Code

        /// <summary>
        /// Local name of this key.
        /// </summary>
        protected string name;

        /// <summary>
        /// (optional) Link to previous key.
        /// </summary>
        protected IAssetKey prevKey;

        /// <summary>
        /// Name of this key.
        /// </summary>
        public virtual String Name => name;

        /// <summary>
        /// Previous key
        /// </summary>
        public virtual IAssetKey PreviousKey => prevKey;

        /// <summary>
        /// Debug print that prints the reference id of this key.
        /// </summary>
        /// <returns></returns>
        public string DebugPrint() 
            => ParameterNamePolicy.Instance.BuildName(this); // AssetKeyNameProvider.Default.BuildName(this);

        /// <summary>
        /// Create new localization key.
        /// </summary>
        /// <param name="name"></param>
        public LocalizationKey(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Create new localization key that has (optionally) a link to <paramref name="prevKey"/>.
        /// </summary>
        /// <param name="prevKey">(optional)</param>
        /// <param name="name"></param>
        public LocalizationKey(IAssetKey prevKey, string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.prevKey = prevKey;
        }

        /// <summary>
        /// Append "Key".
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        public _Key Key(string subkey) => new _Key(this, subkey);

        /// <summary>
        /// Append "Key".
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        IAssetKeyAssigned IAssetKeyAssignable.Key(string subkey) => new _Key(this, subkey);

        /// <summary>
        /// Key part.
        /// </summary>
        [Serializable]
        public class _Key : LocalizationKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            public _Key(IAssetKey prevKey, string name) : base(prevKey, name) { }

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
        IAssetKeyParameterAssigned IAssetKeyParameterAssignable.AppendParameter(string parameterName, string parameterValue)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (parameterValue == null) throw new ArgumentNullException(nameof(parameterValue));
            switch (parameterName)
            {
                case "Key": return new _Key(this, parameterValue);
                case "Culture": return new _Culture(this, parameterValue, null);
                case "Type": return new _Type(this, parameterValue);
                case "Section": return new _Section(this, parameterValue);
                case "Resource": return new _Resource(this, parameterValue);
                case "Assembly": return new _Assembly(this, parameterValue);
                case "Location": return new _Location(this, parameterValue);
                default: return new _Parameter(this, parameterName, parameterValue);
            }
        }

        /// <summary>
        /// Key for a parameterName that wasn't hard coded.
        /// </summary>
        [Serializable]
        public class _Parameter : LocalizationKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            public _Parameter(IAssetKey prevKey, string parameterName, string parameterValue) : base(prevKey, parameterValue)
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
        public _Culture Culture(CultureInfo culture) => new _Culture(this, null, culture);

        /// <summary>
        /// Append a culture key.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public _Culture Culture(string cultureName) => new _Culture(this, cultureName, null);

        /// <summary>
        /// Append a culture key.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.Culture(CultureInfo culture) => new _Culture(this, null, culture);

        /// <summary>
        /// Append a culture key.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.Culture(string cultureName) => new _Culture(this, cultureName, null);

        /// <summary>
        /// Culture key.
        /// </summary>
        [Serializable]
        public class _Culture : LocalizationKey, ILocalizationKeyCultureAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned
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
            public _Culture(IAssetKey prevKey, string cultureName, CultureInfo culture) : base(prevKey, cultureName ?? culture.Name)
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
        protected virtual _Inlines _addinlines() => new _Inlines(this);

        /// <summary>
        /// Append Inlines key part.
        /// </summary>
        /// <returns></returns>
        ILocalizationKeyInlines ILocalizationKeyInlineAssignable.AddInlines() => _addinlines();

        /// <summary>
        /// Key that contains inlines. 
        /// 
        /// The default value is contained in a field <see cref="_default"/>.
        /// Others are allocated dynamically with a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        [Serializable]
        public class _Inlines : LocalizationKey, ILocalizationKeyInlines, IAssetKeyNonCanonicallyCompared
        {
            /// <summary>
            /// The value is here.
            /// </summary>
            protected string _default;

            /// <summary>
            /// Dictionary of inlines other than default.
            /// </summary>
            protected Dictionary<IAssetKey, string> inlines;
            
            /// <summary>
            /// Create inlines.
            /// </summary>
            /// <param name="prevKey"></param>
            public _Inlines(IAssetKey prevKey) : base(prevKey, "") { }

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
                        IAssetKey key = ParameterNamePolicy.Instance.Parse(stringLine.Key);
                        if (AssetKeyComparer.Default.Equals(key, this)) _default = stringLine.Value;
                        else
                        {
                            if (inlines == null) inlines = new Dictionary<IAssetKey, string>();
                            inlines[key] = stringLine.Value;
                        }
                    }
                }
                this.inlines = stringLines.ToKeyLines(ParameterNamePolicy.Instance).ToDictionary(AssetKeyComparer.Default);
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
                if (_default != null) lines.Add(new KeyValuePair<string, string>(ParameterNamePolicy.Instance.BuildName(this), _default));
                if (inlines != null) lines.AddRange(inlines.ToStringLines(ParameterNamePolicy.Instance));
                info.AddValue(nameof(inlines), inlines);
            }

            /// <summary>
            /// Gets or sets the element with the specified key.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="KeyNotFoundException"></exception>
            public string this[IAssetKey key]
            {
                get {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (_default != null && AssetKeyComparer.Default.Equals(key, this)) return _default;
                    if (inlines != null) return inlines[key];
                    throw new KeyNotFoundException(ParameterNamePolicy.Instance.BuildName(key));
                }
                set {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (AssetKeyComparer.Default.Equals(key, this)) { _default = value; return; }
                    if (value == null)
                    {
                        if (inlines != null) inlines.Remove(key);
                    } else
                    {
                        if (inlines == null) inlines = new Dictionary<IAssetKey, string>(AssetKeyComparer.Default);
                        inlines[key] = value;
                    }
                }
            }

            /// <summary>
            /// Gets an System.Collections.Generic.ICollection`1 containing the keys of the System.Collections.Generic.IDictionary`2.
            /// </summary>
            public ICollection<IAssetKey> Keys
            {
                get
                {                    
                    List<IAssetKey> list = new List<IAssetKey>(Count);
                    if (_default != null) list.Add(this);
                    if (inlines != null) list.AddRange(inlines.Keys);
                    return list;
                }
            }

            /// <summary>
            /// Gets an System.Collections.Generic.ICollection`1 containing the values in the System.Collections.Generic.IDictionary`2.
            /// </summary>
            public ICollection<string> Values
            {
                get
                {
                    List<string> list = new List<string>(Count);
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
            public void Add(IAssetKey key, string value)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (AssetKeyComparer.Default.Equals(key, this))
                {
                    if (_default != null) throw new ArgumentException("Key already exists");
                    _default = value;
                    return;
                }
                if (inlines == null) inlines = new Dictionary<IAssetKey, string>(AssetKeyComparer.Default);
                inlines.Add(key, value);
            }

            /// <summary>
            /// Adds an item to the System.Collections.Generic.ICollection`1.
            /// </summary>
            /// <param name="item"></param>
            /// <exception cref="ArgumentNullException">key is null.</exception>
            /// <exception cref="ArgumentException">An element with the same key already exists in the System.Collections.Generic.IDictionary`2.</exception>
            public void Add(KeyValuePair<IAssetKey, string> item)
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
            public bool Contains(KeyValuePair<IAssetKey, string> line)
            {
                if (line.Key == null) return false;
                if (_default != null && AssetKeyComparer.Default.Equals(this, line.Key)) return line.Value == _default;
                if (inlines != null) return inlines.Contains(line);
                return false;
            }

            /// <summary>
            /// Determines whether the System.Collections.Generic.IDictionary`2 contains an element with the specified key.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public bool ContainsKey(IAssetKey key)
            {
                if (key == null) return false;
                if (_default != null && AssetKeyComparer.Default.Equals(this, key)) return true;
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
            public void CopyTo(KeyValuePair<IAssetKey, string>[] array, int arrayIndex)
            {
                if (_default != null) array[arrayIndex++] = new KeyValuePair<IAssetKey, string>(this, _default);
                if (inlines != null) ((ICollection<KeyValuePair<IAssetKey, string>>)inlines).CopyTo(array, arrayIndex); 
            }

            /// <summary>
            /// Removes the element with the specified key from the System.Collections.Generic.IDictionary`2.
            /// </summary>
            /// <param name="key"></param>
            /// <returns>true if the element is successfully removed; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"></exception>
            public bool Remove(IAssetKey key)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (_default != null && AssetKeyComparer.Default.Equals(this, key)) { _default = null; return true; }
                if (inlines != null) return inlines.Remove(key);
                return false;
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection`1.
            /// </summary>
            /// <param name="line"></param>
            /// <returns>true if item was successfully removed</returns>
            public bool Remove(KeyValuePair<IAssetKey, string> line)
            {
                if (_default != null && AssetKeyComparer.Default.Equals(this, line.Key))
                {
                    if (_default == line.Value) { _default = null; return true; } else { return false; }
                }
                if (inlines != null) return ((ICollection<KeyValuePair<IAssetKey, string>>)inlines).Remove(line);
                return false;
            }

            /// <summary>
            /// Gets the value associated with the specified key.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">key is null.</exception>
            public bool TryGetValue(IAssetKey key, out string value)
            {
                if (_default != null && AssetKeyComparer.Default.Equals(this, key)) { value = _default; return true; }
                if (inlines != null) return inlines.TryGetValue(key, out value);
                value = null;
                return false;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
            {
                if (_default != null) yield return new KeyValuePair<IAssetKey, string>(this, _default);
                if (inlines != null) foreach (var line in inlines) yield return line;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                if (_default != null) yield return new KeyValuePair<IAssetKey, string>(this, _default);
                if (inlines != null) foreach (var line in inlines) yield return line;
            }
        }

        /// <summary>
        /// Append format args key part.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        ILocalizationKeyFormatArgs ILocalizationKeyFormattable.Format(params object[] args) => new _FormatArgs(this, args);

        /// <summary>
        /// Append format args key part.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual _FormatArgs Format(params object[] args) => new _FormatArgs(this, args);

        /// <summary>
        /// Format arguments key.
        /// </summary>
        [Serializable]
        public class _FormatArgs : LocalizationKey, ILocalizationKeyFormatArgs, IAssetKeyNonCanonicallyCompared
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
            public _FormatArgs(IAssetKey prevKey, Object[] args) : base(prevKey, "") { this.args = args; }

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
        /// Append plurality key part.
        /// </summary>
        /// <param name="argumentIndex"></param>
        /// <param name="pluralityKind"></param>
        /// <returns></returns>
        public _N N(int argumentIndex, string pluralityKind) => new _N(_N.BackUp(this, argumentIndex), argumentIndex, pluralityKind);

        /// <summary>
        /// Append plurality key part.
        /// </summary>
        /// <param name="argumentIndex"></param>
        /// <param name="pluralityKind"></param>
        /// <returns></returns>
        ILocalizationKeyPluralityAssigned ILocalizationKeyPluralityAssignable.N(int argumentIndex, string pluralityKind) => new _N(_N.BackUp(this, argumentIndex), argumentIndex, pluralityKind);

        /// <summary>
        /// Plurality key part.
        /// </summary>
        [Serializable]
        public class _N : LocalizationKey, IAssetKeySectionAssigned, IAssetKeyParameterAssigned, IAssetKeyNonCanonicallyCompared, ILocalizationKeyPluralityAssigned
        {
            /// <summary>
            /// Argument index the plurality applies to.
            /// </summary>
            int argumentIndex;

            /// <summary>
            /// Argument index the plurality applies to.
            /// </summary>
            int ILocalizationKeyPluralityAssigned.ArgumentIndex => argumentIndex;

            /// <summary>
            /// Create plurality key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="argumentIndex"></param>
            /// <param name="pluralityKind"></param>
            public _N(IAssetKey prevKey, int argumentIndex, string pluralityKind) : base(prevKey, pluralityKind)
            {
                if (argumentIndex < 0) throw new ArgumentException(nameof(argumentIndex));
                this.argumentIndex = argumentIndex;
            }

            /// <summary>
            /// Deserialize plurality key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _N(SerializationInfo info, StreamingContext context) : base(info, context) {
                this.argumentIndex = info.GetInt32("ArgumentIndex");
            }

            /// <summary>
            /// Serialize plurality key part.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {                
                info.AddValue("ArgumentIndex", argumentIndex);
                base.GetObjectData(info, context);
            }

            /// <summary>
            /// ParameterName
            /// </summary>
            public virtual String ParameterName => GetParameterName(argumentIndex);

            /// <summary>
            /// Static parameter names for plurality keys.
            /// </summary>
            private static string[] ParameterNames = new string[] { "N", "N1", "N2", "N3", "N4", "N5", "N6", "N7", "N8", "N9" };

            /// <summary>
            /// Get or create parameter name for argument index.
            /// </summary>
            /// <param name="argumentIndex"></param>
            /// <returns></returns>
            public static string GetParameterName(int argumentIndex) => argumentIndex < ParameterNames.Length ? ParameterNames[argumentIndex] : "N" + argumentIndex;

            /// <summary>
            /// Go back in key chain and find Plurality key.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="argumentIndex"></param>
            /// <returns></returns>
            internal static IAssetKey BackUp(IAssetKey key, int argumentIndex)
            {
                string parameterNameToSearch = _N.GetParameterName(argumentIndex);
                IAssetKey result = null;
                for (IAssetKey k = key; k!=null; k=k.GetPreviousKey())
                {
                    if (k.GetParameterName() == parameterNameToSearch) result = k;
                }
                result = result?.GetPreviousKey();
                return result ?? key;
            }
        }

        /// <summary>
        /// Append "Section" key part.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public _Section Section(string sectionName) => new _Section(this, sectionName);

        /// <summary>
        /// Append "Section" key part.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        IAssetKeySectionAssigned IAssetKeySectionAssignable.Section(string sectionName) => new _Section(this, sectionName);

        /// <summary>
        /// Section key part.
        /// </summary>
        [Serializable]
        public class _Section : LocalizationKey, IAssetKeySectionAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            public _Section(IAssetKey prevKey, string name) : base(prevKey, name) { }

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
        public _Type Type(string typename) => new _Type(this, typename);

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
        public _Type<T> Type<T>() => new _Type<T>(this);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(string typename) => new _Type(this, typename);

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
        IAssetKey<T> IAssetKeyTypeAssignable.Type<T>() => new _Type<T>(this);

        static RuntimeConstructor<IAssetKey, _Type> typeConstructor = new RuntimeConstructor<IAssetKey, _Type>(typeof(_Type<>));

        /// <summary>
        /// Type key part.
        /// </summary>
        [Serializable]
        public class _Type : LocalizationKey, IAssetKeyTypeAssigned, IAssetKeyParameterAssigned, IAssetKeyNonCanonicallyCompared
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
            public _Type(IAssetKey prevKey, Type type) : base(prevKey, type.FullName) { this.type = type; }

            /// <summary>
            /// Create Type part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="name"></param>
            public _Type(IAssetKey prevKey, String name) : base(prevKey, name) { this.name = name; }

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
                if (t == null) info.AddValue(nameof(Type), name); 
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
        public class _Type<T> : _Type, IAssetKey<T>/*TypeSectionInterfaces*/
        {
            /// <summary>
            /// Create Type key part.
            /// </summary>
            /// <param name="prevKey"></param>
            public _Type(IAssetKey prevKey) : base(prevKey, typeof(T)) {}

            /// <summary>
            /// Create Type key part.
            /// </summary>
            /// <param name="root"></param>
            public _Type(IAssetRoot root) : base(root, typeof(T)) { }

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
        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(Assembly assembly) => new _Assembly(this, assembly);

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(String assemblyName) => new _Assembly(this, assemblyName);

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public _Assembly Assembly(Assembly assembly) => new _Assembly(this, assembly);

        /// <summary>
        /// Append Assembly key part.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public _Assembly Assembly(String assemblyName) => new _Assembly(this, assemblyName);

        /// <summary>
        /// Assembly key part.
        /// </summary>
        [Serializable]
        public class _Assembly : LocalizationKey, IAssetKeyAssemblyAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            public _Assembly(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }

            /// <summary>
            /// Create Assembly key part.
            /// </summary>
            /// <param name="prevKey"></param>
            /// <param name="assembly"></param>
            public _Assembly(IAssetKey prevKey, Assembly assembly) : base(prevKey, assembly.GetName().Name) { this.assembly = assembly; }

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
        IAssetKeyResourceAssigned IAssetKeyResourceAssignable.Resource(String resourceName) => new _Resource(this, resourceName);

        /// <summary>
        /// Append Resource key part.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public _Resource Resource(String resourceName) => new _Resource(this, resourceName);

        /// <summary>
        /// Resource key part.
        /// </summary>
        [Serializable]
        public class _Resource : LocalizationKey, IAssetKeyResourceAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            public _Resource(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }

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
        IAssetKeyLocationAssigned IAssetKeyLocationAssignable.Location(String resourceName) => new _Location(this, resourceName);

        /// <summary>
        /// Append Location key part.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public _Location Location(String resourceName) => new _Location(this, resourceName);

        /// <summary>
        /// Location key part.
        /// </summary>
        [Serializable]
        public class _Location : LocalizationKey, IAssetKeyLocationAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            public _Location(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }

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
        public LocalizationKey(SerializationInfo info, StreamingContext context)
        {
            this.name = info.GetString(nameof(Name));
            this.prevKey = info.GetValue(nameof(PreviousKey), typeof(IAssetKey)) as IAssetKey;
        }

        /// <summary>
        /// Append Resolver key part.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        ILocalizationKeyResolverAssigned ILocalizationKeyResolverAssignable.Resolver(ILocalizationResolver resolver) => new _Resolver(this, resolver);

        /// <summary>
        /// Append Resolver key part.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public _Resolver Resolver(ILocalizationResolver resolver) => new _Resolver(this, resolver);

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
            public _Resolver(IAssetKey prevKey, ILocalizationResolver resolver) : base(prevKey, "") { this.resolver = resolver; }

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
        ILocalizationKeyFormatProviderAssigned ILocalizationKeyFormatProviderAssignable.FormatProvider(IFormatProvider formatProvider) => new _FormatProvider(this, formatProvider);

        /// <summary>
        /// Append FormatProvider key part.
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public _FormatProvider FormatProvider(IFormatProvider formatProvider) => new _FormatProvider(this, formatProvider);

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
            public _FormatProvider(IAssetKey prevKey, IFormatProvider formatProvider) : base(prevKey, "") { this.formatProvider = formatProvider; }

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
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(PreviousKey), prevKey);
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
            return dynamicMetaObject = new LocalizationKeyDynamicMetaObject(Library.Default, expression, BindingRestrictions.GetTypeRestriction(expression, typeof(IAssetKey)), this);
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
        static IEqualityComparer<IAssetKey> comparer =
            new AssetKeyComparer()
                .AddCanonicalComparer(ParameterComparer.Instance)
                .AddComparer(NonCanonicalComparer.Instance)
                .AddComparer(new LocalizationKeyFormatArgsComparer())
                .SetReadonly();

        /// <summary>
        /// Comparer that compares key reference and <see cref="ILocalizationKeyFormatArgs"/>.
        /// </summary>
        public static IEqualityComparer<IAssetKey> FormatArgsComparer => comparer;

        /// <summary>
        /// Equals comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) 
            => comparer.Equals(this, obj as IAssetKey);

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
        int IAssetKeyDefaultHashCode.GetDefaultHashCode()
        {
            // Return cached default hashcode
            if (defaultHashcodeCalculated) return defaultHashcode;

            // Get previous key's default hashcode
            if (this is IAssetKeyCanonicallyCompared == false && this is IAssetKeyNonCanonicallyCompared == false && this.prevKey is IAssetKeyDefaultHashCode prevDefaultHashcode)
            {
                defaultHashcode = prevDefaultHashcode.GetDefaultHashCode();
            } else
            {
                defaultHashcode = AssetKeyComparer.Default.CalculateHashCode(this);
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
                    .AddExtensionMethods(typeof(AssetKeyExtensions))
                    .AddExtensionMethods(typeof(AssetKeyExtensions_))
                    .AddInterface(typeof(IAssetKey))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeyAssigned))
                    .AddInterface(typeof(IAssetKeyLinked))
                    .AddInterface(typeof(IAssetKeyAssetAssigned))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeySectionAssigned))
                    .AddInterface(typeof(IAssetKeySectionAssignable))
                    .AddInterface(typeof(IAssetKeyLocationAssigned))
                    .AddInterface(typeof(IAssetKeyLocationAssignable))
                    .AddInterface(typeof(IAssetKeyTypeAssigned))
                    .AddInterface(typeof(IAssetKeyTypeAssignable))
                    .AddInterface(typeof(IAssetKeyAssemblyAssigned))
                    .AddInterface(typeof(IAssetKeyAssemblyAssignable))
                    .AddInterface(typeof(IAssetKeyResourceAssigned))
                    .AddInterface(typeof(IAssetKeyResourceAssignable))
                    .AddExtensionMethods(typeof(LocalizationKeyExtensions))
                    .AddInterface(typeof(ILocalizationKeyCultureAssignable))
                    .AddInterface(typeof(ILocalizationKeyCultureAssigned))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssigned))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssignable))
                    .AddInterface(typeof(ILocalizationKeyFormatArgs))
                    .AddInterface(typeof(ILocalizationKeyFormattable))
                    .AddInterface(typeof(ILocalizationKeyInlineAssignable))
                    .AddInterface(typeof(ILocalizationKeyInlines));
        }

    }
}
