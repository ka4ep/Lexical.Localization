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
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    /// <summary>
    /// <see cref="DynamicMetaObject" /> implementation for <see cref="IAssetKey" /> interfaces.
    /// </summary>
    public class AssetKeyDynamicMetaObject : DynamicMetaObject
    {
        protected DynamicObjectLibrary library;

        public AssetKeyDynamicMetaObject(DynamicObjectLibrary library, Expression expression, BindingRestrictions restrictions) : base(expression, restrictions)
        {
            this.library = library;
        }
        public AssetKeyDynamicMetaObject(DynamicObjectLibrary library, Expression expression, BindingRestrictions restrictions, object value) : base(expression, restrictions, value)
        {
            this.library = library;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            => library.BindGetMember(binder, LimitType ?? this.Value?.GetType(), Expression) ?? createKeyMember(binder);
        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            => library.BindInvokeMember(binder, args, LimitType ?? this.Value?.GetType(), Expression) ?? base.BindInvokeMember(binder, args);
        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] args)
            => library.BindGetIndex(binder, args, LimitType ?? this.Value?.GetType(), Expression) ?? base.BindGetIndex(binder, args);

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            MethodInfo mi = null;

            // (string) maps to obj.ToString()
            if (typeof(String).IsAssignableFrom(binder.ReturnType))
            {
                mi = (HasValue ? Value.GetType() : LimitType).GetMethods().Where(_ => _.Name == "ToString" && _.GetParameters().Length == 0).FirstOrDefault();
            }

            // (byte[]) maps to AssetKeyExtensions.GetResource(obj)
            if (typeof(byte[]).IsAssignableFrom(binder.ReturnType))
            {
                if (miGetResource == null) miGetResource = typeof(AssetKeyExtensions).GetMethod(nameof(AssetKeyExtensions.GetResource));
                mi = miGetResource;
            }

            if (mi != null)
            {
                if (mi.IsStatic)
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    Type parameterType = pis[0].ParameterType;
                    Expression selfExpression = Expression;
                    if (!parameterType.IsAssignableFrom(selfExpression.Type)) selfExpression = Expression.Convert(selfExpression, parameterType);

                    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                    Expression exp = Expression.Call(mi, selfExpression);
                    return new DynamicMetaObject(exp, restrictions);
                } else
                {
                    Expression selfExpression = Expression;
                    if (!mi.ReflectedType.IsAssignableFrom(selfExpression.Type)) selfExpression = Expression.Convert(selfExpression, mi.ReflectedType);
                    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                    Expression exp = Expression.Call(selfExpression, mi);
                    return new DynamicMetaObject(exp, restrictions);
                }
            }

            return base.BindConvert(binder);
        }
        protected static MethodInfo miBuildName, miGetResource;

        protected virtual DynamicMetaObject createKeyMember(GetMemberBinder binder)
        {
            MethodInfo mi = typeof(AssetKeyExtensions).GetMethod("Key");
            BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
            Expression selfExp = Expression.Convert(Expression, mi.GetParameters()[0].ParameterType);
            Expression keyNameExp = Expression.Constant(binder.Name);
            return new DynamicMetaObject(Expression.Call(mi, selfExp, keyNameExp), restrictions);
        }

    }

}
