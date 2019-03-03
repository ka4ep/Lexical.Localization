// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
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
    public class AssetKey :
#region Interfaces
        IAssetKey, IAssetKeyLinked, IAssetKeyTypeSectionAssignable, IAssetKeyAssemblySectionAssignable, IAssetKeyResourceSectionAssignable, IAssetKeyLocationSectionAssignable, IAssetKeySectionAssignable, IAssetKeyAssignable, IAssetKeyParameterAssignable, ISerializable, IDynamicMetaObjectProvider
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

        public AssetKey(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public AssetKey(IAssetKey prevKey, string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.prevKey = prevKey;
        }

        public string DebugPrint() => ParameterNamePolicy.Instance.BuildName(this); //AssetKeyNameProvider.Default.BuildName(this);

        IAssetKeyAssigned IAssetKeyAssignable.Key(string subkey) => new _Key(this, subkey);
        public virtual _Key Key(string subkey) => new _Key(this, subkey);
        [Serializable]
        public class _Key : AssetKey, IAssetKeyAssigned, IAssetKeyParametrized, IAssetKeyCanonicallyCompared
        {
            public _Key(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Key(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public String ParameterName => "Key";
        }

        IAssetKeyParametrized IAssetKeyParameterAssignable.AppendParameter(string parameterName, string parameterValue)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (parameterValue == null) throw new ArgumentNullException(nameof(parameterValue));
            switch (parameterName)
            {
                case "Key": return new _Key(this, parameterValue);
                case "Type": return new _TypeSection(this, parameterValue);
                case "Section": return new _Section(this, parameterValue);
                case "Resource": return new _ResourceSection(this, parameterValue);
                case "Assembly": return new _AssemblySection(this, parameterValue);
                case "Location": return new _LocationSection(this, parameterValue);
                default: return new _Parametrized(this, parameterName, parameterValue);
            }
        }
        [Serializable]
        public class _Parametrized : LocalizationKey, IAssetKeyAssigned, IAssetKeyParametrized, IAssetKeyCanonicallyCompared
        {
            string parameterName;
            public _Parametrized(IAssetKey prevKey, string parameterName, string parameterValue) : base(prevKey, parameterValue)
            {
                this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            }
            public _Parametrized(SerializationInfo info, StreamingContext context) : base(info, context)
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


        IAssetKeySectionAssigned IAssetKeySectionAssignable.Section(string sectionName) => new _Section(this, sectionName);
        [Serializable]
        public class _Section : AssetKey, IAssetKeySectionAssigned, IAssetKeyParametrized, IAssetKeyCanonicallyCompared
        {
            public _Section(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Section(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public virtual String ParameterName => "Section";
        }

        static RuntimeConstructor<IAssetKey, _TypeSection> typeSectionConstructor = new RuntimeConstructor<IAssetKey, _TypeSection>(typeof(_TypeSection<>));
        IAssetKeyTypeSection IAssetKeyTypeSectionAssignable.TypeSection(string typename) => new _TypeSection(this, typename);
        IAssetKeyTypeSection IAssetKeyTypeSectionAssignable.TypeSection(Type t) => typeSectionConstructor.Create(t, this);
        IAssetKey<T> IAssetKeyTypeSectionAssignable.TypeSection<T>() => new _TypeSection<T>(this);
        [Serializable]
        public class _TypeSection : AssetKey, IAssetKeyTypeSection, IAssetKeyParametrized, IAssetKeyCanonicallyCompared
        {
            protected Type type;
            public virtual Type Type => type;
            public _TypeSection(IAssetKey prevKey, Type type) : base(prevKey, type.FullName) { this.type = type; }
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
            public String ParameterName => "Type";
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
        [Serializable]
        public class _AssemblySection : AssetKey, IAssetKeyAssemblySection, IAssetKeyNonCanonicallyCompared, IAssetKeyParametrized, IAssetKeyCanonicallyCompared
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
            public String ParameterName => "Assembly";
        }

        IAssetKeyResourceSection IAssetKeyResourceSectionAssignable.ResourceSection(String resourceName) => new _ResourceSection(this, resourceName);
        [Serializable]
        public class _ResourceSection : AssetKey, IAssetKeyResourceSection, IAssetKeyParametrized, IAssetKeyCanonicallyCompared
        {
            public _ResourceSection(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _ResourceSection(SerializationInfo info, StreamingContext context) : base(info, context) {}
            public String ParameterName => "Resource";
        }

        IAssetKeyLocationSection IAssetKeyLocationSectionAssignable.Location(String resourceName) => new _LocationSection(this, resourceName);
        [Serializable]
        public class _LocationSection : AssetKey, IAssetKeyLocationSection, IAssetKeyParametrized, IAssetKeyCanonicallyCompared
        {
            public _LocationSection(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _LocationSection(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public String ParameterName => "Location";
        }

        /// <summary>
        /// Deserialize from stream.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AssetKey(SerializationInfo info, StreamingContext context)
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
            => AssetKeyNameProvider.Default.BuildName(this);

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
            return dynamicMetaObject = new AssetKeyDynamicMetaObject(Library.Default, expression, BindingRestrictions.GetTypeRestriction(expression, typeof(IAssetKey)), this);
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
                .AddNonCanonicalParametrizedComparer();

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
                    .AddInterface(typeof(IAssetKeyAssetAssignable))
                    .AddInterface(typeof(IAssetKeySection))
                    .AddInterface(typeof(IAssetKeySectionAssignable))
                    .AddInterface(typeof(IAssetKeyLocationSection))
                    .AddInterface(typeof(IAssetKeyLocationSectionAssignable))
                    .AddInterface(typeof(IAssetKeyTypeSection))
                    .AddInterface(typeof(IAssetKeyTypeSectionAssignable))
                    .AddInterface(typeof(IAssetKeyAssemblySection))
                    .AddInterface(typeof(IAssetKeyAssemblySectionAssignable))
                    .AddInterface(typeof(IAssetKeyResourceSection))
                    .AddInterface(typeof(IAssetKeyResourceSectionAssignable));
        }

    }
}
