// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           13.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Collection of interfaces and extension methods.
    /// 
    /// This tool class is used when dynamic meta-object is based on interfaces and extension methods (and classes too).
    /// </summary>
    public class DynamicObjectLibrary
    {
        /// <summary>
        /// Extension method types
        /// </summary>
        public readonly List<Type> exts = new List<Type>();

        /// <summary>
        /// Interface types
        /// </summary>
        public readonly List<Type> intfs = new List<Type>();

        /// <summary>
        /// Class types
        /// </summary>
        public readonly List<Type> classes = new List<Type>();

        /// <summary>
        /// Interface methods
        /// </summary>
        public readonly MapList<string, MethodInfo> intf_methods = new MapList<string, MethodInfo>();

        /// <summary>
        /// Interface properties
        /// </summary>
        public readonly MapList<string, PropertyInfo> intf_properties = new MapList<string, PropertyInfo>();

        /// <summary>
        /// Interface indexers
        /// </summary>
        public readonly List<PropertyInfo> intf_indexers = new List<PropertyInfo>();

        /// <summary>
        /// Extension methods
        /// </summary>
        public readonly MapList<string, MethodInfo> ext_methods = new MapList<string, MethodInfo>();

        /// <summary>
        /// Class fields
        /// </summary>
        public readonly MapList<string, FieldInfo> class_fields = new MapList<string, FieldInfo>();

        /// <summary>
        /// Class methods
        /// </summary>
        public readonly MapList<string, MethodInfo> class_methods = new MapList<string, MethodInfo>();

        /// <summary>
        /// Class properties
        /// </summary>
        public readonly MapList<string, PropertyInfo> class_properties = new MapList<string, PropertyInfo>();

        /// <summary>
        /// Class indexers
        /// </summary>
        public readonly List<PropertyInfo> class_indexers = new List<PropertyInfo>();

        MapList<string, FieldInfo> fields;
        MapList<string, MethodInfo> methods;
        MapList<string, PropertyInfo> properties;
        List<PropertyInfo> indexers;

        /// <summary>
        /// Fields by name
        /// </summary>
        public MapList<string, FieldInfo> Fields => fields ?? (fields = class_fields.ToMapList());

        /// <summary>
        /// Methods by name
        /// </summary>
        public MapList<string, MethodInfo> Methods => methods ?? (methods = class_methods.Concat(intf_methods).Concat(ext_methods).ToMapList());

        /// <summary>
        /// Properties by name
        /// </summary>
        public MapList<string, PropertyInfo> Properties => properties ?? (properties = class_properties.Concat(intf_properties).ToMapList());

        /// <summary>
        /// Indexers
        /// </summary>
        public List<PropertyInfo> Indexers => indexers ?? (indexers = class_indexers.Concat(intf_indexers).ToList());

        /// <summary>
        /// Clear cache
        /// </summary>
        protected void ClearCache()
        {
            fields = null;
            methods = null;
            properties = null;
            indexers = null;
        }

        /// <summary>
        /// Add interface to support
        /// </summary>
        /// <param name="intfType"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddInterface(Type intfType)
        {
            ClearCache();

            intfs.Add(intfType);
            // Methods
            foreach (MethodInfo mi in intfType.GetMethods()) AddInterfaceMethod(mi);
            // Properties
            foreach (PropertyInfo pi in intfType.GetProperties())
            {
                if (pi.GetIndexParameters().Length > 0)
                    // Index
                    AddInterfaceIndex(pi);
                else
                    // Property
                    AddInterfaceProperty(pi);
            }
            return this;
        }

        /// <summary>
        /// Add extension methods to support
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddExtensionMethods(Type type)
        {
            ClearCache();
            exts.Add(type);
            foreach (MethodInfo mi in type.GetMethods())
            {
                if (!mi.IsStatic) continue;
                if (!mi.IsPublic) continue;
                AddExtensionMethod(mi);
            }
            return this;
        }

        /// <summary>
        /// Add class to support
        /// </summary>
        /// <param name="clazz"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddClass(Type clazz)
        {
            ClearCache();
            intfs.Add(clazz);
            // Methods
            foreach (MethodInfo mi in clazz.GetMethods(BindingFlags.Public))
            {
                if (!mi.IsStatic) continue;
                if (!mi.IsPublic) continue;
                AddClassMethod(mi);
            }
            // Properties
            foreach (MemberInfo mi in clazz.GetMembers(BindingFlags.Public))
            {
                if (mi is FieldInfo fi) AddClassField(fi);
                else if (mi is PropertyInfo pi)
                {
                    if (pi.GetIndexParameters().Length > 0)
                        // Index
                        AddClassIndex(pi);
                    else
                        // Property
                        AddClassProperty(pi);
                }
            }
            return this;
        }


        /// <summary>
        /// Add an extension method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddExtensionMethod(MethodInfo info) { ext_methods.Add(info.Name, info); return this; }

        /// <summary>
        /// Add a property
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddInterfaceProperty(PropertyInfo info) { intf_properties.Add(info.Name, info); return this; }

        /// <summary>
        /// Add interface index
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddInterfaceIndex(PropertyInfo info) { intf_indexers.Add(info); return this; }

        /// <summary>
        /// Add interface method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddInterfaceMethod(MethodInfo info) { intf_methods.Add(info.Name, info); return this; }

        /// <summary>
        /// Add class field
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddClassField(FieldInfo info) { class_fields.Add(info.Name, info); return this; }

        /// <summary>
        /// Add class property
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddClassProperty(PropertyInfo info) { class_properties.Add(info.Name, info); return this; }

        /// <summary>
        /// Add class index
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddClassIndex(PropertyInfo info) { class_indexers.Add(info); return this; }

        /// <summary>
        /// Add class method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DynamicObjectLibrary AddClassMethod(MethodInfo info) { class_methods.Add(info.Name, info); return this; }





        // Consumer side //

        /// <summary>
        /// Create member expression to any property that matches in the library.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="dynamicObjectType">The LimitType property or value.GetType() of the dynamic object</param>
        /// <param name="dynamicObjectExpression">the Expression property of the dynamic object</param>
        /// <returns>new DynamicMetaObject or null</returns>
        public virtual DynamicMetaObject BindGetMember(GetMemberBinder binder, Type dynamicObjectType, Expression dynamicObjectExpression)
        {
            // Search library for property
            var pis = Properties.TryGetList(binder.Name);
            if (pis != null && pis.Count > 0)
            {
                foreach (PropertyInfo pi in pis)
                {
                    if (!pi.DeclaringType.IsAssignableFrom(dynamicObjectType)) continue;
                    MethodInfo mi = pi.GetGetMethod();
                    if (mi == null) continue;

                    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(dynamicObjectExpression, dynamicObjectType);
                    Expression selfExp = Expression.Convert(dynamicObjectExpression, pi.DeclaringType);
                    return new DynamicMetaObject(Expression.Call(selfExp, mi), restrictions);
                }

                // Return null
                BindingRestrictions restrictions2 = BindingRestrictions.GetTypeRestriction(dynamicObjectExpression, dynamicObjectType);
                return new DynamicMetaObject(Expression.Constant(null, pis[0].PropertyType), restrictions2);
            }
            return null;
        }

        /// <summary>
        /// Create call expression to any indexer that matches in the library.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="dynamicObjectType">The LimitType property or value.GetType() of the dynamic object</param>
        /// <param name="dynamicObjectExpression">the Expression property of the dynamic object</param>
        /// <returns>new DynamicMetaObject or null</returns>
        public virtual DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] args, Type dynamicObjectType, Expression dynamicObjectExpression)
        {
            Type selfType = dynamicObjectType;
            foreach (PropertyInfo pi in Indexers)
            {
                MethodInfo mi = pi.GetGetMethod();
                if (mi == null) continue;

                ParameterInfo[] pis = mi.GetParameters();

                // Test instance type matches
                if (!mi.DeclaringType.IsAssignableFrom(selfType)) continue;

                // Test if other parameters match arguments
                bool match = true;
                for (int i = 0; i < args.Length; i++)
                {
                    Type piType = pis[i].ParameterType;
                    DynamicMetaObject arg = args[i];
                    bool argTypeMatches =
                        arg.RuntimeType != null && piType.IsAssignableFrom(arg.RuntimeType) |
                        arg.LimitType != null && piType.IsAssignableFrom(arg.LimitType) |
                        arg.HasValue && piType.IsAssignableFrom(arg.Value.GetType()) |
                        arg.Expression != null && piType.IsAssignableFrom(arg.Expression.Type);
                    match &= argTypeMatches;
                    if (!match) break;
                }
                if (!match) continue;

                Expression selfExpression = dynamicObjectExpression;
                if (!selfExpression.Type.Equals(mi.DeclaringType)) selfExpression = Expression.Convert(selfExpression, mi.DeclaringType);
                Expression[] argExps = new Expression[args.Length];
                BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(dynamicObjectExpression, dynamicObjectType);
                for (int i = 0; i < args.Length; i++)
                {
                    argExps[i] =
                        args[i].Expression.Type.Equals(pis[i].ParameterType) ?
                        args[i].Expression :
                        Expression.Convert(args[i].Expression, pis[i].ParameterType);
                    restrictions = restrictions.Merge(BindingRestrictions.GetTypeRestriction(args[i].Expression, pis[i].ParameterType));
                }
                Expression exp =
                    mi.IsStatic ?
                    Expression.Call(mi, argExps) :
                    Expression.Call(selfExpression, mi, argExps);
                //restrictions = restrictions.Merge(BindingRestrictions.GetTypeRestriction(exp, mi.ReturnType));
                return new DynamicMetaObject(exp, restrictions);
            }
            return null;
        }


        /// <summary>
        /// Search library for invokable interface or extension method. 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="dynamicObjectType">The LimitType property or value.GetType() of the dynamic object</param>
        /// <param name="dynamicObjectExpression">the Expression property of the dynamic object</param>
        /// <returns>new DynamicMetaObject or null</returns>
        public virtual DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args, Type dynamicObjectType, Expression dynamicObjectExpression)
        {
            Type selfType = dynamicObjectType;
            // Call method
            var list = Methods.TryGetList(binder.Name);
            if (list != null && list.Count > 0)
            {
                foreach (MethodInfo mi in list)
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    // Offset from argument index to parameter index
                    int offset = mi.IsStatic ? 1 : 0;

                    // Test instance type matches
                    Type methodInstanceType = mi.IsStatic ? pis[0].ParameterType : mi.DeclaringType;
                    if (!methodInstanceType.IsAssignableFrom(selfType)) continue;

                    // Test argument count
                    bool match = false;
                    int ix_parameter_varargs = -1;
                    if (pis.Length == args.Length + offset)
                    {
                        // Test if other parameters match arguments
                        match = true;
                        for (int i = 0; i < args.Length; i++)
                        {
                            Type piType = pis[i + offset].ParameterType;
                            DynamicMetaObject arg = args[i];
                            bool argTypeMatches =
                                arg.RuntimeType != null && piType.IsAssignableFrom(arg.RuntimeType) |
                                arg.LimitType != null && piType.IsAssignableFrom(arg.LimitType) |
                                arg.HasValue && piType.IsAssignableFrom(arg.Value.GetType()) |
                                arg.Expression != null && piType.IsAssignableFrom(arg.Expression.Type);
                            match &= argTypeMatches;
                            if (!match) break;
                        }
                    }

                    // Try matching if last parameter is params object[]
                    if (!match && pis.Length>offset && pis[pis.Length-1].ParameterType.Equals(typeof(object[])))
                    {
                        match = true;
                        ix_parameter_varargs = pis.Length - 1;
                        for (int i = 0; i < args.Length-1; i++)
                        {
                            Type piType = pis[i + offset].ParameterType;
                            DynamicMetaObject arg = args[i];
                            bool argTypeMatches =
                                arg.RuntimeType != null && piType.IsAssignableFrom(arg.RuntimeType) |
                                arg.LimitType != null && piType.IsAssignableFrom(arg.LimitType) |
                                arg.HasValue && piType.IsAssignableFrom(arg.Value.GetType()) |
                                arg.Expression != null && piType.IsAssignableFrom(arg.Expression.Type);
                            match &= argTypeMatches;
                            if (!match) break;
                        }
                    }

                    if (!match) continue;

                    // Test if matches type arguments
                    MethodInfo mi_genericsCasted = mi;
                    if (mi.IsGenericMethod)
                    {
                        Type[] miGenericArguments = mi.GetGenericArguments();
                        IList<Type> callerSuppliedGenericArguments = GetTypeArguments(binder);
                        if (callerSuppliedGenericArguments == null) continue;
                        if (miGenericArguments.Length != callerSuppliedGenericArguments.Count) continue;
                        Type[] typeArgs = callerSuppliedGenericArguments.ToArray();
                        mi_genericsCasted = mi.MakeGenericMethod(typeArgs);
                    }

                    Expression selfExpression = dynamicObjectExpression;
                    if (!selfExpression.Type.Equals(methodInstanceType))
                        selfExpression = Expression.Convert(selfExpression, methodInstanceType);

                    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(dynamicObjectExpression, dynamicObjectType);

                    // Create arg exps
                    Expression[] argExps = new Expression[args.Length + offset];
                    if (mi.IsStatic) argExps[0] = selfExpression;
                    for (int i = 0; i < args.Length; i++)
                    {
                        // Copy
                        argExps[i + offset] = args[i].Expression;

                        // Test if this argument will be inside varargs
                        bool is_in_varargs = ix_parameter_varargs >= 0 && i >= ix_parameter_varargs;

                        if (!is_in_varargs)
                        {
                            // Convert Type
                            if (args[i].Expression.Type.Equals(pis[i + offset].ParameterType)) argExps[i + offset] = Expression.Convert(args[i].Expression, pis[i + offset].ParameterType);
                            // Add restriction
                            restrictions = restrictions.Merge(BindingRestrictions.GetTypeRestriction(args[i].Expression, pis[i + offset].ParameterType));
                        }
                    }

                    // Support varargs
                    if (ix_parameter_varargs >= 0)
                    {
                        // Split argExps to half
                        Expression[] new_argExps = new Expression[ix_parameter_varargs + offset + 1];
                        Array.Copy(argExps, new_argExps, new_argExps.Length-1);
                        new_argExps[ix_parameter_varargs+offset] = 
                            Expression.NewArrayInit(typeof(object), 
                            argExps
                            .Skip(ix_parameter_varargs + offset)   // Skip arguments before the "param object[] args"
                            .Select(e=>typeof(object).Equals(e.Type) ? e : Expression.Convert(e, typeof(object))) // Cast to object
                            .ToArray() // to object[] required by Expression.NewArrayInit
                            );
                        argExps = new_argExps;
                    }
                    Expression exp =
                        mi.IsStatic ?
                        Expression.Call(mi_genericsCasted, argExps) :
                        Expression.Call(selfExpression, mi_genericsCasted, argExps);
                    //restrictions = restrictions.Merge(BindingRestrictions.GetTypeRestriction(exp, mi.ReturnType));
                    return new DynamicMetaObject(exp, restrictions);
                }
            }

            return null;
        }

        //Expression<Func<Object, IList<Type>>> GetTypeArguments        
        //    = o => typeof(RuntimeBinderException)
        //o is Microsoft.CSharp.RuntimeBinder.CSharpInvokeMemberBinder binder ? binder : null;

        protected static Dictionary<Type, Func<Object, IList<Type>>> getTypeArgumentsFuncDictionary = new Dictionary<Type, Func<Object, IList<Type>>>();
        protected static Func<Object, IList<Type>> GetTypeArgumentsFunc(Type binderType)
        {
            Func<Object, IList<Type>> func = null;
            if (getTypeArgumentsFuncDictionary.TryGetValue(binderType, out func)) return func;
            PropertyInfo pi = binderType.GetProperty("TypeArguments", typeof(IList<Type>));
            MethodInfo mi = pi?.GetGetMethod();
            if (pi != null)
            {
                ParameterExpression param = Expression.Parameter(typeof(object));
                Expression body = Expression.Call(Expression.Convert(param, binderType), mi);
                Expression<Func<object, IList<Type>>> lambda = Expression.Lambda<Func<object, IList<Type>>>(body, param);
                func = lambda.Compile();
            }
            getTypeArgumentsFuncDictionary[binderType] = func;
            return func;
        }
        static IList<Type> GetTypeArguments(object binder)
        {
            var func = GetTypeArgumentsFunc(binder.GetType());
            if (func == null) return null;
            return func(binder);
        }
    }
}
