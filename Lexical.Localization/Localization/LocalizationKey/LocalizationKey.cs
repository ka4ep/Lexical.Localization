// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Lexical.Asset.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Lexical.Localization
{    
    [Serializable]
    [DebuggerDisplay("{DebugPrint()}")]
    public class LocalizationKey :
#region Interfaces
        ILocalizationKey, IAssetKeyAssignable, ILocalizationKeyInlineAssignable, ILocalizationKeyFormattable, ILocalizationKeyCultureAssignable, IAssetKeyLinked, IAssetKeyTypeSectionAssignable, IAssetKeyAssemblySectionAssignable, IAssetKeyResourceSectionAssignable, IAssetKeyLocationSectionAssignable, IAssetKeySectionAssignable, ISerializable, IDynamicMetaObjectProvider
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

        public virtual String Name => name;
        public virtual IAssetKey PreviousKey => prevKey;
        public string DebugPrint() => AssetKeyNameProvider.Default.BuildName(this);

        public LocalizationKey(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public LocalizationKey(IAssetKey prevKey, string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.prevKey = prevKey;
        }

        IAssetKeyAssigned IAssetKeyAssignable.Key(string subkey) => new _Key(this, subkey);
        public _Key Key(string subkey) => new _Key(this, subkey);
        [Serializable]
        public class _Key : LocalizationKey, IAssetKeyAssigned
        {
            public _Key(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Key(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        ILocalizationKeyCultured ILocalizationKeyCultureAssignable.SetCulture(CultureInfo culture) => new _Cultured(this, culture);
        public _Cultured SetCulture(CultureInfo culture) => new _Cultured(this, culture);
        [Serializable]
        public class _Cultured : LocalizationKey, ILocalizationKeyCultured, IAssetKeyNonCanonicallyCompared
        {
            protected CultureInfo culture;
            public CultureInfo Culture => culture;
            public _Cultured(IAssetKey prevKey, CultureInfo culture) : base(prevKey, culture.Name)
            {
                this.culture = culture;
            }
            public _Cultured(SerializationInfo info, StreamingContext context) : base(info, context) { this.culture = info.GetValue(nameof(Culture), typeof(CultureInfo)) as CultureInfo; }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Culture), culture);
            }
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
        public class _Inlined : LocalizationKey, ILocalizationKeyInlined, IAssetKeyNonCanonicallyCompared
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
        public class _FormatArgs : LocalizationKey, ILocalizationKeyFormatArgs, IAssetKeyNonCanonicallyCompared
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
        public class _Section : LocalizationKey, IAssetKeySectionAssigned
        {
            public _Section(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Section(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        static RuntimeConstructor<IAssetKey, _TypeSection> typeSectionConstructor = new RuntimeConstructor<IAssetKey, _TypeSection>(typeof(_TypeSection<>));
        IAssetKeyTypeSection IAssetKeyTypeSectionAssignable.TypeSection(string typename) => new _TypeSection(this, typename);
        IAssetKeyTypeSection IAssetKeyTypeSectionAssignable.TypeSection(Type t) => typeSectionConstructor.Create(t, this);
        IAssetKey<T> IAssetKeyTypeSectionAssignable.TypeSection<T>() => new _TypeSection<T>(this);
        public _TypeSection TypeSection(string typename) => new _TypeSection(this, typename);
        public _TypeSection TypeSection(Type t) => typeSectionConstructor.Create(t, this);
        public _TypeSection<T> TypeSection<T>() => new _TypeSection<T>(this);
        [Serializable]
        public class _TypeSection : LocalizationKey, IAssetKeyTypeSection
        {
            protected Type type;
            public virtual Type Type => type;
            public _TypeSection(IAssetKey prevKey, Type type) : base(prevKey, type.FullName/*CanonicalName()*/) { this.type = type; }
            public _TypeSection(IAssetKey prevKey, String name) : base(prevKey, name) { this.name = name; }
            public _TypeSection(SerializationInfo info, StreamingContext context) : base(info, context) {
                this.type = info.GetValue(nameof(Type), typeof(Type)) as Type;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var t = Type;
                // .NET Core can't serialize TypeSection<T> if T isn't [Serializable]
                if (t.IsSerializable) info.AddValue(nameof(Type), t);
                base.GetObjectData(info, context);
            }
        }
        [Serializable]
        public class _TypeSection<T> : _TypeSection, IAssetKey<T>/**TypeSectionInterfaces**/
        {
            public _TypeSection(IAssetKey prevKey) : base(prevKey, typeof(T)) {}
            public _TypeSection(IAssetRoot root) : base(root, typeof(T)) { }
            public _TypeSection(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        IAssetKeyAssemblySection IAssetKeyAssemblySectionAssignable.AssemblySection(Assembly assembly) => new _AssemblySection(this, assembly);
        IAssetKeyAssemblySection IAssetKeyAssemblySectionAssignable.AssemblySection(String assemblyName) => new _AssemblySection(this, assemblyName);
        public _AssemblySection AssemblySection(Assembly assembly) => new _AssemblySection(this, assembly);
        public _AssemblySection AssemblySection(String assemblyName) => new _AssemblySection(this, assemblyName);
        [Serializable]
        public class _AssemblySection : LocalizationKey, IAssetKeyAssemblySection, IAssetKeyNonCanonicallyCompared
        {
            protected Assembly assembly;
            public virtual Assembly Assembly => assembly;
            public _AssemblySection(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _AssemblySection(IAssetKey prevKey, Assembly assembly) : base(prevKey, assembly.GetName().Name) { this.assembly = assembly; }
            public _AssemblySection(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.assembly = info.GetValue(nameof(Assembly), typeof(Assembly)) as Assembly;
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                var a = Assembly;
                info.AddValue(nameof(Assembly), a);
                base.GetObjectData(info, context);
            }
        }

        IAssetKeyResourceSection IAssetKeyResourceSectionAssignable.ResourceSection(String resourceName) => new _ResourceSection(this, resourceName);
        public _ResourceSection ResourceSection(String resourceName) => new _ResourceSection(this, resourceName);
        [Serializable]
        public class _ResourceSection : LocalizationKey, IAssetKeyResourceSection
        {
            public _ResourceSection(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _ResourceSection(SerializationInfo info, StreamingContext context) : base(info, context) {}
        }

        IAssetKeyLocationSection IAssetKeyLocationSectionAssignable.Location(String resourceName) => new _LocationSection(this, resourceName);
        public _LocationSection Location(String resourceName) => new _LocationSection(this, resourceName);
        [Serializable]
        public class _LocationSection : LocalizationKey, IAssetKeyLocationSection
        {
            public _LocationSection(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _LocationSection(SerializationInfo info, StreamingContext context) : base(info, context) { }
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
                .AddCanonicalParametrizerComparer(AssetKeyParametrizer.Singleton)
                .AddNonCanonicalParametrizerComparer(AssetKeyParametrizer.Singleton)
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
        #endregion Code

        /// <summary>
        /// A library of interfaces and extension methods that DynamicMetaObject implementation seaches from when 
        /// invoked with dynamic calls.
        /// </summary>
        public static class Library
        {
            private static DynamicObjectLibrary instance = CreateDefault();
            public static DynamicObjectLibrary Default => instance;

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
                    .AddInterface(typeof(IAssetKeyLocationSection))
                    .AddInterface(typeof(IAssetKeyLocationSectionAssignable))
                    .AddInterface(typeof(IAssetKeyTypeSection))
                    .AddInterface(typeof(IAssetKeyTypeSectionAssignable))
                    .AddInterface(typeof(IAssetKeyAssemblySection))
                    .AddInterface(typeof(IAssetKeyAssemblySectionAssignable))
                    .AddInterface(typeof(IAssetKeyResourceSection))
                    .AddInterface(typeof(IAssetKeyResourceSectionAssignable))
                    .AddExtensionMethods(typeof(LocalizationKeyExtensions))
                    .AddInterface(typeof(ILocalizationKeyCultureAssignable))
                    .AddInterface(typeof(ILocalizationKeyCultured))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicy))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssignable))
                    .AddInterface(typeof(ILocalizationKeyFormatArgs))
                    .AddInterface(typeof(ILocalizationKeyFormattable))
                    .AddInterface(typeof(ILocalizationKeyInlineAssignable))
                    .AddInterface(typeof(ILocalizationKeyInlined));
        }

    }
}
