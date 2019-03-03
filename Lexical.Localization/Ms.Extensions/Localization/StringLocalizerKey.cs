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
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lexical.Localization;
using Lexical.Localization.Internal;
using Microsoft.Extensions.Localization;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// StringLocalizerKey that implements and is assignable to following interfaces:
    ///     <see cref="IStringLocalizer"/>
    ///     <see cref="IStringLocalizerFactory"/>
    ///     <see cref="ILocalizationKey"/>
    /// </summary>
    [DebuggerDisplay("{DebugPrint()}")]
    [Serializable]
    public class StringLocalizerKey :  
        ILocalizationKey, IAssetKeyAssignable, ILocalizationKeyInlineAssignable, ILocalizationKeyFormattable, ILocalizationKeyCultureAssignable, IAssetKeyLinked, IAssetKeyTypeAssignable, IAssetKeyAssemblyAssignable, IAssetKeyResourceAssignable, IAssetKeyLocationAssignable, IAssetKeySectionAssignable, IAssetKeyParameterAssignable, ISerializable, IDynamicMetaObjectProvider,
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

        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.SetCulture(CultureInfo culture) => new _Culture(this, null, culture);
        ILocalizationKeyCultureAssigned ILocalizationKeyCultureAssignable.SetCulture(string cultureName) => new _Culture(this, cultureName, null);
        public _Culture SetCulture(CultureInfo culture) => new _Culture(this, null, culture);
        public _Culture SetCulture(string cultureName) => new _Culture(this, cultureName, null);
        [Serializable]
        public class _Culture : StringLocalizerKey, ILocalizationKeyCultureAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned
        {
            protected CultureInfo culture;
            public CultureInfo Culture => culture;
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

        ILocalizationKeyInlined ILocalizationKeyInlineAssignable.Inline(string culture, string text) => _inline(culture, text);
        public virtual _Inlined Inline(string culture, string text) => _inline(culture, text);
        protected virtual _Inlined _inline(string culture, string text)
        {
            var inlines = new Dictionary<string, string>(3);
            if (text == null) inlines.Remove(culture); else inlines[culture] = text;
            return new _Inlined(this, inlines);
        }
        [Serializable]
        public class _Inlined : StringLocalizerKey, ILocalizationKeyInlined, IAssetKeyNonCanonicallyCompared
        {
            protected IDictionary<string, string> inlines;
            public virtual IDictionary<string, string> Inlines => inlines;
            public _Inlined(IAssetKey prevKey, IDictionary<string, string> inlines) : base(prevKey, "") { this.inlines = inlines; }
            public _Inlined(SerializationInfo info, StreamingContext context) : base(info, context) { this.inlines = info.GetValue(nameof(Inlines), typeof(IDictionary<string, string>)) as IDictionary<string, string>; }
            protected override _Inlined _inline(string culture, string text)
            {
                if (inlines == null) inlines = new Dictionary<string, string>();
                if (text == null) inlines.Remove(culture); else inlines[culture] = text;
                return this;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Inlines), Inlines);
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

        IAssetKeySectionAssigned IAssetKeySectionAssignable.Section(string sectionName) => new _Section(this, sectionName);
        public _Section Section(string sectionName) => new _Section(this, sectionName);
        [Serializable]
        public class _Section : StringLocalizerKey, IAssetKeySectionAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            public _Section(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Section(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public virtual String ParameterName => "Section";
        }

        static RuntimeConstructor<IAssetKey, _Type> typeSectionConstructor = new RuntimeConstructor<IAssetKey, _Type>(typeof(_TypeSection<>));
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(string typename) => new _Type(this, typename);
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(Type t) => typeSectionConstructor.Create(t, this);
        IAssetKey<T> IAssetKeyTypeAssignable.Type<T>() => new _TypeSection<T>(this);
        public _Type TypeSection(string typename) => new _Type(this, typename);
        public _Type TypeSection(Type t) => typeSectionConstructor.Create(t, this);
        public _TypeSection<T> TypeSection<T>() => new _TypeSection<T>(this);
        [Serializable]
        public class _Type : StringLocalizerKey, IAssetKeyTypeAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            protected Type type;
            public virtual Type Type => type;
            public _Type(IAssetKey prevKey, Type type) : base(prevKey, type.FullName) { this.type = type; }
            public _Type(IAssetKey prevKey, String name) : base(prevKey, name) { this.name = name; }
            public _Type(SerializationInfo info, StreamingContext context) : base(info, context) {
                this.type = info.GetValue(nameof(Type), typeof(Type)) as Type;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var t = Type;
                // .NET Core can't serialize TypeSection<T> if T isn't [Serializable]
                if (t == null) info.AddValue(nameof(Type), name); 
                else if (t.IsSerializable) info.AddValue(nameof(Type), t);
                base.GetObjectData(info, context);
            }
            public String ParameterName => "Type";
        }
        [Serializable]
        public class _TypeSection<T> : _Type, IAssetKey<T>, IStringLocalizer<T>
        {
            public _TypeSection(IAssetKey prevKey) : base(prevKey, typeof(T)) {}
            public _TypeSection(IAssetRoot root) : base(root, typeof(T)) { }
            public _TypeSection(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(Assembly assembly) => new _Assembly(this, assembly);
        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(String assemblyName) => new _Assembly(this, assemblyName);
        public _Assembly AssemblySection(Assembly assembly) => new _Assembly(this, assembly);
        public _Assembly AssemblySection(String assemblyName) => new _Assembly(this, assemblyName);
        [Serializable]
        public class _Assembly : StringLocalizerKey, IAssetKeyAssemblyAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            protected Assembly assembly;
            public virtual Assembly Assembly => assembly;
            public _Assembly(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _Assembly(IAssetKey prevKey, Assembly assembly) : base(prevKey, assembly.GetName().Name) { this.assembly = assembly; }
            public _Assembly(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.assembly = info.GetValue(nameof(Assembly), typeof(Assembly)) as Assembly;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var a = Assembly;
                info.AddValue(nameof(Assembly), a);
                base.GetObjectData(info, context);
            }
            public String ParameterName => "Assembly";
        }

        IAssetKeyResourceAssigned IAssetKeyResourceAssignable.Resource(String resourceName) => new _Resource(this, resourceName);
        public _Resource ResourceSection(String resourceName) => new _Resource(this, resourceName);
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
        int hashcode = -1;

        /// <summary>
        /// Determines if hashcode is calculated and cached
        /// </summary>
        bool hashcodeCalculated = false;

        /// <summary>
        /// Preferred comparer
        /// </summary>
        static IEqualityComparer<IAssetKey> comparer =
            new AssetKeyComparer()
                .AddCanonicalParametrizedComparer()
                .AddNonCanonicalParametrizedComparer()
                .AddNonCanonicalComparer(new LocalizationKeyFormatArgsComparer());
                //.AddNonCanonicalComparer(new LocalizationKeyCultureComparer()); // <- culture is already compared by parametrizer comparer.

        /// <summary>
        /// Equals comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => comparer.Equals(this, obj as IAssetKey);

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

        public IStringLocalizer Create(string basename, string location) => new _Resource(new _Assembly(this, location), basename);
        public IStringLocalizer Create(Type type) => typeSectionConstructor.Create(type, this);
        public IStringLocalizer WithCulture(CultureInfo culture) => new _Culture(this, null, culture);
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
            ILocalizationStringCollection collections = this.FindAsset() as ILocalizationStringCollection;
            if (collections == null) return null;

            CultureInfo ci = null;
            if (includeParentCultures && ((ci = this.FindCulture()) != null))
            {
                IEnumerable<LocalizedString> result = null;
                while (true)
                {
                    IEnumerable<KeyValuePair<string, string>> strs = collections?.GetAllStrings(this);
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
                IEnumerable<KeyValuePair<string, string>> strs = collections?.GetAllStrings(this);
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
