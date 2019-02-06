// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lexical.Localization
{
    /// <summary>
    /// This class reads parameters from <see cref="IAssetKey"/> by using two interface annotations:
    ///  <see cref="AssetKeyParameterAttribute"/> that describes that key link represents a parameter name.
    ///  <see cref="AssetKeyConstructorAttribute"/> that describes that method constructs new key of specific parameter.
    /// </summary>
    public class AssetKeyParametrizer : IAssetKeyParametrizer
    {
        private static readonly AssetKeyParametrizer instance = new AssetKeyParametrizer();
        public static AssetKeyParametrizer Singleton => instance;

        ConcurrentDictionary<Type, TypeInfo> map = new ConcurrentDictionary<Type, TypeInfo>();

        TypeInfo getOrCreateTypeInfo(Type type)
        {
            TypeInfo typeInfo;
            if (map.TryGetValue(type, out typeInfo)) return typeInfo;
            typeInfo = TypeInfo.ReadAttributes(type);
            map.TryAdd(type, typeInfo);
            return typeInfo;
        }

        public string[] GetPartParameters(object part)
        {
            IAssetKey key = part as IAssetKey;
            if (key == null) return null;
            TypeInfo typeInfo = getOrCreateTypeInfo(part.GetType());
            return typeInfo.Parameters;
        }

        public string GetPartValue(object key_link, string as_property)
        {
            IAssetKey key = key_link as IAssetKey;
            if (key == null) return null;
            TypeInfo typeInfo = getOrCreateTypeInfo(key_link.GetType());
            if (typeInfo.ParameterSet.Contains(as_property)) return key.Name;
            if (typeInfo.isSection && as_property == "anysection") return key.Name;
            return null;
        }

        public IEnumerable<object> Break(object obj)
            => obj is IAssetKey key ? key.ArrayFromRoot() : null;

        public object GetPreviousPart(object part)
            => part is IAssetKeyLinked linked ? linked.PreviousKey : null;

        public void VisitParts<T>(object obj, ParameterPartVisitor<T> visitor, ref T data)
        {
            IAssetKey key = (obj as IAssetKey);
            if (key == null) return;

            // Push to stack
            IAssetKey prevKey = key.GetPreviousKey();
            if (prevKey != null) VisitParts(prevKey, visitor, ref data);

            // Pop from stack in reverse order
            visitor(key, ref data);
        }

        public object TryCreatePart(object obj, string parameterName, string parameterValue)
        {
            IAssetKey key = (obj as IAssetKey);
            if (key == null) return null;

            Func<object, string, object> func;
            TypeInfo typeInfo = getOrCreateTypeInfo(obj.GetType());

            if (!typeInfo.subKeyConstructors.TryGetValue(parameterName, out func)) return null;

            return func(obj, parameterValue);
        }

        public bool IsCanonical(object part, string parameterName)
            => part is IAssetKey ? part is IAssetKeyNonCanonicallyCompared == false : false;
        public bool IsNonCanonical(object part, string parameterName)
            => part is IAssetKey ? part is IAssetKeyNonCanonicallyCompared : false;

        class TypeInfo
        {
            public Type Type;

            public ISet<string> ParameterSet = new HashSet<string>();
            public string[] Parameters;

            public Dictionary<string, Func<object, string, object>> subKeyConstructors = new Dictionary<string, Func<object, string, object>>();

            /// <summary>
            /// Does type represent "anysection".
            /// </summary>
            public bool isSection;

            public static TypeInfo ReadAttributes(Type type)
            {
                TypeInfo result = new TypeInfo { Type = type };

                // Iterate classes and interfaces
                foreach(Type t in GetAllBaseTypes(type).Concat(type.GetInterfaces()))
                {
                    // Find parameters
                    foreach (object param in t.GetCustomAttributes(typeof(AssetKeyParameterAttribute), false))
                    {
                        if (param is AssetKeyParameterAttribute casted) result.ParameterSet.Add(casted.ParameterName);
                    }

                    // Find constructors
                    foreach(MethodInfo mi in t.GetMethods())
                    {
                        object[] constructorAttributes = mi.GetCustomAttributes(typeof(AssetKeyConstructorAttribute), false);
                        if (constructorAttributes == null || constructorAttributes.Length == 0) continue;
                        ParameterInfo[] pis = mi.GetParameters();
                        if (pis.Length != 1 || pis[0].ParameterType != typeof(string)) continue;
                        foreach (object param in constructorAttributes)
                        {
                            if (param is AssetKeyConstructorAttribute casted)
                            {
                                ParameterExpression paramKeyExp = Expression.Parameter(typeof(object), "key");
                                ParameterExpression paramNameExp = Expression.Parameter(typeof(string), "name");
                                Expression bodyExp = Expression.Convert(Expression.Call(Expression.Convert(paramKeyExp, mi.ReflectedType), mi, paramNameExp), typeof(object));
                                Expression<Func<object, string, object>> exp = Expression.Lambda<Func<object, string, object>>(bodyExp, paramKeyExp, paramNameExp);
                                Func<object, string, object> func = exp.Compile();
                                result.subKeyConstructors[casted.ParameterName] = func;
                                break;
                            }
                        }
                    }
                }

                result.Parameters = result.ParameterSet.ToArray();
                Array.Sort(result.Parameters, StringComparer.InvariantCulture);
                foreach (string x in result.Parameters)
                {
                    result.isSection |= x == "type" || x == "section" || x == "location" || x == "resource" || x == "assembly";
                    //result.subKeyConstructors[x] = null;
                }
                return result;
            }

            /// <summary>
            /// Return type itself and its anchestors.
            /// </summary>
            /// <returns></returns>
            static IEnumerable<Type> GetAllBaseTypes(Type type)
            {
                while (type != null)
                {
                    yield return type;
                    type = type.BaseType;
                }
            }

            public bool HasParameter(string parameterName)
            {
                if (parameterName == "anysection") return isSection;
                else return subKeyConstructors.ContainsKey(parameterName);
            }
        }
    }

    public static class AssetKeyParameterKeyExtensions
    {
        /// <summary>
        /// Matches parameters of <see cref="IAssetKey"/> against <see cref="IAssetNamePattern"/>.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IAssetNamePatternMatch Match(this IAssetNamePattern pattern, IAssetKey key)
            => pattern.Match(key, AssetKeyParametrizer.Singleton);

        /// <summary>
        /// Matches parameters of <see cref="IAssetKey"/> against <see cref="IAssetNamePattern"/> 
        /// and converts to string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="obj"></param>
        /// <param name="parameterReader"></param>
        /// <returns>match as string or null</returns>
        public static string MatchToString(this IAssetNamePattern pattern, IAssetKey key)
            => pattern.MatchToString(key, AssetKeyParametrizer.Singleton);
    }

}
