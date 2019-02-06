using Lexical.Asset;
using Lexical.Asset.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    public class LocalizationKeySerialization
    {
        static LocalizationKeySerialization instance;
        public static LocalizationKeySerialization Instance => instance ?? (instance = new LocalizationKeySerialization());

        IFormatter formatter;

        public LocalizationKeySerialization()
        {
            this.formatter = new BinaryFormatter();
            this.formatter.Binder = new Binder();
        }

        public LocalizationKeySerialization(IFormatter formatter)
        {
            this.formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public byte[] Serialize(IAssetKey key)
        {
            // Serialize
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, key);
            ms.Flush();
            return ms.ToArray();
        }

        public IAssetKey Deserialize(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            return (IAssetKey)formatter.Deserialize(ms);
        }

        /// <summary>
        /// Deserialize into a fake root that reads localization asset and culture policy from another root.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IAssetKey DeserializeTo(IAssetKey root, byte[] data)
            => DeserializeTo(new LocalizationAssetFunc(() => root.FindAsset()), new CulturePolicy().SetSourceFunc(() => root.FindCulturePolicy()), data);

        /// <summary>
        /// Deserialize and assign specific asset and culturepolicy references.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="culturePolicy"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IAssetKey DeserializeTo(IAsset asset, ICulturePolicy culturePolicy, byte[] data)
        {
            // Create formatter
            Dictionary<string, object> args = new Dictionary<string, object>();
            if (asset != null) args[nameof(IAsset)] = asset;
            if (culturePolicy != null) args[nameof(ICulturePolicy)] = culturePolicy;
            StreamingContext ctx = new StreamingContext(StreamingContextStates.Other, args);
            BinaryFormatter formatter = new BinaryFormatter(null, ctx);

            // Deserialize
            MemoryStream ms = new MemoryStream(data);
            return (LocalizationKey) formatter.Deserialize(ms);
        }

        public class Binder : SerializationBinder
        {

            public override Type BindToType(string assemblyName, string typeName)
            {
                CanonicalName canonicalName = CanonicalName.Parse(typeName);

                Type type = canonicalName.BuildType();
                return type ?? throw new SerializationException("Could not find type " + typeName);
            }

            public override void BindToName(Type type, out string assemblyName, out string typeName)
            {
                // [Lexical.Localization, Version=0.5.2018.1010, Culture=neutral, PublicKeyToken=null
                assemblyName = type.Assembly.GetName().Name;
                CanonicalName cn = CanonicalName.FromType(type);
                cn.Visit(theHack);
                typeName = cn.ToString();
            }

            Action<CanonicalName> theHack = cn =>
            {
                if(cn.Name.EndsWith("TypeSection") && cn.Arguments!=null && cn.Arguments.Count>0)
                {
                    cn.Arguments = null;
                }
            };
        }

    }

}
