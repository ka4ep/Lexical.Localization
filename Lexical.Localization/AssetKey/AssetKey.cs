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
        IAssetKey, IAssetKeyLinked, IAssetKeyTypeAssignable, IAssetKeyAssemblyAssignable, IAssetKeyResourceAssignable, IAssetKeyLocationAssignable, IAssetKeySectionAssignable, IAssetKeyAssignable, IAssetKeyParameterAssignable, ISerializable, IDynamicMetaObjectProvider
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
        public class _Key : AssetKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
                case "Type": return new _Type(this, parameterValue);
                case "Section": return new _Section(this, parameterValue);
                case "Resource": return new _Resource(this, parameterValue);
                case "Assembly": return new _Assembly(this, parameterValue);
                case "Location": return new _Location(this, parameterValue);
                default: return new _Parametrized(this, parameterName, parameterValue);
            }
        }
        [Serializable]
        public class _Parametrized : LocalizationKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
        public class _Section : AssetKey, IAssetKeySectionAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            public _Section(IAssetKey prevKey, string name) : base(prevKey, name) { }
            public _Section(SerializationInfo info, StreamingContext context) : base(info, context) { }
            public virtual String ParameterName => "Section";
        }

        static RuntimeConstructor<IAssetKey, _Type> typeSectionConstructor = new RuntimeConstructor<IAssetKey, _Type>(typeof(_Type<>));
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(string typename) => new _Type(this, typename);
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(Type t) => typeSectionConstructor.Create(t, this);
        IAssetKey<T> IAssetKeyTypeAssignable.Type<T>() => new _Type<T>(this);
        [Serializable]
        public class _Type : AssetKey, IAssetKeyTypeAssigned, IAssetKeyParameterAssigned, IAssetKeyNonCanonicallyCompared
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
                // .NET Core can't serialize Type<T> if T isn't [Serializable]
                if (t.IsSerializable) info.AddValue(nameof(Type), t);
                base.GetObjectData(info, context);
            }
            public String ParameterName => "Type";
        }
        [Serializable]
        public class _Type<T> : _Type, IAssetKey<T>/**TypeSectionInterfaces**/
        {
            public _Type(IAssetKey prevKey) : base(prevKey, typeof(T)) {}
            public _Type(IAssetRoot root) : base(root, typeof(T)) { }
            public _Type(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(Assembly assembly) => new _Assembly(this, assembly);
        IAssetKeyAssemblyAssigned IAssetKeyAssemblyAssignable.Assembly(String assemblyName) => new _Assembly(this, assemblyName);
        [Serializable]
        public class _Assembly : AssetKey, IAssetKeyAssemblyAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
        [Serializable]
        public class _Resource : AssetKey, IAssetKeyResourceAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            public _Resource(IAssetKey prevKey, string asmName) : base(prevKey, asmName) { }
            public _Resource(SerializationInfo info, StreamingContext context) : base(info, context) {}
            public String ParameterName => "Resource";
        }

        IAssetKeyLocationAssigned IAssetKeyLocationAssignable.Location(String resourceName) => new _Location(this, resourceName);
        [Serializable]
        public class _Location : AssetKey, IAssetKeyLocationAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
        static IEqualityComparer<IAssetKey> comparer = AssetKeyComparer.Default;

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
                    .AddInterface(typeof(IAssetKeyLocationAssigned))
                    .AddInterface(typeof(IAssetKeyLocationAssignable))
                    .AddInterface(typeof(IAssetKeyTypeAssigned))
                    .AddInterface(typeof(IAssetKeyTypeAssignable))
                    .AddInterface(typeof(IAssetKeyAssemblyAssigned))
                    .AddInterface(typeof(IAssetKeyAssemblyAssignable))
                    .AddInterface(typeof(IAssetKeyResourceAssigned))
                    .AddInterface(typeof(IAssetKeyResourceAssignable));
        }

    }
}
