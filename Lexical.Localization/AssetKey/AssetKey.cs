// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Key for binary asset localizations.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebugPrint()}")]
    public class AssetKey :
#region Interfaces
        IAssetKey, IAssetKeyLinked, IAssetKeyTypeAssignable, IAssetKeyAssemblyAssignable, IAssetKeyResourceAssignable, IAssetKeyLocationAssignable, IAssetKeySectionAssignable, IAssetKeyAssignable, IAssetKeyParameterAssignable, ISerializable, IDynamicMetaObjectProvider, IAssetKeyDefaultHashCode
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
        /// Create new asset key.
        /// </summary>
        /// <param name="name"></param>
        public AssetKey(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Create new asset key.
        /// </summary>
        /// <param name="prevKey"></param>
        /// <param name="name"></param>
        public AssetKey(IAssetKey prevKey, string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.prevKey = prevKey;
        }

        /// <summary>
        /// Debug print that prints the reference id of this key.
        /// </summary>
        /// <returns></returns>
        public string DebugPrint() => ParameterNamePolicy.Instance.BuildName(this); //AssetKeyNameProvider.Default.BuildName(this);

        /// <summary>
        /// Append Key part.
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        IAssetKeyAssigned IAssetKeyAssignable.Key(string subkey) => new _Key(this, subkey);

        /// <summary>
        /// Append Key part.
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        public virtual _Key Key(string subkey) => new _Key(this, subkey);

        /// <summary>
        /// Key part.
        /// </summary>
        [Serializable]
        public class _Key : AssetKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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

        /// <summary>
        /// Key for a parameterName that wasn't hard coded.
        /// </summary>
        [Serializable]
        public class _Parametrized : LocalizationKey, IAssetKeyAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            public _Parametrized(IAssetKey prevKey, string parameterName, string parameterValue) : base(prevKey, parameterValue)
            {
                this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            }

            /// <summary>
            /// Deserialize a parameter key.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public _Parametrized(SerializationInfo info, StreamingContext context) : base(info, context)
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
        /// Append "Section" key part.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        IAssetKeySectionAssigned IAssetKeySectionAssignable.Section(string sectionName) => new _Section(this, sectionName);

        /// <summary>
        /// Section key part.
        /// </summary>
        [Serializable]
        public class _Section : AssetKey, IAssetKeySectionAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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

        static RuntimeConstructor<IAssetKey, _Type> typeSectionConstructor = new RuntimeConstructor<IAssetKey, _Type>(typeof(_Type<>));

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
        IAssetKeyTypeAssigned IAssetKeyTypeAssignable.Type(Type t) => typeSectionConstructor.Create(t, this);

        /// <summary>
        /// Append Type key part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IAssetKey<T> IAssetKeyTypeAssignable.Type<T>() => new _Type<T>(this);

        /// <summary>
        /// Type key part.
        /// </summary>
        [Serializable]
        public class _Type : AssetKey, IAssetKeyTypeAssigned, IAssetKeyParameterAssigned, IAssetKeyNonCanonicallyCompared
        {
            /// <summary>
            /// ParameterName
            /// </summary>
            public String ParameterName => "Type";

            /// <summary>
            /// Refered Type, or null if type was not available at construction time.
            /// </summary>
            protected Type type;

            /// <summary>
            /// Refered Type, or null if type was not available at construction time.
            /// </summary>
            public virtual Type Type => type;

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
                var t = Type;
                // .NET Core can't serialize Type<T> if T isn't [Serializable]
                if (t.IsSerializable) info.AddValue(nameof(Type), t);
                base.GetObjectData(info, context);
            }
        }

        /// <summary>
        /// Type key part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
        public class _Type<T> : _Type, IAssetKey<T>
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
            /// Create Type key part.
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
        /// Assembly key part.
        /// </summary>
        [Serializable]
        public class _Assembly : AssetKey, IAssetKeyAssemblyAssigned, IAssetKeyNonCanonicallyCompared, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
        {
            /// <summary>
            /// Referred Assembly, or null if was not available.
            /// </summary>
            protected Assembly assembly;

            /// <summary>
            /// Referred Assembly, or null if was not available.
            /// </summary>
            public virtual Assembly Assembly => assembly;

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
                var a = Assembly;
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
        /// Resource key part.
        /// </summary>
        [Serializable]
        public class _Resource : AssetKey, IAssetKeyResourceAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
        /// Location key part.
        /// </summary>
        [Serializable]
        public class _Location : AssetKey, IAssetKeyLocationAssigned, IAssetKeyParameterAssigned, IAssetKeyCanonicallyCompared
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
            /// Create Location key part.
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
        bool hashcodeCalculated;

        /// <summary>
        /// Equals comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => AssetKeyComparer.Default.Equals(this, obj as IAssetKey);

        /// <summary>
        /// Calculate hashcode. Result is cached.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Return cached default hashcode
            if (hashcodeCalculated) return hashcode;

            // Get previous key's default hashcode
            if (this is IAssetKeyCanonicallyCompared == false && this is IAssetKeyNonCanonicallyCompared == false && this.prevKey is IAssetKeyDefaultHashCode prevDefaultHashcode)
            {
                hashcode = prevDefaultHashcode.GetDefaultHashCode();
            }
            else
            {
                hashcode = AssetKeyComparer.Default.CalculateHashCode(this);
            }

            // Mark calculated
            Thread.MemoryBarrier();
            hashcodeCalculated = true;
            return hashcode;
        }

        /// <summary>
        /// Get key reference hash-code.
        /// </summary>
        /// <returns></returns>
        int IAssetKeyDefaultHashCode.GetDefaultHashCode() => GetHashCode();
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
