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
    public class StringLocalizerKey :  
        ILocalizationKey, IAssetKeyAssignable, ILocalizationKeyInlineAssignable, ILocalizationKeyFormattable, ILocalizationKeyCultureAssignable, IAssetKeyLinked, IAssetKeyTypeAssignable, IAssetKeyAssemblyAssignable, IAssetKeyResourceAssignable, IAssetKeyLocationAssignable, IAssetKeySectionAssignable, IAssetKeyParameterAssignable, ILocalizationKeyPluralityAssignable, ISerializable, IDynamicMetaObjectProvider, IAssetKeyDefaultHashCode,
        IStringLocalizer, IStringLocalizerFactory
    {
        /// <summary>
        /// Local name of this key.
        /// </summary>
        protected string name;

        /// <summary>
        /// (optional) Link to previous key.
        /// </summary>
        protected IAssetKey prevKey;

        public virtual String Name => name;
        public virtual IAssetKey PreviousKey => prevKey;
        public string DebugPrint() => ParameterNamePolicy.Instance.BuildName(this); // AssetKeyNameProvider.Default.BuildName(this);

        public StringLocalizerKey(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public StringLocalizerKey(IAssetKey prevKey, string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.prevKey = prevKey;
        }

        IAssetKeyAssigned IAssetKeyAssignable.Key(string subkey) => new _Key(this, subkey);
        public _Key Key(string subkey) => new _Key(this, subkey);
        [Serializable]
        public class _Key : StringLocalizerKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            public _Key(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Key(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public String ParameterName => "Key";
        }

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
        [Serializable]
        public class _Parameter : StringLocalizerKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            string parameterName;
            public _Parameter(IAssetKey prevKey, string parameterName, string parameterValue) : base(prevKey, parameterValue)
            {
                this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            }
            public _Parameter(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.parameterName = info.GetValue(nameof(ParameterName), typeof(string)) as string;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(ParameterName), parameterName);
            }
            public String ParameterName => parameterName;
        }

        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.Culture(CultureInfo culture) => new _Culture(this, null, culture);
        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.Culture(string cultureName) => new _Culture(this, cultureName, null);
        public _Culture Culture(CultureInfo culture) => new _Culture(this, null, culture);
        public _Culture Culture(string cultureName) => new _Culture(this, cultureName, null);
        [Serializable]
        public class _Culture : StringLocalizerKey, ILocalizationKeyCultureAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned
        {
            protected CultureInfo culture;
            CultureInfo ILocalizationKeyCultureAssigned.Culture => culture;
            public _Culture(IAssetKey prevKey, string cultureName, CultureInfo culture) : base(prevKey, cultureName ?? culture.Name)
            {
                try
                {
                    this.culture = culture ?? CultureInfo.GetCultureInfo(cultureName);
                }
                catch (Exception) { }
            }
            public _Culture(SerializationInfo info, StreamingContext context) : base(info, context) { this.culture = info.GetValue(nameof(Culture), typeof(CultureInfo)) as CultureInfo; }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Culture), culture);
            }
            public String ParameterName => "Culture";
        }

        ILocalizationKeyInlines ILocalizationKeyInlineAssignable.AddInlines() => _addinlines();
        public _Inlines AddInlines() => _addinlines();
        protected virtual _Inlines _addinlines() => new _Inlines(this);

        /// <summary>
        /// Key that contains inlines. 
        /// 
        /// The default value is contained in a field <see cref="_default"/>.
        /// Others are allocated dynamically with a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        [Serializable]
        public class _Inlines : StringLocalizerKey, ILocalizationKeyInlines, IAssetKeyNonCanonicallyCompared
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

        ILocalizationKeyFormatArgs ILocalizationKeyFormattable.Format(params object[] args) => new _FormatArgs(this, args);
        public virtual _FormatArgs Format(params object[] args) => new _FormatArgs(this, args);
        [Serializable]
        public class _FormatArgs : StringLocalizerKey, ILocalizationKeyFormatArgs, IAssetKeyNonCanonicallyCompared
        {
            protected object[] args;
            public virtual object[] Args => args;
            public _FormatArgs(IAssetKey prevKey, Object[] args) : base(prevKey, "") { this.args = args; }
            public _FormatArgs(SerializationInfo info, StreamingContext context) : base(info, context) { this.args = info.GetValue(nameof(Args), typeof(Object[])) as object[]; }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Args), Args);
            }
        }

        ILocalizationKeyPluralityAssigned ILocalizationKeyPluralityAssignable.N(int argumentIndex, string pluralityKind) => new _N(_N.BackUp(this, argumentIndex), argumentIndex, pluralityKind);
        public _N N(int argumentIndex, string pluralityKind) => new _N(_N.BackUp(this, argumentIndex), argumentIndex, pluralityKind);
        [Serializable]
        public class _N : StringLocalizerKey, IAssetKeySectionAssigned, IAssetKeyParameterAssigned, IAssetKeyNonCanonicallyCompared, ILocalizationKeyPluralityAssigned
        {
            int argumentIndex;
            int ILocalizationKeyPluralityAssigned.ArgumentIndex => argumentIndex;
            public _N(IAssetKey prevKey, int argumentIndex, string pluralityKind) : base(prevKey, pluralityKind)
            {
                if (argumentIndex < 0) throw new ArgumentException(nameof(argumentIndex));
                this.argumentIndex = argumentIndex;
            }
            public _N(SerializationInfo info, StreamingContext context) : base(info, context) {
                this.argumentIndex = info.GetInt32("ArgumentIndex");
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {                
                info.AddValue("ArgumentIndex", argumentIndex);
                base.GetObjectData(info, context);
            }
            public virtual String ParameterName => GetParameterName(argumentIndex);
            private static string[] ParameterNames = new string[] { "N", "N1", "N2", "N3", "N4", "N5", "N6", "N7", "N8", "N9" };
            public static string GetParameterName(int argumentIndex) => argumentIndex < ParameterNames.Length ? ParameterNames[argumentIndex] : "N" + argumentIndex;
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

        IAssetKeySectionAssigned IAssetKeySectionAssignable.Section(string sectionName) => new _Section(this, sectionName);
        public _Section Section(string sectionName) => new _Section(this, sectionName);
        [Serializable]
        public class _Section : StringLocalizerKey, IAssetKeySectionAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            public _Section(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Section(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public virtual String ParameterName => "Section";
        }

        static RuntimeConstructor<IAssetKey, _Type> typeConstructor = new RuntimeConstructor<IAssetKey, _Type>(typeof(_Type<>));
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(string typename) => new _Type(this, typename);
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(Type t) => typeConstructor.Create(t, this);
        IAssetKey<T> IAssetKeyTypeAssignable.Type<T>() => new _Type<T>(this);
        public _Type Type(string typename) => new _Type(this, typename);
        public _Type Type(Type t) => typeConstructor.Create(t, this);
        public _Type<T> Type<T>() => new _Type<T>(this);
        [Serializable]
        public class _Type : StringLocalizerKey, IAssetKeyTypeAssigned, IAssetKeyParameterAssigned, IAssetKeyNonCanonicallyCompared
        {
            protected Type type;
            Type IAssetKeyTypeAssigned.Type => type;
            public _Type(IAssetKey prevKey, Type type) : base(prevKey, type.FullName) { this.type = type; }
            public _Type(IAssetKey prevKey, String name) : base(prevKey, name) { this.name = name; }
            public _Type(SerializationInfo info, StreamingContext context) : base(info, context) {
                this.type = info.GetValue(nameof(Type), typeof(Type)) as Type;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var t = type;
                // .NET Core can't serialize Type<T> if T isn't [Serializable]
                if (t == null) info.AddValue(nameof(Type), name); 
                else if (t.IsSerializable) info.AddValue(nameof(Type), t);
                base.GetObjectData(info, context);
            }
            public String ParameterName => "Type";
        }
        [Serializable]
        public class _Type<T> : _Type, IAssetKey<T>, IStringLocalizer<T>
        {
            public _Type(IAssetKey prevKey) : base(prevKey, typeof(T)) {}
            public _Type(IAssetRoot root) : base(root, typeof(T)) { }
            public _Type(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(Assembly assembly) => new _Assembly(this, assembly);
        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(String assemblyName) => new _Assembly(this, assemblyName);
        public _Assembly Assembly(Assembly assembly) => new _Assembly(this, assembly);
        public _Assembly Assembly(String assemblyName) => new _Assembly(this, assemblyName);
        [Serializable]
        public class _Assembly : StringLocalizerKey, IAssetKeyAssemblyAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            protected Assembly assembly;
            Assembly IAssetKeyAssemblyAssigned.Assembly => assembly;
            public _Assembly(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _Assembly(IAssetKey prevKey, Assembly assembly) : base(prevKey, assembly.GetName().Name) { this.assembly = assembly; }
            public _Assembly(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.assembly = info.GetValue(nameof(Assembly), typeof(Assembly)) as Assembly;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var a = assembly;
                info.AddValue(nameof(Assembly), a);
                base.GetObjectData(info, context);
            }
            public String ParameterName => "Assembly";
        }

        IAssetKeyResourceAssigned IAssetKeyResourceAssignable.Resource(String resourceName) => new _Resource(this, resourceName);
        public _Resource Resource(String resourceName) => new _Resource(this, resourceName);
        [Serializable]
        public class _Resource : StringLocalizerKey, IAssetKeyResourceAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            public _Resource(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _Resource(SerializationInfo info, StreamingContext context) : base(info, context) {}
            public String ParameterName => "Resource";
        }

        IAssetKeyLocationAssigned IAssetKeyLocationAssignable.Location(String resourceName) => new _Location(this, resourceName);
        public _Location Location(String resourceName) => new _Location(this, resourceName);
        [Serializable]
        public class _Location : StringLocalizerKey, IAssetKeyLocationAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            public _Location(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _Location(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public String ParameterName => "Location";
        }

        /// <summary>
        /// Deserialize from stream.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerKey(SerializationInfo info, StreamingContext context)
        {
            this.name = info.GetString(nameof(Name));
            this.prevKey = info.GetValue(nameof(PreviousKey), typeof(IAssetKey)) as IAssetKey;
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
            => this.ResolveFormulatedString() ?? this.DebugPrint();

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
        /// Hashcode calculation
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (hashcodeCalculated) return hashcode;
            hashcode = comparer.GetHashCode(this);
            hashcodeCalculated = true;
            return hashcode;
        }

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

        public IStringLocalizer Create(string basename, string location) => new _Resource(new _Assembly(this, location), basename);
        public IStringLocalizer Create(Type type) => typeConstructor.Create(type, this);
        public IStringLocalizer WithCulture(CultureInfo newCulture)
        {
            // Find culture key
            IAssetKey oldCultureKey = this.FindCultureKey();
            // No culture key, create new
            if (oldCultureKey == null) return newCulture == null ? this : new _Culture(this, null, newCulture);
            // Old culture matches the new, return as is
            if (oldCultureKey?.Name == newCulture?.Name) return this;

            // Replace culture
            IAssetKey beforeCultureKey = oldCultureKey?.GetPreviousKey();
            if (beforeCultureKey == null) throw new InvalidOperationException("Cannot change culture when culture is the root key.");
            // Read parameters
            List<(string, string)> parameters = new List<(string, string)>();
            for (IAssetKey k = this; k != oldCultureKey; k = k.GetPreviousKey())
                if (k is IAssetKeyParameterAssigned parameterKey && parameterKey.ParameterName != null)
                    parameters.Add((parameterKey.ParameterName, parameterKey.Name));
            // Assign new culture
            IAssetKey result = newCulture == null ? beforeCultureKey : beforeCultureKey.Culture(newCulture);
            // Apply parameters
            for (int i = parameters.Count - 1; i >= 0; i--)
                result = result.AppendParameter(parameters[i].Item1, parameters[i].Item2);
            return (IStringLocalizer)result;
        }
        public LocalizedString this[string name] 
        { 
            get {
                ILocalizationKey key = this.Key(name);
                string printedString = key.ResolveFormulatedString();
                if (printedString == null)
                    return new LocalizedString(name, key.BuildName(), true);
                else
                    return new LocalizedString(name, printedString);
                }
        }
        public LocalizedString this[string name, params object[] arguments] 
        { 
            get {
                ILocalizationKey key = this.Key(name).Format(arguments);
                string printedString = key.ResolveFormulatedString();
                if (printedString == null)
                    return new LocalizedString(name, key.BuildName(), true);
                else
                    return new LocalizedString(name, printedString);
                }
        }
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
                    IEnumerable<KeyValuePair<string, string>> strs = collections?.GetAllStringLines(this);
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
                IEnumerable<KeyValuePair<string, string>> strs = collections?.GetAllStringLines(this);
                return strs == null ? null : ConvertStrings(strs);
            }
        }
        IEnumerable<LocalizedString> ConvertStrings(IEnumerable<KeyValuePair<string, string>> lines)
        {
            foreach(var kp in lines)
            {
                string value = kp.Value; // <- What kind of value is expected? Is formulation expected?
                yield return new LocalizedString(kp.Key, value);
            }
        }

        public static class Library
        {
            private static Lazy<DynamicObjectLibrary> lazy = new Lazy<DynamicObjectLibrary>(CreateDefault);
            public static DynamicObjectLibrary Default => lazy.Value;

            public static DynamicObjectLibrary CreateDefault()
                => LocalizationKey.Library.CreateDefault();
        }
    }

}
